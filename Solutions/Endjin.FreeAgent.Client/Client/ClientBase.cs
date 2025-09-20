// <copyright file="ClientBase.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Net.Http.Headers;
using System.Threading;

using Corvus.Retry;
using Corvus.Retry.Policies;
using Corvus.Retry.Strategies;
using Endjin.FreeAgent.Client.OAuth2;
using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Endjin.FreeAgent.Client;

public abstract class ClientBase
{
    internal const string JsonMediaType = "application/json";
    internal readonly Uri ApiBaseUrl = new("https://api.freeagent.com");
    private readonly Lock syncRoot = new();

    // HTTP clients - will be initialized in InitializeAndAuthorizeAsync
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    internal HttpClient HttpClient { get; set; }
    internal HttpClient HttpClientNoAuthHeader { get; set; }
#pragma warning restore CS8618

    // Services
    private IHttpClientFactory? httpClientFactory;
    private IOAuth2Service? oauth2Service;
    private IMemoryCache? memoryCache;
    private ILoggerFactory? loggerFactory;

    internal string? ClientId;
    internal string? ClientSecret;
    internal string? RefreshToken;

    public bool IsInitialized { get; set; }

    /// <summary>
    /// Sets the HttpClientFactory for creating HTTP clients.
    /// </summary>
    internal void SetHttpClientFactory(IHttpClientFactory factory) => this.httpClientFactory = factory;

    /// <summary>
    /// Initializes the HTTP services if they haven't been set.
    /// This ensures backward compatibility with existing code.
    /// </summary>

    public async Task InitializeAndAuthorizeAsync()
    {
        if (!this.IsInitialized)
        {
            // Initialize HTTP clients using factory - required for proper resource management
            lock (syncRoot)
            {
                if (this.httpClientFactory == null)
                {
                    throw new InvalidOperationException(
                        "HttpClientFactory is not configured. Please ensure IHttpClientFactory is registered in the dependency injection container " +
                        "or provide it when creating the FreeAgentClient instance. This is required for proper HTTP connection management.");
                }

                this.HttpClientNoAuthHeader = this.httpClientFactory.CreateClient("FreeAgentNoAuth");
                this.HttpClient = this.httpClientFactory.CreateClient("FreeAgent");

                InitializeHttpClient(this.HttpClientNoAuthHeader);
                InitializeHttpClient(this.HttpClient);
            }

            // Initialize OAuth2Service after HTTP clients are ready
            InitializeOAuth2Service();

            // Use OAuth2Service
            string accessToken = await oauth2Service!.GetAccessTokenAsync().ConfigureAwait(false);

            // Set authorization for HttpClient after async operation completes
            lock (syncRoot)
            {
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                this.IsInitialized = true;
            }
        }
    }

    private static void InitializeHttpClient(HttpClient httpClient)
    {
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "endjin-freeagent-client/1.0");

        if (!httpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue(JsonMediaType)))
        {
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
        }
    }

    /// <summary>
    /// Creates an OAuth2Service instance. Virtual to allow test overrides.
    /// </summary>
    protected virtual IOAuth2Service CreateOAuth2Service(OAuth2Options options, HttpClient httpClient, IMemoryCache cache, ILogger<OAuth2Service> logger) => new OAuth2Service(options, httpClient, cache, logger);

    /// <summary>
    /// Initializes the OAuth2 service if not already initialized.
    /// </summary>
    private void InitializeOAuth2Service()
    {
        if (oauth2Service == null)
        {
            OAuth2Options options = new()
            {
                ClientId = this.ClientId ?? throw new InvalidOperationException("OAuth2 client ID has not been configured."),
                ClientSecret = this.ClientSecret ?? throw new InvalidOperationException("OAuth2 client secret has not been configured."),
                RefreshToken = this.RefreshToken ?? throw new InvalidOperationException("OAuth2 refresh token has not been configured."),
            };

            // Use existing memory cache if available, otherwise create a new one
            memoryCache ??= new MemoryCache(new MemoryCacheOptions());

            // Create logger for OAuth2Service from factory or use NullLogger if factory not available
            ILogger<OAuth2Service> logger = loggerFactory?.CreateLogger<OAuth2Service>() ?? NullLogger<OAuth2Service>.Instance;

            oauth2Service = CreateOAuth2Service(options, HttpClientNoAuthHeader, memoryCache, logger);
        }
    }

    /// <summary>
    /// Sets the memory cache for OAuth2 token caching.
    /// </summary>
    internal void SetMemoryCache(IMemoryCache cache) => this.memoryCache = cache;

    /// <summary>
    /// Sets the logger factory for creating loggers.
    /// </summary>
    internal void SetLoggerFactory(ILoggerFactory factory) => this.loggerFactory = factory;

    /// <summary>
    /// Sets the OAuth2 service for testing purposes.
    /// </summary>
    internal void SetOAuth2Service(IOAuth2Service service) => this.oauth2Service = service;

    public async Task<List<T>> ExecuteRequestAndFollowLinksAsync<T>(Uri uri)
    {
        List<T> results = [];

        if (!this.IsInitialized)
        {
            await this.InitializeAndAuthorizeAsync().ConfigureAwait(false);
        }

        bool haveTriedRefreshingToken = false;
        HttpResponseMessage response = await Retriable.RetryAsync(
            async () =>
            {
                HttpResponseMessage result = await this.HttpClient.GetAsync(uri);

                if (result.StatusCode == HttpStatusCode.Unauthorized && !haveTriedRefreshingToken)
                {
                    // We get a 401 when the access token expires. If that's the first time that's
                    // happened with this request, it might simply be time to refresh the token,
                    // so we should do that and retry the request.
                    InitializeOAuth2Service();

                    string newAccessToken = await oauth2Service!.RefreshAccessTokenAsync().ConfigureAwait(false);

                    lock (syncRoot)
                    {
                        this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                    }

                    haveTriedRefreshingToken = true;

                    result = await this.HttpClient.GetAsync(uri).ConfigureAwait(false);
                }
                result.EnsureSuccessStatusCode();
                return result;
            },
            CancellationToken.None,
            new Backoff(),
            new AnyExceptionPolicy());

        string jsonContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        T content = JsonSerializer.Deserialize<T>(jsonContent, SharedJsonOptions.Instance) ?? throw new InvalidOperationException("Failed to deserialize response content.");

        results.Add(content);

        IEnumerable<Link> links = ExtractLinksFromHeader(response.Headers);
        Link? nextPage = links.FirstOrDefault(x => x.Rel == "next");

        if (nextPage != null)
        {
            List<T> nestedContent = await this.ExecuteRequestAndFollowLinksAsync<T>(nextPage.Uri).ConfigureAwait(false);
            results.AddRange(nestedContent);
        }

        return results;
    }

    public static IEnumerable<Link> ExtractLinksFromHeader(HttpResponseHeaders headers)
    {
        List<Link> result = [];

        if (headers.Contains("Link"))
        {
            headers.TryGetValues("Link", out IEnumerable<string>? header);

            List<string[]>? headerLinks = header?.FirstOrDefault()?.Split(',').Select(x => x.Split(';')).ToList();

            if (headerLinks is { Count: >= 2 })
            {
                foreach (string[] headerLink in headerLinks)
                {
                    string rawLink = headerLink[0].Trim();
                    string rawRel = headerLink[1].Trim();

                    string nextPageUrl = rawLink.TrimStart('<').TrimEnd('>');
                    string rel = rawRel.Replace("rel='", string.Empty).TrimEnd('\'');

                    Link link = new() { Rel = rel, Uri = new Uri(nextPageUrl) };

                    result.Add(link);
                }
            }
        }

        return result;
    }
}
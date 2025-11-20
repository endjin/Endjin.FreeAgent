// <copyright file="ClientBase.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;

using Corvus.Retry;
using Corvus.Retry.Policies;
using Corvus.Retry.Strategies;
using Endjin.FreeAgent.Client.OAuth2;
using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Abstract base class providing core HTTP and OAuth2 functionality for FreeAgent API clients.
/// </summary>
/// <remarks>
/// <para>
/// This class handles the foundational concerns for communicating with the FreeAgent API:
/// <list type="bullet">
/// <item><description>HTTP client initialization and configuration</description></item>
/// <item><description>OAuth2 authentication and token refresh</description></item>
/// <item><description>Request retry logic with exponential backoff</description></item>
/// <item><description>Pagination support via link headers</description></item>
/// <item><description>Thread-safe authorization header management</description></item>
/// </list>
/// </para>
/// <para>
/// The class uses dependency injection for HTTP client factory, memory cache, and logging,
/// ensuring proper resource management and testability. HTTP clients are created through
/// <see cref="IHttpClientFactory"/> to support connection pooling and socket exhaustion prevention.
/// </para>
/// <para>
/// OAuth2 token management is automatic. When a 401 Unauthorized response is received, the class
/// attempts to refresh the access token once before retrying the request. Access tokens are cached
/// to minimize token refresh requests.
/// </para>
/// </remarks>
public abstract class ClientBase
{
    internal const string JsonMediaType = "application/json";
    private readonly Lock syncRoot = new();

    /// <summary>
    /// Gets or sets the base URL for the FreeAgent API.
    /// </summary>
    /// <value>
    /// The API base URL. Defaults to the production URL (https://api.freeagent.com).
    /// Set to the sandbox URL (https://api.sandbox.freeagent.com) for testing.
    /// </value>
    internal Uri ApiBaseUrl { get; set; } = FreeAgentOptions.ProductionApiBaseUrl;

    // HTTP clients - will be initialized in InitializeAndAuthorizeAsync
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
    /// <summary>
    /// Gets or sets the HTTP client configured with OAuth2 authorization headers.
    /// </summary>
    /// <value>The authenticated HTTP client for API requests.</value>
    internal HttpClient HttpClient { get; set; }

    /// <summary>
    /// Gets or sets the HTTP client without authorization headers, used for OAuth2 token operations.
    /// </summary>
    /// <value>The unauthenticated HTTP client for token refresh requests.</value>
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
    internal IRetryStrategy RetryStrategy { get; set; } = new Backoff();
    internal IRetryPolicy RetryPolicy { get; set; } = new AnyExceptionPolicy();

    /// <summary>
    /// Gets or sets a value indicating whether the client has been initialized with HTTP clients and OAuth2 services.
    /// </summary>
    /// <value><c>true</c> if the client is initialized and ready to make API calls; otherwise, <c>false</c>.</value>
    [MemberNotNullWhen(true, nameof(HttpClient), nameof(HttpClientNoAuthHeader), nameof(oauth2Service))]
    public bool IsInitialized { get; set; }

    /// <summary>
    /// Sets the HTTP client factory for creating managed HTTP client instances.
    /// </summary>
    /// <param name="factory">The HTTP client factory to use for creating HTTP clients.</param>
    /// <remarks>
    /// This method is used internally to inject the HTTP client factory dependency.
    /// The factory is required to create both authenticated and unauthenticated HTTP clients.
    /// </remarks>
    internal void SetHttpClientFactory(IHttpClientFactory factory) => this.httpClientFactory = factory;

    /// <summary>
    /// Initializes the HTTP clients and obtains an OAuth2 access token for API authorization.
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the HTTP client factory is not configured.</exception>
    /// <remarks>
    /// <para>
    /// This method is called automatically before the first API request and is thread-safe.
    /// It performs the following operations:
    /// <list type="number">
    /// <item><description>Creates HTTP clients through the factory (authenticated and unauthenticated)</description></item>
    /// <item><description>Configures HTTP client default headers (User-Agent, Accept)</description></item>
    /// <item><description>Initializes the OAuth2 service</description></item>
    /// <item><description>Obtains an access token (either from cache or by refreshing)</description></item>
    /// <item><description>Sets the Authorization header with the bearer token</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Subsequent calls to this method after initialization complete have no effect.
    /// The initialization state is tracked by the <see cref="IsInitialized"/> property.
    /// </para>
    /// </remarks>
    [MemberNotNull(nameof(HttpClient), nameof(HttpClientNoAuthHeader), nameof(oauth2Service))]
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
            string accessToken = await oauth2Service.GetAccessTokenAsync().ConfigureAwait(false);

            // Set authorization for HttpClient after async operation completes
            lock (syncRoot)
            {
                this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                this.IsInitialized = true;
            }
        }
    }

    /// <summary>
    /// Initializes the default request headers for an HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client to configure.</param>
    /// <remarks>
    /// This method sets the User-Agent header to identify the client library
    /// and configures the Accept header to request JSON responses from the API.
    /// </remarks>
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
    /// Creates an OAuth2 service instance for token management.
    /// </summary>
    /// <param name="options">The OAuth2 configuration options.</param>
    /// <param name="httpClient">The HTTP client for token requests (without auth headers).</param>
    /// <param name="cache">The memory cache for storing tokens.</param>
    /// <param name="logger">The logger for OAuth2 operations.</param>
    /// <returns>A new instance of <see cref="IOAuth2Service"/>.</returns>
    /// <remarks>
    /// This method is virtual to allow test implementations to override the OAuth2 service
    /// creation with mock implementations.
    /// </remarks>
    protected virtual IOAuth2Service CreateOAuth2Service(OAuth2Options options, HttpClient httpClient, IMemoryCache cache, ILogger<OAuth2Service> logger) => new OAuth2Service(options, httpClient, cache, logger);

    /// <summary>
    /// Initializes the OAuth2 service with client credentials if not already initialized.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when OAuth2 credentials are not configured.</exception>
    /// <remarks>
    /// This method creates an <see cref="IOAuth2Service"/> instance using the configured
    /// client ID, client secret, and refresh token. It also initializes the memory cache
    /// and logger if they haven't been set through dependency injection.
    /// </remarks>
    [MemberNotNull(nameof(oauth2Service), nameof(memoryCache))]
    private void InitializeOAuth2Service()
    {
        // Use existing memory cache if available, otherwise create a new one
        memoryCache ??= new MemoryCache(new MemoryCacheOptions());

        if (oauth2Service == null)
        {
            OAuth2Options options = new()
            {
                ClientId = this.ClientId ?? throw new InvalidOperationException("OAuth2 client ID has not been configured."),
                ClientSecret = this.ClientSecret ?? throw new InvalidOperationException("OAuth2 client secret has not been configured."),
                RefreshToken = this.RefreshToken ?? throw new InvalidOperationException("OAuth2 refresh token has not been configured."),
                TokenEndpoint = new Uri(this.ApiBaseUrl, "/v2/token_endpoint"),
                AuthorizationEndpoint = new Uri(this.ApiBaseUrl, "/v2/approve_app"),
            };

            // Create logger for OAuth2Service from factory or use NullLogger if factory not available
            ILogger<OAuth2Service> logger = loggerFactory?.CreateLogger<OAuth2Service>() ?? NullLogger<OAuth2Service>.Instance;

            oauth2Service = CreateOAuth2Service(options, HttpClientNoAuthHeader, memoryCache, logger);
        }
    }

    /// <summary>
    /// Sets the memory cache for OAuth2 token storage.
    /// </summary>
    /// <param name="cache">The memory cache instance to use for caching tokens.</param>
    /// <remarks>
    /// This method is used internally to inject the memory cache dependency used
    /// for storing and retrieving OAuth2 access tokens.
    /// </remarks>
    internal void SetMemoryCache(IMemoryCache cache) => this.memoryCache = cache;

    /// <summary>
    /// Sets the logger factory for creating logger instances.
    /// </summary>
    /// <param name="factory">The logger factory to use for creating loggers.</param>
    /// <remarks>
    /// This method is used internally to inject the logger factory dependency.
    /// </remarks>
    internal void SetLoggerFactory(ILoggerFactory factory) => this.loggerFactory = factory;

    /// <summary>
    /// Sets the OAuth2 service instance for token management.
    /// </summary>
    /// <param name="service">The OAuth2 service to use for token operations.</param>
    /// <remarks>
    /// This method is used internally for testing purposes to inject a mock OAuth2 service.
    /// </remarks>
    internal void SetOAuth2Service(IOAuth2Service service) => this.oauth2Service = service;

    /// <summary>
    /// Executes an HTTP GET request and automatically follows pagination links to retrieve all results as an asynchronous stream.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    /// <param name="uri">The initial URI to request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>An asynchronous stream of response objects from the current page and all subsequent pages.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails after retries.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response cannot be deserialized.</exception>
    public async IAsyncEnumerable<T> ExecuteRequestAndFollowLinksAsyncEnumerable<T>(Uri uri, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!this.IsInitialized)
        {
            await this.InitializeAndAuthorizeAsync().ConfigureAwait(false);
        }

        Uri? currentUri = uri;

        while (currentUri != null)
        {
            bool haveTriedRefreshingToken = false;
            HttpResponseMessage response = await Retriable.RetryAsync(
                async () =>
                {
                    HttpResponseMessage result = await this.HttpClient.GetAsync(currentUri, cancellationToken).ConfigureAwait(false);

                    if (result.StatusCode == HttpStatusCode.Unauthorized && !haveTriedRefreshingToken)
                    {
                        InitializeOAuth2Service();

                        string newAccessToken = await oauth2Service.RefreshAccessTokenAsync().ConfigureAwait(false);

                        lock (syncRoot)
                        {
                            this.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
                        }

                        haveTriedRefreshingToken = true;

                        result = await this.HttpClient.GetAsync(currentUri, cancellationToken).ConfigureAwait(false);
                    }
                    result.EnsureSuccessStatusCode();
                    return result;
                },
                cancellationToken,
                this.RetryStrategy,
                this.RetryPolicy).ConfigureAwait(false);

            string jsonContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            T content = JsonSerializer.Deserialize<T>(jsonContent, SharedJsonOptions.Instance) ?? throw new InvalidOperationException("Failed to deserialize response content.");

            yield return content;

            IEnumerable<Link> links = ExtractLinksFromHeader(response.Headers);
            Link? nextPage = links.FirstOrDefault(x => x.Rel == "next");
            currentUri = nextPage?.Uri;
        }
    }

    /// <summary>
    /// Executes an HTTP GET request and automatically follows pagination links to retrieve all results.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    /// <param name="uri">The initial URI to request.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of all response objects from the current page and all subsequent pages.</returns>
    /// <exception cref="HttpRequestException">Thrown when the HTTP request fails after retries.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response cannot be deserialized.</exception>
    /// <remarks>
    /// <para>
    /// This method handles FreeAgent's pagination by:
    /// <list type="number">
    /// <item><description>Making the initial request to the provided URI</description></item>
    /// <item><description>Parsing the Link header to find the "next" page URI</description></item>
    /// <item><description>Recursively fetching all subsequent pages</description></item>
    /// <item><description>Combining all results into a single list</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The method automatically handles OAuth2 token refresh if a 401 Unauthorized response
    /// is received. It uses exponential backoff retry logic with the Corvus.Retry library
    /// to handle transient failures.
    /// </para>
    /// </remarks>
    /// <seealso cref="ExtractLinksFromHeader"/>
    public async Task<List<T>> ExecuteRequestAndFollowLinksAsync<T>(Uri uri)
    {
        List<T> results = [];
        await foreach (T item in ExecuteRequestAndFollowLinksAsyncEnumerable<T>(uri).ConfigureAwait(false))
        {
            results.Add(item);
        }
        return results;
    }

    /// <summary>
    /// Extracts pagination link information from HTTP Link headers.
    /// </summary>
    /// <param name="headers">The HTTP response headers containing Link header information.</param>
    /// <returns>A collection of <see cref="Link"/> objects representing pagination relationships.</returns>
    /// <remarks>
    /// <para>
    /// The FreeAgent API uses Link headers (RFC 5988) to communicate pagination information.
    /// A typical Link header looks like:
    /// <code>
    /// Link: &lt;https://api.freeagent.com/v2/contacts?page=2&gt;; rel='next',
    ///       &lt;https://api.freeagent.com/v2/contacts?page=5&gt;; rel='last'
    /// </code>
    /// </para>
    /// <para>
    /// This method parses the header and extracts each link with its relationship type (rel).
    /// Common relationship types include:
    /// <list type="bullet">
    /// <item><description>next - The next page in the result set</description></item>
    /// <item><description>prev - The previous page in the result set</description></item>
    /// <item><description>first - The first page in the result set</description></item>
    /// <item><description>last - The last page in the result set</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public static IEnumerable<Link> ExtractLinksFromHeader(HttpResponseHeaders headers)
    {
        List<Link> result = [];

        if (headers.Contains("Link"))
        {
            headers.TryGetValues("Link", out IEnumerable<string>? header);

            List<string[]>? headerLinks = header?.FirstOrDefault()?.Split(',').Select(x => x.Split(';')).ToList();

            if (headerLinks is { Count: > 0 })
            {
                foreach (string[] headerLink in headerLinks)
                {
                    if (headerLink.Length < 2)
                    {
                        continue;
                    }

                    string rawLink = headerLink[0].Trim();
                    string rawRel = headerLink[1].Trim();

                    string nextPageUrl = rawLink.TrimStart('<').TrimEnd('>');
                    string rel = rawRel.Replace("rel=", string.Empty).Trim('\'', '"');

                    Link link = new() { Rel = rel, Uri = new Uri(nextPageUrl) };

                    result.Add(link);
                }
            }
        }

        return result;
    }
}
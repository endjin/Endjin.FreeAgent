// <copyright file="OAuth2Service.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Duende.IdentityModel.Client;

using Microsoft.Extensions.Logging;

using System.Threading;

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Service for handling OAuth2 authentication using Duende.IdentityModel.
/// </summary>
/// <remarks>
/// <para>
/// This service provides thread-safe OAuth2 token management with automatic token refresh
/// and caching capabilities. It uses a semaphore to ensure thread-safe token refresh operations
/// and prevents multiple concurrent refresh requests.
/// </para>
/// <para>
/// Token caching is implemented using <see cref="IMemoryCache"/> to minimize unnecessary
/// token refresh requests. Tokens are cached until they expire, with a configurable buffer
/// period (see <see cref="OAuth2Options.TokenRefreshBufferSeconds"/>) to refresh tokens
/// before they actually expire.
/// </para>
/// <para>
/// The service automatically refreshes access tokens when they are close to expiration,
/// and updates the refresh token if a new one is provided by the authorization server.
/// </para>
/// </remarks>
public class OAuth2Service : IOAuth2Service
{
    private OAuth2Options options;
    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;
    private readonly ILogger<OAuth2Service> logger;
    private readonly SemaphoreSlim tokenRefreshSemaphore = new(1, 1);
    private const string TokenCacheKey = "FreeAgent:OAuth2:Token";

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuth2Service"/> class.
    /// </summary>
    /// <param name="options">The OAuth2 configuration options.</param>
    /// <param name="httpClient">The HTTP client for making token requests.</param>
    /// <param name="cache">The memory cache for storing tokens.</param>
    /// <param name="logger">The logger for diagnostic information.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="options"/>, <paramref name="httpClient"/>, or <paramref name="cache"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the provided <paramref name="options"/> fail validation (see <see cref="OAuth2Options.Validate"/>).
    /// </exception>
    public OAuth2Service(
        OAuth2Options options,
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<OAuth2Service> logger)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.logger = logger;

        options.Validate();
    }

    /// <summary>
    /// Gets a valid access token, refreshing if necessary.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a valid access token.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when token refresh fails or when RefreshToken is not configured.
    /// </exception>
    /// <remarks>
    /// This method first checks the cache for a valid token. If the cached token is still valid
    /// (considering the <see cref="OAuth2Options.TokenRefreshBufferSeconds"/> buffer), it returns
    /// the cached token. Otherwise, it refreshes the token using <see cref="RefreshAccessTokenAsync"/>.
    /// </remarks>
    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        // Try to get token from cache
        if (cache.TryGetValue<TokenInfo>(TokenCacheKey, out TokenInfo? cachedToken) && cachedToken != null)
        {
            // Check if token is still valid (with buffer)
            if (cachedToken.ExpiresAt > DateTime.UtcNow.AddSeconds(options.TokenRefreshBufferSeconds))
            {
                logger.LogDebug("Using cached access token");

                return cachedToken.AccessToken;
            }
        }

        // Token needs refresh
        return await RefreshAccessTokenAsync(cancellationToken);
    }

    /// <summary>
    /// Refreshes the access token using the refresh token.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new access token.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the refresh token is not configured or when the token refresh request fails.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method uses a semaphore to ensure thread-safe token refresh. Only one refresh
    /// operation will be in progress at a time. If multiple threads request a refresh simultaneously,
    /// only the first thread will perform the refresh while others wait, and all will receive
    /// the refreshed token.
    /// </para>
    /// <para>
    /// The method implements a double-check pattern: after acquiring the semaphore, it checks
    /// again if the token is still expired in case another thread has already refreshed it.
    /// </para>
    /// <para>
    /// If a new refresh token is provided in the response, it updates the internal options
    /// to use the new refresh token for subsequent refresh operations.
    /// </para>
    /// </remarks>
    public async Task<string> RefreshAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        await tokenRefreshSemaphore.WaitAsync(cancellationToken);
        try
        {
            // Double-check after acquiring lock
            if (cache.TryGetValue<TokenInfo>(TokenCacheKey, out TokenInfo? cachedToken) && cachedToken != null)
            {
                if (cachedToken.ExpiresAt > DateTime.UtcNow.AddSeconds(options.TokenRefreshBufferSeconds))
                {
                    return cachedToken.AccessToken;
                }
            }

            logger.LogInformation("Refreshing access token");

            // Use Duende.IdentityModel's TokenClient for the refresh
            TokenClient tokenClient = new(httpClient, new TokenClientOptions
            {
                Address = options.TokenEndpoint.ToString(),
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
            });

            TokenResponse tokenResponse = await tokenClient.RequestRefreshTokenAsync(
                options.RefreshToken ?? throw new InvalidOperationException("RefreshToken is required for token refresh"),
                cancellationToken: cancellationToken);

            if (tokenResponse.IsError)
            {
                this.logger.LogError("Token refresh failed: {Error} - {ErrorDescription}", tokenResponse.Error, tokenResponse.ErrorDescription);
                throw new InvalidOperationException($"Failed to refresh token: {tokenResponse.Error} - {tokenResponse.ErrorDescription}");
            }

            // Update the refresh token if a new one was provided
            // Create a new options instance with the updated token to maintain immutability
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                options = options with { RefreshToken = tokenResponse.RefreshToken };
            }

            // Calculate expiration time
            DateTime expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : 3600);

            // Cache the new token
            TokenInfo tokenInfo = new()
            {
                AccessToken = tokenResponse.AccessToken ?? throw new InvalidOperationException("No access token in response"),
                RefreshToken = tokenResponse.RefreshToken ?? options.RefreshToken,
                ExpiresAt = expiresAt,
                TokenType = tokenResponse.TokenType ?? "Bearer"
            };

            MemoryCacheEntryOptions cacheOptions = new()
            {
                AbsoluteExpiration = expiresAt
            };

            cache.Set(TokenCacheKey, tokenInfo, cacheOptions);

            this.logger.LogInformation("Access token refreshed successfully, expires at {ExpiresAt}", expiresAt);

            return tokenInfo.AccessToken;
        }
        finally
        {
            tokenRefreshSemaphore.Release();
        }
    }

    /// <summary>
    /// Exchanges an authorization code for access and refresh tokens.
    /// </summary>
    /// <param name="code">The authorization code received from the authorization endpoint.</param>
    /// <param name="codeVerifier">
    /// The PKCE code verifier that corresponds to the code challenge sent during authorization.
    /// Required when PKCE is enabled in <see cref="OAuth2Options.UsePkce"/>.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the
    /// <see cref="TokenResponse"/> with access token, refresh token, and other token information.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the authorization code exchange fails or when the response does not contain an access token.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method is typically called after the user completes the authorization flow and
    /// is redirected back to your application with an authorization code.
    /// </para>
    /// <para>
    /// The method automatically caches the received tokens and updates the internal refresh token
    /// for subsequent token refresh operations.
    /// </para>
    /// </remarks>
    public async Task<TokenResponse> ExchangeAuthorizationCodeAsync(
        string code,
        string? codeVerifier = null,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Exchanging authorization code for tokens");

        TokenClient tokenClient = new(httpClient, new TokenClientOptions
        {
            Address = options.TokenEndpoint.ToString(),
            ClientId = options.ClientId,
            ClientSecret = options.ClientSecret,
        });

        Dictionary<string, string> parameters = [];

        if (!string.IsNullOrEmpty(codeVerifier))
        {
            parameters["code_verifier"] = codeVerifier;
        }

        if (!string.IsNullOrEmpty(options.RedirectUri))
        {
            parameters["redirect_uri"] = options.RedirectUri;
        }

        TokenResponse tokenResponse = await tokenClient.RequestAuthorizationCodeTokenAsync(
            code,
            options.RedirectUri ?? string.Empty,
            codeVerifier: codeVerifier,
            cancellationToken: cancellationToken);

        if (tokenResponse.IsError)
        {
            this.logger.LogError("Authorization code exchange failed: {Error} - {ErrorDescription}", tokenResponse.Error, tokenResponse.ErrorDescription);
            throw new InvalidOperationException($"Failed to exchange authorization code: {tokenResponse.Error} - {tokenResponse.ErrorDescription}");
        }

        // Store the refresh token
        // Create a new options instance with the updated token to maintain immutability
        if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
        {
            options = options with { RefreshToken = tokenResponse.RefreshToken };
        }

        // Cache the token
        DateTime expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : 3600);

        TokenInfo tokenInfo = new()
        {
            AccessToken = tokenResponse.AccessToken ?? throw new InvalidOperationException("No access token in response"),
            RefreshToken = tokenResponse.RefreshToken ?? string.Empty,
            ExpiresAt = expiresAt,
            TokenType = tokenResponse.TokenType ?? "Bearer"
        };

        MemoryCacheEntryOptions cacheOptions = new()
        {
            AbsoluteExpiration = expiresAt
        };
        cache.Set(TokenCacheKey, tokenInfo, cacheOptions);

        logger.LogInformation("Authorization code exchanged successfully");

        return tokenResponse;
    }

    /// <summary>
    /// Clears the cached token.
    /// </summary>
    /// <remarks>
    /// Use this method to force a token refresh on the next request, or when switching
    /// between different user contexts. The next call to <see cref="GetAccessTokenAsync"/>
    /// will refresh the token from the authorization server.
    /// </remarks>
    public void ClearTokenCache()
    {
        cache.Remove(TokenCacheKey);
        logger.LogDebug("Token cache cleared");
    }

    /// <summary>
    /// Internal class for caching token information.
    /// </summary>
    private class TokenInfo
    {
        public required string AccessToken { get; init; }
        public required string? RefreshToken { get; init; }
        public required DateTime ExpiresAt { get; init; }
        public required string TokenType { get; init; }
    }
}
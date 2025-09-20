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
public class OAuth2Service : IOAuth2Service
{
    private OAuth2Options options;
    private readonly HttpClient httpClient;
    private readonly IMemoryCache cache;
    private readonly ILogger<OAuth2Service> logger;
    private readonly SemaphoreSlim tokenRefreshSemaphore = new(1, 1);
    private const string TokenCacheKey = "FreeAgent:OAuth2:Token";

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

// <copyright file="IOAuth2Service.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Duende.IdentityModel.Client;

using System.Threading;

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Interface for OAuth2 service operations.
/// </summary>
/// <remarks>
/// This interface defines the contract for OAuth2 token management operations including
/// token retrieval, refresh, authorization code exchange, and cache management.
/// Implementations should be thread-safe and support automatic token refresh.
/// </remarks>
public interface IOAuth2Service
{
    /// <summary>
    /// Gets a valid access token, refreshing if necessary.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a valid access token.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when token refresh fails or when RefreshToken is not configured.
    /// </exception>
    /// <remarks>
    /// This method should first check for a cached valid token and return it if available.
    /// If no valid token is cached or the cached token is expired, it should automatically
    /// refresh the token before returning.
    /// </remarks>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the access token using the refresh token.
    /// </summary>
    /// <param name="refreshToken">Optional refresh token to use. If null, uses the configured refresh token.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new access token.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the refresh token is not configured or when the token refresh request fails.
    /// </exception>
    /// <remarks>
    /// This method forces a token refresh regardless of the current cached token state.
    /// Implementations should ensure thread-safe token refresh to prevent multiple
    /// concurrent refresh operations.
    /// </remarks>
    Task<string> RefreshAccessTokenAsync(string? refreshToken = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exchanges an authorization code for access and refresh tokens.
    /// </summary>
    /// <param name="code">The authorization code received from the authorization endpoint.</param>
    /// <param name="codeVerifier">
    /// The PKCE code verifier that corresponds to the code challenge sent during authorization.
    /// Required when PKCE is enabled.
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
    /// This method is used in the OAuth2 authorization code flow, typically called after
    /// the user completes authorization and is redirected back with an authorization code.
    /// Implementations should cache the received tokens for future use.
    /// </remarks>
    Task<TokenResponse> ExchangeAuthorizationCodeAsync(
        string code,
        string? codeVerifier = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the cached token.
    /// </summary>
    /// <remarks>
    /// Use this method to force a token refresh on the next request, or when switching
    /// between different user contexts. The next call to <see cref="GetAccessTokenAsync"/>
    /// should refresh the token from the authorization server.
    /// </remarks>
    void ClearTokenCache();
}
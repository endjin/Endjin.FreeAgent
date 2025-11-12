// <copyright file="IOAuth2Service.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Duende.IdentityModel.Client;

using System.Threading;

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Interface for OAuth2 service operations.
/// </summary>
public interface IOAuth2Service
{
    /// <summary>
    /// Gets a valid access token, refreshing if necessary.
    /// </summary>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the access token using the refresh token.
    /// </summary>
    Task<string> RefreshAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Exchanges an authorization code for access and refresh tokens.
    /// </summary>
    Task<TokenResponse> ExchangeAuthorizationCodeAsync(
        string code,
        string? codeVerifier = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the cached token.
    /// </summary>
    void ClearTokenCache();
}
// <copyright file="OAuth2Options.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Configuration options for OAuth2 authentication.
/// </summary>
public record OAuth2Options
{
    /// <summary>
    /// Gets or sets the OAuth2 token endpoint URL.
    /// </summary>
    public Uri TokenEndpoint { get; init; } = new("https://api.freeagent.com/v2/token_endpoint");

    /// <summary>
    /// Gets or sets the OAuth2 authorization endpoint URL.
    /// </summary>
    public Uri AuthorizationEndpoint { get; init; } = new("https://api.freeagent.com/v2/approve_app");

    /// <summary>
    /// Gets or sets the client ID for OAuth2 authentication.
    /// </summary>
    public required string ClientId { get; init; }

    /// <summary>
    /// Gets or sets the client secret for OAuth2 authentication.
    /// </summary>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// Gets or sets the refresh token for OAuth2 authentication.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Gets or sets the redirect URI for OAuth2 authentication flow.
    /// </summary>
    public string? RedirectUri { get; init; }

    /// <summary>
    /// Gets or sets the scope for OAuth2 authentication.
    /// </summary>
    public string Scope { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to use PKCE (Proof Key for Code Exchange) for enhanced security.
    /// </summary>
    public bool UsePkce { get; init; } = true;

    /// <summary>
    /// Gets or sets the number of seconds before token expiry to trigger a refresh.
    /// Default is 60 seconds.
    /// </summary>
    public int TokenRefreshBufferSeconds { get; init; } = 60;

    /// <summary>
    /// Validates the OAuth2 options.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ClientId))
        {
            throw new InvalidOperationException("ClientId is required for OAuth2 authentication");
        }

        if (string.IsNullOrWhiteSpace(ClientSecret))
        {
            throw new InvalidOperationException("ClientSecret is required for OAuth2 authentication");
        }

        if (TokenEndpoint == null)
        {
            throw new InvalidOperationException("TokenEndpoint is required for OAuth2 authentication");
        }
    }
}

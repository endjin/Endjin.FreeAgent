// <copyright file="OAuth2Options.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Configuration options for OAuth2 authentication with the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// This class contains all settings required for OAuth2 authentication flow with FreeAgent.
/// It supports both authorization code flow (for web applications) and refresh token flow
/// (for server-to-server integrations).
/// </para>
/// <para>
/// PKCE (Proof Key for Code Exchange) is enabled by default for enhanced security, following
/// OAuth2 best practices for public clients.
/// </para>
/// </remarks>
/// <seealso cref="OAuth2Service"/>
/// <seealso cref="IOAuth2Service"/>
public record OAuth2Options
{
    /// <summary>
    /// Gets or sets the OAuth2 token endpoint URL.
    /// </summary>
    /// <value>
    /// The FreeAgent token endpoint URL. Defaults to "https://api.freeagent.com/v2/token_endpoint".
    /// </value>
    public Uri TokenEndpoint { get; init; } = new("https://api.freeagent.com/v2/token_endpoint");

    /// <summary>
    /// Gets or sets the OAuth2 authorization endpoint URL.
    /// </summary>
    /// <value>
    /// The FreeAgent authorization endpoint URL. Defaults to "https://api.freeagent.com/v2/approve_app".
    /// </value>
    public Uri AuthorizationEndpoint { get; init; } = new("https://api.freeagent.com/v2/approve_app");

    /// <summary>
    /// Gets or sets the client ID for OAuth2 authentication.
    /// </summary>
    /// <value>
    /// The OAuth2 client identifier obtained from FreeAgent when registering your application.
    /// This is a required property.
    /// </value>
    public required string ClientId { get; init; }

    /// <summary>
    /// Gets or sets the client secret for OAuth2 authentication.
    /// </summary>
    /// <value>
    /// The OAuth2 client secret obtained from FreeAgent when registering your application.
    /// This is a required property and should be stored securely.
    /// </value>
    public required string ClientSecret { get; init; }

    /// <summary>
    /// Gets or sets the refresh token for OAuth2 authentication.
    /// </summary>
    /// <value>
    /// The refresh token used to obtain new access tokens without re-authorization.
    /// This is optional and typically obtained after the initial authorization code exchange.
    /// </value>
    public string? RefreshToken { get; init; }

    /// <summary>
    /// Gets or sets the redirect URI for OAuth2 authentication flow.
    /// </summary>
    /// <value>
    /// The URI where users will be redirected after authorization.
    /// Must match the redirect URI registered with your FreeAgent application.
    /// This is optional but required for authorization code flow.
    /// </value>
    public string? RedirectUri { get; init; }

    /// <summary>
    /// Gets or sets the scope for OAuth2 authentication.
    /// </summary>
    /// <value>
    /// The OAuth2 scope specifying the permissions requested for the access token.
    /// Defaults to an empty string. Consult FreeAgent API documentation for available scopes.
    /// </value>
    public string Scope { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to use PKCE (Proof Key for Code Exchange) for enhanced security.
    /// </summary>
    /// <value>
    /// <c>true</c> to use PKCE for authorization code flow; otherwise, <c>false</c>.
    /// Defaults to <c>true</c>. PKCE is recommended for all OAuth2 clients, especially public clients.
    /// </value>
    public bool UsePkce { get; init; } = true;

    /// <summary>
    /// Gets or sets the number of seconds before token expiry to trigger a refresh.
    /// </summary>
    /// <value>
    /// The buffer time in seconds before token expiration to proactively refresh the token.
    /// Defaults to 60 seconds. This helps prevent token expiration during API operations.
    /// </value>
    public int TokenRefreshBufferSeconds { get; init; } = 60;

    /// <summary>
    /// Validates the OAuth2 options.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required configuration values are missing or invalid:
    /// <list type="bullet">
    /// <item><description>ClientId is null or whitespace</description></item>
    /// <item><description>ClientSecret is null or whitespace</description></item>
    /// <item><description>TokenEndpoint is null</description></item>
    /// </list>
    /// </exception>
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

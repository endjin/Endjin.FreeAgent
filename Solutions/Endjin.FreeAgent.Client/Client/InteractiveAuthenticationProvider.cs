// <copyright file="InteractiveAuthenticationProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading;

using Endjin.FreeAgent.Client.OAuth2;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Authentication provider that supports interactive login and token refresh.
/// </summary>
public class InteractiveAuthenticationProvider : IAuthenticationProvider
{
    private readonly InteractiveLoginHelper loginHelper;
    private readonly IOAuth2Service oauth2Service;
    private readonly ILogger logger;
    private string? refreshToken;
    private string? accessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractiveAuthenticationProvider"/> class.
    /// </summary>
    /// <param name="loginHelper">The interactive login helper.</param>
    /// <param name="oauth2Service">The OAuth2 service for token operations.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="initialRefreshToken">The initial refresh token, if available.</param>
    public InteractiveAuthenticationProvider(
        InteractiveLoginHelper loginHelper,
        IOAuth2Service oauth2Service,
        ILogger<InteractiveAuthenticationProvider> logger,
        string? initialRefreshToken = null)
    {
        this.loginHelper = loginHelper ?? throw new ArgumentNullException(nameof(loginHelper));
        this.oauth2Service = oauth2Service ?? throw new ArgumentNullException(nameof(oauth2Service));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.refreshToken = initialRefreshToken;
    }

    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        // If we have a valid access token and not forcing refresh, return it
        // Note: We don't check expiration here as we don't track it closely, relying on 401s to trigger refresh
        // But if we just logged in, we have a fresh token.
        if (!forceRefresh && !string.IsNullOrEmpty(this.accessToken))
        {
            return this.accessToken;
        }

        // Try to refresh using existing refresh token
        if (!string.IsNullOrEmpty(this.refreshToken))
        {
            try
            {
                this.logger.LogDebug("Attempting to refresh token using existing refresh token.");
                string newAccessToken = await this.oauth2Service.RefreshAccessTokenAsync(this.refreshToken, cancellationToken).ConfigureAwait(false);
                this.accessToken = newAccessToken;
                return newAccessToken;
            }
            catch (Exception ex)
            {
                this.logger.LogWarning(ex, "Failed to refresh token using existing refresh token. Falling back to interactive login.");
            }
        }

        // Interactive login
        this.logger.LogInformation("Starting interactive login...");
        InteractiveLoginResult result = await this.loginHelper.LoginAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        
        this.refreshToken = result.RefreshToken;
        this.accessToken = result.AccessToken;
        
        this.logger.LogInformation("Interactive login successful.");
        
        return result.AccessToken;
    }
}

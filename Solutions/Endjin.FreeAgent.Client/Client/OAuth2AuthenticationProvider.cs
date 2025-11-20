// <copyright file="OAuth2AuthenticationProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading;

using Endjin.FreeAgent.Client.OAuth2;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Authentication provider using OAuth2 service.
/// </summary>
public class OAuth2AuthenticationProvider : IAuthenticationProvider
{
    private readonly IOAuth2Service oauth2Service;

    /// <summary>
    /// Initializes a new instance of the <see cref="OAuth2AuthenticationProvider"/> class.
    /// </summary>
    /// <param name="oauth2Service">The OAuth2 service to use.</param>
    public OAuth2AuthenticationProvider(IOAuth2Service oauth2Service)
    {
        this.oauth2Service = oauth2Service ?? throw new ArgumentNullException(nameof(oauth2Service));
    }

    /// <inheritdoc/>
    public async Task<string> GetAccessTokenAsync(bool forceRefresh = false, CancellationToken cancellationToken = default)
    {
        if (forceRefresh)
        {
            return await this.oauth2Service.RefreshAccessTokenAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return await this.oauth2Service.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
    }
}

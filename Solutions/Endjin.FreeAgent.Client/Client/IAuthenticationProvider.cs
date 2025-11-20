// <copyright file="IAuthenticationProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Threading;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Defines a contract for providing authentication tokens for API requests.
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// Gets a valid access token for API authorization.
    /// </summary>
    /// <param name="forceRefresh">Whether to force a token refresh regardless of cache state.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the access token.</returns>
    /// <remarks>
    /// Implementations should handle token caching and refreshing automatically.
    /// </remarks>
    Task<string> GetAccessTokenAsync(bool forceRefresh = false, CancellationToken cancellationToken = default);
}

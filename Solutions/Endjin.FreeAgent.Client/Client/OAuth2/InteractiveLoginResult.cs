// <copyright file="InteractiveLoginResult.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Result of an interactive login operation.
/// </summary>
public class InteractiveLoginResult
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Gets or sets when the access token expires (UTC).
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Gets or sets the number of seconds until the token expires.
    /// </summary>
    public required int ExpiresInSeconds { get; init; }

    /// <summary>
    /// Gets or sets the token type (typically "Bearer").
    /// </summary>
    public required string TokenType { get; init; }
}

// <copyright file="TestOAuth2Service.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Duende.IdentityModel.Client;
using Endjin.FreeAgent.Client.OAuth2;

namespace Endjin.FreeAgent.Client.Tests.TestHelpers;

/// <summary>
/// Test implementation of IOAuth2Service for unit testing.
/// </summary>
public class TestOAuth2Service : IOAuth2Service
{
    private readonly string fixedToken;

    public TestOAuth2Service(string fixedToken = "test-token")
    {
        this.fixedToken = fixedToken;
    }

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(fixedToken);
    }

    public Task<string> RefreshAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(fixedToken);
    }

    public Task<TokenResponse> ExchangeAuthorizationCodeAsync(
        string code,
        string? codeVerifier = null,
        CancellationToken cancellationToken = default)
    {
        // TokenResponse is from IdentityModel and requires HTTP response
        // For testing, we return a simple successful response
        TokenResponse response = new();
        // The properties are read-only, so we can't set them directly
        // The response will indicate success with IsError = false
        return Task.FromResult(response);
    }

    public void ClearTokenCache()
    {
        // No-op for tests
    }
}

// <copyright file="OAuth2OptionsBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client.Tests.Builders;

using Endjin.FreeAgent.Client.OAuth2;

public class OAuth2OptionsBuilder
{
    private string clientId = "test_client_id";
    private string clientSecret = "test_client_secret";
    private string? refreshToken = "test_refresh_token";
    private string? redirectUri = null;
    private string scope = string.Empty;
    private bool usePkce = true;
    private Uri tokenEndpoint = new("https://api.freeagent.com/v2/token_endpoint");
    private Uri authorizationEndpoint = new("https://api.freeagent.com/v2/approve_app");

    public OAuth2OptionsBuilder WithClientId(string clientId)
    {
        this.clientId = clientId;
        return this;
    }

    public OAuth2OptionsBuilder WithClientSecret(string clientSecret)
    {
        this.clientSecret = clientSecret;
        return this;
    }

    public OAuth2OptionsBuilder WithRefreshToken(string? refreshToken)
    {
        this.refreshToken = refreshToken;
        return this;
    }

    public OAuth2OptionsBuilder WithRedirectUri(string? redirectUri)
    {
        this.redirectUri = redirectUri;
        return this;
    }

    public OAuth2OptionsBuilder WithScope(string scope)
    {
        this.scope = scope;
        return this;
    }

    public OAuth2OptionsBuilder WithUsePkce(bool usePkce)
    {
        this.usePkce = usePkce;
        return this;
    }

    public OAuth2OptionsBuilder WithTokenEndpoint(Uri tokenEndpoint)
    {
        this.tokenEndpoint = tokenEndpoint;
        return this;
    }

    public OAuth2OptionsBuilder WithAuthorizationEndpoint(Uri authorizationEndpoint)
    {
        this.authorizationEndpoint = authorizationEndpoint;
        return this;
    }

    public OAuth2OptionsBuilder WithEmptyClientId()
    {
        this.clientId = string.Empty;
        return this;
    }

    public OAuth2OptionsBuilder WithEmptyClientSecret()
    {
        this.clientSecret = string.Empty;
        return this;
    }

    public OAuth2Options Build() => new()
    {
        ClientId = this.clientId,
        ClientSecret = this.clientSecret,
        RefreshToken = this.refreshToken,
        RedirectUri = this.redirectUri,
        Scope = this.scope,
        UsePkce = this.usePkce,
        TokenEndpoint = this.tokenEndpoint,
        AuthorizationEndpoint = this.authorizationEndpoint
    };

    public static implicit operator OAuth2Options(OAuth2OptionsBuilder builder) => builder.Build();
}

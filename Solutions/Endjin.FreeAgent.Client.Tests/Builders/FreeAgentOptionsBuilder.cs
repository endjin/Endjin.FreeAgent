// <copyright file="FreeAgentOptionsBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client.Tests.Builders;

using Endjin.FreeAgent.Client;

public class FreeAgentOptionsBuilder
{
    private string clientId = "test_client_id";
    private string clientSecret = "test_client_secret";
    private string refreshToken = "test_refresh_token";
    private bool useSandbox;

    public FreeAgentOptionsBuilder WithClientId(string clientId)
    {
        this.clientId = clientId;
        return this;
    }

    public FreeAgentOptionsBuilder WithClientSecret(string clientSecret)
    {
        this.clientSecret = clientSecret;
        return this;
    }

    public FreeAgentOptionsBuilder WithRefreshToken(string refreshToken)
    {
        this.refreshToken = refreshToken;
        return this;
    }

    public FreeAgentOptionsBuilder WithEmptyClientId()
    {
        this.clientId = string.Empty;
        return this;
    }

    public FreeAgentOptionsBuilder WithEmptyClientSecret()
    {
        this.clientSecret = string.Empty;
        return this;
    }

    public FreeAgentOptionsBuilder WithEmptyRefreshToken()
    {
        this.refreshToken = string.Empty;
        return this;
    }

    public FreeAgentOptionsBuilder WithUseSandbox(bool useSandbox = true)
    {
        this.useSandbox = useSandbox;
        return this;
    }

    public FreeAgentOptions Build() => new()
    {
        ClientId = this.clientId,
        ClientSecret = this.clientSecret,
        RefreshToken = this.refreshToken,
        UseSandbox = this.useSandbox
    };

    public static implicit operator FreeAgentOptions(FreeAgentOptionsBuilder builder) => builder.Build();
}

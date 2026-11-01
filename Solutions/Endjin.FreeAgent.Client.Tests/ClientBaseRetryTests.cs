// <copyright file="ClientBaseRetryTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

using Corvus.Retry.Policies;
using Corvus.Retry.Strategies;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

/// <summary>
/// Covers the retry behaviour that <see cref="ClientBase"/> wraps around every request:
/// transient failures are retried, the strategy's limit is honoured, the policy can veto a retry,
/// cancellation stops retrying, and every request starts from a fresh strategy.
/// </summary>
[TestClass]
public class ClientBaseRetryTests
{
    private const string UsersPath = "v2/users";

    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private HttpClient httpClient = null!;
    private TestHttpMessageHandler messageHandler = null!;

    [TestInitialize]
    public async Task Setup()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.messageHandler = new TestHttpMessageHandler();
        this.httpClient = new HttpClient(this.messageHandler);
        this.httpClientFactory = Substitute.For<IHttpClientFactory>();
        this.httpClientFactory.CreateClient(Arg.Any<string>()).Returns(this.httpClient);
        this.loggerFactory = Substitute.For<ILoggerFactory>();

        this.freeAgentClient = new FreeAgentClient(
            new FreeAgentOptionsBuilder().Build(),
            this.cache,
            this.httpClientFactory,
            this.loggerFactory);

        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
    }

    [TestMethod]
    public void Defaults_AreExponentialBackoffAndRetryOnAnyException()
    {
        this.freeAgentClient.RetryStrategyFactory().ShouldBeOfType<Backoff>();
        this.freeAgentClient.RetryPolicy.ShouldBeOfType<AnyExceptionPolicy>();

        // Strategies are stateful, so each request must get its own instance.
        this.freeAgentClient.RetryStrategyFactory().ShouldNotBeSameAs(this.freeAgentClient.RetryStrategyFactory());
    }

    [TestMethod]
    public async Task TransientFailure_IsRetriedUntilTheRequestSucceeds()
    {
        // Arrange
        this.freeAgentClient.RetryStrategyFactory = () => new Count(3);
        this.messageHandler.SetupResponseSequence(
            UsersPath,
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            UsersResponse("Recovered"));

        // Act
        List<UsersRoot> pages = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(this.UsersUri);

        // Assert
        pages.Single().Users.Single().FirstName.ShouldBe("Recovered");
        this.messageHandler.ShouldHaveBeenCalledTimes(2);
    }

    [TestMethod]
    public async Task RepeatedFailures_PropagateOnceTheStrategyGivesUp()
    {
        // Arrange: Count(3) allows three tries in total.
        this.freeAgentClient.RetryStrategyFactory = () => new Count(3);
        this.messageHandler.SetupResponseSequence(
            UsersPath,
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            UsersResponse("Never reached"));

        // Act
        HttpRequestException exception = await Should.ThrowAsync<HttpRequestException>(
            () => this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(this.UsersUri));

        // Assert
        exception.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
        this.messageHandler.ShouldHaveBeenCalledTimes(3);
    }

    [TestMethod]
    public async Task PolicyThatRefuses_PreventsAnyRetry()
    {
        // Arrange: the strategy would allow retries, but the policy vetoes them.
        this.freeAgentClient.RetryStrategyFactory = () => new Count(3);
        this.freeAgentClient.RetryPolicy = new DoNotRetryPolicy();
        this.messageHandler.SetupResponseSequence(
            UsersPath,
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            UsersResponse("Never reached"));

        // Act
        await Should.ThrowAsync<HttpRequestException>(
            () => this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(this.UsersUri));

        // Assert
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task CancelledToken_StopsRetryingAndPropagatesTheFailure()
    {
        // Arrange: the token is cancelled while the first request is in flight, so the failure
        // that request produces must not be retried even though the strategy has tries left.
        using CancellationTokenSource cts = new();
        this.freeAgentClient.RetryStrategyFactory = () => new Count(5);
        this.messageHandler.OnRequest = _ => cts.Cancel();
        this.messageHandler.SetupResponseSequence(
            UsersPath,
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            UsersResponse("Never reached"));

        // Act
        await Should.ThrowAsync<HttpRequestException>(async () =>
        {
            await foreach (UsersRoot _ in this.freeAgentClient.ExecuteRequestAndFollowLinksAsyncEnumerable<UsersRoot>(this.UsersUri, cts.Token))
            {
            }
        });

        // Assert
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task EachRequest_StartsFromAFreshStrategy()
    {
        // Arrange: Count(2) allows one retry per request. A strategy shared across requests would
        // carry its try count over, so the second request's single failure would be fatal.
        this.freeAgentClient.RetryStrategyFactory = () => new Count(2);
        this.messageHandler.SetupResponseSequence(
            UsersPath,
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            UsersResponse("First"),
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable),
            UsersResponse("Second"));

        // Act
        List<UsersRoot> first = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(this.UsersUri);
        List<UsersRoot> second = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(this.UsersUri);

        // Assert
        first.Single().Users.Single().FirstName.ShouldBe("First");
        second.Single().Users.Single().FirstName.ShouldBe("Second");
        this.messageHandler.ShouldHaveBeenCalledTimes(4);
    }

    private Uri UsersUri => new(this.freeAgentClient.ApiBaseUrl, UsersPath);

    private static HttpResponseMessage UsersResponse(string firstName)
    {
        UsersRoot body = new()
        {
            Users = ImmutableList.Create(new User { FirstName = firstName, LastName = "User", Role = Role.Employee })
        };

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
    }
}
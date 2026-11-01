// <copyright file="PaginationTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class PaginationTests
{
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
    [DataRow("next")]
    [DataRow("Next")]
    [DataRow("NEXT")]
    public async Task ExecuteRequestAndFollowLinksAsync_FollowsTheNextLinkRegardlessOfRelCasing(string rel)
    {
        // Arrange: RFC 8288 defines link relation types as case-insensitive.
        HttpResponseMessage firstPage = JsonResponse(new UsersRoot
        {
            Users = ImmutableList.Create(new User { FirstName = "Page1", LastName = "User1", Role = Role.Employee })
        });
        firstPage.Headers.Add("Link", $"<https://api.freeagent.com/v2/users?page=2>; rel=\"{rel}\"");

        HttpResponseMessage secondPage = JsonResponse(new UsersRoot
        {
            Users = ImmutableList.Create(new User { FirstName = "Page2", LastName = "User2", Role = Role.Employee })
        });

        this.messageHandler.Response = firstPage;
        this.messageHandler.SetupResponse("v2/users?page=2", secondPage);

        // Act
        List<UsersRoot> pages = await this.freeAgentClient.ExecuteRequestAndFollowLinksAsync<UsersRoot>(
            new Uri(this.freeAgentClient.ApiBaseUrl, "v2/users"));

        // Assert
        pages.Count.ShouldBe(2);
        pages.SelectMany(x => x.Users).Select(x => x.FirstName).ShouldBe(["Page1", "Page2"]);
        this.messageHandler.ShouldHaveBeenCalledTimes(2);
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users?page=2");
    }

    private static HttpResponseMessage JsonResponse<T>(T body)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
    }
}
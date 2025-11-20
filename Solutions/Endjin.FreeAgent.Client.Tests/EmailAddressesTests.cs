// <copyright file="EmailAddressesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class EmailAddressesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private EmailAddresses emailAddresses = null!;
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
        this.emailAddresses = new EmailAddresses(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllEmailAddresses()
    {
        // Arrange
        List<string> emailAddressesList =
        [
            "John Smith <jsmith@example.com>",
            "Jane Doe <jane@company.com>",
            "Admin <admin@business.org>"
        ];

        EmailAddressesRoot responseRoot = new() { EmailAddresses = emailAddressesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<string> result = await this.emailAddresses.GetAllAsync();

        // Assert
        result.Count().ShouldBe(3);
        result.ShouldContain("John Smith <jsmith@example.com>");
        result.ShouldContain("Jane Doe <jane@company.com>");
        result.ShouldContain("Admin <admin@business.org>");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/email_addresses");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsEmptyListWhenNoEmailAddresses()
    {
        // Arrange
        EmailAddressesRoot responseRoot = new() { EmailAddresses = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<string> result = await this.emailAddresses.GetAllAsync();

        // Assert
        result.Count().ShouldBe(0);
        result.ShouldBeEmpty();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/email_addresses");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsCachedResultOnSecondCall()
    {
        // Arrange
        List<string> emailAddressesList =
        [
            "Cached User <cached@example.com>"
        ];

        EmailAddressesRoot responseRoot = new() { EmailAddresses = emailAddressesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<string> firstResult = await this.emailAddresses.GetAllAsync();
        IEnumerable<string> secondResult = await this.emailAddresses.GetAllAsync();

        // Assert
        firstResult.Count().ShouldBe(1);
        secondResult.Count().ShouldBe(1);
        firstResult.ShouldContain("Cached User <cached@example.com>");
        secondResult.ShouldContain("Cached User <cached@example.com>");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/email_addresses");
    }
}
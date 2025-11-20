// <copyright file="HirePurchasesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class HirePurchasesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private HirePurchases hirePurchases = null!;
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
        this.hirePurchases = new HirePurchases(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllHirePurchases()
    {
        // Arrange
        List<HirePurchase> hirePurchasesList =
        [
            new HirePurchase
            {
                Url = new Uri("https://api.freeagent.com/v2/hire_purchases/1"),
                Description = "Office Equipment Hire Purchase",
                Bill = new Uri("https://api.freeagent.com/v2/bills/123"),
                LiabilitiesOverOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/793-1"),
                LiabilitiesUnderOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/792-1")
            },
            new HirePurchase
            {
                Url = new Uri("https://api.freeagent.com/v2/hire_purchases/2"),
                Description = "Vehicle Hire Purchase",
                Bill = new Uri("https://api.freeagent.com/v2/bills/456"),
                LiabilitiesOverOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/793-1"),
                LiabilitiesUnderOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/792-1")
            }
        ];

        HirePurchasesRoot responseRoot = new() { HirePurchases = hirePurchasesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<HirePurchase> result = await this.hirePurchases.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.First().Url.ShouldBe(new Uri("https://api.freeagent.com/v2/hire_purchases/1"));
        result.First().Description.ShouldBe("Office Equipment Hire Purchase");
        result.First().Bill.ShouldBe(new Uri("https://api.freeagent.com/v2/bills/123"));
        result.First().LiabilitiesOverOneYearCategory.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/793-1"));
        result.First().LiabilitiesUnderOneYearCategory.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/792-1"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsEmptyListWhenNoHirePurchases()
    {
        // Arrange
        HirePurchasesRoot responseRoot = new() { HirePurchases = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<HirePurchase> result = await this.hirePurchases.GetAllAsync();

        // Assert
        result.Count().ShouldBe(0);
        result.ShouldBeEmpty();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsCachedResultOnSecondCall()
    {
        // Arrange
        List<HirePurchase> hirePurchasesList =
        [
            new HirePurchase
            {
                Url = new Uri("https://api.freeagent.com/v2/hire_purchases/1"),
                Description = "Cached Hire Purchase"
            }
        ];

        HirePurchasesRoot responseRoot = new() { HirePurchases = hirePurchasesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<HirePurchase> firstResult = await this.hirePurchases.GetAllAsync();
        IEnumerable<HirePurchase> secondResult = await this.hirePurchases.GetAllAsync();

        // Assert
        firstResult.Count().ShouldBe(1);
        secondResult.Count().ShouldBe(1);
        firstResult.First().Description.ShouldBe("Cached Hire Purchase");
        secondResult.First().Description.ShouldBe("Cached Hire Purchase");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsSingleHirePurchase()
    {
        // Arrange
        HirePurchase hirePurchase = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/hire_purchases/123"),
            Description = "Single Hire Purchase",
            Bill = new Uri("https://api.freeagent.com/v2/bills/789"),
            LiabilitiesOverOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/793-1"),
            LiabilitiesUnderOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/792-1")
        };

        HirePurchaseRoot responseRoot = new() { HirePurchase = hirePurchase };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        HirePurchase result = await this.hirePurchases.GetByIdAsync("123");

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/hire_purchases/123"));
        result.Description.ShouldBe("Single Hire Purchase");
        result.Bill.ShouldBe(new Uri("https://api.freeagent.com/v2/bills/789"));
        result.LiabilitiesOverOneYearCategory.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/793-1"));
        result.LiabilitiesUnderOneYearCategory.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/792-1"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases/123");
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsArgumentExceptionForNullId()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.hirePurchases.GetByIdAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsArgumentExceptionForEmptyId()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.hirePurchases.GetByIdAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsArgumentExceptionForWhitespaceId()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.hirePurchases.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsInvalidOperationExceptionWhenResponseIsEmpty()
    {
        // Arrange
        HirePurchaseRoot responseRoot = new() { HirePurchase = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await this.hirePurchases.GetByIdAsync("123"));
    }

    [TestMethod]
    public async Task GetAllAsync_ThrowsHttpRequestExceptionOn404()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.hirePurchases.GetAllAsync());

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases");
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsHttpRequestExceptionOn404()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.hirePurchases.GetByIdAsync("123"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases/123");
    }

    [TestMethod]
    public async Task GetAllAsync_ThrowsHttpRequestExceptionOnServerError()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server Error", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.hirePurchases.GetAllAsync());

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases");
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsHttpRequestExceptionOnServerError()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Server Error", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.hirePurchases.GetByIdAsync("456"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases/456");
    }

    [TestMethod]
    public async Task GetAllAsync_ThrowsJsonExceptionOnMalformedJson()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<JsonException>(async () =>
            await this.hirePurchases.GetAllAsync());

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases");
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsJsonExceptionOnMalformedJson()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<JsonException>(async () =>
            await this.hirePurchases.GetByIdAsync("789"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases/789");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsCachedResultOnSecondCall()
    {
        // Arrange
        HirePurchase hirePurchase = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/hire_purchases/999"),
            Description = "Cached Single Hire Purchase",
            Bill = new Uri("https://api.freeagent.com/v2/bills/888"),
            LiabilitiesOverOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/793-1"),
            LiabilitiesUnderOneYearCategory = new Uri("https://api.freeagent.com/v2/categories/792-1")
        };

        HirePurchaseRoot responseRoot = new() { HirePurchase = hirePurchase };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        HirePurchase firstResult = await this.hirePurchases.GetByIdAsync("999");
        HirePurchase secondResult = await this.hirePurchases.GetByIdAsync("999");

        // Assert
        firstResult.ShouldNotBeNull();
        secondResult.ShouldNotBeNull();
        firstResult.Description.ShouldBe("Cached Single Hire Purchase");
        secondResult.Description.ShouldBe("Cached Single Hire Purchase");
        firstResult.Bill.ShouldBe(new Uri("https://api.freeagent.com/v2/bills/888"));
        secondResult.Bill.ShouldBe(new Uri("https://api.freeagent.com/v2/bills/888"));

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/hire_purchases/999");
    }
}

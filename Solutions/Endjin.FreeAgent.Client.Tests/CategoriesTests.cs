// <copyright file="CategoriesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CategoriesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Categories categories = null!;
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
        this.categories = new Categories(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        List<Category> categoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/1"),
                NominalCode = "001",
                Description = "Sales"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/2"),
                NominalCode = "002",
                Description = "Purchases"
            }
        ];

        CategoriesRoot responseRoot = new() { Categories = categoriesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Category> result = await this.categories.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(c => c.NominalCode == "001").ShouldBeTrue();
        result.Any(c => c.Description == "Purchases").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories");
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<Category> categoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/10"),
                NominalCode = "010",
                Description = "Cached Category"
            }
        ];

        CategoriesRoot responseRoot = new() { Categories = categoriesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<Category> result1 = await this.categories.GetAllAsync();
        IEnumerable<Category> result2 = await this.categories.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByNominalCodeAsync_WithValidCode_ReturnsCategory()
    {
        // Arrange
        List<Category> categoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/1"),
                NominalCode = "100",
                Description = "Fixed Assets"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/2"),
                NominalCode = "200",
                Description = "Current Assets"
            }
        ];

        CategoriesRoot responseRoot = new() { Categories = categoriesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Category result = await this.categories.GetByNominalCodeAsync("200");

        // Assert
        result.ShouldNotBeNull();
        result.NominalCode.ShouldBe("200");
        result.Description.ShouldBe("Current Assets");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByNominalCodeAsync_WithInvalidCode_ThrowsException()
    {
        // Arrange
        List<Category> categoriesList =
        [
            new()
            {
                NominalCode = "100",
                Description = "Fixed Assets"
            }
        ];

        CategoriesRoot responseRoot = new() { Categories = categoriesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
        {
            await this.categories.GetByNominalCodeAsync("999");
        });
    }

    [TestMethod]
    public async Task GetByNominalCodeAsync_CachesResult()
    {
        // Arrange
        List<Category> categoriesList =
        [
            new()
            {
                NominalCode = "300",
                Description = "Cached Nominal Code Category"
            }
        ];

        CategoriesRoot responseRoot = new() { Categories = categoriesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        Category result1 = await this.categories.GetByNominalCodeAsync("300");
        Category result2 = await this.categories.GetByNominalCodeAsync("300");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }
}

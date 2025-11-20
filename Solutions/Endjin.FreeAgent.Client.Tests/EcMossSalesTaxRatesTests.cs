// <copyright file="EcMossSalesTaxRatesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class EcMossSalesTaxRatesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private EcMossSalesTaxRates ecMossSalesTaxRates = null!;
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
        this.ecMossSalesTaxRates = new EcMossSalesTaxRates(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_WithValidCountryAndDate_ReturnsRates()
    {
        // Arrange
        List<EcMossSalesTaxRate> ratesList =
        [
            new()
            {
                Percentage = 20.0m,
                Band = "Standard"
            },
            new()
            {
                Percentage = 13.0m,
                Band = "Reduced"
            }
        ];

        EcMossSalesTaxRatesRoot responseRoot = new() { SalesTaxRates = ratesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<EcMossSalesTaxRate> result = await this.ecMossSalesTaxRates.GetAsync("Austria", new DateOnly(2017, 1, 1));

        // Assert
        result.Count().ShouldBe(2);
        result.First().Percentage.ShouldBe(20.0m);
        result.First().Band.ShouldBe("Standard");
        result.Last().Percentage.ShouldBe(13.0m);
        result.Last().Band.ShouldBe("Reduced");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/ec_moss/sales_tax_rates?country=Austria&date=2017-01-01");
    }

    [TestMethod]
    public async Task GetAsync_WithCountryContainingSpaces_EncodesCountryCorrectly()
    {
        // Arrange
        List<EcMossSalesTaxRate> ratesList =
        [
            new()
            {
                Percentage = 21.0m,
                Band = "Standard"
            }
        ];

        EcMossSalesTaxRatesRoot responseRoot = new() { SalesTaxRates = ratesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<EcMossSalesTaxRate> result = await this.ecMossSalesTaxRates.GetAsync("Czech Republic", new DateOnly(2024, 6, 15));

        // Assert
        result.Count().ShouldBe(1);
        result.First().Percentage.ShouldBe(21.0m);

        // Mock Verification - URI.ToString() decodes the encoding, so we check for the decoded form
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/ec_moss/sales_tax_rates?country=Czech Republic&date=2024-06-15");
    }

    [TestMethod]
    public async Task GetAsync_WithEmptyCountry_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.ecMossSalesTaxRates.GetAsync(string.Empty, new DateOnly(2024, 1, 1)));
    }

    [TestMethod]
    public async Task GetAsync_WithNullCountry_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.ecMossSalesTaxRates.GetAsync(null!, new DateOnly(2024, 1, 1)));
    }

    [TestMethod]
    public async Task GetAsync_WithNotFound_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.ecMossSalesTaxRates.GetAsync("InvalidCountry", new DateOnly(2024, 1, 1)));
    }

    [TestMethod]
    public async Task GetAsync_CachesResultsPerCountryAndDate()
    {
        // Arrange
        List<EcMossSalesTaxRate> ratesList =
        [
            new()
            {
                Percentage = 19.0m,
                Band = "Standard"
            }
        ];

        EcMossSalesTaxRatesRoot responseRoot = new() { SalesTaxRates = ratesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - call twice with same parameters
        IEnumerable<EcMossSalesTaxRate> result1 = await this.ecMossSalesTaxRates.GetAsync("Germany", new DateOnly(2024, 1, 1));
        IEnumerable<EcMossSalesTaxRate> result2 = await this.ecMossSalesTaxRates.GetAsync("Germany", new DateOnly(2024, 1, 1));

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAsync_DifferentCountries_MakesSeparateCalls()
    {
        // Arrange
        List<EcMossSalesTaxRate> germanyRates =
        [
            new() { Percentage = 19.0m, Band = "Standard" }
        ];

        List<EcMossSalesTaxRate> franceRates =
        [
            new() { Percentage = 20.0m, Band = "Standard" }
        ];

        EcMossSalesTaxRatesRoot germanyRoot = new() { SalesTaxRates = germanyRates };
        EcMossSalesTaxRatesRoot franceRoot = new() { SalesTaxRates = franceRates };

        // Setup responses based on URI patterns
        this.messageHandler.SetupResponse("country=Germany", new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(germanyRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        });
        this.messageHandler.SetupResponse("country=France", new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(franceRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        });

        // Act
        IEnumerable<EcMossSalesTaxRate> germanyResult = await this.ecMossSalesTaxRates.GetAsync("Germany", new DateOnly(2024, 1, 1));
        IEnumerable<EcMossSalesTaxRate> franceResult = await this.ecMossSalesTaxRates.GetAsync("France", new DateOnly(2024, 1, 1));

        // Assert
        germanyResult.First().Percentage.ShouldBe(19.0m);
        franceResult.First().Percentage.ShouldBe(20.0m);

        // Should call API twice for different countries
        this.messageHandler.CallCount.ShouldBe(2);
    }
}

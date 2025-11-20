// <copyright file="CisBandsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CisBandsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CisBands cisBands = null!;
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
        this.cisBands = new CisBands(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_ReturnsAllThreeBands()
    {
        // Arrange
        List<CisBand> bandsList =
        [
            new()
            {
                Name = "cis_gross",
                DeductionRate = 0.0m,
                IncomeDescription = "CIS Income - Gross",
                DeductionDescription = "CIS Deductions Suffered - Gross",
                NominalCode = "061"
            },
            new()
            {
                Name = "cis_standard",
                DeductionRate = 0.2m,
                IncomeDescription = "CIS Income - Standard Rate",
                DeductionDescription = "CIS Deductions Suffered - Standard Rate",
                NominalCode = "062"
            },
            new()
            {
                Name = "cis_higher",
                DeductionRate = 0.3m,
                IncomeDescription = "CIS Income - Higher Rate",
                DeductionDescription = "CIS Deductions Suffered - Higher Rate",
                NominalCode = "063"
            }
        ];

        CisBandsResponse responseRoot = new()
        {
            AvailableBands = bandsList
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        result.Count().ShouldBe(3);
        result.Any(b => b.Name == "cis_gross").ShouldBeTrue();
        result.Any(b => b.Name == "cis_standard").ShouldBeTrue();
        result.Any(b => b.Name == "cis_higher").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/cis_bands");
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_DeserializesAllPropertiesCorrectly()
    {
        // Arrange
        List<CisBand> bandsList =
        [
            new()
            {
                Name = "cis_standard",
                DeductionRate = 0.2m,
                IncomeDescription = "CIS Income - Standard Rate",
                DeductionDescription = "CIS Deductions Suffered - Standard Rate",
                NominalCode = "062"
            }
        ];

        CisBandsResponse responseRoot = new()
        {
            AvailableBands = bandsList
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        CisBand standardBand = result.First();
        standardBand.Name.ShouldBe("cis_standard");
        standardBand.DeductionRate.ShouldBe(0.2m);
        standardBand.IncomeDescription.ShouldBe("CIS Income - Standard Rate");
        standardBand.DeductionDescription.ShouldBe("CIS Deductions Suffered - Standard Rate");
        standardBand.NominalCode.ShouldBe("062");
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_GrossBand_HasZeroDeductionRate()
    {
        // Arrange
        List<CisBand> bandsList =
        [
            new()
            {
                Name = "cis_gross",
                DeductionRate = 0.0m,
                IncomeDescription = "CIS Income - Gross",
                DeductionDescription = "CIS Deductions Suffered - Gross",
                NominalCode = "061"
            }
        ];

        CisBandsResponse responseRoot = new()
        {
            AvailableBands = bandsList
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        CisBand grossBand = result.First(b => b.Name == "cis_gross");
        grossBand.DeductionRate.ShouldBe(0.0m);
        grossBand.NominalCode.ShouldBe("061");
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_StandardBand_HasTwentyPercentDeductionRate()
    {
        // Arrange
        List<CisBand> bandsList =
        [
            new()
            {
                Name = "cis_standard",
                DeductionRate = 0.2m,
                IncomeDescription = "CIS Income - Standard Rate",
                DeductionDescription = "CIS Deductions Suffered - Standard Rate",
                NominalCode = "062"
            }
        ];

        CisBandsResponse responseRoot = new()
        {
            AvailableBands = bandsList
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        CisBand standardBand = result.First(b => b.Name == "cis_standard");
        standardBand.DeductionRate.ShouldBe(0.2m);
        standardBand.NominalCode.ShouldBe("062");
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_HigherBand_HasThirtyPercentDeductionRate()
    {
        // Arrange
        List<CisBand> bandsList =
        [
            new()
            {
                Name = "cis_higher",
                DeductionRate = 0.3m,
                IncomeDescription = "CIS Income - Higher Rate",
                DeductionDescription = "CIS Deductions Suffered - Higher Rate",
                NominalCode = "063"
            }
        ];

        CisBandsResponse responseRoot = new()
        {
            AvailableBands = bandsList
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        CisBand higherBand = result.First(b => b.Name == "cis_higher");
        higherBand.DeductionRate.ShouldBe(0.3m);
        higherBand.NominalCode.ShouldBe("063");
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_CachesResults()
    {
        // Arrange
        List<CisBand> bandsList =
        [
            new()
            {
                Name = "cis_gross",
                DeductionRate = 0.0m,
                IncomeDescription = "CIS Income - Gross",
                DeductionDescription = "CIS Deductions Suffered - Gross",
                NominalCode = "061"
            }
        ];

        CisBandsResponse responseRoot = new()
        {
            AvailableBands = bandsList
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<CisBand> result1 = await this.cisBands.GetAvailableBandsAsync();
        IEnumerable<CisBand> result2 = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_WithEmptyResponse_ReturnsEmptyCollection()
    {
        // Arrange
        CisBandsResponse responseRoot = new()
        {
            AvailableBands = []
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        result.ShouldBeEmpty();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAvailableBandsAsync_WithNullAvailableBands_ReturnsEmptyCollection()
    {
        // Arrange
        CisBandsResponse responseRoot = new()
        {
            AvailableBands = null
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CisBand> result = await this.cisBands.GetAvailableBandsAsync();

        // Assert
        result.ShouldBeEmpty();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }
}

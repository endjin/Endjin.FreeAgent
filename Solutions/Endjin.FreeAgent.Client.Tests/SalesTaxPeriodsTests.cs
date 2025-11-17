// <copyright file="SalesTaxPeriodsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class SalesTaxPeriodsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private SalesTaxPeriods salesTaxPeriods = null!;
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
        this.salesTaxPeriods = new SalesTaxPeriods(this.freeAgentClient);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllSalesTaxPeriods()
    {
        // Arrange
        List<SalesTaxPeriod> periodsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/sales_tax_periods/1"),
                SalesTaxName = "VAT",
                SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
                SalesTaxIsValueAdded = true,
                EffectiveDate = new DateOnly(2024, 1, 1),
                SalesTaxRate1 = 20m,
                SalesTaxRate2 = 5m,
                SalesTaxRate3 = 0m,
                SalesTaxRegistrationNumber = "GB123456789",
                IsLocked = false
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/sales_tax_periods/2"),
                SalesTaxName = "VAT",
                SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.NotRegistered,
                SalesTaxIsValueAdded = true,
                EffectiveDate = new DateOnly(2023, 1, 1),
                SalesTaxRate1 = 20m,
                SalesTaxRate2 = 5m,
                SalesTaxRate3 = 0m,
                IsLocked = true,
                LockedReason = true
            }
        ];

        SalesTaxPeriodsRoot responseRoot = new() { SalesTaxPeriods = periodsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<SalesTaxPeriod> result = await this.salesTaxPeriods.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.First().SalesTaxName.ShouldBe("VAT");
        result.First().SalesTaxRegistrationStatus.ShouldBe(SalesTaxRegistrationStatus.Registered);
        result.First().SalesTaxIsValueAdded.ShouldBe(true);
        result.First().EffectiveDate.ShouldBe(new DateOnly(2024, 1, 1));
        result.First().SalesTaxRate1.ShouldBe(20m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/sales_tax_periods");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsSalesTaxPeriod()
    {
        // Arrange
        SalesTaxPeriod period = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/sales_tax_periods/123"),
            SalesTaxName = "GST",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 6, 15),
            SalesTaxRate1 = 10m,
            SalesTaxRate2 = 0m,
            SalesTaxRate3 = 0m,
            SalesTaxRegistrationNumber = "12-345-678",
            IsLocked = false
        };

        SalesTaxPeriodRoot responseRoot = new() { SalesTaxPeriod = period };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SalesTaxPeriod result = await this.salesTaxPeriods.GetByIdAsync("123");

        // Assert
        result.SalesTaxName.ShouldBe("GST");
        result.SalesTaxRegistrationStatus.ShouldBe(SalesTaxRegistrationStatus.Registered);
        result.SalesTaxIsValueAdded.ShouldBe(true);
        result.EffectiveDate.ShouldBe(new DateOnly(2024, 6, 15));
        result.SalesTaxRate1.ShouldBe(10m);
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/sales_tax_periods/123"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/sales_tax_periods/123");
    }

    [TestMethod]
    public async Task GetByIdAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.GetByIdAsync(string.Empty));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task CreateAsync_CreatesAndReturnsSalesTaxPeriod()
    {
        // Arrange
        SalesTaxPeriod periodToCreate = new()
        {
            SalesTaxName = "Sales Tax",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.NotRegistered,
            SalesTaxIsValueAdded = false,
            EffectiveDate = new DateOnly(2024, 7, 1),
            SalesTaxRate1 = 8.5m,
            SalesTaxRate2 = 4m,
            SalesTaxRate3 = 0m
        };

        SalesTaxPeriod createdPeriod = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/sales_tax_periods/456"),
            SalesTaxName = "Sales Tax",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.NotRegistered,
            SalesTaxIsValueAdded = false,
            EffectiveDate = new DateOnly(2024, 7, 1),
            SalesTaxRate1 = 8.5m,
            SalesTaxRate2 = 4m,
            SalesTaxRate3 = 0m,
            IsLocked = false
        };

        SalesTaxPeriodRoot responseRoot = new() { SalesTaxPeriod = createdPeriod };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SalesTaxPeriod result = await this.salesTaxPeriods.CreateAsync(periodToCreate);

        // Assert
        result.SalesTaxName.ShouldBe("Sales Tax");
        result.SalesTaxRegistrationStatus.ShouldBe(SalesTaxRegistrationStatus.NotRegistered);
        result.SalesTaxIsValueAdded.ShouldBe(false);
        result.EffectiveDate.ShouldBe(new DateOnly(2024, 7, 1));
        result.SalesTaxRate1.ShouldBe(8.5m);
        result.Url.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/sales_tax_periods");
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentNullException_WhenPeriodIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.salesTaxPeriods.CreateAsync(null!));
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenSalesTaxNameIsMissing()
    {
        // Arrange
        SalesTaxPeriod period = new()
        {
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 7, 1)
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.CreateAsync(period));
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenSalesTaxRegistrationStatusIsMissing()
    {
        // Arrange
        SalesTaxPeriod period = new()
        {
            SalesTaxName = "VAT",
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 7, 1)
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.CreateAsync(period));
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenSalesTaxIsValueAddedIsMissing()
    {
        // Arrange
        SalesTaxPeriod period = new()
        {
            SalesTaxName = "VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            EffectiveDate = new DateOnly(2024, 7, 1)
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.CreateAsync(period));
    }

    [TestMethod]
    public async Task CreateAsync_ThrowsArgumentException_WhenEffectiveDateIsMissing()
    {
        // Arrange
        SalesTaxPeriod period = new()
        {
            SalesTaxName = "VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.CreateAsync(period));
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesAndReturnsSalesTaxPeriod()
    {
        // Arrange
        string periodId = "789";
        SalesTaxPeriod periodToUpdate = new()
        {
            SalesTaxName = "Updated VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 8, 1),
            SalesTaxRate1 = 21m,
            SalesTaxRate2 = 6m,
            SalesTaxRate3 = 0m,
            SalesTaxRegistrationNumber = "GB987654321"
        };

        SalesTaxPeriod updatedPeriod = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/sales_tax_periods/{periodId}"),
            SalesTaxName = "Updated VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 8, 1),
            SalesTaxRate1 = 21m,
            SalesTaxRate2 = 6m,
            SalesTaxRate3 = 0m,
            SalesTaxRegistrationNumber = "GB987654321",
            IsLocked = false
        };

        SalesTaxPeriodRoot responseRoot = new() { SalesTaxPeriod = updatedPeriod };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SalesTaxPeriod result = await this.salesTaxPeriods.UpdateAsync(periodId, periodToUpdate);

        // Assert
        result.SalesTaxName.ShouldBe("Updated VAT");
        result.SalesTaxRate1.ShouldBe(21m);
        result.SalesTaxRegistrationNumber.ShouldBe("GB987654321");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/sales_tax_periods/{periodId}");
    }

    [TestMethod]
    public async Task UpdateAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace()
    {
        // Arrange
        SalesTaxPeriod period = new()
        {
            SalesTaxName = "VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 1, 1)
        };

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.UpdateAsync(string.Empty, period));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.UpdateAsync("   ", period));
    }

    [TestMethod]
    public async Task UpdateAsync_ThrowsArgumentNullException_WhenPeriodIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.salesTaxPeriods.UpdateAsync("123", null!));
    }

    [TestMethod]
    public async Task DeleteAsync_DeletesSalesTaxPeriod()
    {
        // Arrange
        string periodId = "999";

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.salesTaxPeriods.DeleteAsync(periodId);

        // Assert
        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/sales_tax_periods/{periodId}");
    }

    [TestMethod]
    public async Task DeleteAsync_ThrowsArgumentException_WhenIdIsNullOrWhitespace()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.DeleteAsync(string.Empty));

        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.salesTaxPeriods.DeleteAsync("   "));
    }

    [TestMethod]
    public async Task DeleteAsync_LockedPeriod_ShouldFailWithAppropriateError()
    {
        // Arrange
        string periodId = "locked-period-123";

        // Simulate API error for locked period deletion
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.UnprocessableEntity)
        {
            Content = new StringContent("{\"error\":\"Cannot delete locked sales tax period\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.salesTaxPeriods.DeleteAsync(periodId));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/sales_tax_periods/{periodId}");
    }

    [TestMethod]
    public async Task CreateAsync_WithUniversalCompanyFields_CreatesAndReturnsPeriod()
    {
        // Arrange - Universal company with second tax system
        SalesTaxPeriod periodToCreate = new()
        {
            SalesTaxName = "VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 7, 1),
            SalesTaxRate1 = 20m,
            SalesTaxRate2 = 5m,
            SalesTaxRate3 = 0m,
            SalesTaxRegistrationNumber = "GB123456789",
            SecondSalesTaxName = "Sales Tax",
            SecondSalesTaxRate1 = 8.5m,
            SecondSalesTaxRate2 = 4m,
            SecondSalesTaxRate3 = 0m,
            SecondSalesTaxIsCompound = true
        };

        SalesTaxPeriod createdPeriod = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/sales_tax_periods/universal-456"),
            SalesTaxName = "VAT",
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            SalesTaxIsValueAdded = true,
            EffectiveDate = new DateOnly(2024, 7, 1),
            SalesTaxRate1 = 20m,
            SalesTaxRate2 = 5m,
            SalesTaxRate3 = 0m,
            SalesTaxRegistrationNumber = "GB123456789",
            SecondSalesTaxName = "Sales Tax",
            SecondSalesTaxRate1 = 8.5m,
            SecondSalesTaxRate2 = 4m,
            SecondSalesTaxRate3 = 0m,
            SecondSalesTaxIsCompound = true,
            IsLocked = false
        };

        SalesTaxPeriodRoot responseRoot = new() { SalesTaxPeriod = createdPeriod };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SalesTaxPeriod result = await this.salesTaxPeriods.CreateAsync(periodToCreate);

        // Assert
        result.SalesTaxName.ShouldBe("VAT");
        result.SecondSalesTaxName.ShouldBe("Sales Tax");
        result.SecondSalesTaxRate1.ShouldBe(8.5m);
        result.SecondSalesTaxIsCompound.ShouldBe(true);
        result.Url.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/sales_tax_periods");
    }
}
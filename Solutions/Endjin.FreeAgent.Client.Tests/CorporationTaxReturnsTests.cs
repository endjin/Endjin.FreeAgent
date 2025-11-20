// <copyright file="CorporationTaxReturnsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;

using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CorporationTaxReturnsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private CorporationTaxReturns corporationTaxReturns = null!;
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
        this.corporationTaxReturns = new CorporationTaxReturns(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllReturns()
    {
        // Arrange
        List<CorporationTaxReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = CorporationTaxFilingStatus.Unfiled,
                AmountDue = 1000.00m,
                PaymentDueOn = new DateOnly(2025, 1, 1),
                FilingDueOn = new DateOnly(2025, 3, 31)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2"),
                PeriodStartsOn = new DateOnly(2024, 4, 1),
                PeriodEndsOn = new DateOnly(2025, 3, 31),
                FilingStatus = CorporationTaxFilingStatus.Draft,
                AmountDue = 1500.00m,
                PaymentDueOn = new DateOnly(2026, 1, 1),
                FilingDueOn = new DateOnly(2026, 3, 31)
            }
        ];

        CorporationTaxReturnsRoot responseRoot = new() { CorporationTaxReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CorporationTaxReturn> result = await this.corporationTaxReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns");
    }

    [TestMethod]
    public async Task GetByPeriodEndDateAsync_WithValidDate_ReturnsReturn()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            AmountDue = 1000.00m,
            PaymentDueOn = new DateOnly(2025, 1, 1),
            FilingDueOn = new DateOnly(2025, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.GetByPeriodEndDateAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();
        result.PeriodStartsOn.ShouldBe(new DateOnly(2023, 4, 1));
        result.PeriodEndsOn.ShouldBe(periodEndsOn);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/2024-03-31");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_MarksReturnAsFiled()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            AmountDue = 1000.00m,
            PaymentDueOn = new DateOnly(2025, 1, 1),
            FilingDueOn = new DateOnly(2025, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.MarkAsFiledAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/2024-03-31/mark_as_filed");
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_MarksReturnAsUnfiled()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            AmountDue = 1000.00m,
            PaymentDueOn = new DateOnly(2025, 1, 1),
            FilingDueOn = new DateOnly(2025, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.MarkAsUnfiledAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/2024-03-31/mark_as_unfiled");
    }

    [TestMethod]
    public async Task MarkAsPaidAsync_MarksReturnAsPaid()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            AmountDue = 1000.00m,
            PaymentDueOn = new DateOnly(2025, 1, 1),
            FilingDueOn = new DateOnly(2025, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.MarkAsPaidAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/2024-03-31/mark_as_paid");
    }

    [TestMethod]
    public async Task MarkAsUnpaidAsync_MarksReturnAsUnpaid()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            AmountDue = 1000.00m,
            PaymentDueOn = new DateOnly(2025, 1, 1),
            FilingDueOn = new DateOnly(2025, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.MarkAsUnpaidAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/corporation_tax_returns/2024-03-31/mark_as_unpaid");
    }

    [TestMethod]
    public async Task GetAllAsync_WithNullableProperties_HandlesCorrectly()
    {
        // Arrange - Test with a return that has no payment required (nullables omitted)
        List<CorporationTaxReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = CorporationTaxFilingStatus.Filed,
                PaymentStatus = null, // No payment required
                AmountDue = null, // No amount due
                PaymentDueOn = null, // No payment due date
                FilingDueOn = new DateOnly(2025, 3, 31),
                FiledAt = new DateTime(2024, 12, 1, 10, 30, 0),
                FiledReference = "CT600-2024-001"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2"),
                PeriodStartsOn = new DateOnly(2024, 4, 1),
                PeriodEndsOn = new DateOnly(2025, 3, 31),
                FilingStatus = CorporationTaxFilingStatus.Unfiled,
                PaymentStatus = CorporationTaxPaymentStatus.Unpaid,
                AmountDue = 2500.50m,
                PaymentDueOn = new DateOnly(2026, 1, 1),
                FilingDueOn = new DateOnly(2026, 3, 31),
                FiledAt = null, // Not filed yet
                FiledReference = null // No reference yet
            }
        ];

        CorporationTaxReturnsRoot responseRoot = new() { CorporationTaxReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CorporationTaxReturn> result = await this.corporationTaxReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        CorporationTaxReturn filedReturn = result.First();
        filedReturn.PaymentStatus.ShouldBeNull();
        filedReturn.AmountDue.ShouldBeNull();
        filedReturn.PaymentDueOn.ShouldBeNull();
        filedReturn.FiledAt.ShouldNotBeNull();
        filedReturn.FiledReference.ShouldBe("CT600-2024-001");

        CorporationTaxReturn unfiledReturn = result.Last();
        unfiledReturn.PaymentStatus.ShouldBe(CorporationTaxPaymentStatus.Unpaid);
        unfiledReturn.AmountDue.ShouldBe(2500.50m);
        unfiledReturn.PaymentDueOn.ShouldBe(new DateOnly(2026, 1, 1));
        unfiledReturn.FiledAt.ShouldBeNull();
        unfiledReturn.FiledReference.ShouldBeNull();
    }

    [TestMethod]
    public async Task GetAllAsync_CachingBehavior_ReturnsFromCacheOnSecondCall()
    {
        // Arrange
        List<CorporationTaxReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = CorporationTaxFilingStatus.Unfiled,
                AmountDue = 1000.00m,
                PaymentDueOn = new DateOnly(2025, 1, 1),
                FilingDueOn = new DateOnly(2025, 3, 31)
            }
        ];

        CorporationTaxReturnsRoot responseRoot = new() { CorporationTaxReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call should hit the API
        IEnumerable<CorporationTaxReturn> firstResult = await this.corporationTaxReturns.GetAllAsync();

        // Act - Second call should come from cache
        IEnumerable<CorporationTaxReturn> secondResult = await this.corporationTaxReturns.GetAllAsync();

        // Assert
        firstResult.Count().ShouldBe(1);
        secondResult.Count().ShouldBe(1);

        // The message handler should only have been called once (cached on second call)
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByPeriodEndDateAsync_CachingBehavior_ReturnsFromCacheOnSecondCall()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        CorporationTaxReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            AmountDue = 1000.00m,
            PaymentDueOn = new DateOnly(2025, 1, 1),
            FilingDueOn = new DateOnly(2025, 3, 31)
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call should hit the API
        CorporationTaxReturn firstResult = await this.corporationTaxReturns.GetByPeriodEndDateAsync(periodEndsOn);

        // Act - Second call should come from cache
        CorporationTaxReturn secondResult = await this.corporationTaxReturns.GetByPeriodEndDateAsync(periodEndsOn);

        // Assert
        firstResult.ShouldNotBeNull();
        secondResult.ShouldNotBeNull();
        firstResult.PeriodEndsOn.ShouldBe(periodEndsOn);
        secondResult.PeriodEndsOn.ShouldBe(periodEndsOn);

        // The message handler should only have been called once (cached on second call)
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_InvalidatesCache()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        string cacheKeySpecific = $"corporation_tax_return_2024-03-31";
        string cacheKeyAll = "corporation_tax_returns_all";

        // Pre-populate cache
        CorporationTaxReturn cachedReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.Unfiled,
            FilingDueOn = new DateOnly(2025, 3, 31)
        };
        this.cache.Set(cacheKeySpecific, cachedReturn, TimeSpan.FromMinutes(5));
        this.cache.Set(cacheKeyAll, new List<CorporationTaxReturn> { cachedReturn }, TimeSpan.FromMinutes(5));

        CorporationTaxReturn filedReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = CorporationTaxFilingStatus.MarkedAsFiled,
            FilingDueOn = new DateOnly(2025, 3, 31),
            FiledAt = DateTime.UtcNow,
            FiledReference = "CT600-2024-FILED"
        };

        CorporationTaxReturnRoot responseRoot = new() { CorporationTaxReturn = filedReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        CorporationTaxReturn result = await this.corporationTaxReturns.MarkAsFiledAsync(periodEndsOn);

        // Assert
        result.FilingStatus.ShouldBe(CorporationTaxFilingStatus.MarkedAsFiled);

        // Cache should be cleared
        this.cache.TryGetValue(cacheKeySpecific, out CorporationTaxReturn? _).ShouldBeFalse();
        this.cache.TryGetValue(cacheKeyAll, out List<CorporationTaxReturn>? _).ShouldBeFalse();
    }

    [TestMethod]
    public async Task GetAllAsync_WithMarkedAsPaidStatus_HandlesCorrectly()
    {
        // Arrange - Test with different payment statuses
        List<CorporationTaxReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/corporation_tax_returns/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = CorporationTaxFilingStatus.Filed,
                PaymentStatus = CorporationTaxPaymentStatus.MarkedAsPaid,
                AmountDue = 5000.00m,
                PaymentDueOn = new DateOnly(2025, 1, 1),
                FilingDueOn = new DateOnly(2025, 3, 31),
                FiledAt = new DateTime(2024, 11, 15, 14, 30, 0),
                FiledReference = "CT600-2024-PAID"
            }
        ];

        CorporationTaxReturnsRoot responseRoot = new() { CorporationTaxReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<CorporationTaxReturn> result = await this.corporationTaxReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(1);

        CorporationTaxReturn paidReturn = result.First();
        paidReturn.PaymentStatus.ShouldBe(CorporationTaxPaymentStatus.MarkedAsPaid);
        paidReturn.FilingStatus.ShouldBe(CorporationTaxFilingStatus.Filed);
        paidReturn.AmountDue.ShouldBe(5000.00m);
    }
}

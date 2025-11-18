// <copyright file="FinalAccountsReportsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Endjin.FreeAgent.Domain.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class FinalAccountsReportsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private FinalAccountsReports finalAccountsReports = null!;
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
        this.finalAccountsReports = new FinalAccountsReports(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllReports()
    {
        // Arrange
        List<FinalAccountsReport> reportsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Unfiled,
                FilingDueOn = new DateOnly(2024, 12, 31)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2"),
                PeriodStartsOn = new DateOnly(2024, 4, 1),
                PeriodEndsOn = new DateOnly(2025, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Draft,
                FilingDueOn = new DateOnly(2025, 12, 31)
            }
        ];

        FinalAccountsReportsRoot responseRoot = new() { FinalAccountsReports = reportsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<FinalAccountsReport> result = await this.finalAccountsReports.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/final_accounts_reports");
    }

    [TestMethod]
    public async Task GetByPeriodEndDateAsync_WithValidDate_ReturnsReport()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        FinalAccountsReport report = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.Unfiled,
            FilingDueOn = new DateOnly(2024, 12, 31)
        };

        FinalAccountsReportRoot responseRoot = new() { FinalAccountsReport = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        FinalAccountsReport result = await this.finalAccountsReports.GetByPeriodEndDateAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();
        result.PeriodStartsOn.ShouldBe(new DateOnly(2023, 4, 1));
        result.PeriodEndsOn.ShouldBe(periodEndsOn);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/final_accounts_reports/2024-03-31");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_MarksReportAsFiled()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        FinalAccountsReport report = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.MarkedAsFiled,
            FilingDueOn = new DateOnly(2024, 12, 31),
            FiledAt = DateTime.UtcNow,
            FiledReference = "FA-2024-001"
        };

        FinalAccountsReportRoot responseRoot = new() { FinalAccountsReport = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        FinalAccountsReport result = await this.finalAccountsReports.MarkAsFiledAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe(FinalAccountsFilingStatus.MarkedAsFiled);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/final_accounts_reports/2024-03-31/mark_as_filed");
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_MarksReportAsUnfiled()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        FinalAccountsReport report = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.Unfiled,
            FilingDueOn = new DateOnly(2024, 12, 31)
        };

        FinalAccountsReportRoot responseRoot = new() { FinalAccountsReport = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        FinalAccountsReport result = await this.finalAccountsReports.MarkAsUnfiledAsync(periodEndsOn);

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe(FinalAccountsFilingStatus.Unfiled);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/final_accounts_reports/2024-03-31/mark_as_unfiled");
    }

    [TestMethod]
    public async Task GetAllAsync_WithNullableProperties_HandlesCorrectly()
    {
        // Arrange - Test with reports that have different nullable field states
        List<FinalAccountsReport> reportsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Filed,
                FilingDueOn = new DateOnly(2024, 12, 31),
                FiledAt = new DateTime(2024, 11, 1, 10, 30, 0),
                FiledReference = "FA-2024-001"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2"),
                PeriodStartsOn = new DateOnly(2024, 4, 1),
                PeriodEndsOn = new DateOnly(2025, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Unfiled,
                FilingDueOn = new DateOnly(2025, 12, 31),
                FiledAt = null, // Not filed yet
                FiledReference = null // No reference yet
            }
        ];

        FinalAccountsReportsRoot responseRoot = new() { FinalAccountsReports = reportsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<FinalAccountsReport> result = await this.finalAccountsReports.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        FinalAccountsReport filedReport = result.First();
        filedReport.FilingStatus.ShouldBe(FinalAccountsFilingStatus.Filed);
        filedReport.FiledAt.ShouldNotBeNull();
        filedReport.FiledReference.ShouldBe("FA-2024-001");

        FinalAccountsReport unfiledReport = result.Last();
        unfiledReport.FilingStatus.ShouldBe(FinalAccountsFilingStatus.Unfiled);
        unfiledReport.FiledAt.ShouldBeNull();
        unfiledReport.FiledReference.ShouldBeNull();
    }

    [TestMethod]
    public async Task GetAllAsync_CachingBehavior_ReturnsFromCacheOnSecondCall()
    {
        // Arrange
        List<FinalAccountsReport> reportsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/1"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Unfiled,
                FilingDueOn = new DateOnly(2024, 12, 31)
            }
        ];

        FinalAccountsReportsRoot responseRoot = new() { FinalAccountsReports = reportsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call should hit the API
        IEnumerable<FinalAccountsReport> firstResult = await this.finalAccountsReports.GetAllAsync();

        // Act - Second call should come from cache
        IEnumerable<FinalAccountsReport> secondResult = await this.finalAccountsReports.GetAllAsync();

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
        FinalAccountsReport report = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.Unfiled,
            FilingDueOn = new DateOnly(2024, 12, 31)
        };

        FinalAccountsReportRoot responseRoot = new() { FinalAccountsReport = report };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call should hit the API
        FinalAccountsReport firstResult = await this.finalAccountsReports.GetByPeriodEndDateAsync(periodEndsOn);

        // Act - Second call should come from cache
        FinalAccountsReport secondResult = await this.finalAccountsReports.GetByPeriodEndDateAsync(periodEndsOn);

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
        string cacheKeySpecific = $"final_accounts_report_2024-03-31";
        string cacheKeyAll = "final_accounts_reports_all";

        // Pre-populate cache
        FinalAccountsReport cachedReport = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.Unfiled,
            FilingDueOn = new DateOnly(2024, 12, 31)
        };
        this.cache.Set(cacheKeySpecific, cachedReport, TimeSpan.FromMinutes(5));
        this.cache.Set(cacheKeyAll, new List<FinalAccountsReport> { cachedReport }, TimeSpan.FromMinutes(5));

        FinalAccountsReport filedReport = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.MarkedAsFiled,
            FilingDueOn = new DateOnly(2024, 12, 31),
            FiledAt = DateTime.UtcNow,
            FiledReference = "FA-2024-FILED"
        };

        FinalAccountsReportRoot responseRoot = new() { FinalAccountsReport = filedReport };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        FinalAccountsReport result = await this.finalAccountsReports.MarkAsFiledAsync(periodEndsOn);

        // Assert
        result.FilingStatus.ShouldBe(FinalAccountsFilingStatus.MarkedAsFiled);

        // Cache should be cleared
        this.cache.TryGetValue(cacheKeySpecific, out FinalAccountsReport? _).ShouldBeFalse();
        this.cache.TryGetValue(cacheKeyAll, out List<FinalAccountsReport>? _).ShouldBeFalse();
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_InvalidatesCache()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        string cacheKeySpecific = $"final_accounts_report_2024-03-31";
        string cacheKeyAll = "final_accounts_reports_all";

        // Pre-populate cache
        FinalAccountsReport cachedReport = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.MarkedAsFiled,
            FilingDueOn = new DateOnly(2024, 12, 31),
            FiledAt = DateTime.UtcNow,
            FiledReference = "FA-2024-FILED"
        };
        this.cache.Set(cacheKeySpecific, cachedReport, TimeSpan.FromMinutes(5));
        this.cache.Set(cacheKeyAll, new List<FinalAccountsReport> { cachedReport }, TimeSpan.FromMinutes(5));

        FinalAccountsReport unfiledReport = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2024-03-31"),
            PeriodStartsOn = new DateOnly(2023, 4, 1),
            PeriodEndsOn = periodEndsOn,
            FilingStatus = FinalAccountsFilingStatus.Unfiled,
            FilingDueOn = new DateOnly(2024, 12, 31),
            FiledAt = null,
            FiledReference = null
        };

        FinalAccountsReportRoot responseRoot = new() { FinalAccountsReport = unfiledReport };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        FinalAccountsReport result = await this.finalAccountsReports.MarkAsUnfiledAsync(periodEndsOn);

        // Assert
        result.FilingStatus.ShouldBe(FinalAccountsFilingStatus.Unfiled);
        result.FiledAt.ShouldBeNull();
        result.FiledReference.ShouldBeNull();

        // Cache should be cleared
        this.cache.TryGetValue(cacheKeySpecific, out FinalAccountsReport? _).ShouldBeFalse();
        this.cache.TryGetValue(cacheKeyAll, out List<FinalAccountsReport>? _).ShouldBeFalse();
    }

    [TestMethod]
    public async Task GetAllAsync_WithAllFilingStatuses_HandlesCorrectly()
    {
        // Arrange - Test with all different filing statuses
        List<FinalAccountsReport> reportsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/1"),
                PeriodStartsOn = new DateOnly(2020, 4, 1),
                PeriodEndsOn = new DateOnly(2021, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Draft,
                FilingDueOn = new DateOnly(2021, 12, 31)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/2"),
                PeriodStartsOn = new DateOnly(2021, 4, 1),
                PeriodEndsOn = new DateOnly(2022, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Pending,
                FilingDueOn = new DateOnly(2022, 12, 31)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/3"),
                PeriodStartsOn = new DateOnly(2022, 4, 1),
                PeriodEndsOn = new DateOnly(2023, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Rejected,
                FilingDueOn = new DateOnly(2023, 12, 31)
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/final_accounts_reports/4"),
                PeriodStartsOn = new DateOnly(2023, 4, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingStatus = FinalAccountsFilingStatus.Filed,
                FilingDueOn = new DateOnly(2024, 12, 31),
                FiledAt = new DateTime(2024, 10, 15, 14, 30, 0),
                FiledReference = "FA-2024-FILED"
            }
        ];

        FinalAccountsReportsRoot responseRoot = new() { FinalAccountsReports = reportsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<FinalAccountsReport> result = await this.finalAccountsReports.GetAllAsync();

        // Assert
        result.Count().ShouldBe(4);

        List<FinalAccountsReport> resultList = result.ToList();
        resultList[0].FilingStatus.ShouldBe(FinalAccountsFilingStatus.Draft);
        resultList[1].FilingStatus.ShouldBe(FinalAccountsFilingStatus.Pending);
        resultList[2].FilingStatus.ShouldBe(FinalAccountsFilingStatus.Rejected);
        resultList[3].FilingStatus.ShouldBe(FinalAccountsFilingStatus.Filed);
    }

    [TestMethod]
    public async Task GetByPeriodEndDateAsync_WithNotFound_ThrowsHttpRequestException()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{\"error\":\"Resource not found\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(
            async () => await this.finalAccountsReports.GetByPeriodEndDateAsync(periodEndsOn));
    }

    [TestMethod]
    public async Task GetAllAsync_WithUnauthorized_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("{\"error\":\"Unauthorized access\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(
            async () => await this.finalAccountsReports.GetAllAsync());
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_WithForbidden_ThrowsHttpRequestException()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
        {
            Content = new StringContent("{\"error\":\"Forbidden\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(
            async () => await this.finalAccountsReports.MarkAsFiledAsync(periodEndsOn));
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_WithServerError_ThrowsHttpRequestException()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("{\"error\":\"Internal server error\"}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(
            async () => await this.finalAccountsReports.MarkAsUnfiledAsync(periodEndsOn));
    }

    [TestMethod]
    public async Task GetAllAsync_WithMalformedJson_ThrowsJsonException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<JsonException>(
            async () => await this.finalAccountsReports.GetAllAsync());
    }

    [TestMethod]
    public async Task GetByPeriodEndDateAsync_WithMissingRequiredFields_ThrowsJsonException()
    {
        // Arrange
        DateOnly periodEndsOn = new DateOnly(2024, 3, 31);

        // Response missing required fields
        string responseJson = @"{
            ""final_accounts_report"": {
                ""url"": ""https://api.freeagent.com/v2/final_accounts_reports/2024-03-31""
            }
        }";

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<JsonException>(
            async () => await this.finalAccountsReports.GetByPeriodEndDateAsync(periodEndsOn));
    }

    [TestMethod]
    public async Task GetAllAsync_WithEmptyResponse_ReturnsEmptyCollection()
    {
        // Arrange
        FinalAccountsReportsRoot responseRoot = new() { FinalAccountsReports = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<FinalAccountsReport> result = await this.finalAccountsReports.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(0);
    }
}

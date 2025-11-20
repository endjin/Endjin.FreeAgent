// <copyright file="SelfAssessmentReturnsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class SelfAssessmentReturnsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private SelfAssessmentReturns selfAssessmentReturns = null!;
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
        this.selfAssessmentReturns = new SelfAssessmentReturns(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllReturnsForUser()
    {
        // Arrange
        List<SelfAssessmentReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/users/123/self_assessment_returns/2024-04-05"),
                PeriodStartsOn = new DateOnly(2023, 4, 6),
                PeriodEndsOn = new DateOnly(2024, 4, 5),
                FilingStatus = "unfiled",
                FilingDueOn = new DateOnly(2025, 1, 31),
                Payments =
                [
                    new()
                    {
                        Label = "Balancing Payment",
                        DueOn = new DateOnly(2025, 1, 31),
                        AmountDue = 1500.00m,
                        Status = "unpaid"
                    },
                    new()
                    {
                        Label = "First Payment on Account",
                        DueOn = new DateOnly(2025, 1, 31),
                        AmountDue = 750.00m,
                        Status = "unpaid"
                    }
                ]
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/users/123/self_assessment_returns/2025-04-05"),
                PeriodStartsOn = new DateOnly(2024, 4, 6),
                PeriodEndsOn = new DateOnly(2025, 4, 5),
                FilingStatus = "unfiled",
                FilingDueOn = new DateOnly(2026, 1, 31),
                Payments = []
            }
        ];

        SelfAssessmentReturnsRoot responseRoot = new() { SelfAssessmentReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<SelfAssessmentReturn> result = await this.selfAssessmentReturns.GetAllAsync("123");

        // Assert
        result.Count().ShouldBe(2);
        SelfAssessmentReturn firstReturn = result.First();
        firstReturn.Payments.ShouldNotBeNull();
        firstReturn.Payments.Count.ShouldBe(2);
        firstReturn.FilingStatus.ShouldBe("unfiled");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/123/self_assessment_returns");
    }

    [TestMethod]
    public async Task GetAllAsync_ThrowsWhenUserIdIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.selfAssessmentReturns.GetAllAsync(null!));
    }

    [TestMethod]
    public async Task GetByPeriodEndAsync_WithValidParameters_ReturnsReturn()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/123/self_assessment_returns/2024-04-05"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5),
            FilingStatus = "filed",
            FilingDueOn = new DateOnly(2025, 1, 31),
            FiledAt = new DateTime(2025, 1, 15, 10, 30, 0),
            FiledReference = "HMRC123456789",
            Payments =
            [
                new()
                {
                    Label = "Balancing Payment",
                    DueOn = new DateOnly(2025, 1, 31),
                    AmountDue = 1500.00m,
                    Status = "marked_as_paid"
                }
            ]
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.GetByPeriodEndAsync("123", new DateOnly(2024, 4, 5));

        // Assert
        result.ShouldNotBeNull();
        result.PeriodStartsOn.ShouldBe(new DateOnly(2023, 4, 6));
        result.FilingStatus.ShouldBe("filed");
        result.FiledReference.ShouldBe("HMRC123456789");
        result.Payments.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("marked_as_paid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/123/self_assessment_returns/2024-04-05");
    }

    [TestMethod]
    public async Task GetByPeriodEndAsync_ThrowsWhenUserIdIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.selfAssessmentReturns.GetByPeriodEndAsync(null!, new DateOnly(2024, 4, 5)));
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_MarksReturnAsFiled()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/456/self_assessment_returns/2024-04-05"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5),
            FilingStatus = "marked_as_filed",
            FilingDueOn = new DateOnly(2025, 1, 31),
            Payments = []
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.MarkAsFiledAsync("456", new DateOnly(2024, 4, 5));

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe("marked_as_filed");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/456/self_assessment_returns/2024-04-05/mark_as_filed");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_ThrowsWhenUserIdIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.selfAssessmentReturns.MarkAsFiledAsync(null!, new DateOnly(2024, 4, 5)));
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_MarksReturnAsUnfiled()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/456/self_assessment_returns/2024-04-05"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5),
            FilingStatus = "unfiled",
            FilingDueOn = new DateOnly(2025, 1, 31),
            Payments = []
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.MarkAsUnfiledAsync("456", new DateOnly(2024, 4, 5));

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe("unfiled");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/456/self_assessment_returns/2024-04-05/mark_as_unfiled");
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_ThrowsWhenUserIdIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.selfAssessmentReturns.MarkAsUnfiledAsync(null!, new DateOnly(2024, 4, 5)));
    }

    [TestMethod]
    public async Task MarkPaymentAsPaidAsync_MarksPaymentAsPaid()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/789/self_assessment_returns/2024-04-05"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5),
            FilingStatus = "filed",
            FilingDueOn = new DateOnly(2025, 1, 31),
            Payments =
            [
                new()
                {
                    Label = "Balancing Payment",
                    DueOn = new DateOnly(2025, 1, 31),
                    AmountDue = 1500.00m,
                    Status = "marked_as_paid"
                }
            ]
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.MarkPaymentAsPaidAsync(
            "789",
            new DateOnly(2024, 4, 5),
            new DateOnly(2025, 1, 31));

        // Assert
        result.ShouldNotBeNull();
        result.Payments.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("marked_as_paid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/789/self_assessment_returns/2024-04-05/payments/2025-01-31/mark_as_paid");
    }

    [TestMethod]
    public async Task MarkPaymentAsPaidAsync_ThrowsWhenUserIdIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.selfAssessmentReturns.MarkPaymentAsPaidAsync(null!, new DateOnly(2024, 4, 5), new DateOnly(2025, 1, 31)));
    }

    [TestMethod]
    public async Task MarkPaymentAsUnpaidAsync_MarksPaymentAsUnpaid()
    {
        // Arrange
        SelfAssessmentReturn taxReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/789/self_assessment_returns/2024-04-05"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5),
            FilingStatus = "filed",
            FilingDueOn = new DateOnly(2025, 1, 31),
            Payments =
            [
                new()
                {
                    Label = "Balancing Payment",
                    DueOn = new DateOnly(2025, 1, 31),
                    AmountDue = 1500.00m,
                    Status = "unpaid"
                }
            ]
        };

        SelfAssessmentReturnRoot responseRoot = new() { SelfAssessmentReturn = taxReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        SelfAssessmentReturn result = await this.selfAssessmentReturns.MarkPaymentAsUnpaidAsync(
            "789",
            new DateOnly(2024, 4, 5),
            new DateOnly(2025, 1, 31));

        // Assert
        result.ShouldNotBeNull();
        result.Payments.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("unpaid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/users/789/self_assessment_returns/2024-04-05/payments/2025-01-31/mark_as_unpaid");
    }

    [TestMethod]
    public async Task MarkPaymentAsUnpaidAsync_ThrowsWhenUserIdIsNull()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.selfAssessmentReturns.MarkPaymentAsUnpaidAsync(null!, new DateOnly(2024, 4, 5), new DateOnly(2025, 1, 31)));
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<SelfAssessmentReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/users/123/self_assessment_returns/2024-04-05"),
                PeriodStartsOn = new DateOnly(2023, 4, 6),
                PeriodEndsOn = new DateOnly(2024, 4, 5),
                FilingStatus = "unfiled",
                FilingDueOn = new DateOnly(2025, 1, 31),
                Payments = []
            }
        ];

        SelfAssessmentReturnsRoot responseRoot = new() { SelfAssessmentReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - call twice
        await this.selfAssessmentReturns.GetAllAsync("123");
        await this.selfAssessmentReturns.GetAllAsync("123");

        // Assert - should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_InvalidatesCache()
    {
        // Arrange
        List<SelfAssessmentReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/users/123/self_assessment_returns/2024-04-05"),
                PeriodStartsOn = new DateOnly(2023, 4, 6),
                PeriodEndsOn = new DateOnly(2024, 4, 5),
                FilingStatus = "unfiled",
                FilingDueOn = new DateOnly(2025, 1, 31),
                Payments = []
            }
        ];

        SelfAssessmentReturnsRoot listResponseRoot = new() { SelfAssessmentReturns = returnsList };
        string listResponseJson = JsonSerializer.Serialize(listResponseRoot, SharedJsonOptions.Instance);

        // First call to populate cache
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(listResponseJson, Encoding.UTF8, "application/json")
        };
        await this.selfAssessmentReturns.GetAllAsync("123");

        // Setup mark as filed response
        SelfAssessmentReturn filedReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/users/123/self_assessment_returns/2024-04-05"),
            PeriodStartsOn = new DateOnly(2023, 4, 6),
            PeriodEndsOn = new DateOnly(2024, 4, 5),
            FilingStatus = "marked_as_filed",
            FilingDueOn = new DateOnly(2025, 1, 31),
            Payments = []
        };
        SelfAssessmentReturnRoot filedResponseRoot = new() { SelfAssessmentReturn = filedReturn };
        string filedResponseJson = JsonSerializer.Serialize(filedResponseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(filedResponseJson, Encoding.UTF8, "application/json")
        };

        // Act - mark as filed should invalidate cache
        await this.selfAssessmentReturns.MarkAsFiledAsync("123", new DateOnly(2024, 4, 5));

        // Setup new list response for after cache invalidation
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(listResponseJson, Encoding.UTF8, "application/json")
        };

        // Call GetAllAsync again - should hit API since cache was invalidated
        await this.selfAssessmentReturns.GetAllAsync("123");

        // Assert - should have called API 3 times (initial list, mark as filed, list after invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }
}

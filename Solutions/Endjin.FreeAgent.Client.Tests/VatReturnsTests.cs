// <copyright file="VatReturnsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class VatReturnsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private VatReturns vatReturns = null!;
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
        this.vatReturns = new VatReturns(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllVatReturns()
    {
        // Arrange
        List<VatReturn> returnsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/vat_returns/2024-03-31"),
                PeriodStartsOn = new DateOnly(2024, 1, 1),
                PeriodEndsOn = new DateOnly(2024, 3, 31),
                FilingDueOn = new DateOnly(2024, 5, 7),
                FilingStatus = "unfiled",
                Payments =
                [
                    new VatReturnPayment
                    {
                        Label = "VAT Payment",
                        DueOn = new DateOnly(2024, 5, 7),
                        AmountDue = 17500.00m,
                        Status = "unpaid"
                    }
                ]
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/vat_returns/2023-12-31"),
                PeriodStartsOn = new DateOnly(2023, 10, 1),
                PeriodEndsOn = new DateOnly(2023, 12, 31),
                FilingDueOn = new DateOnly(2024, 2, 7),
                FilingStatus = "filed",
                FiledAt = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero),
                FiledReference = "123456789",
                Payments =
                [
                    new VatReturnPayment
                    {
                        Label = "VAT Payment",
                        DueOn = new DateOnly(2024, 2, 7),
                        AmountDue = 12500.00m,
                        Status = "marked_as_paid"
                    }
                ]
            }
        ];

        VatReturnsRoot responseRoot = new() { VatReturns = returnsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<VatReturn> result = await this.vatReturns.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.First().FilingStatus.ShouldBe("unfiled");
        result.Last().FilingStatus.ShouldBe("filed");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsVatReturn()
    {
        // Arrange
        VatReturn vatReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2024, 1, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31),
            FilingDueOn = new DateOnly(2024, 5, 7),
            FilingStatus = "unfiled",
            Payments =
            [
                new VatReturnPayment
                {
                    Label = "VAT Payment",
                    DueOn = new DateOnly(2024, 5, 7),
                    AmountDue = 17000.00m,
                    Status = "unpaid"
                }
            ]
        };

        VatReturnRoot responseRoot = new() { VatReturn = vatReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.GetByIdAsync("2024-03-31");

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe("unfiled");
        result.Payments.ShouldNotBeNull();
        result.Payments.Count.ShouldBe(1);
        result.Payments[0].AmountDue.ShouldBe(17000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/2024-03-31");
    }

    [TestMethod]
    public async Task MarkAsFiledAsync_WithValidPeriod_UpdatesVatReturnAsFiled()
    {
        // Arrange
        VatReturn responseReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2024, 1, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31),
            FilingDueOn = new DateOnly(2024, 5, 7),
            FilingStatus = "marked_as_filed",
            Payments =
            [
                new VatReturnPayment
                {
                    Label = "VAT Payment",
                    DueOn = new DateOnly(2024, 5, 7),
                    AmountDue = 17000.00m,
                    Status = "unpaid"
                }
            ]
        };

        VatReturnRoot responseRoot = new() { VatReturn = responseReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.MarkAsFiledAsync("2024-03-31");

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe("marked_as_filed");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/2024-03-31/mark_as_filed");
    }

    [TestMethod]
    public async Task MarkAsUnfiledAsync_WithValidPeriod_UpdatesVatReturnAsUnfiled()
    {
        // Arrange
        VatReturn responseReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2024, 1, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31),
            FilingDueOn = new DateOnly(2024, 5, 7),
            FilingStatus = "unfiled",
            Payments =
            [
                new VatReturnPayment
                {
                    Label = "VAT Payment",
                    DueOn = new DateOnly(2024, 5, 7),
                    AmountDue = 17000.00m,
                    Status = "unpaid"
                }
            ]
        };

        VatReturnRoot responseRoot = new() { VatReturn = responseReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.MarkAsUnfiledAsync("2024-03-31");

        // Assert
        result.ShouldNotBeNull();
        result.FilingStatus.ShouldBe("unfiled");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/2024-03-31/mark_as_unfiled");
    }

    [TestMethod]
    public async Task MarkPaymentAsPaidAsync_WithValidDetails_UpdatesPaymentStatus()
    {
        // Arrange
        VatReturn responseReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2024, 1, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31),
            FilingDueOn = new DateOnly(2024, 5, 7),
            FilingStatus = "filed",
            FiledAt = new DateTimeOffset(2024, 4, 15, 10, 30, 0, TimeSpan.Zero),
            FiledReference = "123456789",
            Payments =
            [
                new VatReturnPayment
                {
                    Label = "VAT Payment",
                    DueOn = new DateOnly(2024, 5, 7),
                    AmountDue = 17000.00m,
                    Status = "marked_as_paid"
                }
            ]
        };

        VatReturnRoot responseRoot = new() { VatReturn = responseReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.MarkPaymentAsPaidAsync("2024-03-31", "2024-05-07");

        // Assert
        result.ShouldNotBeNull();
        result.Payments.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("marked_as_paid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/2024-03-31/payments/2024-05-07/mark_as_paid");
    }

    [TestMethod]
    public async Task MarkPaymentAsUnpaidAsync_WithValidDetails_UpdatesPaymentStatus()
    {
        // Arrange
        VatReturn responseReturn = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/vat_returns/2024-03-31"),
            PeriodStartsOn = new DateOnly(2024, 1, 1),
            PeriodEndsOn = new DateOnly(2024, 3, 31),
            FilingDueOn = new DateOnly(2024, 5, 7),
            FilingStatus = "filed",
            FiledAt = new DateTimeOffset(2024, 4, 15, 10, 30, 0, TimeSpan.Zero),
            FiledReference = "123456789",
            Payments =
            [
                new VatReturnPayment
                {
                    Label = "VAT Payment",
                    DueOn = new DateOnly(2024, 5, 7),
                    AmountDue = 17000.00m,
                    Status = "unpaid"
                }
            ]
        };

        VatReturnRoot responseRoot = new() { VatReturn = responseReturn };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        VatReturn result = await this.vatReturns.MarkPaymentAsUnpaidAsync("2024-03-31", "2024-05-07");

        // Assert
        result.ShouldNotBeNull();
        result.Payments.ShouldNotBeNull();
        result.Payments[0].Status.ShouldBe("unpaid");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/vat_returns/2024-03-31/payments/2024-05-07/mark_as_unpaid");
    }
}

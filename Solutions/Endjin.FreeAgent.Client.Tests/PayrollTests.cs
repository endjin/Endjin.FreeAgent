// <copyright file="PayrollTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class PayrollTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Payroll payroll = null!;
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
        this.payroll = new Payroll(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidPayment_ReturnsCreatedPayment()
    {
        // Arrange
        PayrollPayment inputPayment = new()
        {
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            DatedOn = new DateOnly(2024, 1, 31),
            GrossPay = 3000.00m,
            IncomeTax = 450.00m,
            EmployeeNi = 250.00m,
            NetPay = 2300.00m
        };

        PayrollPayment responsePayment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/payroll_payments/456"),
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            DatedOn = new DateOnly(2024, 1, 31),
            GrossPay = 3000.00m,
            IncomeTax = 450.00m,
            EmployeeNi = 250.00m,
            NetPay = 2300.00m
        };

        PayrollPaymentRoot responseRoot = new() { PayrollPayment = responsePayment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollPayment result = await this.payroll.CreateAsync(inputPayment);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.GrossPay.ShouldBe(3000.00m);
        result.NetPay.ShouldBe(2300.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments");
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutFilters_ReturnsAllPayments()
    {
        // Arrange
        List<PayrollPayment> paymentsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll_payments/1"),
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                DatedOn = new DateOnly(2024, 1, 31),
                GrossPay = 3000.00m,
                NetPay = 2300.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll_payments/2"),
                User = new Uri("https://api.freeagent.com/v2/users/124"),
                DatedOn = new DateOnly(2024, 2, 28),
                GrossPay = 3500.00m,
                NetPay = 2700.00m
            }
        ];

        PayrollPaymentsRoot responseRoot = new() { PayrollPayments = paymentsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollPayment> result = await this.payroll.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUserFilter_ReturnsFilteredPayments()
    {
        // Arrange
        List<PayrollPayment> paymentsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll_payments/1"),
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                DatedOn = new DateOnly(2024, 1, 31),
                GrossPay = 3000.00m
            }
        ];

        PayrollPaymentsRoot responseRoot = new() { PayrollPayments = paymentsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollPayment> result = await this.payroll.GetAllAsync(userId: "123");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments?user=123");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateFilters_ReturnsFilteredPayments()
    {
        // Arrange
        List<PayrollPayment> paymentsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll_payments/1"),
                DatedOn = new DateOnly(2024, 1, 31),
                GrossPay = 3000.00m
            }
        ];

        PayrollPaymentsRoot responseRoot = new() { PayrollPayments = paymentsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollPayment> result = await this.payroll.GetAllAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 1, 31));

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments?from_date=2024-01-01&to_date=2024-01-31");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsPayment()
    {
        // Arrange
        PayrollPayment payment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/payroll_payments/789"),
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            DatedOn = new DateOnly(2024, 1, 31),
            GrossPay = 3000.00m,
            IncomeTax = 450.00m,
            EmployeeNi = 250.00m,
            NetPay = 2300.00m
        };

        PayrollPaymentRoot responseRoot = new() { PayrollPayment = payment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollPayment result = await this.payroll.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.GrossPay.ShouldBe(3000.00m);
        result.NetPay.ShouldBe(2300.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments/789");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidPayment_ReturnsUpdatedPayment()
    {
        // Arrange
        PayrollPayment updatedPayment = new()
        {
            DatedOn = new DateOnly(2024, 1, 31),
            GrossPay = 3200.00m,
            IncomeTax = 480.00m,
            EmployeeNi = 260.00m,
            NetPay = 2460.00m
        };

        PayrollPayment responsePayment = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/payroll_payments/999"),
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            DatedOn = new DateOnly(2024, 1, 31),
            GrossPay = 3200.00m,
            IncomeTax = 480.00m,
            EmployeeNi = 260.00m,
            NetPay = 2460.00m
        };

        PayrollPaymentRoot responseRoot = new() { PayrollPayment = responsePayment };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollPayment result = await this.payroll.UpdateAsync("999", updatedPayment);

        // Assert
        result.ShouldNotBeNull();
        result.GrossPay.ShouldBe(3200.00m);
        result.NetPay.ShouldBe(2460.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments/999");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesPayment()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.payroll.DeleteAsync("888");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_payments/888");
    }
}

// <copyright file="PayslipsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class PayslipsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Payslips payslips = null!;
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
        this.payslips = new Payslips(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutFilters_ReturnsAllPayslips()
    {
        // Arrange
        List<Payslip> payslipsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payslips/1"),
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                DatedOn = new DateOnly(2024, 1, 31),
                GrossSalary = 3000.00m,
                NetSalary = 2300.00m,
                IncomeTax = 450.00m,
                EmployeeNic = 250.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payslips/2"),
                User = new Uri("https://api.freeagent.com/v2/users/124"),
                DatedOn = new DateOnly(2024, 2, 28),
                GrossSalary = 3500.00m,
                NetSalary = 2700.00m,
                IncomeTax = 550.00m,
                EmployeeNic = 250.00m
            }
        ];

        PayslipsRoot responseRoot = new() { Payslips = payslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Payslip> result = await this.payslips.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payslips");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateFilters_ReturnsFilteredPayslips()
    {
        // Arrange
        List<Payslip> payslipsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payslips/1"),
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                DatedOn = new DateOnly(2024, 1, 31),
                GrossSalary = 3000.00m,
                NetSalary = 2300.00m
            }
        ];

        PayslipsRoot responseRoot = new() { Payslips = payslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Payslip> result = await this.payslips.GetAllAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 1, 31));

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payslips?from_date=2024-01-01&to_date=2024-01-31");
    }

    [TestMethod]
    public async Task GetByUserAsync_WithoutDateFilters_ReturnsUserPayslips()
    {
        // Arrange
        Uri userUrl = new("https://api.freeagent.com/v2/users/123");
        List<Payslip> payslipsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payslips/1"),
                User = userUrl,
                DatedOn = new DateOnly(2024, 1, 31),
                GrossSalary = 3000.00m,
                NetSalary = 2300.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payslips/2"),
                User = userUrl,
                DatedOn = new DateOnly(2024, 2, 28),
                GrossSalary = 3000.00m,
                NetSalary = 2300.00m
            }
        ];

        PayslipsRoot responseRoot = new() { Payslips = payslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Payslip> result = await this.payslips.GetByUserAsync(userUrl);

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByUserAsync_WithDateFilters_ReturnsFilteredUserPayslips()
    {
        // Arrange
        Uri userUrl = new("https://api.freeagent.com/v2/users/123");
        List<Payslip> payslipsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payslips/1"),
                User = userUrl,
                DatedOn = new DateOnly(2024, 1, 31),
                GrossSalary = 3000.00m,
                NetSalary = 2300.00m,
                IncomeTax = 450.00m,
                EmployeeNic = 250.00m,
                EmployeePension = 150.00m
            }
        ];

        PayslipsRoot responseRoot = new() { Payslips = payslipsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Payslip> result = await this.payslips.GetByUserAsync(
            userUrl,
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 1, 31));

        // Assert
        result.Count().ShouldBe(1);
        Payslip payslip = result.First();
        payslip.GrossSalary.ShouldBe(3000.00m);
        payslip.NetSalary.ShouldBe(2300.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }
}

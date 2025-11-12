// <copyright file="EstimatesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class EstimatesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Estimates estimates = null!;
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

        // Use proper initialization with TestOAuth2Service
        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);

        this.estimates = new Estimates(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllEstimates()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST001", Status = "Draft", TotalValue = 1000.00m },
            new() { Reference = "EST002", Status = "Sent", TotalValue = 2500.00m },
            new() { Reference = "EST003", Status = "Approved", TotalValue = 5000.00m }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllAsync();

        // Assert
        result.Count().ShouldBe(3);
        result.Any(e => e.Reference == "EST001").ShouldBeTrue();
        result.Any(e => e.Reference == "EST002").ShouldBeTrue();
        result.Any(e => e.Reference == "EST003").ShouldBeTrue();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithDraftStatus_ReturnsOnlyDraftEstimates()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST001", Status = "Draft", TotalValue = 1000.00m },
            new() { Reference = "EST004", Status = "Draft", TotalValue = 1500.00m }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByStatusAsync("draft");

        // Assert
        result.Count().ShouldBe(2);
        result.All(e => e.Status == "Draft").ShouldBeTrue();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "draft");
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithSentStatus_ReturnsOnlySentEstimates()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST002", Status = "Sent", TotalValue = 2500.00m },
            new() { Reference = "EST005", Status = "Sent", TotalValue = 3500.00m }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByStatusAsync("sent");

        // Assert
        result.Count().ShouldBe(2);
        result.All(e => e.Status == "Sent").ShouldBeTrue();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "sent");
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithApprovedStatus_ReturnsOnlyApprovedEstimates()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST003", Status = "Approved", TotalValue = 5000.00m }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByStatusAsync("approved");

        // Assert
        result.Count().ShouldBe(1);
        result.First().Status.ShouldBe("Approved");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "approved");
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST001", Status = "Draft" }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<Estimate> result1 = await this.estimates.GetAllByStatusAsync("draft");

        // Act - Second call (should use cache)
        IEnumerable<Estimate> result2 = await this.estimates.GetAllByStatusAsync("draft");

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);
        result2.ShouldBe(result1); // Should be same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "draft");
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsEstimate()
    {
        // Arrange
        string estimateId = "12345";
        Estimate responseEstimate = new()
        {
            Reference = "EST001",
            Status = "Sent",
            TotalValue = 2500.00m,
            Url = new Uri($"https://api.freeagent.com/v2/estimates/{estimateId}")
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.GetByIdAsync(estimateId);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("EST001");
        result.Status.ShouldBe("Sent");
        result.TotalValue.ShouldBe(2500.00m);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/estimates/{estimateId}");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Estimate not found", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.estimates.GetByIdAsync("invalid-id"));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/invalid-id");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithCaching_UsesCachedResults()
    {
        // Arrange
        string estimateId = "12345";
        Estimate responseEstimate = new()
        {
            Reference = "EST001",
            Status = "Sent",
            TotalValue = 2500.00m,
            Url = new Uri($"https://api.freeagent.com/v2/estimates/{estimateId}")
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        Estimate result1 = await this.estimates.GetByIdAsync(estimateId);

        // Act - Second call (should use cache)
        Estimate result2 = await this.estimates.GetByIdAsync(estimateId);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result2.ShouldBe(result1); // Should be same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/estimates/{estimateId}");
    }

    [TestMethod]
    public async Task GetProjectedMonthlyRevenue_ReturnsEmptyWhenNoEstimates()
    {
        // Arrange - Set up empty estimates response
        EstimatesRoot responseRoot = new() { Estimates = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>> result = await this.estimates.GetProjectedMonthlyRevenue();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(0); // No data since no estimates exist
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithRejectedStatus_ReturnsRejectedEstimates()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST006", Status = "Rejected", TotalValue = 1800.00m }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByStatusAsync("rejected");

        // Assert
        result.Count().ShouldBe(1);
        result.First().Status.ShouldBe("Rejected");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "rejected");
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetAllByStatusAsync_WithInvoicedStatus_ReturnsInvoicedEstimates()
    {
        // Arrange
        List<Estimate> estimatesList =
        [
            new() { Reference = "EST007", Status = "Invoiced", TotalValue = 10000.00m, Invoice = new Uri("https://api.freeagent.com/v2/invoices/INV001") }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByStatusAsync("invoiced");

        // Assert
        result.Count().ShouldBe(1);
        result.First().Status.ShouldBe("Invoiced");
        result.First().Invoice.ShouldNotBeNull();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "invoiced");
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task GetAllAsync_WithEmptyResponse_ReturnsEmptyList()
    {
        // Arrange
        EstimatesRoot responseRoot = new() { Estimates = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(0);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}

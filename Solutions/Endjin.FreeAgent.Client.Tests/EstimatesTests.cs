// <copyright file="EstimatesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;

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

    [TestMethod]
    public async Task CreateAsync_WithValidEstimate_ReturnsCreatedEstimate()
    {
        // Arrange
        Estimate inputEstimate = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Project = new Uri("https://api.freeagent.com/v2/projects/456"),
            Reference = "EST-001",
            Currency = "GBP",
            EstimateItems = [new() { Description = "Development Work", Quantity = 10m, Price = 100.00m }]
        };

        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/789"),
            Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
            Project = new Uri("https://api.freeagent.com/v2/projects/456"),
            Reference = "EST-001",
            Currency = "GBP",
            Status = "Draft"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.CreateAsync(inputEstimate);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Reference.ShouldBe("EST-001");
        result.Status.ShouldBe("Draft");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidEstimate_ReturnsUpdatedEstimate()
    {
        // Arrange
        Estimate updatedEstimate = new()
        {
            Reference = "EST-001-UPDATED",
            Currency = "GBP"
        };

        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/123"),
            Reference = "EST-001-UPDATED",
            Status = "Draft",
            Currency = "GBP"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.UpdateAsync("123", updatedEstimate);

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("EST-001-UPDATED");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/123");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesEstimate()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.estimates.DeleteAsync("456");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/456");
    }

    [TestMethod]
    public async Task MarkAsSentAsync_WithValidId_ReturnsEstimateWithSentStatus()
    {
        // Arrange
        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/111"),
            Reference = "EST-SENT",
            Status = "Sent"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.MarkAsSentAsync("111");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Sent");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/111/transitions/mark_as_sent");
    }

    [TestMethod]
    public async Task MarkAsDraftAsync_WithValidId_ReturnsEstimateWithDraftStatus()
    {
        // Arrange
        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/222"),
            Reference = "EST-DRAFT",
            Status = "Draft"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.MarkAsDraftAsync("222");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Draft");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/222/transitions/mark_as_draft");
    }

    [TestMethod]
    public async Task MarkAsApprovedAsync_WithValidId_ReturnsEstimateWithApprovedStatus()
    {
        // Arrange
        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/333"),
            Reference = "EST-APPROVED",
            Status = "Approved"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.MarkAsApprovedAsync("333");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Approved");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/333/transitions/mark_as_approved");
    }

    [TestMethod]
    public async Task MarkAsRejectedAsync_WithValidId_ReturnsEstimateWithRejectedStatus()
    {
        // Arrange
        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/444"),
            Reference = "EST-REJECTED",
            Status = "Rejected"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.MarkAsRejectedAsync("444");

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Rejected");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/444/transitions/mark_as_rejected");
    }

    [TestMethod]
    public async Task ConvertToInvoiceAsync_WithValidId_ReturnsUpdatedEstimate()
    {
        // Arrange
        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/555"),
            Reference = "EST-001",
            Status = "Invoiced",
            Invoice = new Uri("https://api.freeagent.com/v2/invoices/999")
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.ConvertToInvoiceAsync("555");

        // Assert
        result.ShouldNotBeNull();
        result.Reference.ShouldBe("EST-001");
        result.Status.ShouldBe("Invoiced");
        result.Invoice.ShouldNotBeNull();
        result.Invoice!.ToString().ShouldBe("https://api.freeagent.com/v2/invoices/999");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/555/transitions/convert_to_invoice");
    }

    [TestMethod]
    public async Task GetPdfAsync_WithValidId_ReturnsPdfData()
    {
        // Arrange
        byte[] expectedPdfData = [0x25, 0x50, 0x44, 0x46]; // PDF file signature
        string base64Content = Convert.ToBase64String(expectedPdfData);

        var pdfResponse = new
        {
            pdf = new
            {
                content = base64Content
            }
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                JsonSerializer.Serialize(pdfResponse),
                Encoding.UTF8,
                "application/json")
        };

        // Act
        byte[] result = await this.estimates.GetPdfAsync("666");

        // Assert
        result.ShouldNotBeNull();
        result.Length.ShouldBe(expectedPdfData.Length);
        result.ShouldBe(expectedPdfData);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/666/pdf");
    }

    [TestMethod]
    public async Task DuplicateAsync_WithValidId_ReturnsDuplicatedEstimate()
    {
        // Arrange
        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/888"),
            Reference = "EST-COPY",
            Status = "Draft"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.DuplicateAsync("777");

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Status.ShouldBe("Draft");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/777/duplicate");
    }

    [TestMethod]
    public async Task SendEmailAsync_WithValidEmail_ReturnsEstimate()
    {
        // Arrange
        EstimateEmail email = new()
        {
            To = "client@example.com",
            Subject = "Your Quote",
            Body = "Please review the attached quote."
        };

        Estimate responseEstimate = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/999"),
            Reference = "EST-EMAILED",
            Status = "Sent"
        };

        EstimateRoot responseRoot = new() { Estimate = responseEstimate };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Estimate result = await this.estimates.SendEmailAsync("999", email);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe("Sent");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/estimates/999/send_email");
    }

    [TestMethod]
    public async Task GetAllByContactAsync_WithValidContactUrl_ReturnsFilteredEstimates()
    {
        // Arrange
        Uri contactUrl = new("https://api.freeagent.com/v2/contacts/123");
        List<Estimate> estimatesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/estimates/1"),
                Contact = contactUrl,
                Reference = "EST-001",
                Status = "Draft"
            }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByContactAsync(contactUrl);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Contact.ShouldBe(contactUrl);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllByProjectAsync_WithValidProjectUrl_ReturnsFilteredEstimates()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/456");
        List<Estimate> estimatesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/estimates/2"),
                Project = projectUrl,
                Reference = "EST-002",
                Status = "Sent"
            }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByProjectAsync(projectUrl);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Project.ShouldBe(projectUrl);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllByInvoiceAsync_WithValidInvoiceUrl_ReturnsFilteredEstimates()
    {
        // Arrange
        Uri invoiceUrl = new("https://api.freeagent.com/v2/invoices/789");
        List<Estimate> estimatesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/estimates/3"),
                Invoice = invoiceUrl,
                Reference = "EST-003",
                Status = "Invoiced"
            }
        ];

        EstimatesRoot responseRoot = new() { Estimates = estimatesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Estimate> result = await this.estimates.GetAllByInvoiceAsync(invoiceUrl);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Invoice.ShouldBe(invoiceUrl);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }


    [TestMethod]
    public async Task GetProjectedMonthlyRevenue_IncludesEveryEstimateForTheSameProject()
    {
        // Arrange: two approved estimates for one project. Project is a record, so both resolve to an
        // equal dictionary key; before the fix for issue #75 the second estimate was silently dropped.
        Uri contactUrl = new("https://api.freeagent.com/v2/contacts/1");
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/1");
        Contact contact = new() { Url = contactUrl, OrganisationName = "Acme Ltd" };
        Project project = new() { Url = projectUrl, Name = "Platform build", Contact = contactUrl, Status = "Active", Currency = "GBP", BudgetUnits = "Hours" };

        Estimate first = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/101"),
            Reference = "EST101",
            Status = "Approved",
            Contact = contactUrl,
            Project = projectUrl,
            EstimateItems = [new() { Description = "Sprint 1 - January 2026", Price = 1000m, Quantity = 1m }]
        };

        Estimate second = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/102"),
            Reference = "EST102",
            Status = "Approved",
            Contact = contactUrl,
            Project = projectUrl,
            EstimateItems =
            [
                new() { Description = "Sprint 2 - January 2026", Price = 2000m, Quantity = 1m },
                new() { Description = "Sprint 3 - February 2026", Price = 500m, Quantity = 2m }
            ]
        };

        this.messageHandler.SetupResponse("v2/contacts?view=active_projects", JsonResponse(new ContactsRoot { Contacts = [contact] }));
        this.messageHandler.SetupResponse("v2/projects?view=active", JsonResponse(new ProjectsRoot { Projects = [project] }));
        this.messageHandler.SetupResponse("v2/estimates?view=approved", JsonResponse(new EstimatesRoot { Estimates = [first, second] }));
        this.messageHandler.SetupResponse("v2/estimates?view=draft", JsonResponse(new EstimatesRoot { Estimates = [] }));
        this.messageHandler.SetupResponse("v2/estimates/101", JsonResponse(new EstimateRoot { Estimate = first }));
        this.messageHandler.SetupResponse("v2/estimates/102", JsonResponse(new EstimateRoot { Estimate = second }));

        // Act
        Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>> revenue = await this.estimates.GetProjectedMonthlyRevenue();

        // Assert
        revenue.Keys.ShouldBe([new DateTime(2026, 1, 1), new DateTime(2026, 2, 1)], ignoreOrder: true);

        List<(Contact contact, Project project, decimal price)> january = revenue[new DateTime(2026, 1, 1)];
        january.Count.ShouldBe(2);
        january.Sum(x => x.price).ShouldBe(3000m);
        january.ShouldAllBe(x => x.project.Url == projectUrl && x.contact.Url == contactUrl);

        List<(Contact contact, Project project, decimal price)> february = revenue[new DateTime(2026, 2, 1)];
        february.Count.ShouldBe(1);
        february[0].price.ShouldBe(1000m);
    }

    [TestMethod]
    public async Task GetProjectedMonthlyRevenue_LogsAndSkipsAnEstimateThatFailsToLoad()
    {
        // Arrange: the second estimate's detail request fails. It must be logged as an error and
        // excluded, while the first estimate still contributes to the projection.
        ILogger logger = Substitute.For<ILogger>();
        this.loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);
        Estimates estimatesWithLogger = new(this.freeAgentClient, this.cache);

        Uri contactUrl = new("https://api.freeagent.com/v2/contacts/1");
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/1");
        Contact contact = new() { Url = contactUrl, OrganisationName = "Acme Ltd" };
        Project project = new() { Url = projectUrl, Name = "Platform build", Contact = contactUrl, Status = "Active", Currency = "GBP", BudgetUnits = "Hours" };

        Estimate loads = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/201"),
            Reference = "EST201",
            Status = "Approved",
            Contact = contactUrl,
            Project = projectUrl,
            EstimateItems = [new() { Description = "Discovery - March 2026", Price = 750m, Quantity = 1m }]
        };

        Estimate fails = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/202"),
            Reference = "EST202",
            Status = "Approved",
            Contact = contactUrl,
            Project = projectUrl
        };

        this.messageHandler.SetupResponse("v2/contacts?view=active_projects", JsonResponse(new ContactsRoot { Contacts = [contact] }));
        this.messageHandler.SetupResponse("v2/projects?view=active", JsonResponse(new ProjectsRoot { Projects = [project] }));
        this.messageHandler.SetupResponse("v2/estimates?view=approved", JsonResponse(new EstimatesRoot { Estimates = [loads, fails] }));
        this.messageHandler.SetupResponse("v2/estimates?view=draft", JsonResponse(new EstimatesRoot { Estimates = [] }));
        this.messageHandler.SetupResponse("v2/estimates/201", JsonResponse(new EstimateRoot { Estimate = loads }));
        this.messageHandler.SetupResponse("v2/estimates/202", new HttpResponseMessage(HttpStatusCode.InternalServerError));

        // Act
        Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>> revenue = await estimatesWithLogger.GetProjectedMonthlyRevenue();

        // Assert
        revenue.Keys.ShouldBe([new DateTime(2026, 3, 1)]);
        revenue[new DateTime(2026, 3, 1)].Single().price.ShouldBe(750m);

        bool loggedTheFailure = logger.ReceivedCalls().Any(call =>
            call.GetMethodInfo().Name == nameof(ILogger.Log)
            && (LogLevel)call.GetArguments()[0]! == LogLevel.Error
            && call.GetArguments()[3] is HttpRequestException);

        loggedTheFailure.ShouldBeTrue("Expected the failed estimate request to be logged at Error level with the HttpRequestException.");
    }

    [TestMethod]
    public async Task GetProjectedMonthlyRevenue_LogsAndSkipsAnEstimateWhoseRequestTimesOut()
    {
        // Arrange: a real HttpClient timeout, which surfaces as a TaskCanceledException wrapping a
        // TimeoutException rather than an HttpRequestException. The timed-out estimate must be
        // logged and excluded, and the other estimate still contributes.
        ILogger logger = Substitute.For<ILogger>();
        this.loggerFactory.CreateLogger(Arg.Any<string>()).Returns(logger);

        HangingRequestHandler hangingHandler = new("v2/estimates/302") { InnerHandler = this.messageHandler };
        using HttpClient timingOutClient = new(hangingHandler) { Timeout = TimeSpan.FromMilliseconds(100) };
        IHttpClientFactory timingOutFactory = Substitute.For<IHttpClientFactory>();
        timingOutFactory.CreateClient(Arg.Any<string>()).Returns(timingOutClient);

        FreeAgentClient client = new(new FreeAgentOptionsBuilder().Build(), this.cache, timingOutFactory, this.loggerFactory);
        await TestHelper.SetupForTestingAsync(client, timingOutFactory);
        Estimates estimatesWithTimeout = new(client, this.cache);

        Uri contactUrl = new("https://api.freeagent.com/v2/contacts/1");
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/1");
        Contact contact = new() { Url = contactUrl, OrganisationName = "Acme Ltd" };
        Project project = new() { Url = projectUrl, Name = "Platform build", Contact = contactUrl, Status = "Active", Currency = "GBP", BudgetUnits = "Hours" };

        Estimate loads = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/301"),
            Reference = "EST301",
            Status = "Approved",
            Contact = contactUrl,
            Project = projectUrl,
            EstimateItems = [new() { Description = "Discovery - April 2026", Price = 900m, Quantity = 1m }]
        };

        Estimate timesOut = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/estimates/302"),
            Reference = "EST302",
            Status = "Approved",
            Contact = contactUrl,
            Project = projectUrl
        };

        this.messageHandler.SetupResponse("v2/contacts?view=active_projects", JsonResponse(new ContactsRoot { Contacts = [contact] }));
        this.messageHandler.SetupResponse("v2/projects?view=active", JsonResponse(new ProjectsRoot { Projects = [project] }));
        this.messageHandler.SetupResponse("v2/estimates?view=approved", JsonResponse(new EstimatesRoot { Estimates = [loads, timesOut] }));
        this.messageHandler.SetupResponse("v2/estimates?view=draft", JsonResponse(new EstimatesRoot { Estimates = [] }));
        this.messageHandler.SetupResponse("v2/estimates/301", JsonResponse(new EstimateRoot { Estimate = loads }));

        // Act
        Dictionary<DateTime, List<(Contact contact, Project project, decimal price)>> revenue = await estimatesWithTimeout.GetProjectedMonthlyRevenue();

        // Assert
        revenue.Keys.ShouldBe([new DateTime(2026, 4, 1)]);
        revenue[new DateTime(2026, 4, 1)].Single().price.ShouldBe(900m);

        bool loggedTheTimeout = logger.ReceivedCalls().Any(call =>
            call.GetMethodInfo().Name == nameof(ILogger.Log)
            && (LogLevel)call.GetArguments()[0]! == LogLevel.Error
            && call.GetArguments()[3] is TaskCanceledException { InnerException: TimeoutException });

        loggedTheTimeout.ShouldBeTrue("Expected the timed-out estimate request to be logged at Error level with the timeout exception.");
    }

    /// <summary>
    /// Never answers requests whose URI contains the given fragment, so that the HttpClient's own
    /// timeout fires; every other request is passed to the inner handler.
    /// </summary>
    private sealed class HangingRequestHandler(string uriFragment) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri?.ToString().Contains(uriFragment) == true)
            {
                await Task.Delay(Timeout.Infinite, cancellationToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }

    private static HttpResponseMessage JsonResponse<T>(T body)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(body, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
    }
}
// <copyright file="BankTransactionExplanationsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BankTransactionExplanationsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private BankTransactionExplanations bankTransactionExplanations = null!;
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
        this.bankTransactionExplanations = new BankTransactionExplanations(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidExplanation_ReturnsCreatedExplanation()
    {
        // Arrange
        BankTransactionExplanation inputExplanation = new()
        {
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/123"),
            GrossValue = 500.00m,
            Category = new Uri("https://api.freeagent.com/v2/categories/456")
        };

        BankTransactionExplanation responseExplanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/789"),
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/123"),
            GrossValue = 500.00m,
            Category = new Uri("https://api.freeagent.com/v2/categories/456"),
            DatedOn = new DateOnly(2024, 3, 15)
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = responseExplanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.CreateAsync(inputExplanation);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.GrossValue.ShouldBe(500.00m);
        result.BankTransaction.ShouldBe(new Uri("https://api.freeagent.com/v2/bank_transactions/123"));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transaction_explanations");
    }

    [TestMethod]
    public async Task CreateAsync_WithInvoiceExplanation_CreatesSuccessfully()
    {
        // Arrange
        BankTransactionExplanation inputExplanation = new()
        {
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/555"),
            PaidInvoice = new Uri("https://api.freeagent.com/v2/invoices/999"),
            GrossValue = 1200.00m
        };

        BankTransactionExplanation responseExplanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/111"),
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/555"),
            PaidInvoice = new Uri("https://api.freeagent.com/v2/invoices/999"),
            GrossValue = 1200.00m,
            Type = "Invoice Receipt"
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = responseExplanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.CreateAsync(inputExplanation);

        // Assert
        result.ShouldNotBeNull();
        result.PaidInvoice.ShouldBe(new Uri("https://api.freeagent.com/v2/invoices/999"));
        result.Type.ShouldBe("Invoice Receipt");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutFilter_ReturnsAllExplanations()
    {
        // Arrange
        List<BankTransactionExplanation> explanationsList =
        [
            new() { GrossValue = 100.00m, Description = "Explanation 1" },
            new() { GrossValue = 200.00m, Description = "Explanation 2" }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransactionExplanation> result = await this.bankTransactionExplanations.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(e => e.Description == "Explanation 1").ShouldBeTrue();
        result.Any(e => e.GrossValue == 200.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transaction_explanations");
    }

    [TestMethod]
    public async Task GetAllAsync_WithBankAccountFilter_ReturnsFilteredExplanations()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransactionExplanation> explanationsList =
        [
            new()
            {
                BankAccount = bankAccountUri,
                GrossValue = 150.00m,
                Description = "Account filtered explanation"
            }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransactionExplanation> result = await this.bankTransactionExplanations.GetAllAsync(bankAccountUri: bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().BankAccount.ShouldBe(bankAccountUri);
        result.First().Description.ShouldBe("Account filtered explanation");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithBankTransactionFilter_ReturnsFilteredExplanations()
    {
        // Arrange
        Uri bankTransactionUri = new("https://api.freeagent.com/v2/bank_transactions/321");
        List<BankTransactionExplanation> explanationsList =
        [
            new()
            {
                BankTransaction = bankTransactionUri,
                GrossValue = 150.00m,
                Description = "Filtered explanation"
            }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransactionExplanation> result = await this.bankTransactionExplanations.GetAllAsync(bankTransactionUri: bankTransactionUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().BankTransaction.ShouldBe(bankTransactionUri);
        result.First().Description.ShouldBe("Filtered explanation");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateRangeFilter_ReturnsFilteredExplanations()
    {
        // Arrange
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 3, 31);
        List<BankTransactionExplanation> explanationsList =
        [
            new()
            {
                DatedOn = new DateOnly(2024, 2, 15),
                GrossValue = 250.00m,
                Description = "Date filtered explanation"
            }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransactionExplanation> result = await this.bankTransactionExplanations.GetAllAsync(
            fromDate: fromDate,
            toDate: toDate);

        // Assert
        result.Count().ShouldBe(1);
        result.First().DatedOn.ShouldBe(new DateOnly(2024, 2, 15));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("from_date=2024-01-01");
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("to_date=2024-03-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedSinceFilter_ReturnsFilteredExplanations()
    {
        // Arrange
        DateTime updatedSince = new(2024, 3, 1, 10, 0, 0, DateTimeKind.Utc);
        List<BankTransactionExplanation> explanationsList =
        [
            new()
            {
                UpdatedAt = new DateTime(2024, 3, 15, 14, 30, 0, DateTimeKind.Utc),
                GrossValue = 180.00m,
                Description = "Recently updated explanation"
            }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransactionExplanation> result = await this.bankTransactionExplanations.GetAllAsync(
            updatedSince: updatedSince);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Recently updated explanation");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("updated_since=");
    }

    [TestMethod]
    public async Task GetAllAsync_WithMultipleFilters_ReturnsFilteredExplanations()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/456");
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 12, 31);

        List<BankTransactionExplanation> explanationsList =
        [
            new()
            {
                BankAccount = bankAccountUri,
                DatedOn = new DateOnly(2024, 6, 15),
                GrossValue = 500.00m,
                Description = "Multi-filter explanation"
            }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransactionExplanation> result = await this.bankTransactionExplanations.GetAllAsync(
            bankAccountUri: bankAccountUri,
            fromDate: fromDate,
            toDate: toDate);

        // Assert
        result.Count().ShouldBe(1);
        result.First().BankAccount.ShouldBe(bankAccountUri);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("bank_account=");
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("from_date=");
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("to_date=");
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<BankTransactionExplanation> explanationsList =
        [
            new() { GrossValue = 75.00m, Description = "Cached explanation" }
        ];

        BankTransactionExplanationsRoot responseRoot = new() { BankTransactionExplanations = explanationsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<BankTransactionExplanation> result1 = await this.bankTransactionExplanations.GetAllAsync();
        IEnumerable<BankTransactionExplanation> result2 = await this.bankTransactionExplanations.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsExplanation()
    {
        // Arrange
        BankTransactionExplanation explanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/444"),
            GrossValue = 350.00m,
            Description = "Specific explanation",
            DatedOn = new DateOnly(2024, 3, 20)
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = explanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.GetByIdAsync("444");

        // Assert
        result.ShouldNotBeNull();
        result.GrossValue.ShouldBe(350.00m);
        result.Description.ShouldBe("Specific explanation");
        result.DatedOn.ShouldBe(new DateOnly(2024, 3, 20));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transaction_explanations/444");
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        BankTransactionExplanation explanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/666"),
            GrossValue = 90.00m,
            Description = "Cached single explanation"
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = explanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        BankTransactionExplanation result1 = await this.bankTransactionExplanations.GetByIdAsync("666");
        BankTransactionExplanation result2 = await this.bankTransactionExplanations.GetByIdAsync("666");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Description.ShouldBe("Cached single explanation");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidExplanation_ReturnsUpdatedExplanation()
    {
        // Arrange
        BankTransactionExplanation updatedExplanation = new()
        {
            GrossValue = 275.00m,
            Description = "Updated explanation description"
        };

        BankTransactionExplanation responseExplanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/777"),
            GrossValue = 275.00m,
            Description = "Updated explanation description",
            DatedOn = new DateOnly(2024, 3, 25)
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = responseExplanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.UpdateAsync("777", updatedExplanation);

        // Assert
        result.ShouldNotBeNull();
        result.GrossValue.ShouldBe(275.00m);
        result.Description.ShouldBe("Updated explanation description");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transaction_explanations/777");
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCacheEntry()
    {
        // Arrange
        string explanationId = "888";
        BankTransactionExplanation originalExplanation = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/bank_transaction_explanations/{explanationId}"),
            GrossValue = 100.00m,
            Description = "Original"
        };

        BankTransactionExplanation updatedExplanation = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/bank_transaction_explanations/{explanationId}"),
            GrossValue = 200.00m,
            Description = "Updated"
        };

        BankTransactionExplanationRoot originalRoot = new() { BankTransactionExplanation = originalExplanation };
        BankTransactionExplanationRoot updatedRoot = new() { BankTransactionExplanation = updatedExplanation };

        // First call to GetByIdAsync
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(originalRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.bankTransactionExplanations.GetByIdAsync(explanationId);

        // Update call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };

        // Act
        await this.bankTransactionExplanations.UpdateAsync(explanationId, updatedExplanation);

        // Setup response for second GetByIdAsync (after cache invalidation)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        BankTransactionExplanation result = await this.bankTransactionExplanations.GetByIdAsync(explanationId);

        // Assert
        result.Description.ShouldBe("Updated");

        // Mock Verification - Should have made 3 calls (initial get, update, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesExplanation()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.bankTransactionExplanations.DeleteAsync("555");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transaction_explanations/555");
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesCacheEntry()
    {
        // Arrange
        string explanationId = "999";
        BankTransactionExplanation explanation = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/bank_transaction_explanations/{explanationId}"),
            GrossValue = 50.00m,
            Description = "To be deleted"
        };

        BankTransactionExplanationRoot root = new() { BankTransactionExplanation = explanation };

        // First call to GetByIdAsync to populate cache
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(root, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.bankTransactionExplanations.GetByIdAsync(explanationId);

        // Delete call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.bankTransactionExplanations.DeleteAsync(explanationId);

        // Setup response for second GetByIdAsync (after cache invalidation)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(root, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.bankTransactionExplanations.GetByIdAsync(explanationId);

        // Assert - Mock Verification: Should have made 3 calls (initial get, delete, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNewFields_DeserializesCorrectly()
    {
        // Arrange
        BankTransactionExplanation explanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/1234"),
            Type = "Bill Payment",
            GrossValue = 450.00m,
            SalesTaxRate = 0.20m,
            SalesTaxValue = 75.00m,
            SalesTaxStatus = "TAXABLE",
            EcStatus = "UK/Non-EC",
            IsDeletable = true,
            IsMoneyOut = true,
            IsMoneyIn = false,
            IsMoneyPaidToUser = false,
            PaidBill = new Uri("https://api.freeagent.com/v2/bills/567"),
            Project = new Uri("https://api.freeagent.com/v2/projects/890"),
            DatedOn = new DateOnly(2024, 4, 1)
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = explanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.GetByIdAsync("1234");

        // Assert - Verify new fields
        result.ShouldNotBeNull();
        result.Type.ShouldBe("Bill Payment");
        result.SalesTaxValue.ShouldBe(75.00m);
        result.SalesTaxStatus.ShouldBe("TAXABLE");
        result.EcStatus.ShouldBe("UK/Non-EC");
        result.IsDeletable.ShouldBe(true);
        result.IsMoneyOut.ShouldBe(true);
        result.IsMoneyIn.ShouldBe(false);
        result.IsMoneyPaidToUser.ShouldBe(false);
        result.PaidBill.ShouldBe(new Uri("https://api.freeagent.com/v2/bills/567"));
        result.Project.ShouldBe(new Uri("https://api.freeagent.com/v2/projects/890"));
    }

    [TestMethod]
    public async Task CreateAsync_WithBankTransferFields_CreatesSuccessfully()
    {
        // Arrange
        BankTransactionExplanation inputExplanation = new()
        {
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/111"),
            TransferBankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/222"),
            GrossValue = 1000.00m
        };

        BankTransactionExplanation responseExplanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/333"),
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/111"),
            TransferBankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/222"),
            GrossValue = 1000.00m,
            Type = "Bank Transfer"
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = responseExplanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.CreateAsync(inputExplanation);

        // Assert
        result.ShouldNotBeNull();
        result.TransferBankAccount.ShouldBe(new Uri("https://api.freeagent.com/v2/bank_accounts/222"));
        result.Type.ShouldBe("Bank Transfer");
    }

    [TestMethod]
    public async Task CreateAsync_WithStockFields_CreatesSuccessfully()
    {
        // Arrange
        BankTransactionExplanation inputExplanation = new()
        {
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/444"),
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/555"),
            StockAlteringQuantity = 10,
            GrossValue = 250.00m
        };

        BankTransactionExplanation responseExplanation = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transaction_explanations/666"),
            BankTransaction = new Uri("https://api.freeagent.com/v2/bank_transactions/444"),
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/555"),
            StockAlteringQuantity = 10,
            GrossValue = 250.00m,
            Type = "Stock Purchase"
        };

        BankTransactionExplanationRoot responseRoot = new() { BankTransactionExplanation = responseExplanation };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransactionExplanation result = await this.bankTransactionExplanations.CreateAsync(inputExplanation);

        // Assert
        result.ShouldNotBeNull();
        result.StockItem.ShouldBe(new Uri("https://api.freeagent.com/v2/stock_items/555"));
        result.StockAlteringQuantity.ShouldBe(10);
        result.Type.ShouldBe("Stock Purchase");
    }
}

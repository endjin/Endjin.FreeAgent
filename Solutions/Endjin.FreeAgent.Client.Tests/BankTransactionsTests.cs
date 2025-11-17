// <copyright file="BankTransactionsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BankTransactionsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private BankTransactions bankTransactions = null!;
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
        this.bankTransactions = new BankTransactions(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_WithBankAccount_ReturnsAllBankTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Payment received", Amount = 1000.00m },
            new() { Description = "Office rent", Amount = -500.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetAllAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(2);
        result.Any(t => t.Description == "Payment received").ShouldBeTrue();
        result.Any(t => t.Amount == -500.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("bank_account", bankAccountUri.ToString());
    }

    [TestMethod]
    public async Task GetAllAsync_WithBankAccountFilter_ReturnsFilteredBankTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { BankAccount = bankAccountUri, Description = "Payment 1", Amount = 100.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetAllAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().BankAccount.ShouldBe(bankAccountUri);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/456");
        List<BankTransaction> transactionsList = [new() { Description = "Test", Amount = 100.00m }];
        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<BankTransaction> result1 = await this.bankTransactions.GetAllAsync(bankAccountUri);
        IEnumerable<BankTransaction> result2 = await this.bankTransactions.GetAllAsync(bankAccountUri);

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetUnexplainedAsync_ReturnsUnexplainedTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Unexplained transaction", Amount = 250.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetUnexplainedAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Unexplained transaction");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "unexplained");
    }

    [TestMethod]
    public async Task GetUnexplainedAsync_WithBankAccountFilter_ReturnsFilteredUnexplainedTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/789");
        List<BankTransaction> transactionsList =
        [
            new() { BankAccount = bankAccountUri, Description = "Mystery payment", Amount = 150.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetUnexplainedAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().BankAccount.ShouldBe(bankAccountUri);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetExplainedAsync_ReturnsExplainedTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Explained transaction", Amount = 750.00m },
            new() { Description = "Another explained", Amount = -200.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetExplainedAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "explained");
    }

    [TestMethod]
    public async Task GetExplainedAsync_WithBankAccountFilter_ReturnsFilteredExplainedTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/999");
        List<BankTransaction> transactionsList =
        [
            new() { BankAccount = bankAccountUri, Description = "Reconciled payment", Amount = 300.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetExplainedAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Reconciled payment");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateRangeFilter_ReturnsFilteredTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 3, 31);
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Q1 transaction", Amount = 200.00m, DatedOn = new DateOnly(2024, 2, 15) }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetAllAsync(bankAccountUri, fromDate: fromDate, toDate: toDate);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Q1 transaction");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("from_date", "2024-01-01");
        this.messageHandler.ShouldHaveQueryParameter("to_date", "2024-03-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedSinceFilter_ReturnsRecentlyUpdatedTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        DateTime updatedSince = new(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc);
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Recently updated", Amount = 150.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetAllAsync(bankAccountUri, updatedSince: updatedSince);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Recently updated");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("updated_since=");
    }

    [TestMethod]
    public async Task GetAllAsync_WithLastUploadedOnly_ReturnsLatestStatementTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Latest upload", Amount = 300.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetAllAsync(bankAccountUri, lastUploadedOnly: true);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Latest upload");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("last_uploaded", "true");
    }

    [TestMethod]
    public async Task GetManualTransactionsAsync_ReturnsManuallyEnteredTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Manual entry", Amount = 500.00m, IsManual = true }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetManualTransactionsAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Manual entry");
        result.First().IsManual.ShouldBe(true);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "manual");
    }

    [TestMethod]
    public async Task GetImportedTransactionsAsync_ReturnsImportedTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransaction> transactionsList =
        [
            new() { Description = "Imported from bank", Amount = 750.00m, IsManual = false }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetImportedTransactionsAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Imported from bank");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "imported");
    }

    [TestMethod]
    public async Task GetMarkedForReviewAsync_ReturnsTransactionsNeedingReview()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/456");
        List<BankTransaction> transactionsList =
        [
            new() { BankAccount = bankAccountUri, Description = "Needs review", Amount = 1000.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetMarkedForReviewAsync(bankAccountUri);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Needs review");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveQueryParameter("view", "marked_for_review");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsBankTransaction()
    {
        // Arrange
        BankTransaction transaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transactions/555"),
            Description = "Specific transaction",
            Amount = 425.50m,
            DatedOn = new DateOnly(2024, 3, 10)
        };

        BankTransactionRoot responseRoot = new() { BankTransaction = transaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransaction result = await this.bankTransactions.GetByIdAsync("555");

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Specific transaction");
        result.Amount.ShouldBe(425.50m);
        result.DatedOn.ShouldBe(new DateOnly(2024, 3, 10));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/555");
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        BankTransaction transaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transactions/888"),
            Description = "Cached transaction",
            Amount = 100.00m
        };

        BankTransactionRoot responseRoot = new() { BankTransaction = transaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        BankTransaction result1 = await this.bankTransactions.GetByIdAsync("888");
        BankTransaction result2 = await this.bankTransactions.GetByIdAsync("888");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Description.ShouldBe("Cached transaction");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesBankTransaction()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.bankTransactions.DeleteAsync("666");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/666");
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesCacheEntry()
    {
        // Arrange
        string transactionId = "444";
        BankTransaction transaction = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/bank_transactions/{transactionId}"),
            Description = "To be deleted",
            Amount = 50.00m
        };

        BankTransactionRoot root = new() { BankTransaction = transaction };

        // First call to GetByIdAsync to populate cache
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(root, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.bankTransactions.GetByIdAsync(transactionId);

        // Delete call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.bankTransactions.DeleteAsync(transactionId);

        // Setup response for second GetByIdAsync (after cache invalidation)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(root, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.bankTransactions.GetByIdAsync(transactionId);

        // Assert - Mock Verification: Should have made 3 calls (initial get, delete, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task UploadStatementAsync_WithValidData_ReturnsParsedBankTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        byte[] statementData = Encoding.UTF8.GetBytes("OFX DATA HERE");
        string fileName = "statement.ofx";

        List<BankTransaction> parsedTransactions =
        [
            new() { Description = "Transaction from statement 1", Amount = 100.00m },
            new() { Description = "Transaction from statement 2", Amount = -50.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = parsedTransactions };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.UploadStatementAsync(
            bankAccountUri,
            statementData,
            fileName);

        // Assert
        result.Count().ShouldBe(2);
        result.Any(t => t.Description == "Transaction from statement 1").ShouldBeTrue();
        result.Any(t => t.Amount == -50.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/statement");
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("bank_account=");
        this.messageHandler.LastRequest?.Content?.Headers.ContentType?.MediaType.ShouldBe("multipart/form-data");
    }

    [TestMethod]
    public async Task UploadStatementAsync_WithCsvFileType_ProcessesCorrectly()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/456");
        byte[] statementData = Encoding.UTF8.GetBytes("date,description,amount\n2024-03-01,Payment,100.00");
        string fileName = "statement.csv";

        List<BankTransaction> parsedTransactions =
        [
            new() { Description = "Payment", Amount = 100.00m, DatedOn = new DateOnly(2024, 3, 1) }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = parsedTransactions };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.UploadStatementAsync(
            bankAccountUri,
            statementData,
            fileName);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Payment");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }

    [TestMethod]
    public async Task UploadStatementAsJsonAsync_WithValidTransactions_ReturnsCreatedBankTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/123");
        List<BankTransactionUpload> transactions =
        [
            new() { DatedOn = new DateOnly(2024, 3, 1), Description = "Deposit", Amount = 500.00m, TransactionType = "CREDIT" },
            new() { DatedOn = new DateOnly(2024, 3, 2), Description = "Payment", Amount = -100.00m, Fitid = "TXN123", TransactionType = "DEBIT" }
        ];

        List<BankTransaction> createdTransactions =
        [
            new() { Description = "Deposit", Amount = 500.00m, DatedOn = new DateOnly(2024, 3, 1) },
            new() { Description = "Payment", Amount = -100.00m, DatedOn = new DateOnly(2024, 3, 2) }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = createdTransactions };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.UploadStatementAsJsonAsync(
            bankAccountUri,
            transactions);

        // Assert
        result.Count().ShouldBe(2);
        result.Any(t => t.Description == "Deposit" && t.Amount == 500.00m).ShouldBeTrue();
        result.Any(t => t.Description == "Payment" && t.Amount == -100.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/statement");
        this.messageHandler.LastRequest?.RequestUri?.Query.ShouldContain("bank_account=");
        this.messageHandler.LastRequest?.Content?.Headers.ContentType?.MediaType.ShouldBe("application/json");
    }

    [TestMethod]
    public async Task UploadStatementAsJsonAsync_WithMinimalRequiredFields_CreatesTransactions()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/789");
        List<BankTransactionUpload> transactions =
        [
            new() { DatedOn = new DateOnly(2024, 3, 15) } // Only required field
        ];

        List<BankTransaction> createdTransactions =
        [
            new() { Amount = 0m, DatedOn = new DateOnly(2024, 3, 15), Description = string.Empty }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = createdTransactions };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.UploadStatementAsJsonAsync(
            bankAccountUri,
            transactions);

        // Assert
        result.Count().ShouldBe(1);
        result.First().DatedOn.ShouldBe(new DateOnly(2024, 3, 15));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }
}

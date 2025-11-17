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
    public async Task CreateAsync_WithValidBankTransaction_ReturnsCreatedBankTransaction()
    {
        // Arrange
        BankTransaction inputTransaction = new()
        {
            BankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/123"),
            DatedOn = new DateOnly(2024, 3, 15),
            Amount = 500.00m,
            Description = "Client payment"
        };

        BankTransaction responseTransaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transactions/456"),
            BankAccount = new Uri("https://api.freeagent.com/v2/bank_accounts/123"),
            DatedOn = new DateOnly(2024, 3, 15),
            Amount = 500.00m,
            Description = "Client payment",
            IsManual = true
        };

        BankTransactionRoot responseRoot = new() { BankTransaction = responseTransaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransaction result = await this.bankTransactions.CreateAsync(inputTransaction);

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldNotBeNull();
        result.Amount.ShouldBe(500.00m);
        result.Description.ShouldBe("Client payment");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions");
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutFilters_ReturnsAllBankTransactions()
    {
        // Arrange
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
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(t => t.Description == "Payment received").ShouldBeTrue();
        result.Any(t => t.Amount == -500.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions?view=all");
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
        List<BankTransaction> transactionsList = [new() { Description = "Test", Amount = 100.00m }];
        BankTransactionsRoot responseRoot = new() { BankTransactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<BankTransaction> result1 = await this.bankTransactions.GetAllAsync();
        IEnumerable<BankTransaction> result2 = await this.bankTransactions.GetAllAsync();

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
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetUnexplainedAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Unexplained transaction");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions?view=unexplained");
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
        IEnumerable<BankTransaction> result = await this.bankTransactions.GetExplainedAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions?view=explained");
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
    public async Task UpdateAsync_WithValidTransaction_ReturnsUpdatedBankTransaction()
    {
        // Arrange
        BankTransaction updatedTransaction = new()
        {
            Description = "Updated description",
            Amount = 600.00m
        };

        BankTransaction responseTransaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_transactions/777"),
            Description = "Updated description",
            Amount = 600.00m,
            DatedOn = new DateOnly(2024, 3, 20)
        };

        BankTransactionRoot responseRoot = new() { BankTransaction = responseTransaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankTransaction result = await this.bankTransactions.UpdateAsync("777", updatedTransaction);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Updated description");
        result.Amount.ShouldBe(600.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/777");
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCacheEntry()
    {
        // Arrange
        string transactionId = "999";
        BankTransaction originalTransaction = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/bank_transactions/{transactionId}"),
            Description = "Original",
            Amount = 100.00m
        };

        BankTransaction updatedTransaction = new()
        {
            Url = new Uri($"https://api.freeagent.com/v2/bank_transactions/{transactionId}"),
            Description = "Updated",
            Amount = 200.00m
        };

        BankTransactionRoot originalRoot = new() { BankTransaction = originalTransaction };
        BankTransactionRoot updatedRoot = new() { BankTransaction = updatedTransaction };

        // First call to GetByIdAsync
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(originalRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        await this.bankTransactions.GetByIdAsync(transactionId);

        // Update call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };

        // Act
        await this.bankTransactions.UpdateAsync(transactionId, updatedTransaction);

        // Setup response for second GetByIdAsync (after cache invalidation)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance), Encoding.UTF8, "application/json")
        };
        BankTransaction result = await this.bankTransactions.GetByIdAsync(transactionId);

        // Assert
        result.Description.ShouldBe("Updated");

        // Mock Verification - Should have made 3 calls (initial get, update, second get after cache invalidation)
        this.messageHandler.CallCount.ShouldBe(3);
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
        string statementData = "OFX DATA HERE";
        string fileType = "ofx";

        List<BankTransaction> parsedTransactions =
        [
            new() { Description = "Transaction from statement 1", Amount = 100.00m },
            new() { Description = "Transaction from statement 2", Amount = -50.00m }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = parsedTransactions };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.UploadStatementAsync(
            bankAccountUri,
            statementData,
            fileType);

        // Assert
        result.Count().ShouldBe(2);
        result.Any(t => t.Description == "Transaction from statement 1").ShouldBeTrue();
        result.Any(t => t.Amount == -50.00m).ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_transactions/statement");
    }

    [TestMethod]
    public async Task UploadStatementAsync_WithCsvFileType_ProcessesCorrectly()
    {
        // Arrange
        Uri bankAccountUri = new("https://api.freeagent.com/v2/bank_accounts/456");
        string statementData = "date,description,amount\n2024-03-01,Payment,100.00";
        string fileType = "csv";

        List<BankTransaction> parsedTransactions =
        [
            new() { Description = "Payment", Amount = 100.00m, DatedOn = new DateOnly(2024, 3, 1) }
        ];

        BankTransactionsRoot responseRoot = new() { BankTransactions = parsedTransactions };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankTransaction> result = await this.bankTransactions.UploadStatementAsync(
            bankAccountUri,
            statementData,
            fileType);

        // Assert
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Payment");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
    }
}

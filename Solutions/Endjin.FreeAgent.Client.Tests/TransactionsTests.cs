// <copyright file="TransactionsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Corvus.Retry.Policies;
using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class TransactionsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Transactions transactions = null!;
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

        // Disable retries for tests to speed them up
        this.freeAgentClient.RetryPolicy = Substitute.For<IRetryPolicy>();

        this.transactions = new Transactions(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllTransactions()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 6, 15),
                DebitValue = 1000.00m,
                NominalCode = "001",
                CategoryName = "Sales",
                Description = "Invoice #INV-001"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/2"),
                DatedOn = new DateOnly(2024, 6, 20),
                DebitValue = -500.00m,
                NominalCode = "200",
                CategoryName = "Cost of Sales",
                Description = "Bill #BILL-001"
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions");
    }

    [TestMethod]
    public async Task GetAllAsync_WithDateRange_PassesDatesToApi()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 1, 15),
                DebitValue = 100.00m
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync(
            fromDate: new DateOnly(2024, 1, 1),
            toDate: new DateOnly(2024, 12, 31));

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions?from_date=2024-01-01&to_date=2024-12-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithNominalCode_PassesNominalCodeToApi()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 3, 10),
                DebitValue = 250.00m,
                NominalCode = "001"
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync(nominalCode: "001");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions?nominal_code=001");
    }

    [TestMethod]
    public async Task GetAllAsync_WithAllFilters_PassesAllFiltersToApi()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 6, 15),
                DebitValue = 500.00m,
                NominalCode = "001"
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync(
            fromDate: new DateOnly(2024, 6, 1),
            toDate: new DateOnly(2024, 6, 30),
            nominalCode: "001");

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions?from_date=2024-06-01&to_date=2024-06-30&nominal_code=001");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsTransaction()
    {
        // Arrange
        Transaction transaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/456"),
            DatedOn = new DateOnly(2024, 7, 1),
            DebitValue = 1500.00m,
            NominalCode = "001",
            CategoryName = "Sales Revenue",
            Description = "Invoice #INV-100",
            CreatedAt = new DateTime(2024, 7, 1, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 7, 1, 10, 0, 0, DateTimeKind.Utc)
        };

        TransactionRoot responseRoot = new() { Transaction = transaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Transaction result = await this.transactions.GetByIdAsync("456");

        // Assert
        result.ShouldNotBeNull();
        result.DebitValue.ShouldBe(1500.00m);
        result.NominalCode.ShouldBe("001");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions/456");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNullId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.transactions.GetByIdAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithEmptyId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.transactions.GetByIdAsync(string.Empty));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithWhitespaceId_ThrowsArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () => await this.transactions.GetByIdAsync("   "));
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenTransactionNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        TransactionRoot responseRoot = new() { Transaction = null };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () => await this.transactions.GetByIdAsync("999"));
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsTransactionWithAllFields()
    {
        // Arrange
        DateTime createdAt = new(2024, 5, 1, 9, 0, 0, DateTimeKind.Utc);
        DateTime updatedAt = new(2024, 5, 15, 14, 30, 0, DateTimeKind.Utc);

        Transaction transaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/789"),
            DatedOn = new DateOnly(2024, 5, 1),
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Description = "Complete Transaction",
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            CategoryName = "Sales Revenue",
            NominalCode = "001",
            DebitValue = 2500.00m,
            SourceItemUrl = "https://api.freeagent.com/v2/invoices/123"
        };

        TransactionRoot responseRoot = new() { Transaction = transaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Transaction result = await this.transactions.GetByIdAsync("789");

        // Assert
        result.ShouldNotBeNull();
        result.Url.ShouldBe(new Uri("https://api.freeagent.com/v2/accounting/transactions/789"));
        result.DatedOn.ShouldBe(new DateOnly(2024, 5, 1));
        result.CreatedAt.ShouldBe(createdAt);
        result.UpdatedAt.ShouldBe(updatedAt);
        result.Description.ShouldBe("Complete Transaction");
        result.Category.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/001"));
        result.CategoryName.ShouldBe("Sales Revenue");
        result.NominalCode.ShouldBe("001");
        result.DebitValue.ShouldBe(2500.00m);
        result.SourceItemUrl.ShouldBe("https://api.freeagent.com/v2/invoices/123");
    }

    [TestMethod]
    public async Task GetByIdAsync_ReturnsTransactionWithForeignCurrencyData()
    {
        // Arrange
        Transaction transaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/101"),
            DatedOn = new DateOnly(2024, 8, 1),
            DebitValue = 850.00m,
            NominalCode = "001",
            ForeignCurrencyData = new ForeignCurrencyData
            {
                CurrencyCode = "USD",
                DebitValue = 1000.00m
            }
        };

        TransactionRoot responseRoot = new() { Transaction = transaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Transaction result = await this.transactions.GetByIdAsync("101");

        // Assert
        result.ShouldNotBeNull();
        result.DebitValue.ShouldBe(850.00m);
        result.ForeignCurrencyData.ShouldNotBeNull();
        result.ForeignCurrencyData.CurrencyCode.ShouldBe("USD");
        result.ForeignCurrencyData.DebitValue.ShouldBe(1000.00m);
    }

    [TestMethod]
    public async Task GetAllAsync_WhenNoTransactions_ReturnsEmptyCollection()
    {
        // Arrange
        TransactionsRoot responseRoot = new() { Transactions = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 1, 1),
                DebitValue = 100.00m
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> firstResult = await this.transactions.GetAllAsync();
        IEnumerable<Transaction> secondResult = await this.transactions.GetAllAsync();

        // Assert
        firstResult.Count().ShouldBe(1);
        secondResult.Count().ShouldBe(1);

        // Mock Verification - API should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResults()
    {
        // Arrange
        Transaction transaction = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/123"),
            DatedOn = new DateOnly(2024, 4, 1),
            DebitValue = 750.00m,
            Description = "Cached Transaction"
        };

        TransactionRoot responseRoot = new() { Transaction = transaction };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Transaction firstResult = await this.transactions.GetByIdAsync("123");
        Transaction secondResult = await this.transactions.GetByIdAsync("123");

        // Assert
        firstResult.Description.ShouldBe("Cached Transaction");
        secondResult.Description.ShouldBe("Cached Transaction");

        // Mock Verification - API should only be called once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllAsync_WhenApiReturnsError_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await this.transactions.GetAllAsync());
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenApiReturnsError_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
            Content = new StringContent("Internal Server Error", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () => await this.transactions.GetByIdAsync("123"));
    }

    [TestMethod]
    public async Task GetAllAsync_WithFromDateOnly_PassesFromDateToApi()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 9, 1),
                DebitValue = 300.00m
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync(fromDate: new DateOnly(2024, 9, 1));

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions?from_date=2024-09-01");
    }

    [TestMethod]
    public async Task GetAllAsync_WithToDateOnly_PassesToDateToApi()
    {
        // Arrange
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 3, 15),
                DebitValue = 450.00m
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync(toDate: new DateOnly(2024, 3, 31));

        // Assert
        result.Count().ShouldBe(1);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/accounting/transactions?to_date=2024-03-31");
    }

    [TestMethod]
    public async Task GetAllAsync_WithNegativeDebitValue_ReturnsCredit()
    {
        // Arrange - negative debit_value indicates a credit
        List<Transaction> transactionsList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/accounting/transactions/1"),
                DatedOn = new DateOnly(2024, 10, 1),
                DebitValue = -1200.00m,
                NominalCode = "200",
                CategoryName = "Accounts Payable",
                Description = "Bill Payment"
            }
        ];

        TransactionsRoot responseRoot = new() { Transactions = transactionsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.SourceGenOptions);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Transaction> result = await this.transactions.GetAllAsync();

        // Assert
        Transaction transaction = result.First();
        transaction.DebitValue.ShouldBe(-1200.00m);
    }
}

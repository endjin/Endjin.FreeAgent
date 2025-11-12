// <copyright file="BankAccountsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class BankAccountsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private BankAccounts bankAccounts = null!;
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
        this.bankAccounts = new BankAccounts(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidBankAccount_ReturnsCreatedBankAccount()
    {
        // Arrange
        BankAccount inputAccount = new()
        {
            Type = "StandardBankAccount",
            Name = "Business Current Account",
            NominalCode = "001",
            AccountNumber = "12345678",
            SortCode = "123456",
            Currency = "GBP"
        };

        BankAccount responseAccount = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_accounts/123"),
            Type = "StandardBankAccount",
            Name = "Business Current Account",
            Currency = "GBP",
            CurrentBalance = 1000.00m
        };

        BankAccountRoot responseRoot = new() { BankAccount = responseAccount };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankAccount result = await this.bankAccounts.CreateAsync(inputAccount);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Business Current Account");
        result.Type.ShouldBe("StandardBankAccount");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_accounts");
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllBankAccounts()
    {
        // Arrange
        List<BankAccount> accountsList =
        [
            new() { Name = "Current Account", CurrentBalance = 5000.00m },
            new() { Name = "Savings Account", CurrentBalance = 10000.00m }
        ];

        BankAccountsRoot responseRoot = new() { BankAccounts = accountsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<BankAccount> result = await this.bankAccounts.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(a => a.Name == "Current Account").ShouldBeTrue();
        result.Any(a => a.Name == "Savings Account").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsBankAccount()
    {
        // Arrange
        BankAccount account = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_accounts/123"),
            Name = "Current Account",
            CurrentBalance = 5000.00m
        };

        BankAccountRoot responseRoot = new() { BankAccount = account };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankAccount result = await this.bankAccounts.GetByIdAsync("123");

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Current Account");
        result.CurrentBalance.ShouldBe(5000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_accounts/123");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidChanges_ReturnsUpdatedBankAccount()
    {
        // Arrange
        BankAccount updateAccount = new()
        {
            Name = "Updated Account Name"
        };

        BankAccount responseAccount = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/bank_accounts/123"),
            Name = "Updated Account Name"
        };

        BankAccountRoot responseRoot = new() { BankAccount = responseAccount };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        BankAccount result = await this.bankAccounts.UpdateAsync("123", updateAccount);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Account Name");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_accounts/123");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesSuccessfully()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.bankAccounts.DeleteAsync("123");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/bank_accounts/123");
    }
}

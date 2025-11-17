// <copyright file="CompanyTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CompanyTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Company company = null!;
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
        this.company = new Company(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAsync_ReturnsCompanyDetails()
    {
        // Arrange
        Domain.Company companyDetails = new()
        {
            Name = "Acme Corporation",
            Subdomain = "acme",
            Type = "UkLimitedCompany",
            Currency = "GBP",
            MileageUnits = "miles"
        };

        CompanyRoot responseRoot = new() { Company = companyDetails };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Domain.Company result = await this.company.GetAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Acme Corporation");
        result.Subdomain.ShouldBe("acme");
        result.Type.ShouldBe("UkLimitedCompany");
        result.Currency.ShouldBe("GBP");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/company");
    }

    [TestMethod]
    public async Task GetAsync_CachesResult()
    {
        // Arrange
        Domain.Company companyDetails = new()
        {
            Name = "Cached Company",
            Subdomain = "cached",
            Currency = "GBP"
        };

        CompanyRoot responseRoot = new() { Company = companyDetails };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        Domain.Company result1 = await this.company.GetAsync();
        Domain.Company result2 = await this.company.GetAsync();

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Name.ShouldBe("Cached Company");

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesAndReturnsCompany()
    {
        // Arrange
        Domain.Company updatedCompany = new()
        {
            Name = "Updated Company Name",
            Currency = "USD"
        };

        Domain.Company responseCompany = new()
        {
            Name = "Updated Company Name",
            Subdomain = "acme",
            Type = "UkLimitedCompany",
            Currency = "USD"
        };

        CompanyRoot responseRoot = new() { Company = responseCompany };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Domain.Company result = await this.company.UpdateAsync(updatedCompany);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Updated Company Name");
        result.Currency.ShouldBe("USD");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/company");
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCache()
    {
        // Arrange - First get to populate cache
        Domain.Company originalCompany = new() { Name = "Original Name", Currency = "GBP" };
        CompanyRoot originalRoot = new() { Company = originalCompany };
        string originalJson = JsonSerializer.Serialize(originalRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(originalJson, Encoding.UTF8, "application/json")
        };

        await this.company.GetAsync();

        // Update
        Domain.Company updatedCompany = new() { Name = "Updated Name", Currency = "USD" };
        CompanyRoot updatedRoot = new() { Company = updatedCompany };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.company.UpdateAsync(updatedCompany);

        // Get again to verify cache was invalidated
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        Domain.Company result = await this.company.GetAsync();

        // Assert
        result.Name.ShouldBe("Updated Name");

        // Mock Verification - Should have made 3 calls (initial get, update, second get)
        this.messageHandler.CallCount.ShouldBe(3);
    }
}

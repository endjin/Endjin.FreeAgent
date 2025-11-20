// <copyright file="CategoriesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CategoriesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Categories categories = null!;
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
        this.categories = new Categories(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        // Arrange
        List<Category> incomeCategoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/1"),
                NominalCode = "001",
                Description = "Sales"
            }
        ];

        List<Category> costOfSalesCategoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/2"),
                NominalCode = "002",
                Description = "Purchases"
            }
        ];

        CategoriesRoot responseRoot = new()
        {
            IncomeCategories = incomeCategoriesList,
            CostOfSalesCategories = costOfSalesCategoriesList,
            AdminExpensesCategories = [],
            GeneralCategories = []
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Category> result = await this.categories.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.Any(c => c.NominalCode == "001").ShouldBeTrue();
        result.Any(c => c.Description == "Purchases").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories");
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<Category> incomeCategoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/10"),
                NominalCode = "010",
                Description = "Cached Category"
            }
        ];

        CategoriesRoot responseRoot = new()
        {
            IncomeCategories = incomeCategoriesList,
            CostOfSalesCategories = [],
            AdminExpensesCategories = [],
            GeneralCategories = []
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        IEnumerable<Category> result1 = await this.categories.GetAllAsync();
        IEnumerable<Category> result2 = await this.categories.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetByNominalCodeAsync_WithValidCode_ReturnsCategory()
    {
        // Arrange
        Category category = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/categories/200"),
            NominalCode = "200",
            Description = "Office Costs"
        };

        // Nominal code 200 is in the admin expenses range (200-399)
        CategoryRoot responseRoot = new() { AdminExpensesCategories = category };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Category result = await this.categories.GetByNominalCodeAsync("200");

        // Assert
        result.ShouldNotBeNull();
        result.NominalCode.ShouldBe("200");
        result.Description.ShouldBe("Office Costs");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories/200");
    }

    [TestMethod]
    public async Task GetByNominalCodeAsync_WithInvalidCode_ThrowsException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
        {
            await this.categories.GetByNominalCodeAsync("999");
        });

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories/999");
    }

    [TestMethod]
    public async Task GetByNominalCodeAsync_CachesResult()
    {
        // Arrange
        Category category = new()
        {
            NominalCode = "300",
            Description = "Cached Nominal Code Category"
        };

        // Nominal code 300 is in the admin expenses range (200-399)
        CategoryRoot responseRoot = new() { AdminExpensesCategories = category };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        Category result1 = await this.categories.GetByNominalCodeAsync("300");
        Category result2 = await this.categories.GetByNominalCodeAsync("300");

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories/300");
    }

    [TestMethod]
    public async Task GetAllAsync_WithSubAccounts_ReturnsAllCategories()
    {
        // Arrange
        List<Category> incomeCategoriesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/1"),
                NominalCode = "001",
                Description = "Sales",
                GroupDescription = "Income"
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/categories/1-1"),
                NominalCode = "001-1",
                Description = "Sales - Sub Account",
                GroupDescription = "Income"
            }
        ];

        CategoriesRoot responseRoot = new()
        {
            IncomeCategories = incomeCategoriesList,
            CostOfSalesCategories = [],
            AdminExpensesCategories = [],
            GeneralCategories = []
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Category> result = await this.categories.GetAllAsync(includeSubAccounts: true);

        // Assert
        result.Count().ShouldBe(2);
        result.Any(c => c.NominalCode == "001-1").ShouldBeTrue();

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories?sub_accounts=true");
    }

    [TestMethod]
    public async Task CreateAsync_CreatesNewCategory()
    {
        // Arrange
        CategoryCreateRequest request = new()
        {
            NominalCode = "250",
            Description = "New Category",
            CategoryGroup = CategoryGroupType.AdminExpenses,
            TaxReportingName = "Other business expenses",
            AllowableForTax = true
        };

        Category createdCategory = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/categories/250"),
            NominalCode = "250",
            Description = "New Category",
            GroupDescription = "Admin Expenses",
            TaxReportingName = "Other business expenses",
            AllowableForTax = true
        };

        // Nominal code 250 is in the admin expenses range (200-399)
        CategoryRoot responseRoot = new() { AdminExpensesCategories = createdCategory };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Category result = await this.categories.CreateAsync(request);

        // Assert
        result.ShouldNotBeNull();
        result.NominalCode.ShouldBe("250");
        result.Description.ShouldBe("New Category");
        result.GroupDescription.ShouldBe("Admin Expenses");
        result.TaxReportingName.ShouldBe("Other business expenses");
        result.AllowableForTax.ShouldBe(true);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories");
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesExistingCategory()
    {
        // Arrange
        CategoryUpdateRequest request = new()
        {
            Description = "Updated Category Description",
            TaxReportingName = "Miscellaneous expenses",
            AllowableForTax = false
        };

        Category updatedCategory = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/categories/260"),
            NominalCode = "260",
            Description = "Updated Category Description",
            GroupDescription = "Admin Expenses",
            TaxReportingName = "Miscellaneous expenses",
            AllowableForTax = false
        };

        // Nominal code 260 is in the admin expenses range (200-399)
        CategoryRoot responseRoot = new() { AdminExpensesCategories = updatedCategory };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Category result = await this.categories.UpdateAsync("260", request);

        // Assert
        result.ShouldNotBeNull();
        result.NominalCode.ShouldBe("260");
        result.Description.ShouldBe("Updated Category Description");
        result.TaxReportingName.ShouldBe("Miscellaneous expenses");
        result.AllowableForTax.ShouldBe(false);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories/260");
    }

    [TestMethod]
    public async Task DeleteAsync_DeletesCategory()
    {
        // Arrange
        Category deletedCategory = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/categories/270"),
            NominalCode = "270",
            Description = "Deleted Category"
        };

        // Nominal code 270 is in the admin expenses range (200-399)
        CategoryRoot responseRoot = new() { AdminExpensesCategories = deletedCategory };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Category result = await this.categories.DeleteAsync("270");

        // Assert
        result.ShouldNotBeNull();
        result.NominalCode.ShouldBe("270");
        result.Description.ShouldBe("Deleted Category");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/categories/270");
    }
}

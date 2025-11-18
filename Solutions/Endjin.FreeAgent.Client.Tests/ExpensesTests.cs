// <copyright file="ExpensesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class ExpensesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Expenses expenses = null!;
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
        this.expenses = new Expenses(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidExpense_ReturnsCreatedExpense()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Client lunch meeting",
            User = new Uri("https://api.freeagent.com/v2/users/1")
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Client lunch meeting",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Url = new Uri("https://api.freeagent.com/v2/expenses/12345")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.DatedOn.ShouldBe(new DateOnly(2024, 1, 15));
        result.Category.ShouldBe("https://api.freeagent.com/v2/categories/285");
        result.Currency.ShouldBe("GBP");
        result.GrossValue.ShouldBe(100.00m);
        result.Description.ShouldBe("Client lunch meeting");
        result.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/expenses/12345");
    }

    [TestMethod]
    public async Task CreateAsync_WithAttachment_IncludesAttachmentData()
    {
        // Arrange
        ExpenseAttachment attachment = new()
        {
            Data = "base64encodeddata==",
            FileName = "receipt.pdf",
            ContentType = "application/x-pdf",
            Description = "Receipt for lunch"
        };

        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 50.00m,
            Description = "Office supplies",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Attachment = attachment
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 50.00m,
            Description = "Office supplies",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Attachment = attachment,
            Url = new Uri("https://api.freeagent.com/v2/expenses/12346")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Attachment.ShouldNotBeNull();
        result.Attachment!.FileName.ShouldBe("receipt.pdf");
        result.Attachment.ContentType.ShouldBe("application/x-pdf");
        result.Attachment.Description.ShouldBe("Receipt for lunch");
    }

    [TestMethod]
    public async Task CreateAsync_WithInvalidData_ThrowsHttpRequestException()
    {
        // Arrange
        Expense inputExpense = new()
        {
            // Missing required fields
            Description = "Invalid expense"
        };

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("Validation failed", Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.expenses.CreateAsync(inputExpense));
    }

    [TestMethod]
    public async Task CreateAsync_WithMultipleCurrencies_HandlesCorrectly()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "USD",
            GrossValue = 150.00m,
            Description = "International conference",
            User = new Uri("https://api.freeagent.com/v2/users/1")
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "USD",
            GrossValue = 150.00m,
            Description = "International conference",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Url = new Uri("https://api.freeagent.com/v2/expenses/12347")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Currency.ShouldBe("USD");
        result.GrossValue.ShouldBe(150.00m);
    }

    [TestMethod]
    public async Task CreateAsync_WithProject_AssociatesExpenseToProject()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 75.00m,
            Description = "Project materials",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Project = new Uri("https://api.freeagent.com/v2/projects/5678")
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 75.00m,
            Description = "Project materials",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Project = new Uri("https://api.freeagent.com/v2/projects/5678"),
            Url = new Uri("https://api.freeagent.com/v2/expenses/12348")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Project?.ToString().ShouldBe("https://api.freeagent.com/v2/projects/5678");
    }

    [TestMethod]
    public async Task CreateBatchAsync_WithMultipleExpenses_ReturnsAllCreatedExpenses()
    {
        // Arrange
        List<Expense> inputExpenses =
        [
            new()
            {
                DatedOn = new DateOnly(2024, 1, 15),
                Category = "https://api.freeagent.com/v2/categories/285",
                Currency = "GBP",
                GrossValue = 45.00m,
                Description = "Office supplies",
                User = new Uri("https://api.freeagent.com/v2/users/1")
            },
            new()
            {
                DatedOn = new DateOnly(2024, 1, 16),
                Category = "https://api.freeagent.com/v2/categories/286",
                Currency = "GBP",
                GrossValue = 120.00m,
                Description = "Client lunch",
                User = new Uri("https://api.freeagent.com/v2/users/1")
            },
            new()
            {
                DatedOn = new DateOnly(2024, 1, 17),
                Category = "https://api.freeagent.com/v2/categories/287",
                Currency = "GBP",
                GrossValue = 75.50m,
                Description = "Travel expense",
                User = new Uri("https://api.freeagent.com/v2/users/1")
            }
        ];

        List<Expense> responseExpenses =
        [
            new()
            {
                DatedOn = new DateOnly(2024, 1, 15),
                Category = "https://api.freeagent.com/v2/categories/285",
                Currency = "GBP",
                GrossValue = 45.00m,
                Description = "Office supplies",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/12351")
            },
            new()
            {
                DatedOn = new DateOnly(2024, 1, 16),
                Category = "https://api.freeagent.com/v2/categories/286",
                Currency = "GBP",
                GrossValue = 120.00m,
                Description = "Client lunch",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/12352")
            },
            new()
            {
                DatedOn = new DateOnly(2024, 1, 17),
                Category = "https://api.freeagent.com/v2/categories/287",
                Currency = "GBP",
                GrossValue = 75.50m,
                Description = "Travel expense",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/12353")
            }
        ];

        ExpensesRoot responseRoot = new() { Expenses = responseExpenses };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Expense> result = await this.expenses.CreateBatchAsync(inputExpenses);

        // Assert
        List<Expense> resultList = result.ToList();
        resultList.Count.ShouldBe(3);

        resultList[0].Description.ShouldBe("Office supplies");
        resultList[0].GrossValue.ShouldBe(45.00m);
        resultList[0].Url.ShouldNotBeNull();

        resultList[1].Description.ShouldBe("Client lunch");
        resultList[1].GrossValue.ShouldBe(120.00m);
        resultList[1].Url.ShouldNotBeNull();

        resultList[2].Description.ShouldBe("Travel expense");
        resultList[2].GrossValue.ShouldBe(75.50m);
        resultList[2].Url.ShouldNotBeNull();
    }

    [TestMethod]
    public async Task CreateBatchAsync_WithEmptyList_ThrowsArgumentException()
    {
        // Arrange
        List<Expense> emptyList = [];

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.CreateBatchAsync(emptyList)
        );
    }

    [TestMethod]
    public async Task GetAllAsync_RetrievesExpenses()
    {
        // Arrange
        List<Expense> expenses =
        [
            new()
            {
                DatedOn = new DateOnly(2024, 1, 15),
                Category = "https://api.freeagent.com/v2/categories/285",
                Currency = "GBP",
                GrossValue = 100.00m,
                Description = "Office supplies",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/1")
            },
            new()
            {
                DatedOn = new DateOnly(2024, 1, 16),
                Category = "https://api.freeagent.com/v2/categories/285",
                Currency = "GBP",
                GrossValue = 50.00m,
                Description = "Travel",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/2")
            },
        ];

        ExpensesRoot responseRoot = new() { Expenses = expenses };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Expense> result = await this.expenses.GetAllAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(2);
        result.First().Description.ShouldBe("Office supplies");
        result.Last().Description.ShouldBe("Travel");
    }

    [TestMethod]
    public async Task GetAllAsync_WithFilters_AppliesCorrectly()
    {
        // Arrange
        List<Expense> expenses =
        [
            new()
            {
                DatedOn = new DateOnly(2024, 1, 15),
                Category = "https://api.freeagent.com/v2/categories/285",
                Currency = "GBP",
                GrossValue = 100.00m,
                Description = "Filtered expense",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/1"),
                Project = new Uri("https://api.freeagent.com/v2/projects/5678")
            },
        ];

        ExpensesRoot responseRoot = new() { Expenses = expenses };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        DateOnly fromDate = new(2024, 1, 1);
        DateOnly toDate = new(2024, 1, 31);
        Uri projectUri = new("https://api.freeagent.com/v2/projects/5678");

        // Act
        IEnumerable<Expense> result = await this.expenses.GetAllAsync(
            view: "recent",
            fromDate: fromDate,
            toDate: toDate,
            project: projectUri);

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Filtered expense");
    }

    [TestMethod]
    public async Task GetByIdAsync_RetrievesSpecificExpense()
    {
        // Arrange
        Expense expense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Specific expense",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Url = new Uri("https://api.freeagent.com/v2/expenses/12345")
        };

        ExpenseRoot responseRoot = new() { Expense = expense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.GetByIdAsync("12345");

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Specific expense");
        result.GrossValue.ShouldBe(100.00m);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenExpenseNotFound_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(
                "{\"errors\":{\"error\":{\"message\":\"Resource not found\"}}}",
                Encoding.UTF8,
                "application/json"
            )
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.expenses.GetByIdAsync("99999")
        );
    }

    [TestMethod]
    public async Task UpdateAsync_UpdatesExpense()
    {
        // Arrange
        Expense updateExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 150.00m,
            Description = "Updated expense",
            User = new Uri("https://api.freeagent.com/v2/users/1")
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 150.00m,
            Description = "Updated expense",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Url = new Uri("https://api.freeagent.com/v2/expenses/12345")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.UpdateAsync("12345", updateExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Description.ShouldBe("Updated expense");
        result.GrossValue.ShouldBe(150.00m);
    }

    [TestMethod]
    public async Task DeleteAsync_DeletesExpense()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act & Assert (should not throw)
        await this.expenses.DeleteAsync("12345").ShouldNotThrowAsync();
    }

    [TestMethod]
    public async Task GetMileageSettingsAsync_RetrievesSettings()
    {
        // Arrange
        string settingsJson = """
        {
            "mileage_settings": {
                "engine_type_and_size_options": [
                    {
                        "from": "2024-01-01",
                        "to": "2024-12-31",
                        "value": {
                            "Petrol": ["Up to 1400cc", "1401cc to 2000cc", "Over 2000cc"],
                            "Diesel": ["Up to 1400cc", "1401cc to 2000cc", "Over 2000cc"],
                            "Electric": []
                        }
                    }
                ],
                "mileage_rates": [
                    {
                        "from": "2024-01-01",
                        "to": "2024-12-31",
                        "value": {
                            "Car": {
                                "basic_rate": 0.45,
                                "additional_rate": 0.25
                            },
                            "Motorcycle": {
                                "basic_rate": 0.24,
                                "additional_rate": 0.24
                            },
                            "Bicycle": {
                                "basic_rate": 0.20,
                                "additional_rate": 0.20
                            },
                            "basic_rate_limit": 10000
                        }
                    }
                ]
            }
        }
        """;

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(settingsJson, Encoding.UTF8, "application/json")
        };

        // Act
        MileageSettings result = await this.expenses.GetMileageSettingsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.EngineTypeAndSizeOptions.ShouldNotBeNull();
        result.EngineTypeAndSizeOptions!.Count.ShouldBe(1);
        result.EngineTypeAndSizeOptions[0].Value.ShouldNotBeNull();
        Dictionary<string, string[]> engineOptions = result.EngineTypeAndSizeOptions[0].Value!;
        engineOptions.ShouldContainKey("Petrol");
        engineOptions["Petrol"].ShouldContain("Up to 1400cc");

        result.MileageRates.ShouldNotBeNull();
        result.MileageRates!.Count.ShouldBe(1);
        result.MileageRates[0].Value.ShouldNotBeNull();
        result.MileageRates[0].Value!.Car.ShouldNotBeNull();
        result.MileageRates[0].Value!.Car!.BasicRate.ShouldBe(0.45m);
        result.MileageRates[0].Value!.BasicRateLimit.ShouldBe(10000);
    }

    [TestMethod]
    public async Task CreateAsync_WithMileageExpense_ReturnsCreatedMileageExpense()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "Mileage",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Mileage = 50.5m,
            VehicleType = VehicleType.Car,
            EngineType = EngineType.Petrol,
            EngineSize = "1401cc to 2000cc",
            ReclaimMileage = 1,
            Description = "Client visit travel"
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "Mileage",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Mileage = 50.5m,
            VehicleType = VehicleType.Car,
            EngineType = EngineType.Petrol,
            EngineSize = "1401cc to 2000cc",
            ReclaimMileage = 1,
            Description = "Client visit travel",
            GrossValue = 22.73m,
            ReclaimMileageRate = 0.45m,
            Url = new Uri("https://api.freeagent.com/v2/expenses/12349")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Category.ShouldBe("Mileage");
        result.Mileage.ShouldBe(50.5m);
        result.VehicleType.ShouldBe(VehicleType.Car);
        result.EngineType.ShouldBe(EngineType.Petrol);
        result.EngineSize.ShouldBe("1401cc to 2000cc");
        result.ReclaimMileage.ShouldBe(1);
        result.GrossValue.ShouldBe(22.73m);
        result.ReclaimMileageRate.ShouldBe(0.45m);
    }

    [TestMethod]
    [DataRow(VehicleType.Car, "Car", DisplayName = "Car Vehicle Type")]
    [DataRow(VehicleType.Motorcycle, "Motorcycle", DisplayName = "Motorcycle Vehicle Type")]
    [DataRow(VehicleType.Bicycle, "Bicycle", DisplayName = "Bicycle Vehicle Type")]
    public async Task CreateAsync_WithAllVehicleTypes_HandlesCorrectly(VehicleType vehicleType, string expectedJsonValue)
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "Mileage",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Mileage = 25.0m,
            VehicleType = vehicleType,
            ReclaimMileage = 1,
            Description = $"Travel by {vehicleType}",
            // Include engine details only for motorized vehicles
            EngineType = (vehicleType == VehicleType.Car || vehicleType == VehicleType.Motorcycle) ? EngineType.Petrol : null,
            EngineSize = vehicleType == VehicleType.Car ? "Up to 1400cc" :
                         vehicleType == VehicleType.Motorcycle ? "Over 500cc" : null
        };

        Expense responseExpense = inputExpense with
        {
            GrossValue = vehicleType == VehicleType.Bicycle ? 5.00m : 11.25m,
            ReclaimMileageRate = vehicleType == VehicleType.Bicycle ? 0.20m : 0.45m,
            Url = new Uri($"https://api.freeagent.com/v2/expenses/{12370 + (int)vehicleType}")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.VehicleType.ShouldBe(vehicleType);
        result.Mileage.ShouldBe(25.0m);
    }

    [TestMethod]
    public async Task CreateAsync_WithRecurringExpense_ReturnsCreatedRecurringExpense()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 29.99m,
            Description = "Monthly subscription",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Recurring = RecurringPattern.TwoMonthly,
            RecurringEndDate = new DateOnly(2024, 12, 31)
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 29.99m,
            Description = "Monthly subscription",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Recurring = RecurringPattern.TwoMonthly,
            RecurringEndDate = new DateOnly(2024, 12, 31),
            NextRecursOn = new DateOnly(2024, 2, 15),
            Url = new Uri("https://api.freeagent.com/v2/expenses/12350")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Recurring.ShouldBe(RecurringPattern.TwoMonthly);
        result.RecurringEndDate.ShouldBe(new DateOnly(2024, 12, 31));
        result.NextRecursOn.ShouldBe(new DateOnly(2024, 2, 15));
    }

    [TestMethod]
    [DataRow(RecurringPattern.Weekly, "Weekly", DisplayName = "Weekly Pattern")]
    [DataRow(RecurringPattern.TwoWeekly, "Two Weekly", DisplayName = "Two Weekly Pattern")]
    [DataRow(RecurringPattern.FourWeekly, "Four Weekly", DisplayName = "Four Weekly Pattern")]
    [DataRow(RecurringPattern.TwoMonthly, "Two Monthly", DisplayName = "Two Monthly Pattern")]
    [DataRow(RecurringPattern.Quarterly, "Quarterly", DisplayName = "Quarterly Pattern")]
    [DataRow(RecurringPattern.Biannually, "Biannually", DisplayName = "Biannually Pattern")]
    [DataRow(RecurringPattern.Annually, "Annually", DisplayName = "Annually Pattern")]
    [DataRow(RecurringPattern.TwoYearly, "2-Yearly", DisplayName = "Two Yearly Pattern")]
    public async Task CreateAsync_WithAllRecurringPatterns_HandlesCorrectly(RecurringPattern pattern, string expectedJsonValue)
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 50.00m,
            Description = $"Recurring expense with {pattern} pattern",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Recurring = pattern,
            RecurringEndDate = new DateOnly(2025, 12, 31)
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 50.00m,
            Description = $"Recurring expense with {pattern} pattern",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Recurring = pattern,
            RecurringEndDate = new DateOnly(2025, 12, 31),
            NextRecursOn = new DateOnly(2024, 2, 15),
            Url = new Uri($"https://api.freeagent.com/v2/expenses/{12360 + (int)pattern}")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Recurring.ShouldBe(pattern);
        result.RecurringEndDate.ShouldBe(new DateOnly(2025, 12, 31));
    }

    [TestMethod]
    public async Task CreateAsync_WithStockPurchase_ReturnsCreatedStockExpense()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 500.00m,
            Description = "Stock purchase",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/123"),
            StockAlteringQuantity = 100m
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 500.00m,
            Description = "Stock purchase",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/123"),
            StockAlteringQuantity = 100m,
            StockItemDescription = "Widget Component",
            Url = new Uri("https://api.freeagent.com/v2/expenses/12351")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.StockItem?.ToString().ShouldBe("https://api.freeagent.com/v2/stock_items/123");
        result.StockAlteringQuantity.ShouldBe(100m);
        result.StockItemDescription.ShouldBe("Widget Component");
    }

    [TestMethod]
    public async Task CreateAsync_WithRebilling_ReturnsCreatedRebillableExpense()
    {
        // Arrange
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 200.00m,
            Description = "Client materials",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Project = new Uri("https://api.freeagent.com/v2/projects/5678"),
            RebillType = RebillType.Markup,
            RebillFactor = 1.15m
        };

        Expense responseExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 200.00m,
            Description = "Client materials",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Project = new Uri("https://api.freeagent.com/v2/projects/5678"),
            RebillType = RebillType.Markup,
            RebillFactor = 1.15m,
            Url = new Uri("https://api.freeagent.com/v2/expenses/12352")
        };

        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Expense result = await this.expenses.CreateAsync(inputExpense);

        // Assert
        result.ShouldNotBeNull();
        result.Project?.ToString().ShouldBe("https://api.freeagent.com/v2/projects/5678");
        result.RebillType.ShouldBe(RebillType.Markup);
        result.RebillFactor.ShouldBe(1.15m);
    }

    [TestMethod]
    public async Task GetAllAsync_WithUpdatedSince_AppliesFilter()
    {
        // Arrange
        List<Expense> expenses =
        [
            new()
            {
                DatedOn = new DateOnly(2024, 1, 15),
                Category = "https://api.freeagent.com/v2/categories/285",
                Currency = "GBP",
                GrossValue = 100.00m,
                Description = "Recently updated expense",
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                Url = new Uri("https://api.freeagent.com/v2/expenses/1"),
                UpdatedAt = new DateTime(2024, 1, 20, 10, 30, 0)
            },
        ];

        ExpensesRoot responseRoot = new() { Expenses = expenses };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        DateTime updatedSince = new(2024, 1, 19, 0, 0, 0);

        // Act
        IEnumerable<Expense> result = await this.expenses.GetAllAsync(updatedSince: updatedSince);

        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Recently updated expense");

        // Verify the request URL contains the updated_since parameter
        this.messageHandler.LastRequest.ShouldNotBeNull();
        this.messageHandler.LastRequest!.RequestUri?.Query.ShouldContain("updated_since=");
    }

    [TestMethod]
    public async Task CreateAsync_WithNullExpense_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.expenses.CreateAsync(null!));
    }

    [TestMethod]
    public async Task CreateBatchAsync_WithNullExpenses_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.expenses.CreateBatchAsync(null!));
    }

    [TestMethod]
    public async Task GetByIdAsync_WithNullOrEmptyId_ThrowsArgumentException()
    {
        // Act & Assert for null
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.GetByIdAsync(null!));

        // Act & Assert for empty string
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.GetByIdAsync(string.Empty));

        // Act & Assert for whitespace
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.GetByIdAsync("  "));
    }

    [TestMethod]
    public async Task UpdateAsync_WithNullOrEmptyId_ThrowsArgumentException()
    {
        // Arrange
        Expense expense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Test expense",
            User = new Uri("https://api.freeagent.com/v2/users/1")
        };

        // Act & Assert for null
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.UpdateAsync(null!, expense));

        // Act & Assert for empty string
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.UpdateAsync(string.Empty, expense));

        // Act & Assert for whitespace
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.UpdateAsync("  ", expense));
    }

    [TestMethod]
    public async Task UpdateAsync_WithNullExpense_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await this.expenses.UpdateAsync("12345", null!));
    }

    [TestMethod]
    public async Task DeleteAsync_WithNullOrEmptyId_ThrowsArgumentException()
    {
        // Act & Assert for null
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.DeleteAsync(null!));

        // Act & Assert for empty string
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.DeleteAsync(string.Empty));

        // Act & Assert for whitespace
        await Should.ThrowAsync<ArgumentException>(async () =>
            await this.expenses.DeleteAsync("  "));
    }

    [TestMethod]
    public async Task CreateAsync_InvalidatesCache()
    {
        // Arrange
        // First, populate the cache by calling GetAllAsync
        ExpensesRoot initialRoot = new() { Expenses = [] };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.expenses.GetAllAsync();

        // Now create a new expense
        Expense inputExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Test expense",
            User = new Uri("https://api.freeagent.com/v2/users/1")
        };

        Expense responseExpense = inputExpense with { Url = new Uri("https://api.freeagent.com/v2/expenses/12345") };
        ExpenseRoot responseRoot = new() { Expense = responseExpense };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        await this.expenses.CreateAsync(inputExpense);

        // Setup response for next GetAllAsync call with new expense
        ExpensesRoot updatedRoot = new() { Expenses = [responseExpense] };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Get all expenses again
        IEnumerable<Expense> result = await this.expenses.GetAllAsync();

        // Assert - should have made a new request (cache was invalidated)
        this.messageHandler.CallCount.ShouldBe(3); // 1 for initial GetAll, 1 for Create, 1 for second GetAll
        result.Count().ShouldBe(1);
        result.First().Description.ShouldBe("Test expense");
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCache()
    {
        // Arrange
        string expenseId = "12345";

        // First, populate the cache by calling GetByIdAsync
        Expense initialExpense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Original description",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Url = new Uri($"https://api.freeagent.com/v2/expenses/{expenseId}")
        };

        ExpenseRoot initialRoot = new() { Expense = initialExpense };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.expenses.GetByIdAsync(expenseId);

        // Now update the expense
        Expense updatedExpense = initialExpense with { Description = "Updated description" };
        ExpenseRoot updateRoot = new() { Expense = updatedExpense };
        string updateJson = JsonSerializer.Serialize(updateRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updateJson, Encoding.UTF8, "application/json")
        };

        await this.expenses.UpdateAsync(expenseId, updatedExpense);

        // Setup response for next GetByIdAsync call
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updateJson, Encoding.UTF8, "application/json")
        };

        // Get the expense again
        Expense result = await this.expenses.GetByIdAsync(expenseId);

        // Assert - should have made a new request (cache was invalidated)
        this.messageHandler.CallCount.ShouldBe(3); // 1 for initial Get, 1 for Update, 1 for second Get
        result.Description.ShouldBe("Updated description");
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesCache()
    {
        // Arrange
        string expenseId = "12345";

        // First, populate the cache by calling GetByIdAsync
        Expense expense = new()
        {
            DatedOn = new DateOnly(2024, 1, 15),
            Category = "https://api.freeagent.com/v2/categories/285",
            Currency = "GBP",
            GrossValue = 100.00m,
            Description = "Expense to delete",
            User = new Uri("https://api.freeagent.com/v2/users/1"),
            Url = new Uri($"https://api.freeagent.com/v2/expenses/{expenseId}")
        };

        ExpenseRoot root = new() { Expense = expense };
        string json = JsonSerializer.Serialize(root, SharedJsonOptions.Instance);
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        await this.expenses.GetByIdAsync(expenseId);

        // Now delete the expense
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);
        await this.expenses.DeleteAsync(expenseId);

        // Setup response for next GetByIdAsync call (should return 404)
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Try to get the expense again
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.expenses.GetByIdAsync(expenseId));

        // Assert - should have made a new request (cache was invalidated)
        this.messageHandler.CallCount.ShouldBe(3); // 1 for initial Get, 1 for Delete, 1 for second Get attempt
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}

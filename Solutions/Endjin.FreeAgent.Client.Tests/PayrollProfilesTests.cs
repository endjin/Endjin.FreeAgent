// <copyright file="PayrollProfilesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class PayrollProfilesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private PayrollProfiles payrollProfiles = null!;
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
        this.payrollProfiles = new PayrollProfiles(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllProfiles()
    {
        // Arrange
        List<PayrollProfile> profilesList =
        [
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll_profiles/1"),
                Id = 1,
                User = new Uri("https://api.freeagent.com/v2/users/123"),
                TaxCode = "1257L",
                NiLetter = "A",
                AnnualSalary = 36000.00m
            },
            new()
            {
                Url = new Uri("https://api.freeagent.com/v2/payroll_profiles/2"),
                Id = 2,
                User = new Uri("https://api.freeagent.com/v2/users/124"),
                TaxCode = "1257L",
                NiLetter = "A",
                AnnualSalary = 42000.00m
            }
        ];

        PayrollProfilesRoot responseRoot = new() { PayrollProfiles = profilesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollProfile> result = await this.payrollProfiles.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles");
    }

    [TestMethod]
    public async Task GetAsync_WithValidId_ReturnsProfile()
    {
        // Arrange
        PayrollProfile profile = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/payroll_profiles/456"),
            Id = 456,
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            TaxCode = "1257L",
            NiLetter = "A",
            AnnualSalary = 36000.00m,
            PensionContributionPercentage = 5.0m,
            StudentLoanType = "Plan 2"
        };

        PayrollProfileRoot responseRoot = new() { PayrollProfile = profile };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollProfile result = await this.payrollProfiles.GetAsync(456);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(456);
        result.TaxCode.ShouldBe("1257L");
        result.AnnualSalary.ShouldBe(36000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles/456");
    }

    [TestMethod]
    public async Task CreateAsync_WithValidProfile_ReturnsCreatedProfile()
    {
        // Arrange
        PayrollProfile inputProfile = new()
        {
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            TaxCode = "1257L",
            NiLetter = "A",
            AnnualSalary = 36000.00m,
            PensionContributionPercentage = 5.0m
        };

        PayrollProfile responseProfile = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/payroll_profiles/789"),
            Id = 789,
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            TaxCode = "1257L",
            NiLetter = "A",
            AnnualSalary = 36000.00m,
            PensionContributionPercentage = 5.0m
        };

        PayrollProfileRoot responseRoot = new() { PayrollProfile = responseProfile };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollProfile result = await this.payrollProfiles.CreateAsync(inputProfile);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(789);
        result.TaxCode.ShouldBe("1257L");
        result.AnnualSalary.ShouldBe(36000.00m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidProfile_ReturnsUpdatedProfile()
    {
        // Arrange
        PayrollProfile updatedProfile = new()
        {
            TaxCode = "1257L",
            NiLetter = "A",
            AnnualSalary = 38000.00m,
            PensionContributionPercentage = 6.0m
        };

        PayrollProfile responseProfile = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/payroll_profiles/999"),
            Id = 999,
            User = new Uri("https://api.freeagent.com/v2/users/123"),
            TaxCode = "1257L",
            NiLetter = "A",
            AnnualSalary = 38000.00m,
            PensionContributionPercentage = 6.0m
        };

        PayrollProfileRoot responseRoot = new() { PayrollProfile = responseProfile };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        PayrollProfile result = await this.payrollProfiles.UpdateAsync(999, updatedProfile);

        // Assert
        result.ShouldNotBeNull();
        result.AnnualSalary.ShouldBe(38000.00m);
        result.PensionContributionPercentage.ShouldBe(6.0m);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles/999");
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesProfile()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NoContent);

        // Act
        await this.payrollProfiles.DeleteAsync(888);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles/888");
    }
}

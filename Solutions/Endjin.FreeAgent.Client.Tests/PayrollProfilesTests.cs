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
    public async Task GetAllAsync_ReturnsAllProfilesForYear()
    {
        // Arrange
        List<PayrollProfile> profilesList =
        [
            new()
            {
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                AddressLine1 = "123 Main Street",
                AddressLine2 = "Floor 2",
                Postcode = "SW1A 1AA",
                Title = "Mr",
                Gender = "M",
                DateOfBirth = new DateOnly(1985, 6, 15),
                TotalPayInPreviousEmployment = 15000.00m,
                TotalTaxInPreviousEmployment = 3000.00m,
                EmploymentStartsOn = new DateOnly(2024, 5, 1),
                CreatedAt = new DateTimeOffset(2024, 4, 1, 10, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2024, 4, 15, 14, 30, 0, TimeSpan.Zero)
            },
            new()
            {
                User = new Uri("https://api.freeagent.com/v2/users/2"),
                AddressLine1 = "456 High Street",
                Postcode = "M1 2AB",
                Title = "Ms",
                Gender = "F",
                DateOfBirth = new DateOnly(1990, 3, 20),
                CreatedAt = new DateTimeOffset(2024, 4, 1, 11, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2024, 4, 1, 11, 0, 0, TimeSpan.Zero)
            }
        ];

        PayrollProfilesRoot responseRoot = new() { Profiles = [.. profilesList] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollProfile> result = await this.payrollProfiles.GetAllAsync(2025);

        // Assert
        result.Count().ShouldBe(2);

        PayrollProfile firstProfile = result.First();
        firstProfile.User.ShouldBe(new Uri("https://api.freeagent.com/v2/users/1"));
        firstProfile.AddressLine1.ShouldBe("123 Main Street");
        firstProfile.AddressLine2.ShouldBe("Floor 2");
        firstProfile.Postcode.ShouldBe("SW1A 1AA");
        firstProfile.Title.ShouldBe("Mr");
        firstProfile.Gender.ShouldBe("M");
        firstProfile.DateOfBirth.ShouldBe(new DateOnly(1985, 6, 15));
        firstProfile.TotalPayInPreviousEmployment.ShouldBe(15000.00m);
        firstProfile.TotalTaxInPreviousEmployment.ShouldBe(3000.00m);
        firstProfile.EmploymentStartsOn.ShouldBe(new DateOnly(2024, 5, 1));

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles/2025");
    }

    [TestMethod]
    public async Task GetAllAsync_CachesResults()
    {
        // Arrange
        List<PayrollProfile> profilesList =
        [
            new()
            {
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                AddressLine1 = "123 Main Street"
            }
        ];

        PayrollProfilesRoot responseRoot = new() { Profiles = [.. profilesList] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        await this.payrollProfiles.GetAllAsync(2025);
        await this.payrollProfiles.GetAllAsync(2025);

        // Assert - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetAllAsync_UsesDifferentCacheKeyForDifferentYears()
    {
        // Arrange
        List<PayrollProfile> profilesList =
        [
            new()
            {
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                AddressLine1 = "123 Main Street"
            }
        ];

        PayrollProfilesRoot responseRoot = new() { Profiles = [.. profilesList] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call with different years
        await this.payrollProfiles.GetAllAsync(2025);
        await this.payrollProfiles.GetAllAsync(2026);

        // Assert - Should call API twice (different cache keys)
        this.messageHandler.CallCount.ShouldBe(2);
    }

    [TestMethod]
    public async Task GetByUserAsync_ReturnsProfileForUser()
    {
        // Arrange
        Uri userUrl = new("https://api.freeagent.com/v2/users/1");
        List<PayrollProfile> profilesList =
        [
            new()
            {
                User = userUrl,
                AddressLine1 = "123 Main Street",
                Postcode = "SW1A 1AA",
                Title = "Mr",
                Gender = "M",
                DateOfBirth = new DateOnly(1985, 6, 15)
            }
        ];

        PayrollProfilesRoot responseRoot = new() { Profiles = [.. profilesList] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollProfile> result = await this.payrollProfiles.GetByUserAsync(2025, userUrl);

        // Assert
        result.Count().ShouldBe(1);
        result.First().User.ShouldBe(userUrl);

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();

        // Verify the URL contains the user parameter
        string requestUri = this.messageHandler.LastRequest!.RequestUri!.ToString();
        requestUri.ShouldContain("/v2/payroll_profiles/2025");
        requestUri.ShouldContain("user=");
        requestUri.ShouldContain(Uri.EscapeDataString(userUrl.ToString()));
    }

    [TestMethod]
    public async Task GetByUserAsync_WithNullUserUrl_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(
            () => this.payrollProfiles.GetByUserAsync(2025, null!));
    }

    [TestMethod]
    public async Task GetAllAsync_WithEmptyResponse_ReturnsEmptyCollection()
    {
        // Arrange
        PayrollProfilesRoot responseRoot = new() { Profiles = [] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollProfile> result = await this.payrollProfiles.GetAllAsync(2025);

        // Assert
        result.ShouldBeEmpty();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/payroll_profiles/2025");
    }

    [TestMethod]
    public async Task GetAllAsync_DeserializesAllProperties()
    {
        // Arrange
        List<PayrollProfile> profilesList =
        [
            new()
            {
                User = new Uri("https://api.freeagent.com/v2/users/1"),
                AddressLine1 = "Line 1",
                AddressLine2 = "Line 2",
                AddressLine3 = "Line 3",
                AddressLine4 = "Line 4",
                Postcode = "SW1A 1AA",
                Title = "Dr",
                Gender = "M",
                DateOfBirth = new DateOnly(1975, 12, 25),
                TotalPayInPreviousEmployment = 25000.50m,
                TotalTaxInPreviousEmployment = 5000.25m,
                EmploymentStartsOn = new DateOnly(2024, 6, 1),
                CreatedAt = new DateTimeOffset(2024, 4, 1, 9, 0, 0, TimeSpan.Zero),
                UpdatedAt = new DateTimeOffset(2024, 11, 15, 16, 45, 0, TimeSpan.Zero)
            }
        ];

        PayrollProfilesRoot responseRoot = new() { Profiles = [.. profilesList] };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<PayrollProfile> result = await this.payrollProfiles.GetAllAsync(2025);

        // Assert
        PayrollProfile profile = result.Single();
        profile.User.ShouldBe(new Uri("https://api.freeagent.com/v2/users/1"));
        profile.AddressLine1.ShouldBe("Line 1");
        profile.AddressLine2.ShouldBe("Line 2");
        profile.AddressLine3.ShouldBe("Line 3");
        profile.AddressLine4.ShouldBe("Line 4");
        profile.Postcode.ShouldBe("SW1A 1AA");
        profile.Country.ShouldBeNull();
        profile.Title.ShouldBe("Dr");
        profile.Gender.ShouldBe("M");
        profile.DateOfBirth.ShouldBe(new DateOnly(1975, 12, 25));
        profile.TotalPayInPreviousEmployment.ShouldBe(25000.50m);
        profile.TotalTaxInPreviousEmployment.ShouldBe(5000.25m);
        profile.EmploymentStartsOn.ShouldBe(new DateOnly(2024, 6, 1));
        profile.CreatedAt.ShouldBe(new DateTimeOffset(2024, 4, 1, 9, 0, 0, TimeSpan.Zero));
        profile.UpdatedAt.ShouldBe(new DateTimeOffset(2024, 11, 15, 16, 45, 0, TimeSpan.Zero));
    }
}

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
            Type = CompanyType.UkLimitedCompany,
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
        result.Type.ShouldBe(CompanyType.UkLimitedCompany);
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
    public async Task GetAsync_DeserializesComplexProperties()
    {
        // Arrange
        Domain.Company companyDetails = new()
        {
            Id = 12345,
            Url = new Uri("https://api.freeagent.com/v2/company"),
            Name = "Test Company Ltd",
            Subdomain = "testcompany",
            Type = CompanyType.UkLimitedCompany,
            Currency = "GBP",
            MileageUnits = "miles",
            CompanyStartDate = new DateOnly(2020, 1, 1),
            TradingStartDate = new DateOnly(2020, 2, 1),
            FreeagentStartDate = new DateOnly(2020, 3, 1),
            FirstAccountingYearEnd = new DateOnly(2020, 12, 31),
            CompanyRegistrationNumber = "12345678",
            Address1 = "123 Main Street",
            Address2 = "Suite 100",
            Address3 = "Building A",
            Town = "London",
            Region = "Greater London",
            Postcode = "SW1A 1AA",
            Country = "United Kingdom",
            ContactEmail = "info@testcompany.com",
            ContactPhone = "+44 20 7123 4567",
            Website = "https://www.testcompany.com",
            BusinessType = "Software Development",
            BusinessCategory = "Technology",
            ShortDateFormat = Domain.ShortDateFormat.European,
            SalesTaxRegistrationNumber = "GB123456789",
            SalesTaxName = "VAT",
            SalesTaxEffectiveDate = new DateOnly(2020, 4, 1),
            SalesTaxIsValueAdded = true,
            SalesTaxRegistrationStatus = SalesTaxRegistrationStatus.Registered,
            VatFirstReturnPeriodEndsOn = new DateOnly(2020, 6, 30),
            InitialVatBasis = VatBasis.Invoice,
            InitiallyOnFrs = false,
            InitialVatFrsType = null,
            CisEnabled = false,
            CisSubcontractor = false,
            CisContractor = false,
            CreatedAt = new DateTime(2020, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 15, 30, 0, DateTimeKind.Utc),
            SalesTaxRates =
            [
                20.0m,
                5.0m,
                0.0m
            ],
            AnnualAccountingPeriods =
            [
                new() { StartsOn = new DateOnly(2020, 1, 1), EndsOn = new DateOnly(2020, 12, 31) },
                new() { StartsOn = new DateOnly(2021, 1, 1), EndsOn = new DateOnly(2021, 12, 31) },
                new() { StartsOn = new DateOnly(2022, 1, 1), EndsOn = new DateOnly(2022, 12, 31) }
            ],
            LockedAttributes = ["company_start_date", "company_registration_number"]
        };

        CompanyRoot responseRoot = new() { Company = companyDetails };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Domain.Company result = await this.company.GetAsync();

        // Assert - Basic properties
        result.ShouldNotBeNull();
        result.Id.ShouldBe(12345);
        result.Url.ShouldNotBeNull();
        result.Url!.ToString().ShouldBe("https://api.freeagent.com/v2/company");
        result.Name.ShouldBe("Test Company Ltd");
        result.Subdomain.ShouldBe("testcompany");
        result.Type.ShouldBe(CompanyType.UkLimitedCompany);
        result.Currency.ShouldBe("GBP");
        result.MileageUnits.ShouldBe("miles");

        // Assert - Date properties
        result.CompanyStartDate.ShouldBe(new DateOnly(2020, 1, 1));
        result.TradingStartDate.ShouldBe(new DateOnly(2020, 2, 1));
        result.FreeagentStartDate.ShouldBe(new DateOnly(2020, 3, 1));
        result.FirstAccountingYearEnd.ShouldBe(new DateOnly(2020, 12, 31));

        // Assert - Address properties
        result.Address1.ShouldBe("123 Main Street");
        result.Address2.ShouldBe("Suite 100");
        result.Address3.ShouldBe("Building A");
        result.Town.ShouldBe("London");
        result.Region.ShouldBe("Greater London");
        result.Postcode.ShouldBe("SW1A 1AA");
        result.Country.ShouldBe("United Kingdom");

        // Assert - Contact properties
        result.CompanyRegistrationNumber.ShouldBe("12345678");
        result.ContactEmail.ShouldBe("info@testcompany.com");
        result.ContactPhone.ShouldBe("+44 20 7123 4567");
        result.Website.ShouldBe("https://www.testcompany.com");

        // Assert - Business classification
        result.BusinessType.ShouldBe("Software Development");
        result.BusinessCategory.ShouldBe("Technology");
        result.ShortDateFormat.ShouldBe(Domain.ShortDateFormat.European);

        // Assert - Sales tax properties
        result.SalesTaxRegistrationNumber.ShouldBe("GB123456789");
        result.SalesTaxName.ShouldBe("VAT");
        result.SalesTaxEffectiveDate.ShouldBe(new DateOnly(2020, 4, 1));
        result.SalesTaxIsValueAdded.ShouldBe(true);
        result.SalesTaxRegistrationStatus.ShouldBe(SalesTaxRegistrationStatus.Registered);

        // Assert - VAT properties
        result.VatFirstReturnPeriodEndsOn.ShouldBe(new DateOnly(2020, 6, 30));
        result.InitialVatBasis.ShouldBe(VatBasis.Invoice);
        result.InitiallyOnFrs.ShouldBe(false);
        result.InitialVatFrsType.ShouldBeNull();

        // Assert - CIS properties
        result.CisEnabled.ShouldBe(false);
        result.CisSubcontractor.ShouldBe(false);
        result.CisContractor.ShouldBe(false);

        // Assert - Timestamps
        result.CreatedAt.ShouldBe(new DateTime(2020, 1, 1, 10, 0, 0, DateTimeKind.Utc));
        result.UpdatedAt.ShouldBe(new DateTime(2024, 1, 1, 15, 30, 0, DateTimeKind.Utc));

        // Assert - Complex collections
        result.SalesTaxRates.ShouldNotBeNull();
        result.SalesTaxRates!.Count.ShouldBe(3);
        result.SalesTaxRates[0].ShouldBe(20.0m);
        result.SalesTaxRates[1].ShouldBe(5.0m);
        result.SalesTaxRates[2].ShouldBe(0.0m);

        result.AnnualAccountingPeriods.ShouldNotBeNull();
        result.AnnualAccountingPeriods!.Count.ShouldBe(3);
        result.AnnualAccountingPeriods[0].StartsOn.ShouldBe(new DateOnly(2020, 1, 1));
        result.AnnualAccountingPeriods[0].EndsOn.ShouldBe(new DateOnly(2020, 12, 31));
        result.AnnualAccountingPeriods[1].StartsOn.ShouldBe(new DateOnly(2021, 1, 1));
        result.AnnualAccountingPeriods[2].StartsOn.ShouldBe(new DateOnly(2022, 1, 1));

        result.LockedAttributes.ShouldNotBeNull();
        result.LockedAttributes!.Count.ShouldBe(2);
        result.LockedAttributes.ShouldContain("company_start_date");
        result.LockedAttributes.ShouldContain("company_registration_number");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/company");
    }

    [TestMethod]
    public async Task GetBusinessCategoriesAsync_ReturnsListOfCategories()
    {
        // Arrange
        BusinessCategoriesRoot responseRoot = new()
        {
            BusinessCategories = ["Accounting & Bookkeeping", "Administration", "Agriculture"]
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        List<string> result = await this.company.GetBusinessCategoriesAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(3);
        result[0].ShouldBe("Accounting & Bookkeeping");
        result[1].ShouldBe("Administration");
        result[2].ShouldBe("Agriculture");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/company/business_categories");
    }

    [TestMethod]
    public async Task GetBusinessCategoriesAsync_CachesResult()
    {
        // Arrange
        BusinessCategoriesRoot responseRoot = new()
        {
            BusinessCategories = ["Category 1", "Category 2"]
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call twice
        List<string> result1 = await this.company.GetBusinessCategoriesAsync();
        List<string> result2 = await this.company.GetBusinessCategoriesAsync();

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Count.ShouldBe(2);

        // Mock Verification - Should only call API once due to caching
        this.messageHandler.ShouldHaveBeenCalledOnce();
    }

    [TestMethod]
    public async Task GetTaxTimelineAsync_ReturnsTaxEvents()
    {
        // Arrange
        TaxTimelineRoot responseRoot = new()
        {
            TimelineItems =
            [
                new TaxTimelineItem
                {
                    Description = "VAT Return 09 11",
                    Nature = "Electronic Submission and Payment Due",
                    DatedOn = new DateOnly(2024, 12, 7),
                    AmountDue = 1234.56m,
                    IsPersonal = false
                },
                new TaxTimelineItem
                {
                    Description = "Corporation Tax Return Due",
                    Nature = "Filing Due",
                    DatedOn = new DateOnly(2025, 1, 31),
                    AmountDue = null,
                    IsPersonal = false
                }
            ]
        };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        List<TaxTimelineItem> result = await this.company.GetTaxTimelineAsync();

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Description.ShouldBe("VAT Return 09 11");
        result[0].Nature.ShouldBe("Electronic Submission and Payment Due");
        result[0].AmountDue.ShouldBe(1234.56m);
        result[1].Description.ShouldBe("Corporation Tax Return Due");

        // Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/company/tax_timeline");
    }
}

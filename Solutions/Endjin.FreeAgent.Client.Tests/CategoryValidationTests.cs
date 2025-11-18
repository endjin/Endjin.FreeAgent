// <copyright file="CategoryValidationTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;
using Endjin.FreeAgent.Validation;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CategoryValidationTests
{
    [TestMethod]
    [DataRow("001", CategoryGroupType.Income, true)]
    [DataRow("049", CategoryGroupType.Income, true)]
    [DataRow("050", CategoryGroupType.Income, false)]
    [DataRow("096", CategoryGroupType.CostOfSales, true)]
    [DataRow("199", CategoryGroupType.CostOfSales, true)]
    [DataRow("095", CategoryGroupType.CostOfSales, false)]
    [DataRow("200", CategoryGroupType.AdminExpenses, true)]
    [DataRow("399", CategoryGroupType.AdminExpenses, true)]
    [DataRow("400", CategoryGroupType.AdminExpenses, false)]
    [DataRow("671", CategoryGroupType.CurrentAssets, true)]
    [DataRow("720", CategoryGroupType.CurrentAssets, true)]
    [DataRow("670", CategoryGroupType.CurrentAssets, false)]
    [DataRow("731", CategoryGroupType.Liabilities, true)]
    [DataRow("780", CategoryGroupType.Liabilities, true)]
    [DataRow("781", CategoryGroupType.Liabilities, false)]
    [DataRow("921", CategoryGroupType.Equities, true)]
    [DataRow("960", CategoryGroupType.Equities, true)]
    [DataRow("920", CategoryGroupType.Equities, false)]
    public void IsValidNominalCodeForGroup_ValidatesCorrectly(string nominalCode, CategoryGroupType categoryGroup, bool expectedResult)
    {
        // Act
        bool result = CategoryValidation.IsValidNominalCodeForGroup(nominalCode, categoryGroup);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    public void IsValidNominalCodeForGroup_ThrowsForNullNominalCode()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => CategoryValidation.IsValidNominalCodeForGroup(null!, CategoryGroupType.Income));
    }

    [TestMethod]
    [DataRow("abc", CategoryGroupType.Income, false)]
    [DataRow("", CategoryGroupType.Income, false)]
    [DataRow("1.5", CategoryGroupType.Income, false)]
    public void IsValidNominalCodeForGroup_ReturnsFalseForInvalidFormats(string nominalCode, CategoryGroupType categoryGroup, bool expectedResult)
    {
        // Act
        bool result = CategoryValidation.IsValidNominalCodeForGroup(nominalCode, categoryGroup);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow(CategoryGroupType.Income, 1, 49)]
    [DataRow(CategoryGroupType.CostOfSales, 96, 199)]
    [DataRow(CategoryGroupType.AdminExpenses, 200, 399)]
    [DataRow(CategoryGroupType.CurrentAssets, 671, 720)]
    [DataRow(CategoryGroupType.Liabilities, 731, 780)]
    [DataRow(CategoryGroupType.Equities, 921, 960)]
    public void GetNominalCodeRange_ReturnsCorrectRanges(CategoryGroupType categoryGroup, int expectedMin, int expectedMax)
    {
        // Act
        var (min, max) = CategoryValidation.GetNominalCodeRange(categoryGroup);

        // Assert
        min.ShouldBe(expectedMin);
        max.ShouldBe(expectedMax);
    }

    [TestMethod]
    [DataRow(CategoryGroupType.Income, false)]
    [DataRow(CategoryGroupType.CostOfSales, true)]
    [DataRow(CategoryGroupType.AdminExpenses, true)]
    [DataRow(CategoryGroupType.CurrentAssets, true)]
    [DataRow(CategoryGroupType.Liabilities, true)]
    [DataRow(CategoryGroupType.Equities, false)]
    public void IsTaxReportingNameRequired_ValidatesCorrectly(CategoryGroupType categoryGroup, bool expectedResult)
    {
        // Act
        bool result = CategoryValidation.IsTaxReportingNameRequired(categoryGroup);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow(CategoryGroupType.Income, false)]
    [DataRow(CategoryGroupType.CostOfSales, true)]
    [DataRow(CategoryGroupType.AdminExpenses, true)]
    [DataRow(CategoryGroupType.CurrentAssets, false)]
    [DataRow(CategoryGroupType.Liabilities, false)]
    [DataRow(CategoryGroupType.Equities, false)]
    public void IsAllowableForTaxRequired_ValidatesCorrectly(CategoryGroupType categoryGroup, bool expectedResult)
    {
        // Act
        bool result = CategoryValidation.IsAllowableForTaxRequired(categoryGroup);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    [DataRow(CategoryGroupType.Income, true)]
    [DataRow(CategoryGroupType.CostOfSales, true)]
    [DataRow(CategoryGroupType.AdminExpenses, true)]
    [DataRow(CategoryGroupType.CurrentAssets, false)]
    [DataRow(CategoryGroupType.Liabilities, false)]
    [DataRow(CategoryGroupType.Equities, false)]
    public void CanApplyAutoSalesTaxRate_ValidatesCorrectly(CategoryGroupType categoryGroup, bool expectedResult)
    {
        // Act
        bool result = CategoryValidation.CanApplyAutoSalesTaxRate(categoryGroup);

        // Assert
        result.ShouldBe(expectedResult);
    }

    [TestMethod]
    public void IsValidAutoSalesTaxRateForGroup_AllowsNullForAllGroups()
    {
        // Act & Assert
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(null, CategoryGroupType.Income).ShouldBeTrue();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(null, CategoryGroupType.CostOfSales).ShouldBeTrue();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(null, CategoryGroupType.AdminExpenses).ShouldBeTrue();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(null, CategoryGroupType.CurrentAssets).ShouldBeTrue();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(null, CategoryGroupType.Liabilities).ShouldBeTrue();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(null, CategoryGroupType.Equities).ShouldBeTrue();
    }

    [TestMethod]
    public void IsValidAutoSalesTaxRateForGroup_ExemptOnlyValidForIncome()
    {
        // Act & Assert
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType.Exempt, CategoryGroupType.Income).ShouldBeTrue();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType.Exempt, CategoryGroupType.CostOfSales).ShouldBeFalse();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType.Exempt, CategoryGroupType.AdminExpenses).ShouldBeFalse();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType.Exempt, CategoryGroupType.CurrentAssets).ShouldBeFalse();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType.Exempt, CategoryGroupType.Liabilities).ShouldBeFalse();
        CategoryValidation.IsValidAutoSalesTaxRateForGroup(AutoSalesTaxRateType.Exempt, CategoryGroupType.Equities).ShouldBeFalse();
    }

    [TestMethod]
    public void IsValidAutoSalesTaxRateForGroup_OtherRatesValidForIncomeAndSpending()
    {
        AutoSalesTaxRateType[] rates =
        [
            AutoSalesTaxRateType.OutsideScope,
            AutoSalesTaxRateType.ZeroRate,
            AutoSalesTaxRateType.ReducedRate,
            AutoSalesTaxRateType.StandardRate
        ];

        foreach (var rate in rates)
        {
            // Valid for income and spending categories
            CategoryValidation.IsValidAutoSalesTaxRateForGroup(rate, CategoryGroupType.Income).ShouldBeTrue();
            CategoryValidation.IsValidAutoSalesTaxRateForGroup(rate, CategoryGroupType.CostOfSales).ShouldBeTrue();
            CategoryValidation.IsValidAutoSalesTaxRateForGroup(rate, CategoryGroupType.AdminExpenses).ShouldBeTrue();

            // Invalid for other categories
            CategoryValidation.IsValidAutoSalesTaxRateForGroup(rate, CategoryGroupType.CurrentAssets).ShouldBeFalse();
            CategoryValidation.IsValidAutoSalesTaxRateForGroup(rate, CategoryGroupType.Liabilities).ShouldBeFalse();
            CategoryValidation.IsValidAutoSalesTaxRateForGroup(rate, CategoryGroupType.Equities).ShouldBeFalse();
        }
    }

    [TestMethod]
    public void ValidateCategoryCreateRequest_ValidRequest_ReturnsNoErrors()
    {
        // Arrange
        CategoryCreateRequest request = new()
        {
            NominalCode = "250",
            Description = "Office Supplies",
            CategoryGroup = CategoryGroupType.AdminExpenses,
            TaxReportingName = "Other business expenses",
            AllowableForTax = true,
            AutoSalesTaxRate = AutoSalesTaxRateType.StandardRate
        };

        // Act
        IEnumerable<string> errors = CategoryValidation.ValidateCategoryCreateRequest(request);

        // Assert
        errors.ShouldBeEmpty();
    }

    [TestMethod]
    public void ValidateCategoryCreateRequest_InvalidNominalCodeRange_ReturnsError()
    {
        // Arrange
        CategoryCreateRequest request = new()
        {
            NominalCode = "500", // Invalid for AdminExpenses (should be 200-399)
            Description = "Office Supplies",
            CategoryGroup = CategoryGroupType.AdminExpenses,
            TaxReportingName = "Other business expenses",
            AllowableForTax = true
        };

        // Act
        IEnumerable<string> errors = CategoryValidation.ValidateCategoryCreateRequest(request);

        // Assert
        errors.ShouldNotBeEmpty();
        errors.First().ShouldContain("Nominal code 500 is not valid for category group AdminExpenses");
    }

    [TestMethod]
    public void ValidateCategoryCreateRequest_MissingRequiredTaxReportingName_ReturnsError()
    {
        // Arrange
        CategoryCreateRequest request = new()
        {
            NominalCode = "250",
            Description = "Office Supplies",
            CategoryGroup = CategoryGroupType.AdminExpenses,
            TaxReportingName = null, // Required for AdminExpenses
            AllowableForTax = true
        };

        // Act
        IEnumerable<string> errors = CategoryValidation.ValidateCategoryCreateRequest(request);

        // Assert
        errors.ShouldNotBeEmpty();
        errors.Any(e => e.Contains("Tax reporting name is required")).ShouldBeTrue();
    }

    [TestMethod]
    public void ValidateCategoryCreateRequest_MissingRequiredAllowableForTax_ReturnsError()
    {
        // Arrange
        CategoryCreateRequest request = new()
        {
            NominalCode = "250",
            Description = "Office Supplies",
            CategoryGroup = CategoryGroupType.AdminExpenses,
            TaxReportingName = "Other business expenses",
            AllowableForTax = null // Required for AdminExpenses
        };

        // Act
        IEnumerable<string> errors = CategoryValidation.ValidateCategoryCreateRequest(request);

        // Assert
        errors.ShouldNotBeEmpty();
        errors.Any(e => e.Contains("Allowable for tax is required")).ShouldBeTrue();
    }

    [TestMethod]
    public void ValidateCategoryCreateRequest_ExemptTaxRateOnNonIncome_ReturnsError()
    {
        // Arrange
        CategoryCreateRequest request = new()
        {
            NominalCode = "250",
            Description = "Office Supplies",
            CategoryGroup = CategoryGroupType.AdminExpenses,
            TaxReportingName = "Other business expenses",
            AllowableForTax = true,
            AutoSalesTaxRate = AutoSalesTaxRateType.Exempt // Invalid for non-income
        };

        // Act
        IEnumerable<string> errors = CategoryValidation.ValidateCategoryCreateRequest(request);

        // Assert
        errors.ShouldNotBeEmpty();
        errors.Any(e => e.Contains("Auto sales tax rate 'Exempt' is only valid for income categories")).ShouldBeTrue();
    }

    [TestMethod]
    public void ValidateCategoryCreateRequest_ThrowsForNullRequest()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => CategoryValidation.ValidateCategoryCreateRequest(null!));
    }

    [TestMethod]
    [DataRow(CategoryGroupType.Income, "001-049")]
    [DataRow(CategoryGroupType.CostOfSales, "096-199")]
    [DataRow(CategoryGroupType.AdminExpenses, "200-399")]
    [DataRow(CategoryGroupType.CurrentAssets, "671-720")]
    [DataRow(CategoryGroupType.Liabilities, "731-780")]
    [DataRow(CategoryGroupType.Equities, "921-960")]
    public void GetNominalCodeRangeDescription_ReturnsCorrectDescription(CategoryGroupType categoryGroup, string expectedDescription)
    {
        // Act
        string description = CategoryValidation.GetNominalCodeRangeDescription(categoryGroup);

        // Assert
        description.ShouldBe(expectedDescription);
    }
}
// <copyright file="CategoryEnumConverterTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;
using Endjin.FreeAgent.Converters;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class CategoryEnumConverterTests
{
    private JsonSerializerOptions options = null!;

    [TestInitialize]
    public void Setup()
    {
        this.options = new JsonSerializerOptions
        {
            Converters =
            {
                new CategoryGroupTypeJsonConverter(),
                new CategoryGroupTypeNonNullableJsonConverter(),
                new AutoSalesTaxRateTypeJsonConverter(),
                new AutoSalesTaxRateTypeNonNullableJsonConverter()
            }
        };
    }

    #region CategoryGroupType Tests

    [TestMethod]
    [DataRow(CategoryGroupType.Income, "\"income\"")]
    [DataRow(CategoryGroupType.CostOfSales, "\"cost_of_sales\"")]
    [DataRow(CategoryGroupType.AdminExpenses, "\"admin_expenses\"")]
    [DataRow(CategoryGroupType.CurrentAssets, "\"current_assets\"")]
    [DataRow(CategoryGroupType.Liabilities, "\"liabilities\"")]
    [DataRow(CategoryGroupType.Equities, "\"equities\"")]
    public void CategoryGroupType_SerializesCorrectly(CategoryGroupType value, string expectedJson)
    {
        // Act
        string json = JsonSerializer.Serialize(value, this.options);

        // Assert
        json.ShouldBe(expectedJson);
    }

    [TestMethod]
    [DataRow("\"income\"", CategoryGroupType.Income)]
    [DataRow("\"cost_of_sales\"", CategoryGroupType.CostOfSales)]
    [DataRow("\"admin_expenses\"", CategoryGroupType.AdminExpenses)]
    [DataRow("\"current_assets\"", CategoryGroupType.CurrentAssets)]
    [DataRow("\"liabilities\"", CategoryGroupType.Liabilities)]
    [DataRow("\"equities\"", CategoryGroupType.Equities)]
    public void CategoryGroupType_DeserializesCorrectly(string json, CategoryGroupType expectedValue)
    {
        // Act
        CategoryGroupType? result = JsonSerializer.Deserialize<CategoryGroupType>(json, this.options);

        // Assert
        result.ShouldBe(expectedValue);
    }

    [TestMethod]
    public void CategoryGroupType_DeserializesNullCorrectly()
    {
        // Act
        CategoryGroupType? result = JsonSerializer.Deserialize<CategoryGroupType?>("null", this.options);

        // Assert
        result.ShouldBeNull();
    }

    [TestMethod]
    public void CategoryGroupType_SerializesNullCorrectly()
    {
        // Arrange
        CategoryGroupType? value = null;

        // Act
        string json = JsonSerializer.Serialize(value, this.options);

        // Assert
        json.ShouldBe("null");
    }

    [TestMethod]
    public void CategoryGroupType_ThrowsForInvalidValue()
    {
        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<CategoryGroupType>("\"invalid_group\"", this.options));
    }

    [TestMethod]
    public void CategoryGroupTypeNonNullable_ThrowsForNull()
    {
        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<CategoryGroupType>("null", this.options));
    }

    #endregion

    #region AutoSalesTaxRateType Tests

    [TestMethod]
    [DataRow(AutoSalesTaxRateType.OutsideScope, "\"Outside of the scope of VAT\"")]
    [DataRow(AutoSalesTaxRateType.ZeroRate, "\"Zero rate\"")]
    [DataRow(AutoSalesTaxRateType.ReducedRate, "\"Reduced rate\"")]
    [DataRow(AutoSalesTaxRateType.StandardRate, "\"Standard rate\"")]
    [DataRow(AutoSalesTaxRateType.Exempt, "\"Exempt\"")]
    public void AutoSalesTaxRateType_SerializesCorrectly(AutoSalesTaxRateType value, string expectedJson)
    {
        // Act
        string json = JsonSerializer.Serialize(value, this.options);

        // Assert
        json.ShouldBe(expectedJson);
    }

    [TestMethod]
    [DataRow("\"Outside of the scope of VAT\"", AutoSalesTaxRateType.OutsideScope)]
    [DataRow("\"Zero rate\"", AutoSalesTaxRateType.ZeroRate)]
    [DataRow("\"Reduced rate\"", AutoSalesTaxRateType.ReducedRate)]
    [DataRow("\"Standard rate\"", AutoSalesTaxRateType.StandardRate)]
    [DataRow("\"Exempt\"", AutoSalesTaxRateType.Exempt)]
    public void AutoSalesTaxRateType_DeserializesCorrectly(string json, AutoSalesTaxRateType expectedValue)
    {
        // Act
        AutoSalesTaxRateType? result = JsonSerializer.Deserialize<AutoSalesTaxRateType>(json, this.options);

        // Assert
        result.ShouldBe(expectedValue);
    }

    [TestMethod]
    public void AutoSalesTaxRateType_DeserializesNullCorrectly()
    {
        // Act
        AutoSalesTaxRateType? result = JsonSerializer.Deserialize<AutoSalesTaxRateType?>("null", this.options);

        // Assert
        result.ShouldBeNull();
    }

    [TestMethod]
    public void AutoSalesTaxRateType_SerializesNullCorrectly()
    {
        // Arrange
        AutoSalesTaxRateType? value = null;

        // Act
        string json = JsonSerializer.Serialize(value, this.options);

        // Assert
        json.ShouldBe("null");
    }

    [TestMethod]
    public void AutoSalesTaxRateType_ThrowsForInvalidValue()
    {
        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<AutoSalesTaxRateType>("\"Invalid rate\"", this.options));
    }

    [TestMethod]
    public void AutoSalesTaxRateTypeNonNullable_ThrowsForNull()
    {
        // Act & Assert
        Should.Throw<JsonException>(() => JsonSerializer.Deserialize<AutoSalesTaxRateType>("null", this.options));
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public void CategoryCreateRequest_SerializesCorrectlyWithEnums()
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
        string json = JsonSerializer.Serialize(request, SharedJsonOptions.Instance);

        // Assert
        json.ShouldContain("\"nominal_code\":\"250\"");
        json.ShouldContain("\"description\":\"Office Supplies\"");
        json.ShouldContain("\"category_group\":\"admin_expenses\"");
        json.ShouldContain("\"tax_reporting_name\":\"Other business expenses\"");
        json.ShouldContain("\"allowable_for_tax\":true");
        json.ShouldContain("\"auto_sales_tax_rate\":\"Standard rate\"");
    }

    [TestMethod]
    public void CategoryUpdateRequest_SerializesCorrectlyWithEnums()
    {
        // Arrange
        CategoryUpdateRequest request = new()
        {
            Description = "Updated Description",
            TaxReportingName = "Miscellaneous expenses",
            AllowableForTax = false,
            AutoSalesTaxRate = AutoSalesTaxRateType.ZeroRate
        };

        // Act
        string json = JsonSerializer.Serialize(request, SharedJsonOptions.Instance);

        // Assert
        json.ShouldContain("\"description\":\"Updated Description\"");
        json.ShouldContain("\"tax_reporting_name\":\"Miscellaneous expenses\"");
        json.ShouldContain("\"allowable_for_tax\":false");
        json.ShouldContain("\"auto_sales_tax_rate\":\"Zero rate\"");
    }

    [TestMethod]
    public void CategoryCreateRequest_DeserializesCorrectlyWithEnums()
    {
        // Arrange
        string json = @"{
            ""nominal_code"": ""250"",
            ""description"": ""Office Supplies"",
            ""category_group"": ""admin_expenses"",
            ""tax_reporting_name"": ""Other business expenses"",
            ""allowable_for_tax"": true,
            ""auto_sales_tax_rate"": ""Standard rate""
        }";

        // Act
        CategoryCreateRequest? result = JsonSerializer.Deserialize<CategoryCreateRequest>(json, SharedJsonOptions.Instance);

        // Assert
        result.ShouldNotBeNull();
        result.NominalCode.ShouldBe("250");
        result.Description.ShouldBe("Office Supplies");
        result.CategoryGroup.ShouldBe(CategoryGroupType.AdminExpenses);
        result.TaxReportingName.ShouldBe("Other business expenses");
        result.AllowableForTax.ShouldBe(true);
        result.AutoSalesTaxRate.ShouldBe(AutoSalesTaxRateType.StandardRate);
    }

    #endregion
}
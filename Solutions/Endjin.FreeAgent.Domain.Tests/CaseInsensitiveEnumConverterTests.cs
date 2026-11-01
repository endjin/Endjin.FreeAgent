// <copyright file="CaseInsensitiveEnumConverterTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;

using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain.Tests;

/// <summary>
/// The enum converters match the FreeAgent API's string values case-insensitively, so a change in
/// the casing the API uses cannot fail deserialization. Unknown values must still be rejected.
/// </summary>
[TestClass]
public class CaseInsensitiveEnumConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        Converters =
        {
            new AutoSalesTaxRateTypeJsonConverter(),
            new CategoryGroupTypeJsonConverter(),
            new EcStatusJsonConverter(),
            new EngineTypeJsonConverter(),
            new RecurringPatternJsonConverter(),
            new VehicleTypeJsonConverter(),
        },
    };

    [TestMethod]
    [DataRow("Outside of the scope of VAT", AutoSalesTaxRateType.OutsideScope)]
    [DataRow("OUTSIDE OF THE SCOPE OF VAT", AutoSalesTaxRateType.OutsideScope)]
    [DataRow("zero rate", AutoSalesTaxRateType.ZeroRate)]
    [DataRow("Reduced Rate", AutoSalesTaxRateType.ReducedRate)]
    [DataRow("STANDARD RATE", AutoSalesTaxRateType.StandardRate)]
    [DataRow("exempt", AutoSalesTaxRateType.Exempt)]
    public void AutoSalesTaxRateType_DeserializesRegardlessOfCase(string value, AutoSalesTaxRateType expected)
    {
        Deserialize<AutoSalesTaxRateType?>(value).ShouldBe(expected);
    }

    [TestMethod]
    [DataRow("income", CategoryGroupType.Income)]
    [DataRow("Cost_Of_Sales", CategoryGroupType.CostOfSales)]
    [DataRow("ADMIN_EXPENSES", CategoryGroupType.AdminExpenses)]
    [DataRow("Current_assets", CategoryGroupType.CurrentAssets)]
    [DataRow("LIABILITIES", CategoryGroupType.Liabilities)]
    [DataRow("Equities", CategoryGroupType.Equities)]
    public void CategoryGroupType_DeserializesRegardlessOfCase(string value, CategoryGroupType expected)
    {
        Deserialize<CategoryGroupType?>(value).ShouldBe(expected);
    }

    [TestMethod]
    [DataRow("UK/Non-EC", EcStatus.UkNonEc)]
    [DataRow("uk/non-ec", EcStatus.UkNonEc)]
    [DataRow("ec goods", EcStatus.EcGoods)]
    [DataRow("EC SERVICES", EcStatus.EcServices)]
    [DataRow("reverse charge", EcStatus.ReverseCharge)]
    [DataRow("Ec Vat Moss", EcStatus.EcVatMoss)]
    public void EcStatus_DeserializesRegardlessOfCase(string value, EcStatus expected)
    {
        Deserialize<EcStatus?>(value).ShouldBe(expected);
    }

    [TestMethod]
    [DataRow("Petrol", EngineType.Petrol)]
    [DataRow("petrol", EngineType.Petrol)]
    [DataRow("DIESEL", EngineType.Diesel)]
    [DataRow("lpg", EngineType.Lpg)]
    [DataRow("electric", EngineType.Electric)]
    [DataRow("Electric (home charger)", EngineType.ElectricHomeCharger)]
    [DataRow("ELECTRIC (PUBLIC CHARGER)", EngineType.ElectricPublicCharger)]
    public void EngineType_DeserializesRegardlessOfCase(string value, EngineType expected)
    {
        Deserialize<EngineType?>(value).ShouldBe(expected);
    }

    [TestMethod]
    [DataRow("Weekly", RecurringPattern.Weekly)]
    [DataRow("weekly", RecurringPattern.Weekly)]
    [DataRow("two weekly", RecurringPattern.TwoWeekly)]
    [DataRow("FOUR WEEKLY", RecurringPattern.FourWeekly)]
    [DataRow("monthly", RecurringPattern.Monthly)]
    [DataRow("Two monthly", RecurringPattern.TwoMonthly)]
    [DataRow("QUARTERLY", RecurringPattern.Quarterly)]
    [DataRow("biannually", RecurringPattern.Biannually)]
    [DataRow("ANNUALLY", RecurringPattern.Annually)]
    [DataRow("2-yearly", RecurringPattern.TwoYearly)]
    public void RecurringPattern_DeserializesRegardlessOfCase(string value, RecurringPattern expected)
    {
        Deserialize<RecurringPattern?>(value).ShouldBe(expected);
    }

    [TestMethod]
    [DataRow("Car", VehicleType.Car)]
    [DataRow("car", VehicleType.Car)]
    [DataRow("MOTORCYCLE", VehicleType.Motorcycle)]
    [DataRow("bicycle", VehicleType.Bicycle)]
    public void VehicleType_DeserializesRegardlessOfCase(string value, VehicleType expected)
    {
        Deserialize<VehicleType?>(value).ShouldBe(expected);
    }

    [TestMethod]
    public void UnknownValues_StillThrow()
    {
        Should.Throw<JsonException>(() => Deserialize<AutoSalesTaxRateType?>("Super reduced rate"));
        Should.Throw<JsonException>(() => Deserialize<CategoryGroupType?>("fixed_assets"));
        Should.Throw<JsonException>(() => Deserialize<EcStatus?>("EC Nonsense"));
        Should.Throw<JsonException>(() => Deserialize<EngineType?>("Steam"));
        Should.Throw<JsonException>(() => Deserialize<RecurringPattern?>("Fortnightly"));
        Should.Throw<JsonException>(() => Deserialize<VehicleType?>("Van"));
    }

    [TestMethod]
    public void Serialization_StillWritesTheDocumentedCasing()
    {
        JsonSerializer.Serialize<AutoSalesTaxRateType?>(AutoSalesTaxRateType.OutsideScope, Options).ShouldBe("\"Outside of the scope of VAT\"");
        JsonSerializer.Serialize<CategoryGroupType?>(CategoryGroupType.CostOfSales, Options).ShouldBe("\"cost_of_sales\"");
        JsonSerializer.Serialize<EcStatus?>(EcStatus.EcVatMoss, Options).ShouldBe("\"EC VAT MOSS\"");
        JsonSerializer.Serialize<EngineType?>(EngineType.ElectricHomeCharger, Options).ShouldBe("\"Electric (Home charger)\"");
        JsonSerializer.Serialize<RecurringPattern?>(RecurringPattern.TwoYearly, Options).ShouldBe("\"2-Yearly\"");
        JsonSerializer.Serialize<VehicleType?>(VehicleType.Motorcycle, Options).ShouldBe("\"Motorcycle\"");
    }

    private static T? Deserialize<T>(string value)
    {
        return JsonSerializer.Deserialize<T>($"\"{value}\"", Options);
    }
}
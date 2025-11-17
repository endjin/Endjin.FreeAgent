// <copyright file="CompanyTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain.Converters;

/// <summary>
/// JSON converter for <see cref="CompanyType"/> enum that handles snake_case string conversion.
/// </summary>
/// <remarks>
/// <para>
/// Converts between FreeAgent API's snake_case strings (e.g., "uk_limited_company") and
/// the <see cref="CompanyType"/> enum values (e.g., CompanyType.UkLimitedCompany).
/// </para>
/// </remarks>
public class CompanyTypeJsonConverter : JsonConverter<CompanyType?>
{
    /// <inheritdoc/>
    public override CompanyType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        return value switch
        {
            "uk_limited_company" => CompanyType.UkLimitedCompany,
            "uk_limited_liability_partnership" => CompanyType.UkLimitedLiabilityPartnership,
            "uk_partnership" => CompanyType.UkPartnership,
            "uk_sole_trader" => CompanyType.UkSoleTrader,
            "uk_unincorporated_landlord" => CompanyType.UkUnincorporatedLandlord,
            "us_limited_liability_company" => CompanyType.UsLimitedLiabilityCompany,
            "us_partnership" => CompanyType.UsPartnership,
            "us_sole_proprietor" => CompanyType.UsSoleProprietor,
            "us_c_corp" => CompanyType.UsCCorp,
            "us_s_corp" => CompanyType.UsSCorp,
            "universal_company" => CompanyType.UniversalCompany,
            _ => throw new JsonException($"Unknown company type value: {value}")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, CompanyType? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            CompanyType.UkLimitedCompany => "uk_limited_company",
            CompanyType.UkLimitedLiabilityPartnership => "uk_limited_liability_partnership",
            CompanyType.UkPartnership => "uk_partnership",
            CompanyType.UkSoleTrader => "uk_sole_trader",
            CompanyType.UkUnincorporatedLandlord => "uk_unincorporated_landlord",
            CompanyType.UsLimitedLiabilityCompany => "us_limited_liability_company",
            CompanyType.UsPartnership => "us_partnership",
            CompanyType.UsSoleProprietor => "us_sole_proprietor",
            CompanyType.UsCCorp => "us_c_corp",
            CompanyType.UsSCorp => "us_s_corp",
            CompanyType.UniversalCompany => "universal_company",
            _ => throw new JsonException($"Unknown company type enum value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
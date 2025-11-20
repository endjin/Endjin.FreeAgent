// <copyright file="CompanyTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Converters;

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

        // Normalize: lowercase and remove underscores to handle both snake_case and PascalCase
        string normalized = value.ToLowerInvariant().Replace("_", "");

        return normalized switch
        {
            "uklimitedcompany" => CompanyType.UkLimitedCompany,
            "uklimitedliabilitypartnership" => CompanyType.UkLimitedLiabilityPartnership,
            "ukpartnership" => CompanyType.UkPartnership,
            "uksoletrader" => CompanyType.UkSoleTrader,
            "ukunincorporatedlandlord" => CompanyType.UkUnincorporatedLandlord,
            "uslimitedliabilitycompany" => CompanyType.UsLimitedLiabilityCompany,
            "uspartnership" => CompanyType.UsPartnership,
            "ussoleproprietor" => CompanyType.UsSoleProprietor,
            "usccorp" => CompanyType.UsCCorp,
            "usscorp" => CompanyType.UsSCorp,
            "universalcompany" => CompanyType.UniversalCompany,
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
            CompanyType.UkLimitedCompany => "UkLimitedCompany",
            CompanyType.UkLimitedLiabilityPartnership => "UkLimitedLiabilityPartnership",
            CompanyType.UkPartnership => "UkPartnership",
            CompanyType.UkSoleTrader => "UkSoleTrader",
            CompanyType.UkUnincorporatedLandlord => "UkUnincorporatedLandlord",
            CompanyType.UsLimitedLiabilityCompany => "UsLimitedLiabilityCompany",
            CompanyType.UsPartnership => "UsPartnership",
            CompanyType.UsSoleProprietor => "UsSoleProprietor",
            CompanyType.UsCCorp => "UsCCorp",
            CompanyType.UsSCorp => "UsSCorp",
            CompanyType.UniversalCompany => "UniversalCompany",
            _ => throw new JsonException($"Unknown company type enum value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
// <copyright file="AutoSalesTaxRateTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="AutoSalesTaxRateType"/> enum that handles
/// string formats from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents auto sales tax rate values as descriptive strings
/// (e.g., "Outside of the scope of VAT", "Zero rate"). This converter handles deserialization of
/// these formats, normalizing them to the appropriate <see cref="AutoSalesTaxRateType"/> enum value.
/// </para>
/// <para>
/// During serialization, auto sales tax rate values are converted to the exact string format
/// expected by the API.
/// </para>
/// </remarks>
/// <seealso cref="AutoSalesTaxRateType"/>
/// <seealso cref="AutoSalesTaxRateTypeNonNullableJsonConverter"/>
public class AutoSalesTaxRateTypeJsonConverter : JsonConverter<AutoSalesTaxRateType?>
{
    /// <summary>
    /// Reads a nullable <see cref="AutoSalesTaxRateType"/> value from JSON, handling API string formats.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (AutoSalesTaxRateType?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="AutoSalesTaxRateType"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="AutoSalesTaxRateType"/> enum value.
    /// </exception>
    public override AutoSalesTaxRateType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        // API uses descriptive string values
        return value switch
        {
            "Outside of the scope of VAT" => AutoSalesTaxRateType.OutsideScope,
            "Zero rate" => AutoSalesTaxRateType.ZeroRate,
            "Reduced rate" => AutoSalesTaxRateType.ReducedRate,
            "Standard rate" => AutoSalesTaxRateType.StandardRate,
            "Exempt" => AutoSalesTaxRateType.Exempt,
            _ => throw new JsonException($"Unable to convert '{value}' to AutoSalesTaxRateType enum")
        };
    }

    /// <summary>
    /// Writes a nullable <see cref="AutoSalesTaxRateType"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="AutoSalesTaxRateType"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="AutoSalesTaxRateType"/> enum value.
    /// </exception>
    /// <remarks>
    /// AutoSalesTaxRateType values are serialized to descriptive strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="AutoSalesTaxRateType.OutsideScope"/> → "Outside of the scope of VAT"</item>
    /// <item><see cref="AutoSalesTaxRateType.ZeroRate"/> → "Zero rate"</item>
    /// <item><see cref="AutoSalesTaxRateType.ReducedRate"/> → "Reduced rate"</item>
    /// <item><see cref="AutoSalesTaxRateType.StandardRate"/> → "Standard rate"</item>
    /// <item><see cref="AutoSalesTaxRateType.Exempt"/> → "Exempt"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, AutoSalesTaxRateType? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to API-expected strings
        string stringValue = value switch
        {
            AutoSalesTaxRateType.OutsideScope => "Outside of the scope of VAT",
            AutoSalesTaxRateType.ZeroRate => "Zero rate",
            AutoSalesTaxRateType.ReducedRate => "Reduced rate",
            AutoSalesTaxRateType.StandardRate => "Standard rate",
            AutoSalesTaxRateType.Exempt => "Exempt",
            _ => throw new JsonException($"Unknown AutoSalesTaxRateType value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
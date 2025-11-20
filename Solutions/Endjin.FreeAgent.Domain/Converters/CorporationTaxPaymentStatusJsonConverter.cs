// <copyright file="CorporationTaxPaymentStatusJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="CorporationTaxPaymentStatus"/> enum that handles the
/// FreeAgent API's string format for Corporation Tax payment status values.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents payment status values as strings:
/// "unpaid" and "marked_as_paid". This field is omitted if no payment is required.
/// This converter handles the deserialization of these string values to the appropriate
/// <see cref="CorporationTaxPaymentStatus"/> enum value.
/// </para>
/// <para>
/// During serialization, status values are converted back to the API-expected format.
/// </para>
/// </remarks>
/// <seealso cref="CorporationTaxPaymentStatus"/>
/// <seealso cref="CorporationTaxPaymentStatusNonNullableJsonConverter"/>
public class CorporationTaxPaymentStatusJsonConverter : JsonConverter<CorporationTaxPaymentStatus?>
{
    /// <summary>
    /// Reads a nullable <see cref="CorporationTaxPaymentStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (CorporationTaxPaymentStatus?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="CorporationTaxPaymentStatus"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="CorporationTaxPaymentStatus"/> enum value.
    /// </exception>
    public override CorporationTaxPaymentStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        // Normalize the value: convert to lowercase for case-insensitive comparison
        string normalizedValue = value.ToLowerInvariant();

        return normalizedValue switch
        {
            "unpaid" => CorporationTaxPaymentStatus.Unpaid,
            "marked_as_paid" => CorporationTaxPaymentStatus.MarkedAsPaid,
            _ => throw new JsonException($"Unable to convert '{value}' to CorporationTaxPaymentStatus enum")
        };
    }

    /// <summary>
    /// Writes a nullable <see cref="CorporationTaxPaymentStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="CorporationTaxPaymentStatus"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="CorporationTaxPaymentStatus"/> enum value.
    /// </exception>
    /// <remarks>
    /// Status values are serialized to strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="CorporationTaxPaymentStatus.Unpaid"/> → "unpaid"</item>
    /// <item><see cref="CorporationTaxPaymentStatus.MarkedAsPaid"/> → "marked_as_paid"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, CorporationTaxPaymentStatus? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to the API format
        string stringValue = value switch
        {
            CorporationTaxPaymentStatus.Unpaid => "unpaid",
            CorporationTaxPaymentStatus.MarkedAsPaid => "marked_as_paid",
            _ => throw new JsonException($"Unknown CorporationTaxPaymentStatus value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
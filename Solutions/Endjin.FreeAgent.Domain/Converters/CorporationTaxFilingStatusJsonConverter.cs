// <copyright file="CorporationTaxFilingStatusJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="CorporationTaxFilingStatus"/> enum that handles the
/// FreeAgent API's string format for Corporation Tax filing status values.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents filing status values as strings:
/// "draft", "unfiled", "pending", "rejected", "filed", and "marked_as_filed".
/// This converter handles the deserialization of these string values to the appropriate
/// <see cref="CorporationTaxFilingStatus"/> enum value.
/// </para>
/// <para>
/// During serialization, status values are converted back to the API-expected format.
/// </para>
/// </remarks>
/// <seealso cref="CorporationTaxFilingStatus"/>
/// <seealso cref="CorporationTaxFilingStatusNonNullableJsonConverter"/>
public class CorporationTaxFilingStatusJsonConverter : JsonConverter<CorporationTaxFilingStatus?>
{
    /// <summary>
    /// Reads a nullable <see cref="CorporationTaxFilingStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (CorporationTaxFilingStatus?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="CorporationTaxFilingStatus"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="CorporationTaxFilingStatus"/> enum value.
    /// </exception>
    public override CorporationTaxFilingStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            "draft" => CorporationTaxFilingStatus.Draft,
            "unfiled" => CorporationTaxFilingStatus.Unfiled,
            "pending" => CorporationTaxFilingStatus.Pending,
            "rejected" => CorporationTaxFilingStatus.Rejected,
            "filed" => CorporationTaxFilingStatus.Filed,
            "marked_as_filed" => CorporationTaxFilingStatus.MarkedAsFiled,
            _ => throw new JsonException($"Unable to convert '{value}' to CorporationTaxFilingStatus enum")
        };
    }

    /// <summary>
    /// Writes a nullable <see cref="CorporationTaxFilingStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="CorporationTaxFilingStatus"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="CorporationTaxFilingStatus"/> enum value.
    /// </exception>
    /// <remarks>
    /// Status values are serialized to strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="CorporationTaxFilingStatus.Draft"/> → "draft"</item>
    /// <item><see cref="CorporationTaxFilingStatus.Unfiled"/> → "unfiled"</item>
    /// <item><see cref="CorporationTaxFilingStatus.Pending"/> → "pending"</item>
    /// <item><see cref="CorporationTaxFilingStatus.Rejected"/> → "rejected"</item>
    /// <item><see cref="CorporationTaxFilingStatus.Filed"/> → "filed"</item>
    /// <item><see cref="CorporationTaxFilingStatus.MarkedAsFiled"/> → "marked_as_filed"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, CorporationTaxFilingStatus? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to the API format
        string stringValue = value switch
        {
            CorporationTaxFilingStatus.Draft => "draft",
            CorporationTaxFilingStatus.Unfiled => "unfiled",
            CorporationTaxFilingStatus.Pending => "pending",
            CorporationTaxFilingStatus.Rejected => "rejected",
            CorporationTaxFilingStatus.Filed => "filed",
            CorporationTaxFilingStatus.MarkedAsFiled => "marked_as_filed",
            _ => throw new JsonException($"Unknown CorporationTaxFilingStatus value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
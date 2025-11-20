// <copyright file="CorporationTaxFilingStatusNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom non-nullable JSON converter for the <see cref="CorporationTaxFilingStatus"/> enum that handles the
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
/// This converter is for non-nullable <see cref="CorporationTaxFilingStatus"/> properties.
/// For nullable properties, use <see cref="CorporationTaxFilingStatusJsonConverter"/>.
/// </para>
/// </remarks>
/// <seealso cref="CorporationTaxFilingStatus"/>
/// <seealso cref="CorporationTaxFilingStatusJsonConverter"/>
public class CorporationTaxFilingStatusNonNullableJsonConverter : JsonConverter<CorporationTaxFilingStatus>
{
    /// <summary>
    /// Reads a non-nullable <see cref="CorporationTaxFilingStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (CorporationTaxFilingStatus).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="CorporationTaxFilingStatus"/> value.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON value is null, not a string, or when the string value cannot be
    /// converted to a valid <see cref="CorporationTaxFilingStatus"/> enum value.
    /// </exception>
    public override CorporationTaxFilingStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Cannot convert null to non-nullable CorporationTaxFilingStatus");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException("Cannot convert empty string to non-nullable CorporationTaxFilingStatus");
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
    /// Writes a non-nullable <see cref="CorporationTaxFilingStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="CorporationTaxFilingStatus"/> value to serialize.</param>
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
    public override void Write(Utf8JsonWriter writer, CorporationTaxFilingStatus value, JsonSerializerOptions options)
    {
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
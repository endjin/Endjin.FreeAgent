// <copyright file="FinalAccountsFilingStatusNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom non-nullable JSON converter for the <see cref="FinalAccountsFilingStatus"/> enum that handles the
/// FreeAgent API's string format for Final Accounts filing status values.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents filing status values as strings:
/// "draft", "unfiled", "pending", "rejected", "filed", and "marked_as_filed".
/// This converter handles the deserialization of these string values to the appropriate
/// <see cref="FinalAccountsFilingStatus"/> enum value.
/// </para>
/// <para>
/// This converter is for non-nullable <see cref="FinalAccountsFilingStatus"/> properties.
/// For nullable properties, use <see cref="FinalAccountsFilingStatusJsonConverter"/>.
/// </para>
/// </remarks>
/// <seealso cref="FinalAccountsFilingStatus"/>
/// <seealso cref="FinalAccountsFilingStatusJsonConverter"/>
public class FinalAccountsFilingStatusNonNullableJsonConverter : JsonConverter<FinalAccountsFilingStatus>
{
    /// <summary>
    /// Reads a non-nullable <see cref="FinalAccountsFilingStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (FinalAccountsFilingStatus).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="FinalAccountsFilingStatus"/> value.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON value is null, not a string, or when the string value cannot be
    /// converted to a valid <see cref="FinalAccountsFilingStatus"/> enum value.
    /// </exception>
    public override FinalAccountsFilingStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("Cannot convert null to non-nullable FinalAccountsFilingStatus");
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Unexpected token type: {reader.TokenType}");
        }

        string? value = reader.GetString();
        if (string.IsNullOrEmpty(value))
        {
            throw new JsonException("Cannot convert empty string to non-nullable FinalAccountsFilingStatus");
        }

        // Normalize the value: convert to lowercase for case-insensitive comparison
        string normalizedValue = value.ToLowerInvariant();

        return normalizedValue switch
        {
            "draft" => FinalAccountsFilingStatus.Draft,
            "unfiled" => FinalAccountsFilingStatus.Unfiled,
            "pending" => FinalAccountsFilingStatus.Pending,
            "rejected" => FinalAccountsFilingStatus.Rejected,
            "filed" => FinalAccountsFilingStatus.Filed,
            "marked_as_filed" => FinalAccountsFilingStatus.MarkedAsFiled,
            _ => throw new JsonException($"Unable to convert '{value}' to FinalAccountsFilingStatus enum")
        };
    }

    /// <summary>
    /// Writes a non-nullable <see cref="FinalAccountsFilingStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="FinalAccountsFilingStatus"/> value to serialize.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="FinalAccountsFilingStatus"/> enum value.
    /// </exception>
    /// <remarks>
    /// Status values are serialized to strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="FinalAccountsFilingStatus.Draft"/> → "draft"</item>
    /// <item><see cref="FinalAccountsFilingStatus.Unfiled"/> → "unfiled"</item>
    /// <item><see cref="FinalAccountsFilingStatus.Pending"/> → "pending"</item>
    /// <item><see cref="FinalAccountsFilingStatus.Rejected"/> → "rejected"</item>
    /// <item><see cref="FinalAccountsFilingStatus.Filed"/> → "filed"</item>
    /// <item><see cref="FinalAccountsFilingStatus.MarkedAsFiled"/> → "marked_as_filed"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, FinalAccountsFilingStatus value, JsonSerializerOptions options)
    {
        // Convert enum to the API format
        string stringValue = value switch
        {
            FinalAccountsFilingStatus.Draft => "draft",
            FinalAccountsFilingStatus.Unfiled => "unfiled",
            FinalAccountsFilingStatus.Pending => "pending",
            FinalAccountsFilingStatus.Rejected => "rejected",
            FinalAccountsFilingStatus.Filed => "filed",
            FinalAccountsFilingStatus.MarkedAsFiled => "marked_as_filed",
            _ => throw new JsonException($"Unknown FinalAccountsFilingStatus value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

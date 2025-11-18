// <copyright file="FinalAccountsFilingStatusJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="FinalAccountsFilingStatus"/> enum that handles the
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
/// During serialization, status values are converted back to the API-expected format.
/// </para>
/// </remarks>
/// <seealso cref="FinalAccountsFilingStatus"/>
/// <seealso cref="FinalAccountsFilingStatusNonNullableJsonConverter"/>
public class FinalAccountsFilingStatusJsonConverter : JsonConverter<FinalAccountsFilingStatus?>
{
    /// <summary>
    /// Reads a nullable <see cref="FinalAccountsFilingStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (FinalAccountsFilingStatus?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="FinalAccountsFilingStatus"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="FinalAccountsFilingStatus"/> enum value.
    /// </exception>
    public override FinalAccountsFilingStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
    /// Writes a nullable <see cref="FinalAccountsFilingStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="FinalAccountsFilingStatus"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
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
    public override void Write(Utf8JsonWriter writer, FinalAccountsFilingStatus? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

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

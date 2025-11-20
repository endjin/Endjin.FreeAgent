// <copyright file="SalesTaxRegistrationStatusJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="SalesTaxRegistrationStatus"/> enum that handles the
/// FreeAgent API's string format for sales tax registration status values.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents sales tax registration status values as strings:
/// "Not Registered" and "Registered". This converter handles the deserialization of these
/// string values to the appropriate <see cref="SalesTaxRegistrationStatus"/> enum value.
/// </para>
/// <para>
/// During serialization, status values are converted back to the API-expected format:
/// <see cref="SalesTaxRegistrationStatus.NotRegistered"/> is serialized as "Not Registered" and
/// <see cref="SalesTaxRegistrationStatus.Registered"/> is serialized as "Registered".
/// </para>
/// </remarks>
/// <seealso cref="SalesTaxRegistrationStatus"/>
/// <seealso cref="SalesTaxRegistrationStatusNonNullableJsonConverter"/>
public class SalesTaxRegistrationStatusJsonConverter : JsonConverter<SalesTaxRegistrationStatus?>
{
    /// <summary>
    /// Reads a nullable <see cref="SalesTaxRegistrationStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (SalesTaxRegistrationStatus?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="SalesTaxRegistrationStatus"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="SalesTaxRegistrationStatus"/> enum value.
    /// </exception>
    public override SalesTaxRegistrationStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            "not registered" => SalesTaxRegistrationStatus.NotRegistered,
            "registered" => SalesTaxRegistrationStatus.Registered,
            _ => throw new JsonException($"Unable to convert '{value}' to SalesTaxRegistrationStatus enum")
        };
    }

    /// <summary>
    /// Writes a nullable <see cref="SalesTaxRegistrationStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="SalesTaxRegistrationStatus"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="SalesTaxRegistrationStatus"/> enum value.
    /// </exception>
    /// <remarks>
    /// Status values are serialized to strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="SalesTaxRegistrationStatus.NotRegistered"/> → "Not Registered"</item>
    /// <item><see cref="SalesTaxRegistrationStatus.Registered"/> → "Registered"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, SalesTaxRegistrationStatus? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to the API format
        string stringValue = value switch
        {
            SalesTaxRegistrationStatus.NotRegistered => "Not Registered",
            SalesTaxRegistrationStatus.Registered => "Registered",
            _ => throw new JsonException($"Unknown SalesTaxRegistrationStatus value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
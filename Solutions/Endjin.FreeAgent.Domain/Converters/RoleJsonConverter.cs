// <copyright file="RoleJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="Role"/> enum that handles various string formats
/// from the FreeAgent API (snake_case, lowercase, hyphenated, etc.).
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents role values as strings in snake_case format (e.g., "company_secretary").
/// This converter handles deserialization of various formats including snake_case, hyphenated, and
/// plain lowercase strings, normalizing them to the appropriate <see cref="Role"/> enum value.
/// </para>
/// <para>
/// During serialization, role values are converted to snake_case format for API compatibility.
/// For example, <see cref="Role.CompanySecretary"/> is serialized as "company_secretary".
/// </para>
/// </remarks>
/// <seealso cref="Role"/>
/// <seealso cref="RoleNonNullableJsonConverter"/>
public class RoleJsonConverter : JsonConverter<Role?>
{
    /// <summary>
    /// Reads a nullable <see cref="Role"/> value from JSON, handling various string formats.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (Role?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="Role"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="Role"/> enum value.
    /// </exception>
    public override Role? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        // Normalize the value: remove underscores and convert to lowercase for comparison
        string normalizedValue = value.Replace("_", "").Replace("-", "").ToLowerInvariant();

        return normalizedValue switch
        {
            "owner" => Role.Owner,
            "director" => Role.Director,
            "partner" => Role.Partner,
            "companysecretary" or "company_secretary" => Role.CompanySecretary,
            "employee" => Role.Employee,
            "shareholder" => Role.Shareholder,
            "accountant" => Role.Accountant,
            _ => throw new JsonException($"Unable to convert '{value}' to Role enum")
        };
    }

    /// <summary>
    /// Writes a nullable <see cref="Role"/> value to JSON in snake_case format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="Role"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="Role"/> enum value.
    /// </exception>
    /// <remarks>
    /// Role values are serialized to snake_case strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="Role.Owner"/> → "owner"</item>
    /// <item><see cref="Role.Director"/> → "director"</item>
    /// <item><see cref="Role.Partner"/> → "partner"</item>
    /// <item><see cref="Role.CompanySecretary"/> → "company_secretary"</item>
    /// <item><see cref="Role.Employee"/> → "employee"</item>
    /// <item><see cref="Role.Shareholder"/> → "shareholder"</item>
    /// <item><see cref="Role.Accountant"/> → "accountant"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, Role? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to snake_case for API compatibility
        string stringValue = value switch
        {
            Role.Owner => "owner",
            Role.Director => "director",
            Role.Partner => "partner",
            Role.CompanySecretary => "company_secretary",
            Role.Employee => "employee",
            Role.Shareholder => "shareholder",
            Role.Accountant => "accountant",
            _ => throw new JsonException($"Unknown Role value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
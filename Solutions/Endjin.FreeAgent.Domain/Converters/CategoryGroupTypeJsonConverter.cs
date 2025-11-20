// <copyright file="CategoryGroupTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="CategoryGroupType"/> enum that handles snake_case
/// string formats from the FreeAgent API.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API represents category group values as strings in snake_case format
/// (e.g., "cost_of_sales", "admin_expenses"). This converter handles deserialization of
/// these formats, normalizing them to the appropriate <see cref="CategoryGroupType"/> enum value.
/// </para>
/// <para>
/// During serialization, category group values are converted to snake_case format for API compatibility.
/// For example, <see cref="CategoryGroupType.CostOfSales"/> is serialized as "cost_of_sales".
/// </para>
/// </remarks>
/// <seealso cref="CategoryGroupType"/>
/// <seealso cref="CategoryGroupTypeNonNullableJsonConverter"/>
public class CategoryGroupTypeJsonConverter : JsonConverter<CategoryGroupType?>
{
    /// <summary>
    /// Reads a nullable <see cref="CategoryGroupType"/> value from JSON, handling snake_case string formats.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (CategoryGroupType?).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>
    /// The deserialized <see cref="CategoryGroupType"/> value, or <see langword="null"/> if the JSON value is null or empty.
    /// </returns>
    /// <exception cref="JsonException">
    /// Thrown when the JSON token is not a string or null, or when the string value cannot be
    /// converted to a valid <see cref="CategoryGroupType"/> enum value.
    /// </exception>
    public override CategoryGroupType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        // API uses snake_case values
        return value switch
        {
            "income" => CategoryGroupType.Income,
            "cost_of_sales" => CategoryGroupType.CostOfSales,
            "admin_expenses" => CategoryGroupType.AdminExpenses,
            "current_assets" => CategoryGroupType.CurrentAssets,
            "liabilities" => CategoryGroupType.Liabilities,
            "equities" => CategoryGroupType.Equities,
            _ => throw new JsonException($"Unable to convert '{value}' to CategoryGroupType enum")
        };
    }

    /// <summary>
    /// Writes a nullable <see cref="CategoryGroupType"/> value to JSON in snake_case format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="CategoryGroupType"/> value to serialize, or <see langword="null"/> to write a JSON null value.</param>
    /// <param name="options">The serializer options to use.</param>
    /// <exception cref="JsonException">
    /// Thrown when the <paramref name="value"/> is not a recognized <see cref="CategoryGroupType"/> enum value.
    /// </exception>
    /// <remarks>
    /// CategoryGroupType values are serialized to snake_case strings for FreeAgent API compatibility:
    /// <list type="bullet">
    /// <item><see cref="CategoryGroupType.Income"/> → "income"</item>
    /// <item><see cref="CategoryGroupType.CostOfSales"/> → "cost_of_sales"</item>
    /// <item><see cref="CategoryGroupType.AdminExpenses"/> → "admin_expenses"</item>
    /// <item><see cref="CategoryGroupType.CurrentAssets"/> → "current_assets"</item>
    /// <item><see cref="CategoryGroupType.Liabilities"/> → "liabilities"</item>
    /// <item><see cref="CategoryGroupType.Equities"/> → "equities"</item>
    /// </list>
    /// </remarks>
    public override void Write(Utf8JsonWriter writer, CategoryGroupType? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to snake_case for API compatibility
        string stringValue = value switch
        {
            CategoryGroupType.Income => "income",
            CategoryGroupType.CostOfSales => "cost_of_sales",
            CategoryGroupType.AdminExpenses => "admin_expenses",
            CategoryGroupType.CurrentAssets => "current_assets",
            CategoryGroupType.Liabilities => "liabilities",
            CategoryGroupType.Equities => "equities",
            _ => throw new JsonException($"Unknown CategoryGroupType value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
// <copyright file="RecurringPatternJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="RecurringPattern"/> enum that handles the
/// FreeAgent API's string format for recurring pattern values.
/// </summary>
/// <remarks>
/// Deserialization matches the API's values case-insensitively, so a change in the casing the API uses
/// does not break deserialization. Serialization always writes the documented casing.
/// </remarks>
public class RecurringPatternJsonConverter : JsonConverter<RecurringPattern?>
{
    /// <inheritdoc/>
    public override RecurringPattern? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        // Matched case-insensitively, so a change in the API's casing cannot fail deserialization.
        return value.ToLowerInvariant() switch
        {
            "weekly" => RecurringPattern.Weekly,
            "two weekly" => RecurringPattern.TwoWeekly,
            "four weekly" => RecurringPattern.FourWeekly,
            "monthly" => RecurringPattern.Monthly,
            "two monthly" => RecurringPattern.TwoMonthly,
            "quarterly" => RecurringPattern.Quarterly,
            "biannually" => RecurringPattern.Biannually,
            "annually" => RecurringPattern.Annually,
            "2-yearly" => RecurringPattern.TwoYearly,
            _ => throw new JsonException($"Unable to convert '{value}' to RecurringPattern enum")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RecurringPattern? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            RecurringPattern.Weekly => "Weekly",
            RecurringPattern.TwoWeekly => "Two Weekly",
            RecurringPattern.FourWeekly => "Four Weekly",
            RecurringPattern.Monthly => "Monthly",
            RecurringPattern.TwoMonthly => "Two Monthly",
            RecurringPattern.Quarterly => "Quarterly",
            RecurringPattern.Biannually => "Biannually",
            RecurringPattern.Annually => "Annually",
            RecurringPattern.TwoYearly => "2-Yearly",
            _ => throw new JsonException($"Unknown RecurringPattern value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Non-nullable JSON converter for the <see cref="RecurringPattern"/> enum.
/// </summary>
public class RecurringPatternNonNullableJsonConverter : JsonConverter<RecurringPattern>
{
    private readonly RecurringPatternJsonConverter nullableConverter = new();

    /// <inheritdoc/>
    public override RecurringPattern Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return this.nullableConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException("Expected non-null RecurringPattern value");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RecurringPattern value, JsonSerializerOptions options)
    {
        this.nullableConverter.Write(writer, value, options);
    }
}
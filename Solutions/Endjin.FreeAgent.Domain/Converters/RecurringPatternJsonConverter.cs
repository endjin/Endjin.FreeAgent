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

        return value switch
        {
            "Weekly" => RecurringPattern.Weekly,
            "Two Weekly" => RecurringPattern.TwoWeekly,
            "Four Weekly" => RecurringPattern.FourWeekly,
            "Two Monthly" => RecurringPattern.TwoMonthly,
            "Quarterly" => RecurringPattern.Quarterly,
            "Biannually" => RecurringPattern.Biannually,
            "Annually" => RecurringPattern.Annually,
            "2-Yearly" => RecurringPattern.TwoYearly,
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

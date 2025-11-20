// <copyright file="RebillTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="RebillType"/> enum that handles the
/// FreeAgent API's string format for rebill type values.
/// </summary>
public class RebillTypeJsonConverter : JsonConverter<RebillType?>
{
    /// <inheritdoc/>
    public override RebillType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        return value.ToLowerInvariant() switch
        {
            "cost" => RebillType.Cost,
            "markup" => RebillType.Markup,
            "price" => RebillType.Price,
            _ => throw new JsonException($"Unable to convert '{value}' to RebillType enum")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RebillType? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            RebillType.Cost => "cost",
            RebillType.Markup => "markup",
            RebillType.Price => "price",
            _ => throw new JsonException($"Unknown RebillType value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Non-nullable JSON converter for the <see cref="RebillType"/> enum.
/// </summary>
public class RebillTypeNonNullableJsonConverter : JsonConverter<RebillType>
{
    private readonly RebillTypeJsonConverter nullableConverter = new();

    /// <inheritdoc/>
    public override RebillType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return this.nullableConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException("Expected non-null RebillType value");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RebillType value, JsonSerializerOptions options)
    {
        this.nullableConverter.Write(writer, value, options);
    }
}

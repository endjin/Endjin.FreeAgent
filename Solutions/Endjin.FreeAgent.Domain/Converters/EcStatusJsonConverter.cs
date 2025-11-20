// <copyright file="EcStatusJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="EcStatus"/> enum that handles the
/// FreeAgent API's string format for EC status values.
/// </summary>
public class EcStatusJsonConverter : JsonConverter<EcStatus?>
{
    /// <inheritdoc/>
    public override EcStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            "UK/Non-EC" => EcStatus.UkNonEc,
            "EC Goods" => EcStatus.EcGoods,
            "EC Services" => EcStatus.EcServices,
            "Reverse Charge" => EcStatus.ReverseCharge,
            "EC VAT MOSS" => EcStatus.EcVatMoss,
            _ => throw new JsonException($"Unable to convert '{value}' to EcStatus enum")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EcStatus? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            EcStatus.UkNonEc => "UK/Non-EC",
            EcStatus.EcGoods => "EC Goods",
            EcStatus.EcServices => "EC Services",
            EcStatus.ReverseCharge => "Reverse Charge",
            EcStatus.EcVatMoss => "EC VAT MOSS",
            _ => throw new JsonException($"Unknown EcStatus value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Non-nullable JSON converter for the <see cref="EcStatus"/> enum.
/// </summary>
public class EcStatusNonNullableJsonConverter : JsonConverter<EcStatus>
{
    private readonly EcStatusJsonConverter nullableConverter = new();

    /// <inheritdoc/>
    public override EcStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return this.nullableConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException("Expected non-null EcStatus value");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EcStatus value, JsonSerializerOptions options)
    {
        this.nullableConverter.Write(writer, value, options);
    }
}

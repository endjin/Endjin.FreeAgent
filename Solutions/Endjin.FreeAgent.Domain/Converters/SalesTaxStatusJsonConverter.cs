// <copyright file="SalesTaxStatusJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="SalesTaxStatus"/> enum that handles the
/// FreeAgent API's string format for sales tax status values.
/// </summary>
public class SalesTaxStatusJsonConverter : JsonConverter<SalesTaxStatus?>
{
    /// <inheritdoc/>
    public override SalesTaxStatus? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        return value.ToUpperInvariant() switch
        {
            "TAXABLE" => SalesTaxStatus.Taxable,
            "EXEMPT" => SalesTaxStatus.Exempt,
            "OUT_OF_SCOPE" => SalesTaxStatus.OutOfScope,
            _ => throw new JsonException($"Unable to convert '{value}' to SalesTaxStatus enum")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SalesTaxStatus? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            SalesTaxStatus.Taxable => "TAXABLE",
            SalesTaxStatus.Exempt => "EXEMPT",
            SalesTaxStatus.OutOfScope => "OUT_OF_SCOPE",
            _ => throw new JsonException($"Unknown SalesTaxStatus value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Non-nullable JSON converter for the <see cref="SalesTaxStatus"/> enum.
/// </summary>
public class SalesTaxStatusNonNullableJsonConverter : JsonConverter<SalesTaxStatus>
{
    private readonly SalesTaxStatusJsonConverter nullableConverter = new();

    /// <inheritdoc/>
    public override SalesTaxStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return this.nullableConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException("Expected non-null SalesTaxStatus value");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SalesTaxStatus value, JsonSerializerOptions options)
    {
        this.nullableConverter.Write(writer, value, options);
    }
}

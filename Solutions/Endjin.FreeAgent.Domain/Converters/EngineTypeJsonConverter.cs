// <copyright file="EngineTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="EngineType"/> enum that handles the
/// FreeAgent API's string format for engine type values.
/// </summary>
public class EngineTypeJsonConverter : JsonConverter<EngineType?>
{
    /// <inheritdoc/>
    public override EngineType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            "Petrol" => EngineType.Petrol,
            "Diesel" => EngineType.Diesel,
            "LPG" => EngineType.Lpg,
            "Electric" => EngineType.Electric,
            "Electric (Home charger)" => EngineType.ElectricHomeCharger,
            "Electric (Public charger)" => EngineType.ElectricPublicCharger,
            _ => throw new JsonException($"Unable to convert '{value}' to EngineType enum")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EngineType? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            EngineType.Petrol => "Petrol",
            EngineType.Diesel => "Diesel",
            EngineType.Lpg => "LPG",
            EngineType.Electric => "Electric",
            EngineType.ElectricHomeCharger => "Electric (Home charger)",
            EngineType.ElectricPublicCharger => "Electric (Public charger)",
            _ => throw new JsonException($"Unknown EngineType value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Non-nullable JSON converter for the <see cref="EngineType"/> enum.
/// </summary>
public class EngineTypeNonNullableJsonConverter : JsonConverter<EngineType>
{
    private readonly EngineTypeJsonConverter nullableConverter = new();

    /// <inheritdoc/>
    public override EngineType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return this.nullableConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException("Expected non-null EngineType value");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, EngineType value, JsonSerializerOptions options)
    {
        this.nullableConverter.Write(writer, value, options);
    }
}

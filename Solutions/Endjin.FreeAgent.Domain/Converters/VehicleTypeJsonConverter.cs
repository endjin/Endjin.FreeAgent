// <copyright file="VehicleTypeJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the <see cref="VehicleType"/> enum that handles the
/// FreeAgent API's string format for vehicle type values.
/// </summary>
public class VehicleTypeJsonConverter : JsonConverter<VehicleType?>
{
    /// <inheritdoc/>
    public override VehicleType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
            "Car" => VehicleType.Car,
            "Motorcycle" => VehicleType.Motorcycle,
            "Bicycle" => VehicleType.Bicycle,
            _ => throw new JsonException($"Unable to convert '{value}' to VehicleType enum")
        };
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, VehicleType? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        string stringValue = value switch
        {
            VehicleType.Car => "Car",
            VehicleType.Motorcycle => "Motorcycle",
            VehicleType.Bicycle => "Bicycle",
            _ => throw new JsonException($"Unknown VehicleType value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}

/// <summary>
/// Non-nullable JSON converter for the <see cref="VehicleType"/> enum.
/// </summary>
public class VehicleTypeNonNullableJsonConverter : JsonConverter<VehicleType>
{
    private readonly VehicleTypeJsonConverter nullableConverter = new();

    /// <inheritdoc/>
    public override VehicleType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return this.nullableConverter.Read(ref reader, typeToConvert, options)
            ?? throw new JsonException("Expected non-null VehicleType value");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, VehicleType value, JsonSerializerOptions options)
    {
        this.nullableConverter.Write(writer, value, options);
    }
}

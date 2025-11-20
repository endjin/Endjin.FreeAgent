// <copyright file="AutoSalesTaxRateTypeNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Non-nullable version of the AutoSalesTaxRateType JSON converter that enforces non-null AutoSalesTaxRateType values during deserialization.
/// </summary>
/// <remarks>
/// This converter wraps <see cref="AutoSalesTaxRateTypeJsonConverter"/> and throws a <see cref="JsonException"/> if the JSON value is null.
/// Use this converter when deserializing AutoSalesTaxRateType properties that must have a value and cannot be null.
/// </remarks>
/// <seealso cref="AutoSalesTaxRateTypeJsonConverter"/>
/// <seealso cref="AutoSalesTaxRateType"/>
public class AutoSalesTaxRateTypeNonNullableJsonConverter : JsonConverter<AutoSalesTaxRateType>
{
    private readonly AutoSalesTaxRateTypeJsonConverter nullableConverter = new();

    /// <summary>
    /// Reads a non-null <see cref="AutoSalesTaxRateType"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (AutoSalesTaxRateType).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>The deserialized <see cref="AutoSalesTaxRateType"/> value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is null or cannot be converted to an AutoSalesTaxRateType.</exception>
    public override AutoSalesTaxRateType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        AutoSalesTaxRateType? result = nullableConverter.Read(ref reader, typeof(AutoSalesTaxRateType?), options);
        return result ?? throw new JsonException("AutoSalesTaxRateType value cannot be null");
    }

    /// <summary>
    /// Writes an <see cref="AutoSalesTaxRateType"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="AutoSalesTaxRateType"/> value to serialize.</param>
    /// <param name="options">The serializer options to use.</param>
    public override void Write(Utf8JsonWriter writer, AutoSalesTaxRateType value, JsonSerializerOptions options) => nullableConverter.Write(writer, value, options);
}
// <copyright file="SalesTaxRegistrationStatusNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json;

using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Non-nullable version of the SalesTaxRegistrationStatus JSON converter that enforces non-null
/// SalesTaxRegistrationStatus values during deserialization.
/// </summary>
/// <remarks>
/// This converter wraps <see cref="SalesTaxRegistrationStatusJsonConverter"/> and throws a
/// <see cref="JsonException"/> if the JSON value is null. Use this converter when deserializing
/// SalesTaxRegistrationStatus properties that must have a value and cannot be null.
/// </remarks>
/// <seealso cref="SalesTaxRegistrationStatusJsonConverter"/>
/// <seealso cref="SalesTaxRegistrationStatus"/>
public class SalesTaxRegistrationStatusNonNullableJsonConverter : JsonConverter<SalesTaxRegistrationStatus>
{
    private readonly SalesTaxRegistrationStatusJsonConverter nullableConverter = new();

    /// <summary>
    /// Reads a non-null <see cref="SalesTaxRegistrationStatus"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (SalesTaxRegistrationStatus).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>The deserialized <see cref="SalesTaxRegistrationStatus"/> value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is null or cannot be converted to a SalesTaxRegistrationStatus.</exception>
    public override SalesTaxRegistrationStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        SalesTaxRegistrationStatus? result = nullableConverter.Read(ref reader, typeof(SalesTaxRegistrationStatus?), options);
        return result ?? throw new JsonException("SalesTaxRegistrationStatus value cannot be null");
    }

    /// <summary>
    /// Writes a <see cref="SalesTaxRegistrationStatus"/> value to JSON in the API-expected format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="SalesTaxRegistrationStatus"/> value to serialize.</param>
    /// <param name="options">The serializer options to use.</param>
    public override void Write(Utf8JsonWriter writer, SalesTaxRegistrationStatus value, JsonSerializerOptions options) => nullableConverter.Write(writer, value, options);
}
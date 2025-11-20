// <copyright file="CategoryGroupTypeNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Non-nullable version of the CategoryGroupType JSON converter that enforces non-null CategoryGroupType values during deserialization.
/// </summary>
/// <remarks>
/// This converter wraps <see cref="CategoryGroupTypeJsonConverter"/> and throws a <see cref="JsonException"/> if the JSON value is null.
/// Use this converter when deserializing CategoryGroupType properties that must have a value and cannot be null.
/// </remarks>
/// <seealso cref="CategoryGroupTypeJsonConverter"/>
/// <seealso cref="CategoryGroupType"/>
public class CategoryGroupTypeNonNullableJsonConverter : JsonConverter<CategoryGroupType>
{
    private readonly CategoryGroupTypeJsonConverter nullableConverter = new();

    /// <summary>
    /// Reads a non-null <see cref="CategoryGroupType"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (CategoryGroupType).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>The deserialized <see cref="CategoryGroupType"/> value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is null or cannot be converted to a CategoryGroupType.</exception>
    public override CategoryGroupType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        CategoryGroupType? result = nullableConverter.Read(ref reader, typeof(CategoryGroupType?), options);
        return result ?? throw new JsonException("CategoryGroupType value cannot be null");
    }

    /// <summary>
    /// Writes a <see cref="CategoryGroupType"/> value to JSON in snake_case format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="CategoryGroupType"/> value to serialize.</param>
    /// <param name="options">The serializer options to use.</param>
    public override void Write(Utf8JsonWriter writer, CategoryGroupType value, JsonSerializerOptions options) => nullableConverter.Write(writer, value, options);
}
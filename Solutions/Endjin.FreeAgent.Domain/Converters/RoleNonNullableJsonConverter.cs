// <copyright file="RoleNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Non-nullable version of the Role JSON converter that enforces non-null Role values during deserialization.
/// </summary>
/// <remarks>
/// This converter wraps <see cref="RoleJsonConverter"/> and throws a <see cref="JsonException"/> if the JSON value is null.
/// Use this converter when deserializing Role properties that must have a value and cannot be null.
/// </remarks>
/// <seealso cref="RoleJsonConverter"/>
/// <seealso cref="Role"/>
public class RoleNonNullableJsonConverter : JsonConverter<Role>
{
    private readonly RoleJsonConverter nullableConverter = new();

    /// <summary>
    /// Reads a non-null <see cref="Role"/> value from JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert to (Role).</param>
    /// <param name="options">The serializer options to use.</param>
    /// <returns>The deserialized <see cref="Role"/> value.</returns>
    /// <exception cref="JsonException">Thrown when the JSON value is null or cannot be converted to a Role.</exception>
    public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Role? result = nullableConverter.Read(ref reader, typeof(Role?), options);
        return result ?? throw new JsonException("Role value cannot be null");
    }

    /// <summary>
    /// Writes a <see cref="Role"/> value to JSON in snake_case format.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="value">The <see cref="Role"/> value to serialize.</param>
    /// <param name="options">The serializer options to use.</param>
    public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options) => nullableConverter.Write(writer, value, options);
}
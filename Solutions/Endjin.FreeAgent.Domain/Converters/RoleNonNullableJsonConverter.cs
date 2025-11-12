// <copyright file="RoleNonNullableJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Non-nullable version of the Role JSON converter.
/// </summary>
public class RoleNonNullableJsonConverter : JsonConverter<Role>
{
    private readonly RoleJsonConverter nullableConverter = new();

    public override Role Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Role? result = nullableConverter.Read(ref reader, typeof(Role?), options);
        return result ?? throw new JsonException("Role value cannot be null");
    }

    public override void Write(Utf8JsonWriter writer, Role value, JsonSerializerOptions options) => nullableConverter.Write(writer, value, options);
}
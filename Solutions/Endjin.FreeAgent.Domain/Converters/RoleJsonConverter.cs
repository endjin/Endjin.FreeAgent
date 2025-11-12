// <copyright file="RoleJsonConverter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain;

using System.Text.Json;

namespace Endjin.FreeAgent.Converters;

/// <summary>
/// Custom JSON converter for the Role enum that handles various string formats
/// from the FreeAgent API (snake_case, lowercase, etc.).
/// </summary>
public class RoleJsonConverter : JsonConverter<Role?>
{
    public override Role? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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

        // Normalize the value: remove underscores and convert to lowercase for comparison
        string normalizedValue = value.Replace("_", "").Replace("-", "").ToLowerInvariant();

        return normalizedValue switch
        {
            "owner" => Role.Owner,
            "director" => Role.Director,
            "partner" => Role.Partner,
            "companysecretary" or "company_secretary" => Role.CompanySecretary,
            "employee" => Role.Employee,
            "shareholder" => Role.Shareholder,
            "accountant" => Role.Accountant,
            _ => throw new JsonException($"Unable to convert '{value}' to Role enum")
        };
    }

    public override void Write(Utf8JsonWriter writer, Role? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        // Convert enum to snake_case for API compatibility
        string stringValue = value switch
        {
            Role.Owner => "owner",
            Role.Director => "director",
            Role.Partner => "partner",
            Role.CompanySecretary => "company_secretary",
            Role.Employee => "employee",
            Role.Shareholder => "shareholder",
            Role.Accountant => "accountant",
            _ => throw new JsonException($"Unknown Role value: {value}")
        };

        writer.WriteStringValue(stringValue);
    }
}
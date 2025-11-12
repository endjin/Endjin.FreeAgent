// <copyright file="User.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain;

[DebuggerDisplay("Name = {FullName}")]
public record User
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("role")]
    [JsonConverter(typeof(RoleJsonConverter))]
    public Role? Role { get; init; }

    [JsonPropertyName("hidden")]
    public bool? Hidden { get; init; }

    [JsonPropertyName("permission_level")]
    public long? PermissionLevel { get; init; }

    [JsonPropertyName("opening_mileage")]
    public string? OpeningMileage { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("ni_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NiNumber { get; init; }

    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}
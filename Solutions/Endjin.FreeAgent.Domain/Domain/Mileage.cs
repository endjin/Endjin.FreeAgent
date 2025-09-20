// <copyright file="Mileage.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record Mileage
{
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    [JsonPropertyName("project")]
    public Uri? Project { get; init; }

    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("mileage")]
    public decimal? Miles { get; init; }

    [JsonPropertyName("reclaim_mileage")]
    public bool? ReclaimMileage { get; init; }

    [JsonPropertyName("reclaim_mileage_rate")]
    public decimal? ReclaimMileageRate { get; init; }

    [JsonPropertyName("rebill_mileage")]
    public bool? RebillMileage { get; init; }

    [JsonPropertyName("rebill_mileage_rate")]
    public decimal? RebillMileageRate { get; init; }

    [JsonPropertyName("reclaim_mileage_value")]
    public decimal? ReclaimMileageValue { get; init; }

    [JsonPropertyName("rebill_mileage_value")]
    public decimal? RebillMileageValue { get; init; }

    [JsonPropertyName("vehicle_type")]
    public string? VehicleType { get; init; }

    [JsonPropertyName("engine_type")]
    public string? EngineType { get; init; }

    [JsonPropertyName("engine_size")]
    public string? EngineSize { get; init; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}
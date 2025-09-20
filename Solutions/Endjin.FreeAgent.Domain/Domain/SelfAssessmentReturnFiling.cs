// <copyright file="SelfAssessmentReturnFiling.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SelfAssessmentReturnFiling
{
    [JsonPropertyName("filed_on")]
    public string? FiledOn { get; init; }

    [JsonPropertyName("filed_online")]
    public bool FiledOnline { get; init; }

    [JsonPropertyName("utr_number")]
    public string? UtrNumber { get; init; }
}
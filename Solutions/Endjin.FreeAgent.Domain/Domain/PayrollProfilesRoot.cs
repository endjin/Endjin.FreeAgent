// <copyright file="PayrollProfilesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PayrollProfilesRoot
{
    [JsonPropertyName("payroll_profiles")]
    public List<PayrollProfile> PayrollProfiles { get; init; } = [];
}
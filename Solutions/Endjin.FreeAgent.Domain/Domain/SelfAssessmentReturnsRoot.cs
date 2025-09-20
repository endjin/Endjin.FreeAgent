// <copyright file="SelfAssessmentReturnsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SelfAssessmentReturnsRoot
{
    [JsonPropertyName("self_assessment_returns")]
    public List<SelfAssessmentReturn>? SelfAssessmentReturns { get; init; }
}
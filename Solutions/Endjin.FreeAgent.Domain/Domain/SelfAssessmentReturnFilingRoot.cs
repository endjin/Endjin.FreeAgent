// <copyright file="SelfAssessmentReturnFilingRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SelfAssessmentReturnFilingRoot
{
    [JsonPropertyName("self_assessment_return")]
    public SelfAssessmentReturnFiling? SelfAssessmentReturn { get; init; }
}
// <copyright file="SelfAssessmentReturnRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record SelfAssessmentReturnRoot
{
    [JsonPropertyName("self_assessment_return")]
    public SelfAssessmentReturn? SelfAssessmentReturn { get; init; }
}
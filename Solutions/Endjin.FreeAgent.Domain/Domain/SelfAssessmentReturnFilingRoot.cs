// <copyright file="SelfAssessmentReturnFilingRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for <see cref="Domain.SelfAssessmentReturnFiling"/> data.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when recording Self Assessment return filing details via the FreeAgent API.
/// </remarks>
/// <seealso cref="SelfAssessmentReturnFiling"/>
/// <seealso cref="SelfAssessmentReturn"/>
public record SelfAssessmentReturnFilingRoot
{
    /// <summary>
    /// Gets the Self Assessment return filing data for the API request.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.SelfAssessmentReturnFiling"/> object containing filing details.
    /// </value>
    [JsonPropertyName("self_assessment_return")]
    public SelfAssessmentReturnFiling? SelfAssessmentReturn { get; init; }
}
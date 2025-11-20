// <copyright file="SelfAssessmentReturnRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.SelfAssessmentReturn"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single Self Assessment return.
/// </remarks>
/// <seealso cref="SelfAssessmentReturn"/>
public record SelfAssessmentReturnRoot
{
    /// <summary>
    /// Gets the Self Assessment return from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.SelfAssessmentReturn"/> object returned by the API.
    /// </value>
    [JsonPropertyName("self_assessment_return")]
    public SelfAssessmentReturn? SelfAssessmentReturn { get; init; }
}
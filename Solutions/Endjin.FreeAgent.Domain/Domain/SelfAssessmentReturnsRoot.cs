// <copyright file="SelfAssessmentReturnsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a collection of <see cref="SelfAssessmentReturn"/> resources.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return multiple Self Assessment returns.
/// </remarks>
/// <seealso cref="SelfAssessmentReturn"/>
public record SelfAssessmentReturnsRoot
{
    /// <summary>
    /// Gets the collection of Self Assessment returns from the API response.
    /// </summary>
    /// <value>
    /// A list of <see cref="SelfAssessmentReturn"/> objects returned by the API.
    /// </value>
    [JsonPropertyName("self_assessment_returns")]
    public List<SelfAssessmentReturn>? SelfAssessmentReturns { get; init; }
}
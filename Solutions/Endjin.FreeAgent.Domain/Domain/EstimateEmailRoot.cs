// <copyright file="EstimateEmailRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for estimate email operations.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses when working with estimate email operations.
/// </remarks>
/// <seealso cref="EstimateEmailWrapper"/>
/// <seealso cref="EstimateEmail"/>
/// <seealso cref="Estimate"/>
public record EstimateEmailRoot
{
    /// <summary>
    /// Gets the estimate email wrapper from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="EstimateEmailWrapper"/> object containing email configuration.
    /// </value>
    [JsonPropertyName("estimate")]
    public EstimateEmailWrapper? Estimate { get; init; }
}

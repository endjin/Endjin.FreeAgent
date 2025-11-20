// <copyright file="CompanyRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for a <see cref="Domain.Company"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses that return a single company object.
/// </remarks>
/// <seealso cref="Company"/>
public record CompanyRoot
{
    /// <summary>
    /// Gets the company from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.Company"/> object returned by the API.
    /// </value>
    [JsonPropertyName("company")]
    public Company? Company { get; init; }
}
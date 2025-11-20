// <copyright file="MileageSettingsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON response wrapper for <see cref="MileageSettings"/>.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON deserialization of FreeAgent API responses
/// from the GET /v2/expenses/mileage_settings endpoint.
/// </remarks>
/// <seealso cref="MileageSettings"/>
public record MileageSettingsRoot
{
    /// <summary>
    /// Gets the mileage settings from the API response.
    /// </summary>
    /// <value>
    /// The <see cref="MileageSettings"/> object containing engine type options and mileage rates.
    /// </value>
    [JsonPropertyName("mileage_settings")]
    public MileageSettings? MileageSettings { get; init; }
}

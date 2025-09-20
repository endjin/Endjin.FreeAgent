// <copyright file="FreeAgentOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Configuration options for the FreeAgent API client.
/// </summary>
public class FreeAgentOptions
{
    /// <summary>
    /// The configuration section name for FreeAgent settings.
    /// </summary>
    public const string SectionName = "FreeAgent";

    /// <summary>
    /// Gets or sets the OAuth2 client ID for the FreeAgent application.
    /// </summary>
    [Required(ErrorMessage = "ClientId is required for FreeAgent authentication")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth2 client secret for the FreeAgent application.
    /// </summary>
    [Required(ErrorMessage = "ClientSecret is required for FreeAgent authentication")]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth2 refresh token for the FreeAgent application.
    /// </summary>
    /// <remarks>
    /// This token is used to obtain new access tokens without requiring user interaction.
    /// </remarks>
    [Required(ErrorMessage = "RefreshToken is required for FreeAgent authentication")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Validates the options and throws if invalid.
    /// </summary>
    public void Validate()
    {
        ValidationContext context = new(this);
        List<ValidationResult> results = [];

        if (!Validator.TryValidateObject(this, context, results, validateAllProperties: true))
        {
            string errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Invalid FreeAgent configuration: {errors}");
        }
    }
}
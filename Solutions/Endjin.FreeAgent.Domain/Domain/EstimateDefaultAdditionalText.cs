// <copyright file="EstimateDefaultAdditionalText.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the default additional text that appears on all estimates.
/// </summary>
/// <remarks>
/// <para>
/// Default additional text is a template that automatically appears on new estimates, typically used for
/// standard terms and conditions, company information, or legal disclaimers. This text can be overridden on
/// individual estimates if needed.
/// </para>
/// <para>
/// API Endpoint: /v2/estimates/default_additional_text
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="Estimate"/>
[DebuggerDisplay("{" + nameof(Text) + "}")]
public record EstimateDefaultAdditionalText
{
    /// <summary>
    /// Gets the default additional text that will appear on estimates.
    /// </summary>
    /// <value>
    /// The default text content that is automatically added to new estimates. This can include
    /// terms and conditions, validity period, or other standard estimate footer information.
    /// </value>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

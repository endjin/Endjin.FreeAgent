// <copyright file="InvoiceDefaultAdditionalText.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the default additional text that appears on all invoices.
/// </summary>
/// <remarks>
/// <para>
/// Default additional text is a template that automatically appears on new invoices, typically used for
/// standard payment terms, company information, or legal disclaimers. This text can be overridden on
/// individual invoices if needed.
/// </para>
/// <para>
/// API Endpoint: /v2/invoices/default_additional_text
/// </para>
/// <para>
/// Minimum Access Level: Invoices
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
[DebuggerDisplay("{" + nameof(Text) + "}")]
public record InvoiceDefaultAdditionalText
{
    /// <summary>
    /// Gets the default additional text that will appear on invoices.
    /// </summary>
    /// <value>
    /// The default text content that is automatically added to new invoices. This can include
    /// payment terms, bank details, or other standard invoice footer information.
    /// </value>
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

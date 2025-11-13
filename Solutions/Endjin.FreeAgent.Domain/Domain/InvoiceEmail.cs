// <copyright file="InvoiceEmail.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an email message for sending invoices or credit notes to customers via the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Invoice emails allow businesses to send professional, branded invoices directly to customers from within
/// FreeAgent. The system automatically generates a PDF attachment of the invoice and includes payment instructions
/// and company branding.
/// </para>
/// <para>
/// This type is used for both invoices and credit notes, providing a consistent email interface for customer
/// communications. The email can be customized with recipient addresses, subject line, and body text while
/// maintaining professional presentation.
/// </para>
/// <para>
/// Email features:
/// <list type="bullet">
/// <item>Automatic PDF generation and attachment of the invoice/credit note</item>
/// <item>Multiple recipients via To, CC, and BCC fields</item>
/// <item>Customizable subject line and message body</item>
/// <item>Company branding and payment information automatically included</item>
/// <item>Option to send with or without PDF attachment</item>
/// <item>Delivery tracking and email history</item>
/// </list>
/// </para>
/// <para>
/// Common use cases:
/// <list type="bullet">
/// <item>Sending invoices to customers for payment</item>
/// <item>Following up on overdue invoices with reminders</item>
/// <item>Sending credit notes for refunds or corrections</item>
/// <item>CCing accountants or internal stakeholders</item>
/// <item>BCCing for record-keeping without revealing other recipients</item>
/// </list>
/// </para>
/// <para>
/// API Access: Used with POST /v2/invoices/{id}/send_email and POST /v2/credit_notes/{id}/send_email
/// Minimum Access Level: Invoicing access
/// </para>
/// </remarks>
/// <seealso cref="Invoice"/>
/// <seealso cref="CreditNote"/>
/// <seealso cref="InvoiceEmailWrapper"/>
/// <seealso cref="CreditNoteEmailWrapper"/>
public record InvoiceEmail
{
    /// <summary>
    /// Gets the primary recipient email address(es).
    /// </summary>
    /// <value>
    /// One or more email addresses for the main recipients. Multiple addresses can be separated by commas.
    /// Typically the customer's email address from their contact record. This field is required.
    /// </value>
    [JsonPropertyName("to")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? To { get; init; }

    /// <summary>
    /// Gets the carbon copy (CC) recipient email address(es).
    /// </summary>
    /// <value>
    /// Optional email addresses for recipients who should receive a copy. Multiple addresses can be
    /// separated by commas. CC recipients are visible to all recipients. Useful for copying accountants,
    /// managers, or other stakeholders.
    /// </value>
    [JsonPropertyName("cc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Cc { get; init; }

    /// <summary>
    /// Gets the blind carbon copy (BCC) recipient email address(es).
    /// </summary>
    /// <value>
    /// Optional email addresses for recipients who should receive a copy without being visible to other
    /// recipients. Multiple addresses can be separated by commas. Useful for record-keeping or discreet
    /// notification of internal staff.
    /// </value>
    [JsonPropertyName("bcc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bcc { get; init; }

    /// <summary>
    /// Gets the email subject line.
    /// </summary>
    /// <value>
    /// The subject line for the email. If not specified, FreeAgent generates a default subject line
    /// such as "Invoice #12345 from Your Company Name" or "Credit Note #12345 from Your Company Name".
    /// </value>
    [JsonPropertyName("subject")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subject { get; init; }

    /// <summary>
    /// Gets the email message body text.
    /// </summary>
    /// <value>
    /// The main content of the email message. Can include personalized messages, payment instructions,
    /// or any relevant information for the recipient. If not specified, FreeAgent uses a default
    /// professional message template.
    /// </value>
    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Body { get; init; }

    /// <summary>
    /// Gets a value indicating whether to attach the invoice/credit note as a PDF.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to include a PDF attachment of the invoice or credit note;
    /// <see langword="false"/> to send the email without attachment. Defaults to true if not specified.
    /// Most scenarios include the PDF attachment for customer reference and payment.
    /// </value>
    [JsonPropertyName("send_pdf_attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendPdfAttachment { get; init; }
}
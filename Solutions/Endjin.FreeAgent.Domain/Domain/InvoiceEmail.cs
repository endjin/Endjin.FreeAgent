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
    /// Gets the sender email address.
    /// </summary>
    /// <value>
    /// The email address to use as the sender. This must be a verified sender email address from
    /// the FreeAgent account. Use the EmailAddresses service to retrieve the list of
    /// valid sender addresses. The sender's name and email will appear in the From field of the
    /// email sent to customers.
    /// </value>
    [JsonPropertyName("from")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? From { get; init; }

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
    /// Gets a value indicating whether to use a predefined email template.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to use the default FreeAgent email template instead of custom subject and body;
    /// <see langword="false"/> to use the custom <see cref="Subject"/> and <see cref="Body"/> values.
    /// When set to true, the <see cref="Subject"/> and <see cref="Body"/> fields are ignored and FreeAgent
    /// generates professional default content.
    /// </value>
    [JsonPropertyName("use_template")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? UseTemplate { get; init; }

    /// <summary>
    /// Gets a value indicating whether to send a copy of the email to the sender.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to send a copy to the authenticated user who is sending the email;
    /// <see langword="false"/> to only send to the recipients specified in <see cref="To"/>, <see cref="Cc"/>,
    /// and <see cref="Bcc"/>. Defaults to true if not specified, ensuring the sender has a record of sent invoices.
    /// </value>
    [JsonPropertyName("email_to_sender")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? EmailToSender { get; init; }

    /// <summary>
    /// Gets a value indicating whether to attach expense receipt PDFs to the email.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to include PDF attachments of any expense receipts associated with the invoice;
    /// <see langword="false"/> to only include the invoice PDF. This is useful when expenses are being passed
    /// through to the client and supporting documentation is required.
    /// </value>
    [JsonPropertyName("attach_expense_receipts")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AttachExpenseReceipts { get; init; }

    /// <summary>
    /// Gets the collection of additional file attachments to include with the email.
    /// </summary>
    /// <value>
    /// A list of <see cref="EmailAttachment"/> objects containing file data to include with the email.
    /// Each attachment has a maximum size of 5MB. This allows including supporting documents,
    /// contracts, terms and conditions, or other relevant files alongside the invoice.
    /// </value>
    /// <remarks>
    /// The FreeAgent API accepts attachments as an array of objects with file name, content type,
    /// and base64-encoded data. Maximum total size across all attachments should not exceed
    /// reasonable email limits (typically 10-25MB total).
    /// </remarks>
    /// <seealso cref="EmailAttachment"/>
    [JsonPropertyName("attachments")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<EmailAttachment>? Attachments { get; init; }
}
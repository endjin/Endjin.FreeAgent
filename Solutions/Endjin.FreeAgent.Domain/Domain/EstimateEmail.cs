// <copyright file="EstimateEmail.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents an email message for sending estimates (quotes) to customers via the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// Estimate emails allow businesses to send professional, branded estimates directly to potential customers
/// from within FreeAgent. The system automatically generates a PDF attachment of the estimate and includes
/// company branding and contact information.
/// </para>
/// <para>
/// The email can be customized with recipient addresses, subject line, and body text while maintaining
/// professional presentation. This is commonly used for sending quotes to clients before work begins.
/// </para>
/// <para>
/// Email features:
/// <list type="bullet">
/// <item>Automatic PDF generation and attachment of the estimate</item>
/// <item>Multiple recipients via To, CC, and BCC fields</item>
/// <item>Customizable subject line and message body</item>
/// <item>Company branding automatically included</item>
/// <item>Option to send with or without PDF attachment</item>
/// </list>
/// </para>
/// <para>
/// API Access: Used with POST /v2/estimates/{id}/send_email
/// Minimum Access Level: Invoicing access
/// </para>
/// </remarks>
/// <seealso cref="Estimate"/>
/// <seealso cref="EstimateEmailWrapper"/>
public record EstimateEmail
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
    /// separated by commas. CC recipients are visible to all recipients.
    /// </value>
    [JsonPropertyName("cc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Cc { get; init; }

    /// <summary>
    /// Gets the blind carbon copy (BCC) recipient email address(es).
    /// </summary>
    /// <value>
    /// Optional email addresses for recipients who should receive a copy without being visible to other
    /// recipients. Multiple addresses can be separated by commas.
    /// </value>
    [JsonPropertyName("bcc")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Bcc { get; init; }

    /// <summary>
    /// Gets the email subject line.
    /// </summary>
    /// <value>
    /// The subject line for the email. If not specified, FreeAgent generates a default subject line
    /// such as "Estimate #12345 from Your Company Name".
    /// </value>
    [JsonPropertyName("subject")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Subject { get; init; }

    /// <summary>
    /// Gets the email message body text.
    /// </summary>
    /// <value>
    /// The main content of the email message. Can include personalized messages or any relevant
    /// information for the recipient. If not specified, FreeAgent uses a default professional message template.
    /// </value>
    [JsonPropertyName("body")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Body { get; init; }

    /// <summary>
    /// Gets a value indicating whether to attach the estimate as a PDF.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to include a PDF attachment of the estimate;
    /// <see langword="false"/> to send the email without attachment. Defaults to true if not specified.
    /// </value>
    [JsonPropertyName("send_pdf_attachment")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendPdfAttachment { get; init; }

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
    /// and <see cref="Bcc"/>. Defaults to true if not specified, ensuring the sender has a record of sent estimates.
    /// </value>
    [JsonPropertyName("email_to_sender")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? EmailToSender { get; init; }

    /// <summary>
    /// Gets the collection of additional file attachments to include with the email.
    /// </summary>
    /// <value>
    /// A list of <see cref="EmailAttachment"/> objects containing file data to include with the email.
    /// Each attachment has a maximum size of 5MB. This allows including supporting documents,
    /// specifications, terms and conditions, or other relevant files alongside the estimate.
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

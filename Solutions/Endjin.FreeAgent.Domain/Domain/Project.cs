// <copyright file="Project.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a project in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Projects are used to organize work, track time, and manage billing for a specific client engagement or internal initiative.
/// Each project is associated with a <see cref="Contact"/> and can have multiple <see cref="Timeslip"/> entries,
/// invoices, and other related resources.
/// </para>
/// <para>
/// Projects support various status states (Active, Completed, Cancelled, Hidden) and can track budgets in terms
/// of hours, days, or monetary amounts. Projects can be configured with billing rates and associated with
/// UK IR35 tax status for compliance purposes.
/// </para>
/// <para>
/// API Endpoint: /v2/projects
/// </para>
/// <para>
/// Minimum Access Level: Contacts and Projects
/// </para>
/// </remarks>
/// <seealso cref="Contact"/>
/// <seealso cref="Timeslip"/>
/// <seealso cref="Invoice"/>
[DebuggerDisplay("Name = {ContactEntry.OrganisationName}{Name}")]
public record Project
{
    /// <summary>
    /// Gets the unique URI identifier for this project.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this project in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the project in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the <see cref="Contact"/> associated with this project.
    /// </summary>
    /// <value>
    /// The URI of the contact (client or supplier) for whom this project is being performed. This field is required when creating a project.
    /// </value>
    /// <seealso cref="ContactEntry"/>
    [JsonPropertyName("contact")]
    public required Uri Contact { get; init; }

    /// <summary>
    /// Gets the display name of the contact associated with this project.
    /// </summary>
    /// <value>
    /// The display name of the contact for whom this project is being performed.
    /// This value is returned by the API but is read-only.
    /// </value>
    [JsonPropertyName("contact_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContactName { get; init; }

    /// <summary>
    /// Gets the <see cref="Domain.Contact"/> object associated with this project.
    /// </summary>
    /// <value>
    /// The full contact object when nested resources are requested. This property is not serialized to JSON.
    /// </value>
    /// <seealso cref="Contact"/>
    [JsonIgnore]
    public Contact? ContactEntry { get; init; }

    /// <summary>
    /// Gets the name of the project.
    /// </summary>
    /// <value>
    /// A descriptive name identifying the project. This field is required when creating a project.
    /// </value>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the current status of the project.
    /// </summary>
    /// <value>
    /// Required. One of "Active", "Completed", "Cancelled", or "Hidden".
    /// </value>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// Gets the contract or purchase order reference number for this project.
    /// </summary>
    /// <value>
    /// An optional reference number used for tracking contracts or purchase orders associated with this project.
    /// </value>
    [JsonPropertyName("contract_po_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ContractPoReference { get; init; }

    /// <summary>
    /// Gets a value indicating whether this project uses a separate invoice numbering sequence.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if this project has its own invoice numbering sequence; otherwise, <see langword="false"/>
    /// to use the contact or global invoice numbering sequence. This field is required when creating a project.
    /// </value>
    [JsonPropertyName("uses_project_invoice_sequence")]
    public required bool UsesProjectInvoiceSequence { get; init; }

    /// <summary>
    /// Gets the currency code for this project.
    /// </summary>
    /// <value>
    /// A three-letter ISO 4217 currency code (e.g., "GBP", "USD", "EUR").
    /// This field is required when creating a project.
    /// </value>
    [JsonPropertyName("currency")]
    public required string Currency { get; init; }

    /// <summary>
    /// Gets the budget amount for this project.
    /// </summary>
    /// <value>
    /// The budget value, interpreted according to <see cref="BudgetUnits"/>. Can represent hours, days, or monetary amount.
    /// This field is required when creating a project. Use 0 for no budget.
    /// </value>
    /// <seealso cref="BudgetUnits"/>
    [JsonPropertyName("budget")]
    public required decimal Budget { get; init; }

    /// <summary>
    /// Gets the units for the project budget.
    /// </summary>
    /// <value>
    /// One of "Hours", "Days", or "Monetary (ex-VAT)", determining how <see cref="Budget"/> should be interpreted.
    /// </value>
    /// <seealso cref="Budget"/>
    [JsonPropertyName("budget_units")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BudgetUnits { get; init; }

    /// <summary>
    /// Gets the number of hours considered to constitute a working day for this project.
    /// </summary>
    /// <value>
    /// A decimal value representing hours per day (e.g., 1.5 for 1 hour 30 minutes, or 7.5 for a standard working day).
    /// </value>
    [JsonPropertyName("hours_per_day")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? HoursPerDay { get; init; }

    /// <summary>
    /// Gets the standard billing rate for this project.
    /// </summary>
    /// <value>
    /// The hourly or daily billing rate, interpreted according to <see cref="BillingPeriod"/>.
    /// </value>
    /// <seealso cref="BillingPeriod"/>
    [JsonPropertyName("normal_billing_rate")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public decimal? NormalBillingRate { get; init; }

    /// <summary>
    /// Gets the billing period unit for this project's billing rate.
    /// </summary>
    /// <value>
    /// Either "hour" or "day", determining the unit for <see cref="NormalBillingRate"/>.
    /// </value>
    /// <seealso cref="NormalBillingRate"/>
    [JsonPropertyName("billing_period")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? BillingPeriod { get; init; }

    /// <summary>
    /// Gets a value indicating whether this project falls under UK IR35 off-payroll working rules.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the project is subject to IR35 regulations; otherwise, <see langword="false"/>.
    /// IR35 is UK tax legislation affecting contractors and determines employment status for tax purposes.
    /// </value>
    [JsonPropertyName("is_ir35")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsIr35 { get; init; }

    /// <summary>
    /// Gets a value indicating whether this project is associated with a draft estimate.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the project is associated with a draft estimate; otherwise, <see langword="false"/>.
    /// This is a client-side property set when processing estimates and is not serialized to JSON.
    /// </value>
    [JsonIgnore]
    public bool? IsEstimate { get; init; }

    /// <summary>
    /// Gets the start date for this project.
    /// </summary>
    /// <value>
    /// The date when the project is scheduled to begin, in ISO 8601 format (YYYY-MM-DD).
    /// </value>
    [JsonPropertyName("starts_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? StartsOn { get; init; }

    /// <summary>
    /// Gets the end date for this project.
    /// </summary>
    /// <value>
    /// The date when the project is scheduled to end, in ISO 8601 format (YYYY-MM-DD).
    /// </value>
    [JsonPropertyName("ends_on")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? EndsOn { get; init; }

    /// <summary>
    /// Gets a value indicating whether unbilled time should be included in profitability calculations.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to include unbilled time in profitability reports; otherwise, <see langword="false"/>.
    /// </value>
    [JsonPropertyName("include_unbilled_time_in_profitability")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IncludeUnbilledTimeInProfitability { get; init; }

    /// <summary>
    /// Gets a value indicating whether this project can be deleted.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the project can be deleted; otherwise, <see langword="false"/>.
    /// This property is only returned when retrieving a single project.
    /// </value>
    [JsonPropertyName("is_deletable")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsDeletable { get; init; }

    /// <summary>
    /// Gets the date and time when this project was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this project was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the collection of timeslip entries associated with this project.
    /// </summary>
    /// <value>
    /// An immutable list of <see cref="Timeslip"/> objects representing time tracked against this project.
    /// This property is not serialized to JSON.
    /// </value>
    /// <seealso cref="Timeslip"/>
    [JsonIgnore]
    public ImmutableList<Timeslip> TimeslipEntries { get; init; } = [];
}
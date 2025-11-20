// <copyright file="TrialBalanceSummaryEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a single entry in a trial balance summary from the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Trial balance summary entries show account balances grouped by category. Each entry represents
/// one line in the trial balance report, containing the category reference, nominal codes, name,
/// and total balance.
/// </para>
/// <para>
/// Special handling applies to certain categories:
/// <list type="bullet">
/// <item><description>Bank account categories include a reference to the associated bank account</description></item>
/// <item><description>User-related categories include a reference to the associated user</description></item>
/// </list>
/// </para>
/// <para>
/// The <see cref="DisplayNominalCode"/> should be preferred over <see cref="NominalCode"/> as it
/// always returns the full category code as displayed on accounting reports in the FreeAgent UI.
/// </para>
/// </remarks>
/// <seealso cref="TrialBalanceSummaryRoot"/>
/// <seealso cref="Category"/>
public record TrialBalanceSummaryEntry
{
    /// <summary>
    /// Gets the URI reference to the accounting category for this entry.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Category"/> resource in the chart of accounts that this trial balance line represents.
    /// </value>
    [JsonPropertyName("category")]
    public required Uri Category { get; init; }

    /// <summary>
    /// Gets the legacy nominal code for this category.
    /// </summary>
    /// <value>
    /// The legacy numeric code for this category. For codes 750 and 900-910, the second part
    /// references the resource ID. Prefer <see cref="DisplayNominalCode"/> for display purposes.
    /// </value>
    [JsonPropertyName("nominal_code")]
    public required string NominalCode { get; init; }

    /// <summary>
    /// Gets the display nominal code for this category.
    /// </summary>
    /// <value>
    /// The full category code as displayed on accounting reports in the FreeAgent UI.
    /// This should be preferred over <see cref="NominalCode"/> for display purposes.
    /// </value>
    [JsonPropertyName("display_nominal_code")]
    public required string DisplayNominalCode { get; init; }

    /// <summary>
    /// Gets the name of the category.
    /// </summary>
    /// <value>
    /// The human-readable name of the accounting category, such as "Sales", "Cost of Sales",
    /// "Bank Charges", or "Share Capital".
    /// </value>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Gets the total balance amount for this category.
    /// </summary>
    /// <value>
    /// The balance amount for this category. Positive values typically represent debit balances
    /// (assets, expenses), while negative values represent credit balances (liabilities, equity, income).
    /// </value>
    [JsonPropertyName("total")]
    public required decimal Total { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated bank account, if applicable.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="BankAccount"/> resource associated with this category,
    /// or <c>null</c> if this category is not a bank account category.
    /// </value>
    [JsonPropertyName("bank_account")]
    public Uri? BankAccount { get; init; }

    /// <summary>
    /// Gets the URI reference to the associated user, if applicable.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="User"/> resource associated with this category,
    /// or <c>null</c> if this category is not a user-related category.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }
}

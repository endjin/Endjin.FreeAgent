// <copyright file="OpeningBalance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents opening balances for a company when first setting up accounts in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Opening balances establish the initial financial position when migrating to FreeAgent from another accounting
/// system or when starting to use FreeAgent mid-year. They represent the carried-forward balances from the
/// previous accounting system or period.
/// </para>
/// <para>
/// Opening balances typically include:
/// - Bank account balances at the migration date
/// - Outstanding customer invoices (accounts receivable)
/// - Unpaid supplier bills (accounts payable)
/// - Asset and liability balances
/// - Equity/capital balances
/// </para>
/// <para>
/// The opening balance is structured as a journal entry that must balance (debits equal credits), ensuring
/// the accounting equation remains valid when importing historical data. This provides continuity and accurate
/// reporting from the transition date forward.
/// </para>
/// <para>
/// API Endpoint: /v2/opening_balances
/// </para>
/// <para>
/// Minimum Access Level: Full Access
/// </para>
/// </remarks>
/// <seealso cref="OpeningBalanceJournal"/>
/// <seealso cref="JournalSet"/>
/// <seealso cref="BankAccount"/>
public record OpeningBalance
{
    /// <summary>
    /// Gets the unique URI identifier for this opening balance.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this opening balance in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the journal containing the opening balance entries.
    /// </summary>
    /// <value>
    /// An <see cref="OpeningBalanceJournal"/> containing the detailed journal entries that establish
    /// the initial account balances. These entries must balance and represent the financial position
    /// at the start of FreeAgent usage.
    /// </value>
    [JsonPropertyName("journal")]
    public OpeningBalanceJournal? Journal { get; init; }

    /// <summary>
    /// Gets the date and time when this opening balance was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing when the opening balance was first entered into the system.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this opening balance was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last time the opening balance was modified.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
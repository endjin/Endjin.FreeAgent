// <copyright file="CorporationTaxPaymentStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Domain;

/// <summary>
/// Represents the payment status of a Corporation Tax return in FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// This enum reflects the payment state of a Corporation Tax return.
/// </para>
/// <para>
/// The FreeAgent API represents payment status values as strings:
/// "unpaid" and "marked_as_paid". This field is omitted if no payment is required.
/// </para>
/// </remarks>
public enum CorporationTaxPaymentStatus
{
    /// <summary>
    /// The Corporation Tax payment is unpaid.
    /// </summary>
    Unpaid,

    /// <summary>
    /// The Corporation Tax payment has been marked as paid.
    /// </summary>
    MarkedAsPaid
}
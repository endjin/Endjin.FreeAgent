// <copyright file="SalesTaxStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the sales tax status for an expense.
/// </summary>
/// <remarks>
/// <para>
/// This status determines how VAT/GST is applied to an expense.
/// </para>
/// </remarks>
public enum SalesTaxStatus
{
    /// <summary>
    /// Indicates the expense is subject to sales tax.
    /// </summary>
    Taxable,

    /// <summary>
    /// Indicates the expense is exempt from sales tax.
    /// </summary>
    Exempt,

    /// <summary>
    /// Indicates the expense is out of scope for sales tax.
    /// </summary>
    OutOfScope
}

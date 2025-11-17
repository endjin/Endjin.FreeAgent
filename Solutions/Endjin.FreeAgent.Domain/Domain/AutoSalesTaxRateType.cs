// <copyright file="AutoSalesTaxRateType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Defines the VAT/sales tax classification options available in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Auto sales tax rate types determine how VAT is automatically applied to income and spending categories.
/// These classifications help ensure correct VAT treatment for different types of transactions.
/// </para>
/// <para>
/// Note that the <see cref="Exempt"/> classification is only available for income categories,
/// not for spending categories. This reflects the tax treatment where income can be exempt from VAT,
/// but spending categories typically cannot claim exemption in the same way.
/// </para>
/// </remarks>
/// <seealso cref="Category"/>
/// <seealso cref="CategoryCreateRequest"/>
/// <seealso cref="CategoryUpdateRequest"/>
public enum AutoSalesTaxRateType
{
    /// <summary>
    /// Transactions are outside the scope of VAT (e.g., non-business activities).
    /// </summary>
    OutsideScope,

    /// <summary>
    /// Zero-rated VAT applies (0% rate but still within VAT system).
    /// </summary>
    ZeroRate,

    /// <summary>
    /// Reduced VAT rate applies (typically 5% in the UK).
    /// </summary>
    ReducedRate,

    /// <summary>
    /// Standard VAT rate applies (typically 20% in the UK).
    /// </summary>
    StandardRate,

    /// <summary>
    /// VAT exempt transactions (available for income categories only).
    /// </summary>
    /// <remarks>
    /// This option is only valid for income categories and will cause an error
    /// if used with spending or other category types.
    /// </remarks>
    Exempt,
}
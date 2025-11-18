// <copyright file="EcStatus.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the European Community VAT reporting status for expenses.
/// </summary>
/// <remarks>
/// <para>
/// This status is used for VAT reporting purposes on expenses. The available
/// options changed post-Brexit for GB-based companies.
/// </para>
/// </remarks>
public enum EcStatus
{
    /// <summary>
    /// Indicates the expense is from a UK or non-EC source.
    /// </summary>
    UkNonEc,

    /// <summary>
    /// Indicates the expense is for goods from an EC country.
    /// </summary>
    EcGoods,

    /// <summary>
    /// Indicates the expense is for services from an EC country.
    /// </summary>
    EcServices,

    /// <summary>
    /// Indicates the expense is subject to reverse charge VAT.
    /// </summary>
    ReverseCharge
}

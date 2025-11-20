// <copyright file="CompanyType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the type of business structure for a company in the FreeAgent system.
/// </summary>
/// <remarks>
/// <para>
/// The company type determines applicable tax and regulatory requirements. Different company
/// types have different features and requirements based on their jurisdiction (UK, US, or Universal).
/// </para>
/// </remarks>
public enum CompanyType
{
    /// <summary>
    /// UK Limited Company - A private limited company registered in the UK.
    /// </summary>
    UkLimitedCompany,

    /// <summary>
    /// UK Limited Liability Partnership - An LLP registered in the UK.
    /// </summary>
    UkLimitedLiabilityPartnership,

    /// <summary>
    /// UK Partnership - A general partnership operating in the UK.
    /// </summary>
    UkPartnership,

    /// <summary>
    /// UK Sole Trader - An individual operating as a sole proprietorship in the UK.
    /// </summary>
    UkSoleTrader,

    /// <summary>
    /// UK Unincorporated Landlord - A UK landlord operating as an unincorporated entity.
    /// </summary>
    UkUnincorporatedLandlord,

    /// <summary>
    /// US Limited Liability Company - An LLC registered in the United States.
    /// </summary>
    UsLimitedLiabilityCompany,

    /// <summary>
    /// US Partnership - A general partnership operating in the United States.
    /// </summary>
    UsPartnership,

    /// <summary>
    /// US Sole Proprietor - An individual operating as a sole proprietorship in the United States.
    /// </summary>
    UsSoleProprietor,

    /// <summary>
    /// US C Corporation - A C-Corp registered in the United States.
    /// </summary>
    UsCCorp,

    /// <summary>
    /// US S Corporation - An S-Corp registered in the United States.
    /// </summary>
    UsSCorp,

    /// <summary>
    /// Universal Company - A company type for entities operating outside UK and US jurisdictions.
    /// </summary>
    UniversalCompany,
}
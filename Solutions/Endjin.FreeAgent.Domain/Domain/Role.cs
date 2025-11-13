// <copyright file="Role.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Defines the organizational roles available for users in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Roles indicate a user's position within the company and affect certain business logic and reporting features.
/// While roles are distinct from permission levels, they provide important context for user relationships
/// and responsibilities within the organization.
/// </para>
/// <para>
/// The role assigned to a user does not directly control access permissions; instead, use the permission level
/// property (0-8) on the <see cref="User"/> object to manage feature access.
/// </para>
/// </remarks>
/// <seealso cref="User"/>
public enum Role
{
    /// <summary>
    /// The company owner, typically the founder or primary stakeholder.
    /// </summary>
    Owner,

    /// <summary>
    /// A company director with formal governance responsibilities.
    /// </summary>
    Director,

    /// <summary>
    /// A partner in the business, relevant for partnerships and multi-owner structures.
    /// </summary>
    Partner,

    /// <summary>
    /// The company secretary responsible for corporate compliance and governance.
    /// </summary>
    CompanySecretary,

    /// <summary>
    /// An employee of the company.
    /// </summary>
    Employee,

    /// <summary>
    /// A shareholder with ownership interest in the company.
    /// </summary>
    Shareholder,

    /// <summary>
    /// An accountant or bookkeeper providing professional accounting services.
    /// </summary>
    Accountant,
};
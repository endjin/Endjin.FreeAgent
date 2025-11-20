// <copyright file="RebillType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the rebilling method for an expense.
/// </summary>
/// <remarks>
/// <para>
/// This type determines how an expense associated with a project will be rebilled to the client.
/// </para>
/// </remarks>
public enum RebillType
{
    /// <summary>
    /// Rebill at the actual cost of the expense.
    /// </summary>
    Cost,

    /// <summary>
    /// Add a percentage markup to the expense cost when rebilling.
    /// </summary>
    Markup,

    /// <summary>
    /// Rebill at a fixed price regardless of actual cost.
    /// </summary>
    Price
}

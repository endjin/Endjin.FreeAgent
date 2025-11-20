// <copyright file="RecurringPattern.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the recurrence pattern for a recurring expense.
/// </summary>
public enum RecurringPattern
{
    /// <summary>
    /// Expense recurs every week.
    /// </summary>
    Weekly,

    /// <summary>
    /// Expense recurs every two weeks.
    /// </summary>
    TwoWeekly,

    /// <summary>
    /// Expense recurs every four weeks.
    /// </summary>
    FourWeekly,

    /// <summary>
    /// Expense recurs every month.
    /// </summary>
    Monthly,

    /// <summary>
    /// Expense recurs every two months.
    /// </summary>
    TwoMonthly,

    /// <summary>
    /// Expense recurs every three months.
    /// </summary>
    Quarterly,

    /// <summary>
    /// Expense recurs every six months.
    /// </summary>
    Biannually,

    /// <summary>
    /// Expense recurs every year.
    /// </summary>
    Annually,

    /// <summary>
    /// Expense recurs every two years.
    /// </summary>
    TwoYearly
}

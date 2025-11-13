// <copyright file="FreeAgentDateTimeFormatExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NodaTime;

using System.Globalization;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Provides extension methods for formatting dates in the FreeAgent API's required format.
/// </summary>
/// <remarks>
/// <para>
/// The FreeAgent API requires dates to be formatted as ISO 8601 date strings (yyyy-MM-dd format).
/// These extension methods provide convenient conversion from .NET date types (DateTime and NodaTime LocalDate)
/// to the required string format.
/// </para>
/// <para>
/// Using these extensions ensures consistent date formatting across the client library and prevents
/// localization issues by using InvariantCulture formatting.
/// </para>
/// <para>
/// Example usage:
/// <code>
/// DateTime date = DateTime.Today;
/// string formattedDate = date.ToFreeAgentDateString(); // Returns "2024-01-15"
///
/// LocalDate nodaDate = LocalDate.FromDateTime(DateTime.Today);
/// string nodaFormatted = nodaDate.ToFreeAgentDateString(); // Returns "2024-01-15"
/// </code>
/// </para>
/// </remarks>
public static class FreeAgentDateTimeFormatExtensions
{
    /// <summary>
    /// Converts a NodaTime LocalDate to the FreeAgent API date string format (yyyy-MM-dd).
    /// </summary>
    /// <param name="date">The LocalDate to format.</param>
    /// <returns>A string in yyyy-MM-dd format suitable for FreeAgent API requests.</returns>
    public static string ToFreeAgentDateString(this LocalDate date)
    {
        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a .NET DateTime to the FreeAgent API date string format (yyyy-MM-dd).
    /// </summary>
    /// <param name="date">The DateTime to format.</param>
    /// <returns>A string in yyyy-MM-dd format suitable for FreeAgent API requests.</returns>
    /// <remarks>
    /// This method only formats the date component. Time information is ignored.
    /// </remarks>
    public static string ToFreeAgentDateString(this DateTime date)
    {
        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
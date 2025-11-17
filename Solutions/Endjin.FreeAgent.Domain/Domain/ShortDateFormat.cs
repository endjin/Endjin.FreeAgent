// <copyright file="ShortDateFormat.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Defines the valid short date format options for displaying dates in the FreeAgent interface.
/// </summary>
/// <remarks>
/// <para>
/// These format strings control how dates are displayed throughout the FreeAgent user interface.
/// The format selected affects all date displays for the company account.
/// </para>
/// </remarks>
public static class ShortDateFormat
{
    /// <summary>
    /// Gets the abbreviated month format (e.g., "01 Jan 20").
    /// </summary>
    public const string AbbreviatedMonth = "dd mmm yy";

    /// <summary>
    /// Gets the European format with dashes (e.g., "01-01-2020").
    /// </summary>
    public const string European = "dd-mm-yyyy";

    /// <summary>
    /// Gets the US format with slashes (e.g., "01/31/2020").
    /// </summary>
    public const string US = "mm/dd/yyyy";

    /// <summary>
    /// Gets the ISO 8601 format (e.g., "2020-01-31").
    /// </summary>
    public const string ISO = "yyyy-mm-dd";

    /// <summary>
    /// Determines whether the specified value is a valid short date format.
    /// </summary>
    /// <param name="value">The format string to validate.</param>
    /// <returns><see langword="true"/> if the value is a valid short date format; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(string? value)
    {
        return value switch
        {
            AbbreviatedMonth => true,
            European => true,
            US => true,
            ISO => true,
            _ => false,
        };
    }

    /// <summary>
    /// Gets all valid short date format values.
    /// </summary>
    /// <returns>An array containing all valid short date format strings.</returns>
    public static string[] GetValidFormats()
    {
        return new[]
        {
            AbbreviatedMonth,
            European,
            US,
            ISO,
        };
    }
}
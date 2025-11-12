// <copyright file="FreeAgentDateTimeFormatExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using NodaTime;

using System.Globalization;

namespace Endjin.FreeAgent.Domain;

public static class FreeAgentDateTimeFormatExtensions
{
    public static string ToFreeAgentDateString(this LocalDate date)
    {
        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public static string ToFreeAgentDateString(this DateTime date)
    {
        return date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
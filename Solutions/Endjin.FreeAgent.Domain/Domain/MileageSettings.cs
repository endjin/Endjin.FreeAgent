// <copyright file="MileageSettings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the mileage settings configuration from FreeAgent.
/// </summary>
/// <remarks>
/// <para>
/// Mileage settings contain the engine type and size options, as well as mileage rates for different
/// vehicle types over different time periods. These settings are used to calculate mileage expense
/// reimbursements based on HMRC AMAP (Approved Mileage Allowance Payments) rates.
/// </para>
/// <para>
/// API Endpoint: GET /v2/expenses/mileage_settings
/// </para>
/// </remarks>
public record MileageSettings
{
    /// <summary>
    /// Gets the available engine type and size options over different time periods.
    /// </summary>
    /// <value>
    /// A collection of time-bounded options specifying which engine types and sizes are available.
    /// Different options apply based on the expense date.
    /// </value>
    [JsonPropertyName("engine_type_and_size_options")]
    public IReadOnlyList<EngineTypeAndSizeOption>? EngineTypeAndSizeOptions { get; init; }

    /// <summary>
    /// Gets the mileage rates for different vehicle types over different time periods.
    /// </summary>
    /// <value>
    /// A collection of time-bounded mileage rates. Rates vary by vehicle type and may change
    /// annually based on HMRC guidelines.
    /// </value>
    [JsonPropertyName("mileage_rates")]
    public IReadOnlyList<MileageRateOption>? MileageRates { get; init; }
}

/// <summary>
/// Represents engine type and size options for a specific time period.
/// </summary>
public record EngineTypeAndSizeOption
{
    /// <summary>
    /// Gets the start date from which these options apply.
    /// </summary>
    /// <value>
    /// The date from which these engine type and size options are valid.
    /// </value>
    [JsonPropertyName("from")]
    public DateOnly? From { get; init; }

    /// <summary>
    /// Gets the end date until which these options apply.
    /// </summary>
    /// <value>
    /// The date until which these engine type and size options are valid.
    /// </value>
    [JsonPropertyName("to")]
    public DateOnly? To { get; init; }

    /// <summary>
    /// Gets the engine types and their available size options.
    /// </summary>
    /// <value>
    /// A dictionary mapping engine type names (e.g., "Petrol", "Diesel", "Electric") to arrays
    /// of available engine sizes (e.g., "Up to 1400cc", "1401cc to 2000cc").
    /// </value>
    [JsonPropertyName("value")]
    public Dictionary<string, string[]>? Value { get; init; }
}

/// <summary>
/// Represents mileage rates for a specific time period.
/// </summary>
public record MileageRateOption
{
    /// <summary>
    /// Gets the start date from which these rates apply.
    /// </summary>
    /// <value>
    /// The date from which these mileage rates are valid.
    /// </value>
    [JsonPropertyName("from")]
    public DateOnly? From { get; init; }

    /// <summary>
    /// Gets the end date until which these rates apply.
    /// </summary>
    /// <value>
    /// The date until which these mileage rates are valid.
    /// </value>
    [JsonPropertyName("to")]
    public DateOnly? To { get; init; }

    /// <summary>
    /// Gets the mileage rates and limits for this time period.
    /// </summary>
    /// <value>
    /// The rates for different vehicle types and the basic rate mileage limit.
    /// </value>
    [JsonPropertyName("value")]
    public MileageRatesValue? Value { get; init; }
}

/// <summary>
/// Represents the mileage rates for different vehicle types.
/// </summary>
public record MileageRatesValue
{
    /// <summary>
    /// Gets the mileage rates for cars.
    /// </summary>
    /// <value>
    /// The basic and additional rates for car mileage expenses.
    /// </value>
    [JsonPropertyName("Car")]
    public VehicleMileageRate? Car { get; init; }

    /// <summary>
    /// Gets the mileage rates for motorcycles.
    /// </summary>
    /// <value>
    /// The basic and additional rates for motorcycle mileage expenses.
    /// </value>
    [JsonPropertyName("Motorcycle")]
    public VehicleMileageRate? Motorcycle { get; init; }

    /// <summary>
    /// Gets the mileage rates for bicycles.
    /// </summary>
    /// <value>
    /// The basic and additional rates for bicycle mileage expenses.
    /// </value>
    [JsonPropertyName("Bicycle")]
    public VehicleMileageRate? Bicycle { get; init; }

    /// <summary>
    /// Gets the mileage limit before the additional rate applies.
    /// </summary>
    /// <value>
    /// The number of miles per tax year before the additional (lower) rate applies.
    /// For HMRC AMAP, this is typically 10,000 miles.
    /// </value>
    [JsonPropertyName("basic_rate_limit")]
    public int? BasicRateLimit { get; init; }
}

/// <summary>
/// Represents the mileage rates for a specific vehicle type.
/// </summary>
public record VehicleMileageRate
{
    /// <summary>
    /// Gets the basic mileage rate for the first miles up to the limit.
    /// </summary>
    /// <value>
    /// The rate per mile for miles up to the basic rate limit (e.g., first 10,000 miles).
    /// </value>
    [JsonPropertyName("basic_rate")]
    public decimal? BasicRate { get; init; }

    /// <summary>
    /// Gets the additional mileage rate for miles beyond the limit.
    /// </summary>
    /// <value>
    /// The rate per mile for miles exceeding the basic rate limit.
    /// </value>
    [JsonPropertyName("additional_rate")]
    public decimal? AdditionalRate { get; init; }
}

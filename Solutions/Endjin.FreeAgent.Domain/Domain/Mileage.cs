// <copyright file="Mileage.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a mileage claim in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Mileage entries track business travel by vehicle, allowing users to claim reimbursement for business mileage
/// and optionally rebill mileage costs to clients. Each mileage entry records the distance traveled, vehicle details,
/// and calculates reimbursement based on approved rates (such as HMRC AMAP rates in the UK).
/// </para>
/// <para>
/// Mileage can be claimed for reimbursement from the company and/or rebilled to clients at different rates.
/// Vehicle type and engine details are tracked for compliance and rate calculation purposes.
/// </para>
/// <para>
/// API Endpoint: /v2/mileages
/// </para>
/// <para>
/// Minimum Access Level: My Money
/// </para>
/// </remarks>
/// <seealso cref="User"/>
/// <seealso cref="Project"/>
/// <seealso cref="Expense"/>
public record Mileage
{
    /// <summary>
    /// Gets the unique URI identifier for this mileage entry.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this mileage entry in the FreeAgent system.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the URI reference to the user who made this mileage claim.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.User"/> who recorded this mileage entry.
    /// </value>
    [JsonPropertyName("user")]
    public Uri? User { get; init; }

    /// <summary>
    /// Gets the URI reference to the project associated with this mileage claim.
    /// </summary>
    /// <value>
    /// The URI of the <see cref="Domain.Project"/> to which this mileage should be allocated or rebilled, if applicable.
    /// </value>
    [JsonPropertyName("project")]
    public Uri? Project { get; init; }

    /// <summary>
    /// Gets the date when the mileage journey occurred.
    /// </summary>
    /// <value>
    /// The journey date in YYYY-MM-DD format.
    /// </value>
    [JsonPropertyName("dated_on")]
    public DateOnly? DatedOn { get; init; }

    /// <summary>
    /// Gets the description or purpose of the mileage journey.
    /// </summary>
    /// <value>
    /// A text description explaining the business purpose of the journey, such as destination or reason for travel.
    /// </value>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Gets the distance traveled in miles.
    /// </summary>
    /// <value>
    /// The number of miles traveled during this business journey.
    /// </value>
    [JsonPropertyName("mileage")]
    public decimal? Miles { get; init; }

    /// <summary>
    /// Gets a value indicating whether the user is claiming reimbursement from the company for this mileage.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the user wants to be reimbursed by the company for this business mileage;
    /// otherwise, <see langword="false"/>.
    /// </value>
    /// <seealso cref="ReclaimMileageRate"/>
    /// <seealso cref="ReclaimMileageValue"/>
    [JsonPropertyName("reclaim_mileage")]
    public bool? ReclaimMileage { get; init; }

    /// <summary>
    /// Gets the rate per mile used to calculate the reimbursement amount.
    /// </summary>
    /// <value>
    /// The reimbursement rate per mile in the company's base currency. Typically based on approved rates
    /// such as HMRC AMAP rates in the UK.
    /// </value>
    /// <seealso cref="ReclaimMileage"/>
    /// <seealso cref="ReclaimMileageValue"/>
    [JsonPropertyName("reclaim_mileage_rate")]
    public decimal? ReclaimMileageRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether this mileage cost should be rebilled to the client.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the mileage cost should be charged to the client associated with the project;
    /// otherwise, <see langword="false"/>.
    /// </value>
    /// <seealso cref="RebillMileageRate"/>
    /// <seealso cref="RebillMileageValue"/>
    [JsonPropertyName("rebill_mileage")]
    public bool? RebillMileage { get; init; }

    /// <summary>
    /// Gets the rate per mile used to calculate the rebill amount charged to the client.
    /// </summary>
    /// <value>
    /// The billing rate per mile for client invoicing. This can differ from <see cref="ReclaimMileageRate"/>
    /// to allow markup or different charging structures.
    /// </value>
    /// <seealso cref="RebillMileage"/>
    /// <seealso cref="RebillMileageValue"/>
    [JsonPropertyName("rebill_mileage_rate")]
    public decimal? RebillMileageRate { get; init; }

    /// <summary>
    /// Gets the calculated total reimbursement amount owed to the user.
    /// </summary>
    /// <value>
    /// The total reimbursement value calculated as <see cref="Miles"/> × <see cref="ReclaimMileageRate"/>.
    /// </value>
    /// <seealso cref="ReclaimMileage"/>
    /// <seealso cref="ReclaimMileageRate"/>
    [JsonPropertyName("reclaim_mileage_value")]
    public decimal? ReclaimMileageValue { get; init; }

    /// <summary>
    /// Gets the calculated total rebill amount to charge the client.
    /// </summary>
    /// <value>
    /// The total rebill value calculated as <see cref="Miles"/> × <see cref="RebillMileageRate"/>.
    /// This amount will be added to the client's invoice for the associated project.
    /// </value>
    /// <seealso cref="RebillMileage"/>
    /// <seealso cref="RebillMileageRate"/>
    [JsonPropertyName("rebill_mileage_value")]
    public decimal? RebillMileageValue { get; init; }

    /// <summary>
    /// Gets the type of vehicle used for this journey.
    /// </summary>
    /// <value>
    /// A string identifier for the vehicle type, such as "Car", "Motorcycle", "Bicycle", or other vehicle classifications.
    /// Required for compliance and accurate rate calculation.
    /// </value>
    [JsonPropertyName("vehicle_type")]
    public string? VehicleType { get; init; }

    /// <summary>
    /// Gets the engine type of the vehicle.
    /// </summary>
    /// <value>
    /// A string describing the engine type, such as "Petrol", "Diesel", "Electric", or "Hybrid".
    /// Used for environmental reporting and rate calculation where applicable.
    /// </value>
    [JsonPropertyName("engine_type")]
    public string? EngineType { get; init; }

    /// <summary>
    /// Gets the engine size or capacity of the vehicle.
    /// </summary>
    /// <value>
    /// A string representation of the engine size, typically in cubic centimeters (cc) or liters.
    /// For example: "1600cc", "2.0L". Used for rate calculation based on engine capacity bands.
    /// </value>
    [JsonPropertyName("engine_size")]
    public string? EngineSize { get; init; }

    /// <summary>
    /// Gets the date and time when this mileage entry was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this mileage entry was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTime"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; init; }
}
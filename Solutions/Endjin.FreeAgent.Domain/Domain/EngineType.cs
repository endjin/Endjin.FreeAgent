// <copyright file="EngineType.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the engine type for a vehicle used in mileage expenses.
/// </summary>
/// <remarks>
/// <para>
/// This is required for car mileage expenses. The engine type affects the mileage rate applied.
/// </para>
/// </remarks>
public enum EngineType
{
    /// <summary>
    /// A petrol/gasoline engine (default).
    /// </summary>
    Petrol,

    /// <summary>
    /// A diesel engine.
    /// </summary>
    Diesel,

    /// <summary>
    /// A liquefied petroleum gas engine.
    /// </summary>
    Lpg,

    /// <summary>
    /// An electric vehicle (general).
    /// </summary>
    Electric,

    /// <summary>
    /// An electric vehicle charged at a home charger.
    /// </summary>
    ElectricHomeCharger,

    /// <summary>
    /// An electric vehicle charged at a public charger.
    /// </summary>
    ElectricPublicCharger
}

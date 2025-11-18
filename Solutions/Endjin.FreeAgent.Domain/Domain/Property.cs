// <copyright file="Property.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a property in the FreeAgent accounting system for UK unincorporated landlords.
/// </summary>
/// <remarks>
/// <para>
/// Properties are rental or investment properties that can be managed within FreeAgent
/// for UK unincorporated landlords. Each property has an address that can be associated
/// with income and expenses for property management purposes.
/// </para>
/// <para>
/// API Endpoint: /v2/properties
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users.
/// </para>
/// <para>
/// Note: This endpoint is only available for companies of type <c>UkUnincorporatedLandlord</c>.
/// Other company types cannot have or create properties.
/// </para>
/// </remarks>
/// <seealso cref="PropertyRoot"/>
[DebuggerDisplay("Address1 = {" + nameof(Address1) + "}")]
public record Property
{
    /// <summary>
    /// Gets the unique URI identifier for this property.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this property in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the property in other resources.
    /// </value>
    [JsonPropertyName("url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the first line of the property's address.
    /// </summary>
    /// <value>
    /// The primary address line, typically containing the building number and street name.
    /// This field is required when creating a property.
    /// </value>
    [JsonPropertyName("address1")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address1 { get; init; }

    /// <summary>
    /// Gets the second line of the property's address.
    /// </summary>
    /// <value>
    /// Additional address information such as suite or apartment number.
    /// </value>
    [JsonPropertyName("address2")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address2 { get; init; }

    /// <summary>
    /// Gets the third line of the property's address.
    /// </summary>
    /// <value>
    /// Additional address information if needed for complex addresses.
    /// </value>
    [JsonPropertyName("address3")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address3 { get; init; }

    /// <summary>
    /// Gets the town or city of the property's address.
    /// </summary>
    /// <value>
    /// The town or city name.
    /// </value>
    [JsonPropertyName("town")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Town { get; init; }

    /// <summary>
    /// Gets the region, state, or county of the property's address.
    /// </summary>
    /// <value>
    /// The region, state, province, or county name.
    /// </value>
    [JsonPropertyName("region")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Region { get; init; }

    /// <summary>
    /// Gets the postal code or ZIP code of the property's address.
    /// </summary>
    /// <value>
    /// The postcode or ZIP code.
    /// </value>
    [JsonPropertyName("postcode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Postcode { get; init; }

    /// <summary>
    /// Gets the country of the property's address.
    /// </summary>
    /// <value>
    /// The country name. This field defaults to "United Kingdom" and cannot be overridden.
    /// </value>
    /// <remarks>
    /// The country is automatically set to "United Kingdom" by the FreeAgent API
    /// and any value provided during creation or update will be ignored.
    /// </remarks>
    [JsonPropertyName("country")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Country { get; init; }
}

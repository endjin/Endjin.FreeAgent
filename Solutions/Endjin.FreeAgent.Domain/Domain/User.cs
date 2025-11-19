// <copyright file="User.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Converters;

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents a user in the FreeAgent accounting system.
/// </summary>
/// <remarks>
/// <para>
/// Users are individuals with access to a FreeAgent company account. Each user has a role (such as Owner, Director,
/// Employee, or Accountant) and a permission level (0-8) that determines their access to features and data.
/// </para>
/// <para>
/// Users can track time, submit expenses, and access various features based on their permission level.
/// The permission levels range from 0 (No Access) to 8 (Full Access), with intermediate levels granting
/// access to specific functional areas like Time (1), My Money (2), Banking (6), etc.
/// </para>
/// <para>
/// API Endpoint: /v2/users
/// </para>
/// <para>
/// Minimum Access Level: Tax, Accounting &amp; Users (level 7) for most operations; Time (level 1) for updating personal profile.
/// </para>
/// </remarks>
/// <seealso cref="Role"/>
[DebuggerDisplay("Name = {FullName}")]
public record User
{
    /// <summary>
    /// Gets the unique URI identifier for this user.
    /// </summary>
    /// <value>
    /// A URI that uniquely identifies this user in the FreeAgent system.
    /// This value is assigned by the API upon creation and is used to reference the user in other resources.
    /// </value>
    [JsonPropertyName("url")]
    public Uri? Url { get; init; }

    /// <summary>
    /// Gets the user's first name.
    /// </summary>
    /// <value>
    /// The first name of the user.
    /// </value>
    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets the user's last name.
    /// </summary>
    /// <value>
    /// The last name of the user.
    /// </value>
    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    /// <summary>
    /// Gets the user's email address.
    /// </summary>
    /// <value>
    /// The email address used for login and communications. This must be unique within the company.
    /// </value>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Gets the user's organizational role.
    /// </summary>
    /// <value>
    /// A <see cref="Domain.Role"/> value indicating the user's position in the organization.
    /// Available roles include Owner, Director, Partner, Company Secretary, Employee, Shareholder, and Accountant.
    /// </value>
    /// <seealso cref="Domain.Role"/>
    [JsonPropertyName("role")]
    [JsonConverter(typeof(RoleJsonConverter))]
    public Role? Role { get; init; }

    /// <summary>
    /// Gets a value indicating whether this user is hidden from the active user list.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the user is hidden (archived); otherwise, <see langword="false"/>.
    /// Hidden users are removed from active lists but their historical data remains accessible.
    /// </value>
    /// <remarks>
    /// This property is not officially documented in the FreeAgent API documentation but is returned by the API
    /// and referenced in the view filter descriptions (e.g., "active_staff" filters for non-hidden staff users).
    /// </remarks>
    [JsonPropertyName("hidden")]
    public bool? Hidden { get; init; }

    /// <summary>
    /// Gets the user's permission level, determining their access to features and data.
    /// </summary>
    /// <value>
    /// An integer from 0 to 8 representing the access level:
    /// 0 = No Access, 1 = Time, 2 = My Money, 3 = Contacts &amp; Projects, 4 = Invoices/Estimates/Files,
    /// 5 = Bills, 6 = Banking, 7 = Tax/Accounting/Users, 8 = Full Access.
    /// </value>
    [JsonPropertyName("permission_level")]
    public int? PermissionLevel { get; init; }

    /// <summary>
    /// Gets the opening mileage value for this user as of the company start date.
    /// </summary>
    /// <value>
    /// The mileage value as of the company start date, used as a baseline for mileage expense calculations.
    /// </value>
    [JsonPropertyName("opening_mileage")]
    public decimal? OpeningMileage { get; init; }

    /// <summary>
    /// Gets the date and time when this user record was last updated.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the last modification timestamp in UTC.
    /// </value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }

    /// <summary>
    /// Gets the date and time when this user record was created.
    /// </summary>
    /// <value>
    /// A <see cref="DateTimeOffset"/> representing the creation timestamp in UTC.
    /// </value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Gets the user's UK National Insurance Number.
    /// </summary>
    /// <value>
    /// The National Insurance Number for UK-based users, used for tax and payroll purposes.
    /// </value>
    [JsonPropertyName("ni_number")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? NiNumber { get; init; }

    /// <summary>
    /// Gets the user's Unique Tax Reference (UTR).
    /// </summary>
    /// <value>
    /// A 10-digit UK tax reference number used for self-assessment.
    /// </value>
    [JsonPropertyName("unique_tax_reference")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? UniqueTaxReference { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether to send an invitation email to the user.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to send a password setup invitation; otherwise, <see langword="false"/>.
    /// This is a write-only property used when creating users.
    /// </value>
    [JsonPropertyName("send_invitation")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? SendInvitation { get; init; }

    /// <summary>
    /// Gets the current payroll profile for this user.
    /// </summary>
    /// <value>
    /// The payroll profile data when payroll is active. This property is read-only.
    /// </value>
    [JsonPropertyName("current_payroll_profile")]
    public UserPayrollProfile? CurrentPayrollProfile { get; init; }

    /// <summary>
    /// Gets the full name of the user by combining first and last names.
    /// </summary>
    /// <value>
    /// A concatenation of <see cref="FirstName"/> and <see cref="LastName"/> separated by a space.
    /// This property is not serialized to JSON.
    /// </value>
    [JsonIgnore]
    public string FullName => $"{FirstName} {LastName}";
}
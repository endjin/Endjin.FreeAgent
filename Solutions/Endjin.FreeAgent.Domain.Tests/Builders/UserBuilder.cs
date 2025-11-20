// <copyright file="UserBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Tests.Builders;

using System;

public class UserBuilder
{
    private Uri? url = new("https://api.freeagent.com/v2/users/1");
    private string? firstName = "Jane";
    private string? lastName = "Smith";
    private string? email = "jane.smith@example.com";
    private Role role = Role.Employee;
    private bool hidden = false;
    private int permissionLevel = 5;
    private decimal? openingMileage = 0;
    private DateTimeOffset updatedAt = new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
    private DateTimeOffset createdAt = new(2024, 3, 17, 0, 0, 0, TimeSpan.Zero);
    private string? niNumber;
    private string? uniqueTaxReference;
    private bool? sendInvitation;
    private UserPayrollProfile? currentPayrollProfile;

    public UserBuilder WithUrl(Uri? url)
    {
        this.url = url;
        return this;
    }

    public UserBuilder WithName(string? firstName, string? lastName)
    {
        this.firstName = firstName;
        this.lastName = lastName;
        return this;
    }

    public UserBuilder WithEmail(string? email)
    {
        this.email = email;
        return this;
    }

    public UserBuilder WithRole(Role role)
    {
        this.role = role;
        return this;
    }

    public UserBuilder AsOwner()
    {
        this.role = Role.Owner;
        this.permissionLevel = 8; // Full Access (valid range is 0-8)
        return this;
    }

    public UserBuilder AsDirector()
    {
        this.role = Role.Director;
        this.permissionLevel = 8;
        return this;
    }

    public UserBuilder AsEmployee()
    {
        this.role = Role.Employee;
        this.permissionLevel = 5;
        return this;
    }

    public UserBuilder AsHidden()
    {
        this.hidden = true;
        return this;
    }

    public UserBuilder WithPermissionLevel(int level)
    {
        this.permissionLevel = level;
        return this;
    }

    public UserBuilder WithNiNumber(string? niNumber)
    {
        this.niNumber = niNumber;
        return this;
    }

    public UserBuilder WithUniqueTaxReference(string? uniqueTaxReference)
    {
        this.uniqueTaxReference = uniqueTaxReference;
        return this;
    }

    public UserBuilder WithSendInvitation(bool? sendInvitation)
    {
        this.sendInvitation = sendInvitation;
        return this;
    }

    public UserBuilder WithCurrentPayrollProfile(UserPayrollProfile? currentPayrollProfile)
    {
        this.currentPayrollProfile = currentPayrollProfile;
        return this;
    }

    public User Build() => new()
    {
        Url = url,
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        Role = role,
        Hidden = hidden,
        PermissionLevel = permissionLevel,
        OpeningMileage = openingMileage,
        UpdatedAt = updatedAt,
        CreatedAt = createdAt,
        NiNumber = niNumber,
        UniqueTaxReference = uniqueTaxReference,
        SendInvitation = sendInvitation,
        CurrentPayrollProfile = currentPayrollProfile
    };

    public static implicit operator User(UserBuilder builder) => builder.Build();
}

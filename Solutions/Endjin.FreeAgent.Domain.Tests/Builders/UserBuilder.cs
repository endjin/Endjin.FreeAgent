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
    private long permissionLevel = 5;
    private string? openingMileage = "0";
    private DateTimeOffset updatedAt = new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
    private DateTimeOffset createdAt = new(2024, 3, 17, 0, 0, 0, TimeSpan.Zero);
    private string? niNumber;

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
        this.permissionLevel = 10;
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

    public UserBuilder WithPermissionLevel(long level)
    {
        this.permissionLevel = level;
        return this;
    }

    public UserBuilder WithNiNumber(string? niNumber)
    {
        this.niNumber = niNumber;
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
        NiNumber = niNumber
    };

    public static implicit operator User(UserBuilder builder) => builder.Build();
}

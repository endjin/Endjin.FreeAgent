// <copyright file="UserTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Tests.Builders;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void User_FullName_CombinesFirstAndLastName()
    {
        // Arrange
        User user = new UserBuilder()
            .WithName("John", "Smith");

        // Act
        string fullName = user.FullName;

        // Assert
        fullName.ShouldBe("John Smith");
    }

    [TestMethod]
    public void User_FullName_HandlesNullNames()
    {
        // Arrange
        User user = new()
        {
            FirstName = null,
            LastName = null
        };

        // Act
        string fullName = user.FullName;

        // Assert
        fullName.ShouldBe(" ");
    }

    [TestMethod]
    public void User_AsRecord_SupportsValueEquality()
    {
        // Arrange
        User user1 = new UserBuilder()
            .WithEmail("test@example.com")
            .AsEmployee();

        User user2 = new UserBuilder()
            .WithEmail("test@example.com")
            .AsEmployee();

        // Act & Assert
        user1.ShouldBe(user2);
        (user1 == user2).ShouldBeTrue();
    }

    [TestMethod]
    public void User_WithDifferentRoles_HaveDifferentPermissionLevels()
    {
        // Arrange & Act
        User owner = new UserBuilder().AsOwner();
        User director = new UserBuilder().AsDirector();
        User employee = new UserBuilder().AsEmployee();

        // Assert
        owner.Role.ShouldBe(Role.Owner);
        owner.PermissionLevel.ShouldBe(8); // Full Access (valid range is 0-8)

        director.Role.ShouldBe(Role.Director);
        director.PermissionLevel.ShouldBe(8);

        employee.Role.ShouldBe(Role.Employee);
        employee.PermissionLevel.ShouldBe(5);
    }

    [TestMethod]
    public void User_AsHidden_SetsHiddenFlag()
    {
        // Arrange & Act
        User user = new UserBuilder()
            .AsHidden();

        // Assert
        user.Hidden.ShouldBe(true);
    }

    [TestMethod]
    public void User_WithNiNumber_StoresNationalInsuranceNumber()
    {
        // Arrange & Act
        User user = new UserBuilder()
            .WithNiNumber("AB123456C");

        // Assert
        user.NiNumber.ShouldBe("AB123456C");
    }

    [TestMethod]
    public void User_UsingWithExpression_CreatesNewInstanceWithUpdatedValues()
    {
        // Arrange
        User original = new UserBuilder()
            .WithEmail("original@example.com")
            .AsEmployee();

        // Act
        User updated = original with
        {
            Email = "updated@example.com",
            Role = Role.Director
        };

        // Assert
        updated.Email.ShouldBe("updated@example.com");
        updated.Role.ShouldBe(Role.Director);
        updated.ShouldNotBeSameAs(original);
        original.Email.ShouldBe("original@example.com");
        original.Role.ShouldBe(Role.Employee);
    }

    [TestMethod]
    public void User_WithOpeningMileage_StoresInitialMileage()
    {
        // Arrange & Act
        User user = new()
        {
            OpeningMileage = 12500.5m
        };

        // Assert
        user.OpeningMileage.ShouldBe(12500.5m);
    }

    [TestMethod]
    public void User_WithTimestamps_TracksCreationAndUpdates()
    {
        // Arrange
        DateTimeOffset createdDate = DateTimeOffset.UtcNow.AddDays(-90);
        DateTimeOffset updatedDate = DateTimeOffset.UtcNow;

        // Act
        User user = new()
        {
            CreatedAt = createdDate,
            UpdatedAt = updatedDate
        };

        // Assert
        user.CreatedAt.ShouldBe(createdDate);
        user.UpdatedAt.ShouldBe(updatedDate);
        (user.UpdatedAt > user.CreatedAt).ShouldBeTrue();
    }

    [TestMethod]
    public void User_AllRoleValues_AreValid()
    {
        // Arrange & Act & Assert
        foreach (Role role in Enum.GetValues<Role>())
        {
            User user = new() { Role = role };
            user.Role.ShouldBeOfType<Role>();
            Enum.IsDefined(typeof(Role), user.Role).ShouldBeTrue();
        }
    }

    [TestMethod]
    public void User_WithUrl_StoresApiEndpoint()
    {
        // Arrange
        Uri userUrl = new("https://api.freeagent.com/v2/users/42");

        // Act
        User user = new UserBuilder()
            .WithUrl(userUrl);

        // Assert
        user.Url.ShouldBe(userUrl);
        user.Url?.ToString().ShouldContain("/users/42");
    }

    [TestMethod]
    public void User_RecordToString_GeneratesCorrectOutput()
    {
        // Arrange
        User user = new UserBuilder()
            .WithName("Jane", "Doe")
            .WithEmail("jane@example.com");

        // Act
        string result = user.ToString();

        // Assert
        result.ShouldContain("User");
        result.ShouldContain("FirstName = Jane");
        result.ShouldContain("LastName = Doe");
        result.ShouldContain("Email = jane@example.com");
    }

    [TestMethod]
    public void User_WithUniqueTaxReference_StoresTenDigitUtr()
    {
        // Arrange & Act
        User user = new()
        {
            UniqueTaxReference = "1234567890"
        };

        // Assert
        user.UniqueTaxReference.ShouldBe("1234567890");
    }

    [TestMethod]
    public void User_WithSendInvitation_StoresInvitationFlag()
    {
        // Arrange & Act
        User user = new()
        {
            SendInvitation = true
        };

        // Assert
        user.SendInvitation.ShouldBe(true);
    }

    [TestMethod]
    public void User_WithCurrentPayrollProfile_StoresPayrollData()
    {
        // Arrange
        UserPayrollProfile payrollProfile = new()
        {
            TotalPayInPreviousEmployment = 25000.00m,
            TotalTaxInPreviousEmployment = 5000.00m
        };

        // Act
        User user = new()
        {
            CurrentPayrollProfile = payrollProfile
        };

        // Assert
        user.CurrentPayrollProfile.ShouldNotBeNull();
        user.CurrentPayrollProfile.TotalPayInPreviousEmployment.ShouldBe(25000.00m);
        user.CurrentPayrollProfile.TotalTaxInPreviousEmployment.ShouldBe(5000.00m);
    }
}

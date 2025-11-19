// <copyright file="TimeslipTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Tests.Builders;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class TimeslipTests
{
    [TestMethod]
    public void Timeslip_AsRecord_SupportsValueEquality()
    {
        // Arrange
        Timeslip timeslip1 = new TimeslipBuilder()
            .WithHours(8m)
            .WithComment("Development work");

        Timeslip timeslip2 = new TimeslipBuilder()
            .WithHours(8m)
            .WithComment("Development work");

        // Act & Assert
        timeslip1.ShouldBe(timeslip2);
        (timeslip1 == timeslip2).ShouldBeTrue();
    }

    [TestMethod]
    public void Timeslip_WithRelatedEntries_MaintainsRelationships()
    {
        // Arrange
        User user = new UserBuilder().WithName("John", "Doe");
        Project project = new ProjectBuilder().WithName("Test Project");
        TaskItem task = new() { Name = "Development", IsBillable = true, Status = "Active" };

        // Act
        Timeslip timeslip = new TimeslipBuilder()
            .WithUser(user)
            .WithProject(project)
            .WithTask(task);

        // Assert
        timeslip.UserEntry.ShouldBe(user);
        timeslip.ProjectEntry.ShouldBe(project);
        timeslip.TaskEntry.ShouldBe(task);
        timeslip.User.ShouldBe(user.Url);
        timeslip.Project.ShouldBe(project.Url);
        timeslip.Task.ShouldBe(task.Url);
    }

    [TestMethod]
    public void Timeslip_WithHours_StoresDecimalHours()
    {
        // Arrange & Act
        Timeslip timeslip = new TimeslipBuilder()
            .WithHours(7.5m);

        // Assert
        timeslip.Hours.ShouldBe(7.5m);
    }

    [TestMethod]
    public void Timeslip_ForToday_SetsCurrentDate()
    {
        // Arrange & Act
        Timeslip timeslip = new TimeslipBuilder()
            .ForToday();

        // Assert
        timeslip.DatedOn?.Date.ShouldBe(new DateTime(2024, 6, 15));
    }

    [TestMethod]
    public void Timeslip_ForYesterday_SetsPreviousDate()
    {
        // Arrange & Act
        Timeslip timeslip = new TimeslipBuilder()
            .ForYesterday();

        // Assert
        timeslip.DatedOn?.Date.ShouldBe(new DateTime(2024, 6, 14));
    }

    [TestMethod]
    public void Timeslip_WithComment_StoresDescription()
    {
        // Arrange & Act
        Timeslip timeslip = new TimeslipBuilder()
            .WithComment("Implemented user authentication feature");

        // Assert
        timeslip.Comment.ShouldBe("Implemented user authentication feature");
    }

    [TestMethod]
    public void Timeslip_UsingWithExpression_CreatesNewInstanceWithUpdatedValues()
    {
        // Arrange
        Timeslip original = new TimeslipBuilder()
            .WithHours(4m)
            .WithComment("Original work");

        // Act
        Timeslip updated = original with
        {
            Hours = 8m,
            Comment = "Updated work"
        };

        // Assert
        updated.Hours.ShouldBe(8m);
        updated.Comment.ShouldBe("Updated work");
        updated.ShouldNotBeSameAs(original);
        original.Hours.ShouldBe(4m);
        original.Comment.ShouldBe("Original work");
    }

    [TestMethod]
    public void Timeslip_WithSpecificDate_StoresExactDate()
    {
        // Arrange
        DateTimeOffset specificDate = new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);

        // Act
        Timeslip timeslip = new TimeslipBuilder()
            .OnDate(specificDate);

        // Assert
        timeslip.DatedOn.ShouldBe(specificDate);
    }

    [TestMethod]
    public void Timeslip_WithNullComment_HandlesNullCorrectly()
    {
        // Arrange & Act
        Timeslip timeslip = new()
        {
            Hours = 8m,
            DatedOn = DateTimeOffset.UtcNow,
            Comment = null,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Assert
        timeslip.Comment.ShouldBeNull();
        timeslip.Hours.ShouldBe(8m);
    }

    [TestMethod]
    public void Timeslip_WithZeroHours_AllowsZeroTimeEntry()
    {
        // Arrange & Act
        Timeslip timeslip = new TimeslipBuilder()
            .WithHours(0m);

        // Assert
        timeslip.Hours.ShouldBe(0m);
    }

    [TestMethod]
    public void Timeslip_WithFractionalHours_StoresPreciseTime()
    {
        // Arrange & Act
        Timeslip timeslip = new TimeslipBuilder()
            .WithHours(2.25m); // 2 hours 15 minutes

        // Assert
        timeslip.Hours.ShouldBe(2.25m);
    }

    [TestMethod]
    public void Timeslip_WithTimestamps_TracksCreationAndUpdates()
    {
        // Arrange
        DateTimeOffset createdDate = DateTimeOffset.UtcNow.AddHours(-2);
        DateTimeOffset updatedDate = DateTimeOffset.UtcNow;

        // Act
        Timeslip timeslip = new()
        {
            CreatedAt = createdDate,
            UpdatedAt = updatedDate,
            DatedOn = DateTimeOffset.UtcNow,
            Hours = 8m
        };

        // Assert
        timeslip.CreatedAt.ShouldBe(createdDate);
        timeslip.UpdatedAt.ShouldBe(updatedDate);
        (timeslip.UpdatedAt > timeslip.CreatedAt).ShouldBeTrue();
    }

    [TestMethod]
    public void Timeslip_RecordToString_GeneratesCorrectOutput()
    {
        // Arrange
        Timeslip timeslip = new TimeslipBuilder()
            .WithHours(8m)
            .WithComment("Testing work");

        // Act
        string result = timeslip.ToString();

        // Assert
        result.ShouldContain("Timeslip");
        result.ShouldContain("Hours = 8");
        result.ShouldContain("Comment = Testing work");
    }
}

// <copyright file="ProjectTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Tests.Builders;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class ProjectTests
{
    [TestMethod]
    public void Project_AsRecord_SupportsValueEquality()
    {
        // Arrange
        Project project1 = new ProjectBuilder()
            .WithName("Test Project")
            .WithBudget(10000m);

        Project project2 = new ProjectBuilder()
            .WithName("Test Project")
            .WithBudget(10000m);

        // Act & Assert
        project1.ShouldBe(project2);
        (project1 == project2).ShouldBeTrue();
    }

    [TestMethod]
    public void Project_WithTimeslips_MaintainsImmutableCollection()
    {
        // Arrange
        Timeslip timeslip1 = new TimeslipBuilder().WithHours(4);
        Timeslip timeslip2 = new TimeslipBuilder().WithHours(8);

        // Act
        Project project = new ProjectBuilder()
            .WithTimeslips(timeslip1, timeslip2);

        // Assert
        project.TimeslipEntries.Count.ShouldBe(2);
        project.TimeslipEntries[0].Hours.ShouldBe(4);
        project.TimeslipEntries[1].Hours.ShouldBe(8);
        project.TimeslipEntries.ShouldBeOfType<ImmutableList<Timeslip>>();
    }

    [TestMethod]
    public void Project_WithContactEntry_MaintainsRelationship()
    {
        // Arrange
        Contact contact = new ContactBuilder()
            .WithOrganisationName("Client Corp");

        // Act
        Project project = new ProjectBuilder()
            .WithContact(contact);

        // Assert
        project.ContactEntry.ShouldBe(contact);
        project.Contact.ShouldBe(contact.Url);
        project.ContactEntry?.OrganisationName.ShouldBe("Client Corp");
    }

    [TestMethod]
    public void Project_AsEstimate_ConfiguresEstimateProperties()
    {
        // Arrange & Act
        Project project = new ProjectBuilder()
            .AsEstimate()
            .WithStatus("Draft");

        // Assert
        project.IsEstimate.ShouldBe(true);
        project.Status.ShouldBe("Draft");
    }

    [TestMethod]
    public void Project_WithBillingConfiguration_SetsBillingCorrectly()
    {
        // Arrange & Act
        Project project = new ProjectBuilder()
            .WithBillingRate(150m, "hour")
            .WithBudget(5000m, "Hours");

        // Assert
        project.NormalBillingRate.ShouldBe(150m);
        project.BillingPeriod.ShouldBe("hour");
        project.Budget.ShouldBe(5000m);
        project.BudgetUnits.ShouldBe("Hours");
    }

    [TestMethod]
    public void Project_WithDateRange_TracksProjectTimeline()
    {
        // Arrange
        DateOnly startDate = new(2024, 1, 1);
        DateOnly endDate = new(2024, 7, 1);

        // Act
        Project project = new ProjectBuilder()
            .WithDates(startDate, endDate);

        // Assert
        project.StartsOn.ShouldBe(startDate);
        project.EndsOn.ShouldBe(endDate);
        (project.EndsOn > project.StartsOn).ShouldBeTrue();
    }

    [TestMethod]
    public void Project_AsCompleted_SetsCompletedStatus()
    {
        // Arrange & Act
        Project project = new ProjectBuilder()
            .AsCompleted();

        // Assert
        project.Status.ShouldBe("Completed");
        project.EndsOn.ShouldNotBeNull();
        project.EndsOn.ShouldBe(new DateOnly(2024, 5, 31));
    }

    [TestMethod]
    public void Project_WithIr35Status_ConfiguresIr35()
    {
        // Arrange & Act
        Project project = new ProjectBuilder()
            .AsIr35();

        // Assert
        project.IsIr35.ShouldBe(true);
    }

    [TestMethod]
    public void Project_UsingWithExpression_CreatesNewInstanceWithUpdatedValues()
    {
        // Arrange
        Project original = new ProjectBuilder()
            .WithName("Original Project")
            .WithBudget(5000m);

        // Act
        Project updated = original with
        {
            Name = "Updated Project",
            Budget = 10000m
        };

        // Assert
        updated.Name.ShouldBe("Updated Project");
        updated.Budget.ShouldBe(10000m);
        updated.ShouldNotBeSameAs(original);
        original.Name.ShouldBe("Original Project");
        original.Budget.ShouldBe(5000m);
    }

    [TestMethod]
    public void Project_WithEmptyTimeslips_HasEmptyImmutableList()
    {
        // Arrange & Act
        Project project = new ProjectBuilder().Build();

        // Assert
        project.TimeslipEntries.ShouldNotBeNull();
        project.TimeslipEntries.Count.ShouldBe(0);
        project.TimeslipEntries.ShouldBe([]);
    }

    [TestMethod]
    public void Project_WithCurrency_StoresCurrencyCode()
    {
        // Arrange & Act
        Project project = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
            Name = "Test Project",
            Status = "Active",
            BudgetUnits = "Hours",
            Budget = 0m,
            UsesProjectInvoiceSequence = false,
            Currency = "USD"
        };

        // Assert
        project.Currency.ShouldBe("USD");
    }

    [TestMethod]
    public void Project_WithContractReference_StoresPoReference()
    {
        // Arrange & Act
        Project project = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
            Name = "Test Project",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 0m,
            UsesProjectInvoiceSequence = false,
            ContractPoReference = "PO-2024-001"
        };

        // Assert
        project.ContractPoReference.ShouldBe("PO-2024-001");
    }

    [TestMethod]
    public void Project_WithHoursPerDay_ConfiguresWorkingHours()
    {
        // Arrange & Act
        Project project = new()
        {
            Contact = new Uri("https://api.freeagent.com/v2/contacts/1"),
            Name = "Test Project",
            Status = "Active",
            Currency = "GBP",
            BudgetUnits = "Hours",
            Budget = 0m,
            UsesProjectInvoiceSequence = false,
            HoursPerDay = 7.5m
        };

        // Assert
        project.HoursPerDay.ShouldBe(7.5m);
    }

    [TestMethod]
    public void Project_TotalTimeslipHours_CalculatesCorrectly()
    {
        // Arrange
        Timeslip[] timeslips =
        [
            new TimeslipBuilder().WithHours(4).Build(),
            new TimeslipBuilder().WithHours(8).Build(),
            new TimeslipBuilder().WithHours(6).Build()
        ];

        // Act
        Project project = new ProjectBuilder()
            .WithTimeslips(timeslips);

        decimal totalHours = project.TimeslipEntries.Sum(t => t.Hours ?? 0m);

        // Assert
        totalHours.ShouldBe(18m);
    }
}

// <copyright file="JournalSetTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class JournalSetTests
{
    [TestMethod]
    public void JournalSet_AsRecord_SupportsValueEquality()
    {
        // Arrange
        JournalEntry entry = new()
        {
            Category = "Sales",
            DebitValue = "100.00",
            Description = "Test entry"
        };

        ImmutableList<JournalEntry> sharedList = [entry];

        JournalSet set1 = new()
        {
            DatedOn = "2024-06-15",
            Description = "Test set",
            JournalEntries = sharedList
        };

        JournalSet set2 = new()
        {
            DatedOn = "2024-06-15",
            Description = "Test set",
            JournalEntries = sharedList
        };

        // Act & Assert
        set1.ShouldBe(set2);
        (set1 == set2).ShouldBeTrue();

        // Test structural equality for journal entries
        set1.JournalEntries.SequenceEqual(set2.JournalEntries).ShouldBeTrue();
    }

    [TestMethod]
    public void JournalEntry_AsRecord_SupportsValueEquality()
    {
        // Arrange
        JournalEntry entry1 = new()
        {
            Category = "Sales",
            DebitValue = "100.00",
            Description = "Sales entry"
        };

        JournalEntry entry2 = new()
        {
            Category = "Sales",
            DebitValue = "100.00",
            Description = "Sales entry"
        };

        // Act & Assert
        entry1.ShouldBe(entry2);
        (entry1 == entry2).ShouldBeTrue();
    }

    [TestMethod]
    public void JournalEntry_WithOptionalFields_HandlesNullsCorrectly()
    {
        // Arrange & Act
        JournalEntry entry = new()
        {
            Category = "Sales",
            DebitValue = "100.00",
            Description = "Test",
            Url = null,
            User = null,
            DisplayText = null
        };

        // Assert
        entry.Category.ShouldBe("Sales");
        entry.DebitValue.ShouldBe("100.00");
        entry.Description.ShouldBe("Test");
        entry.Url.ShouldBeNull();
        entry.User.ShouldBeNull();
        entry.DisplayText.ShouldBeNull();
    }

    [TestMethod]
    public void JournalSet_WithTag_StoresTagCorrectly()
    {
        // Arrange & Act
        JournalSet journalSet = new()
        {
            DatedOn = "2024-06-15",
            Description = "Tagged set",
            Tag = "MYAPP-001"
        };

        // Assert
        journalSet.Tag.ShouldBe("MYAPP-001");
    }

    [TestMethod]
    public void JournalEntry_NegativeDebitValue_ParsesCorrectly()
    {
        // Arrange
        JournalEntry entry = new()
        {
            Category = "Expenses",
            DebitValue = "-123.45",
            Description = "Expense reversal"
        };

        // Act
        decimal value = decimal.Parse(entry.DebitValue);

        // Assert
        value.ShouldBe(-123.45m);
    }

    [TestMethod]
    public void JournalSet_RequiredProperties_EnforcedByRecord()
    {
        // Arrange & Act
        JournalSet journalSet = new()
        {
            DatedOn = "2024-06-15",
            Description = "Required fields test"
        };

        // Assert
        journalSet.DatedOn.ShouldNotBeNull();
        journalSet.Description.ShouldNotBeNull();
        journalSet.JournalEntries.ShouldNotBeNull();
    }
}

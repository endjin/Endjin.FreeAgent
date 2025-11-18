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
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            DebitValue = 100.00m,
            Description = "Test entry"
        };

        ImmutableList<JournalEntry> sharedList = [entry];

        JournalSet set1 = new()
        {
            DatedOn = new DateOnly(2024, 6, 15),
            Description = "Test set",
            JournalEntries = sharedList
        };

        JournalSet set2 = new()
        {
            DatedOn = new DateOnly(2024, 6, 15),
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
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            DebitValue = 100.00m,
            Description = "Sales entry"
        };

        JournalEntry entry2 = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            DebitValue = 100.00m,
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
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            DebitValue = 100.00m,
            Description = "Test",
            Url = null,
            User = null,
            CapitalAssetType = null,
            StockItem = null,
            StockAlteringQuantity = null,
            BankAccount = null,
            Property = null,
            Destroy = null
        };

        // Assert
        entry.Category.ShouldBe(new Uri("https://api.freeagent.com/v2/categories/001"));
        entry.DebitValue.ShouldBe(100.00m);
        entry.Description.ShouldBe("Test");
        entry.Url.ShouldBeNull();
        entry.User.ShouldBeNull();
        entry.CapitalAssetType.ShouldBeNull();
        entry.StockItem.ShouldBeNull();
        entry.StockAlteringQuantity.ShouldBeNull();
        entry.BankAccount.ShouldBeNull();
        entry.Property.ShouldBeNull();
        entry.Destroy.ShouldBeNull();
    }

    [TestMethod]
    public void JournalSet_WithTag_StoresTagCorrectly()
    {
        // Arrange & Act
        JournalSet journalSet = new()
        {
            DatedOn = new DateOnly(2024, 6, 15),
            Description = "Tagged set",
            Tag = "MYAPP-001"
        };

        // Assert
        journalSet.Tag.ShouldBe("MYAPP-001");
    }

    [TestMethod]
    public void JournalEntry_NegativeDebitValue_RepresentsCredit()
    {
        // Arrange
        JournalEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/250"),
            DebitValue = -123.45m,
            Description = "Expense reversal"
        };

        // Assert - negative debit values represent credits
        entry.DebitValue.ShouldBe(-123.45m);
    }

    [TestMethod]
    public void JournalSet_RequiredProperties_EnforcedByRecord()
    {
        // Arrange & Act
        JournalSet journalSet = new()
        {
            DatedOn = new DateOnly(2024, 6, 15),
            Description = "Required fields test"
        };

        // Assert
        journalSet.DatedOn.ShouldBe(new DateOnly(2024, 6, 15));
        journalSet.Description.ShouldNotBeNull();
        journalSet.JournalEntries.ShouldNotBeNull();
    }

    [TestMethod]
    public void JournalEntry_WithCapitalAssetType_StoresCorrectly()
    {
        // Arrange & Act
        JournalEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/601"),
            DebitValue = 5000.00m,
            Description = "Capital asset purchase",
            CapitalAssetType = new Uri("https://api.freeagent.com/v2/capital_asset_types/1")
        };

        // Assert
        entry.CapitalAssetType.ShouldBe(new Uri("https://api.freeagent.com/v2/capital_asset_types/1"));
    }

    [TestMethod]
    public void JournalEntry_WithStockItem_StoresCorrectly()
    {
        // Arrange & Act
        JournalEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/500"),
            DebitValue = 250.00m,
            Description = "Stock adjustment",
            StockItem = new Uri("https://api.freeagent.com/v2/stock_items/123"),
            StockAlteringQuantity = 10
        };

        // Assert
        entry.StockItem.ShouldBe(new Uri("https://api.freeagent.com/v2/stock_items/123"));
        entry.StockAlteringQuantity.ShouldBe(10);
    }

    [TestMethod]
    public void JournalEntry_WithDestroyFlag_MarksForDeletion()
    {
        // Arrange & Act
        JournalEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            DebitValue = 100.00m,
            Url = new Uri("https://api.freeagent.com/v2/journal_entries/456"),
            Destroy = true
        };

        // Assert
        entry.Destroy.ShouldBe(true);
    }

    [TestMethod]
    public void JournalEntry_WithoutDescription_IsValid()
    {
        // Arrange & Act - Description is optional per API docs
        JournalEntry entry = new()
        {
            Category = new Uri("https://api.freeagent.com/v2/categories/001"),
            DebitValue = 100.00m
        };

        // Assert
        entry.Description.ShouldBeNull();
        entry.Category.ShouldNotBeNull();
    }

    [TestMethod]
    public void JournalSet_WithOpeningBalanceFields_StoresCorrectly()
    {
        // Arrange & Act
        JournalSet journalSet = new()
        {
            DatedOn = new DateOnly(2024, 1, 1),
            Description = "Opening balances",
            BankAccounts = [new Uri("https://api.freeagent.com/v2/bank_accounts/1")],
            StockItems = [new Uri("https://api.freeagent.com/v2/stock_items/1")]
        };

        // Assert
        journalSet.BankAccounts.ShouldNotBeNull();
        journalSet.BankAccounts.Count.ShouldBe(1);
        journalSet.StockItems.ShouldNotBeNull();
        journalSet.StockItems.Count.ShouldBe(1);
    }
}

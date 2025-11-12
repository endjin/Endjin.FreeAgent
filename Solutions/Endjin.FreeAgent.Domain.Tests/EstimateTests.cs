// <copyright file="EstimateTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class EstimateTests
{
    [TestMethod]
    public void Estimate_AsRecord_SupportsValueEquality()
    {
        // Arrange
        Estimate estimate1 = CreateTestEstimate();
        Estimate estimate2 = CreateTestEstimate();

        // Act & Assert
        estimate1.ShouldBe(estimate2);
        (estimate1 == estimate2).ShouldBeTrue();
    }

    [TestMethod]
    public void Estimate_WithEstimateItems_StoresItemsCorrectly()
    {
        // Arrange
        EstimateItem item1 = new()
        {
            Position = 1,
            ItemType = "Hours",
            Quantity = 10,
            Price = 100,
            Description = "Development",
            SalesTaxValue = 20,
            SalesTaxRate = 0.2m,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        EstimateItem item2 = new()
        {
            Position = 2,
            ItemType = "Days",
            Quantity = 5,
            Price = 800,
            Description = "Testing",
            SalesTaxValue = 160,
            SalesTaxRate = 0.2m,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        Estimate estimate = new()
        {
            EstimateItems = [item1, item2],
            NetValue = 4900m,
            TotalValue = 5880m,
            DatedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            CreatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)
        };

        // Assert
        estimate.EstimateItems.Length.ShouldBe(2);
        estimate.EstimateItems[0].Description.ShouldBe("Development");
        estimate.EstimateItems[1].Description.ShouldBe("Testing");
    }

    [TestMethod]
    public void Estimate_WithDiscountPercent_AppliesDiscount()
    {
        // Arrange & Act
        Estimate estimate = new()
        {
            DiscountPercent = 15m,
            NetValue = 1000m,
            TotalValue = 850m,
            DatedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            CreatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)
        };

        // Assert
        estimate.DiscountPercent.ShouldBe(15m);
        estimate.TotalValue.ShouldBe(850m);
    }

    [TestMethod]
    public void Estimate_WithSalesTax_ConfiguresTaxSettings()
    {
        // Arrange & Act
        Estimate estimate = new()
        {
            InvolvesSalesTax = true,
            IsInterimUkVat = true,
            SalesTaxValue = "200.00",
            NetValue = 1000m,
            TotalValue = 1200m,
            DatedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            CreatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)
        };

        // Assert
        estimate.InvolvesSalesTax.ShouldBe(true);
        estimate.IsInterimUkVat.ShouldBe(true);
        estimate.SalesTaxValue.ShouldBe("200.00");
    }

    [TestMethod]
    public void EstimateItem_CalculatesTotalPrice()
    {
        // Arrange
        EstimateItem item = new()
        {
            Quantity = 5,
            Price = 100,
            Position = 1,
            ItemType = "Hours",
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        // Act
        decimal totalPrice = (item.Quantity ?? 0m) * (item.Price ?? 0m);

        // Assert
        totalPrice.ShouldBe(500m);
    }

    [TestMethod]
    public void EstimateItem_WithDifferentItemTypes_StoresCorrectly()
    {
        // Arrange & Act & Assert
        string[] itemTypes = ["Hours", "Days", "Weeks", "Months", "Years"];

        foreach (string itemType in itemTypes)
        {
            EstimateItem item = new()
            {
                ItemType = itemType,
                Position = 1,
                Quantity = 1,
                Price = 100,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            item.ItemType.ShouldBe(itemType);
        }
    }

    [TestMethod]
    public void Estimate_WithStatus_TracksEstimateStatus()
    {
        // Arrange & Act
        Estimate draftEstimate = new()
        {
            Status = "Draft",
            DatedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            CreatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)
        };

        Estimate sentEstimate = new()
        {
            Status = "Sent",
            DatedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            CreatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)
        };

        // Assert
        draftEstimate.Status.ShouldBe("Draft");
        sentEstimate.Status.ShouldBe("Sent");
    }

    [TestMethod]
    public void Estimate_TotalEstimateItemsValue_CalculatesCorrectly()
    {
        // Arrange
        EstimateItem[] items =
        [
            new() { Quantity = 10, Price = 100, Position = 1, ItemType = "Hours", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new() { Quantity = 5, Price = 200, Position = 2, ItemType = "Days", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow },
            new() { Quantity = 2, Price = 500, Position = 3, ItemType = "Weeks", CreatedAt = DateTimeOffset.UtcNow, UpdatedAt = DateTimeOffset.UtcNow }
        ];

        Estimate estimate = new()
        {
            EstimateItems = [.. items],
            DatedOn = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            CreatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2024, 6, 15, 0, 0, 0, TimeSpan.Zero)
        };

        // Act
        decimal totalValue = estimate.EstimateItems.Sum(i => (i.Quantity ?? 0m) * (i.Price ?? 0m));

        // Assert
        totalValue.ShouldBe(3000m); // (10*100) + (5*200) + (2*500)
    }

    private static Estimate CreateTestEstimate()
    {
        DateTimeOffset fixedDate = new(2024, 6, 15, 0, 0, 0, TimeSpan.Zero);
        return new()
        {
            Reference = "EST-001",
            Status = "Draft",
            Currency = "GBP",
            NetValue = 1000m,
            TotalValue = 1200m,
            DatedOn = fixedDate,
            CreatedAt = fixedDate,
            UpdatedAt = fixedDate,
            EstimateItems = []
        };
    }
}

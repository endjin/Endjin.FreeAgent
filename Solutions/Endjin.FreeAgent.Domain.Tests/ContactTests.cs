// <copyright file="ContactTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Tests.Builders;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class ContactTests
{
    [TestMethod]
    public void Contact_AsRecord_SupportsValueEquality()
    {
        // Arrange
        Contact contact1 = new ContactBuilder()
            .WithOrganisationName("Acme Corp")
            .WithEmail("contact@acme.com");

        Contact contact2 = new ContactBuilder()
            .WithOrganisationName("Acme Corp")
            .WithEmail("contact@acme.com");

        // Act & Assert
        contact1.ShouldBe(contact2);
        (contact1 == contact2).ShouldBeTrue();
    }

    [TestMethod]
    public void Contact_WithDifferentValues_AreNotEqual()
    {
        // Arrange
        Contact contact1 = new ContactBuilder()
            .WithOrganisationName("Acme Corp");

        Contact contact2 = new ContactBuilder()
            .WithOrganisationName("Different Corp");

        // Act & Assert
        contact1.ShouldNotBe(contact2);
        (contact1 != contact2).ShouldBeTrue();
    }

    [TestMethod]
    public void Contact_UsingWithExpression_CreatesNewInstanceWithUpdatedValues()
    {
        // Arrange
        Contact original = new ContactBuilder()
            .WithOrganisationName("Original Corp")
            .WithEmail("original@corp.com");

        // Act
        Contact updated = original with
        {
            OrganisationName = "Updated Corp",
            Email = "updated@corp.com"
        };

        // Assert
        updated.OrganisationName.ShouldBe("Updated Corp");
        updated.Email.ShouldBe("updated@corp.com");
        updated.ShouldNotBeSameAs(original);
        original.OrganisationName.ShouldBe("Original Corp");
    }

    [TestMethod]
    public void Contact_WithActiveProjects_ReflectsCorrectCount()
    {
        // Arrange & Act
        Contact contact = new ContactBuilder()
            .WithActiveProjects(5);

        // Assert
        contact.ActiveProjectsCount.ShouldBe("5");
    }

    [TestMethod]
    public void Contact_WithAccountBalance_StoresBalanceCorrectly()
    {
        // Arrange & Act
        Contact contact = new ContactBuilder()
            .WithAccountBalance("1234.56");

        // Assert
        contact.AccountBalance.ShouldBe("1234.56");
    }

    [TestMethod]
    public void Contact_WithNullableProperties_HandlesNullsCorrectly()
    {
        // Arrange & Act
        Contact contact = new()
        {
            Url = null,
            FirstName = null,
            LastName = null,
            OrganisationName = "Company Only",
            Email = null
        };

        // Assert
        contact.Url.ShouldBeNull();
        contact.FirstName.ShouldBeNull();
        contact.LastName.ShouldBeNull();
        contact.OrganisationName.ShouldBe("Company Only");
        contact.Email.ShouldBeNull();
    }

    [TestMethod]
    public void Contact_WithFullAddress_StoresAllAddressComponents()
    {
        // Arrange & Act
        Contact contact = new ContactBuilder()
            .WithAddress("123 Main St", "Springfield", "62701");

        // Assert
        contact.Address1.ShouldBe("123 Main St");
        contact.Town.ShouldBe("Springfield");
        contact.Postcode.ShouldBe("62701");
    }

    [TestMethod]
    public void Contact_WithSalesTaxSettings_ConfiguresTaxCorrectly()
    {
        // Arrange & Act
        Contact contact = new()
        {
            ChargeSalesTax = "Always",
            SalesTaxRegistrationNumber = "GB123456789"
        };

        // Assert
        contact.ChargeSalesTax.ShouldBe("Always");
        contact.SalesTaxRegistrationNumber.ShouldBe("GB123456789");
    }

    [TestMethod]
    public void Contact_WithTimestamps_TracksCreationAndUpdates()
    {
        // Arrange
        DateTimeOffset createdDate = DateTimeOffset.UtcNow.AddDays(-30);
        DateTimeOffset updatedDate = DateTimeOffset.UtcNow;

        // Act
        Contact contact = new()
        {
            CreatedAt = createdDate,
            UpdatedAt = updatedDate
        };

        // Assert
        contact.CreatedAt.ShouldBe(createdDate);
        contact.UpdatedAt.ShouldBe(updatedDate);
        (contact.UpdatedAt > contact.CreatedAt).ShouldBeTrue();
    }

    [TestMethod]
    public void Contact_WithInvoiceSettings_ConfiguresInvoicingCorrectly()
    {
        // Arrange & Act
        Contact contact = new()
        {
            ContactNameOnInvoices = true,
            UsesContactInvoiceSequence = true,
            BillingEmail = "billing@company.com"
        };

        // Assert
        contact.ContactNameOnInvoices.ShouldBe(true);
        contact.UsesContactInvoiceSequence.ShouldBe(true);
        contact.BillingEmail.ShouldBe("billing@company.com");
    }

    [TestMethod]
    public void Contact_RecordToString_GeneratesCorrectOutput()
    {
        // Arrange
        Contact contact = new ContactBuilder()
            .WithOrganisationName("Test Corp")
            .WithFirstName("John")
            .WithLastName("Doe");

        // Act
        string result = contact.ToString();

        // Assert
        result.ShouldContain("Contact");
        result.ShouldContain("OrganisationName = Test Corp");
        result.ShouldContain("FirstName = John");
        result.ShouldContain("LastName = Doe");
    }
}

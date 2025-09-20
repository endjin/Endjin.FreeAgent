// <copyright file="RootClassTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Domain.Tests.Builders;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class RootClassTests
{
    [TestMethod]
    public void ContactRoot_WrapsContactCorrectly()
    {
        // Arrange
        Contact contact = new ContactBuilder()
            .WithOrganisationName("Test Org");

        // Act
        ContactRoot root = new()
        {
            Contact = contact
        };

        // Assert
        root.Contact.ShouldBe(contact);
        root.Contact?.OrganisationName.ShouldBe("Test Org");
    }

    [TestMethod]
    public void ContactsRoot_StoresMultipleContacts()
    {
        // Arrange
        Contact contact1 = new ContactBuilder().WithOrganisationName("Org1");
        Contact contact2 = new ContactBuilder().WithOrganisationName("Org2");

        // Act
        ContactsRoot root = new()
        {
            Contacts = ImmutableList.Create(contact1, contact2)
        };

        // Assert
        root.Contacts.Count.ShouldBe(2);
        root.Contacts[0].OrganisationName.ShouldBe("Org1");
        root.Contacts[1].OrganisationName.ShouldBe("Org2");
    }

    [TestMethod]
    public void ContactsRoot_WithEmptyList_HasEmptyImmutableList()
    {
        // Arrange & Act
        ContactsRoot root = new();

        // Assert
        root.Contacts.ShouldNotBeNull();
        root.Contacts.Count.ShouldBe(0);
        root.Contacts.ShouldBe([]);
    }

    [TestMethod]
    public void ProjectRoot_WrapsProjectCorrectly()
    {
        // Arrange
        Project project = new ProjectBuilder()
            .WithName("Test Project");

        // Act
        ProjectRoot root = new()
        {
            Project = project
        };

        // Assert
        root.Project.ShouldBe(project);
        root.Project?.Name.ShouldBe("Test Project");
    }

    [TestMethod]
    public void ProjectsRoot_StoresMultipleProjects()
    {
        // Arrange
        Project project1 = new ProjectBuilder().WithName("Project1");
        Project project2 = new ProjectBuilder().WithName("Project2");

        // Act
        ProjectsRoot root = new()
        {
            Projects = ImmutableList.Create(project1, project2)
        };

        // Assert
        root.Projects.Count.ShouldBe(2);
        root.Projects[0].Name.ShouldBe("Project1");
        root.Projects[1].Name.ShouldBe("Project2");
    }

    [TestMethod]
    public void UserRoot_WrapsUserCorrectly()
    {
        // Arrange
        User user = new UserBuilder()
            .WithEmail("test@example.com");

        // Act
        UserRoot root = new()
        {
            User = user
        };

        // Assert
        root.User.ShouldBe(user);
        root.User?.Email.ShouldBe("test@example.com");
    }

    [TestMethod]
    public void UsersRoot_StoresMultipleUsers()
    {
        // Arrange
        User user1 = new UserBuilder().WithEmail("user1@example.com");
        User user2 = new UserBuilder().WithEmail("user2@example.com");

        // Act
        UsersRoot root = new()
        {
            Users = ImmutableList.Create(user1, user2)
        };

        // Assert
        root.Users.Count.ShouldBe(2);
        root.Users[0].Email.ShouldBe("user1@example.com");
        root.Users[1].Email.ShouldBe("user2@example.com");
    }

    [TestMethod]
    public void TimeslipsRoot_StoresMultipleTimeslips()
    {
        // Arrange
        Timeslip timeslip1 = new TimeslipBuilder().WithHours(4);
        Timeslip timeslip2 = new TimeslipBuilder().WithHours(8);

        // Act
        TimeslipsRoot root = new()
        {
            Timeslips = ImmutableList.Create(timeslip1, timeslip2)
        };

        // Assert
        root.Timeslips.Count.ShouldBe(2);
        root.Timeslips[0].Hours.ShouldBe(4);
        root.Timeslips[1].Hours.ShouldBe(8);
    }

    [TestMethod]
    public void RootClasses_AsRecords_SupportValueEquality()
    {
        // Arrange
        Contact contact = new ContactBuilder().WithOrganisationName("Test");

        ContactRoot root1 = new() { Contact = contact };
        ContactRoot root2 = new() { Contact = contact };

        // Act & Assert
        root1.ShouldBe(root2);
        (root1 == root2).ShouldBeTrue();
    }

    [TestMethod]
    public void RootClasses_UsingWithExpression_CreatesNewInstance()
    {
        // Arrange
        Contact originalContact = new ContactBuilder().WithOrganisationName("Original");
        Contact newContact = new ContactBuilder().WithOrganisationName("New");
        ContactRoot original = new() { Contact = originalContact };

        // Act
        ContactRoot updated = original with { Contact = newContact };

        // Assert
        updated.Contact?.OrganisationName.ShouldBe("New");
        original.Contact?.OrganisationName.ShouldBe("Original");
        updated.ShouldNotBeSameAs(original);
    }

    [TestMethod]
    public void CollectionRoots_WithImmutableCollections_PreventMutation()
    {
        // Arrange
        ImmutableList<Contact> contacts = ImmutableList.Create(
            new ContactBuilder().WithOrganisationName("Org1").Build()
        );
        ContactsRoot root = new() { Contacts = contacts };

        // Act
        ImmutableList<Contact> newContacts = root.Contacts.Add(
            new ContactBuilder().WithOrganisationName("Org2").Build()
        );

        // Assert
        root.Contacts.Count.ShouldBe(1); // Original unchanged
        newContacts.Count.ShouldBe(2);   // New list has 2 items
        root.Contacts.ShouldNotBeSameAs(newContacts);
    }
}

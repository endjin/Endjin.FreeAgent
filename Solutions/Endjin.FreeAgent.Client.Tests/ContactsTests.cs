// <copyright file="ContactsTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Immutable;
using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class ContactsTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Contacts contacts = null!;
    private HttpClient httpClient = null!;
    private TestHttpMessageHandler messageHandler = null!;

    [TestInitialize]
    public async Task Setup()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.messageHandler = new TestHttpMessageHandler();
        this.httpClient = new HttpClient(this.messageHandler);
        this.httpClientFactory = Substitute.For<IHttpClientFactory>();
        this.httpClientFactory.CreateClient(Arg.Any<string>()).Returns(this.httpClient);
        this.loggerFactory = Substitute.For<ILoggerFactory>();

        this.freeAgentClient = new FreeAgentClient(
            new FreeAgentOptionsBuilder().Build(),
            this.cache,
            this.httpClientFactory,
            this.loggerFactory);

        // Use proper initialization with TestOAuth2Service
        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);

        this.contacts = new Contacts(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidContact_ReturnsCreatedContact()
    {
        // Arrange
        Contact inputContact = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            OrganisationName = "Acme Corp"
        };

        Contact responseContact = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            OrganisationName = "Acme Corp",
            Url = new Uri("https://api.freeagent.com/v2/contacts/12345")
        };

        ContactRoot responseRoot = new() { Contact = responseContact };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Contact result = await this.contacts.CreateAsync(inputContact);

        // Assert - Result
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
        result.Email.ShouldBe("john.doe@example.com");
        result.OrganisationName.ShouldBe("Acme Corp");
        result.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/contacts/12345");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<ContactRoot>(body =>
        {
            body.Contact.ShouldNotBeNull();
            body.Contact.FirstName.ShouldBe("John");
            body.Contact.LastName.ShouldBe("Doe");
            body.Contact.Email.ShouldBe("john.doe@example.com");
            body.Contact.OrganisationName.ShouldBe("Acme Corp");
        });
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllContacts_AndCachesResult()
    {
        // Arrange
        ImmutableList<Contact> contactsList = ImmutableList.Create(
            new Contact { FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new Contact { FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        );

        ContactsRoot responseRoot = new() { Contacts = contactsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<Contact> result1 = await this.contacts.GetAllAsync();

        // Act - Second call (should use cache)
        IEnumerable<Contact> result2 = await this.contacts.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(2);
        result1.First().FirstName.ShouldBe("John");
        result1.Last().FirstName.ShouldBe("Jane");

        result2.Count().ShouldBe(2);
        result2.ShouldBe(result1); // Should be the same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts?view=active");
    }

    [TestMethod]
    public async Task GetAllWithActiveProjectsAsync_FiltersContactsWithActiveProjects()
    {
        // Arrange
        // When using 'active_projects' view, the API should only return contacts with active projects
        ImmutableList<Contact> contactsList = ImmutableList.Create(
            new Contact { FirstName = "John", LastName = "Doe", ActiveProjectsCount = "2" },
            new Contact { FirstName = "Bob", LastName = "Johnson", ActiveProjectsCount = "5" }
        );

        ContactsRoot responseRoot = new() { Contacts = contactsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<Contact> result = await this.contacts.GetAllWithActiveProjectsAsync();

        // Assert
        result.Count().ShouldBe(2);
        result.All(c => !string.IsNullOrEmpty(c.ActiveProjectsCount) && int.Parse(c.ActiveProjectsCount) > 0).ShouldBeTrue();
        result.Any(c => c.FirstName == "John").ShouldBeTrue();
        result.Any(c => c.FirstName == "Bob").ShouldBeTrue();

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts?view=active_projects");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsContact()
    {
        // Arrange
        string contactId = "12345";
        Contact responseContact = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Url = new Uri($"https://api.freeagent.com/v2/contacts/{contactId}")
        };

        ContactRoot responseRoot = new() { Contact = responseContact };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Contact? result = await this.contacts.GetByIdAsync(contactId);

        // Assert
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
        result.Email.ShouldBe("john@example.com");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/contacts/{contactId}");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.contacts.GetByIdAsync("invalid-id"));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts/invalid-id");
    }

    [TestMethod]
    public async Task GetByOrganisationNameAsync_WithMatchingName_ReturnsContacts()
    {
        // Arrange
        ImmutableList<Contact> contactsList = ImmutableList.Create(
            new Contact { FirstName = "John", LastName = "Doe", OrganisationName = "Acme Corp" },
            new Contact { FirstName = "Jane", LastName = "Smith", OrganisationName = "Acme Corp" }
        );

        ContactsRoot responseRoot = new() { Contacts = contactsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Contact? result = await this.contacts.GetByOrganisationNameAsync("Acme Corp");

        // Assert
        result.ShouldNotBeNull();
        result.OrganisationName.ShouldBe("Acme Corp");
        result.FirstName.ShouldBe("John"); // Returns first match

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts?view=active");
    }

    [TestMethod]
    public async Task GetByOrganisationNameAsync_WithNoMatches_ThrowsInvalidOperationException()
    {
        // Arrange
        ContactsRoot responseRoot = new() { Contacts = ImmutableList<Contact>.Empty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(async () =>
            await this.contacts.GetByOrganisationNameAsync("NonExistent Corp"));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts?view=active");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidContact_ReturnsUpdatedContact()
    {
        // Arrange
        string contactId = "12345";
        Contact inputContact = new()
        {
            FirstName = "John",
            LastName = "Updated",
            Email = "john.updated@example.com",
            OrganisationName = "Updated Corp"
        };

        Contact responseContact = new()
        {
            FirstName = "John",
            LastName = "Updated",
            Email = "john.updated@example.com",
            OrganisationName = "Updated Corp",
            Url = new Uri($"https://api.freeagent.com/v2/contacts/{contactId}")
        };

        ContactRoot responseRoot = new() { Contact = responseContact };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Contact result = await this.contacts.UpdateAsync(contactId, inputContact);

        // Assert - Result
        result.ShouldNotBeNull();
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Updated");
        result.Email.ShouldBe("john.updated@example.com");
        result.OrganisationName.ShouldBe("Updated Corp");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/contacts/{contactId}");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<ContactRoot>(body =>
        {
            body.Contact.ShouldNotBeNull();
            body.Contact.FirstName.ShouldBe("John");
            body.Contact.LastName.ShouldBe("Updated");
            body.Contact.Email.ShouldBe("john.updated@example.com");
            body.Contact.OrganisationName.ShouldBe("Updated Corp");
        });
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesContact()
    {
        // Arrange
        string contactId = "12345";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.contacts.DeleteAsync(contactId);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/contacts/{contactId}");
    }

    [TestMethod]
    public async Task DeleteAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        string contactId = "invalid-id";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Contact not found", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.contacts.DeleteAsync(contactId));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/contacts/{contactId}");
    }

    [TestMethod]
    public async Task GetAllAsync_WithQueryParameters_AppliesFiltersCorrectly()
    {
        // Arrange
        ImmutableList<Contact> contactsList = ImmutableList.Create(
            new Contact { FirstName = "John", LastName = "Doe", Email = "john@example.com" }
        );

        ContactsRoot responseRoot = new() { Contacts = contactsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        DateTimeOffset updatedSince = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        IEnumerable<Contact> result = await this.contacts.GetAllAsync(
            view: "clients",
            sort: "-updated_at",
            updatedSince: updatedSince);

        // Assert
        result.Count().ShouldBe(1);
        result.First().FirstName.ShouldBe("John");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();

        // Check that the URL contains the expected query parameters
        string expectedUpdatedSince = Uri.EscapeDataString(updatedSince.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
        string expectedUri = $"/v2/contacts?view=clients&sort=-updated_at&updated_since={expectedUpdatedSince}";
        this.messageHandler.ShouldHaveBeenCalledWithUri(expectedUri);
    }

    [TestMethod]
    public async Task GetAllAsync_WithoutParameters_UsesDefaultView()
    {
        // Arrange
        ImmutableList<Contact> contactsList = ImmutableList.Create(
            new Contact { FirstName = "Test", LastName = "User" }
        );

        ContactsRoot responseRoot = new() { Contacts = contactsList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Call the overload without parameters
        IEnumerable<Contact> result = await this.contacts.GetAllAsync();

        // Assert
        result.Count().ShouldBe(1);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        // Should use view=active when called without parameters (since it delegates to GetAllAsync("active"))
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/contacts?view=active");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }

}

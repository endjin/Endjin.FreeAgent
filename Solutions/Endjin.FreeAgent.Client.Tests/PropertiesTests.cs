// <copyright file="PropertiesTests.cs" company="Endjin Limited">
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
public class PropertiesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Properties properties = null!;
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

        this.properties = new Properties(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task CreateAsync_WithValidProperty_ReturnsCreatedProperty()
    {
        // Arrange
        Property inputProperty = new()
        {
            Address1 = "123 Test Street",
            Address2 = "Apartment 4B",
            Town = "London",
            Postcode = "SW1A 1AA"
        };

        Property responseProperty = new()
        {
            Address1 = "123 Test Street",
            Address2 = "Apartment 4B",
            Town = "London",
            Postcode = "SW1A 1AA",
            Country = "United Kingdom",
            Url = new Uri("https://api.freeagent.com/v2/properties/12345")
        };

        PropertyRoot responseRoot = new() { Property = responseProperty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Property result = await this.properties.CreateAsync(inputProperty);

        // Assert - Result
        result.ShouldNotBeNull();
        result.Address1.ShouldBe("123 Test Street");
        result.Address2.ShouldBe("Apartment 4B");
        result.Town.ShouldBe("London");
        result.Postcode.ShouldBe("SW1A 1AA");
        result.Country.ShouldBe("United Kingdom");
        result.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/properties/12345");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/properties");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<PropertyRoot>(body =>
        {
            body.Property.ShouldNotBeNull();
            body.Property.Address1.ShouldBe("123 Test Street");
            body.Property.Address2.ShouldBe("Apartment 4B");
            body.Property.Town.ShouldBe("London");
            body.Property.Postcode.ShouldBe("SW1A 1AA");
        });
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllProperties_AndCachesResult()
    {
        // Arrange
        ImmutableList<Property> propertiesList = ImmutableList.Create(
            new Property { Address1 = "123 Main Street", Town = "London", Country = "United Kingdom" },
            new Property { Address1 = "456 High Street", Town = "Manchester", Country = "United Kingdom" }
        );

        PropertiesRoot responseRoot = new() { Properties = propertiesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<Property> result1 = await this.properties.GetAllAsync();

        // Act - Second call (should use cache)
        IEnumerable<Property> result2 = await this.properties.GetAllAsync();

        // Assert
        result1.Count().ShouldBe(2);
        result1.First().Address1.ShouldBe("123 Main Street");
        result1.Last().Address1.ShouldBe("456 High Street");

        result2.Count().ShouldBe(2);
        result2.ShouldBe(result1); // Should be the same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/properties");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsProperty()
    {
        // Arrange
        string propertyId = "12345";
        Property responseProperty = new()
        {
            Address1 = "123 Test Street",
            Town = "London",
            Postcode = "SW1A 1AA",
            Country = "United Kingdom",
            Url = new Uri($"https://api.freeagent.com/v2/properties/{propertyId}")
        };

        PropertyRoot responseRoot = new() { Property = responseProperty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Property? result = await this.properties.GetByIdAsync(propertyId);

        // Assert
        result.ShouldNotBeNull();
        result.Address1.ShouldBe("123 Test Street");
        result.Town.ShouldBe("London");
        result.Postcode.ShouldBe("SW1A 1AA");
        result.Country.ShouldBe("United Kingdom");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/properties/{propertyId}");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.properties.GetByIdAsync("invalid-id"));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/properties/invalid-id");
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidProperty_ReturnsUpdatedProperty()
    {
        // Arrange
        string propertyId = "12345";
        Property inputProperty = new()
        {
            Address1 = "789 Updated Street",
            Town = "Birmingham",
            Postcode = "B1 1AA"
        };

        Property responseProperty = new()
        {
            Address1 = "789 Updated Street",
            Town = "Birmingham",
            Postcode = "B1 1AA",
            Country = "United Kingdom",
            Url = new Uri($"https://api.freeagent.com/v2/properties/{propertyId}")
        };

        PropertyRoot responseRoot = new() { Property = responseProperty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        Property result = await this.properties.UpdateAsync(propertyId, inputProperty);

        // Assert - Result
        result.ShouldNotBeNull();
        result.Address1.ShouldBe("789 Updated Street");
        result.Town.ShouldBe("Birmingham");
        result.Postcode.ShouldBe("B1 1AA");
        result.Country.ShouldBe("United Kingdom");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/properties/{propertyId}");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<PropertyRoot>(body =>
        {
            body.Property.ShouldNotBeNull();
            body.Property.Address1.ShouldBe("789 Updated Street");
            body.Property.Town.ShouldBe("Birmingham");
            body.Property.Postcode.ShouldBe("B1 1AA");
        });
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesProperty()
    {
        // Arrange
        string propertyId = "12345";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.properties.DeleteAsync(propertyId);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/properties/{propertyId}");
    }

    [TestMethod]
    public async Task DeleteAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        string propertyId = "invalid-id";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Property not found", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.properties.DeleteAsync(propertyId));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/properties/{propertyId}");
    }

    [TestMethod]
    public async Task GetByIdAsync_CachesResult()
    {
        // Arrange
        string propertyId = "12345";
        Property responseProperty = new()
        {
            Address1 = "123 Cached Street",
            Town = "London",
            Country = "United Kingdom",
            Url = new Uri($"https://api.freeagent.com/v2/properties/{propertyId}")
        };

        PropertyRoot responseRoot = new() { Property = responseProperty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        Property result1 = await this.properties.GetByIdAsync(propertyId);

        // Act - Second call (should use cache)
        Property result2 = await this.properties.GetByIdAsync(propertyId);

        // Assert
        result1.ShouldNotBeNull();
        result2.ShouldNotBeNull();
        result1.Address1.ShouldBe("123 Cached Street");
        result2.Address1.ShouldBe("123 Cached Street");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
    }

    [TestMethod]
    public async Task CreateAsync_InvalidatesCacheAfterCreation()
    {
        // Arrange - First get all to populate cache
        ImmutableList<Property> initialList = ImmutableList.Create(
            new Property { Address1 = "Initial Property", Country = "United Kingdom" }
        );

        PropertiesRoot initialRoot = new() { Properties = initialList };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.properties.GetAllAsync();

        // Arrange - Create new property
        Property newProperty = new() { Address1 = "New Property" };
        Property responseProperty = new()
        {
            Address1 = "New Property",
            Country = "United Kingdom",
            Url = new Uri("https://api.freeagent.com/v2/properties/12345")
        };

        PropertyRoot responseRoot = new() { Property = responseProperty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Create invalidates cache
        await this.properties.CreateAsync(newProperty);

        // Arrange - Setup response for second GetAllAsync
        ImmutableList<Property> updatedList = ImmutableList.Create(
            new Property { Address1 = "Initial Property", Country = "United Kingdom" },
            new Property { Address1 = "New Property", Country = "United Kingdom" }
        );

        PropertiesRoot updatedRoot = new() { Properties = updatedList };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act - GetAllAsync should make a new request since cache was invalidated
        IEnumerable<Property> result = await this.properties.GetAllAsync();

        // Assert
        result.Count().ShouldBe(2);

        // Assert - Mock Verification (3 calls: initial GetAll, Create, second GetAll)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task UpdateAsync_InvalidatesCacheAfterUpdate()
    {
        // Arrange - First get all to populate cache
        ImmutableList<Property> initialList = ImmutableList.Create(
            new Property { Address1 = "Initial Property", Country = "United Kingdom" }
        );

        PropertiesRoot initialRoot = new() { Properties = initialList };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.properties.GetAllAsync();

        // Arrange - Update property
        string propertyId = "12345";
        Property updatedProperty = new() { Address1 = "Updated Property" };
        Property responseProperty = new()
        {
            Address1 = "Updated Property",
            Country = "United Kingdom",
            Url = new Uri($"https://api.freeagent.com/v2/properties/{propertyId}")
        };

        PropertyRoot responseRoot = new() { Property = responseProperty };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - Update invalidates cache
        await this.properties.UpdateAsync(propertyId, updatedProperty);

        // Arrange - Setup response for second GetAllAsync
        ImmutableList<Property> updatedList = ImmutableList.Create(
            new Property { Address1 = "Updated Property", Country = "United Kingdom" }
        );

        PropertiesRoot updatedRoot = new() { Properties = updatedList };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act - GetAllAsync should make a new request since cache was invalidated
        IEnumerable<Property> result = await this.properties.GetAllAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Address1.ShouldBe("Updated Property");

        // Assert - Mock Verification (3 calls: initial GetAll, Update, second GetAll)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public async Task DeleteAsync_InvalidatesCacheAfterDeletion()
    {
        // Arrange - First get all to populate cache
        ImmutableList<Property> initialList = ImmutableList.Create(
            new Property { Address1 = "Property 1", Country = "United Kingdom" },
            new Property { Address1 = "Property 2", Country = "United Kingdom" }
        );

        PropertiesRoot initialRoot = new() { Properties = initialList };
        string initialJson = JsonSerializer.Serialize(initialRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(initialJson, Encoding.UTF8, "application/json")
        };

        await this.properties.GetAllAsync();

        // Arrange - Delete property
        string propertyId = "12345";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act - Delete invalidates cache
        await this.properties.DeleteAsync(propertyId);

        // Arrange - Setup response for second GetAllAsync
        ImmutableList<Property> updatedList = ImmutableList.Create(
            new Property { Address1 = "Property 1", Country = "United Kingdom" }
        );

        PropertiesRoot updatedRoot = new() { Properties = updatedList };
        string updatedJson = JsonSerializer.Serialize(updatedRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(updatedJson, Encoding.UTF8, "application/json")
        };

        // Act - GetAllAsync should make a new request since cache was invalidated
        IEnumerable<Property> result = await this.properties.GetAllAsync();

        // Assert
        result.Count().ShouldBe(1);
        result.First().Address1.ShouldBe("Property 1");

        // Assert - Mock Verification (3 calls: initial GetAll, Delete, second GetAll)
        this.messageHandler.CallCount.ShouldBe(3);
    }

    [TestMethod]
    public void Property_SerializesCorrectly()
    {
        // Arrange
        Property property = new()
        {
            Url = new Uri("https://api.freeagent.com/v2/properties/12345"),
            Address1 = "123 Test Street",
            Address2 = "Suite 100",
            Address3 = "Building A",
            Town = "London",
            Region = "Greater London",
            Postcode = "SW1A 1AA",
            Country = "United Kingdom"
        };

        // Act
        string json = JsonSerializer.Serialize(property, SharedJsonOptions.SourceGenOptions);
        Property? deserialized = JsonSerializer.Deserialize<Property>(json, SharedJsonOptions.SourceGenOptions);

        // Assert
        deserialized.ShouldNotBeNull();
        deserialized.Url?.ToString().ShouldBe("https://api.freeagent.com/v2/properties/12345");
        deserialized.Address1.ShouldBe("123 Test Street");
        deserialized.Address2.ShouldBe("Suite 100");
        deserialized.Address3.ShouldBe("Building A");
        deserialized.Town.ShouldBe("London");
        deserialized.Region.ShouldBe("Greater London");
        deserialized.Postcode.ShouldBe("SW1A 1AA");
        deserialized.Country.ShouldBe("United Kingdom");
    }

    [TestMethod]
    public void Property_SerializesWithCorrectPropertyNames()
    {
        // Arrange
        Property property = new()
        {
            Address1 = "123 Test Street",
            Address2 = "Suite 100"
        };

        // Act
        string json = JsonSerializer.Serialize(property, SharedJsonOptions.SourceGenOptions);

        // Assert - Should use correct JSON property names
        json.ShouldContain("\"address1\"");
        json.ShouldContain("\"address2\"");
    }

    [TestMethod]
    public void Property_OmitsNullValues()
    {
        // Arrange
        Property property = new()
        {
            Address1 = "123 Test Street"
            // All other properties are null
        };

        // Act
        string json = JsonSerializer.Serialize(property, SharedJsonOptions.SourceGenOptions);

        // Assert - Null values should be omitted
        json.ShouldNotContain("address2");
        json.ShouldNotContain("address3");
        json.ShouldNotContain("town");
        json.ShouldNotContain("region");
        json.ShouldNotContain("postcode");
        json.ShouldNotContain("country");
        json.ShouldNotContain("url");
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }
}

// <copyright file="NotesTests.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Net;
using System.Text;
using System.Text.Json;

using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client.Tests;

[TestClass]
public class NotesTests
{
    private MemoryCache cache = null!;
    private IHttpClientFactory httpClientFactory = null!;
    private ILoggerFactory loggerFactory = null!;
    private FreeAgentClient freeAgentClient = null!;
    private Notes notes = null!;
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

        await TestHelper.SetupForTestingAsync(this.freeAgentClient, this.httpClientFactory);
        this.notes = new Notes(this.freeAgentClient, this.cache);
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_ReturnsProjectNotes()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/123");
        List<NoteItem> notesList =
        [
            new()
            {
                Url = "https://api.freeagent.com/v2/notes/1",
                Note = "First project note",
                ParentUrl = "https://api.freeagent.com/v2/projects/123",
                Author = "John Doe",
                CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0),
                UpdatedAt = new DateTime(2024, 1, 15, 10, 30, 0)
            },
            new()
            {
                Url = "https://api.freeagent.com/v2/notes/2",
                Note = "Second project note",
                ParentUrl = "https://api.freeagent.com/v2/projects/123",
                Author = "Jane Smith",
                CreatedAt = new DateTime(2024, 1, 20, 14, 45, 0),
                UpdatedAt = new DateTime(2024, 1, 21, 9, 0, 0)
            }
        ];

        NotesRoot responseRoot = new() { Notes = notesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<NoteItem> result = await this.notes.GetAllByProjectUrlAsync(projectUrl);

        // Assert - Result
        result.Count().ShouldBe(2);

        NoteItem firstNote = result.First();
        firstNote.Url.ShouldBe("https://api.freeagent.com/v2/notes/1");
        firstNote.Note.ShouldBe("First project note");
        firstNote.ParentUrl.ShouldBe("https://api.freeagent.com/v2/projects/123");
        firstNote.Author.ShouldBe("John Doe");
        firstNote.CreatedAt.ShouldBe(new DateTime(2024, 1, 15, 10, 30, 0));
        firstNote.UpdatedAt.ShouldBe(new DateTime(2024, 1, 15, 10, 30, 0));

        NoteItem secondNote = result.Last();
        secondNote.Url.ShouldBe("https://api.freeagent.com/v2/notes/2");
        secondNote.Note.ShouldBe("Second project note");
        secondNote.Author.ShouldBe("Jane Smith");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes?project={projectUrl}");
    }

    [TestMethod]
    public async Task GetAllByContactUrlAsync_ReturnsContactNotes()
    {
        // Arrange
        Uri contactUrl = new("https://api.freeagent.com/v2/contacts/456");
        List<NoteItem> notesList =
        [
            new()
            {
                Url = "https://api.freeagent.com/v2/notes/3",
                Note = "Contact note",
                ParentUrl = "https://api.freeagent.com/v2/contacts/456",
                Author = "Bob Wilson",
                CreatedAt = new DateTime(2024, 2, 1, 8, 0, 0),
                UpdatedAt = new DateTime(2024, 2, 1, 8, 0, 0)
            }
        ];

        NotesRoot responseRoot = new() { Notes = notesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<NoteItem> result = await this.notes.GetAllByContactUrlAsync(contactUrl);

        // Assert - Result
        result.Count().ShouldBe(1);

        NoteItem note = result.First();
        note.Url.ShouldBe("https://api.freeagent.com/v2/notes/3");
        note.Note.ShouldBe("Contact note");
        note.ParentUrl.ShouldBe("https://api.freeagent.com/v2/contacts/456");
        note.Author.ShouldBe("Bob Wilson");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes?contact={contactUrl}");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithValidId_ReturnsNote()
    {
        // Arrange
        string noteId = "123";
        NoteItem responseNote = new()
        {
            Url = "https://api.freeagent.com/v2/notes/123",
            Note = "Test note content",
            ParentUrl = "https://api.freeagent.com/v2/projects/456",
            Author = "Test Author",
            CreatedAt = new DateTime(2024, 3, 1, 12, 0, 0),
            UpdatedAt = new DateTime(2024, 3, 2, 15, 30, 0)
        };

        NoteRoot responseRoot = new() { Note = responseNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        NoteItem result = await this.notes.GetByIdAsync(noteId);

        // Assert - Result
        result.ShouldNotBeNull();
        result.Url.ShouldBe("https://api.freeagent.com/v2/notes/123");
        result.Note.ShouldBe("Test note content");
        result.ParentUrl.ShouldBe("https://api.freeagent.com/v2/projects/456");
        result.Author.ShouldBe("Test Author");
        result.CreatedAt.ShouldBe(new DateTime(2024, 3, 1, 12, 0, 0));
        result.UpdatedAt.ShouldBe(new DateTime(2024, 3, 2, 15, 30, 0));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes/{noteId}");
    }

    [TestMethod]
    public async Task GetByIdAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound);

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.notes.GetByIdAsync("invalid-id"));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenGetRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri("/v2/notes/invalid-id");
    }

    [TestMethod]
    public async Task CreateForProjectAsync_WithValidNote_ReturnsCreatedNote()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/789");
        NoteItem inputNote = new()
        {
            Note = "New project note"
        };

        NoteItem responseNote = new()
        {
            Url = "https://api.freeagent.com/v2/notes/100",
            Note = "New project note",
            ParentUrl = "https://api.freeagent.com/v2/projects/789",
            Author = "Current User",
            CreatedAt = new DateTime(2024, 4, 1, 10, 0, 0),
            UpdatedAt = new DateTime(2024, 4, 1, 10, 0, 0)
        };

        NoteRoot responseRoot = new() { Note = responseNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        NoteItem result = await this.notes.CreateForProjectAsync(projectUrl, inputNote);

        // Assert - Result
        result.ShouldNotBeNull();
        result.Url.ShouldBe("https://api.freeagent.com/v2/notes/100");
        result.Note.ShouldBe("New project note");
        result.ParentUrl.ShouldBe("https://api.freeagent.com/v2/projects/789");
        result.Author.ShouldBe("Current User");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes?project={projectUrl}");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<NoteRoot>(body =>
        {
            body.Note.ShouldNotBeNull();
            body.Note.Note.ShouldBe("New project note");
        });
    }

    [TestMethod]
    public async Task CreateForContactAsync_WithValidNote_ReturnsCreatedNote()
    {
        // Arrange
        Uri contactUrl = new("https://api.freeagent.com/v2/contacts/321");
        NoteItem inputNote = new()
        {
            Note = "New contact note"
        };

        NoteItem responseNote = new()
        {
            Url = "https://api.freeagent.com/v2/notes/101",
            Note = "New contact note",
            ParentUrl = "https://api.freeagent.com/v2/contacts/321",
            Author = "Current User",
            CreatedAt = new DateTime(2024, 4, 2, 11, 0, 0),
            UpdatedAt = new DateTime(2024, 4, 2, 11, 0, 0)
        };

        NoteRoot responseRoot = new() { Note = responseNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.Created)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        NoteItem result = await this.notes.CreateForContactAsync(contactUrl, inputNote);

        // Assert - Result
        result.ShouldNotBeNull();
        result.Url.ShouldBe("https://api.freeagent.com/v2/notes/101");
        result.Note.ShouldBe("New contact note");
        result.ParentUrl.ShouldBe("https://api.freeagent.com/v2/contacts/321");
        result.Author.ShouldBe("Current User");

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPostRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes?contact={contactUrl}");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<NoteRoot>(body =>
        {
            body.Note.ShouldNotBeNull();
            body.Note.Note.ShouldBe("New contact note");
        });
    }

    [TestMethod]
    public async Task UpdateAsync_WithValidNote_ReturnsUpdatedNote()
    {
        // Arrange
        string noteId = "200";
        NoteItem inputNote = new()
        {
            Note = "Updated note content"
        };

        NoteItem responseNote = new()
        {
            Url = "https://api.freeagent.com/v2/notes/200",
            Note = "Updated note content",
            ParentUrl = "https://api.freeagent.com/v2/projects/456",
            Author = "Original Author",
            CreatedAt = new DateTime(2024, 1, 1, 9, 0, 0),
            UpdatedAt = new DateTime(2024, 5, 1, 16, 0, 0)
        };

        NoteRoot responseRoot = new() { Note = responseNote };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        NoteItem result = await this.notes.UpdateAsync(noteId, inputNote);

        // Assert - Result
        result.ShouldNotBeNull();
        result.Url.ShouldBe("https://api.freeagent.com/v2/notes/200");
        result.Note.ShouldBe("Updated note content");
        result.UpdatedAt.ShouldBe(new DateTime(2024, 5, 1, 16, 0, 0));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenPutRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes/{noteId}");
        this.messageHandler.ShouldHaveJsonContentType();
        await this.messageHandler.ShouldHaveJsonBody<NoteRoot>(body =>
        {
            body.Note.ShouldNotBeNull();
            body.Note.Note.ShouldBe("Updated note content");
        });
    }

    [TestMethod]
    public async Task DeleteAsync_WithValidId_DeletesNote()
    {
        // Arrange
        string noteId = "300";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        await this.notes.DeleteAsync(noteId);

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes/{noteId}");
    }

    [TestMethod]
    public async Task DeleteAsync_WithInvalidId_ThrowsHttpRequestException()
    {
        // Arrange
        string noteId = "invalid-id";
        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Note not found", Encoding.UTF8, "text/plain")
        };

        // Act & Assert
        await Should.ThrowAsync<HttpRequestException>(async () =>
            await this.notes.DeleteAsync(noteId));

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce();
        this.messageHandler.ShouldHaveBeenDeleteRequest();
        this.messageHandler.ShouldHaveBeenCalledWithUri($"/v2/notes/{noteId}");
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_CachesResults()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/123");
        List<NoteItem> notesList =
        [
            new()
            {
                Url = "https://api.freeagent.com/v2/notes/1",
                Note = "Cached note"
            }
        ];

        NotesRoot responseRoot = new() { Notes = notesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act - First call
        IEnumerable<NoteItem> result1 = await this.notes.GetAllByProjectUrlAsync(projectUrl);

        // Act - Second call (should use cache)
        IEnumerable<NoteItem> result2 = await this.notes.GetAllByProjectUrlAsync(projectUrl);

        // Assert
        result1.Count().ShouldBe(1);
        result2.Count().ShouldBe(1);
        result2.ShouldBe(result1); // Should be the same cached instance

        // Assert - Mock Verification
        this.messageHandler.ShouldHaveBeenCalledOnce(); // Only one HTTP call due to caching
    }

    [TestMethod]
    public async Task GetAllByProjectUrlAsync_WithNullableTimestamps_HandlesNullValues()
    {
        // Arrange
        Uri projectUrl = new("https://api.freeagent.com/v2/projects/123");
        List<NoteItem> notesList =
        [
            new()
            {
                Url = "https://api.freeagent.com/v2/notes/1",
                Note = "Note without timestamps",
                ParentUrl = "https://api.freeagent.com/v2/projects/123",
                Author = null,
                CreatedAt = null,
                UpdatedAt = null
            }
        ];

        NotesRoot responseRoot = new() { Notes = notesList };
        string responseJson = JsonSerializer.Serialize(responseRoot, SharedJsonOptions.Instance);

        this.messageHandler.Response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        };

        // Act
        IEnumerable<NoteItem> result = await this.notes.GetAllByProjectUrlAsync(projectUrl);

        // Assert
        result.Count().ShouldBe(1);

        NoteItem note = result.First();
        note.Author.ShouldBeNull();
        note.CreatedAt.ShouldBeNull();
        note.UpdatedAt.ShouldBeNull();
    }

    [TestCleanup]
    public void Cleanup()
    {
        this.cache?.Dispose();
        this.httpClient?.Dispose();
        this.messageHandler?.Dispose();
    }
}

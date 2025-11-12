# Copilot Instructions for Endjin.FreeAgent

## Repository Overview

This is a comprehensive .NET client library for the FreeAgent accounting API, providing strongly-typed access to all FreeAgent resources with modern C# features. The library is built with .NET 10 and C# 14 preview features, emphasizing type safety, performance, and developer experience.

## Technology Stack

- **Runtime**: .NET 10.0 (RC)
- **Language**: C# 14 (with preview features)
- **Testing Framework**: MSTest with Shouldly assertions and NSubstitute mocking
- **JSON Serialization**: System.Text.Json with source generation
- **HTTP Client**: Microsoft.Extensions.Http with built-in retry logic (Corvus.Retry)
- **Caching**: Microsoft.Extensions.Caching.Memory
- **Authentication**: OAuth2 via Duende.IdentityModel
- **Build System**: PowerShell-based build process using InvokeBuild and ZeroFailed modules

## Project Structure

The solution consists of the following projects:

- **Endjin.FreeAgent.Client** - Main client library providing API operations and HTTP communication
- **Endjin.FreeAgent.Domain** - Domain models and types representing all FreeAgent resources
- **Endjin.FreeAgent.Client.Tests** - Tests for the client library
- **Endjin.FreeAgent.Domain.Tests** - Tests for domain models
- **DemoApp** - Demonstration application showcasing client features

## Building and Testing

### Build the Solution
```bash
cd Solutions
dotnet build Endjin.FreeAgent.slnx
```

### Run Tests
```bash
dotnet test Endjin.FreeAgent.slnx
```

### Build with PowerShell Build Script
```powershell
./build.ps1 -Configuration Release
```

## Code Style and Conventions

### General C# Style
- Use **file-scoped namespaces** (enforced by .editorconfig)
- Use **init-only properties** on records for immutability
- Prefer **record types** for domain models
- Use **nullable reference types** throughout (enabled in all projects)
- Use **primary constructors** only when they improve clarity (not enforced by default)
- Use **top-level statements** where appropriate

### Copyright Headers
All source files must include the following copyright header:
```csharp
// <copyright file="FileName.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
```

### Global Usings
Common namespaces are defined in `GlobalUsings.cs` files:
- Domain project: System, System.Collections.Generic, System.Collections.Immutable, System.Diagnostics, System.Text.Json.Serialization
- Client project: System, System.Collections.Generic, System.Linq, System.Net.Http, System.Threading.Tasks, System.Text.Json, Microsoft.Extensions.Caching.Memory, Microsoft.Extensions.Options

### JSON Serialization
- Use `[JsonPropertyName("property_name")]` attributes for all JSON properties
- Follow snake_case naming for JSON property names (FreeAgent API convention)
- Use PascalCase for C# property names

### Patterns and Practices
- All domain models should be `record` types with init-only properties
- Use nullable types (`?`) to represent optional API fields
- API client methods should be async and return `Task<T>` or `IAsyncEnumerable<T>`
- Use dependency injection for services (IHttpClientFactory, IMemoryCache, ILoggerFactory)
- Follow the repository pattern for API resources (e.g., Invoices, Contacts, Projects)

### Testing Conventions
- Use MSTest framework (`[TestClass]`, `[TestMethod]`)
- Use Shouldly for assertions (e.g., `result.ShouldBe(expected)`)
- Use NSubstitute for mocking (e.g., `Substitute.For<IInterface>()`)
- Test file names should match the class being tested with `.cs` suffix
- Organize tests by feature/resource area

## Development Workflow

### Adding New API Resources
1. Define domain models in `Endjin.FreeAgent.Domain/Domain/` as record types
2. Add JSON converters if needed in `Endjin.FreeAgent.Domain/Converters/`
3. Create client methods in `Endjin.FreeAgent.Client/Client/` following existing patterns
4. Add comprehensive unit tests for both domain and client
5. Update README.md if adding major features

### Making Code Changes
- Keep changes minimal and focused
- Follow existing patterns and conventions
- Ensure all tests pass before committing
- Add tests for new functionality
- Update documentation for public API changes

### Working with Dependencies
- Dependencies are managed via NuGet packages
- The build uses ZeroFailed module for orchestration
- Auto-merge is configured for Endjin.*, Corvus.*, and Menes.* packages

## Key Architecture Concepts

### Client Architecture
- `FreeAgentClient` is the main entry point providing access to all resources
- Each resource (Invoices, Contacts, etc.) has its own service class
- HTTP communication uses `IHttpClientFactory` for proper connection pooling
- Retry logic is implemented using Corvus.Retry
- Authentication tokens are cached using `IMemoryCache`

### Domain Models
- All models are immutable records with init-only properties
- Models follow FreeAgent's API structure with snake_case JSON names
- Use `DateOnly` for date fields and `decimal` for monetary values
- Enums are used for fixed value sets with JSON converters

### OAuth2 Authentication
- OAuth2Service handles the authentication flow
- Supports authorization code exchange and token refresh
- Tokens are cached to minimize API calls
- Uses Duende.IdentityModel for OAuth2 protocol implementation

## Common Tasks

### Adding a New Domain Model
```csharp
// <copyright file="NewModel.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record NewModel
{
    [JsonPropertyName("id")]
    public int? Id { get; init; }
    
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
```

### Adding a New Client Method
```csharp
public async Task<NewModel?> GetAsync(int id)
{
    string endpoint = $"new_models/{id}";
    return await this.GetAsync<NewModel>(endpoint).ConfigureAwait(false);
}
```

## Important Notes

- This project uses .NET 10 RC which may have preview features
- Some warnings are suppressed: SYSLIB1031, NETSDK1057
- The solution uses a `.slnx` file format (XML-based solution file)
- InternalsVisibleTo is configured to allow test assemblies to access internal members
- The build system expects PowerShell 7 or later

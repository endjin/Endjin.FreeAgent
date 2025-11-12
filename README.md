# Endjin.FreeAgent

A comprehensive .NET client library for the [FreeAgent](https://www.freeagent.com/) accounting API, providing strongly-typed access to all FreeAgent resources with modern C# features.

## Overview

Endjin.FreeAgent is a fully-featured .NET client library designed to simplify integration with the FreeAgent accounting platform. Built with .NET 10 and C# 14 preview features, it provides a type-safe, performant, and developer-friendly way to interact with FreeAgent's REST API.

## Architecture

The solution consists of two core packages:

- **[Endjin.FreeAgent.Client](./Solutions/Endjin.FreeAgent.Client)** - The main client library providing API operations and HTTP communication
- **[Endjin.FreeAgent.Domain](./Solutions/Endjin.FreeAgent.Domain)** - Domain models and types representing all FreeAgent resources

## Features

### Comprehensive API Coverage
- ✅ **Invoicing** - Create, manage, and track sales invoices
- ✅ **Contacts** - Manage customers, suppliers, and other contacts
- ✅ **Projects** - Track projects and their associated tasks
- ✅ **Expenses** - Handle expense claims and receipts
- ✅ **Banking** - Bank accounts, transactions, and reconciliation
- ✅ **Time Tracking** - Timeslips and task management
- ✅ **Bills** - Purchase bills and supplier invoices
- ✅ **Credit Notes** - Refunds and corrections
- ✅ **Estimates** - Quotes and proposals
- ✅ **VAT Returns** - VAT calculations and submissions
- ✅ **Company Settings** - Company configuration and users

### Technical Features
- 🚀 **Modern .NET 10** - Built on the latest .NET runtime (RC)
- 📝 **Strongly-Typed** - Full type safety with C# 14 records
- ⚡ **High Performance** - System.Text.Json with source generation
- 🔄 **Async/Await** - Fully asynchronous operations
- 💾 **Built-in Caching** - Memory caching for improved performance
- 🔐 **OAuth2 Support** - Complete authentication flow implementation
- 📊 **Comprehensive Logging** - Integration with Microsoft.Extensions.Logging
- ♻️ **Retry Logic** - Resilient HTTP operations with Polly
- 🧪 **Well-Tested** - Extensive test coverage

## Getting Started

### Prerequisites

- .NET 10.0 RC or later
- A FreeAgent account with API access
- OAuth2 credentials (Client ID and Secret)

### Installation

Install the client library via NuGet:

```bash
dotnet add package Endjin.FreeAgent.Client --version 1.0.0-preview.1
```

The domain package is automatically included as a dependency.

### Basic Usage

```csharp
using Endjin.FreeAgent.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

// Configure services
ServiceCollection services = new();
services.AddMemoryCache();
services.AddHttpClient();
services.AddLogging(builder => builder.AddConsole());

ServiceProvider serviceProvider = services.BuildServiceProvider();

// Configure client options
FreeAgentClientOptions options = new()
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    RefreshToken = "your-refresh-token",
    ApiUri = new Uri("https://api.freeagent.com/v2/")
};

// Create client
IMemoryCache cache = serviceProvider.GetRequiredService<IMemoryCache>();
IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

FreeAgentClient client = new(options, cache, httpClientFactory, loggerFactory);

// Use the client
IEnumerable<Invoice> invoices = await client.Invoices.GetAllAsync();
IEnumerable<Contact> contacts = await client.Contacts.GetAllActiveAsync();
IEnumerable<Project> projects = await client.Projects.GetAllActiveAsync();
```

### OAuth2 Authentication

#### Interactive Login (Recommended for First-Time Setup)

The easiest way to obtain access and refresh tokens is to use the interactive login flow:

```csharp
using Endjin.FreeAgent.Client.OAuth2;
using Microsoft.Extensions.Logging;

// Configure OAuth2 options
OAuth2Options options = new()
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    UsePkce = true // Enable PKCE for enhanced security
};

// Create HTTP client and logger
using var httpClient = new HttpClient();
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<InteractiveLoginHelper>();

// Create the interactive login helper
var loginHelper = new InteractiveLoginHelper(options, httpClient, logger);

// Perform interactive login
// This will open a browser window for authorization
InteractiveLoginResult result = await loginHelper.LoginAsync(redirectPort: 5000);

Console.WriteLine($"Access Token: {result.AccessToken}");
Console.WriteLine($"Refresh Token: {result.RefreshToken}");
Console.WriteLine($"Expires At: {result.ExpiresAt}");

// Save the refresh token for future use
```

The interactive login flow will:
1. Start a local HTTP listener on the specified port (default: 5000)
2. Open your browser to the FreeAgent authorization page
3. Wait for the OAuth callback with the authorization code
4. Automatically exchange the code for access and refresh tokens
5. Return the tokens for you to save and use

You can also use the DemoApp to perform interactive login:

```bash
cd Solutions/DemoApp
dotnet run -- --interactive-login
```

#### Manual OAuth2 Flow

For more control over the OAuth2 flow, you can use the OAuth2Service directly:

```csharp
// Initialize OAuth2 service
IOAuth2Service oauth2Service = new OAuth2Service(options, httpClient, cache, logger);

// Exchange authorization code for tokens
TokenResponse tokens = await oauth2Service.ExchangeAuthorizationCodeAsync(authorizationCode);

// Refresh access token when needed
string newAccessToken = await oauth2Service.RefreshAccessTokenAsync();
```

## Building from Source

### Clone the Repository

```bash
git clone https://github.com/endjin/Endjin.FreeAgent.git
cd Endjin.FreeAgent
```

### Build the Solution

```bash
cd Solutions
dotnet build Endjin.FreeAgent.slnx
```

### Run Tests

```bash
dotnet test Endjin.FreeAgent.slnx
```

## Demo Application

The repository includes a comprehensive demo application showcasing various client features:

### Interactive Login Mode (Get Your First Tokens)

If you don't have a refresh token yet, use interactive login mode:

```bash
cd Solutions/DemoApp
dotnet run -- --interactive-login
```

This will:
1. Open your browser to authorize the application with FreeAgent
2. Automatically receive and display your access and refresh tokens
3. Test the tokens by fetching your contacts

You only need ClientId and ClientSecret in your appsettings.json for this mode.

### Standard Mode (Using Existing Refresh Token)

Once you have a refresh token, run the demo app normally:

```bash
cd Solutions/DemoApp
dotnet run
```

The demo app demonstrates:
- Interactive OAuth2 authentication flow
- Token refresh and management
- Fetching and displaying various resources
- Creating and updating entities
- Error handling and retry logic

### Configuration

#### Option 1: Configuration File

Create an `appsettings.json` file in the DemoApp directory:

For interactive login (first time):
```json
{
  "FreeAgent": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret"
  }
}
```

For standard mode (with existing refresh token):
```json
{
  "FreeAgent": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RefreshToken": "your-refresh-token",
    "ApiUri": "https://api.freeagent.com/v2/"
  }
}
```

#### Option 2: Environment Variables
Set the following environment variables:
```bash
# Linux/macOS
export FreeAgent__ClientId="your-client-id"
export FreeAgent__ClientSecret="your-client-secret"
export FreeAgent__RefreshToken="your-refresh-token"
export FreeAgent__ApiUri="https://api.freeagent.com/v2/"
# Windows (Command Prompt)
set FreeAgent__ClientId=your-client-id
set FreeAgent__ClientSecret=your-client-secret
set FreeAgent__RefreshToken=your-refresh-token
set FreeAgent__ApiUri=https://api.freeagent.com/v2/
# Windows (PowerShell)
$env:FreeAgent__ClientId = "your-client-id"
$env:FreeAgent__ClientSecret = "your-client-secret"
$env:FreeAgent__RefreshToken = "your-refresh-token"
$env:FreeAgent__ApiUri = "https://api.freeagent.com/v2/"
```

## Project Structure

```
Endjin.FreeAgent/
├── Solutions/
│   ├── Endjin.FreeAgent.Client/         # Main client library
│   │   ├── Client/                      # API client implementations
│   │   ├── Configuration/               # Client configuration
│   │   └── OAuth2/                      # Authentication logic
│   ├── Endjin.FreeAgent.Domain/         # Domain models
│   │   ├── Banking/                     # Banking-related models
│   │   ├── Contacts/                    # Contact models
│   │   ├── Invoicing/                   # Invoice models
│   │   └── ...                          # Other domain models
│   ├── Endjin.FreeAgent.Client.Tests/   # Client unit tests
│   ├── Endjin.FreeAgent.Domain.Tests/   # Domain unit tests
│   └── DemoApp/                         # Demo application
└── README.md                             # This file
```

## API Coverage

The client currently supports the following FreeAgent API endpoints:

| Resource | Operations | Status |
|----------|-----------|---------|
| Invoices | Create, Read, Update, Delete, List, Email, Mark as Sent | ✅ Complete |
| Contacts | Create, Read, Update, Delete, List | ✅ Complete |
| Projects | Create, Read, Update, Delete, List | ✅ Complete |
| Expenses | Create, Read, Update, Delete, List | ✅ Complete |
| Bank Accounts | Read, List | ✅ Complete |
| Bank Transactions | Create, Read, Update, Delete, List, Explain | ✅ Complete |
| Bills | Create, Read, Update, Delete, List | ✅ Complete |
| Credit Notes | Create, Read, Delete, List | ✅ Complete |
| Estimates | Create, Read, Update, Delete, List, Send, Mark as Sent | ✅ Complete |
| Tasks | Create, Read, Update, Delete, List | ✅ Complete |
| Timeslips | Create, Read, Update, Delete, List | ✅ Complete |
| Users | Read, List, Update | ✅ Complete |
| Company | Read, Update | ✅ Complete |
| VAT Returns | Read, List | ✅ Complete |
| Attachments | Create, Delete | ✅ Complete |
| Categories | List | ✅ Complete |
| Journal Sets | Create, Read, Delete, List | ✅ Complete |
| Notes | Create, Read, Update, Delete, List | ✅ Complete |
| Recurring Invoices | Read, List | ✅ Complete |
| Stock Items | Read, List | ✅ Complete |
| Trial Balance | Read | ✅ Complete |

## Contributing

We welcome contributions! Please see our contributing guidelines for details on:
- Code style and conventions
- Testing requirements
- Pull request process
- Issue reporting

### Development Requirements

- Visual Studio 2022 Preview or later (for C# 14 support)
- .NET 10 SDK RC
- Git

### Running Tests

The solution includes comprehensive unit tests:

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Endjin.FreeAgent.Client.Tests/Endjin.FreeAgent.Client.Tests.csproj
```

## Performance Considerations

The client library is optimized for performance:

- **Source-Generated JSON** - Uses System.Text.Json source generation for minimal allocations
- **HTTP Client Factory** - Proper HttpClient lifecycle management
- **Memory Caching** - Built-in caching for frequently accessed resources
- **Async Throughout** - Non-blocking I/O operations
- **Minimal Dependencies** - Carefully selected, essential dependencies only

## Error Handling

The client provides comprehensive error handling:

```csharp
try
{
    Invoice invoice = await client.Invoices.GetAsync(invoiceId);
}
catch (FreeAgentException ex)
{
    // Handle FreeAgent-specific errors
    Console.WriteLine($"FreeAgent error: {ex.Message}");
    Console.WriteLine($"Error code: {ex.ErrorCode}");
}
catch (HttpRequestException ex)
{
    // Handle network errors
    Console.WriteLine($"Network error: {ex.Message}");
}
```

## Roadmap

- [ ] .NET 10 stable release update

## Support

For issues, feature requests, or questions:

- 🐛 [Report an Issue](https://github.com/endjin/Endjin.FreeAgent/issues)
- 💬 [Discussions](https://github.com/endjin/Endjin.FreeAgent/discussions)
- 📧 Contact Endjin Limited

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

---

Copyright © Endjin Limited. All rights reserved.
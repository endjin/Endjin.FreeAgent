# Endjin.FreeAgent.Client

A comprehensive .NET client library for the [FreeAgent](https://www.freeagent.com/) accounting API, providing strongly-typed access to all FreeAgent resources.

## Features

- **Strongly-typed models** for all FreeAgent API resources
- **Modern .NET 10** implementation with C# 14 features
- **Async/await support** throughout
- **Built-in caching** for improved performance
- **OAuth2 authentication** support
- **Comprehensive resource coverage** including:
  - Invoices and Credit Notes
  - Contacts and Projects
  - Expenses and Bills
  - Bank Accounts and Transactions
  - Timeslips and Tasks
  - VAT Returns
  - Users and Company settings
  - And much more...

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Endjin.FreeAgent.Client
```

Or via Package Manager Console:

```powershell
Install-Package Endjin.FreeAgent.Client
```

## Quick Start

### Configuration

```csharp
using Endjin.FreeAgent.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

// Configure services
ServiceCollection services = new();
services.AddMemoryCache();
services.AddHttpClient();
services.AddLogging();

// Configure FreeAgent options
FreeAgentOptions options = new()
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    RefreshToken = "your-refresh-token"
};

// Create client
ServiceProvider serviceProvider = services.BuildServiceProvider();
IMemoryCache cache = serviceProvider.GetRequiredService<IMemoryCache>();
IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

FreeAgentClient client = new(options, cache, httpClientFactory, loggerFactory);
await client.InitializeAndAuthorizeAsync();
```

### Basic Usage

```csharp
using Endjin.FreeAgent.Domain;

// Get all active projects
IEnumerable<Project> projects = await client.Projects.GetAllActiveAsync();

// Get all contacts
IEnumerable<Contact> contacts = await client.Contacts.GetAllAsync();

// Create an invoice
Invoice invoice = new()
{
    Contact = "https://api.freeagent.com/v2/contacts/123",
    DatedOn = DateOnly.FromDateTime(DateTime.Now),
    PaymentTermsInDays = 30,
    InvoiceItems = new List<InvoiceItem>
    {
        new()
        {
            Description = "Consulting Services",
            ItemType = "Services",
            Quantity = 1,
            Price = 1000.00m
        }
    }
};

Invoice createdInvoice = await client.Invoices.CreateAsync(invoice);

// Get timeslips for a project
IEnumerable<Timeslip> timeslips = await client.Timeslips.GetByProjectUrlAsync("https://api.freeagent.com/v2/projects/456");
```

## Advanced Features

### Interactive OAuth2 Login

The easiest way to get your initial access and refresh tokens is to use the interactive login helper:

```csharp
using Endjin.FreeAgent.Client.OAuth2;
using Microsoft.Extensions.Logging;

// Configure OAuth2 options (only need ClientId and ClientSecret)
OAuth2Options options = new()
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    UsePkce = true
};

// Create HTTP client and logger
using var httpClient = new HttpClient();
using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
var logger = loggerFactory.CreateLogger<InteractiveLoginHelper>();

// Create and use the interactive login helper
var loginHelper = new InteractiveLoginHelper(options, httpClient, logger);
InteractiveLoginResult result = await loginHelper.LoginAsync(redirectPort: 5000);

// Save the refresh token for future use
Console.WriteLine($"Refresh Token: {result.RefreshToken}");
```

The `LoginAsync` method will:
1. Start a local HTTP listener
2. Open your browser to the FreeAgent authorization page
3. Wait for the OAuth callback
4. Exchange the authorization code for tokens
5. Return both access and refresh tokens

### Caching

The client includes built-in memory caching with configurable expiration:

```csharp
// Cache is automatically used for GET operations
IEnumerable<Project> projects1 = await client.Projects.GetAllAsync(); // Fetches from API
IEnumerable<Project> projects2 = await client.Projects.GetAllAsync(); // Returns cached result
```

### Error Handling

The client provides detailed error information for API failures:

```csharp
try
{
    Invoice invoice = await client.Invoices.GetByIdAsync("invalid-id");
}
catch (HttpRequestException ex)
{
    // Handle API errors
    Console.WriteLine($"API Error: {ex.Message}");
}
```

## Requirements

- .NET 10.0 or later
- FreeAgent API credentials (Client Id and Client Secret)
- Active FreeAgent account
- For first-time setup, you can use interactive login to obtain a refresh token

## Documentation

For complete API documentation, visit the [FreeAgent API Documentation](https://dev.freeagent.com/).

## Contributing

We welcome contributions! Please feel free to submit issues and pull requests.

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Support

For support, please contact Endjin Limited or raise an issue on our GitHub repository.

---

Copyright (c) Endjin Limited. All rights reserved.
// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using DemoApp;
using Endjin.FreeAgent.Client;
using Endjin.FreeAgent.Client.OAuth2;
using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Build configuration
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true)
    .Build();

// Check if we should run in interactive login mode
bool useInteractiveLogin = args.Contains("--interactive-login") || args.Contains("-i");

if (useInteractiveLogin)
{
    // Interactive Login Mode
    Console.WriteLine("=== FreeAgent Interactive Login Mode ===\n");

    string? clientId = configuration["FreeAgent:ClientId"];
    string? clientSecret = configuration["FreeAgent:ClientSecret"];

    if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
    {
        Console.WriteLine("Error: ClientId and ClientSecret must be configured in appsettings.json");
        Console.WriteLine("\nPlease ensure your appsettings.json contains:");
        Console.WriteLine("{");
        Console.WriteLine("  \"FreeAgent\": {");
        Console.WriteLine("    \"ClientId\": \"your-client-id\",");
        Console.WriteLine("    \"ClientSecret\": \"your-client-secret\"");
        Console.WriteLine("  }");
        Console.WriteLine("}");
        return;
    }

    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });

    var logger = loggerFactory.CreateLogger<Program>();

    try
    {
        InteractiveLoginResult result = await InteractiveLoginExample.PerformInteractiveLoginAsync(
            clientId,
            clientSecret,
            logger);

        // Optionally, test the token by making a simple API call
        Console.WriteLine("\n=== Testing the tokens ===");

        var testHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();
                services.AddHttpClient();
                services.AddLogging(builder => builder.AddConsole());
            })
            .Build();

        var cache = testHost.Services.GetRequiredService<IMemoryCache>();
        var httpClientFactory = testHost.Services.GetRequiredService<IHttpClientFactory>();
        var testLoggerFactory = testHost.Services.GetRequiredService<ILoggerFactory>();

        var testClient = new FreeAgentClient(
            clientId,
            clientSecret,
            result.RefreshToken,
            cache,
            httpClientFactory,
            testLoggerFactory);

        await testClient.InitializeAndAuthorizeAsync();

        var contacts = await testClient.Contacts.GetAllAsync();
        Console.WriteLine($"✅ Successfully retrieved {contacts.Count()} contacts from FreeAgent!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error during interactive login: {ex.Message}");
        logger.LogError(ex, "Interactive login failed");
    }
}
else
{
    // Standard Mode (using existing refresh token)
    Console.WriteLine("=== FreeAgent Standard Mode ===");
    Console.WriteLine("Tip: Use '--interactive-login' or '-i' to perform interactive OAuth login\n");

    // Setup DI container
    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            // Add memory cache
            services.AddMemoryCache();

            // Add FreeAgent client services with configuration
            services.AddFreeAgentClientServices(configuration);
        })
        .Build();

    // Get the FreeAgent client from DI
    FreeAgentClient client = host.Services.GetRequiredService<FreeAgentClient>();

    // Initialize and authorize the client
    await client.InitializeAndAuthorizeAsync();

    // Use the client - example with Contacts
    IEnumerable<Contact> contacts = await client.Contacts.GetAllAsync();

    Console.WriteLine($"Retrieved {contacts.Count()} contacts from FreeAgent.");
}
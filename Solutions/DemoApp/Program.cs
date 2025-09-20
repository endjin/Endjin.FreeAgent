// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Client;
using Endjin.FreeAgent.Domain;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build configuration
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true)
    .Build();

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
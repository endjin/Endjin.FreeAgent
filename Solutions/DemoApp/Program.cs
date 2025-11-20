// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using DemoApp.Commands;
using DemoApp.Infrastructure;
using Endjin.FreeAgent.Client;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace DemoApp;

public static class Program
{
    public static int Main(string[] args)
    {
        // Build configuration
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets(typeof(Program).Assembly, optional: true)
            .Build();

        var services = new ServiceCollection();

        // Register Configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Add memory cache
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddLogging(builder => builder.AddConsole());

        // Add FreeAgent client services with configuration
        // We register this unconditionally. If config is missing, it might throw during resolution,
        // but RunDemoCommand handles that by catching the exception in Standard Mode.
        // In Interactive Mode, we don't resolve it from the container.
        try 
        {
            services.AddFreeAgentClientServices(configuration);
        }
        catch
        {
            // Ignore registration errors here; they will be caught when resolving if needed.
            // However, AddFreeAgentClientServices usually just registers options and services, 
            // validation happens at resolution time.
        }

        var registrar = new TypeRegistrar(services);
        var app = new CommandApp<RunDemoCommand>(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("DemoApp");
            config.AddExample(new[] { "--interactive-login" });
            config.AddExample(new[] { "-i" });
        });

        return app.Run(args);
    }
}

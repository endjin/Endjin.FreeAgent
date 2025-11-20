// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using DemoApp.Commands;
using DemoApp.Infrastructure;
using Endjin.FreeAgent.Client;
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

        ServiceCollection services = new();

        // Register Configuration
        services.AddSingleton<IConfiguration>(configuration);

        // Add memory cache
        services.AddMemoryCache();
        services.AddHttpClient();
        services.AddLogging(builder => 
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Warning);
        });

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

        TypeRegistrar registrar = new(services);
        CommandApp<RunDemoCommand> app = new(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("DemoApp");
            config.AddExample(["--interactive-login"]);
            config.AddExample(["-i"]);
        });

        return app.Run(args);
    }
}

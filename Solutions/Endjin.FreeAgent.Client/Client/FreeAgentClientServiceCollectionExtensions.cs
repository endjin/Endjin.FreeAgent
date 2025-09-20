// <copyright file="FreeAgentClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client;

public static class FreeAgentClientServiceCollectionExtensions
{
    /// <summary>
    /// Extension methods for adding FreeAgent client services to the services container.
    /// </summary>
    /// <param name="services">The collection to add the services to.</param>
    /// <param name="configuration">The configuration instance.</param>
    /// <returns>The <see cref="IServiceCollection" /> with the FreeAgent services added.</returns>
    public static IServiceCollection AddFreeAgentClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure the FreeAgentOptions from configuration
        IConfigurationSection section = configuration.GetSection(FreeAgentOptions.SectionName);
        services.Configure<FreeAgentOptions>(section);

        // Add options validation
        services.AddSingleton<IValidateOptions<FreeAgentOptions>, FreeAgentOptionsValidator>();

        // Register HTTP services for dependency injection if AddHttpClient is available
        // This requires Microsoft.Extensions.Http package
        // For simpler setup, just register the services directly
        services.AddHttpClient();

        // Register FreeAgentClient with IHttpClientFactory and ILoggerFactory
        services.AddSingleton<FreeAgentClient>(serviceProvider =>
        {
            IOptions<FreeAgentOptions> options = serviceProvider.GetRequiredService<IOptions<FreeAgentOptions>>();
            IMemoryCache cache = serviceProvider.GetRequiredService<IMemoryCache>();
            IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new FreeAgentClient(options, cache, httpClientFactory, loggerFactory);
        });

        return services;
    }

    /// <summary>
    /// Extension methods for adding FreeAgent client services with explicit options.
    /// </summary>
    /// <param name="services">The collection to add the services to.</param>
    /// <param name="configureOptions">Action to configure the FreeAgent options.</param>
    /// <returns>The <see cref="IServiceCollection" /> with the FreeAgent services added.</returns>
    public static IServiceCollection AddFreeAgentClientServices(this IServiceCollection services, Action<FreeAgentOptions> configureOptions)
    {
        // Configure the FreeAgentOptions using the provided action
        services.Configure(configureOptions);

        // Add options validation
        services.AddSingleton<IValidateOptions<FreeAgentOptions>, FreeAgentOptionsValidator>();

        // Register HTTP services for dependency injection
        // This requires Microsoft.Extensions.Http package
        services.AddHttpClient();

        // Register FreeAgentClient with IHttpClientFactory and ILoggerFactory
        services.AddSingleton<FreeAgentClient>(serviceProvider =>
        {
            IOptions<FreeAgentOptions> options = serviceProvider.GetRequiredService<IOptions<FreeAgentOptions>>();
            IMemoryCache cache = serviceProvider.GetRequiredService<IMemoryCache>();
            IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
            return new FreeAgentClient(options, cache, httpClientFactory, loggerFactory);
        });

        return services;
    }
}
// <copyright file="FreeAgentClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Extension methods for registering FreeAgent client services in the dependency injection container.
/// </summary>
/// <remarks>
/// This class provides extension methods to simplify the registration of the <see cref="FreeAgentClient"/>
/// and its dependencies in the ASP.NET Core or .NET Generic Host dependency injection container.
/// </remarks>
public static class FreeAgentClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds FreeAgent client services to the dependency injection container using configuration.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration instance containing FreeAgent settings.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This method registers the following services:
    /// <list type="bullet">
    /// <item><description><see cref="FreeAgentOptions"/> configured from the "FreeAgent" configuration section</description></item>
    /// <item><description><see cref="FreeAgentOptionsValidator"/> for validating options at startup</description></item>
    /// <item><description><see cref="IHttpClientFactory"/> for creating HTTP clients</description></item>
    /// <item><description><see cref="FreeAgentClient"/> as a singleton</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// Example appsettings.json configuration:
    /// <code>
    /// {
    ///   "FreeAgent": {
    ///     "ClientId": "your-client-id",
    ///     "ClientSecret": "your-client-secret",
    ///     "RefreshToken": "your-refresh-token"
    ///   }
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Usage in Program.cs or Startup.cs:
    /// <code>
    /// services.AddFreeAgentClientServices(configuration);
    /// </code>
    /// </para>
    /// </remarks>
    /// <seealso cref="FreeAgentOptions"/>
    /// <seealso cref="FreeAgentClient"/>
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
    /// Adds FreeAgent client services to the dependency injection container using an action to configure options.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configureOptions">An action to configure the <see cref="FreeAgentOptions"/> programmatically.</param>
    /// <returns>The <see cref="IServiceCollection"/> for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// This overload allows you to configure options programmatically instead of using configuration files.
    /// It registers the same services as <see cref="AddFreeAgentClientServices(IServiceCollection, IConfiguration)"/>.
    /// </para>
    /// <para>
    /// Usage example:
    /// <code>
    /// services.AddFreeAgentClientServices(options =>
    /// {
    ///     options.ClientId = "your-client-id";
    ///     options.ClientSecret = "your-client-secret";
    ///     options.RefreshToken = "your-refresh-token";
    /// });
    /// </code>
    /// </para>
    /// <para>
    /// This approach is useful for:
    /// <list type="bullet">
    /// <item><description>Setting credentials from environment variables</description></item>
    /// <item><description>Using secrets management services</description></item>
    /// <item><description>Testing scenarios with mock credentials</description></item>
    /// <item><description>Dynamic configuration based on runtime conditions</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <seealso cref="FreeAgentOptions"/>
    /// <seealso cref="FreeAgentClient"/>
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
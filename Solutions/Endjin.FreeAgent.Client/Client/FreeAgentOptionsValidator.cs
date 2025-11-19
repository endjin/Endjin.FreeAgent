// <copyright file="FreeAgentOptionsValidator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Validates <see cref="FreeAgentOptions"/> to ensure all required configuration is present and valid at application startup.
/// </summary>
/// <remarks>
/// <para>
/// This validator implements <see cref="IValidateOptions{TOptions}"/> from the Microsoft.Extensions.Options
/// framework to perform eager validation of configuration options when the application starts.
/// This ensures that configuration errors are caught early rather than at runtime when the client
/// is first used.
/// </para>
/// <para>
/// The validator is automatically registered when using
/// <see cref="FreeAgentClientServiceCollectionExtensions.AddFreeAgentClientServices(Microsoft.Extensions.DependencyInjection.IServiceCollection, Microsoft.Extensions.Configuration.IConfiguration)"/>
/// or <see cref="FreeAgentClientServiceCollectionExtensions.AddFreeAgentClientServices(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Action{FreeAgentOptions})"/>.
/// </para>
/// <para>
/// Validation checks include:
/// <list type="bullet">
/// <item><description>Options object is not null</description></item>
/// <item><description>All required properties (ClientId, ClientSecret, RefreshToken) are present and non-empty</description></item>
/// <item><description>Data annotations validation rules are satisfied</description></item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="FreeAgentOptions"/>
/// <seealso cref="IValidateOptions{TOptions}"/>
public class FreeAgentOptionsValidator : IValidateOptions<FreeAgentOptions>
{
    /// <summary>
    /// Validates the specified <see cref="FreeAgentOptions"/> instance.
    /// </summary>
    /// <param name="name">The name of the options instance being validated (can be null for default options).</param>
    /// <param name="options">The options instance to validate.</param>
    /// <returns>A <see cref="ValidateOptionsResult"/> indicating success or failure with error details.</returns>
    /// <remarks>
    /// This method is called automatically by the options framework during application startup
    /// when options are first resolved from the dependency injection container. If validation
    /// fails, the application will fail to start with a clear error message indicating what
    /// configuration is missing or invalid.
    /// </remarks>
    public ValidateOptionsResult Validate(string? name, FreeAgentOptions options)
    {
        if (options == null)
        {
            return ValidateOptionsResult.Fail("FreeAgentOptions cannot be null");
        }

        try
        {
            options.Validate();

            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException ex)
        {
            return ValidateOptionsResult.Fail(ex.Message);
        }
    }
}
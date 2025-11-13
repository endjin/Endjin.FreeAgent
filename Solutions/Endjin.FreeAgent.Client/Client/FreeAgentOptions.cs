// <copyright file="FreeAgentOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Configuration options for the FreeAgent API client.
/// </summary>
/// <remarks>
/// <para>
/// This class contains the OAuth2 credentials required to authenticate with the FreeAgent API.
/// The options can be configured via:
/// <list type="bullet">
/// <item><description>appsettings.json using the "FreeAgent" section</description></item>
/// <item><description>Environment variables</description></item>
/// <item><description>User secrets for development</description></item>
/// <item><description>Azure Key Vault or other secret providers</description></item>
/// <item><description>Programmatically via <see cref="FreeAgentClientServiceCollectionExtensions.AddFreeAgentClientServices(IServiceCollection, Action{FreeAgentOptions})"/></description></item>
/// </list>
/// </para>
/// <para>
/// All properties are validated at startup when using dependency injection through
/// <see cref="FreeAgentOptionsValidator"/>.
/// </para>
/// <example>
/// Example appsettings.json:
/// <code>
/// {
///   "FreeAgent": {
///     "ClientId": "your-oauth2-client-id",
///     "ClientSecret": "your-oauth2-client-secret",
///     "RefreshToken": "your-oauth2-refresh-token"
///   }
/// }
/// </code>
/// </example>
/// </remarks>
/// <seealso cref="FreeAgentOptionsValidator"/>
/// <seealso cref="FreeAgentClient"/>
public class FreeAgentOptions
{
    /// <summary>
    /// Gets the configuration section name for FreeAgent settings.
    /// </summary>
    /// <value>The string "FreeAgent".</value>
    /// <remarks>
    /// This constant is used by <see cref="FreeAgentClientServiceCollectionExtensions.AddFreeAgentClientServices(IServiceCollection, IConfiguration)"/>
    /// to locate the configuration section in appsettings.json.
    /// </remarks>
    public const string SectionName = "FreeAgent";

    /// <summary>
    /// Gets or sets the OAuth2 client ID for the FreeAgent application.
    /// </summary>
    /// <value>The OAuth2 client identifier obtained from the FreeAgent developer portal.</value>
    /// <remarks>
    /// This is the unique identifier for your FreeAgent application. You can obtain this by
    /// registering your application at https://dev.freeagent.com. This value is required for
    /// OAuth2 authentication.
    /// </remarks>
    [Required(ErrorMessage = "ClientId is required for FreeAgent authentication")]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth2 client secret for the FreeAgent application.
    /// </summary>
    /// <value>The OAuth2 client secret obtained from the FreeAgent developer portal.</value>
    /// <remarks>
    /// This is the secret key for your FreeAgent application. Keep this value secure and never
    /// commit it to source control. Use user secrets, environment variables, or a secure vault
    /// for storing this value. This value is required for OAuth2 authentication.
    /// </remarks>
    [Required(ErrorMessage = "ClientSecret is required for FreeAgent authentication")]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the OAuth2 refresh token for the FreeAgent application.
    /// </summary>
    /// <value>The OAuth2 refresh token used to obtain new access tokens.</value>
    /// <remarks>
    /// <para>
    /// The refresh token is used to obtain new access tokens without requiring user interaction.
    /// Access tokens expire after a certain period (typically 1 hour), but the refresh token
    /// remains valid for a longer period and can be used to get new access tokens automatically.
    /// </para>
    /// <para>
    /// You obtain the initial refresh token through the OAuth2 authorization flow. Once obtained,
    /// the refresh token can be used indefinitely (until revoked) to maintain API access.
    /// Keep this value secure like the client secret.
    /// </para>
    /// </remarks>
    [Required(ErrorMessage = "RefreshToken is required for FreeAgent authentication")]
    public string RefreshToken { get; set; } = string.Empty;

    /// <summary>
    /// Validates the configuration options and throws an exception if any validation fails.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when one or more options are invalid or missing.</exception>
    /// <remarks>
    /// This method uses data annotations validation to check all required properties.
    /// It's called automatically when using dependency injection, but can also be called
    /// manually when creating a <see cref="FreeAgentClient"/> directly.
    /// </remarks>
    public void Validate()
    {
        ValidationContext context = new(this);
        List<ValidationResult> results = [];

        if (!Validator.TryValidateObject(this, context, results, validateAllProperties: true))
        {
            string errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new InvalidOperationException($"Invalid FreeAgent configuration: {errors}");
        }
    }
}
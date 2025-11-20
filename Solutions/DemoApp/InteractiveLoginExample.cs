// <copyright file="InteractiveLoginExample.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Client.OAuth2;

using Microsoft.Extensions.Logging;

namespace DemoApp;

/// <summary>
/// Example demonstrating how to use the InteractiveLoginHelper to retrieve OAuth2 tokens.
/// </summary>
public static class InteractiveLoginExample
{
    /// <summary>
    /// Demonstrates the interactive login flow.
    /// </summary>
    /// <param name="clientId">Your FreeAgent OAuth2 client ID.</param>
    /// <param name="clientSecret">Your FreeAgent OAuth2 client secret.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="useSandbox">Whether to use the sandbox environment.</param>
    /// <returns>The login result containing access and refresh tokens.</returns>
    public static async Task<InteractiveLoginResult> PerformInteractiveLoginAsync(
        string clientId,
        string clientSecret,
        ILogger logger,
        bool useSandbox = false)
    {
        Console.WriteLine($"=== FreeAgent Interactive Login ({(useSandbox ? "Sandbox" : "Production")}) ===\n");

        var authBaseUrl = useSandbox ? "https://api.sandbox.freeagent.com" : "https://api.freeagent.com";

        // Configure OAuth2 options
        OAuth2Options options = new()
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            AuthorizationEndpoint = new Uri($"{authBaseUrl}/v2/approve_app"),
            TokenEndpoint = new Uri($"{authBaseUrl}/v2/token_endpoint"),
            UsePkce = true // Enable PKCE for enhanced security
        };

        // Create HTTP client
        using var httpClient = new HttpClient();

        // Create logger for InteractiveLoginHelper
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var loginLogger = loggerFactory.CreateLogger<InteractiveLoginHelper>();

        // Create the interactive login helper
        var loginHelper = new InteractiveLoginHelper(options, httpClient, loginLogger);

        Console.WriteLine("Starting interactive login flow...");
        Console.WriteLine("A browser window will open to authorize the application.");
        Console.WriteLine("After authorizing, you will be redirected back to this application.\n");

        try
        {
            // Perform the interactive login
            // The helper will:
            // 1. Start a local HTTP listener
            // 2. Open the browser to the FreeAgent authorization page
            // 3. Wait for the callback with the authorization code
            // 4. Exchange the code for access and refresh tokens
            InteractiveLoginResult result = await loginHelper.LoginAsync(redirectPort: 5000);

            Console.WriteLine("\n=== Login Successful! ===");
            Console.WriteLine($"\nAccess Token: {result.AccessToken[..20]}...");
            Console.WriteLine($"Refresh Token: {result.RefreshToken}");
            Console.WriteLine($"Token Type: {result.TokenType}");
            Console.WriteLine($"Expires At: {result.ExpiresAt:u}");
            Console.WriteLine($"Expires In: {result.ExpiresInSeconds} seconds");

            Console.WriteLine("\n=== Next Steps ===");
            Console.WriteLine("Save the refresh token securely. You can use it to configure your FreeAgent client:");
            Console.WriteLine("\nOption 1: In appsettings.json:");
            Console.WriteLine("{");
            Console.WriteLine("  \"FreeAgent\": {");
            Console.WriteLine($"    \"ClientId\": \"{clientId}\",");
            Console.WriteLine($"    \"ClientSecret\": \"{clientSecret}\",");
            Console.WriteLine($"    \"RefreshToken\": \"{result.RefreshToken}\"");
            Console.WriteLine("  }");
            Console.WriteLine("}");

            Console.WriteLine("\nOption 2: As environment variables:");
            Console.WriteLine($"export FreeAgent__ClientId=\"{clientId}\"");
            Console.WriteLine($"export FreeAgent__ClientSecret=\"{clientSecret}\"");
            Console.WriteLine($"export FreeAgent__RefreshToken=\"{result.RefreshToken}\"");

            return result;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\nLogin cancelled by user.");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nLogin failed: {ex.Message}");
            logger.LogError(ex, "Interactive login failed");
            throw;
        }
    }
}

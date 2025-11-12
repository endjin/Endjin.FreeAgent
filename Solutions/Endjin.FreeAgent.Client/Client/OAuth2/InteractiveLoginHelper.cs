// <copyright file="InteractiveLoginHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Duende.IdentityModel;
using Duende.IdentityModel.Client;

using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;

namespace Endjin.FreeAgent.Client.OAuth2;

/// <summary>
/// Helper class for performing interactive OAuth2 login to retrieve access and refresh tokens.
/// </summary>
public class InteractiveLoginHelper
{
    private readonly OAuth2Options options;
    private readonly HttpClient httpClient;
    private readonly ILogger<InteractiveLoginHelper> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractiveLoginHelper"/> class.
    /// </summary>
    /// <param name="options">OAuth2 configuration options.</param>
    /// <param name="httpClient">HTTP client for token exchange.</param>
    /// <param name="logger">Logger instance.</param>
    public InteractiveLoginHelper(
        OAuth2Options options,
        HttpClient httpClient,
        ILogger<InteractiveLoginHelper> logger)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Validate required options
        if (string.IsNullOrWhiteSpace(options.ClientId))
        {
            throw new ArgumentException("ClientId is required", nameof(options));
        }

        if (string.IsNullOrWhiteSpace(options.ClientSecret))
        {
            throw new ArgumentException("ClientSecret is required", nameof(options));
        }
    }

    /// <summary>
    /// Performs an interactive login flow to retrieve access and refresh tokens.
    /// This method will:
    /// 1. Start a local HTTP listener on the specified port
    /// 2. Open the browser to the FreeAgent authorization page
    /// 3. Wait for the callback with the authorization code
    /// 4. Exchange the code for access and refresh tokens
    /// </summary>
    /// <param name="redirectPort">The local port to listen on for the OAuth callback. Default is 5000.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="InteractiveLoginResult"/> containing the access token, refresh token, and expiration information.</returns>
    public async Task<InteractiveLoginResult> LoginAsync(
        int redirectPort = 5000,
        CancellationToken cancellationToken = default)
    {
        string redirectUri = $"http://localhost:{redirectPort}/callback";
        
        // Generate PKCE parameters if enabled
        string? codeVerifier = null;
        string? codeChallenge = null;

        if (options.UsePkce)
        {
            codeVerifier = CryptoRandom.CreateUniqueId(32);
            codeChallenge = GenerateCodeChallenge(codeVerifier);
            this.logger.LogDebug("Generated PKCE code verifier and challenge");
        }

        // Build the authorization URL
        string authorizationUrl = BuildAuthorizationUrl(redirectUri, codeChallenge);

        this.logger.LogInformation("Starting interactive login flow on port {Port}", redirectPort);
        this.logger.LogInformation("Authorization URL: {Url}", authorizationUrl);

        // Start local HTTP listener to receive the callback
        using HttpListener listener = new();
        listener.Prefixes.Add($"http://localhost:{redirectPort}/");
        
        try
        {
            listener.Start();
            this.logger.LogDebug("HTTP listener started on port {Port}", redirectPort);
        }
        catch (HttpListenerException ex)
        {
            this.logger.LogError(ex, "Failed to start HTTP listener on port {Port}", redirectPort);
            throw new InvalidOperationException(
                $"Failed to start HTTP listener on port {redirectPort}. " +
                $"Make sure the port is not already in use and you have permission to listen on it.", ex);
        }

        // Open the browser to the authorization URL
        try
        {
            OpenBrowser(authorizationUrl);
            this.logger.LogInformation("Browser opened to authorization URL");
        }
        catch (Exception ex)
        {
            this.logger.LogWarning(ex, "Failed to open browser automatically. Please navigate to: {Url}", authorizationUrl);
            Console.WriteLine($"\nPlease open your browser and navigate to:\n{authorizationUrl}\n");
        }

        // Wait for the callback
        this.logger.LogInformation("Waiting for authorization callback...");
        Console.WriteLine("\nWaiting for authorization callback from FreeAgent...");

        HttpListenerContext context;
        try
        {
            context = await listener.GetContextAsync().WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            this.logger.LogWarning("Authorization callback wait was cancelled");
            throw;
        }

        // Extract the authorization code from the callback
        string? code = null;
        string? error = null;

        try
        {
            string? query = context.Request.Url?.Query;
            if (!string.IsNullOrEmpty(query))
            {
                var queryParams = HttpUtility.ParseQueryString(query);
                code = queryParams["code"];
                error = queryParams["error"];
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to parse callback URL");
        }

        // Send response to browser
        await SendCallbackResponseAsync(context.Response, code, error);

        // Check for errors
        if (!string.IsNullOrEmpty(error))
        {
            this.logger.LogError("Authorization failed with error: {Error}", error);
            throw new InvalidOperationException($"Authorization failed: {error}");
        }

        if (string.IsNullOrEmpty(code))
        {
            this.logger.LogError("No authorization code received in callback");
            throw new InvalidOperationException("No authorization code received from FreeAgent");
        }

        this.logger.LogInformation("Received authorization code, exchanging for tokens");
        Console.WriteLine("\nAuthorization successful! Exchanging code for tokens...");

        // Exchange the authorization code for tokens
        TokenResponse tokenResponse = await ExchangeCodeForTokensAsync(code, redirectUri, codeVerifier, cancellationToken);

        if (tokenResponse.IsError)
        {
            this.logger.LogError("Token exchange failed: {Error} - {ErrorDescription}", 
                tokenResponse.Error, tokenResponse.ErrorDescription);
            throw new InvalidOperationException(
                $"Failed to exchange authorization code for tokens: {tokenResponse.Error} - {tokenResponse.ErrorDescription}");
        }

        this.logger.LogInformation("Token exchange successful");
        Console.WriteLine("Login successful! Tokens retrieved.\n");

        // Calculate expiration time
        DateTime expiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn > 0 ? tokenResponse.ExpiresIn : 3600);

        return new InteractiveLoginResult
        {
            AccessToken = tokenResponse.AccessToken ?? throw new InvalidOperationException("No access token in response"),
            RefreshToken = tokenResponse.RefreshToken ?? throw new InvalidOperationException("No refresh token in response"),
            ExpiresAt = expiresAt,
            ExpiresInSeconds = tokenResponse.ExpiresIn,
            TokenType = tokenResponse.TokenType ?? "Bearer"
        };
    }

    private string BuildAuthorizationUrl(string redirectUri, string? codeChallenge)
    {
        var queryParams = new Dictionary<string, string>
        {
            { "response_type", "code" },
            { "client_id", options.ClientId },
            { "redirect_uri", redirectUri }
        };

        if (!string.IsNullOrEmpty(options.Scope))
        {
            queryParams["scope"] = options.Scope;
        }

        if (!string.IsNullOrEmpty(codeChallenge))
        {
            queryParams["code_challenge"] = codeChallenge;
            queryParams["code_challenge_method"] = "S256";
        }

        var queryString = string.Join("&", queryParams.Select(kvp => 
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{options.AuthorizationEndpoint}?{queryString}";
    }

    private async Task<TokenResponse> ExchangeCodeForTokensAsync(
        string code,
        string redirectUri,
        string? codeVerifier,
        CancellationToken cancellationToken)
    {
        var tokenClient = new TokenClient(httpClient, new TokenClientOptions
        {
            Address = options.TokenEndpoint.ToString(),
            ClientId = options.ClientId,
            ClientSecret = options.ClientSecret,
        });

        return await tokenClient.RequestAuthorizationCodeTokenAsync(
            code,
            redirectUri,
            codeVerifier: codeVerifier,
            cancellationToken: cancellationToken);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Base64Url.Encode(hash);
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            // Try to open the browser using the default application
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // If the above fails, try platform-specific approaches
            if (OperatingSystem.IsWindows())
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", url);
            }
            else if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }

    private async Task SendCallbackResponseAsync(HttpListenerResponse response, string? code, string? error)
    {
        response.ContentType = "text/html";
        response.StatusCode = 200;

        string htmlResponse;
        
        if (!string.IsNullOrEmpty(error))
        {
            htmlResponse = $@"
<!DOCTYPE html>
<html>
<head>
    <title>FreeAgent Authorization Failed</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; background-color: #f5f5f5; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .error {{ color: #d32f2f; }}
        h1 {{ color: #333; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1 class='error'>❌ Authorization Failed</h1>
        <p>An error occurred during authorization: <strong>{HttpUtility.HtmlEncode(error)}</strong></p>
        <p>Please check the console for more details and try again.</p>
        <p>You can close this window.</p>
    </div>
</body>
</html>";
        }
        else if (!string.IsNullOrEmpty(code))
        {
            htmlResponse = @"
<!DOCTYPE html>
<html>
<head>
    <title>FreeAgent Authorization Successful</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background-color: #f5f5f5; }
        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .success { color: #2e7d32; }
        h1 { color: #333; }
    </style>
</head>
<body>
    <div class='container'>
        <h1 class='success'>✅ Authorization Successful!</h1>
        <p>You have successfully authorized the FreeAgent application.</p>
        <p>Your access and refresh tokens are being retrieved...</p>
        <p>You can close this window and return to the application.</p>
    </div>
</body>
</html>";
        }
        else
        {
            htmlResponse = @"
<!DOCTYPE html>
<html>
<head>
    <title>FreeAgent Authorization</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; background-color: #f5f5f5; }
        .container { max-width: 600px; margin: 0 auto; background-color: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .warning { color: #f57c00; }
        h1 { color: #333; }
    </style>
</head>
<body>
    <div class='container'>
        <h1 class='warning'>⚠️ Unexpected Response</h1>
        <p>No authorization code was received in the callback.</p>
        <p>Please check the console for more details and try again.</p>
        <p>You can close this window.</p>
    </div>
</body>
</html>";
        }

        byte[] buffer = Encoding.UTF8.GetBytes(htmlResponse);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }
}

/// <summary>
/// Result of an interactive login operation.
/// </summary>
public class InteractiveLoginResult
{
    /// <summary>
    /// Gets or sets the access token.
    /// </summary>
    public required string AccessToken { get; init; }

    /// <summary>
    /// Gets or sets the refresh token.
    /// </summary>
    public required string RefreshToken { get; init; }

    /// <summary>
    /// Gets or sets when the access token expires (UTC).
    /// </summary>
    public required DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Gets or sets the number of seconds until the token expires.
    /// </summary>
    public required int ExpiresInSeconds { get; init; }

    /// <summary>
    /// Gets or sets the token type (typically "Bearer").
    /// </summary>
    public required string TokenType { get; init; }
}

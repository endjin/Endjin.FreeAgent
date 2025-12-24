// <copyright file="InteractiveLoginHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Specialized;

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
public partial class InteractiveLoginHelper
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

        if (this.options.UsePkce)
        {
            codeVerifier = CryptoRandom.CreateUniqueId(32);
            codeChallenge = GenerateCodeChallenge(codeVerifier);
            this.logger.LogDebug("Generated PKCE code verifier and challenge");
        }

        // Build the authorization URL
        string authorizationUrl = this.BuildAuthorizationUrl(redirectUri, codeChallenge);

        this.LogStartingLogin(redirectPort);
        this.LogAuthorizationUrl(authorizationUrl);

        // Start local HTTP listener to receive the callback
        using HttpListener listener = new() { Prefixes = { $"http://localhost:{redirectPort}/" } };
        
        try
        {
            listener.Start();
            this.LogHttpListenerStarted(redirectPort);
        }
        catch (HttpListenerException ex)
        {
            this.LogHttpListenerFailed(ex, redirectPort);
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
            this.LogBrowserOpenFailed(ex, authorizationUrl);
            Console.WriteLine($"\nPlease open your browser and navigate to:\n{authorizationUrl}\n");
        }

        // Wait for the callback
        this.logger.LogInformation("Waiting for authorization callback...");
        Console.WriteLine("\nWaiting for authorization callback from FreeAgent...");

        HttpListenerContext context;
        try
        {
            context = await listener.GetContextAsync().WaitAsync(cancellationToken).ConfigureAwait(false);
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
                NameValueCollection queryParams = HttpUtility.ParseQueryString(query);
                code = queryParams["code"];
                error = queryParams["error"];
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to parse callback URL");
        }

        // Send response to browser
        await SendCallbackResponseAsync(context.Response, code, error).ConfigureAwait(false);

        // Check for errors
        if (!string.IsNullOrEmpty(error))
        {
            this.LogAuthorizationFailed(error);
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
        TokenResponse tokenResponse = await this.ExchangeCodeForTokensAsync(code, redirectUri, codeVerifier, cancellationToken).ConfigureAwait(false);

        if (tokenResponse.IsError)
        {
            this.LogTokenExchangeFailed(tokenResponse.Error, tokenResponse.ErrorDescription);
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
        Dictionary<string, string> queryParams = new()
        {
            { "response_type", "code" },
            { "client_id", this.options.ClientId },
            { "redirect_uri", redirectUri }
        };

        if (!string.IsNullOrEmpty(this.options.Scope))
        {
            queryParams["scope"] = this.options.Scope;
        }

        if (!string.IsNullOrEmpty(codeChallenge))
        {
            queryParams["code_challenge"] = codeChallenge;
            queryParams["code_challenge_method"] = "S256";
        }

        string queryString = string.Join('&', queryParams.Select(kvp => 
            $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

        return $"{this.options.AuthorizationEndpoint}?{queryString}";
    }

    private async Task<TokenResponse> ExchangeCodeForTokensAsync(
        string code,
        string redirectUri,
        string? codeVerifier,
        CancellationToken cancellationToken)
    {
        TokenClient tokenClient = new(this.httpClient, new TokenClientOptions
        {
            Address = this.options.TokenEndpoint.ToString(),
            ClientId = this.options.ClientId,
            ClientSecret = this.options.ClientSecret,
        });

        return await tokenClient.RequestAuthorizationCodeTokenAsync(
            code,
            redirectUri,
            codeVerifier: codeVerifier,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
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

    private static async Task SendCallbackResponseAsync(HttpListenerResponse response, string? code, string? error)
    {
        response.ContentType = "text/html";
        response.StatusCode = 200;

        string htmlResponse = !string.IsNullOrEmpty(error)
            ? $@"
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
</html>"
            : !string.IsNullOrEmpty(code)
                ? @"
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
</html>"
                : @"
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

        byte[] buffer = Encoding.UTF8.GetBytes(htmlResponse);
        response.ContentLength64 = buffer.Length;
        await response.OutputStream.WriteAsync(buffer).ConfigureAwait(false);
        response.OutputStream.Close();
    }

    [LoggerMessage(1, LogLevel.Information, "Starting interactive login flow on port {Port}")]
    private partial void LogStartingLogin(int port);

    [LoggerMessage(2, LogLevel.Information, "Authorization URL: {Url}")]
    private partial void LogAuthorizationUrl(string url);

    [LoggerMessage(3, LogLevel.Debug, "HTTP listener started on port {Port}")]
    private partial void LogHttpListenerStarted(int port);

    [LoggerMessage(4, LogLevel.Error, "Failed to start HTTP listener on port {Port}")]
    private partial void LogHttpListenerFailed(Exception ex, int port);

    [LoggerMessage(5, LogLevel.Warning, "Failed to open browser automatically. Please navigate to: {Url}")]
    private partial void LogBrowserOpenFailed(Exception ex, string url);

    [LoggerMessage(6, LogLevel.Error, "Authorization failed with error: {Error}")]
    private partial void LogAuthorizationFailed(string error);

    [LoggerMessage(7, LogLevel.Error, "Token exchange failed: {Error} - {ErrorDescription}")]
    private partial void LogTokenExchangeFailed(string? error, string? errorDescription);
}

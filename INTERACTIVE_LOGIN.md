# Interactive Login Quick Start

This guide shows you how to use the `InteractiveLoginHelper` to obtain OAuth2 tokens for FreeAgent.

## Prerequisites

1. A FreeAgent account
2. OAuth2 credentials (Client ID and Client Secret) from FreeAgent
   - Sign in to FreeAgent
   - Go to Settings → Developer
   - Create a new OAuth2 application
   - Save your Client ID and Client Secret

## Option 1: Using the DemoApp

The easiest way to get your tokens is to use the DemoApp:

1. Update `Solutions/DemoApp/appsettings.json` with your credentials:

```json
{
  "FreeAgent": {
    "ClientId": "your-client-id-here",
    "ClientSecret": "your-client-secret-here"
  }
}
```

2. Run the DemoApp in interactive login mode:

```bash
cd Solutions/DemoApp
dotnet run -- --interactive-login
```

3. The application will:
   - Open your browser to authorize the application
   - Wait for you to approve the authorization
   - Automatically retrieve your tokens
   - Display them in the console

4. Copy the **Refresh Token** from the output and save it securely.

5. Update your `appsettings.json` with the refresh token:

```json
{
  "FreeAgent": {
    "ClientId": "your-client-id-here",
    "ClientSecret": "your-client-secret-here",
    "RefreshToken": "your-refresh-token-here"
  }
}
```

## Option 2: Using the InteractiveLoginHelper in Your Code

If you want to integrate interactive login into your own application:

```csharp
using Endjin.FreeAgent.Client.OAuth2;
using Microsoft.Extensions.Logging;

// Configure OAuth2 options
OAuth2Options options = new()
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    UsePkce = true // Enable PKCE for enhanced security
};

// Create HTTP client and logger
using var httpClient = new HttpClient();
using var loggerFactory = LoggerFactory.Create(builder => 
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

var logger = loggerFactory.CreateLogger<InteractiveLoginHelper>();

// Create the interactive login helper
var loginHelper = new InteractiveLoginHelper(options, httpClient, logger);

// Perform interactive login
// This will open a browser window for authorization
InteractiveLoginResult result = await loginHelper.LoginAsync(redirectPort: 5000);

// Display the tokens
Console.WriteLine($"Access Token: {result.AccessToken}");
Console.WriteLine($"Refresh Token: {result.RefreshToken}");
Console.WriteLine($"Expires At: {result.ExpiresAt}");

// Save the refresh token for future use
// You can store it in:
// - appsettings.json
// - Environment variables
// - Secure key storage (recommended for production)
```

## What Happens During Interactive Login?

1. **Local Server Starts**: A temporary HTTP server starts on `localhost:5000` (or your chosen port)

2. **Browser Opens**: Your default browser opens to the FreeAgent authorization page

3. **User Authorization**: You log in to FreeAgent and approve the application

4. **Callback**: FreeAgent redirects back to `localhost:5000/callback` with an authorization code

5. **Token Exchange**: The helper automatically exchanges the authorization code for tokens

6. **Tokens Returned**: You receive both an access token and a refresh token

7. **Server Stops**: The local HTTP server shuts down

## Security Notes

- **Refresh Token**: This is the most important token. Keep it secure! Anyone with this token can access your FreeAgent account.

- **PKCE**: The helper uses PKCE (Proof Key for Code Exchange) by default for enhanced security. This is recommended for all OAuth2 flows.

- **Local Listener**: The HTTP listener only accepts connections from localhost, so it's safe to use on your local machine.

## Troubleshooting

### Port Already in Use

If port 5000 is already in use, you can specify a different port:

```csharp
InteractiveLoginResult result = await loginHelper.LoginAsync(redirectPort: 5001);
```

Or when using the DemoApp, the port is configurable in the code.

### Browser Doesn't Open

If the browser doesn't open automatically, copy the URL from the console and paste it into your browser manually.

### Permission Denied

On some systems, you may need administrator/root privileges to listen on certain ports. Try using a port number above 1024 (e.g., 5000, 8080).

### Firewall Blocking

Ensure your firewall allows localhost connections on the chosen port.

## Next Steps

Once you have your refresh token:

1. Store it securely in your application configuration
2. Use it to create a `FreeAgentClient` instance
3. The client will automatically refresh the access token as needed

```csharp
FreeAgentClient client = new(
    clientId: "your-client-id",
    clientSecret: "your-client-secret",
    refreshToken: "your-refresh-token",
    cache: cache,
    httpClientFactory: httpClientFactory,
    loggerFactory: loggerFactory);

await client.InitializeAndAuthorizeAsync();

// Now you can use the client
var contacts = await client.Contacts.GetAllAsync();
```

## Support

For issues or questions:
- 🐛 [Report an Issue](https://github.com/endjin/Endjin.FreeAgent/issues)
- 💬 [Discussions](https://github.com/endjin/Endjin.FreeAgent/discussions)

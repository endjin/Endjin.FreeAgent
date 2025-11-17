# FreeAgentClient Mocking Guide

## Overview

The FreeAgentClient has been modified to support mocking of HTTP calls for testing purposes. The key changes introduce an abstraction layer between the client code and the actual HTTP operations.

## Architecture Changes

### 1. IHttpService Interface
A new `IHttpService` interface has been introduced that abstracts HTTP operations:
- `GetAsync()` - Sends GET requests
- `PostAsync()` - Sends POST requests
- `PutAsync()` - Sends PUT requests
- `DeleteAsync()` - Sends DELETE requests
- `SetAuthorizationHeader()` - Sets authorization headers
- `ClearAuthorizationHeader()` - Clears authorization headers

### 2. HttpService Implementation
The `HttpService` class is the default production implementation that wraps an actual `HttpClient`.

### 3. MockHttpService for Testing
The `MockHttpService` class in the `Testing` namespace provides a complete mocking solution:
- Queue responses to be returned
- Capture and inspect requests
- Verify authorization headers
- Support complex scenarios like pagination and token refresh

## Usage Examples

### 1. Creating a Client with Mock Services

```csharp
// Create mock services
var mockHttpService = new MockHttpService();
var mockHttpServiceNoAuth = new MockHttpService();

// Create client with mocks
var options = new FreeAgentOptions
{
    ClientId = "test_client_id",
    ClientSecret = "test_client_secret",
    RefreshToken = "test_refresh_token"
};

var client = new FreeAgentClient(
    options,
    cache,
    mockHttpService,
    mockHttpServiceNoAuth);
```

### 2. Setting Up Mock Responses

```csharp
// Queue a simple response
mockHttpService.EnqueueResponse(
    JsonSerializer.Serialize(responseObject),
    HttpStatusCode.OK);

// Queue a response with headers (for pagination)
var response = new MockHttpResponse
{
    StatusCode = HttpStatusCode.OK,
    Content = new StringContent(jsonContent),
    Headers = new Dictionary<string, string>
    {
        ["Link"] = "<https://api.example.com/page2>; rel='next'"
    }
};
mockHttpService.EnqueueResponse(response);
```

### 3. Verifying Requests

```csharp
// Check captured requests
Assert.AreEqual(2, mockHttpService.CapturedRequests.Count);

var firstRequest = mockHttpService.CapturedRequests[0];
Assert.AreEqual("https://api.freeagent.com/v2/contacts",
    firstRequest.RequestUri?.ToString());
Assert.AreEqual(HttpMethod.Post, firstRequest.Method);

// Verify authorization
Assert.AreEqual("Bearer", mockHttpService.AuthorizationScheme);
Assert.AreEqual("test_token", mockHttpService.AuthorizationParameter);
```

### 4. Testing Token Refresh

```csharp
// First response returns 401 Unauthorized
mockHttpService.EnqueueResponse("", HttpStatusCode.Unauthorized);

// Token refresh endpoint returns new token
mockHttpServiceNoAuth.EnqueueResponse(
    JsonSerializer.Serialize(newTokenResponse),
    HttpStatusCode.OK);

// Retry with new token succeeds
mockHttpService.EnqueueResponse(
    JsonSerializer.Serialize(successResponse),
    HttpStatusCode.OK);

// Make the request - client will automatically refresh token
var result = await client.ExecuteRequestAndFollowLinksAsync<ContactList>(uri);
```

## Backward Compatibility

The changes maintain full backward compatibility:

1. **Existing constructors still work** - The original `FreeAgentClient` constructors that don't take `IHttpService` parameters continue to function as before.

2. **Automatic initialization** - If HTTP services aren't injected, they are automatically created using the legacy `HttpClient` instances.

3. **Dual support** - The `ClientBase` class checks for both the new `IHttpService` instances and falls back to legacy `HttpClient` if needed.

## Dependency Injection

For ASP.NET Core applications, the service registration has been updated:

```csharp
services.AddFreeAgentClientServices(configuration);
```

This will automatically:
- Register the FreeAgentClient as a singleton
- Set up HTTP services if available
- Use the appropriate constructor based on registered services

## Testing Best Practices

1. **Always verify all responses consumed**:
```csharp
Assert.IsTrue(mockHttpService.VerifyAllResponsesConsumed());
```

2. **Clear captured requests between tests**:
```csharp
mockHttpService.ClearCapturedRequests();
```

3. **Use realistic test data** - Mock responses should match the actual API structure

4. **Test error scenarios** - Include tests for:
   - Network errors
   - Authentication failures
   - Rate limiting
   - Malformed responses

## Migration Path

For existing code:
1. No changes required - existing code continues to work
2. To add testing, use the new constructor overloads with mock services
3. Gradually migrate integration tests to use mocks for faster, more reliable testing

## Benefits

- **Faster tests** - No network calls required
- **Deterministic** - Tests produce consistent results
- **Comprehensive** - Can test error conditions and edge cases
- **Isolated** - Tests don't depend on external services
- **Debuggable** - Can inspect exact requests and responses
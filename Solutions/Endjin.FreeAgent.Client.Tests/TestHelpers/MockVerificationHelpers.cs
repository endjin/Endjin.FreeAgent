// <copyright file="MockVerificationHelpers.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Collections.Specialized;
using System.Net;
using System.Text.Json;

namespace Endjin.FreeAgent.Client.Tests.TestHelpers;

/// <summary>
/// Helper methods for verifying mock behavior in tests.
/// </summary>
public static class MockVerificationHelpers
{
    /// <summary>
    /// Verifies that the handler was called with the expected URI.
    /// </summary>
    public static void ShouldHaveBeenCalledWithUri(this TestHttpMessageHandler handler, string expectedUri)
    {
        handler.LastRequest.ShouldNotBeNull($"Expected HTTP request to {expectedUri} but no request was made");
        handler.LastRequest!.RequestUri?.ToString().ShouldContain(expectedUri);
    }

    /// <summary>
    /// Verifies that the handler was called with the exact URI.
    /// </summary>
    public static void ShouldHaveBeenCalledWithExactUri(this TestHttpMessageHandler handler, string expectedUri)
    {
        handler.LastRequest.ShouldNotBeNull($"Expected HTTP request to {expectedUri} but no request was made");
        handler.LastRequest!.RequestUri?.ToString().ShouldBe(expectedUri,
            $"Expected request URI to be '{expectedUri}' but was '{handler.LastRequest.RequestUri}'");
    }

    /// <summary>
    /// Verifies that the handler was called with the expected HTTP method.
    /// </summary>
    public static void ShouldHaveBeenCalledWithMethod(this TestHttpMessageHandler handler, HttpMethod expectedMethod)
    {
        handler.LastRequest.ShouldNotBeNull($"Expected {expectedMethod} request but no request was made");
        handler.LastRequest!.Method.ShouldBe(expectedMethod,
            $"Expected {expectedMethod} request but was {handler.LastRequest.Method}");
    }

    /// <summary>
    /// Verifies that the handler was called with a GET request.
    /// </summary>
    public static void ShouldHaveBeenGetRequest(this TestHttpMessageHandler handler)
    {
        handler.ShouldHaveBeenCalledWithMethod(HttpMethod.Get);
    }

    /// <summary>
    /// Verifies that the handler was called with a POST request.
    /// </summary>
    public static void ShouldHaveBeenPostRequest(this TestHttpMessageHandler handler)
    {
        handler.ShouldHaveBeenCalledWithMethod(HttpMethod.Post);
    }

    /// <summary>
    /// Verifies that the handler was called with a PUT request.
    /// </summary>
    public static void ShouldHaveBeenPutRequest(this TestHttpMessageHandler handler)
    {
        handler.ShouldHaveBeenCalledWithMethod(HttpMethod.Put);
    }

    /// <summary>
    /// Verifies that the handler was called with a DELETE request.
    /// </summary>
    public static void ShouldHaveBeenDeleteRequest(this TestHttpMessageHandler handler)
    {
        handler.ShouldHaveBeenCalledWithMethod(HttpMethod.Delete);
    }

    /// <summary>
    /// Verifies that the request contains the expected header.
    /// </summary>
    public static void ShouldHaveHeader(this TestHttpMessageHandler handler, string headerName, string expectedValue)
    {
        handler.LastRequest.ShouldNotBeNull($"Expected request with header '{headerName}' but no request was made");

        bool headerExists = handler.LastRequest!.Headers.TryGetValues(headerName, out IEnumerable<string>? values) ||
                            (handler.LastRequest.Content?.Headers.TryGetValues(headerName, out values) ?? false);

        headerExists.ShouldBeTrue($"Expected header '{headerName}' to be present in request");
        values?.FirstOrDefault().ShouldBe(expectedValue,
            $"Expected header '{headerName}' to have value '{expectedValue}' but was '{values?.FirstOrDefault()}'");
    }

    /// <summary>
    /// Verifies that the request has JSON content type.
    /// </summary>
    public static void ShouldHaveJsonContentType(this TestHttpMessageHandler handler)
    {
        handler.LastRequest.ShouldNotBeNull("Expected request with JSON content but no request was made");
        handler.LastRequest!.Content.ShouldNotBeNull("Expected request to have content");

        string? contentType = handler.LastRequest.Content!.Headers.ContentType?.MediaType;
        contentType.ShouldBe("application/json",
            $"Expected content type 'application/json' but was '{contentType}'");
    }

    /// <summary>
    /// Verifies the JSON body of the request and allows assertions on the deserialized object.
    /// </summary>
    public static async Task ShouldHaveJsonBody<T>(this TestHttpMessageHandler handler, Action<T> assertions) where T : class
    {
        handler.LastRequest.ShouldNotBeNull("Expected request with JSON body but no request was made");
        handler.LastRequest!.Content.ShouldNotBeNull("Expected request to have content");

        string content = await handler.LastRequest.Content!.ReadAsStringAsync();
        content.ShouldNotBeNullOrWhiteSpace("Expected request to have non-empty content");

        T? deserialized = JsonSerializer.Deserialize<T>(content, SharedJsonOptions.Instance);
        deserialized.ShouldNotBeNull($"Failed to deserialize request body as {typeof(T).Name}");

        assertions(deserialized!);
    }

    /// <summary>
    /// Verifies that the handler was called exactly the specified number of times.
    /// </summary>
    public static void ShouldHaveBeenCalledTimes(this TestHttpMessageHandler handler, int expectedCount)
    {
        handler.CallCount.ShouldBe(expectedCount,
            $"Expected {expectedCount} HTTP call(s) but was called {handler.CallCount} time(s)");
    }

    /// <summary>
    /// Verifies that the handler was called exactly once.
    /// </summary>
    public static void ShouldHaveBeenCalledOnce(this TestHttpMessageHandler handler)
    {
        handler.ShouldHaveBeenCalledTimes(1);
    }

    /// <summary>
    /// Verifies that the handler was never called.
    /// </summary>
    public static void ShouldNotHaveBeenCalled(this TestHttpMessageHandler handler)
    {
        handler.ShouldHaveBeenCalledTimes(0);
    }

    /// <summary>
    /// Verifies that the request contains a specific query parameter.
    /// </summary>
    public static void ShouldHaveQueryParameter(this TestHttpMessageHandler handler, string paramName, string expectedValue)
    {
        handler.LastRequest.ShouldNotBeNull($"Expected request with query parameter '{paramName}' but no request was made");

        string? query = handler.LastRequest!.RequestUri?.Query;
        query.ShouldNotBeNullOrEmpty($"Expected query string with parameter '{paramName}' but query was empty");

        NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(query!);
        string? actualValue = queryParams[paramName];

        actualValue.ShouldNotBeNull($"Expected query parameter '{paramName}' to be present");
        actualValue.ShouldBe(expectedValue,
            $"Expected query parameter '{paramName}' to be '{expectedValue}' but was '{actualValue}'");
    }

    /// <summary>
    /// Gets a specific request by index for multi-request scenarios.
    /// </summary>
    public static HttpRequestMessage? GetRequest(this TestHttpMessageHandler handler, int index)
    {
        return index < handler.ReceivedRequests.Count ? handler.ReceivedRequests[index] : null;
    }

    /// <summary>
    /// Verifies that all requests match a predicate.
    /// </summary>
    public static void AllRequestsShouldMatch(this TestHttpMessageHandler handler, Func<HttpRequestMessage, bool> predicate, string description)
    {
        handler.ReceivedRequests.Count.ShouldBeGreaterThan(0, "Expected at least one request but none were made");

        bool allMatch = handler.ReceivedRequests.All(predicate);
        allMatch.ShouldBeTrue($"Not all requests matched: {description}");
    }
}

/// <summary>
/// Enhanced test HTTP message handler that captures request details for verification.
/// </summary>
public class TestHttpMessageHandler : HttpMessageHandler
{
    private readonly List<HttpRequestMessage> receivedRequests = [];
    private readonly Dictionary<string, Func<HttpResponseMessage>> responseFactories = new();
    private Func<HttpResponseMessage>? defaultResponseFactory;

    /// <summary>
    /// Gets or sets the default response to return for all requests.
    /// </summary>
    public HttpResponseMessage Response
    {
        get => this.defaultResponseFactory?.Invoke() ?? new HttpResponseMessage(HttpStatusCode.OK);
        set => this.defaultResponseFactory = () => CloneResponse(value);
    }

    /// <summary>
    /// Gets the total number of calls made to this handler.
    /// </summary>
    public int CallCount { get; private set; } = 0;

    /// <summary>
    /// Gets all received requests.
    /// </summary>
    public IReadOnlyList<HttpRequestMessage> ReceivedRequests => this.receivedRequests.AsReadOnly();

    /// <summary>
    /// Gets the last request received, or null if no requests were made.
    /// </summary>
    public HttpRequestMessage? LastRequest => this.receivedRequests.LastOrDefault();

    /// <summary>
    /// Sets up a response for a specific URI pattern.
    /// </summary>
    public void SetupResponse(string uriPattern, HttpResponseMessage response)
    {
        this.responseFactories[uriPattern] = () => CloneResponse(response);
    }

    /// <summary>
    /// Clears all captured requests.
    /// </summary>
    public void Reset()
    {
        this.CallCount = 0;
        this.receivedRequests.Clear();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        this.CallCount++;

        // Clone the request to preserve it (including content)
        HttpRequestMessage clonedRequest = await CloneRequestAsync(request);
        this.receivedRequests.Add(clonedRequest);

        // Find matching response factory or use default
        Func<HttpResponseMessage>? responseFactory = FindMatchingResponseFactory(request) ?? this.defaultResponseFactory;
        return responseFactory?.Invoke() ?? new HttpResponseMessage(HttpStatusCode.OK);
    }

    private Func<HttpResponseMessage>? FindMatchingResponseFactory(HttpRequestMessage request)
    {
        string uri = request.RequestUri?.ToString() ?? string.Empty;

        foreach (KeyValuePair<string, Func<HttpResponseMessage>> kvp in this.responseFactories)
        {
            if (uri.Contains(kvp.Key))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        HttpRequestMessage clone = new(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        // Copy headers
        foreach (KeyValuePair<string, IEnumerable<string>> header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content if present
        if (request.Content != null)
        {
            byte[] contentBytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(contentBytes);

            // Copy content headers
            foreach (KeyValuePair<string, IEnumerable<string>> header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Copy properties
        foreach (KeyValuePair<string, object?> prop in request.Options)
        {
            clone.Options.Set(new HttpRequestOptionsKey<object?>(prop.Key), prop.Value);
        }

        return clone;
    }

    private static HttpResponseMessage CloneResponse(HttpResponseMessage original)
    {
        HttpResponseMessage clone = new(original.StatusCode)
        {
            Version = original.Version,
            ReasonPhrase = original.ReasonPhrase
        };

        // Clone headers
        foreach (KeyValuePair<string, IEnumerable<string>> header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Clone content if present
        if (original.Content != null)
        {
            // Read content synchronously to avoid async issues in property getter
            byte[] contentBytes = original.Content.ReadAsByteArrayAsync().Result;
            clone.Content = new ByteArrayContent(contentBytes);

            // Copy content headers
            foreach (KeyValuePair<string, IEnumerable<string>> header in original.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}

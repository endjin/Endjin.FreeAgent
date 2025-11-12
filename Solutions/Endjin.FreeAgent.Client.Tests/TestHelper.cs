// <copyright file="TestHelper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Reflection;

namespace Endjin.FreeAgent.Client.Tests;

internal static class TestHelper
{
    public static async Task SetupForTestingAsync(FreeAgentClient freeAgentClient, IHttpClientFactory httpClientFactory)
    {
        // Set up a test OAuth2Service
        TestOAuth2Service testOAuth2Service = new("test-token");
        freeAgentClient.SetOAuth2Service(testOAuth2Service);

        // Now we can call the real initialization which will use the factory
        await freeAgentClient.InitializeAndAuthorizeAsync();
    }

    // Keep the old method for backward compatibility if needed
    public static void SetupHttpClient(FreeAgentClient freeAgentClient, HttpClient httpClient)
    {
        // Use reflection to set the internal HttpClient property in the base class
        Type? baseType = freeAgentClient.GetType().BaseType; // ClientBase

        // HttpClient and HttpClientNoAuthHeader are properties
        PropertyInfo? httpClientProperty = baseType?.GetProperty("HttpClient", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (httpClientProperty != null)
        {
            httpClientProperty.SetValue(freeAgentClient, httpClient);
        }

        PropertyInfo? httpClientNoAuthProperty = baseType?.GetProperty("HttpClientNoAuthHeader", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        if (httpClientNoAuthProperty != null)
        {
            httpClientNoAuthProperty.SetValue(freeAgentClient, httpClient);
        }

        // ApiBaseUrl is a readonly field, not a property - it's already initialized in ClientBase
        // No need to set it as it has a default value of https://api.freeagent.com

        // Mark as initialized so we can use the client without calling InitializeAndAuthorizeAsync
        PropertyInfo? isInitializedProperty = baseType?.GetProperty("IsInitialized", BindingFlags.Instance | BindingFlags.Public);
        if (isInitializedProperty != null)
        {
            isInitializedProperty.SetValue(freeAgentClient, true);
        }
    }
}

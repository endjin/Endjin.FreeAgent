// <copyright file="SharedJsonOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides shared, thread-safe JsonSerializerOptions instances for the entire application.
/// This improves performance by avoiding multiple instances and enables consistent serialization behavior.
/// Supports both reflection-based and source-generated serialization.
/// </summary>
public static class SharedJsonOptions
{
    private static readonly FreeAgentJsonContext s_sourceGenContext = new();

    /// <summary>
    /// Gets the shared JsonSerializerOptions instance configured for web defaults.
    /// Uses reflection-based serialization for compatibility.
    /// </summary>
    public static JsonSerializerOptions Instance { get; } = CreateOptions();

    /// <summary>
    /// Gets the shared JsonSerializerOptions instance with source generation support.
    /// Provides better performance and AOT compilation support.
    /// </summary>
    public static JsonSerializerOptions SourceGenOptions { get; } = CreateSourceGenOptions();

    /// <summary>
    /// Gets the source generation context for direct use with JsonSerializer methods.
    /// </summary>
    public static FreeAgentJsonContext Context => s_sourceGenContext;

    private static JsonSerializerOptions CreateOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        // Add any custom converters here if needed
        // options.Converters.Add(new CustomConverter());

        return options;
    }

    private static JsonSerializerOptions CreateSourceGenOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            TypeInfoResolver = s_sourceGenContext
        };

        return options;
    }
}

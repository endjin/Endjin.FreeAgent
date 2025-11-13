// <copyright file="SharedJsonOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Provides shared, thread-safe <see cref="JsonSerializerOptions"/> instances for consistent JSON serialization throughout the library.
/// </summary>
/// <remarks>
/// <para>
/// This static class provides centralized JSON serialization configuration for the FreeAgent client library.
/// Using shared options instances provides several important benefits:
/// <list type="bullet">
/// <item><description>Performance - Reusing options instances avoids the overhead of creating and configuring new instances</description></item>
/// <item><description>Consistency - All serialization uses the same settings (camelCase, ignore nulls, etc.)</description></item>
/// <item><description>Memory efficiency - A single instance is shared across all operations</description></item>
/// <item><description>Thread safety - The instances are immutable after creation</description></item>
/// </list>
/// </para>
/// <para>
/// The class provides two options configurations:
/// <list type="bullet">
/// <item><description><see cref="Instance"/> - Reflection-based serialization for maximum compatibility</description></item>
/// <item><description><see cref="SourceGenOptions"/> - Source-generated serialization for optimal performance and AOT support</description></item>
/// </list>
/// </para>
/// <para>
/// Both configurations use web defaults with the following customizations:
/// <list type="bullet">
/// <item><description>Property naming policy: camelCase (to match FreeAgent API conventions)</description></item>
/// <item><description>Null value handling: Ignore null values when writing JSON</description></item>
/// <item><description>Case sensitivity: Case-insensitive property name matching when reading JSON</description></item>
/// <item><description>Trailing commas: Allow trailing commas in JSON (for robustness)</description></item>
/// <item><description>Comments: Skip JSON comments when reading</description></item>
/// <item><description>Indentation: No indentation (compact JSON for network efficiency)</description></item>
/// </list>
/// </para>
/// </remarks>
/// <seealso cref="FreeAgentJsonContext"/>
/// <seealso cref="JsonSerializerOptions"/>
public static class SharedJsonOptions
{
    private static readonly FreeAgentJsonContext s_sourceGenContext = new();

    /// <summary>
    /// Gets the shared <see cref="JsonSerializerOptions"/> instance using reflection-based serialization.
    /// </summary>
    /// <value>A thread-safe, immutable <see cref="JsonSerializerOptions"/> instance configured for the FreeAgent API.</value>
    /// <remarks>
    /// <para>
    /// This instance uses reflection-based serialization which provides maximum compatibility
    /// but with some runtime overhead. It's suitable for:
    /// <list type="bullet">
    /// <item><description>Development and testing scenarios</description></item>
    /// <item><description>Dynamic types not included in the source generation context</description></item>
    /// <item><description>Situations where AOT compilation is not required</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// For production scenarios and better performance, prefer <see cref="SourceGenOptions"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="SourceGenOptions"/>
    public static JsonSerializerOptions Instance { get; } = CreateOptions();

    /// <summary>
    /// Gets the shared <see cref="JsonSerializerOptions"/> instance with source generation support.
    /// </summary>
    /// <value>A thread-safe, immutable <see cref="JsonSerializerOptions"/> instance configured for source-generated serialization.</value>
    /// <remarks>
    /// <para>
    /// This instance uses compile-time source generation via <see cref="FreeAgentJsonContext"/>
    /// to provide optimal performance and support for AOT (Ahead-of-Time) compilation. Benefits include:
    /// <list type="bullet">
    /// <item><description>Faster serialization/deserialization (no reflection overhead)</description></item>
    /// <item><description>Reduced memory allocation</description></item>
    /// <item><description>Smaller deployment size with IL trimming</description></item>
    /// <item><description>Native AOT compilation support</description></item>
    /// <item><description>Better startup performance</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// This is the recommended options instance for all production scenarios and is used throughout
    /// the library by default via <see cref="HttpResponseMessageExtensions"/>.
    /// </para>
    /// </remarks>
    /// <seealso cref="FreeAgentJsonContext"/>
    /// <seealso cref="Instance"/>
    public static JsonSerializerOptions SourceGenOptions { get; } = CreateSourceGenOptions();

    /// <summary>
    /// Gets the source generation context for direct use with <see cref="JsonSerializer"/> methods.
    /// </summary>
    /// <value>The <see cref="FreeAgentJsonContext"/> instance containing serialization metadata for all FreeAgent types.</value>
    /// <remarks>
    /// This property provides direct access to the source generation context, which can be used
    /// with <see cref="JsonSerializer"/> methods that accept a <see cref="JsonSerializerContext"/>.
    /// Most consumers should use <see cref="SourceGenOptions"/> instead, which wraps this context
    /// in a <see cref="JsonSerializerOptions"/> instance.
    /// </remarks>
    /// <seealso cref="FreeAgentJsonContext"/>
    public static FreeAgentJsonContext Context => s_sourceGenContext;

    /// <summary>
    /// Creates a new <see cref="JsonSerializerOptions"/> instance with reflection-based serialization.
    /// </summary>
    /// <returns>A configured <see cref="JsonSerializerOptions"/> instance.</returns>
    /// <remarks>
    /// This method is called once during static initialization to create the <see cref="Instance"/> property.
    /// The returned options are configured with web defaults and customized for the FreeAgent API.
    /// </remarks>
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

    /// <summary>
    /// Creates a new <see cref="JsonSerializerOptions"/> instance with source-generated serialization.
    /// </summary>
    /// <returns>A configured <see cref="JsonSerializerOptions"/> instance with source generation support.</returns>
    /// <remarks>
    /// This method is called once during static initialization to create the <see cref="SourceGenOptions"/> property.
    /// The returned options are configured identically to <see cref="CreateOptions"/> but with the
    /// <see cref="FreeAgentJsonContext"/> set as the <see cref="JsonSerializerOptions.TypeInfoResolver"/>
    /// to enable source generation.
    /// </remarks>
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
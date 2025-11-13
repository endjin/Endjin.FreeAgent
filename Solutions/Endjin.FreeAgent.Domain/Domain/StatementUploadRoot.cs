// <copyright file="StatementUploadRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

/// <summary>
/// Represents the root-level JSON request wrapper for a <see cref="Domain.StatementUpload"/> resource.
/// </summary>
/// <remarks>
/// This wrapper type is used for JSON serialization when uploading bank statement files via the FreeAgent API.
/// </remarks>
/// <seealso cref="StatementUpload"/>
/// <seealso cref="BankStatementUpload"/>
public record StatementUploadRoot
{
    /// <summary>
    /// Gets the statement upload data for the API request.
    /// </summary>
    /// <value>
    /// The <see cref="Domain.StatementUpload"/> object containing statement file details.
    /// </value>
    [JsonPropertyName("statement")]
    public StatementUpload? Statement { get; init; }
}
// <copyright file="StatementUploadRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record StatementUploadRoot
{
    [JsonPropertyName("statement")]
    public StatementUpload? Statement { get; init; }
}
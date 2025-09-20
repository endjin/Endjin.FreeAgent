// <copyright file="AttachmentRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record AttachmentRoot
{
    [JsonPropertyName("attachment")]
    public Attachment? Attachment { get; init; }
}
// <copyright file="NotesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public class NotesRoot
{
    [JsonPropertyName("notes")]
    public List<NoteItem>? Notes { get; set; }
}
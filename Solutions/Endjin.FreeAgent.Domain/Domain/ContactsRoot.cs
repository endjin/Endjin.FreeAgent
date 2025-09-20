// <copyright file="ContactsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record ContactsRoot
{
    [JsonPropertyName("contacts")]
    public ImmutableList<Contact> Contacts { get; init; } = [];
}
// <copyright file="Link.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

using System;

/// <summary>
/// Represents a hyperlink relationship in the FreeAgent API following REST/HATEOAS principles.
/// </summary>
/// <remarks>
/// <para>
/// Links define relationships between resources in the FreeAgent API, providing URIs for related entities
/// and operations. They follow the HATEOAS (Hypermedia as the Engine of Application State) pattern,
/// allowing clients to discover available actions and navigate between related resources dynamically.
/// </para>
/// <para>
/// Common relationship types include "self" for the resource's own URI, "parent" for the containing resource,
/// and named relationships like "invoice", "project", or "contact" for related entities.
/// </para>
/// </remarks>
public record Link
{
    /// <summary>
    /// Gets the relationship type that defines how this link relates to the current resource.
    /// </summary>
    /// <value>
    /// A string identifier describing the relationship, such as "self", "parent", "invoice", "project",
    /// or other semantic relationship names. This field is required.
    /// </value>
    public required string Rel { get; init; }

    /// <summary>
    /// Gets the URI target of this link.
    /// </summary>
    /// <value>
    /// The absolute URI pointing to the related resource or operation endpoint. This field is required.
    /// </value>
    public required Uri Uri { get; init; }
}
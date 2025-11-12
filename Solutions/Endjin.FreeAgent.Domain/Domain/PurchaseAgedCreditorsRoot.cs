// <copyright file="PurchaseAgedCreditorsRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record PurchaseAgedCreditorsRoot
{
    [JsonPropertyName("purchase_aged_creditors")]
    public PurchaseAgedCreditors? PurchaseAgedCreditors { get; init; }
}
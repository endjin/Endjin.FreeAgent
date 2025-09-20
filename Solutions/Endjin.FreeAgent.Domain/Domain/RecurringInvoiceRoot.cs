// <copyright file="RecurringInvoiceRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record RecurringInvoiceRoot
{
    [JsonPropertyName("recurring_invoice")]
    public RecurringInvoice? RecurringInvoice { get; init; }
}
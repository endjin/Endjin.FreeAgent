// <copyright file="RecurringInvoicesRoot.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain;

public record RecurringInvoicesRoot
{
    [JsonPropertyName("recurring_invoices")]
    public List<RecurringInvoice>? RecurringInvoices { get; init; }
}
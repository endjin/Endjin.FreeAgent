// <copyright file="ProjectBuilder.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Endjin.FreeAgent.Domain.Tests.Builders;

using System;
using System.Collections.Immutable;

public class ProjectBuilder
{
    private Uri? url = new("https://api.freeagent.com/v2/projects/1");
    private Uri? contact = new("https://api.freeagent.com/v2/contacts/1");
    private Contact? contactEntry;
    private string? name = "Test Project";
    private string? status = "Active";
    private string? contractPoReference = "PO-12345";
    private bool usesProjectInvoiceSequence = false;
    private string? currency = "GBP";
    private decimal? budget = 10000m;
    private string? budgetUnits = "Hours";
    private decimal? hoursPerDay = 8m;
    private decimal? normalBillingRate = 100m;
    private string? billingPeriod = "hour";
    private bool? isIr35 = false;
    private bool? isEstimate = false;
    private DateTimeOffset? startsOn = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero);
    private DateTimeOffset? endsOn = new DateTimeOffset(2024, 9, 1, 0, 0, 0, TimeSpan.Zero);
    private DateTimeOffset? createdAt = new DateTimeOffset(2024, 5, 25, 0, 0, 0, TimeSpan.Zero);
    private DateTimeOffset? updatedAt = new DateTimeOffset(2024, 6, 1, 0, 0, 0, TimeSpan.Zero);
    private ImmutableList<Timeslip> timeslipEntries = [];

    public ProjectBuilder WithUrl(Uri? url)
    {
        this.url = url;
        return this;
    }

    public ProjectBuilder WithName(string? name)
    {
        this.name = name;
        return this;
    }

    public ProjectBuilder WithContact(Contact contact)
    {
        this.contact = contact.Url;
        this.contactEntry = contact;
        return this;
    }

    public ProjectBuilder WithStatus(string? status)
    {
        this.status = status;
        return this;
    }

    public ProjectBuilder AsCompleted()
    {
        this.status = "Completed";
        this.endsOn = new DateTimeOffset(2024, 5, 31, 0, 0, 0, TimeSpan.Zero);
        return this;
    }

    public ProjectBuilder WithBudget(decimal? budget, string? budgetUnits = "Hours")
    {
        this.budget = budget;
        this.budgetUnits = budgetUnits;
        return this;
    }

    public ProjectBuilder WithBillingRate(decimal? rate, string? period = "hour")
    {
        this.normalBillingRate = rate;
        this.billingPeriod = period;
        return this;
    }

    public ProjectBuilder WithTimeslips(params Timeslip[] timeslips)
    {
        this.timeslipEntries = [.. timeslips];
        return this;
    }

    public ProjectBuilder AsEstimate()
    {
        this.isEstimate = true;
        return this;
    }

    public ProjectBuilder AsIr35()
    {
        this.isIr35 = true;
        return this;
    }

    public ProjectBuilder WithDates(DateTimeOffset? startsOn, DateTimeOffset? endsOn)
    {
        this.startsOn = startsOn;
        this.endsOn = endsOn;
        return this;
    }

    public Project Build() => new()
    {
        Url = url,
        Contact = contact,
        ContactEntry = contactEntry,
        Name = name,
        Status = status,
        ContractPoReference = contractPoReference,
        UsesProjectInvoiceSequence = usesProjectInvoiceSequence,
        Currency = currency,
        Budget = budget,
        BudgetUnits = budgetUnits,
        HoursPerDay = hoursPerDay,
        NormalBillingRate = normalBillingRate,
        BillingPeriod = billingPeriod,
        IsIr35 = isIr35,
        IsEstimate = isEstimate,
        StartsOn = startsOn,
        EndsOn = endsOn,
        CreatedAt = createdAt,
        UpdatedAt = updatedAt,
        TimeslipEntries = timeslipEntries
    };

    public static implicit operator Project(ProjectBuilder builder) => builder.Build();
}

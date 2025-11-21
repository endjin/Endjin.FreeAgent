# Endjin.FreeAgent.Domain

Domain models and types for the [FreeAgent](https://www.freeagent.com/) accounting API, providing strongly-typed representations of all FreeAgent resources.

## Features

- **Comprehensive domain models** for all FreeAgent API resources
- **Immutable record types** for thread-safety and reliability
- **System.Text.Json serialization** with source generation support
- **Modern C# 14 features** including records, init-only properties, and required members
- **Full nullable reference type annotations**

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Endjin.FreeAgent.Domain
```

Or via Package Manager Console:

```powershell
Install-Package Endjin.FreeAgent.Domain
```

## Domain Models

This package provides strongly-typed models for all FreeAgent resources:

### Core Entities
- **Company** - Company settings and configuration
- **User** - User accounts and profiles
- **Contact** - Customers, suppliers, and other contacts
- **Project** - Projects and their settings

### Financial Documents
- **Invoice** - Sales invoices and invoice items
- **CreditNote** - Credit notes for refunds and corrections
- **Bill** - Purchase bills and expenses
- **Expense** - Expense claims and receipts
- **Estimate** - Quotes and estimates

### Banking
- **BankAccount** - Bank account information
- **BankTransaction** - Bank transactions and reconciliation
- **BankTransactionExplanation** - Transaction categorization

### Time Tracking
- **Task** - Project tasks and activities
- **Timeslip** - Time entries for billing
- **Timesheet** - Weekly timesheets

### Accounting
- **Category** - Income and expense categories
- **VatReturn** - VAT return submissions
- **CorporationTaxReturn** - Corporation tax filings
- **SelfAssessmentReturn** - Self assessment tax returns
- **JournalSet** - Manual journal entries

### Reports and Analytics
- **AccountingPeriod** - Financial year periods
- **TrialBalance** - Trial balance reports
- **ProfitAndLoss** - P&L statements
- **BalanceSheet** - Balance sheet reports

## Usage Examples

```csharp
using Endjin.FreeAgent.Domain;
using System.Collections.Immutable;

// Create a new invoice
Invoice invoice = new()
{
    Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
    DatedOn = DateOnly.FromDateTime(DateTime.Now),
    PaymentTermsInDays = 30,
    Currency = "GBP",
    Status = "Draft",
    InvoiceItems = new List<InvoiceItem>
    {
        new()
        {
            Description = "Consulting Services",
            ItemType = "Services",
            Quantity = 1,
            Price = 1000.00m,
            SalesTaxRate = 20.0m
        }
    }
};

// Create a contact
Contact contact = new()
{
    OrganisationName = "Acme Corp",
    FirstName = "John",
    LastName = "Smith",
    Email = "john.smith@acme.com",
    Country = "United Kingdom",
    UsesContactInvoiceSequence = true
};

// Create a project
Project project = new()
{
    Name = "Website Development",
    Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
    Status = "Active",
    Currency = "GBP",
    BudgetUnits = "Hours",
    HoursPerDay = 8.0m,
    NormalBillingRate = 150.0m
};
```

## Serialization

All domain models are configured for optimal JSON serialization using System.Text.Json:

```csharp
using System.Text.Json;
using Endjin.FreeAgent.Domain;

// Serialize
Invoice invoice = new() { /* ... */ };
string json = JsonSerializer.Serialize(invoice, SharedJsonOptions.Instance);

// Deserialize
Invoice? deserialized = JsonSerializer.Deserialize<Invoice>(json, SharedJsonOptions.Instance);
```

## Type Safety

All models use strongly-typed properties with appropriate .NET types:

- **DateOnly** for dates without time components
- **DateTimeOffset** for timestamps with timezone information
- **decimal** for monetary values
- **ImmutableList<T>** or **List<T>** for collections
- **Uri** for API resource URLs

## Requirements

- .NET 10.0 or later
- System.Text.Json for serialization
- System.Collections.Immutable for immutable collections

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE](LICENSE) file for details.

## Support

For support, please contact Endjin Limited or raise an issue on our GitHub repository.

---

Copyright (c) Endjin Limited. All rights reserved.
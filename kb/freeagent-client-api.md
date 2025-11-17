# FreeAgent Client API - Developer Guidelines

## Overview

The Endjin.FreeAgent library is a comprehensive .NET client for the FreeAgent accounting API. It provides strongly-typed, async access to all FreeAgent API endpoints with built-in OAuth2 authentication, retry logic, and caching.

### Key Characteristics

- **Target Framework**: .NET 10 (C# 14)
- **Architecture**: Clean separation between client services (`Endjin.FreeAgent.Client`) and domain models (`Endjin.FreeAgent.Domain`)
- **Async/Await**: All API calls are fully asynchronous
- **Dependency Injection**: First-class support for Microsoft.Extensions.DependencyInjection
- **Immutable Models**: Domain models use C# record types for immutability
- **Built-in Retry**: Exponential backoff retry logic via Corvus.Retry
- **Caching**: In-memory caching with 5-minute sliding expiration for most GET operations
- **JSON Serialization**: System.Text.Json with source generation for performance

---

## Getting Started

### Installation

Reference both packages in your project:

```xml
<PackageReference Include="Endjin.FreeAgent.Client" />
<PackageReference Include="Endjin.FreeAgent.Domain" />
```

### Configuration

The library requires OAuth2 credentials. There are three configuration patterns:

#### 1. Using appsettings.json (Recommended)

```json
{
  "FreeAgent": {
    "ClientId": "your-client-id",
    "ClientSecret": "your-client-secret",
    "RefreshToken": "your-refresh-token"
  }
}
```

#### 2. Direct Options Object

```csharp
var options = new FreeAgentOptions
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    RefreshToken = "your-refresh-token"
};

var client = new FreeAgentClient(options);
```

#### 3. Dependency Injection (Best for ASP.NET Core)

```csharp
// In Program.cs or Startup.cs
services.AddFreeAgentClient(configuration);

// Then inject
public class MyService
{
    private readonly IFreeAgentClient _client;

    public MyService(IFreeAgentClient client)
    {
        _client = client;
    }
}
```

### Basic Usage Example

```csharp
using Endjin.FreeAgent.Client;
using Endjin.FreeAgent.Domain;

// Initialize client
var options = new FreeAgentOptions
{
    ClientId = "your-client-id",
    ClientSecret = "your-client-secret",
    RefreshToken = "your-refresh-token"
};

var client = new FreeAgentClient(options);

// Get all contacts
List<Contact> contacts = await client.Contacts.GetAllAsync();

// Get all invoices
List<Invoice> invoices = await client.Invoices.GetAllAsync();

// Get a specific invoice by URL
var invoice = await client.Invoices.GetAsync(new Uri("https://api.freeagent.com/v2/invoices/123"));
```

---

## Authentication & OAuth2

### OAuth2Service

The library handles OAuth2 token management automatically through the `IOAuth2Service` interface:

- **Automatic Token Refresh**: Access tokens are refreshed automatically when expired
- **Token Caching**: Access tokens are cached in memory with proper expiration
- **Base URL**: All API calls use `https://api.freeagent.com` as the base URL

### Key Methods

```csharp
// Get current access token (automatically refreshes if needed)
Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)

// Manually refresh access token
Task<string> RefreshAccessTokenAsync(CancellationToken cancellationToken = default)
```

### ClientBase

All service classes inherit from `ClientBase` which provides:
- HTTP client management via `IHttpClientFactory`
- OAuth2 integration
- Retry logic with exponential backoff
- Standardized error handling

---

## Core API Services

The `FreeAgentClient` class provides access to 39 specialized service interfaces. Below is a complete reference organized by functional area.

### Invoicing Services

#### 1. Invoices (`IInvoicesService`)

Manage sales invoices.

**Methods:**
- `Task<List<Invoice>> GetAllAsync(InvoiceView? view = null, Contact? contact = null, Project? project = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, DateTimeOffset? updatedSince = null, bool? nested = null, CancellationToken cancellationToken = default)`
- `Task<Invoice> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Invoice> CreateAsync(Invoice invoice, CancellationToken cancellationToken = default)`
- `Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Email> SendEmailAsync(Uri url, Email email, CancellationToken cancellationToken = default)`
- `Task MarkAsScheduledAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task MarkAsSentAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task MarkAsCancelledAsync(Uri url, CancellationToken cancellationToken = default)`

**Key Domain Model:**
```csharp
public record Invoice
{
    public Uri? Url { get; init; }
    public Uri? Contact { get; init; }
    public Uri? Project { get; init; }
    public string? Reference { get; init; }
    public string? Currency { get; init; }
    public decimal? NetValue { get; init; }
    public decimal? SalesTax { get; init; }
    public decimal? TotalValue { get; init; }
    public DateOnly? DatedOn { get; init; }
    public DateOnly? DueOn { get; init; }
    public string? Status { get; init; }
    public List<InvoiceItem>? InvoiceItems { get; init; }
    // ... additional properties
}
```

**Usage Example:**
```csharp
// Get all draft invoices
var draftInvoices = await client.Invoices.GetAllAsync(view: InvoiceView.Draft);

// Create a new invoice
var newInvoice = new Invoice
{
    Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
    DatedOn = DateOnly.FromDateTime(DateTime.Today),
    Currency = "GBP",
    InvoiceItems = new List<InvoiceItem>
    {
        new InvoiceItem
        {
            ItemType = "Hours",
            Description = "Consulting services",
            Quantity = 10,
            Price = 100
        }
    }
};
var created = await client.Invoices.CreateAsync(newInvoice);

// Mark as sent
await client.Invoices.MarkAsSentAsync(created.Url!);
```

#### 2. Credit Notes (`ICreditNotesService`)

Manage credit notes for refunds and adjustments.

**Methods:**
- `Task<List<CreditNote>> GetAllAsync(CreditNoteView? view = null, Contact? contact = null, Project? project = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, CancellationToken cancellationToken = default)`
- `Task<CreditNote> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<CreditNote> CreateAsync(CreditNote creditNote, CancellationToken cancellationToken = default)`
- `Task<CreditNote> UpdateAsync(CreditNote creditNote, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 3. Estimates (`IEstimatesService`)

Manage project estimates and quotes.

**Methods:**
- `Task<List<Estimate>> GetAllAsync(EstimateView? view = null, Contact? contact = null, CancellationToken cancellationToken = default)`
- `Task<Estimate> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Estimate> CreateAsync(Estimate estimate, CancellationToken cancellationToken = default)`
- `Task<Estimate> UpdateAsync(Estimate estimate, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task MarkAsApprovedAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task MarkAsRejectedAsync(Uri url, CancellationToken cancellationToken = default)`

#### 4. Recurring Invoices (`IRecurringInvoicesService`)

Manage automated recurring invoices.

**Methods:**
- `Task<List<RecurringInvoice>> GetAllAsync(RecurringInvoiceView? view = null, Contact? contact = null, CancellationToken cancellationToken = default)`
- `Task<RecurringInvoice> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<RecurringInvoice> CreateAsync(RecurringInvoice recurringInvoice, CancellationToken cancellationToken = default)`
- `Task<RecurringInvoice> UpdateAsync(RecurringInvoice recurringInvoice, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

---

### Contacts & Projects

#### 5. Contacts (`IContactsService`)

Manage customers, suppliers, and other business contacts.

**Methods:**
- `Task<List<Contact>> GetAllAsync(ContactView? view = null, CancellationToken cancellationToken = default)`
- `Task<Contact> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Contact> CreateAsync(Contact contact, CancellationToken cancellationToken = default)`
- `Task<Contact> UpdateAsync(Contact contact, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

**Key Domain Model:**
```csharp
public record Contact
{
    public Uri? Url { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? OrganisationName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Mobile { get; init; }
    public string? Address1 { get; init; }
    public string? Address2 { get; init; }
    public string? Address3 { get; init; }
    public string? Town { get; init; }
    public string? Region { get; init; }
    public string? Postcode { get; init; }
    public string? Country { get; init; }
    public string? ContactNameOnInvoices { get; init; }
    public string? DefaultPaymentTermsInDays { get; init; }
    public string? Locale { get; init; }
    public bool? AccountBalance { get; init; }
    public bool? UsesContactInvoiceSequence { get; init; }
    public string? Status { get; init; }
    // ... additional properties
}
```

#### 6. Projects (`IProjectsService`)

Manage client projects and track work.

**Methods:**
- `Task<List<Project>> GetAllAsync(ProjectView? view = null, Contact? contact = null, CancellationToken cancellationToken = default)`
- `Task<Project> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Project> CreateAsync(Project project, CancellationToken cancellationToken = default)`
- `Task<Project> UpdateAsync(Project project, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

**Key Domain Model:**
```csharp
public record Project
{
    public Uri? Url { get; init; }
    public Uri? Contact { get; init; }
    public string? Name { get; init; }
    public string? Budget { get; init; }
    public string? BudgetUnits { get; init; }
    public bool? IsIr35 { get; init; }
    public string? Status { get; init; }
    public string? Currency { get; init; }
    public string? BillingPeriod { get; init; }
    public decimal? NormalBillingRate { get; init; }
    public decimal? HoursPerDay { get; init; }
    // ... additional properties
}
```

---

### Expenses & Mileage

#### 7. Expenses (`IExpensesService`)

Manage business expenses and receipts.

**Methods:**
- `Task<List<Expense>> GetAllAsync(ExpenseView? view = null, User? user = null, Project? project = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, DateTimeOffset? updatedSince = null, CancellationToken cancellationToken = default)`
- `Task<Expense> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Expense> CreateAsync(Expense expense, CancellationToken cancellationToken = default)`
- `Task<Expense> UpdateAsync(Expense expense, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

**Key Domain Model:**
```csharp
public record Expense
{
    public Uri? Url { get; init; }
    public Uri? User { get; init; }
    public Uri? Project { get; init; }
    public Uri? Category { get; init; }
    public Uri? BankAccount { get; init; }
    public DateOnly? DatedOn { get; init; }
    public string? Description { get; init; }
    public decimal? GrossValue { get; init; }
    public decimal? SalesTaxValue { get; init; }
    public string? SalesTaxRate { get; init; }
    public string? Currency { get; init; }
    public decimal? NativeGrossValue { get; init; }
    public decimal? NativeSalesTaxValue { get; init; }
    public Uri? Attachment { get; init; }
    public Uri? Receipt { get; init; }
    public bool? ManualSalesTaxAmount { get; init; }
    public string? RebillType { get; init; }
    public decimal? RebillFactor { get; init; }
    // ... additional properties
}
```

#### 8. JournalSets (`IJournalSetsService`)

Manage journal entries for expense batching.

**Methods:**
- `Task<List<JournalSet>> GetAllAsync(JournalSetView? view = null, CancellationToken cancellationToken = default)`
- `Task<JournalSet> GetAsync(Uri url, CancellationToken cancellationToken = default)`

---

### Banking Services

#### 9. Bank Accounts (`IBankAccountsService`)

Manage bank accounts and payment methods.

**Methods:**
- `Task<List<BankAccount>> GetAllAsync(CancellationToken cancellationToken = default)`
- `Task<BankAccount> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<BankAccount> CreateAsync(BankAccount bankAccount, CancellationToken cancellationToken = default)`
- `Task<BankAccount> UpdateAsync(BankAccount bankAccount, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

**Key Domain Model:**
```csharp
public record BankAccount
{
    public Uri? Url { get; init; }
    public string? AccountNumber { get; init; }
    public string? SortCode { get; init; }
    public string? Iban { get; init; }
    public string? Bic { get; init; }
    public string? Name { get; init; }
    public string? Type { get; init; }
    public string? Currency { get; init; }
    public decimal? OpeningBalance { get; init; }
    public decimal? CurrentBalance { get; init; }
    public bool? IsPersonal { get; init; }
    public bool? IsPrimary { get; init; }
    public string? Status { get; init; }
    // ... additional properties
}
```

#### 10. Bank Transactions (`IBankTransactionsService`)

Manage bank transactions and categorization.

**Methods:**
- `Task<List<BankTransaction>> GetAllAsync(BankAccount? bankAccount = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, DateTimeOffset? updatedSince = null, BankTransactionView? view = null, CancellationToken cancellationToken = default)`
- `Task<BankTransaction> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<BankTransaction> CreateAsync(BankTransaction bankTransaction, CancellationToken cancellationToken = default)`
- `Task<BankTransaction> UpdateAsync(BankTransaction bankTransaction, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 11. Bank Transaction Explanations (`IBankTransactionExplanationsService`)

Explain and categorize bank transactions.

**Methods:**
- `Task<BankTransactionExplanation> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<BankTransactionExplanation> CreateAsync(BankTransactionExplanation explanation, CancellationToken cancellationToken = default)`
- `Task<BankTransactionExplanation> UpdateAsync(BankTransactionExplanation explanation, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 12. Bank Statement Uploads (`IBankStatementUploadsService`)

Upload bank statements in various formats.

**Methods:**
- `Task<BankStatementUpload> UploadAsync(BankAccount bankAccount, string fileName, Stream fileContent, CancellationToken cancellationToken = default)`

---

### Financial Reports

#### 13. Balance Sheet (`IBalanceSheetService`)

Generate balance sheet reports.

**Methods:**
- `Task<BalanceSheet> GetAsync(DateOnly date, CancellationToken cancellationToken = default)`

#### 14. Cash Flow (`ICashFlowService`)

Generate cash flow statements.

**Methods:**
- `Task<CashFlow> GetAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)`

#### 15. Profit and Loss (`IProfitAndLossService`)

Generate profit and loss statements.

**Methods:**
- `Task<ProfitAndLoss> GetAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)`

#### 16. Trial Balance (`ITrialBalanceService`)

Generate trial balance reports.

**Methods:**
- `Task<TrialBalance> GetAsync(DateOnly fromDate, DateOnly toDate, CancellationToken cancellationToken = default)`

---

### Payroll & Time Tracking

#### 17. Timeslips (`ITimeslipsService`)

Track time entries for projects.

**Methods:**
- `Task<List<Timeslip>> GetAllAsync(TimeslipView? view = null, User? user = null, Project? project = null, Task? task = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, DateTimeOffset? updatedSince = null, CancellationToken cancellationToken = default)`
- `Task<Timeslip> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Timeslip> CreateAsync(Timeslip timeslip, CancellationToken cancellationToken = default)`
- `Task<Timeslip> UpdateAsync(Timeslip timeslip, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

**Key Domain Model:**
```csharp
public record Timeslip
{
    public Uri? Url { get; init; }
    public Uri? User { get; init; }
    public Uri? Project { get; init; }
    public Uri? Task { get; init; }
    public DateOnly? DatedOn { get; init; }
    public decimal? Hours { get; init; }
    public string? Comment { get; init; }
    public bool? Billable { get; init; }
    // ... additional properties
}
```

#### 18. Tasks (`ITasksService`)

Manage project tasks for time tracking.

**Methods:**
- `Task<List<Domain.Task>> GetAllAsync(Project? project = null, CancellationToken cancellationToken = default)`
- `Task<Domain.Task> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Domain.Task> CreateAsync(Domain.Task task, CancellationToken cancellationToken = default)`
- `Task<Domain.Task> UpdateAsync(Domain.Task task, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 19. Users (`IUsersService`)

Manage user accounts and permissions.

**Methods:**
- `Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)`
- `Task<User> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)`
- `Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

---

### Tax & Compliance

#### 20. Company (`ICompanyService`)

Get company information and settings.

**Methods:**
- `Task<Company> GetAsync(CancellationToken cancellationToken = default)`
- `Task<Company> UpdateAsync(Company company, CancellationToken cancellationToken = default)`

#### 21. Bills (`IBillsService`)

Manage supplier bills and purchase invoices.

**Methods:**
- `Task<List<Bill>> GetAllAsync(BillView? view = null, Contact? contact = null, Project? project = null, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, DateTimeOffset? updatedSince = null, CancellationToken cancellationToken = default)`
- `Task<Bill> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Bill> CreateAsync(Bill bill, CancellationToken cancellationToken = default)`
- `Task<Bill> UpdateAsync(Bill bill, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 22. Categories (`ICategoriesService`)

Get expense and income categories.

**Methods:**
- `Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)`
- `Task<Category> GetAsync(Uri url, CancellationToken cancellationToken = default)`

---

### Additional Services

#### 23. Attachments (`IAttachmentsService`)

Manage file attachments for expenses and other records.

**Methods:**
- `Task<Attachment> UploadAsync(string fileName, Stream fileContent, CancellationToken cancellationToken = default)`
- `Task<Attachment> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 24. Notes (`INotesService`)

Add notes to contacts, projects, and other entities.

**Methods:**
- `Task<List<Note>> GetAllAsync(CancellationToken cancellationToken = default)`
- `Task<Note> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Note> CreateAsync(Note note, CancellationToken cancellationToken = default)`
- `Task<Note> UpdateAsync(Note note, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 25. Capital Assets (`ICapitalAssetsService`)

Track fixed assets and depreciation.

**Methods:**
- `Task<List<CapitalAsset>> GetAllAsync(CapitalAssetView? view = null, CancellationToken cancellationToken = default)`
- `Task<CapitalAsset> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<CapitalAsset> CreateAsync(CapitalAsset capitalAsset, CancellationToken cancellationToken = default)`
- `Task<CapitalAsset> UpdateAsync(CapitalAsset capitalAsset, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

#### 26. Webhooks (`IWebhooksService`)

Configure webhooks for event notifications.

**Methods:**
- `Task<List<Webhook>> GetAllAsync(CancellationToken cancellationToken = default)`
- `Task<Webhook> GetAsync(Uri url, CancellationToken cancellationToken = default)`
- `Task<Webhook> CreateAsync(Webhook webhook, CancellationToken cancellationToken = default)`
- `Task<Webhook> UpdateAsync(Webhook webhook, CancellationToken cancellationToken = default)`
- `Task DeleteAsync(Uri url, CancellationToken cancellationToken = default)`

---

## Complete Service List Reference

Here is the complete list of all 39 services available through `FreeAgentClient`:

| Property | Interface | Purpose |
|----------|-----------|---------|
| `Attachments` | `IAttachmentsService` | File attachments for expenses |
| `BankAccounts` | `IBankAccountsService` | Bank account management |
| `BankStatementUploads` | `IBankStatementUploadsService` | Upload bank statements |
| `BankTransactionExplanations` | `IBankTransactionExplanationsService` | Explain transactions |
| `BankTransactions` | `IBankTransactionsService` | Bank transaction management |
| `Bills` | `IBillsService` | Supplier bills |
| `CapitalAssets` | `ICapitalAssetsService` | Fixed asset tracking |
| `Categories` | `ICategoriesService` | Expense/income categories |
| `Company` | `ICompanyService` | Company information |
| `Contacts` | `IContactsService` | Customer/supplier contacts |
| `CreditNotes` | `ICreditNotesService` | Credit notes |
| `Currencies` | `ICurrenciesService` | Currency information |
| `Estimates` | `IEstimatesService` | Project estimates/quotes |
| `Expenses` | `IExpensesService` | Business expenses |
| `Invoices` | `IInvoicesService` | Sales invoices |
| `JournalSets` | `IJournalSetsService` | Journal entries |
| `Notes` | `INotesService` | Notes for entities |
| `Projects` | `IProjectsService` | Client projects |
| `RecurringInvoices` | `IRecurringInvoicesService` | Recurring invoices |
| `Tasks` | `ITasksService` | Project tasks |
| `Timeslips` | `ITimeslipsService` | Time tracking |
| `Users` | `IUsersService` | User management |
| `Webhooks` | `IWebhooksService` | Event webhooks |
| `BalanceSheet` | `IBalanceSheetService` | Balance sheet reports |
| `CashFlow` | `ICashFlowService` | Cash flow reports |
| `ProfitAndLoss` | `IProfitAndLossService` | P&L reports |
| `TrialBalance` | `ITrialBalanceService` | Trial balance reports |
| `StockItems` | `IStockItemsService` | Inventory management |
| `Bills` | `IBillsService` | Purchase invoices |
| `OutgoingEmailAddresses` | `IOutgoingEmailAddressesService` | Email configuration |
| `OutgoingInvoiceEmails` | `IOutgoingInvoiceEmailsService` | Invoice email tracking |
| `BankAccountOpeningBalances` | `IBankAccountOpeningBalancesService` | Initial balances |
| `CapitalAssetTypes` | `ICapitalAssetTypesService` | Asset categories |
| `TaxTimelines` | `ITaxTimelinesService` | Tax payment schedules |
| `Mileage` | `IMileageService` | Mileage tracking |
| `Files` | `IFilesService` | File management |
| `Journals` | `IJournalsService` | Manual journal entries |
| `AccountingReports` | `IAccountingReportsService` | Custom accounting reports |
| `OutgoingCreditNoteEmails` | `IOutgoingCreditNoteEmailsService` | Credit note emails |

---

## Domain Model Patterns

### Uri-Based Relationships

FreeAgent uses URIs to reference related entities. All relationships are represented as `Uri?` properties:

```csharp
// Example: Invoice references Contact and Project
var invoice = new Invoice
{
    Contact = new Uri("https://api.freeagent.com/v2/contacts/123"),
    Project = new Uri("https://api.freeagent.com/v2/projects/456")
};
```

### Immutable Records

All domain models are C# records with init-only properties:

```csharp
public record Contact
{
    public Uri? Url { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    // Cannot be modified after creation
}
```

### JSON Property Names

Properties use camelCase for JSON serialization via `[JsonPropertyName]` attributes:

```csharp
[JsonPropertyName("first_name")]
public string? FirstName { get; init; }
```

### Date Handling

- **DateOnly**: Used for dates without time (e.g., invoice dates, expense dates)
- **DateTimeOffset**: Used for timestamps with timezone information

```csharp
invoice.DatedOn = DateOnly.FromDateTime(DateTime.Today);
invoice.UpdatedAt = DateTimeOffset.UtcNow;
```

---

## Error Handling

### Exception Types

The library throws standard .NET exceptions:

- **`HttpRequestException`**: Network errors, API errors, HTTP status codes
- **`InvalidOperationException`**: Deserialization failures, invalid state
- **`ArgumentNullException`**: Missing required parameters

### Retry Logic

Built-in retry with exponential backoff via Corvus.Retry:

- **Default Strategy**: Exponential backoff
- **Automatic**: Applied to all HTTP operations
- **Configurable**: Through Corvus.Retry policies

### Best Practices

```csharp
try
{
    var invoice = await client.Invoices.GetAsync(invoiceUrl);
}
catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
{
    // Handle 404 - resource not found
    Console.WriteLine("Invoice not found");
}
catch (HttpRequestException ex)
{
    // Handle other HTTP errors
    Console.WriteLine($"API error: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    // Handle deserialization errors
    Console.WriteLine($"Data error: {ex.Message}");
}
```

---

## Caching Strategy

### In-Memory Caching

Most GET operations use in-memory caching with:

- **Expiration**: 5-minute sliding expiration
- **Scope**: Per service instance
- **Cache Keys**: Based on method parameters and query strings

### Services with Caching

Services that implement caching (via `MemoryCache`):
- Contacts
- Projects
- Invoices
- Expenses
- Bank Transactions
- Categories
- Users
- Most other GET operations

### Cache Bypass

Some operations bypass cache:
- POST/PUT/DELETE operations
- Operations with `updatedSince` parameter
- Report generation

---

## Thread Safety & Concurrency

### Thread-Safe Components

- **FreeAgentClient**: Safe for concurrent calls across different services
- **OAuth2Service**: Thread-safe token refresh with locking
- **HttpClient**: Managed via `IHttpClientFactory` for proper pooling

### Safe Patterns

```csharp
// Safe: Multiple concurrent calls to different services
var contactsTask = client.Contacts.GetAllAsync();
var projectsTask = client.Projects.GetAllAsync();
var invoicesTask = client.Invoices.GetAllAsync();

await Task.WhenAll(contactsTask, projectsTask, invoicesTask);

// Safe: Multiple concurrent calls to same service
var invoice1Task = client.Invoices.GetAsync(url1);
var invoice2Task = client.Invoices.GetAsync(url2);

await Task.WhenAll(invoice1Task, invoice2Task);
```

### Considerations

- Each service is lazily initialized once
- HttpClient instances are properly pooled
- OAuth2 token refresh is protected with locks

---

## Common Usage Patterns

### Pattern 1: List All Resources

```csharp
// Get all contacts
List<Contact> allContacts = await client.Contacts.GetAllAsync();

// Get filtered list
List<Invoice> draftInvoices = await client.Invoices.GetAllAsync(
    view: InvoiceView.Draft
);

// Get by date range
List<Expense> recentExpenses = await client.Expenses.GetAllAsync(
    fromDate: DateTimeOffset.UtcNow.AddMonths(-1),
    toDate: DateTimeOffset.UtcNow
);
```

### Pattern 2: Get Single Resource

```csharp
// Get by URL (from another resource's reference)
var invoice = await client.Invoices.GetAsync(
    new Uri("https://api.freeagent.com/v2/invoices/123")
);

// Get from a relationship
var contact = await client.Contacts.GetAsync(invoice.Contact!);
```

### Pattern 3: Create New Resource

```csharp
// Create contact
var newContact = new Contact
{
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    OrganisationName = "Acme Corp"
};
var createdContact = await client.Contacts.CreateAsync(newContact);

// Use the created resource
Console.WriteLine($"Created contact: {createdContact.Url}");
```

### Pattern 4: Update Existing Resource

```csharp
// Get existing resource
var contact = await client.Contacts.GetAsync(contactUrl);

// Create updated version (records are immutable)
var updatedContact = contact with
{
    Email = "new.email@example.com",
    PhoneNumber = "555-1234"
};

// Save changes
var result = await client.Contacts.UpdateAsync(updatedContact);
```

### Pattern 5: Delete Resource

```csharp
await client.Invoices.DeleteAsync(invoiceUrl);
```

### Pattern 6: Complex Workflow

```csharp
// Create a complete invoice workflow
// 1. Create contact
var contact = await client.Contacts.CreateAsync(new Contact
{
    OrganisationName = "New Client Ltd",
    Email = "billing@newclient.com"
});

// 2. Create project
var project = await client.Projects.CreateAsync(new Project
{
    Contact = contact.Url,
    Name = "Website Redesign",
    BudgetUnits = "Hours",
    Budget = "100"
});

// 3. Create timeslips
var timeslip = await client.Timeslips.CreateAsync(new Timeslip
{
    User = currentUserUrl,
    Project = project.Url,
    DatedOn = DateOnly.FromDateTime(DateTime.Today),
    Hours = 8,
    Comment = "Initial design work"
});

// 4. Create invoice
var invoice = await client.Invoices.CreateAsync(new Invoice
{
    Contact = contact.Url,
    Project = project.Url,
    DatedOn = DateOnly.FromDateTime(DateTime.Today),
    Currency = "GBP",
    InvoiceItems = new List<InvoiceItem>
    {
        new InvoiceItem
        {
            ItemType = "Hours",
            Description = "Design work",
            Quantity = 8,
            Price = 100
        }
    }
});

// 5. Send the invoice
await client.Invoices.MarkAsSentAsync(invoice.Url!);
```

### Pattern 7: Generate Reports

```csharp
// Profit & Loss for last quarter
var fromDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3));
var toDate = DateOnly.FromDateTime(DateTime.Today);

var profitAndLoss = await client.ProfitAndLoss.GetAsync(fromDate, toDate);

// Balance sheet for specific date
var balanceSheet = await client.BalanceSheet.GetAsync(
    DateOnly.FromDateTime(DateTime.Today)
);
```

### Pattern 8: File Uploads

```csharp
// Upload expense receipt
using var fileStream = File.OpenRead("receipt.pdf");
var attachment = await client.Attachments.UploadAsync(
    "receipt.pdf",
    fileStream
);

// Create expense with attachment
var expense = await client.Expenses.CreateAsync(new Expense
{
    User = currentUserUrl,
    DatedOn = DateOnly.FromDateTime(DateTime.Today),
    Description = "Office supplies",
    GrossValue = 45.99m,
    Category = suppliesCategoryUrl,
    Attachment = attachment.Url
});
```

---

## Advanced Topics

### Dependency Injection Setup

Full example with logging and configuration:

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Endjin.FreeAgent.Client;

var services = new ServiceCollection();

// Add configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Add logging
services.AddLogging(builder => builder.AddConsole());

// Add FreeAgent client
services.AddFreeAgentClient(configuration);

// Build service provider
var serviceProvider = services.BuildServiceProvider();

// Use the client
var client = serviceProvider.GetRequiredService<IFreeAgentClient>();
var contacts = await client.Contacts.GetAllAsync();
```

### Custom HttpClient Configuration

```csharp
services.AddHttpClient<IFreeAgentClient, FreeAgentClient>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(GetRetryPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, retryAttempt =>
            TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
```

### View Enumerations

Many list operations support view filtering:

```csharp
// Invoice views
public enum InvoiceView
{
    All,
    Draft,
    Scheduled,
    Sent,
    Overdue,
    Paid
}

// Contact views
public enum ContactView
{
    Active,
    Hidden,
    Clients,
    Suppliers,
    All
}

// Project views
public enum ProjectView
{
    Active,
    Completed,
    Cancelled,
    Hidden,
    All
}
```

---

## Quick Reference: Key Classes

### FreeAgentOptions

```csharp
public class FreeAgentOptions
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string RefreshToken { get; set; }
}
```

### IFreeAgentClient Interface

```csharp
public interface IFreeAgentClient
{
    IInvoicesService Invoices { get; }
    IContactsService Contacts { get; }
    IProjectsService Projects { get; }
    IExpensesService Expenses { get; }
    IBankAccountsService BankAccounts { get; }
    // ... all 39 services
}
```

### Common Domain Models

**Invoice, CreditNote, Estimate**: Financial documents
**Contact**: Customers and suppliers
**Project**: Client projects
**Expense**: Business expenses
**BankTransaction**: Bank movements
**Timeslip**: Time entries
**User**: User accounts
**Category**: Expense/income classifications

---

## Tips for Claude Code Tool Implementation

### 1. Always Use Async/Await

All API calls are asynchronous. Always await them:

```csharp
var contacts = await client.Contacts.GetAllAsync();
```

### 2. Handle URIs Correctly

FreeAgent uses URIs for relationships. Always construct proper URIs:

```csharp
var contactUrl = new Uri("https://api.freeagent.com/v2/contacts/123");
var contact = await client.Contacts.GetAsync(contactUrl);
```

### 3. Use Record 'with' for Updates

Since models are immutable records, use 'with' syntax:

```csharp
var updated = original with { Email = "new@email.com" };
```

### 4. Check for Null

All reference properties are nullable. Always check:

```csharp
if (invoice.Contact != null)
{
    var contact = await client.Contacts.GetAsync(invoice.Contact);
}
```

### 5. Use DateOnly for Dates

Don't use DateTime for date-only fields:

```csharp
// Correct
DatedOn = DateOnly.FromDateTime(DateTime.Today)

// Incorrect
DatedOn = DateTime.Today // Wrong type
```

### 6. Leverage View Parameters

Use view enums to filter lists efficiently:

```csharp
// Get only unpaid invoices
var unpaid = await client.Invoices.GetAllAsync(view: InvoiceView.Sent);
```

### 7. Handle Pagination

The API returns all results in lists. No manual pagination needed:

```csharp
// This gets ALL contacts, API handles pagination internally
var allContacts = await client.Contacts.GetAllAsync();
```

### 8. Properly Dispose Resources

When using file streams:

```csharp
await using var stream = File.OpenRead("file.pdf");
var attachment = await client.Attachments.UploadAsync("file.pdf", stream);
```

---

## API Endpoints Reference

All API calls use base URL: `https://api.freeagent.com`

Common endpoint patterns:
- List: `GET /v2/resources`
- Get: `GET /v2/resources/{id}`
- Create: `POST /v2/resources`
- Update: `PUT /v2/resources/{id}`
- Delete: `DELETE /v2/resources/{id}`

---

## Version Information

- **Library Version**: Check NuGet package version
- **API Version**: FreeAgent API v2
- **Target Framework**: .NET 10
- **Language Version**: C# 14

---

## Support & Documentation

- **FreeAgent API Docs**: https://dev.freeagent.com/docs
- **GitHub Repository**: Check project source for latest updates
- **Issues**: Report via GitHub issues

---

## Summary

The Endjin.FreeAgent library provides:

✅ **Complete API Coverage**: All FreeAgent v2 endpoints
✅ **Type Safety**: Strong typing with C# records
✅ **Modern .NET**: .NET 10, async/await, dependency injection
✅ **Robust**: Built-in retry logic and error handling
✅ **Efficient**: HTTP client pooling and response caching
✅ **Developer Friendly**: Clear naming, XML docs, intuitive patterns

**Recommended Usage Flow**:
1. Configure with OAuth2 credentials
2. Initialize `FreeAgentClient` (or inject `IFreeAgentClient`)
3. Access services via client properties
4. Call async methods with proper error handling
5. Work with immutable domain models
6. Use Uri references for relationships

This library is production-ready and suitable for building FreeAgent integrations, automation tools, reporting systems, and custom business applications.

// <copyright file="FreeAgentJsonContext.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using System.Text.Json.Serialization;
using Endjin.FreeAgent.Domain;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// JSON source generation context for FreeAgent API types, providing compile-time JSON serialization metadata.
/// </summary>
/// <remarks>
/// <para>
/// This class uses .NET's source generation feature to generate JSON serialization code at compile time
/// rather than using reflection at runtime. This provides several benefits:
/// <list type="bullet">
/// <item><description>Improved performance - no reflection overhead during serialization/deserialization</description></item>
/// <item><description>Reduced memory allocation - serialization metadata is generated once at compile time</description></item>
/// <item><description>AOT (Ahead-of-Time) compilation support - works with Native AOT and trimming</description></item>
/// <item><description>Better startup time - no runtime code generation needed</description></item>
/// <item><description>Smaller deployment size - only referenced types are included</description></item>
/// </list>
/// </para>
/// <para>
/// The context includes all FreeAgent API types used for requests and responses, covering:
/// <list type="bullet">
/// <item><description>Root wrapper types (e.g., ContactRoot, InvoiceRoot) for API responses</description></item>
/// <item><description>Domain model types (e.g., Contact, Invoice) for entities</description></item>
/// <item><description>Supporting types for bank transactions, reports, tax returns, etc.</description></item>
/// </list>
/// </para>
/// <para>
/// This context is used by <see cref="SharedJsonOptions.SourceGenOptions"/> to provide
/// source-generated serialization throughout the library. The serialization options are configured
/// to match the FreeAgent API requirements (camelCase naming, ignore null values, etc.).
/// </para>
/// </remarks>
/// <seealso cref="SharedJsonOptions"/>
/// <seealso cref="JsonSerializerContext"/>
[JsonSourceGenerationOptions(
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    PropertyNameCaseInsensitive = true,
    WriteIndented = false,
    AllowTrailingCommas = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
// Root types for API responses
[JsonSerializable(typeof(AttachmentRoot))]
[JsonSerializable(typeof(BalanceSheetRoot))]
[JsonSerializable(typeof(BalanceSheetAccount))]
[JsonSerializable(typeof(CapitalAssetsSection))]
[JsonSerializable(typeof(AssetsSection))]
[JsonSerializable(typeof(LiabilitiesSection))]
[JsonSerializable(typeof(OwnersEquitySection))]
[JsonSerializable(typeof(BankAccountRoot))]
[JsonSerializable(typeof(BankAccountsRoot))]
[JsonSerializable(typeof(BankStatementUploadRoot))]
[JsonSerializable(typeof(BankStatementUploadResponseRoot))]
[JsonSerializable(typeof(BankTransactionExplanationRoot))]
[JsonSerializable(typeof(BankTransactionExplanationsRoot))]
[JsonSerializable(typeof(BankTransactionRoot))]
[JsonSerializable(typeof(BankTransactionsRoot))]
[JsonSerializable(typeof(BankTransactionUploadRoot))]
[JsonSerializable(typeof(BillRoot))]
[JsonSerializable(typeof(BillsRoot))]
[JsonSerializable(typeof(CapitalAssetRoot))]
[JsonSerializable(typeof(CapitalAssetsRoot))]
[JsonSerializable(typeof(CapitalAssetTypeRoot))]
[JsonSerializable(typeof(CapitalAssetTypesRoot))]
[JsonSerializable(typeof(CashFlowRoot))]
[JsonSerializable(typeof(CashFlowDirection))]
[JsonSerializable(typeof(CashFlowMonthly))]
[JsonSerializable(typeof(CategoriesRoot))]
[JsonSerializable(typeof(CategoryRoot))]
[JsonSerializable(typeof(CategoryCreateRequestRoot))]
[JsonSerializable(typeof(CategoryUpdateRequestRoot))]
[JsonSerializable(typeof(CompanyRoot))]
[JsonSerializable(typeof(ContactRoot))]
[JsonSerializable(typeof(ContactsRoot))]
[JsonSerializable(typeof(CorporationTaxReturnRoot))]
[JsonSerializable(typeof(CorporationTaxReturnsRoot))]
[JsonSerializable(typeof(CorporationTaxReturnFilingRoot))]
[JsonSerializable(typeof(CreditNoteRoot))]
[JsonSerializable(typeof(CreditNotesRoot))]
[JsonSerializable(typeof(CreditNoteRefundRoot))]
[JsonSerializable(typeof(CreditNoteEmailRoot))]
[JsonSerializable(typeof(CurrenciesRoot))]
[JsonSerializable(typeof(DepreciationProfilesRoot))]
[JsonSerializable(typeof(EstimateRoot))]
[JsonSerializable(typeof(EstimatesRoot))]
[JsonSerializable(typeof(EstimateEmailRoot))]
[JsonSerializable(typeof(EstimateItemRoot))]
[JsonSerializable(typeof(EstimateDefaultAdditionalTextRoot))]
[JsonSerializable(typeof(ExpenseRoot))]
[JsonSerializable(typeof(InvoiceRoot))]
[JsonSerializable(typeof(InvoicesRoot))]
[JsonSerializable(typeof(InvoiceEmailRoot))]
[JsonSerializable(typeof(InvoiceTimelineRoot))]
[JsonSerializable(typeof(InvoiceDefaultAdditionalTextRoot))]
[JsonSerializable(typeof(JournalSetRoot))]
[JsonSerializable(typeof(MileageRoot))]
[JsonSerializable(typeof(MileagesRoot))]
[JsonSerializable(typeof(NotesRoot))]
[JsonSerializable(typeof(OpeningBalanceRoot))]
[JsonSerializable(typeof(PayrollPaymentRoot))]
[JsonSerializable(typeof(PayrollPaymentsRoot))]
[JsonSerializable(typeof(PayrollProfileRoot))]
[JsonSerializable(typeof(PayrollProfilesRoot))]
[JsonSerializable(typeof(PayslipRoot))]
[JsonSerializable(typeof(PayslipsRoot))]
[JsonSerializable(typeof(ProfitAndLossRoot))]
[JsonSerializable(typeof(ProjectRoot))]
[JsonSerializable(typeof(ProjectsRoot))]
[JsonSerializable(typeof(PurchaseAgedCreditorsRoot))]
[JsonSerializable(typeof(RecurringInvoiceRoot))]
[JsonSerializable(typeof(RecurringInvoicesRoot))]
[JsonSerializable(typeof(SalesAgedDebtorsRoot))]
[JsonSerializable(typeof(SalesTaxRatesRoot))]
[JsonSerializable(typeof(SelfAssessmentReturnRoot))]
[JsonSerializable(typeof(SelfAssessmentReturnsRoot))]
[JsonSerializable(typeof(SelfAssessmentReturnFilingRoot))]
[JsonSerializable(typeof(StatementUploadRoot))]
[JsonSerializable(typeof(StockItemRoot))]
[JsonSerializable(typeof(StockItemsRoot))]
[JsonSerializable(typeof(TaskRoot))]
[JsonSerializable(typeof(TasksRoot))]
[JsonSerializable(typeof(TimeslipsRoot))]
[JsonSerializable(typeof(TrialBalanceRoot))]
[JsonSerializable(typeof(UserRoot))]
[JsonSerializable(typeof(UsersRoot))]
[JsonSerializable(typeof(VatReturnRoot))]
[JsonSerializable(typeof(VatReturnsRoot))]
[JsonSerializable(typeof(VatReturnFilingRoot))]
[JsonSerializable(typeof(WebhookRoot))]
[JsonSerializable(typeof(WebhooksRoot))]
// Domain models (needed for serialization of request bodies)
[JsonSerializable(typeof(Attachment))]
[JsonSerializable(typeof(BankAccount))]
[JsonSerializable(typeof(BankTransaction))]
[JsonSerializable(typeof(BankTransactionExplanation))]
[JsonSerializable(typeof(BankTransactionUpload))]
[JsonSerializable(typeof(Bill))]
[JsonSerializable(typeof(BillAttachment))]
[JsonSerializable(typeof(BillItem))]
[JsonSerializable(typeof(CapitalAsset))]
[JsonSerializable(typeof(Category))]
[JsonSerializable(typeof(CategoryCreateRequest))]
[JsonSerializable(typeof(CategoryUpdateRequest))]
[JsonSerializable(typeof(CategoryGroupType))]
[JsonSerializable(typeof(CategoryGroupType?))]
[JsonSerializable(typeof(AutoSalesTaxRateType))]
[JsonSerializable(typeof(AutoSalesTaxRateType?))]
[JsonSerializable(typeof(Domain.Company))]
[JsonSerializable(typeof(Contact))]
[JsonSerializable(typeof(CreditNote))]
[JsonSerializable(typeof(Estimate))]
[JsonSerializable(typeof(EstimateEmail))]
[JsonSerializable(typeof(EstimateItem))]
[JsonSerializable(typeof(EstimateDefaultAdditionalText))]
[JsonSerializable(typeof(Expense))]
[JsonSerializable(typeof(Invoice))]
[JsonSerializable(typeof(InvoiceEmail))]
[JsonSerializable(typeof(InvoiceTimelineEntry))]
[JsonSerializable(typeof(InvoiceDefaultAdditionalText))]
[JsonSerializable(typeof(JournalSet))]
[JsonSerializable(typeof(Mileage))]
[JsonSerializable(typeof(OpeningBalance))]
[JsonSerializable(typeof(PayrollPayment))]
[JsonSerializable(typeof(PayrollProfile))]
[JsonSerializable(typeof(Payslip))]
[JsonSerializable(typeof(Project))]
[JsonSerializable(typeof(RecurringInvoice))]
[JsonSerializable(typeof(SalesTaxRate))]
[JsonSerializable(typeof(StockItem))]
[JsonSerializable(typeof(Task))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(Webhook))]
public partial class FreeAgentJsonContext : JsonSerializerContext
{
}
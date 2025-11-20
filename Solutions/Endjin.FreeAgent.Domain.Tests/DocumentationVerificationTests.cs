using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Endjin.FreeAgent.Domain.Tests;

[TestClass]
public class DocumentationVerificationTests
{
    private static readonly string DocsPath = Path.Combine(AppContext.BaseDirectory, "docs");
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new Converters.CompanyTypeJsonConverter(),
            new Converters.SalesTaxRegistrationStatusJsonConverter(),
            new Converters.EcStatusJsonConverter(),
            new Converters.RecurringPatternJsonConverter(),
            new Converters.RoleJsonConverter()
        }
    };

    [TestMethod]
    public void VerifyCompanyDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "company.md"));

        VerifyEndpoint<CompanyRoot>(markdown, "GET https://api.freeagent.com/v2/company");
        VerifyEndpoint<BusinessCategoriesRoot>(markdown, "GET https://api.freeagent.com/v2/company/business_categories");
        VerifyEndpoint<TaxTimelineRoot>(markdown, "GET https://api.freeagent.com/v2/company/tax_timeline");
    }

    [TestMethod]
    public void VerifyInvoicesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "invoices.md"));

        VerifyEndpoint<InvoicesRoot>(markdown, "GET https://api.freeagent.com/v2/invoices");
        VerifyEndpoint<InvoicesRoot>(markdown, "GET https://api.freeagent.com/v2/invoices?nested_invoice_items=true");
        VerifyEndpoint<InvoiceRoot>(markdown, "GET https://api.freeagent.com/v2/invoices/:id");
        VerifyEndpoint<InvoicePdfRoot>(markdown, "GET https://api.freeagent.com/v2/invoices/:id/pdf");
        VerifyEndpoint<InvoiceRoot>(markdown, "POST https://api.freeagent.com/v2/invoices");
        VerifyEndpoint<InvoiceTimelineRoot>(markdown, "GET https://api.freeagent.com/v2/invoices/timeline");
        VerifyEndpoint<InvoiceDefaultAdditionalTextRoot>(markdown, "GET https://api.freeagent.com/v2/invoices/default_additional_text");
    }

    [TestMethod]
    public void VerifyContactsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "contacts.md"));

        VerifyEndpoint<ContactsRoot>(markdown, "GET https://api.freeagent.com/v2/contacts");
        VerifyEndpoint<ContactRoot>(markdown, "GET https://api.freeagent.com/v2/contacts/:id");
        VerifyEndpoint<ContactRoot>(markdown, "POST https://api.freeagent.com/v2/contacts");
    }

    [TestMethod]
    public void VerifyAttachmentsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "attachments.md"));
        VerifyEndpoint<AttachmentRoot>(markdown, "GET https://api.freeagent.com/v2/attachments/:id");
    }

    [TestMethod]
    public void VerifyBalanceSheetDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "balance_sheet.md"));
        VerifyEndpoint<BalanceSheetRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/balance_sheet");
        VerifyEndpoint<BalanceSheetRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/balance_sheet/opening_balances");
    }

    [TestMethod]
    public void VerifyBankAccountsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "bank_accounts.md"));
        VerifyEndpoint<BankAccountsRoot>(markdown, "GET https://api.freeagent.com/v2/bank_accounts");
        VerifyEndpoint<BankAccountsRoot>(markdown, "GET https://api.freeagent.com/v2/bank_accounts?view=standard_bank_accounts");
        VerifyEndpoint<BankAccountRoot>(markdown, "GET https://api.freeagent.com/v2/bank_accounts/:id");
        VerifyEndpoint<BankAccountRoot>(markdown, "POST https://api.freeagent.com/v2/bank_accounts");
    }

    [TestMethod]
    public void VerifyBankFeedsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "bank_feeds.md"));
        VerifyEndpoint<BankFeedsRoot>(markdown, "GET https://api.freeagent.com/v2/bank_feeds");
        VerifyEndpoint<BankFeedRoot>(markdown, "GET https://api.freeagent.com/v2/bank_feeds/:id");
    }

    [TestMethod]
    public void VerifyBankTransactionExplanationsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "bank_transaction_explanations.md"));
        VerifyEndpoint<BankTransactionExplanationsRoot>(markdown, "GET https://api.freeagent.com/v2/bank_transaction_explanations");
        VerifyEndpoint<BankTransactionExplanationRoot>(markdown, "GET https://api.freeagent.com/v2/bank_transaction_explanations/:id");
        VerifyEndpoint<BankTransactionExplanationRoot>(markdown, "POST https://api.freeagent.com/v2/bank_transaction_explanations");
    }

    [TestMethod]
    public void VerifyBankTransactionsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "bank_transactions.md"));
        VerifyEndpoint<BankTransactionsRoot>(markdown, "GET https://api.freeagent.com/v2/bank_transactions");
        VerifyEndpoint<BankTransactionRoot>(markdown, "GET https://api.freeagent.com/v2/bank_transactions/:id");
    }

    [TestMethod]
    public void VerifyBillsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "bills.md"));
        VerifyEndpoint<BillsRoot>(markdown, "GET https://api.freeagent.com/v2/bills");
        VerifyEndpoint<BillRoot>(markdown, "GET https://api.freeagent.com/v2/bills/:id");
        VerifyEndpoint<BillRoot>(markdown, "POST https://api.freeagent.com/v2/bills");
    }

    [TestMethod]
    public void VerifyCapitalAssetsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "capital_assets.md"));
        VerifyEndpoint<CapitalAssetsRoot>(markdown, "GET https://api.freeagent.com/v2/capital_assets");
        VerifyEndpoint<CapitalAssetRoot>(markdown, "GET https://api.freeagent.com/v2/capital_assets/:id");
    }

    [TestMethod]
    public void VerifyCapitalAssetTypesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "capital_asset_types.md"));
        VerifyEndpoint<CapitalAssetTypesRoot>(markdown, "GET https://api.freeagent.com/v2/capital_asset_types");
    }

    [TestMethod]
    public void VerifyCashFlowDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "cashflow.md"));
        VerifyEndpoint<CashFlowRoot>(markdown, "GET https://api.freeagent.com/v2/cashflow?from_date=2019-07-01&to_date=2019-09-30");
    }

    [TestMethod]
    public void VerifyCategoriesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "categories.md"));
        VerifyEndpoint<CategoriesRoot>(markdown, "GET https://api.freeagent.com/v2/categories");
        VerifyEndpoint<CategoriesRoot>(markdown, "GET https://api.freeagent.com/v2/categories?sub_accounts=true");
        VerifyEndpoint<CategoryRoot>(markdown, "GET https://api.freeagent.com/v2/categories/:nominal_code");
        VerifyEndpoint<CategoryRoot>(markdown, "POST https://api.freeagent.com/v2/categories");
    }

    [TestMethod]
    public void VerifyCorporationTaxReturnsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "corporation_tax_returns.md"));
        VerifyEndpoint<CorporationTaxReturnsRoot>(markdown, "GET https://api.freeagent.com/v2/corporation_tax_returns");
        VerifyEndpoint<CorporationTaxReturnRoot>(markdown, "GET https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on");
        VerifyEndpoint<CorporationTaxReturnRoot>(markdown, "PUT https://api.freeagent.com/v2/corporation_tax_returns/:period_ends_on/mark_as_filed");
    }

    [TestMethod]
    public void VerifyCreditNoteReconciliationsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "credit_note_reconciliations.md"));
        VerifyEndpoint<CreditNoteReconciliationsRoot>(markdown, "GET https://api.freeagent.com/v2/credit_note_reconciliations");
        VerifyEndpoint<CreditNoteReconciliationRoot>(markdown, "GET https://api.freeagent.com/v2/credit_note_reconciliations/:id");
        VerifyEndpoint<CreditNoteReconciliationRoot>(markdown, "POST https://api.freeagent.com/v2/credit_note_reconciliations");
    }

    [TestMethod]
    public void VerifyCreditNotesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "credit_notes.md"));
        VerifyEndpoint<CreditNotesRoot>(markdown, "GET https://api.freeagent.com/v2/credit_notes");
        VerifyEndpoint<CreditNoteRoot>(markdown, "GET https://api.freeagent.com/v2/credit_notes/:id");
        VerifyEndpoint<CreditNotePdfRoot>(markdown, "GET https://api.freeagent.com/v2/credit_notes/:id/pdf");
        VerifyEndpoint<CreditNoteRoot>(markdown, "POST https://api.freeagent.com/v2/credit_notes");
    }

    [TestMethod]
    public void VerifyEmailAddressesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "email_addresses.md"));
        VerifyEndpoint<EmailAddressesRoot>(markdown, "GET https://api.freeagent.com/v2/email_addresses");
    }

    [TestMethod]
    public void VerifyEstimatesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "estimates.md"));
        VerifyEndpoint<EstimatesRoot>(markdown, "GET https://api.freeagent.com/v2/estimates");
        VerifyEndpoint<EstimateRoot>(markdown, "GET https://api.freeagent.com/v2/estimates/:id");
        VerifyEndpoint<EstimatePdfRoot>(markdown, "GET https://api.freeagent.com/v2/estimates/:id/pdf");
        VerifyEndpoint<EstimateRoot>(markdown, "POST https://api.freeagent.com/v2/estimates");
        VerifyEndpoint<EstimateDefaultAdditionalTextRoot>(markdown, "GET https://api.freeagent.com/v2/estimates/default_additional_text");
        VerifyEndpoint<EstimateItemRoot>(markdown, "POST https://api.freeagent.com/v2/estimate_items");
    }

    [TestMethod]
    public void VerifyExpensesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "expenses.md"));
        VerifyEndpoint<ExpensesRoot>(markdown, "GET https://api.freeagent.com/v2/expenses");
        VerifyEndpoint<ExpenseRoot>(markdown, "GET https://api.freeagent.com/v2/expenses/:id");
        VerifyEndpoint<ExpenseRoot>(markdown, "POST https://api.freeagent.com/v2/expenses");
        VerifyEndpoint<MileageSettingsRoot>(markdown, "GET https://api.freeagent.com/v2/expenses/mileage_settings");
    }

    [TestMethod]
    public void VerifyFinalAccountsReportsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "final_accounts_reports.md"));
        VerifyEndpoint<FinalAccountsReportsRoot>(markdown, "GET https://api.freeagent.com/v2/final_accounts_reports");
        VerifyEndpoint<FinalAccountsReportRoot>(markdown, "GET https://api.freeagent.com/v2/final_accounts_reports/:period_ends_on");
        VerifyEndpoint<FinalAccountsReportRoot>(markdown, "PUT https://api.freeagent.com/v2/final_accounts_reports/:period_ends_on/mark_as_filed");
    }

    [TestMethod]
    public void VerifyHirePurchasesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "hire_purchases.md"));
        VerifyEndpoint<HirePurchasesRoot>(markdown, "GET https://api.freeagent.com/v2/hire_purchases");
        VerifyEndpoint<HirePurchaseRoot>(markdown, "GET https://api.freeagent.com/v2/hire_purchases/:id");
    }

    [TestMethod]
    public void VerifyJournalSetsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "journal_sets.md"));
        VerifyEndpoint<JournalSetsRoot>(markdown, "GET https://api.freeagent.com/v2/journal_sets");
        VerifyEndpoint<JournalSetRoot>(markdown, "GET https://api.freeagent.com/v2/journal_sets/:id");
        VerifyEndpoint<JournalSetRoot>(markdown, "POST https://api.freeagent.com/v2/journal_sets");
        VerifyEndpoint<JournalSetRoot>(markdown, "GET https://api.freeagent.com/v2/journal_sets/opening_balances");
    }

    [TestMethod]
    public void VerifyNotesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "notes.md"));
        VerifyEndpoint<NotesRoot>(markdown, "GET https://api.freeagent.com/v2/notes");
        VerifyEndpoint<NoteRoot>(markdown, "GET https://api.freeagent.com/v2/notes/1");
        VerifyEndpoint<NoteRoot>(markdown, "POST https://api.freeagent.com/v2/notes");
    }

    [TestMethod]
    public void VerifyPayrollDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "payroll.md"));
        VerifyEndpoint<PayrollYearRoot>(markdown, "GET https://api.freeagent.com/v2/payroll/:year");
        VerifyEndpoint<PayrollPeriodRoot>(markdown, "GET https://api.freeagent.com/v2/payroll/:year/:period");
        VerifyEndpoint<PayrollYearRoot>(markdown, "PUT https://api.freeagent.com/v2/payroll/:year/payments/:payment_date/mark_as_paid");
        VerifyEndpoint<PayrollYearRoot>(markdown, "GET https://api.freeagent.com/v2/payroll/:year/payments/:payment_date/mark_as_unpaid");
    }

    [TestMethod]
    public void VerifyPayrollProfilesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "payroll_profiles.md"));
        VerifyEndpoint<PayrollProfilesRoot>(markdown, "GET https://api.freeagent.com/v2/payroll_profiles/:year");
        VerifyEndpoint<PayrollProfilesRoot>(markdown, "GET https://api.freeagent.com/v2/payroll_profiles/:year?user=https://api.freeagent.com/v2/users/107");
    }

    [TestMethod]
    public void VerifyPriceListItemsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "price_list_items.md"));
        VerifyEndpoint<PriceListItemsRoot>(markdown, "GET https://api.freeagent.com/v2/price_list_items");
        VerifyEndpoint<PriceListItemRoot>(markdown, "GET https://api.freeagent.com/v2/price_list_items/:id");
        VerifyEndpoint<PriceListItemRoot>(markdown, "POST https://api.freeagent.com/v2/price_list_items");
    }

    [TestMethod]
    public void VerifyProfitAndLossDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "profit_and_loss.md"));
        VerifyEndpoint<ProfitAndLossRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/profit_and_loss/summary");
    }

    [TestMethod]
    public void VerifyProjectsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "projects.md"));
        VerifyEndpoint<ProjectsRoot>(markdown, "GET https://api.freeagent.com/v2/projects");
        VerifyEndpoint<ProjectRoot>(markdown, "GET https://api.freeagent.com/v2/projects/:id");
        VerifyEndpoint<ProjectRoot>(markdown, "POST https://api.freeagent.com/v2/projects");
    }

    [TestMethod]
    public void VerifyPropertiesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "properties.md"));
        VerifyEndpoint<PropertiesRoot>(markdown, "GET https://api.freeagent.com/v2/properties");
        VerifyEndpoint<PropertyRoot>(markdown, "GET https://api.freeagent.com/v2/properties/:id");
        VerifyEndpoint<PropertyRoot>(markdown, "POST https://api.freeagent.com/v2/properties");
    }

    [TestMethod]
    public void VerifyRecurringInvoicesDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "recurring_invoices.md"));
        VerifyEndpoint<RecurringInvoicesRoot>(markdown, "GET https://api.freeagent.com/v2/recurring_invoices");
        VerifyEndpoint<RecurringInvoiceRoot>(markdown, "GET https://api.freeagent.com/v2/recurring_invoices/:id");
    }

    [TestMethod]
    public void VerifySalesTaxDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "sales_tax.md"));
        VerifyEndpoint<EcMossSalesTaxRatesRoot>(markdown, "GET https://api.freeagent.com/v2/ec_moss/sales_tax_rates?country=Austria&date=2017-01-01");
    }

    [TestMethod]
    public void VerifySalesTaxPeriodsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "sales_tax_periods.md"));
        VerifyEndpoint<SalesTaxPeriodsRoot>(markdown, "GET https://api.freeagent.com/v2/sales_tax_periods");
        VerifyEndpoint<SalesTaxPeriodRoot>(markdown, "GET https://api.freeagent.com/v2/sales_tax_periods/:id");
        VerifyEndpoint<SalesTaxPeriodRoot>(markdown, "POST https://api.freeagent.com/v2/sales_tax_periods");
    }

    [TestMethod]
    public void VerifySelfAssessmentReturnsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "self_assessment_returns.md"));
        VerifyEndpoint<SelfAssessmentReturnsRoot>(markdown, "GET https://api.freeagent.com/v2/users/:user_id/self_assessment_returns");
        VerifyEndpoint<SelfAssessmentReturnRoot>(markdown, "GET https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on");
        VerifyEndpoint<SelfAssessmentReturnRoot>(markdown, "PUT https://api.freeagent.com/v2/users/:user_id/self_assessment_returns/:period_ends_on/mark_as_filed");
    }

    [TestMethod]
    public void VerifyStockItemsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "stock_items.md"));
        VerifyEndpoint<StockItemsRoot>(markdown, "GET https://api.freeagent.com/v2/stock_items");
        VerifyEndpoint<StockItemRoot>(markdown, "GET https://api.freeagent.com/v2/stock_items/:id");
    }

    [TestMethod]
    public void VerifyTasksDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "tasks.md"));
        VerifyEndpoint<TasksRoot>(markdown, "GET https://api.freeagent.com/v2/tasks");
        VerifyEndpoint<TaskRoot>(markdown, "GET https://api.freeagent.com/v2/tasks/:id");
        VerifyEndpoint<TaskRoot>(markdown, "POST https://api.freeagent.com/v2/tasks");
    }

    [TestMethod]
    public void VerifyTimeslipsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "timeslips.md"));
        VerifyEndpoint<TimeslipsRoot>(markdown, "GET https://api.freeagent.com/v2/timeslips");
        VerifyEndpoint<TimeslipRoot>(markdown, "GET https://api.freeagent.com/v2/timeslips/:id");
        VerifyEndpoint<TimeslipRoot>(markdown, "POST https://api.freeagent.com/v2/timeslips");
    }

    [TestMethod]
    public void VerifyTransactionsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "transactions.md"));
        VerifyEndpoint<TransactionsRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/transactions");
        VerifyEndpoint<TransactionRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/transactions/:id");
    }

    [TestMethod]
    public void VerifyTrialBalanceDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "trial_balance.md"));
        VerifyEndpoint<TrialBalanceSummaryRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/trial_balance/summary");
        VerifyEndpoint<TrialBalanceSummaryRoot>(markdown, "GET https://api.freeagent.com/v2/accounting/trial_balance/summary/opening_balances");
    }

    [TestMethod]
    public void VerifyUsersDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "users.md"));
        VerifyEndpoint<UsersRoot>(markdown, "GET https://api.freeagent.com/v2/users");
        VerifyEndpoint<UserRoot>(markdown, "GET https://api.freeagent.com/v2/users/:id");
        VerifyEndpoint<UserRoot>(markdown, "GET https://api.freeagent.com/v2/users/me");
        VerifyEndpoint<UserRoot>(markdown, "POST https://api.freeagent.com/v2/users");
    }

    [TestMethod]
    public void VerifyVatReturnsDocs()
    {
        string markdown = File.ReadAllText(Path.Combine(DocsPath, "vat_returns.md"));
        VerifyEndpoint<VatReturnsRoot>(markdown, "GET https://api.freeagent.com/v2/vat_returns");
        VerifyEndpoint<VatReturnRoot>(markdown, "GET https://api.freeagent.com/v2/vat_returns/:period_ends_on");
        VerifyEndpoint<VatReturnRoot>(markdown, "PUT https://api.freeagent.com/v2/vat_returns/:period_ends_on/mark_as_filed");
    }

    private void VerifyEndpoint<T>(string markdown, string endpoint) where T : class
    {
        int endpointIndex = markdown.IndexOf(endpoint);
        if (endpointIndex == -1)
        {
            throw new Exception($"Could not find endpoint {endpoint}");
        }

        string jsonStartMarker = "```json";
        int currentIndex = endpointIndex;
        string? lastJson = null;
        Exception? lastException = null;

        while (true)
        {
            int jsonStartIndex = markdown.IndexOf(jsonStartMarker, currentIndex);
            if (jsonStartIndex == -1)
            {
                if (lastException != null)
                {
                    throw new Exception($"Failed to deserialize any JSON for {endpoint} into {typeof(T).Name}. Last JSON: {lastJson}", lastException);
                }
                throw new Exception($"Could not find JSON for {endpoint}");
            }

            int jsonEndIndex = markdown.IndexOf("```", jsonStartIndex + jsonStartMarker.Length);
            if (jsonEndIndex == -1)
            {
                throw new Exception($"Could not find end of JSON block for {endpoint}");
            }

            string json = markdown.Substring(jsonStartIndex + jsonStartMarker.Length, jsonEndIndex - (jsonStartIndex + jsonStartMarker.Length)).Trim();
            
            // Skip blocks that don't look like JSON objects or arrays
            if (!json.StartsWith("{") && !json.StartsWith("["))
            {
                currentIndex = jsonEndIndex;
                continue;
            }

            lastJson = json;

            try
            {
                T? result = JsonSerializer.Deserialize<T>(json, Options);
                result.ShouldNotBeNull();
                return; // Success!
            }
            catch (JsonException ex)
            {
                lastException = ex;
                // Failed to deserialize, try the next block
                currentIndex = jsonEndIndex;
            }
        }
    }
}
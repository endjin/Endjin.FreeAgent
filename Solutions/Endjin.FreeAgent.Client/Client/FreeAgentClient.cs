// <copyright file="FreeAgentClient.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
public class FreeAgentClient : ClientBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAgentClient"/> class using options.
    /// </summary>
    /// <param name="options">The FreeAgent configuration options.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="httpClientFactory">The HTTP client factory for creating managed HTTP clients.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    public FreeAgentClient(FreeAgentOptions options, IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        options.Validate();

        this.ClientId = options.ClientId;
        this.ClientSecret = options.ClientSecret;
        this.RefreshToken = options.RefreshToken;

        this.InitializeServices(cache, httpClientFactory, loggerFactory);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAgentClient"/> class using IOptions.
    /// </summary>
    /// <param name="options">The FreeAgent configuration options from DI.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="httpClientFactory">The HTTP client factory for creating managed HTTP clients.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    public FreeAgentClient(IOptions<FreeAgentOptions> options, IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        : this(options?.Value ?? throw new ArgumentNullException(nameof(options)), cache, httpClientFactory, loggerFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAgentClient"/> class with explicit parameters.
    /// </summary>
    /// <param name="clientId">The OAuth2 client ID.</param>
    /// <param name="clientSecret">The OAuth2 client secret.</param>
    /// <param name="refreshToken">The OAuth2 refresh token.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="httpClientFactory">The HTTP client factory for creating managed HTTP clients.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    public FreeAgentClient(string clientId, string clientSecret, string refreshToken, IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSecret);
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        this.ClientId = clientId;
        this.ClientSecret = clientSecret;
        this.RefreshToken = refreshToken;

        this.InitializeServices(cache, httpClientFactory, loggerFactory);
    }

    private void InitializeServices(IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        // Set the memory cache for OAuth2 token management
        this.SetMemoryCache(cache);

        // Set the HTTP client factory - required for proper resource management
        this.SetHttpClientFactory(httpClientFactory);

        // Set the logger factory - required for creating loggers
        this.SetLoggerFactory(loggerFactory);

        this.AgedDebtorsAndCreditors = new AgedDebtorsAndCreditors(this, cache);
        this.Attachments = new Attachments(this, cache);
        this.BalanceSheetReports = new BalanceSheetReports(this, cache);
        this.BankAccounts = new BankAccounts(this, cache);
        this.BankStatementUploads = new BankStatementUploads(this, cache);
        this.BankTransactions = new BankTransactions(this, cache);
        this.BankTransactionExplanations = new BankTransactionExplanations(this, cache);
        this.Bills = new Bills(this, cache);
        this.CapitalAssets = new CapitalAssets(this, cache);
        this.CapitalAssetTypes = new CapitalAssetTypes(this, cache);
        this.CashFlowReports = new CashFlowReports(this, cache);
        this.Categories = new Categories(this, cache);
        this.Currencies = new Currencies(this, cache);
        this.Company = new Company(this, cache);
        this.Contacts = new Contacts(this, cache);
        this.CorporationTaxReturns = new CorporationTaxReturns(this, cache);
        this.CreditNotes = new CreditNotes(this, cache);
        this.DepreciationProfiles = new DepreciationProfiles(this, cache);
        this.Estimates = new Estimates(this, cache);
        this.Expenses = new Expenses(this);
        this.Invoices = new Invoices(this, cache);
        this.JournalSets = new JournalSets(this);
        this.Mileages = new Mileages(this, cache);
        this.Notes = new Notes(this, cache);
        this.OpeningBalances = new OpeningBalances(this, cache);
        this.Payroll = new Payroll(this, cache);
        this.PayrollProfiles = new PayrollProfiles(this, cache);
        this.Payslips = new Payslips(this, cache);
        this.ProfitAndLossReports = new ProfitAndLossReports(this, cache);
        this.Projects = new Projects(this, cache);
        this.RecurringInvoices = new RecurringInvoices(this, cache);
        this.SalesTaxRates = new SalesTaxRates(this, cache);
        this.SelfAssessmentReturns = new SelfAssessmentReturns(this, cache);
        this.StockItems = new StockItems(this, cache);
        this.Tasks = new Tasks(this, cache);
        this.Timeslips = new Timeslips(this, cache);
        this.TrialBalances = new TrialBalances(this, cache);
        this.Users = new Users(this, cache);
        this.VatReturns = new VatReturns(this, cache);
        this.Webhooks = new Webhooks(this, cache);
    }

    // These properties are initialized in InitializeServices which is called from all constructors
    public AgedDebtorsAndCreditors AgedDebtorsAndCreditors { get; private set; }

    public Attachments Attachments { get; private set; }

    public BalanceSheetReports BalanceSheetReports { get; private set; }

    public BankAccounts BankAccounts { get; private set; }

    public BankStatementUploads BankStatementUploads { get; private set; }

    public BankTransactions BankTransactions { get; private set; }

    public BankTransactionExplanations BankTransactionExplanations { get; private set; }

    public Bills Bills { get; private set; }

    public CapitalAssets CapitalAssets { get; private set; }

    public CapitalAssetTypes CapitalAssetTypes { get; private set; }

    public CashFlowReports CashFlowReports { get; private set; }

    public Categories Categories { get; private set; }

    public Currencies Currencies { get; private set; }

    public Company Company { get; private set; }

    public Contacts Contacts { get; private set; }

    public CorporationTaxReturns CorporationTaxReturns { get; private set; }

    public CreditNotes CreditNotes { get; private set; }

    public DepreciationProfiles DepreciationProfiles { get; private set; }

    public Estimates Estimates { get; private set; }

    public Expenses Expenses { get; private set; }

    public Invoices Invoices { get; private set; }

    public JournalSets JournalSets { get; private set; }

    public Mileages Mileages { get; private set; }

    public Notes Notes { get; private set; }

    public OpeningBalances OpeningBalances { get; private set; }

    public Payroll Payroll { get; private set; }

    public PayrollProfiles PayrollProfiles { get; private set; }

    public Payslips Payslips { get; private set; }

    public ProfitAndLossReports ProfitAndLossReports { get; private set; }

    public Projects Projects { get; private set; }

    public RecurringInvoices RecurringInvoices { get; private set; }

    public SalesTaxRates SalesTaxRates { get; private set; }

    public SelfAssessmentReturns SelfAssessmentReturns { get; private set; }

    public StockItems StockItems { get; private set; }

    public Tasks Tasks { get; private set; }

    public Timeslips Timeslips { get; private set; }

    public TrialBalances TrialBalances { get; private set; }

    public Users Users { get; private set; }

    public VatReturns VatReturns { get; private set; }

    public Webhooks Webhooks { get; private set; }
}
#pragma warning restore CS8618
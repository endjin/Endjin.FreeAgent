// <copyright file="FreeAgentClient.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Client.OAuth2;
using Microsoft.Extensions.Logging;

namespace Endjin.FreeAgent.Client;

/// <summary>
/// Main client for interacting with the FreeAgent API, providing access to all API endpoints.
/// </summary>
/// <remarks>
/// <para>
/// This class serves as the entry point for all FreeAgent API operations. It provides
/// strongly-typed access to all FreeAgent API resources through dedicated service properties
/// such as <see cref="Contacts"/>, <see cref="Invoices"/>, <see cref="BankTransactions"/>, etc.
/// </para>
/// <para>
/// The client requires the following dependencies:
/// <list type="bullet">
/// <item><description><see cref="IHttpClientFactory"/> for creating managed HTTP clients</description></item>
/// <item><description><see cref="IMemoryCache"/> for caching OAuth2 tokens and API responses</description></item>
/// <item><description><see cref="ILoggerFactory"/> for logging operations and diagnostics</description></item>
/// <item><description>OAuth2 credentials (client ID, client secret, refresh token)</description></item>
/// </list>
/// </para>
/// <para>
/// The client automatically handles OAuth2 authentication, token refresh, and request retries.
/// It supports both direct instantiation and dependency injection through
/// <c>FreeAgentClientServiceCollectionExtensions.AddFreeAgentClientServices</c>.
/// </para>
/// <example>
/// Using dependency injection:
/// <code>
/// services.AddFreeAgentClientServices(configuration);
/// // Then inject FreeAgentClient in your classes
/// </code>
/// Direct instantiation:
/// <code>
/// FreeAgentClient client = new(
///     clientId: "your-client-id",
///     clientSecret: "your-client-secret",
///     refreshToken: "your-refresh-token",
///     cache: memoryCache,
///     httpClientFactory: httpClientFactory,
///     loggerFactory: loggerFactory);
///
/// var contacts = await client.Contacts.GetAllAsync();
/// </code>
/// </example>
/// </remarks>
/// <seealso cref="ClientBase"/>
/// <seealso cref="FreeAgentOptions"/>
/// <seealso cref="FreeAgentClientServiceCollectionExtensions"/>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor
public class FreeAgentClient : ClientBase
{
    /// <summary>
    /// Creates a new <see cref="FreeAgentClient"/> configured for interactive authentication.
    /// </summary>
    /// <param name="clientId">The OAuth2 client ID.</param>
    /// <param name="clientSecret">The OAuth2 client secret.</param>
    /// <param name="useSandbox">Whether to use the sandbox environment.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="refreshToken">Optional initial refresh token.</param>
    /// <returns>A configured <see cref="FreeAgentClient"/> instance.</returns>
    public static FreeAgentClient CreateInteractive(
        string clientId,
        string clientSecret,
        bool useSandbox,
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory,
        string? refreshToken = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientSecret);
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        Uri apiBaseUrl = useSandbox ? FreeAgentOptions.SandboxApiBaseUrl : FreeAgentOptions.ProductionApiBaseUrl;

        OAuth2Options oauth2Options = new()
        {
            ClientId = clientId,
            ClientSecret = clientSecret,
            AuthorizationEndpoint = new Uri(apiBaseUrl, "/v2/approve_app"),
            TokenEndpoint = new Uri(apiBaseUrl, "/v2/token_endpoint"),
            UsePkce = true
        };

        HttpClient httpClient = httpClientFactory.CreateClient("FreeAgentAuth");

        ILogger<InteractiveLoginHelper> loginLogger = loggerFactory.CreateLogger<InteractiveLoginHelper>();
        InteractiveLoginHelper loginHelper = new(oauth2Options, httpClient, loginLogger);

        ILogger<OAuth2Service> oauth2Logger = loggerFactory.CreateLogger<OAuth2Service>();
        OAuth2Service oauth2Service = new(oauth2Options, httpClient, cache, oauth2Logger);

        ILogger<InteractiveAuthenticationProvider> providerLogger = loggerFactory.CreateLogger<InteractiveAuthenticationProvider>();
        InteractiveAuthenticationProvider provider = new(loginHelper, oauth2Service, providerLogger, refreshToken);

        return new FreeAgentClient(provider, cache, httpClientFactory, loggerFactory, apiBaseUrl);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAgentClient"/> class using configuration options.
    /// </summary>
    /// <param name="options">The FreeAgent configuration options containing OAuth2 credentials.</param>
    /// <param name="cache">The memory cache instance for caching tokens and API responses.</param>
    /// <param name="httpClientFactory">The HTTP client factory for creating managed HTTP clients.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the options are invalid (via <see cref="FreeAgentOptions.Validate"/>).</exception>
    public FreeAgentClient(FreeAgentOptions options, IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        options.Validate();

        this.ApiBaseUrl = options.ApiBaseUrl;

        OAuth2Options oauth2Options = new()
        {
            ClientId = options.ClientId,
            ClientSecret = options.ClientSecret,
            RefreshToken = options.RefreshToken,
            TokenEndpoint = new Uri(this.ApiBaseUrl, "/v2/token_endpoint"),
            AuthorizationEndpoint = new Uri(this.ApiBaseUrl, "/v2/approve_app"),
        };

        HttpClient httpClientNoAuth = httpClientFactory.CreateClient("FreeAgentNoAuth");
        InitializeHttpClient(httpClientNoAuth);

        ILogger<OAuth2Service> logger = loggerFactory.CreateLogger<OAuth2Service>();
        OAuth2Service oauth2Service = new(oauth2Options, httpClientNoAuth, cache, logger);

        this.AuthenticationProvider = new OAuth2AuthenticationProvider(oauth2Service);

        this.InitializeServices(cache, httpClientFactory, loggerFactory);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAgentClient"/> class using the options pattern from dependency injection.
    /// </summary>
    /// <param name="options">The FreeAgent configuration options wrapped in IOptions from the dependency injection container.</param>
    /// <param name="cache">The memory cache instance for caching tokens and API responses.</param>
    /// <param name="httpClientFactory">The HTTP client factory for creating managed HTTP clients.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the options are invalid (via <see cref="FreeAgentOptions.Validate"/>).</exception>
    /// <remarks>
    /// This constructor is typically used when the client is resolved from the dependency injection container
    /// after calling <c>FreeAgentClientServiceCollectionExtensions.AddFreeAgentClientServices</c>.
    /// </remarks>
    public FreeAgentClient(IOptions<FreeAgentOptions> options, IMemoryCache cache, IHttpClientFactory httpClientFactory, ILoggerFactory loggerFactory)
        : this(options?.Value ?? throw new ArgumentNullException(nameof(options)), cache, httpClientFactory, loggerFactory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FreeAgentClient"/> class with a custom authentication provider.
    /// </summary>
    /// <param name="authenticationProvider">The authentication provider to use.</param>
    /// <param name="cache">The memory cache instance.</param>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="apiBaseUrl">The API base URL (optional).</param>
    public FreeAgentClient(
        IAuthenticationProvider authenticationProvider,
        IMemoryCache cache,
        IHttpClientFactory httpClientFactory,
        ILoggerFactory loggerFactory,
        Uri? apiBaseUrl = null)
    {
        ArgumentNullException.ThrowIfNull(authenticationProvider);
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        this.AuthenticationProvider = authenticationProvider;
        this.ApiBaseUrl = apiBaseUrl ?? FreeAgentOptions.ProductionApiBaseUrl;

        this.InitializeServices(cache, httpClientFactory, loggerFactory);
    }

    /// <summary>
    /// Initializes all API service instances and configures dependencies.
    /// </summary>
    /// <param name="cache">The memory cache for caching tokens and API responses.</param>
    /// <param name="httpClientFactory">The HTTP client factory for creating HTTP clients.</param>
    /// <param name="loggerFactory">The logger factory for creating loggers.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClientFactory"/> or <paramref name="loggerFactory"/> is null.</exception>
    /// <remarks>
    /// This method is called by all constructors to set up the HTTP client factory, logger factory,
    /// and memory cache, then initializes all 39+ API service properties (Contacts, Invoices, etc.).
    /// Each service is configured with a reference to this client instance and the shared cache.
    /// </remarks>
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
        this.BankFeeds = new BankFeeds(this, cache);
        this.BankStatementUploads = new BankStatementUploads(this, cache);
        this.BankTransactions = new BankTransactions(this, cache);
        this.BankTransactionExplanations = new BankTransactionExplanations(this, cache);
        this.Bills = new Bills(this, cache);
        this.CapitalAssets = new CapitalAssets(this, cache);
        this.CapitalAssetTypes = new CapitalAssetTypes(this, cache);
        this.CashFlowReports = new CashFlowReports(this, cache);
        this.Categories = new Categories(this, cache);
        this.EmailAddresses = new EmailAddresses(this, cache);
        this.Company = new Company(this, cache);
        this.Contacts = new Contacts(this, cache);
        this.CorporationTaxReturns = new CorporationTaxReturns(this, cache);
        this.CreditNoteReconciliations = new CreditNoteReconciliations(this, cache);
        this.CreditNotes = new CreditNotes(this, cache);
        this.DepreciationProfiles = new DepreciationProfiles(this, cache);
        this.EcMossSalesTaxRates = new EcMossSalesTaxRates(this, cache);
        this.Estimates = new Estimates(this, cache);
        this.Expenses = new Expenses(this, cache);
        this.FinalAccountsReports = new FinalAccountsReports(this, cache);
        this.HirePurchases = new HirePurchases(this, cache);
        this.Invoices = new Invoices(this, cache);
        this.InvoiceSettings = new InvoiceSettings(this, cache);
        this.JournalSets = new JournalSets(this);
        this.Mileages = new Mileages(this, cache);
        this.Notes = new Notes(this, cache);
        this.OpeningBalances = new OpeningBalances(this, cache);
        this.Payroll = new Payroll(this, cache);
        this.PriceListItems = new PriceListItems(this, cache);
        this.ProfitAndLossReports = new ProfitAndLossReports(this, cache);
        this.Projects = new Projects(this, cache);
        this.Properties = new Properties(this, cache);
        this.RecurringInvoices = new RecurringInvoices(this, cache);
        this.SalesTaxRates = new SalesTaxRates(this, cache);
        this.SalesTaxPeriods = new SalesTaxPeriods(this);
        this.SelfAssessmentReturns = new SelfAssessmentReturns(this, cache);
        this.StockItems = new StockItems(this, cache);
        this.Tasks = new Tasks(this, cache);
        this.Timeslips = new Timeslips(this, cache);
        this.Transactions = new Transactions(this, cache);
        this.TrialBalances = new TrialBalances(this, cache);
        this.Users = new Users(this, cache);
        this.VatReturns = new VatReturns(this, cache);
        this.Webhooks = new Webhooks(this, cache);
    }

    // API Service Properties - All properties are initialized in InitializeServices which is called from all constructors

    /// <summary>
    /// Gets the service for accessing aged debtors and creditors reports.
    /// </summary>
    /// <value>The <see cref="AgedDebtorsAndCreditors"/> service instance.</value>
    public AgedDebtorsAndCreditors AgedDebtorsAndCreditors { get; private set; }

    /// <summary>
    /// Gets the service for managing file attachments.
    /// </summary>
    /// <value>The <see cref="Attachments"/> service instance.</value>
    public Attachments Attachments { get; private set; }

    /// <summary>
    /// Gets the service for accessing balance sheet reports.
    /// </summary>
    /// <value>The <see cref="BalanceSheetReports"/> service instance.</value>
    public BalanceSheetReports BalanceSheetReports { get; private set; }

    /// <summary>
    /// Gets the service for managing bank accounts.
    /// </summary>
    /// <value>The <see cref="BankAccounts"/> service instance.</value>
    public BankAccounts BankAccounts { get; private set; }

    /// <summary>
    /// Gets the service for accessing bank feeds.
    /// </summary>
    /// <value>The <see cref="BankFeeds"/> service instance.</value>
    public BankFeeds BankFeeds { get; private set; }

    /// <summary>
    /// Gets the service for uploading bank statements in OFX, QIF, or CSV format.
    /// </summary>
    /// <value>The <see cref="BankStatementUploads"/> service instance.</value>
    public BankStatementUploads BankStatementUploads { get; private set; }

    /// <summary>
    /// Gets the service for managing bank transactions.
    /// </summary>
    /// <value>The <see cref="BankTransactions"/> service instance.</value>
    public BankTransactions BankTransactions { get; private set; }

    /// <summary>
    /// Gets the service for managing bank transaction explanations and categorization.
    /// </summary>
    /// <value>The <see cref="BankTransactionExplanations"/> service instance.</value>
    public BankTransactionExplanations BankTransactionExplanations { get; private set; }

    /// <summary>
    /// Gets the service for managing bills (purchase invoices).
    /// </summary>
    /// <value>The <see cref="Bills"/> service instance.</value>
    public Bills Bills { get; private set; }

    /// <summary>
    /// Gets the service for managing capital assets.
    /// </summary>
    /// <value>The <see cref="CapitalAssets"/> service instance.</value>
    public CapitalAssets CapitalAssets { get; private set; }

    /// <summary>
    /// Gets the service for managing capital asset types and categories.
    /// </summary>
    /// <value>The <see cref="CapitalAssetTypes"/> service instance.</value>
    public CapitalAssetTypes CapitalAssetTypes { get; private set; }

    /// <summary>
    /// Gets the service for accessing cash flow reports.
    /// </summary>
    /// <value>The <see cref="CashFlowReports"/> service instance.</value>
    public CashFlowReports CashFlowReports { get; private set; }

    /// <summary>
    /// Gets the service for accessing accounting categories.
    /// </summary>
    /// <value>The <see cref="Categories"/> service instance.</value>
    public Categories Categories { get; private set; }

    /// <summary>
    /// Gets the service for accessing verified sender email addresses.
    /// </summary>
    /// <value>The <see cref="EmailAddresses"/> service instance.</value>
    public EmailAddresses EmailAddresses { get; private set; }

    /// <summary>
    /// Gets the service for accessing company information.
    /// </summary>
    /// <value>The <see cref="Company"/> service instance.</value>
    public Company Company { get; private set; }

    /// <summary>
    /// Gets the service for managing contacts (customers, suppliers, etc.).
    /// </summary>
    /// <value>The <see cref="Contacts"/> service instance.</value>
    public Contacts Contacts { get; private set; }

    /// <summary>
    /// Gets the service for managing corporation tax returns.
    /// </summary>
    /// <value>The <see cref="CorporationTaxReturns"/> service instance.</value>
    public CorporationTaxReturns CorporationTaxReturns { get; private set; }

    /// <summary>
    /// Gets the service for managing credit note reconciliations.
    /// </summary>
    /// <value>The <see cref="CreditNoteReconciliations"/> service instance.</value>
    public CreditNoteReconciliations CreditNoteReconciliations { get; private set; }

    /// <summary>
    /// Gets the service for managing credit notes.
    /// </summary>
    /// <value>The <see cref="CreditNotes"/> service instance.</value>
    public CreditNotes CreditNotes { get; private set; }

    /// <summary>
    /// Gets the service for managing depreciation profiles for capital assets.
    /// </summary>
    /// <value>The <see cref="DepreciationProfiles"/> service instance.</value>
    public DepreciationProfiles DepreciationProfiles { get; private set; }

    /// <summary>
    /// Gets the service for accessing EC MOSS (Mini One Stop Shop) sales tax rates for EU member states.
    /// </summary>
    /// <value>The <see cref="EcMossSalesTaxRates"/> service instance.</value>
    public EcMossSalesTaxRates EcMossSalesTaxRates { get; private set; }

    /// <summary>
    /// Gets the service for managing estimates (quotes).
    /// </summary>
    /// <value>The <see cref="Estimates"/> service instance.</value>
    public Estimates Estimates { get; private set; }

    /// <summary>
    /// Gets the service for managing expense claims.
    /// </summary>
    /// <value>The <see cref="Expenses"/> service instance.</value>
    public Expenses Expenses { get; private set; }

    /// <summary>
    /// Gets the service for managing Final Accounts reports (statutory accounts).
    /// </summary>
    /// <value>The <see cref="FinalAccountsReports"/> service instance.</value>
    public FinalAccountsReports FinalAccountsReports { get; private set; }

    /// <summary>
    /// Gets the service for accessing hire purchase records.
    /// </summary>
    /// <value>The <see cref="HirePurchases"/> service instance.</value>
    /// <remarks>
    /// This endpoint is only available for UK companies.
    /// </remarks>
    public HirePurchases HirePurchases { get; private set; }

    /// <summary>
    /// Gets the service for managing invoices (sales invoices).
    /// </summary>
    /// <value>The <see cref="Invoices"/> service instance.</value>
    public Invoices Invoices { get; private set; }

    /// <summary>
    /// Gets the service for managing invoice settings and templates.
    /// </summary>
    /// <value>The <see cref="InvoiceSettings"/> service instance.</value>
    public InvoiceSettings InvoiceSettings { get; private set; }

    /// <summary>
    /// Gets the service for managing journal sets and manual journal entries.
    /// </summary>
    /// <value>The <see cref="JournalSets"/> service instance.</value>
    public JournalSets JournalSets { get; private set; }

    /// <summary>
    /// Gets the service for managing mileage expenses.
    /// </summary>
    /// <value>The <see cref="Mileages"/> service instance.</value>
    public Mileages Mileages { get; private set; }

    /// <summary>
    /// Gets the service for managing notes attached to various entities.
    /// </summary>
    /// <value>The <see cref="Notes"/> service instance.</value>
    public Notes Notes { get; private set; }

    /// <summary>
    /// Gets the service for managing opening balances.
    /// </summary>
    /// <value>The <see cref="OpeningBalances"/> service instance.</value>
    public OpeningBalances OpeningBalances { get; private set; }

    /// <summary>
    /// Gets the service for accessing UK RTI payroll data.
    /// </summary>
    /// <value>The <see cref="Payroll"/> service instance.</value>
    public Payroll Payroll { get; private set; }

    /// <summary>
    /// Gets the service for managing price list items.
    /// </summary>
    /// <value>The <see cref="PriceListItems"/> service instance.</value>
    public PriceListItems PriceListItems { get; private set; }

    /// <summary>
    /// Gets the service for accessing profit and loss reports.
    /// </summary>
    /// <value>The <see cref="ProfitAndLossReports"/> service instance.</value>
    public ProfitAndLossReports ProfitAndLossReports { get; private set; }

    /// <summary>
    /// Gets the service for managing projects.
    /// </summary>
    /// <value>The <see cref="Projects"/> service instance.</value>
    public Projects Projects { get; private set; }

    /// <summary>
    /// Gets the service for managing properties for UK unincorporated landlords.
    /// </summary>
    /// <value>The <see cref="Properties"/> service instance.</value>
    /// <remarks>
    /// This endpoint is only available for companies of type <c>UkUnincorporatedLandlord</c>.
    /// </remarks>
    public Properties Properties { get; private set; }

    /// <summary>
    /// Gets the service for managing recurring invoices.
    /// </summary>
    /// <value>The <see cref="RecurringInvoices"/> service instance.</value>
    public RecurringInvoices RecurringInvoices { get; private set; }

    /// <summary>
    /// Gets the service for accessing sales tax rates.
    /// </summary>
    /// <value>The <see cref="SalesTaxRates"/> service instance.</value>
    public SalesTaxRates SalesTaxRates { get; private set; }

    /// <summary>
    /// Gets the service for managing sales tax periods.
    /// </summary>
    /// <value>The <see cref="SalesTaxPeriods"/> service instance.</value>
    public SalesTaxPeriods SalesTaxPeriods { get; private set; }

    /// <summary>
    /// Gets the service for managing self assessment tax returns.
    /// </summary>
    /// <value>The <see cref="SelfAssessmentReturns"/> service instance.</value>
    public SelfAssessmentReturns SelfAssessmentReturns { get; private set; }

    /// <summary>
    /// Gets the service for managing stock items and inventory.
    /// </summary>
    /// <value>The <see cref="StockItems"/> service instance.</value>
    public StockItems StockItems { get; private set; }

    /// <summary>
    /// Gets the service for managing project tasks.
    /// </summary>
    /// <value>The <see cref="Tasks"/> service instance.</value>
    public Tasks Tasks { get; private set; }

    /// <summary>
    /// Gets the service for managing time tracking entries.
    /// </summary>
    /// <value>The <see cref="Timeslips"/> service instance.</value>
    public Timeslips Timeslips { get; private set; }

    /// <summary>
    /// Gets the service for accessing accounting transactions.
    /// </summary>
    /// <value>The <see cref="Transactions"/> service instance.</value>
    public Transactions Transactions { get; private set; }

    /// <summary>
    /// Gets the service for accessing trial balance reports.
    /// </summary>
    /// <value>The <see cref="TrialBalances"/> service instance.</value>
    public TrialBalances TrialBalances { get; private set; }

    /// <summary>
    /// Gets the service for managing user accounts.
    /// </summary>
    /// <value>The <see cref="Users"/> service instance.</value>
    public Users Users { get; private set; }

    /// <summary>
    /// Gets the service for managing VAT returns.
    /// </summary>
    /// <value>The <see cref="VatReturns"/> service instance.</value>
    public VatReturns VatReturns { get; private set; }

    /// <summary>
    /// Gets the service for managing webhooks for API event notifications.
    /// </summary>
    /// <value>The <see cref="Webhooks"/> service instance.</value>
    public Webhooks Webhooks { get; private set; }
}
#pragma warning restore CS8618
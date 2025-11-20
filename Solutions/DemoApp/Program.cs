// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using DemoApp;
using Endjin.FreeAgent.Client;
using Endjin.FreeAgent.Client.OAuth2;
using Endjin.FreeAgent.Domain;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console;

using Company = Endjin.FreeAgent.Domain.Company;

// Build configuration
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true)
    .Build();

// Check if we should run in interactive login mode
bool useInteractiveLogin = args.Contains("--interactive-login") || args.Contains("-i");

if (useInteractiveLogin)
{
    // Interactive Login Mode
    Console.WriteLine("=== FreeAgent Interactive Login Mode ===\n");

    string? clientId = configuration["FreeAgent:ClientId"];
    string? clientSecret = configuration["FreeAgent:ClientSecret"];

    if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
    {
        Console.WriteLine("Error: ClientId and ClientSecret must be configured in appsettings.json");
        Console.WriteLine("\nPlease ensure your appsettings.json contains:");
        Console.WriteLine("{");
        Console.WriteLine("  \"FreeAgent\": {");
        Console.WriteLine("    \"ClientId\": \"your-client-id\",");
        Console.WriteLine("    \"ClientSecret\": \"your-client-secret\"");
        Console.WriteLine("  }");
        Console.WriteLine("}");
        return;
    }

    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });

    var logger = loggerFactory.CreateLogger<Program>();

    try
    {
        InteractiveLoginResult result = await InteractiveLoginExample.PerformInteractiveLoginAsync(
            clientId,
            clientSecret,
            logger);

        // Optionally, test the token by making a simple API call
        Console.WriteLine("\n=== Testing the tokens ===");

        var testHost = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddMemoryCache();
                services.AddHttpClient();
                services.AddLogging(builder => builder.AddConsole());
            })
            .Build();

        var cache = testHost.Services.GetRequiredService<IMemoryCache>();
        var httpClientFactory = testHost.Services.GetRequiredService<IHttpClientFactory>();
        var testLoggerFactory = testHost.Services.GetRequiredService<ILoggerFactory>();

        var testClient = new FreeAgentClient(
            clientId,
            clientSecret,
            result.RefreshToken,
            cache,
            httpClientFactory,
            testLoggerFactory);

        await testClient.InitializeAndAuthorizeAsync();

        var contacts = await testClient.Contacts.GetAllAsync();
        Console.WriteLine($"✅ Successfully retrieved {contacts.Count()} contacts from FreeAgent!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\n❌ Error during interactive login: {ex.Message}");
        logger.LogError(ex, "Interactive login failed");
    }
}
else
{
    // Standard Mode (using existing refresh token)
    Console.WriteLine("=== FreeAgent Standard Mode ===");
    Console.WriteLine("Tip: Use '--interactive-login' or '-i' to perform interactive OAuth login\n");

    // Setup DI container
    IHost host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((context, services) =>
        {
            // Add memory cache
            services.AddMemoryCache();

            // Add FreeAgent client services with configuration
            services.AddFreeAgentClientServices(configuration);
        })
        .Build();

    // Get the FreeAgent client from DI
    FreeAgentClient client = host.Services.GetRequiredService<FreeAgentClient>();

    // Initialize and authorize the client
    await client.InitializeAndAuthorizeAsync();

    // Use the client - example with Contacts
    IEnumerable<Contact> contacts = await client.Contacts.GetAllAsync();

    Console.WriteLine($"Retrieved {contacts.Count()} contacts from FreeAgent.");
}
AnsiConsole.Write(
    new FigletText("FreeAgent Demo")
        .Color(Color.Green));

try
{
    await AnsiConsole.Status()
        .StartAsync("Initializing and authorizing...", async ctx =>
        {
            await client.InitializeAndAuthorizeAsync();
        });

    AnsiConsole.MarkupLine("[green]Successfully connected to FreeAgent API[/]");
}
catch (Exception ex)
{
    AnsiConsole.WriteException(ex);
    return;
}

while (true)
{
    string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select an action:")
            .PageSize(10)
            .AddChoices(new[]
            {
                "Show Company Info",
                "List Contacts",
                "List Active Projects",
                "List Recent Invoices",
                "List Open Bills",
                "List Recent Expenses",
                "List Recent Bank Transactions",
                "Show Profit & Loss (YTD)",
                "List Users",
                "List EC MOSS Tax Rates",
                "Exit"
            }));

    if (choice == "Exit")
    {
        break;
    }

    try
    {
        switch (choice)
        {
            case "Show Company Info":
                await ShowCompanyInfo(client);
                break;
            case "List Contacts":
                await ListContacts(client);
                break;
            case "List Active Projects":
                await ListActiveProjects(client);
                break;
            case "List Recent Invoices":
                await ListRecentInvoices(client);
                break;
            case "List Open Bills":
                await ListOpenBills(client);
                break;
            case "List Recent Expenses":
                await ListRecentExpenses(client);
                break;
            case "List Recent Bank Transactions":
                await ListRecentBankTransactions(client);
                break;
            case "Show Profit & Loss (YTD)":
                await ShowProfitAndLoss(client);
                break;
            case "List Users":
                await ListUsers(client);
                break;
            case "List EC MOSS Tax Rates":
                await ListEcMossTaxRates(client);
                break;
        }
    }
    catch (Exception ex)
    {
        AnsiConsole.WriteException(ex);
    }

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("Press [blue]Enter[/] to continue...");
    Console.ReadLine();
    AnsiConsole.Clear();
}

static async Task ShowCompanyInfo(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching company info...", async ctx =>
        {
            Company company = await client.Company.GetAsync();
            
            Table table = new Table();
            table.AddColumn("Property");
            table.AddColumn("Value");

            table.AddRow("Name", company.Name ?? "N/A");
            table.AddRow("Subdomain", company.Subdomain ?? "N/A");
            table.AddRow("Type", company.Type?.ToString() ?? "N/A");
            table.AddRow("Currency", company.Currency ?? "N/A");
            table.AddRow("Mileage Units", company.MileageUnits ?? "N/A");

            AnsiConsole.Write(new Panel(table)
                .Header("Company Information")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListContacts(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching contacts...", async ctx =>
        {
            IEnumerable<Contact> contacts = await client.Contacts.GetAllAsync();

            Table table = new Table();
            table.AddColumn("Organization");
            table.AddColumn("First Name");
            table.AddColumn("Last Name");
            table.AddColumn("Email");
            table.AddColumn("Status");

            foreach (Contact contact in contacts.Take(10))
            {
                table.AddRow(
                    contact.OrganisationName ?? "-",
                    contact.FirstName ?? "-",
                    contact.LastName ?? "-",
                    contact.Email ?? "-",
                    contact.Status ?? "-"
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Contacts (First {Math.Min(10, contacts.Count())} of {contacts.Count()})")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListActiveProjects(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching active projects...", async ctx =>
        {
            IEnumerable<Project> projects = await client.Projects.GetAllActiveAsync();

            Table table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Contact");
            table.AddColumn("Status");
            table.AddColumn("Budget");

            foreach (Project project in projects.Take(10))
            {
                table.AddRow(
                    project.Name ?? "-",
                    project.ContactEntry?.OrganisationName ?? project.ContactEntry?.FirstName ?? "-",
                    project.Status ?? "-",
                    $"{project.Currency} {project.Budget:N0}"
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Active Projects (First {Math.Min(10, projects.Count())} of {projects.Count()})")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListOpenBills(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching open bills...", async ctx =>
        {
            IEnumerable<Bill> bills = await client.Bills.GetAllAsync(view: "open");

            Table table = new Table();
            table.AddColumn("Reference");
            table.AddColumn("Contact");
            table.AddColumn("Due Date");
            table.AddColumn(new TableColumn("Total").RightAligned());

            foreach (Bill bill in bills.Take(10))
            {
                // We might need to fetch contact name if not available, but for now let's use the URL or ID if nested is not used
                // Ideally we would fetch contacts or use a method that expands them, but GetAllAsync(view="open") doesn't support nested contacts directly in the signature I saw?
                // Actually Bills.GetAllAsync doesn't seem to have a 'nested' param for contacts, only bill items.
                // So we'll just show the contact URL or ID for now, or try to parse it.
                string contactId = bill.Contact?.ToString().Split('/').Last() ?? "-";

                table.AddRow(
                    bill.Reference ?? "-",
                    $"Contact #{contactId}", 
                    bill.DueOn?.ToString("yyyy-MM-dd") ?? "-",
                    $"{bill.TotalValue:N2}"
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Open Bills (First {Math.Min(10, bills.Count())})")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ShowProfitAndLoss(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching Profit & Loss...", async ctx =>
        {
            var pnl = await client.ProfitAndLossReports.GetCurrentYearToDateAsync();

            Table table = new Table();
            table.AddColumn("Category");
            table.AddColumn(new TableColumn("Amount").RightAligned());

            table.AddRow("Income", $"[green]{pnl.Income:N2}[/]");
            table.AddRow("Expenses", $"[red]{pnl.Expenses:N2}[/]");
            table.AddRow("Operating Profit", $"[bold]{pnl.OperatingProfit:N2}[/]");
            
            if (pnl.Less != null)
            {
                foreach (var deduction in pnl.Less)
                {
                    table.AddRow(deduction.Title ?? "Deduction", $"[red]{deduction.Total:N2}[/]");
                }
            }
                
            table.AddRow("Retained Profit", $"[bold green]{pnl.RetainedProfit:N2}[/]");

            AnsiConsole.Write(new Panel(table)
                .Header("Profit & Loss (Year to Date)")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListUsers(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching users...", async ctx =>
        {
            IEnumerable<User> users = await client.Users.GetAllUsersAsync();

            Table table = new Table();
            table.AddColumn("Name");
            table.AddColumn("Email");
            table.AddColumn("Role");

            foreach (User user in users)
            {
                table.AddRow(
                    $"{user.FirstName} {user.LastName}",
                    user.Email ?? "-",
                    user.Role?.ToString() ?? "-"
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Users ({users.Count()})")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListEcMossTaxRates(FreeAgentClient client)
{
    string[] countries = new[]
    {
        "Austria", "Belgium", "Bulgaria", "Croatia", "Cyprus", "Czech Republic",
        "Denmark", "Estonia", "Finland", "France", "Germany", "Greece", "Hungary",
        "Ireland", "Italy", "Latvia", "Lithuania", "Luxembourg", "Malta",
        "Netherlands", "Poland", "Portugal", "Romania", "Slovakia", "Slovenia",
        "Spain", "Sweden"
    };

    string country = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select country:")
            .PageSize(10)
            .AddChoices(countries));
    
    await AnsiConsole.Status()
        .StartAsync($"Fetching EC MOSS tax rates for {country}...", async ctx =>
        {
            try 
            {
                IEnumerable<EcMossSalesTaxRate> rates = await client.EcMossSalesTaxRates.GetAsync(country, DateOnly.FromDateTime(DateTime.Today));

                Table table = new Table();
                table.AddColumn("Band");
                table.AddColumn(new TableColumn("Percentage").RightAligned());

                foreach (EcMossSalesTaxRate rate in rates)
                {
                    table.AddRow(
                        rate.Band ?? "-",
                        $"{rate.Percentage}%"
                    );
                }

                AnsiConsole.Write(new Panel(table)
                    .Header($"EC MOSS Tax Rates for {country}")
                    .Border(BoxBorder.Rounded));
            }
            catch (HttpRequestException ex)
            {
                AnsiConsole.MarkupLine($"[red]Error fetching tax rates: {ex.Message}[/]");
            }
        });
}

static async Task ListRecentInvoices(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching invoices...", async ctx =>
        {
            // Get invoices from the last 6 months
            IEnumerable<Invoice> invoices = await client.Invoices.GetAllAsync(view: "last_6_months");

            Table table = new Table();
            table.AddColumn("Reference");
            table.AddColumn("Date");
            table.AddColumn("Due Date");
            table.AddColumn("Status");
            table.AddColumn(new TableColumn("Total").RightAligned());

            foreach (Invoice invoice in invoices.Take(10))
            {
                string statusColor = invoice.Status switch
                {
                    "Paid" => "green",
                    "Overdue" => "red",
                    "Open" => "yellow",
                    "Draft" => "grey",
                    _ => "white"
                };

                table.AddRow(
                    invoice.Reference ?? "-",
                    invoice.DatedOn?.ToString("yyyy-MM-dd") ?? "-",
                    invoice.DueOn?.ToString("yyyy-MM-dd") ?? "-",
                    $"[{statusColor}]{invoice.Status}[/]",
                    $"{invoice.Currency} {invoice.TotalValue:N2}"
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Recent Invoices (First {Math.Min(10, invoices.Count())})")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListRecentExpenses(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching expenses...", async ctx =>
        {
            DateOnly fromDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3));
            IEnumerable<Expense> expenses = await client.Expenses.GetAllAsync(view: "recent", fromDate: fromDate);

            Table table = new Table();
            table.AddColumn("Date");
            table.AddColumn("Category");
            table.AddColumn("Description");
            table.AddColumn(new TableColumn("Amount").RightAligned());

            foreach (Expense expense in expenses.Take(10))
            {
                // Extract category name from URL if possible, or just show the URL
                string category = expense.Category ?? "-";
                if (category.Contains("/categories/"))
                {
                    category = category.Split('/').Last();
                }

                table.AddRow(
                    expense.DatedOn?.ToString("yyyy-MM-dd") ?? "-",
                    category,
                    expense.Description ?? "-",
                    $"{expense.Currency} {expense.GrossValue:N2}"
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Recent Expenses (First {Math.Min(10, expenses.Count())})")
                .Border(BoxBorder.Rounded));
        });
}

static async Task ListRecentBankTransactions(FreeAgentClient client)
{
    await AnsiConsole.Status()
        .StartAsync("Fetching bank accounts...", async ctx =>
        {
            IEnumerable<BankAccount> accounts = await client.BankAccounts.GetAllAsync();
            BankAccount? primaryAccount = accounts.FirstOrDefault();

            if (primaryAccount?.Url == null)
            {
                AnsiConsole.MarkupLine("[yellow]No bank accounts found.[/]");
                return;
            }

            ctx.Status($"Fetching transactions for {primaryAccount.Name}...");
            
            DateOnly fromDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1));
            IEnumerable<BankTransaction> transactions = await client.BankTransactions.GetAllAsync(primaryAccount.Url, fromDate: fromDate);

            Table table = new Table();
            table.AddColumn("Date");
            table.AddColumn("Description");
            table.AddColumn(new TableColumn("Amount").RightAligned());
            table.AddColumn("Status");

            foreach (BankTransaction txn in transactions.Take(15))
            {
                string amountColor = txn.Amount < 0 ? "red" : "green";
                bool isExplained = txn.UnexplainedAmount == 0;
                string status = isExplained ? "[green]Explained[/]" : "[yellow]Unexplained[/]";

                table.AddRow(
                    txn.DatedOn?.ToString("yyyy-MM-dd") ?? "-",
                    txn.Description ?? "-",
                    $"[{amountColor}]{txn.Amount:N2}[/]",
                    status
                );
            }

            AnsiConsole.Write(new Panel(table)
                .Header($"Recent Transactions for {primaryAccount.Name}")
                .Border(BoxBorder.Rounded));
        });
}

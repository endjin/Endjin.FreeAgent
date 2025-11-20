// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Endjin.FreeAgent.Client;
using Endjin.FreeAgent.Domain;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

using Company = Endjin.FreeAgent.Domain.Company;

// Build configuration
IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>(optional: true)
    .Build();

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
                "List Recent Invoices",
                "List Recent Expenses",
                "List Recent Bank Transactions",
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
            case "List Recent Invoices":
                await ListRecentInvoices(client);
                break;
            case "List Recent Expenses":
                await ListRecentExpenses(client);
                break;
            case "List Recent Bank Transactions":
                await ListRecentBankTransactions(client);
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

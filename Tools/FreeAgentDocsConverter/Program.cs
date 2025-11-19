using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using HtmlAgilityPack;

using Microsoft.Extensions.DependencyInjection;

using ReverseMarkdown;

using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.IO;

using IEnvironment = Spectre.IO.IEnvironment;

var services = new ServiceCollection();
services.AddSingleton<IEnvironment, Spectre.IO.Environment>();
services.AddSingleton<IFileSystem, FileSystem>();

var registrar = new TypeRegistrar(services);
var app = new CommandApp<ConvertCommand>(registrar);

app.Configure(config =>
{
    config.SetApplicationName("FreeAgentDocsConverter");
    config.SetApplicationVersion("1.0.0");

    config.AddExample(["https://dev.freeagent.com/docs/attachments"]);
    config.AddExample(["https://dev.freeagent.com/docs/invoices", "-o", "./docs"]);
});

return app.Run(args);

public sealed class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _builder;

    public TypeRegistrar(IServiceCollection builder)
    {
        _builder = builder;
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_builder.BuildServiceProvider());
    }

    [UnconditionalSuppressMessage("Trimming", "IL2067", Justification = "TypeRegistrar is only used at startup with known types")]
    public void Register(Type service, Type implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _builder.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        _builder.AddSingleton(service, _ => factory());
    }
}

public sealed class TypeResolver : ITypeResolver
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        _provider = provider;
    }

    public object? Resolve(Type? type)
    {
        return type is null ? null : _provider.GetService(type);
    }
}

public class ConvertSettings : CommandSettings
{
    [CommandArgument(0, "<url>")]
    [Description("The FreeAgent API documentation URL to convert (e.g., https://dev.freeagent.com/docs/attachments)")]
    public string Url { get; set; } = string.Empty;

    [CommandOption("-o|--output <directory>")]
    [Description("Output directory for the generated markdown file (defaults to current directory)")]
    public string? OutputDirectory { get; set; }

    public override ValidationResult Validate()
    {
        if (!Uri.TryCreate(Url, UriKind.Absolute, out var uri))
        {
            return ValidationResult.Error("URL must be a valid absolute URL");
        }

        if (uri.Host != "dev.freeagent.com")
        {
            return ValidationResult.Error("URL must be from dev.freeagent.com");
        }

        if (!uri.AbsolutePath.StartsWith("/docs/"))
        {
            return ValidationResult.Error("URL must be a documentation page (e.g., /docs/attachments)");
        }

        return ValidationResult.Success();
    }
}

public partial class ConvertCommand : AsyncCommand<ConvertSettings>
{
    private static readonly Regex InternalLinkRegex = LinkRegExpr();

    private readonly IFileSystem _fileSystem;
    private readonly IEnvironment _environment;

    public ConvertCommand(IFileSystem fileSystem, IEnvironment environment)
    {
        _fileSystem = fileSystem;
        _environment = environment;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
    {
        var uri = new Uri(settings.Url);
        var resourceName = uri.AbsolutePath.Split('/').Last();
        var outputFileName = $"{resourceName}.md";

        var outputDir = settings.OutputDirectory is not null
            ? new DirectoryPath(settings.OutputDirectory)
            : _environment.WorkingDirectory;
        var outputPath = outputDir.CombineWithFilePath(outputFileName);

        var directory = _fileSystem.GetDirectory(outputDir);
        if (!directory.Exists)
        {
            directory.Create();
        }

        string? markdown = null;

        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Star)
            .SpinnerStyle(Style.Parse("green"))
            .StartAsync("Converting documentation...", async ctx =>
            {
                ctx.Status("Fetching HTML...");
                var html = await FetchHtmlAsync(settings.Url);

                ctx.Status("Extracting content...");
                var content = ExtractContent(html);

                ctx.Status("Converting to Markdown...");
                markdown = ConvertToMarkdown(content);

                ctx.Status("Post-processing...");
                markdown = PostProcessLinks(markdown);
                markdown = PostProcessMarkdown(markdown);
            });

        if (markdown is null)
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Failed to convert documentation");
            return 1;
        }

        var file = _fileSystem.GetFile(outputPath);
        await file.WriteAllTextAsync(markdown);

        AnsiConsole.MarkupLineInterpolated($"[green]Success![/] Converted [yellow]{resourceName}[/] to [blue]{outputPath.FullPath}[/]");

        return 0;
    }

    private static async Task<string> FetchHtmlAsync(string url)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "FreeAgentDocsConverter/1.0");

        return await httpClient.GetStringAsync(url);
    }

    private static string ExtractContent(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        // Work with body content
        var bodyNode = doc.DocumentNode.SelectSingleNode("//body");
        if (bodyNode is null)
        {
            return html;
        }

        // Remove standard navigation elements
        RemoveNodes(bodyNode, "//nav");
        RemoveNodes(bodyNode, "//footer");
        RemoveNodes(bodyNode, "//header");
        RemoveNodes(bodyNode, "//script");
        RemoveNodes(bodyNode, "//style");
        RemoveNodes(bodyNode, "//*[contains(@class, 'sidebar')]");
        RemoveNodes(bodyNode, "//*[contains(@class, 'navigation')]");

        // Remove the sidebar navigation list (ol containing /docs/ links)
        // This is specific to FreeAgent's docs structure
        var sidebarLists = bodyNode.SelectNodes("//ol[.//a[contains(@href, '/docs/')]]");
        if (sidebarLists is not null)
        {
            foreach (var list in sidebarLists.ToList())
            {
                list.Remove();
            }
        }

        // Remove header links (logo, menu items)
        var headerLinks = bodyNode.SelectNodes("//a[contains(@href, '/apps') or contains(@href, '/login') or contains(@href, '/logout') or contains(@href, '/signup')]");
        if (headerLinks is not null)
        {
            foreach (var link in headerLinks.ToList())
            {
                // Remove the parent list item if it exists
                var parent = link.ParentNode;
                if (parent?.Name == "li")
                {
                    parent.Remove();
                }
                else
                {
                    link.Remove();
                }
            }
        }

        // Remove any remaining top-level ol/ul that look like navigation menus
        var topLevelLists = bodyNode.SelectNodes("//body/ol | //body/ul");
        if (topLevelLists is not null)
        {
            foreach (var list in topLevelLists.ToList())
            {
                list.Remove();
            }
        }

        // Remove images that are logos
        RemoveNodes(bodyNode, "//img[contains(@src, 'logo')]");
        RemoveNodes(bodyNode, "//a[contains(@title, 'homepage')]");

        // Remove select elements (dropdown menus like "Choose an area")
        RemoveNodes(bodyNode, "//select");

        // Remove any remaining empty list elements
        RemoveNodes(bodyNode, "//ul[not(normalize-space())]");
        RemoveNodes(bodyNode, "//ol[not(normalize-space())]");

        // Pre-process colspan rows (section headers within tables)
        PreProcessColspanRows(bodyNode);

        // Pre-process table cells to handle nested HTML
        PreProcessTableCells(bodyNode);

        return bodyNode.InnerHtml;
    }

    private static void PreProcessTableCells(HtmlNode bodyNode)
    {
        var tableCells = bodyNode.SelectNodes("//td");
        if (tableCells is null)
        {
            return;
        }

        foreach (var cell in tableCells)
        {
            // First, convert anchor tags to markdown format
            var anchors = cell.SelectNodes(".//a[@href]");
            if (anchors is not null)
            {
                foreach (var anchor in anchors.ToList())
                {
                    var href = anchor.GetAttributeValue("href", "");
                    var text = HttpUtility.HtmlDecode(anchor.InnerText.Trim());
                    var markdownLink = $"[{text}]({href})";
                    var textNode = cell.OwnerDocument.CreateTextNode(markdownLink);
                    anchor.ParentNode.ReplaceChild(textNode, anchor);
                }
            }

            // Convert code tags to backtick format
            var codes = cell.SelectNodes(".//code");
            if (codes is not null)
            {
                foreach (var code in codes.ToList())
                {
                    var text = HttpUtility.HtmlDecode(code.InnerText);
                    var backtickCode = $"`{text}`";
                    var textNode = cell.OwnerDocument.CreateTextNode(backtickCode);
                    code.ParentNode.ReplaceChild(textNode, code);
                }
            }

            // Convert lists to comma-separated format
            var lists = cell.SelectNodes(".//ul | .//ol");
            if (lists is not null)
            {
                foreach (var list in lists.ToList())
                {
                    var items = list.SelectNodes(".//li");
                    if (items is not null)
                    {
                        var inlineItems = new StringBuilder();
                        foreach (var item in items)
                        {
                            var itemText = HttpUtility.HtmlDecode(item.InnerText.Trim());
                            if (inlineItems.Length > 0)
                            {
                                inlineItems.Append(", ");
                            }

                            inlineItems.Append(itemText);
                        }

                        var textNode = cell.OwnerDocument.CreateTextNode(inlineItems.ToString());
                        list.ParentNode.ReplaceChild(textNode, list);
                    }
                }
            }

            // Convert <br> tags to a single <br> marker
            var brTags = cell.SelectNodes(".//br");
            if (brTags is not null)
            {
                foreach (var br in brTags.ToList())
                {
                    var textNode = cell.OwnerDocument.CreateTextNode("<br>");
                    br.ParentNode.ReplaceChild(textNode, br);
                }
            }

            // Clean up the cell text - remove excessive whitespace
            var cellText = cell.InnerText;
            cellText = Regex.Replace(cellText, @"\s+", " ");
            cellText = Regex.Replace(cellText, @"(<br>\s*)+", "<br>");
            cellText = cellText.Trim();

            // Escape pipe characters to prevent extra columns
            cellText = cellText.Replace("|", "\\|");

            // Replace cell content with cleaned text
            cell.InnerHtml = cellText;
        }
    }

    private static void PreProcessColspanRows(HtmlNode bodyNode)
    {
        var tables = bodyNode.SelectNodes("//table");
        if (tables is null)
        {
            return;
        }

        foreach (var table in tables)
        {
            var rows = table.SelectNodes(".//tr");
            if (rows is null)
            {
                continue;
            }

            var rowsToRemove = new List<HtmlNode>();

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td | th");
                if (cells is null || cells.Count == 0)
                {
                    continue;
                }

                // Check for actual colspan attribute
                if (cells.Count == 1)
                {
                    var colspan = cells[0].GetAttributeValue("colspan", 1);
                    if (colspan > 1)
                    {
                        // Single cell with colspan - this is a header row, remove it
                        rowsToRemove.Add(row);
                        continue;
                    }
                }

                if (cells.Count <= 1)
                {
                    continue;
                }

                // Check if all cells have the same text content (colspan simulation)
                // Normalize whitespace for comparison
                var firstText = NormalizeText(cells[0].InnerText);
                if (string.IsNullOrEmpty(firstText))
                {
                    continue;
                }

                var allSame = cells.All(c => NormalizeText(c.InnerText) == firstText);
                if (allSame)
                {
                    // This is a colspan header row - mark for removal
                    // We could also convert it to a heading, but removing is cleaner
                    rowsToRemove.Add(row);
                }
            }

            foreach (var row in rowsToRemove)
            {
                row.Remove();
            }
        }
    }

    private static string NormalizeText(string text)
    {
        // Normalize whitespace: collapse all whitespace to single spaces and trim
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    private static void RemoveNodes(HtmlNode parent, string xpath)
    {
        var nodesToRemove = parent.SelectNodes(xpath);
        if (nodesToRemove is not null)
        {
            foreach (var node in nodesToRemove.ToList())
            {
                node.Remove();
            }
        }
    }

    private static string ConvertToMarkdown(string html)
    {
        var config = new Config
        {
            UnknownTags = Config.UnknownTagsOption.Bypass,
            GithubFlavored = true,
            RemoveComments = true,
            SmartHrefHandling = true,
            DefaultCodeBlockLanguage = "json",
            ListBulletChar = '-'
        };

        var converter = new Converter(config);
        return converter.Convert(html);
    }

    private static string PostProcessLinks(string markdown)
    {
        // Convert internal documentation links from /docs/resource to resource.md
        // Handles links with anchors like /docs/sales_tax#section -> sales_tax.md#section
        return InternalLinkRegex.Replace(markdown, "](${1}.md${2})");
    }

    private static string PostProcessMarkdown(string markdown)
    {
        // Remove multiple consecutive <br> tags
        markdown = Regex.Replace(markdown, @"(<br>\s*){2,}", "<br>");

        // Remove leading/trailing <br> in table cells (between | and content)
        markdown = Regex.Replace(markdown, @"\|\s*<br>\s*", "| ");
        markdown = Regex.Replace(markdown, @"\s*<br>\s*\|", " |");

        // Clean up escaped underscores inside backticks
        markdown = Regex.Replace(markdown, @"`([^`]*)\\_([^`]*)`", m =>
        {
            var content = m.Groups[1].Value + "_" + m.Groups[2].Value;
            // Handle multiple escaped underscores
            while (content.Contains("\\_"))
            {
                content = content.Replace("\\_", "_");
            }

            return $"`{content}`";
        });

        // Remove any leftover HTML list tags
        markdown = Regex.Replace(markdown, @"</?(?:ul|ol|li)[^>]*>", "", RegexOptions.IgnoreCase);

        // Convert HTTP request code blocks from json to http language
        markdown = Regex.Replace(
            markdown,
            @"```json\n((?:GET|POST|PUT|DELETE|PATCH)\s+https?://[^\n]+)\n```",
            "```http\n$1\n```");

        // Convert HTTP status code blocks from json to http language (handles multi-line with Location headers)
        markdown = Regex.Replace(
            markdown,
            @"```json\n(Status:\s*\d+[^\n]*(?:\nLocation:[^\n]*)?)\n```",
            "```http\n$1\n```");

        // Fix table column counts
        markdown = FixTableColumnCounts(markdown);

        // Format tables with aligned columns
        markdown = FormatTables(markdown);

        // Clean up excessive blank lines
        markdown = Regex.Replace(markdown, @"\n{3,}", "\n\n");

        // Fix escaped underscores in attribute names within tables
        // Keep them escaped as markdown requires it

        return markdown.Trim();
    }

    private static string FixTableColumnCounts(string markdown)
    {
        var lines = markdown.Split('\n');
        var result = new List<string>();
        var inTable = false;
        var expectedColumns = 0;
        var knownTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "String", "Integer", "Decimal", "Boolean", "URI", "Date", "Timestamp", "Array", "Hash"
        };

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            // Check if this is a table row
            if (line.TrimStart().StartsWith('|'))
            {
                var columns = CountTableColumns(line);

                if (!inTable)
                {
                    // First row of a table - this is the header
                    inTable = true;
                    expectedColumns = columns;
                    result.Add(line);
                }
                else if (line.Contains("| ---"))
                {
                    // Separator row - keep as is but ensure correct column count
                    if (columns < expectedColumns)
                    {
                        // Pad with additional separators
                        var padding = string.Concat(Enumerable.Repeat(" --- |", expectedColumns - columns));
                        line = line.TrimEnd() + padding;
                    }

                    result.Add(line);
                }
                else
                {
                    // Data row - check column count
                    if (columns < expectedColumns)
                    {
                        // Try to fix by splitting the last cell if it contains a known type
                        line = TryFixMissingColumn(line, expectedColumns, knownTypes);
                    }

                    result.Add(line);
                }
            }
            else
            {
                // Not a table row
                if (inTable)
                {
                    inTable = false;
                    expectedColumns = 0;
                }

                result.Add(line);
            }
        }

        return string.Join('\n', result);
    }

    private static int CountTableColumns(string line)
    {
        // Count pipes, subtract 1 for the trailing pipe
        var pipeCount = line.Count(c => c == '|');

        // Account for escaped pipes
        var escapedPipes = Regex.Matches(line, @"\\[|]").Count;

        return pipeCount - escapedPipes - 1;
    }

    private static string TryFixMissingColumn(string line, int expectedColumns, HashSet<string> knownTypes)
    {
        var currentColumns = CountTableColumns(line);
        if (currentColumns >= expectedColumns)
        {
            return line;
        }

        // Try to find a known type at the end of the last cell
        // Pattern: "some description TypeWord |" where TypeWord should be split out
        var match = Regex.Match(line, @"\s+(\w+)\s*\|$");
        if (match.Success)
        {
            var possibleType = match.Groups[1].Value;
            if (knownTypes.Contains(possibleType))
            {
                // Split out the type into its own column
                var beforeType = line.Substring(0, match.Index);
                line = $"{beforeType} | {possibleType} |";
            }
        }

        // If still missing columns, pad with empty cells
        currentColumns = CountTableColumns(line);
        while (currentColumns < expectedColumns)
        {
            line = line.TrimEnd().TrimEnd('|') + " |  |";
            currentColumns++;
        }

        return line;
    }

    private static string FormatTables(string markdown)
    {
        var lines = markdown.Split('\n').ToList();
        var result = new List<string>();
        var tableLines = new List<string>();
        var inTable = false;

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (line.TrimStart().StartsWith('|'))
            {
                inTable = true;
                tableLines.Add(line);
            }
            else
            {
                if (inTable && tableLines.Count > 0)
                {
                    // Format and add the collected table
                    result.AddRange(FormatSingleTable(tableLines));
                    tableLines.Clear();
                }

                inTable = false;
                result.Add(line);
            }
        }

        // Handle table at end of file
        if (tableLines.Count > 0)
        {
            result.AddRange(FormatSingleTable(tableLines));
        }

        return string.Join('\n', result);
    }

    private static List<string> FormatSingleTable(List<string> tableLines)
    {
        // Parse cells for each row
        var rows = tableLines.Select(ParseTableRow).ToList();

        // Calculate max width for each column
        var columnCount = rows.Max(r => r.Count);
        var columnWidths = new int[columnCount];

        for (int col = 0; col < columnCount; col++)
        {
            columnWidths[col] = rows
                .Where(r => col < r.Count)
                .Max(r => r[col].Length);

            // Ensure minimum width of 3 for separator dashes
            if (columnWidths[col] < 3)
            {
                columnWidths[col] = 3;
            }
        }

        // Rebuild rows with padding
        var result = new List<string>();
        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            var isSeparator = tableLines[i].Contains("---");

            var cells = new List<string>();
            for (int col = 0; col < columnCount; col++)
            {
                var content = col < row.Count ? row[col] : "";
                if (isSeparator)
                {
                    cells.Add(new string('-', columnWidths[col]));
                }
                else
                {
                    cells.Add(content.PadRight(columnWidths[col]));
                }
            }

            result.Add("| " + string.Join(" | ", cells) + " |");
        }

        return result;
    }

    private static List<string> ParseTableRow(string line)
    {
        // Split by | and trim each cell
        return line.Split('|')
            .Skip(1)  // Skip empty before first |
            .SkipLast(1)  // Skip empty after last |
            .Select(c => c.Trim())
            .ToList();
    }

    [GeneratedRegex(@"\]\(/docs/([^#)]+)(#[^)]+)?\)", RegexOptions.Compiled)]
    private static partial Regex LinkRegExpr();
}

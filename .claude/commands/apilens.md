---
description: Search and analyze .NET API documentation
allowed-tools: Bash, Read, Grep, LS, Glob
---

I'll help you search and analyze .NET API documentation using ApiLens.

$ARGUMENTS

Let me determine what you'd like to do with ApiLens:

## Available Commands

### 📚 **index** - Index XML documentation files
Index XML documentation from your project or specific files.
- `index ./docs` - Index all XML files in docs directory
- `index ./MyLib.xml` - Index a specific XML file
- `index ./docs --clean` - Clean index before adding new documents
- `index ./docs --index ./custom-index` - Use custom index location

### 🔍 **query** - Search API documentation
Search indexed documentation with powerful query options.

**Query Syntax:**
- **Name searches (default)**: Exact matches, case-insensitive
- **Content searches**: Full Lucene query syntax including:
  - Wildcards: `*` (multiple chars), `?` (single char)
    - Example: `string*` matches string, strings, stringify
  - Fuzzy: `~` for similar terms
    - Example: `roam~` matches foam, roams
  - Boolean: AND, OR, NOT (must be uppercase)
    - Example: `string AND utility`
  - Phrases: Use quotes for exact phrases
    - Example: `"extension methods"`

**Search Types:**
- `--type name` - Exact name match (default)
- `--type content` - Full-text search in documentation
- `--type namespace` - Exact namespace match
- `--type id` - Exact member ID (e.g., T:System.String)
- `--type assembly` - Exact assembly name match

**Examples:**
- `query String` - Exact name match
- `query ILogger` - Search for interfaces
- `query "Log*" --type content` - Wildcard search in content
- `query "extension methods" --type content` - Phrase search
- `query roam~ --type content` - Fuzzy search
- `query async AND Task --type content` - Boolean search
- `query ILogger --format json` - Get results in JSON format

### 📝 **examples** - Find code examples
Search for methods with code examples.
- `examples` - List all methods with examples
- `examples CalculateTotal` - Find examples containing pattern
- `examples async` - Find async code examples
Note: May encounter XML parsing issues with malformed documentation

### ⚠️ **exceptions** - Find exception throwers
Discover methods that throw specific exceptions.
- `exceptions ArgumentNullException` - Find ArgumentNullException throwers
- `exceptions System.IO.IOException` - Find IO exception throwers

### 📊 **complexity** - Analyze method complexity
Analyze cyclomatic complexity and parameter counts.
- `complexity --min-params 5` - Find methods with 5+ parameters
- `complexity --min-complexity 10` - Find complex methods
- `complexity --stats` - Show complexity statistics
Note: Requires methods to have parameter information in XML docs

### 📦 **nuget** - Index NuGet cache
Scan and index your NuGet package cache.
- `nuget` - Index all NuGet packages
- `nuget --list` - List packages without indexing
- `nuget --filter microsoft.*` - Index specific packages
- `nuget --latest-only` - Index only latest versions

### 📈 **stats** - Display index statistics
Show information about the current index.
- `stats` - Display index statistics
- `stats --format json` - Get stats in JSON format

### 🗂️ **list-types** - List types from assemblies [NEW]
List types from assemblies, packages, or namespaces with filtering support.
- `list-types --assembly System.Collections` - List types from assembly
- `list-types --package Newtonsoft.Json` - List types from NuGet package
- `list-types --namespace System.Collections.Generic` - List types from namespace
- `list-types --assembly "Microsoft.*"` - Wildcard filtering
- `list-types --package Serilog.AspNetCore --include-members` - Include all members
- `list-types --assembly System.* --group-by namespace` - Group results
- `list-types --package Serilog.AspNetCore --format json` - JSON output

**Options:**
- `--assembly / -a` - Filter by assembly name (supports wildcards)
- `--package / -p` - Filter by NuGet package ID (supports wildcards)
- `--namespace / -n` - Filter by namespace (supports wildcards)
- `--include-members` - Include all members, not just types
- `--group-by` - Group results by assembly, package, or namespace
- `--max / -m` - Maximum number of results
- `--format / -f` - Output format (table, json, markdown)

### 🔬 **analyze** - Analyze project or solution [NEW]
Analyze and index all packages from a project or solution file.
- `analyze ./MyProject.csproj` - Analyze a project file
- `analyze ./MySolution.sln` - Analyze a solution file
- `analyze ./MyProject.csproj --include-transitive` - Include transitive deps
- `analyze ./MySolution.sln --use-assets` - Use project.assets.json
- `analyze ./MyProject.csproj --clean` - Clean index before analyzing
- `analyze ./MySolution.sln --format json` - JSON output

**Options:**
- `--include-transitive` - Include transitive dependencies
- `--use-assets` - Parse project.assets.json for resolved versions
- `--format` - Output format: table, json, markdown
- `--clean` - Clean the index before analyzing
- `--index` - Custom index directory path (default: ./.index)

## Installation & Setup

To install ApiLens:
```bash
dotnet tool install -g ApiLens
```

To update to the latest version:
```bash
dotnet tool update -g ApiLens
```

Current version: 1.0.4 (Released with major new features)

Default index location: `./index` (current directory)
To use a custom index location, add `--index /path/to/index` to any command.

## Key Improvements in v1.0.4

### ✨ New Commands
- **list-types**: Browse and filter types from assemblies, packages, or namespaces
- **analyze**: Automatically index all packages from .csproj or .sln files

### 🔍 Enhanced Query Syntax
- Full Lucene query support with wildcards, fuzzy search, boolean operators
- Multiple search types: name, content, namespace, id, assembly
- Improved phrase searching with quotes

### 📊 Better Filtering
- Wildcard support in assembly, package, and namespace filters
- Include members option for comprehensive type exploration
- Group-by functionality for organized results

### 🚀 Project Integration
- Direct analysis of .csproj, .fsproj, .vbproj, and .sln files
- Automatic NuGet cache discovery and indexing
- Support for transitive dependencies

If no specific command is provided, I'll help you choose the best approach based on your needs.
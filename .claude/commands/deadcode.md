---
description: Analyze .NET project for dead/unused code
allowed-tools: Bash, Read, Grep, LS
---

I'll help you analyze your .NET project for dead/unused code using the deadcode tool.

$ARGUMENTS

Let me first check if this is a .NET project and what analysis mode you'd like:

Available modes:
- **quick** - Fast method inventory extraction
- **full** - Complete analysis with profiling (requires executable)
- **extract** - Just extract method inventory
- **help** - Show deadcode tool help

If no mode is specified, I'll automatically choose the best approach based on your project structure.

Installation & Updates:
- Install the tool using `dotnet tool install -g deadcode`
- Ensure the latest version using `dotnet tool update -g deadcode`
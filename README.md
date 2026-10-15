# MarcusMedina.Fluent.Data

[![NuGet](https://img.shields.io/nuget/v/MarcusMedina.Fluent.Data.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MarcusMedina.Fluent.Data.svg?style=for-the-badge&logo=nuget)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data/)
[![C#](https://img.shields.io/badge/C%23-14.0-239120?style=for-the-badge&logo=csharp&logoColor=white)](#)
[![.NET](https://img.shields.io/badge/.NET-10.0+-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)
[![Open Source](https://raw.githubusercontent.com/MarcusMedinaPro/MarcusMedina.Fluent.Data/main/assets/open-source.svg)](https://opensource.org)
[![Build](https://img.shields.io/github/actions/workflow/status/MarcusMedinaPro/MarcusMedina.Fluent.Data/release.yml?branch=main&label=Build&style=for-the-badge&logo=github)](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data/actions)
[![Signed](https://img.shields.io/badge/Signed-Sigstore-green?style=for-the-badge&logo=linux)](https://docs.sigstore.dev)
[![Wiki](https://img.shields.io/badge/docs-wiki-blue?style=for-the-badge&logo=github)](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data/wiki)

**Fluent data format extensions for CSV, JSON, and XML in .NET 10+**

Convert between strings, objects, and structured data with a clean fluent API — perfect for file import/export, API responses, and data transformation pipelines.

> This started as a C++ tool built for a tenant association — reading laundry-room access-tag logs to work out who kept stealing other residents' booked slots. Years later it turned into a fun assignment for my students: read the logs, identify whose tag was used, work out who went in and out and how long they stayed — extended so we could also track people entering and leaving the building itself. It's stuck with me ever since as a reminder never to get sloppy with CSV/JSON when importing real-world data.
>
> In this case, I wanted a fluent layer solid enough that I'd trust it with real log data again, not just toy examples.

---

## Features

- ✅ **CSV** — Parse and generate CSV lines, fields, and full documents (with escaping)
- ✅ **JSON** — Serialize/deserialize objects, validate JSON strings, escape content
- ✅ **XML** — Build XML elements and attributes fluently, escape XML content
- ✅ **Fluent API** — Chain operations naturally
- ✅ **Type-Safe** — Full generics support for `FromCsv<T>` with custom mapping
- ✅ **Zero dependencies** — Pure .NET, no external packages

---

## Installation

```bash
dotnet add package MarcusMedina.Fluent.Data
```

**Requirements:** .NET 10.0+, C# 14.0+

---

## Quick Start

### CSV

```csharp
using MarcusMedina.Fluent.Data;

// Parse CSV
var csv = "\"Hello, world\",42,true";
var fields = csv.SplitCsvLine();

// Build CSV
var line = new[] { "Hello, world", "42", "true" }.ToCsvLine();
// => "Hello, world",42,true

// Full document
var records = new[] {
    new[] { "Alice", "30" },
    new[] { "Bob", "25" }
};
var doc = records.ToCsvDocument();
```

### JSON

```csharp
// Objects to/from JSON
var json = new { Name = "Alice", Age = 30 }.ToJson();
var person = json.FromJson<Person>();

// Validate
bool valid = json.IsValidJson();
```

### XML

```csharp
// Build XML elements
var attrs = new[] {
    new KeyValuePair<string, object?>("id", 42),
    new KeyValuePair<string, object?>("name", "Alice")
};
string xml = attrs.ToXmlElement("person", "Hello");
// => <person id="42" name="Alice">Hello</person>
```

---

## API Overview

| Method | Description |
|--------|-------------|
| `ToCsvField()` | Escape a string for CSV (quotes, commas, newlines) |
| `FromCsvField()` | Unescape a CSV field value |
| `SplitCsvLine()` | Parse a CSV line into fields |
| `ToCsvLine()` | Join fields into a CSV line |
| `ToCsvDocument()` | Generate a multi-line CSV document |
| `FromCsvDocument()` | Parse a CSV document into rows |
| `FromCsv<T>()` | Parse CSV with custom mapping |
| `ToJson()` | Serialize object to JSON |
| `FromJson<T>()` | Deserialize JSON to object |
| `IsValidJson()` | Validate JSON string |
| `ToXmlContent()` | Escape string for XML content |
| `FromXmlContent()` | Unescape XML content |
| `ToXmlAttributes()` | Build XML attribute string |
| `ToXmlElement()` | Build XML element string |

---

## Testing

```bash
dotnet test --configuration Release
```

Tests: **97 passed** — covering CSV escaping edge cases, JSON validation, XML generation.

---

## License

MIT — see [LICENSE](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data/blob/main/LICENSE) for details.

---

## Built with Human + AI Collaboration

This library was written by **Marcus Medina** together with **Claude Code** (Anthropic) — not through "vibe coding" where you just describe and accept, but through genuine collaboration: planning together, reviewing each other's decisions, pushing back when something felt wrong, and iterating until the result felt right.

The goal was always to write code worth reading and code worth using — the kind a student can open, understand, and learn from, and the kind any programmer can drop into real, professional work without wanting to rewrite it from scratch. AI was a partner in that process, not a shortcut around it.

If you're curious about this way of working, the source code and git history are open. Every decision has a reason behind it.

## Made for Curious Minds

This library was built with students in mind — not as a black box to copy and paste, but as a real-world example of how clean, purposeful code is written and shared.

Whether you're discovering C# for the first time, need a reliable helper for your school project, or are simply trying to fall in love with writing code — you're exactly who this was made for.

The source is open. Read it, fork it, break it, improve it. That's the whole point.

And if this library saved you an afternoon, or made something click that didn't before — that's everything.

*Non-students are equally welcome. Good code doesn't care about your diploma.*

⭐ If this helped you, consider starring the project on GitHub — it helps other students find it too.

💬 Have an idea, a feature request, or just want to say hi? Open an issue on GitHub — I'd love to hear from you.

## Package Integrity

All releases are signed with [cosign](https://docs.sigstore.dev) (Sigstore keyless signing).

To verify a downloaded package, download both the `.nupkg` and its `.sigstore.json` bundle from the [GitHub Release](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data/releases), then run:

```bash
cosign verify-blob <package.nupkg> \
  --bundle <package.nupkg.sigstore.json> \
  --certificate-identity-regexp "https://github.com/MarcusMedinaPro/.*/release.yml" \
  --certificate-oidc-issuer https://token.actions.githubusercontent.com
```

Expected output: `Verified OK`

## Related Projects

- [MarcusMedina.Fluent.Data.Sql](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data.Sql) — Fluent SQL query builder
- [MarcusMedina.Fluent.Pattern](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Pattern) — String pattern matching and fuzzy search
- [MarcusMedina.Maths.Algebra](https://github.com/MarcusMedinaPro/MarcusMedina.Maths.Algebra) — Algebraic expressions and symbolic math
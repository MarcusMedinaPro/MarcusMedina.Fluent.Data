# MarcusMedina.Fluent.Data

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download)
[![NuGet](https://img.shields.io/badge/NuGet-1.0.0-blue.svg)](https://www.nuget.org/packages/MarcusMedina.Fluent.Data/)
[![Tests](https://img.shields.io/badge/tests-97%20passed-brightgreen)]()

**Fluent data format extensions for CSV, JSON, and XML in .NET 10+**

Convert between strings, objects, and structured data with a clean fluent API — perfect for file import/export, API responses, and data transformation pipelines.

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

MIT — see [LICENSE](LICENSE) for details.

---

## Related Projects

- [MarcusMedina.Fluent.Data.Sql](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Data.Sql) — Fluent SQL query builder
- [MarcusMedina.Fluent.Pattern](https://github.com/MarcusMedinaPro/MarcusMedina.Fluent.Pattern) — String pattern matching and fuzzy search
- [MarcusMedina.Maths.Algebra](https://github.com/MarcusMedinaPro/MarcusMedina.Maths.Algebra) — Algebraic expressions and symbolic math

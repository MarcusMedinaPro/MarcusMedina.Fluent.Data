using System.Text;
using System.Text.Json;
using System.Xml;

namespace MarcusMedina.Fluent.Data.Extensions;

/// <summary>
/// CSV, JSON och XML konverteringsmetoder för strängar.
/// Inspirationskälla för studerande att bygga egna hjälpklasser.
/// </summary>
public static class StringDataFormatExtensions
{
    /// <summary>
    /// Escapar ett fält för CSV — omsluter med citattecken om det behövs.
    /// </summary>
    /// <param name="value">Strängen att escaoa.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// "hello, world".ToCsvField() → "\"hello, world\""
    /// </example>
    public static string ToCsvField(this string value, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Contains(delimiter) || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    /// <summary>
    /// Återställer ett CSV-fält från dess escapade form.
    /// </summary>
    /// <param name="value">CSV-fältet att återställa.</param>
    /// <example>
    /// "\"hello, world\"".FromCsvField() → "hello, world"
    /// </example>
    public static string FromCsvField(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (value.Length >= 2 && value[0] == '"' && value[^1] == '"')
            return value[1..^1].Replace("\"\"", "\"");
        return value;
    }

    /// <summary>
    /// Delar en CSV-rad i dess individuella fält, med stöd för citattecken.
    /// </summary>
    /// <param name="value">CSV-raden att dela.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// "a,b,\"c,d\"".SplitCsvLine() → ["a", "b", "c,d"]
    /// </example>
    public static string[] SplitCsvLine(this string value, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(value);
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
                current.Append(c);
            }
            else if (c == delimiter && !inQuotes)
            {
                result.Add(current.ToString().FromCsvField());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
        result.Add(current.ToString().FromCsvField());
        return [.. result];
    }

    /// <summary>
    /// Skapar en CSV-rad från en samling strängar.
    /// </summary>
    /// <param name="values">Värdena att skriva.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// new[] { "a", "hej världen", "c" }.ToCsvLine() → "a,\"hej världen\",c"
    /// </example>
    public static string ToCsvLine(this IEnumerable<string> values, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(values);
        return string.Join(delimiter, values.Select(v => v.ToCsvField(delimiter)));
    }

    /// <summary>
    /// Escapar en sträng för inbäddning i JSON (lägger till omvända snedstreck).
    /// </summary>
    /// <param name="value">Strängen att escaoa.</param>
    /// <example>
    /// "say \"hi\"".ToJsonString() → "say \\\"hi\\\""
    /// </example>
    public static string ToJsonString(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    /// <summary>
    /// Återställer en JSON-escapad sträng till sitt ursprungliga innehåll.
    /// </summary>
    /// <param name="value">Den JSON-escapade strängen.</param>
    /// <example>
    /// "say \\\"hi\\\"".FromJsonString() → "say \"hi\""
    /// </example>
    public static string FromJsonString(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value
            .Replace("\\\"", "\"")
            .Replace("\\n", "\n")
            .Replace("\\r", "\r")
            .Replace("\\t", "\t")
            .Replace("\\\\", "\\");
    }

    /// <summary>
    /// Kontrollerar om en sträng är giltig JSON.
    /// </summary>
    /// <param name="value">Strängen att validera.</param>
    /// <example>
    /// "{\"name\":\"John\"}".IsValidJson() → true
    /// </example>
    public static bool IsValidJson(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (string.IsNullOrEmpty(value)) return false;
        try
        {
            using var doc = JsonDocument.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Escapar en sträng för säker inbäddning i XML.
    /// </summary>
    /// <param name="value">Strängen att escaoa.</param>
    /// <example>
    /// "<tag>".ToXmlContent() → "&amp;lt;tag&amp;gt;"
    /// </example>
    public static string ToXmlContent(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return System.Security.SecurityElement.Escape(value);
    }

    /// <summary>
    /// Återställer en XML-escapad sträng.
    /// </summary>
    /// <param name="value">Den XML-escapade strängen.</param>
    /// <example>
    /// "&amp;lt;tag&amp;gt;".FromXmlContent() → "<tag>"
    /// </example>
    public static string FromXmlContent(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        var doc = new XmlDocument();
        var temp = doc.CreateElement("root");
        temp.InnerXml = value;
        return temp.InnerText;
    }

    /// <summary>
    /// Tolkar en hel CSV-text till en samling strängrader.
    /// </summary>
    /// <param name="csv">CSV-dokumentet som sträng.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// var rows = csv.FromCsvDocument();
    /// foreach (var row in rows) { /* row[0], row[1], ... */ }
    /// </example>
    public static IEnumerable<string[]> FromCsvDocument(this string csv, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(csv);
        if (csv.Length == 0) return [];
        return csv.TrimEnd('\r', '\n')
            .Split(Environment.NewLine)
            .Select(line => line.SplitCsvLine(delimiter));
    }

    /// <summary>
    /// Tolkar CSV och mappar varje rad via en delegat.
    /// </summary>
    /// <param name="csv">CSV-dokumentet som sträng.</param>
    /// <param name="mapRow">Funktion som mappar en rad (string[]) till T.</param>
    /// <param name="hasHeaders">Om true hoppas första raden över.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// csv.FromCsv(row => new Person { Name = row[0], Age = int.Parse(row[1]) }, hasHeaders: true)
    /// </example>
    public static IEnumerable<T> FromCsv<T>(this string csv, Func<string[], T> mapRow,
        bool hasHeaders = false, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(csv);
        ArgumentNullException.ThrowIfNull(mapRow);
        if (csv.Length == 0) return [];

        var lines = csv.TrimEnd('\r', '\n').Split(Environment.NewLine);
        var rows = lines.Select(line => line.SplitCsvLine(delimiter));
        if (hasHeaders) rows = rows.Skip(1);
        return rows.Select(mapRow);
    }
}

namespace MarcusMedina.Fluent.Data.Extensions;

/// <summary>
/// CSV-dokumenthantering för samlingar och objekt.
/// Inspirationskälla för studerande att bygga egna hjälpklasser.
/// </summary>
public static class CollectionDataFormatExtensions
{
    /// <summary>
    /// Skapar en CSV-rad från en samling objekt (anropar ToString() på varje).
    /// </summary>
    /// <param name="values">Värdena att skriva.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// new object[] { 1, "hej", true }.ToCsvLine() → "1,hej,True"
    /// </example>
    public static string ToCsvLine(this IEnumerable<object?> values, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(values);
        return string.Join(delimiter, values.Select(v => (v?.ToString() ?? "").ToCsvField(delimiter)));
    }

    /// <summary>
    /// Skapar ett CSV-dokument från rader av objekt.
    /// </summary>
    /// <param name="rows">Rader med värden.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// var csv = new[] { new[] { "a", "1" }, new[] { "b", "2" } }.ToCsvDocument();
    /// </example>
    public static string ToCsvDocument(this IEnumerable<IEnumerable<object?>> rows, char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(rows);
        var lines = rows.Select(r => r.ToCsvLine(delimiter));
        return string.Join(Environment.NewLine, lines);
    }

    /// <summary>
    /// Skapar ett CSV-dokument från en samling objekt med en mappningsfunktion.
    /// </summary>
    /// <param name="items">Objekten att serialisera.</param>
    /// <param name="mapRow">Funktion som mappar varje objekt till en rad värden.</param>
    /// <param name="headers">Optional rubrikrad.</param>
    /// <param name="delimiter">Avgränsare (standard komma).</param>
    /// <example>
    /// users.ToCsvDocument(u => new object[] { u.Name, u.Age }, headers: ["Name", "Age"])
    /// </example>
    public static string ToCsvDocument<T>(this IEnumerable<T> items,
        Func<T, IEnumerable<object?>> mapRow,
        IEnumerable<string>? headers = null,
        char delimiter = ',')
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(mapRow);

        var lines = new List<string>();
        if (headers != null)
            lines.Add(headers.ToCsvLine(delimiter));
        lines.AddRange(items.Select(item => mapRow(item).ToCsvLine(delimiter)));
        return string.Join(Environment.NewLine, lines);
    }
}

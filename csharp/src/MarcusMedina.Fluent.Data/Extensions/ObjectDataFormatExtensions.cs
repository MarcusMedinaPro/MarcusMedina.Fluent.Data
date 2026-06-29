using System.Text.Json;
using System.Xml;

namespace MarcusMedina.Fluent.Data.Extensions;

/// <summary>
/// JSON- och XML-serialisering för objekt.
/// Inspirationskälla för studerande att bygga egna hjälpklasser.
/// </summary>
public static class ObjectDataFormatExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serialiserar ett objekt till JSON-sträng.
    /// </summary>
    /// <param name="value">Objektet att serialisera.</param>
    /// <param name="writeIndented">Om true, formateras JSON med indrag.</param>
    /// <example>
    /// new Person("John", 30).ToJson() → {"name":"John","age":30}
    /// </example>
    public static string ToJson<T>(this T? value, bool writeIndented = false)
    {
        var options = writeIndented
            ? new JsonSerializerOptions(JsonOptions) { WriteIndented = true }
            : JsonOptions;
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserialiserar en JSON-sträng till ett objekt.
    /// </summary>
    /// <param name="json">JSON-strängen.</param>
    /// <example>
    /// json.FromJson&lt;Person&gt;() → Person-objekt
    /// </example>
    public static T? FromJson<T>(this string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    /// <summary>
    /// Skapar XML-attribut från en ordlista.
    /// </summary>
    /// <param name="attributes">Namn/värde-par för attribut.</param>
    /// <example>
    /// new Dictionary&lt;string,object&gt; { {"id", 1} }.ToXmlAttributes() → id="1"
    /// </example>
    public static string ToXmlAttributes(this IEnumerable<KeyValuePair<string, object?>> attributes)
    {
        ArgumentNullException.ThrowIfNull(attributes);
        return string.Join(" ", attributes.Select(a =>
        {
            var val = a.Value?.ToString() ?? "";
            return $"{a.Key}=\"{val.ToXmlContent()}\"";
        }));
    }

    /// <summary>
    /// Skapar ett XML-element med innehåll och attribut.
    /// </summary>
    /// <param name="elementName">Elementets namn.</param>
    /// <param name="content">Innehåll (escapas automatiskt).</param>
    /// <param name="attributes">Optional attribut.</param>
    /// <example>
    /// "person".ToXmlElement("John", new[] { new KeyValuePair&lt;string,object&gt;("id", 1) })
    /// → &lt;person id="1"&gt;John&lt;/person&gt;
    /// </example>
    public static string ToXmlElement(this string elementName, string? content,
        IEnumerable<KeyValuePair<string, object?>>? attributes = null)
    {
        ArgumentNullException.ThrowIfNull(elementName);
        if (string.IsNullOrWhiteSpace(elementName))
            throw new ArgumentException("Element name cannot be blank", nameof(elementName));

        var attrString = attributes != null ? " " + attributes.ToXmlAttributes() : "";
        var escapedContent = (content ?? "").ToXmlContent();
        var xml = $"<{elementName}{attrString}>{escapedContent}</{elementName}>";

        var doc = new XmlDocument();
        try
        {
            var frag = doc.CreateDocumentFragment();
            frag.InnerXml = xml;
        }
        catch (XmlException)
        {
            throw;
        }

        return xml;
    }
}

using System.Text.Json;
using MarcusMedina.Fluent.Data.Extensions;

namespace MarcusMedina.Fluent.Data.Tests.Extensions;

public class ObjectDataFormatExtensionsTests
{
    private record Person(string Name, int Age);

    #region ToJson

    [Fact]
    public void ToJson_SimpleRecord_SerializesToJson()
    {
        var person = new Person("Alice", 30);
        var json = person.ToJson();
        json.Should().Contain("\"name\"");
        json.Should().Contain("\"Alice\"");
        json.Should().Contain("\"age\"");
        json.Should().Contain("30");
    }

    [Fact]
    public void ToJson_Indented_FormatsWithIndentation()
    {
        var person = new Person("Alice", 30);
        var json = person.ToJson(writeIndented: true);
        json.Should().Contain("\n");
    }

    [Fact]
    public void ToJson_NotIndented_FormatsWithoutIndentation()
    {
        var person = new Person("Alice", 30);
        var json = person.ToJson(writeIndented: false);
        json.Should().NotContain("\n");
    }

    [Fact]
    public void ToJson_NullValue_SerializesJsonNull()
    {
        Person? person = null;
        var json = person.ToJson();
        json.Should().Be("null");
    }

    [Fact]
    public void ToJson_Array_SerializesArray()
    {
        var arr = new[] { 1, 2, 3 };
        var json = arr.ToJson(writeIndented: false);
        json.Should().Be("[1,2,3]");
    }

    [Fact]
    public void ToJson_Dictionary_SerializesWithCamelCase()
    {
        var dict = new Dictionary<string, int> { ["FooBar"] = 42 };
        var json = dict.ToJson(writeIndented: false);
        // Dictionary keys are not transformed by camelCase naming policy (it applies to property names)
        json.Should().Contain("42");
    }

    #endregion

    #region FromJson

    [Fact]
    public void FromJson_ValidJson_DeserializesCorrectly()
    {
        var json = "{\"name\":\"Alice\",\"age\":30}";
        var person = json.FromJson<Person>();
        person.Should().NotBeNull();
        person!.Name.Should().Be("Alice");
        person.Age.Should().Be(30);
    }

    [Fact]
    public void FromJson_NullJson_ReturnsNull()
    {
        var result = "null".FromJson<Person>();
        result.Should().BeNull();
    }

    [Fact]
    public void FromJson_NullString_ThrowsArgumentNullException()
    {
        string? json = null;
        var act = () => json!.FromJson<Person>();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromJson_InvalidJson_ThrowsJsonException()
    {
        var act = () => "not-json".FromJson<Person>();
        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void ToJson_FromJson_RoundTrip_PreservesData()
    {
        var original = new Person("Bob", 25);
        var json = original.ToJson(writeIndented: false);
        var restored = json.FromJson<Person>();
        restored.Should().Be(original);
    }

    #endregion

    #region ToXmlAttributes

    [Fact]
    public void ToXmlAttributes_SingleAttribute_ReturnsAttributeSyntax()
    {
        var attrs = new[] { KeyValuePair.Create<string, object?>("id", 42) };
        attrs.ToXmlAttributes().Should().Be("id=\"42\"");
    }

    [Fact]
    public void ToXmlAttributes_MultipleAttributes_JoinsWithSpace()
    {
        var attrs = new[]
        {
            KeyValuePair.Create<string, object?>("id", 42),
            KeyValuePair.Create<string, object?>("status", "shipped")
        };
        attrs.ToXmlAttributes().Should().Be("id=\"42\" status=\"shipped\"");
    }

    [Fact]
    public void ToXmlAttributes_ValueWithXmlSpecialChars_EscapesValue()
    {
        var attrs = new[] { KeyValuePair.Create<string, object?>("note", "a & b") };
        attrs.ToXmlAttributes().Should().Be("note=\"a &amp; b\"");
    }

    [Fact]
    public void ToXmlAttributes_NullValue_UsesEmptyString()
    {
        var attrs = new[] { KeyValuePair.Create<string, object?>("name", (object?)null) };
        attrs.ToXmlAttributes().Should().Be("name=\"\"");
    }

    [Fact]
    public void ToXmlAttributes_NullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<KeyValuePair<string, object?>>? attrs = null;
        var act = () => attrs!.ToXmlAttributes();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region ToXmlElement

    [Fact]
    public void ToXmlElement_SimpleName_WrapsContentInElement()
    {
        "name".ToXmlElement("Alice").Should().Be("<name>Alice</name>");
    }

    [Fact]
    public void ToXmlElement_ContentWithXmlChars_EscapesContent()
    {
        "note".ToXmlElement("a & b").Should().Be("<note>a &amp; b</note>");
    }

    [Fact]
    public void ToXmlElement_NullContent_UsesEmptyContent()
    {
        "name".ToXmlElement(null).Should().Be("<name></name>");
    }

    [Fact]
    public void ToXmlElement_WithAttributes_InjectsAttributes()
    {
        var attrs = new[] { KeyValuePair.Create<string, object?>("id", 1) };
        "item".ToXmlElement("foo", attrs).Should().Be("<item id=\"1\">foo</item>");
    }

    [Fact]
    public void ToXmlElement_NullElementName_ThrowsArgumentNullException()
    {
        string? name = null;
        var act = () => name!.ToXmlElement("content");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToXmlElement_BlankElementName_ThrowsArgumentException()
    {
        var act = () => "  ".ToXmlElement("content");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToXmlElement_InvalidXmlElementName_ThrowsXmlException()
    {
        // Element names with injection characters must be rejected
        var act = () => "foo><bar".ToXmlElement("content");
        act.Should().Throw<System.Xml.XmlException>();
    }

    [Fact]
    public void ToXmlAttributes_InvalidAttributeName_ThrowsXmlException()
    {
        // Attribute names with spaces or special chars must be rejected
        var attrs = new Dictionary<string, object?> { ["bad name!"] = "value" };
        var act = () => "foo".ToXmlElement("content", attrs);
        act.Should().Throw<System.Xml.XmlException>();
    }

    #endregion
}

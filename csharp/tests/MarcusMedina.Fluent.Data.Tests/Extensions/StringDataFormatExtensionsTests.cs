using MarcusMedina.Fluent.Data.Extensions;

namespace MarcusMedina.Fluent.Data.Tests.Extensions;

public class StringDataFormatExtensionsTests
{
    #region ToCsvField

    [Fact]
    public void ToCsvField_PlainString_ReturnsAsIs()
    {
        "hello".ToCsvField().Should().Be("hello");
    }

    [Fact]
    public void ToCsvField_ContainsComma_WrapsInQuotes()
    {
        "hello, world".ToCsvField().Should().Be("\"hello, world\"");
    }

    [Fact]
    public void ToCsvField_ContainsQuote_EscapesAndWraps()
    {
        "say \"hi\"".ToCsvField().Should().Be("\"say \"\"hi\"\"\"");
    }

    [Fact]
    public void ToCsvField_ContainsNewline_WrapsInQuotes()
    {
        "line1\nline2".ToCsvField().Should().Be("\"line1\nline2\"");
    }

    [Fact]
    public void ToCsvField_CustomDelimiter_EscapesDelimiter()
    {
        "value;data".ToCsvField(';').Should().Be("\"value;data\"");
    }

    [Fact]
    public void ToCsvField_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.ToCsvField();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToCsvField_EmptyString_ReturnsEmpty()
    {
        "".ToCsvField().Should().Be("");
    }

    #endregion

    #region FromCsvField

    [Fact]
    public void FromCsvField_PlainString_ReturnsAsIs()
    {
        "hello".FromCsvField().Should().Be("hello");
    }

    [Fact]
    public void FromCsvField_QuotedString_UnwrapsQuotes()
    {
        "\"hello, world\"".FromCsvField().Should().Be("hello, world");
    }

    [Fact]
    public void FromCsvField_EscapedQuotes_UnescapesQuotes()
    {
        "\"say \"\"hi\"\"\"".FromCsvField().Should().Be("say \"hi\"");
    }

    [Fact]
    public void FromCsvField_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.FromCsvField();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToCsvField_RoundTrip_PreservesOriginalValue()
    {
        var original = "hello, \"world\"";
        original.ToCsvField().FromCsvField().Should().Be(original);
    }

    #endregion

    #region SplitCsvLine

    [Fact]
    public void SplitCsvLine_SimpleLine_SplitsOnComma()
    {
        "a,b,c".SplitCsvLine().Should().Equal("a", "b", "c");
    }

    [Fact]
    public void SplitCsvLine_CustomDelimiter_SplitsCorrectly()
    {
        "a;b;c".SplitCsvLine(';').Should().Equal("a", "b", "c");
    }

    [Fact]
    public void SplitCsvLine_QuotedFieldWithComma_TreatedAsOneField()
    {
        "\"hello, world\",test".SplitCsvLine().Should().Equal("hello, world", "test");
    }

    [Fact]
    public void SplitCsvLine_QuotedFieldWithEscapedQuotes_UnescapesCorrectly()
    {
        "\"say \"\"hi\"\"\",data".SplitCsvLine().Should().Equal("say \"hi\"", "data");
    }

    [Fact]
    public void SplitCsvLine_EmptyFields_ReturnsEmptyStrings()
    {
        "a,,c".SplitCsvLine().Should().Equal("a", "", "c");
    }

    [Fact]
    public void SplitCsvLine_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.SplitCsvLine();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region ToCsvLine (string enumerable)

    [Fact]
    public void ToCsvLine_SimpleValues_JoinsWithComma()
    {
        new[] { "a", "b", "c" }.ToCsvLine().Should().Be("a,b,c");
    }

    [Fact]
    public void ToCsvLine_ValuesWithComma_EscapesAndJoins()
    {
        new[] { "hello, world", "test" }.ToCsvLine().Should().Be("\"hello, world\",test");
    }

    [Fact]
    public void ToCsvLine_CustomDelimiter_UsesDelimiter()
    {
        new[] { "a", "b", "c" }.ToCsvLine(';').Should().Be("a;b;c");
    }

    [Fact]
    public void ToCsvLine_NullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<string>? values = null;
        var act = () => values!.ToCsvLine();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region ToJsonString / FromJsonString

    [Fact]
    public void ToJsonString_PlainString_ReturnsAsIs()
    {
        "hello".ToJsonString().Should().Be("hello");
    }

    [Fact]
    public void ToJsonString_ContainsQuote_EscapesQuote()
    {
        "say \"hi\"".ToJsonString().Should().Be("say \\\"hi\\\"");
    }

    [Fact]
    public void ToJsonString_ContainsBackslash_EscapesBackslash()
    {
        "path\\file".ToJsonString().Should().Be("path\\\\file");
    }

    [Fact]
    public void ToJsonString_ContainsNewline_EscapesNewline()
    {
        "line1\nline2".ToJsonString().Should().Be("line1\\nline2");
    }

    [Fact]
    public void ToJsonString_ContainsTab_EscapesTab()
    {
        "col1\tcol2".ToJsonString().Should().Be("col1\\tcol2");
    }

    [Fact]
    public void ToJsonString_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.ToJsonString();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromJsonString_EscapedQuote_UnescapesQuote()
    {
        "say \\\"hi\\\"".FromJsonString().Should().Be("say \"hi\"");
    }

    [Fact]
    public void FromJsonString_EscapedNewline_UnescapesNewline()
    {
        "line1\\nline2".FromJsonString().Should().Be("line1\nline2");
    }

    [Fact]
    public void FromJsonString_PlainString_ReturnsAsIs()
    {
        "hello".FromJsonString().Should().Be("hello");
    }

    [Fact]
    public void ToJsonString_RoundTrip_PreservesOriginalValue()
    {
        var original = "say \"hi\" and \\go\\ there\nnewline";
        original.ToJsonString().FromJsonString().Should().Be(original);
    }

    [Fact]
    public void FromJsonString_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.FromJsonString();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region IsValidJson

    [Fact]
    public void IsValidJson_ValidObject_ReturnsTrue()
    {
        "{\"name\":\"John\"}".IsValidJson().Should().BeTrue();
    }

    [Fact]
    public void IsValidJson_ValidArray_ReturnsTrue()
    {
        "[1,2,3]".IsValidJson().Should().BeTrue();
    }

    [Fact]
    public void IsValidJson_InvalidJson_ReturnsFalse()
    {
        "not json".IsValidJson().Should().BeFalse();
    }

    [Fact]
    public void IsValidJson_EmptyString_ReturnsFalse()
    {
        "".IsValidJson().Should().BeFalse();
    }

    [Fact]
    public void IsValidJson_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.IsValidJson();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region ToXmlContent / FromXmlContent

    [Fact]
    public void ToXmlContent_PlainString_ReturnsAsIs()
    {
        "hello".ToXmlContent().Should().Be("hello");
    }

    [Fact]
    public void ToXmlContent_ContainsAmpersand_EscapesAmpersand()
    {
        "Tom & Jerry".ToXmlContent().Should().Be("Tom &amp; Jerry");
    }

    [Fact]
    public void ToXmlContent_ContainsAngleBrackets_EscapesBrackets()
    {
        "<tag>".ToXmlContent().Should().Be("&lt;tag&gt;");
    }

    [Fact]
    public void ToXmlContent_ContainsQuote_EscapesQuote()
    {
        "say \"hi\"".ToXmlContent().Should().Be("say &quot;hi&quot;");
    }

    [Fact]
    public void ToXmlContent_ContainsApostrophe_EscapesApostrophe()
    {
        "it's".ToXmlContent().Should().Be("it&apos;s");
    }

    [Fact]
    public void ToXmlContent_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.ToXmlContent();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromXmlContent_EscapedAmpersand_Unescapes()
    {
        "Tom &amp; Jerry".FromXmlContent().Should().Be("Tom & Jerry");
    }

    [Fact]
    public void FromXmlContent_EscapedBrackets_Unescapes()
    {
        "&lt;tag&gt;".FromXmlContent().Should().Be("<tag>");
    }

    [Fact]
    public void ToXmlContent_RoundTrip_PreservesOriginalValue()
    {
        var original = "<Tom & Jerry> said \"it's\" fine";
        original.ToXmlContent().FromXmlContent().Should().Be(original);
    }

    [Fact]
    public void FromXmlContent_NullValue_ThrowsArgumentNullException()
    {
        string? value = null;
        var act = () => value!.FromXmlContent();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion
}

using MarcusMedina.Fluent.Data.Extensions;

namespace MarcusMedina.Fluent.Data.Tests.Extensions;

public class CollectionDataFormatExtensionsTests
{
    #region ToCsvLine (object collection)

    [Fact]
    public void ToCsvLine_SimpleValues_JoinsWithComma()
    {
        new object?[] { "a", "b", "c" }.ToCsvLine().Should().Be("a,b,c");
    }

    [Fact]
    public void ToCsvLine_IntegerValues_SerializesCorrectly()
    {
        new object?[] { 1, 2, 3 }.ToCsvLine().Should().Be("1,2,3");
    }

    [Fact]
    public void ToCsvLine_DecimalValues_UsesInvariantCulture()
    {
        new object?[] { 1.5m, 2.75m }.ToCsvLine().Should().Be("1.5,2.75");
    }

    [Fact]
    public void ToCsvLine_NullValue_OutputsEmptyField()
    {
        new object?[] { "a", null, "c" }.ToCsvLine().Should().Be("a,,c");
    }

    [Fact]
    public void ToCsvLine_ValueWithComma_WrapsInQuotes()
    {
        new object?[] { "hello, world" }.ToCsvLine().Should().Be("\"hello, world\"");
    }

    [Fact]
    public void ToCsvLine_CustomDelimiter_UsesDelimiter()
    {
        new object?[] { "a", "b", "c" }.ToCsvLine(';').Should().Be("a;b;c");
    }

    [Fact]
    public void ToCsvLine_NullCollection_ThrowsArgumentNullException()
    {
        IEnumerable<object?>? values = null;
        var act = () => values!.ToCsvLine();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region ToCsvDocument (rows)

    [Fact]
    public void ToCsvDocument_TwoRows_JoinsWithNewline()
    {
        var rows = new[]
        {
            new object?[] { "a", "b" },
            new object?[] { "c", "d" }
        };
        var result = rows.ToCsvDocument();
        result.Should().Be($"a,b{Environment.NewLine}c,d");
    }

    [Fact]
    public void ToCsvDocument_EmptyRows_ReturnsEmptyString()
    {
        var rows = Array.Empty<object?[]>();
        rows.ToCsvDocument().Should().Be("");
    }

    [Fact]
    public void ToCsvDocument_NullRows_ThrowsArgumentNullException()
    {
        IEnumerable<IEnumerable<object?>>? rows = null;
        var act = () => rows!.ToCsvDocument();
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region ToCsvDocument<T> (typed with projection)

    private record Person(string Name, int Age);

    [Fact]
    public void ToCsvDocument_TypedItems_SerializesWithProjection()
    {
        var people = new[] { new Person("Alice", 30), new Person("Bob", 25) };
        var result = people.ToCsvDocument(p => [p.Name, p.Age]);
        result.Should().Be($"Alice,30{Environment.NewLine}Bob,25");
    }

    [Fact]
    public void ToCsvDocument_WithHeaders_PrependHeaderRow()
    {
        var people = new[] { new Person("Alice", 30) };
        var result = people.ToCsvDocument(
            p => [p.Name, p.Age],
            headers: ["Name", "Age"]);
        result.Should().Be($"Name,Age{Environment.NewLine}Alice,30");
    }

    [Fact]
    public void ToCsvDocument_FieldWithComma_EscapesCorrectly()
    {
        var people = new[] { new Person("Smith, John", 40) };
        var result = people.ToCsvDocument(p => [p.Name, p.Age]);
        result.Should().Be("\"Smith, John\",40");
    }

    [Fact]
    public void ToCsvDocument_NullItems_ThrowsArgumentNullException()
    {
        IEnumerable<Person>? people = null;
        var act = () => people!.ToCsvDocument(p => [p.Name, p.Age]);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ToCsvDocument_NullMapRow_ThrowsArgumentNullException()
    {
        var people = new[] { new Person("Alice", 30) };
        Func<Person, IEnumerable<object?>>? mapRow = null;
        var act = () => people.ToCsvDocument(mapRow!);
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region FromCsvDocument

    [Fact]
    public void FromCsvDocument_TwoRows_SplitsCorrectly()
    {
        var csv = $"a,b{Environment.NewLine}c,d";
        var result = csv.FromCsvDocument().ToArray();
        result.Should().HaveCount(2);
        result[0].Should().Equal("a", "b");
        result[1].Should().Equal("c", "d");
    }

    [Fact]
    public void FromCsvDocument_QuotedFields_UnescapesCorrectly()
    {
        var csv = $"\"hello, world\",test{Environment.NewLine}foo,bar";
        var result = csv.FromCsvDocument().ToArray();
        result[0].Should().Equal("hello, world", "test");
    }

    [Fact]
    public void FromCsvDocument_EmptyString_ReturnsEmpty()
    {
        "".FromCsvDocument().Should().BeEmpty();
    }

    [Fact]
    public void FromCsvDocument_NullValue_ThrowsArgumentNullException()
    {
        string? csv = null;
        var act = () => csv!.FromCsvDocument();
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromCsvDocument_CustomDelimiter_UsesDelimiter()
    {
        var csv = $"a;b{Environment.NewLine}c;d";
        var result = csv.FromCsvDocument(';').ToArray();
        result[0].Should().Equal("a", "b");
    }

    #endregion

    #region FromCsv<T>

    [Fact]
    public void FromCsv_TypedItems_ParsesWithMapper()
    {
        var csv = $"Alice,30{Environment.NewLine}Bob,25";
        var result = csv.FromCsv(cols => new Person(cols[0], int.Parse(cols[1]))).ToArray();
        result.Should().HaveCount(2);
        result[0].Should().Be(new Person("Alice", 30));
        result[1].Should().Be(new Person("Bob", 25));
    }

    [Fact]
    public void FromCsv_WithHeaders_SkipsFirstRow()
    {
        var csv = $"Name,Age{Environment.NewLine}Alice,30{Environment.NewLine}Bob,25";
        var result = csv.FromCsv(cols => new Person(cols[0], int.Parse(cols[1])),
            hasHeaders: true).ToArray();
        result.Should().HaveCount(2);
    }

    [Fact]
    public void FromCsv_NullCsv_ThrowsArgumentNullException()
    {
        string? csv = null;
        var act = () => csv!.FromCsv(cols => new Person("", 0));
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromCsv_NullMapper_ThrowsArgumentNullException()
    {
        var act = () => "a,b".FromCsv((Func<string[], Person>?)null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void FromCsv_RoundTrip_PreservesData()
    {
        var people = new[] { new Person("Alice", 30), new Person("Bob", 25) };
        var csv = people.ToCsvDocument(p => [p.Name, p.Age]);
        var result = csv.FromCsv(cols => new Person(cols[0], int.Parse(cols[1]))).ToArray();
        result.Should().BeEquivalentTo(people);
    }

    #endregion
}

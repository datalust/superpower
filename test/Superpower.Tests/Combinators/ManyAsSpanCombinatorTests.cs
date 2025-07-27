namespace Superpower.Tests.Combinators;

public class ManyAsSpanCombinatorTests
{
	[Fact]
	public void ManyAsSpanSucceedsWithZeroMatches()
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var result = parser.Parse("");

		Assert.True(result.IsAtEnd);
		Assert.Equal(0, result.Length);
		Assert.Equal("", result.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanSucceedsWithSingleMatch()
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var result = parser.Parse("a");

		Assert.Equal(1, result.Length);
		Assert.Equal("a", result.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanSucceedsWithMultipleMatches()
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var result = parser.Parse("aaaaa");

		Assert.Equal(5, result.Length);
		Assert.Equal("aaaaa", result.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanStopsAtFirstNonMatch()
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var parseResult = parser.TryParse("aaabbb");

		Assert.True(parseResult.HasValue);
		Assert.Equal("aaa", parseResult.Value.ToStringValue());
		Assert.Equal("bbb", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanWorksWithComplexParser()
	{
		var digitParser = Character.Digit.ManyAsSpan();
		var result = digitParser.Parse("12345abc");

		Assert.Equal("12345", result.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanWorksWithDigitSequence()
	{
		var parser = Character.Digit.ManyAsSpan();
		var parseResult = parser.TryParse("123abc");

		Assert.True(parseResult.HasValue);
		Assert.Equal("123", parseResult.Value.ToStringValue());
		Assert.Equal("abc", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanWorksWithLetterSequence()
	{
		var parser = Character.Letter.ManyAsSpan();
		var parseResult = parser.TryParse("hello123");

		Assert.True(parseResult.HasValue);
		Assert.Equal("hello", parseResult.Value.ToStringValue());
		Assert.Equal("123", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanHandlesEmptyStringGracefully()
	{
		var parser = Character.EqualTo('x').ManyAsSpan();
		var parseResult = parser.TryParse("");

		Assert.True(parseResult.HasValue);
		Assert.True(parseResult.Value.IsAtEnd);
		Assert.Equal("", parseResult.Value.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanPreservesCorrectSourcePositions()
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var input = new TextSpan("aaabbb");
		var result = parser(input);

		Assert.True(result.HasValue);
		Assert.Equal(0, result.Value.Position.Absolute);
		Assert.Equal(3, result.Value.Length);
		Assert.Equal(3, result.Remainder.Position.Absolute);
	}

	[Fact]
	public void ManyAsSpanWithPartialMatchAndBacktrack()
	{
		var ab = Character.EqualTo('a').Then(_ => Character.EqualTo('b'));
		var parser = ab.Try().ManyAsSpan();
		var parseResult = parser.TryParse("ababac");

		Assert.True(parseResult.HasValue);
		Assert.Equal("abab", parseResult.Value.ToStringValue());
		Assert.Equal("ac", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanFailsWithPartialMatchWithoutBacktrack()
	{
		var ab = Character.EqualTo('a').Then(_ => Character.EqualTo('b'));
		var parser = ab.ManyAsSpan();
		var parseResult = parser.TryParse("ababac");

		Assert.False(parseResult.HasValue);
	}

	[Fact]
	public void ManyAsSpanThrowsOnZeroWidthParser()
	{
		var zeroWidthParser = Parse.Return('x');
		var manyParser = zeroWidthParser.ManyAsSpan();

		var exception = Assert.Throws<ParseException>(() => manyParser.Parse("anything"));
		Assert.Contains("ManyAsSpan() cannot be applied to zero-width parsers", exception.Message);
	}

	[Fact]
	public void ManyAsSpanThrowsArgumentNullExceptionForNullParser()
	{
		TextParser<char> nullParser = null!;
		Assert.Throws<ArgumentNullException>(() => nullParser.ManyAsSpan());
	}

	[Fact]
	public void ManyAsSpanWorksWithWhitespaceParser()
	{
		var parser = Character.WhiteSpace.ManyAsSpan();
		var parseResult = parser.TryParse("   \t\n  abc");

		Assert.True(parseResult.HasValue);
		Assert.Equal("   \t\n  ", parseResult.Value.ToStringValue());
		Assert.Equal("abc", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanWorksWithAlphanumericParser()
	{
		var parser = Character.LetterOrDigit.ManyAsSpan();
		var parseResult = parser.TryParse("abc123xyz!@#");

		Assert.True(parseResult.HasValue);
		Assert.Equal("abc123xyz", parseResult.Value.ToStringValue());
		Assert.Equal("!@#", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanCanBeUsedInComplexParsers()
	{
		var identifierParser =
			from first in Character.Letter
			from rest in Character.LetterOrDigit.ManyAsSpan()
			select new { First = first, Rest = rest.ToStringValue() };

		var result = identifierParser.Parse("hello123world!@#");

		Assert.Equal('h', result.First);
		Assert.Equal("ello123world", result.Rest);
	}

	[Fact]
	public void ManyAsSpanProducesCorrectSpanBoundaries()
	{
		var parser = Character.EqualTo('x').ManyAsSpan();
		var input = "xxxyyyzzz";
		var textSpan = new TextSpan(input);
		var result = parser(textSpan);

		Assert.True(result.HasValue);
		Assert.Same(input, result.Value.Source);
		Assert.Equal(0, result.Value.Position.Absolute);
		Assert.Equal(3, result.Value.Length);
		Assert.Equal("xxx", result.Value.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanWorksWithMatchedByPattern()
	{
		var parser = Span.MatchedBy(Character.EqualTo('a').ManyAsSpan());
		var result = parser.Parse("aaabbb");

		Assert.Equal("aaa", result.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanPerformanceComparedToMany()
	{
		// This test demonstrates that ManyAsSpan should be more efficient than Many
		// by not creating intermediate char arrays
		var manyParser = Character.Digit.Many().Select(chars => new string(chars));
		var manyAsSpanParser = Character.Digit.ManyAsSpan().Select(span => span.ToStringValue());

		var input = "1234567890abcdef";

		var manyResult = manyParser.Parse(input);
		var spanResult = manyAsSpanParser.Parse(input);

		Assert.Equal(manyResult, spanResult);
		Assert.Equal("1234567890", manyResult);
		Assert.Equal("1234567890", spanResult);
	}

	[Theory]
	[InlineData("", "")]
	[InlineData("a", "a")]
	[InlineData("aa", "aa")]
	[InlineData("aaa", "aaa")]
	[InlineData("aaab", "aaa")]
	[InlineData("b", "")]
	[InlineData("bbb", "")]
	public void ManyAsSpanTheoryTests(string input, string expected)
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var result = parser.Parse(input);
		Assert.Equal(expected, result.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanWithMultiCharacterSequenceParser()
	{
		var parser = Span.EqualTo("ab").ManyAsSpan();
		var parseResult = parser.TryParse("ababababcd");

		Assert.True(parseResult.HasValue);
		Assert.Equal("abababab", parseResult.Value.ToStringValue());
		Assert.Equal("cd", parseResult.Remainder.ToStringValue());
	}

	[Fact]
	public void ManyAsSpanReturnsCorrectLocationAndRemainder()
	{
		var parser = Character.EqualTo('a').ManyAsSpan();
		var input = new TextSpan("aaabbb");
		var result = parser(input);

		Assert.True(result.HasValue);
		Assert.Equal(input, result.Location); // Location should be the start
		Assert.Equal("bbb", result.Remainder.ToStringValue());
		Assert.Equal(3, result.Remainder.Position.Absolute);
	}
}
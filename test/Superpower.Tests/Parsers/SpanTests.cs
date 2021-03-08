using Superpower.Model;
using Superpower.Parsers;
using Xunit;

namespace Superpower.Tests.Parsers
{
    using System;
    using System.Text.RegularExpressions;

    public class SpanTests
    {
        [Theory]
        [InlineData("aaa", "aa", "aa")]
        [InlineData("aaa", "a+", "aaa")]
        [InlineData("aaa", "b", null)]
        [InlineData("abcd", "bc", "bc", 1)]
        [InlineData("abcd", "bc", null, 1, 1)]
        public void RegularExpressionParsersAreApplied(
            string input, 
            string regex, 
            string match, 
            int start = 0,
            int length = -1)
        {
            var parser = Span.Regex(regex);
            var i = new TextSpan(input).Skip(start).First(length == -1 ? input.Length - start : length);
            var r = parser(i);
            if (match == null && !r.HasValue)
                return; // Success, shouldn't have matched
            
            Assert.Equal(match, i.Until(r.Remainder).ToStringValue());
        }
    
        [Fact]
        public void WhiteSpaceMatches()
        {
            var parser = Span.WhiteSpace;
            var input = new TextSpan("  a");
            var r = parser(input);
            Assert.True(r.Value.ToStringValue() == "  ");
        }
    
        [Fact]
        public void WhiteSpaceDoesNotMatchZeroLength()
        {
            var parser = Span.WhiteSpace;
            var input = new TextSpan("a");
            var r = parser(input);
            Assert.False(r.HasValue);
        }
    
        [Fact]
        public void NonWhiteSpaceMatches()
        {
            var parser = Span.NonWhiteSpace;
            var input = new TextSpan("ab ");
            var r = parser(input);
            Assert.True(r.Value.ToStringValue() == "ab");
        }

        [Fact]
        public void NonWhiteSpaceDoesNotMatchZeroLength()
        {
            var parser = Span.NonWhiteSpace;
            var input = new TextSpan(" ");
            var r = parser(input);
            Assert.False(r.HasValue);
        }

        [Fact]
        public void MatchedByReturnsTheSpanMatchedByAParser()
        {
            var parser = Span.MatchedBy(Numerics.IntegerInt32);
            var input = new TextSpan("123abc");
            var r = parser(input);
            Assert.Equal("123", r.Value.ToStringValue());
        }

        [Fact]
        public void RegexMatches()
        {
            var parser = Span.Regex("foo", RegexOptions.IgnoreCase);
            var input = new TextSpan("Foo");
            var r = parser(input);
            Assert.Equal("Foo", r.Value.ToStringValue());
        }

        [Theory]
        [InlineData("STOP", "")]
        [InlineData("123STOP", "123")]
        [InlineData("STOP123", "")]
        [InlineData("123STOP456STOP789", "123")]
        public void UntilMatchesWhenStopwordIsPresent(string text, string expected)
        {
            var result = Span.Until("STOP").Parse(text);
            Assert.Equal(expected, result.ToStringValue());
        }

        [Theory]
        [InlineData("", "Syntax error: Until expected 'STOP'.")]
        [InlineData("123", "Syntax error (line 1, column 1): Until expected 'STOP'.")]
        [InlineData("12345", "Syntax error (line 1, column 1): Until expected 'STOP'.")]
        public void UntilFailsWhenStopwordIsNotPresent(string text, string expectedErrorMessage)
        {
            var parseException = Assert.Throws<ParseException>(() => Span.Until("STOP").Parse(text));
            Assert.Equal(0, parseException.ErrorPosition.Absolute);
            Assert.Equal(expectedErrorMessage, parseException.Message);
        }

        [Theory]
        [InlineData(null)]
        public void UntilFailsWhenArgumentIsNull(string text)
        {
            Assert.Throws<ArgumentNullException>(() => Span.Until(text).Parse(""));
        }
    }
}

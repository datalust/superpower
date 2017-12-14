using Superpower.Model;
using System;
using Xunit;

namespace Superpower.Tests
{
    public class StringSpanTests
    {
        [Fact]
        public void ADefaultSpanHasNoValue()
        {
            var span = default(TextSpan);
            Assert.Throws<InvalidOperationException>(() => span.ToStringValue());
        }

        [Theory]
        [InlineData("Hello", 0, 5, "Hello")]
        [InlineData("Hello", 1, 4, "ello")]
        [InlineData("Hello", 1, 3, "ell")]
        [InlineData("Hello", 0, 0, "")]
        public void ASpanIsEqualInValueToAMatchingString(string str, int offset, int length, string value)
        {
            var span = new TextSpan(str, new Position(offset, 1, offset + 1), length);
            Assert.True(span.EqualsValue(value));
        }

        [Theory]
        [InlineData("Hello", 0, 5, "HELLO")]
        [InlineData("Hello", 1, 4, "ELLO")]
        [InlineData("Hello", 1, 3, "ELL")]
        [InlineData("Hello", 0, 0, "")]
        public void ASpanIsEqualInValueIgnoringCaseToAMatchingUppsercaseString(string str, int offset, int length, string value)
        {
            var span = new TextSpan(str, new Position(offset, 1, offset + 1), length);
            Assert.True(span.EqualsValueIgnoreCase(value));
        }

        [Theory]
        [InlineData("Hello", 0, 5, "HELLO")]
        [InlineData("Hello", 1, 4, "Hell")]
        [InlineData("Hello", 1, 3, "fll")]
        public void ASpanIsNotEqualToADifferentString(string str, int offset, int length, string value)
        {
            var span = new TextSpan(str, new Position(offset, 1, offset + 1), length);
            Assert.False(span.EqualsValue(value));
        }


        [Theory]
        [InlineData("Hello World", 0, 11, "Hello|World", "Hello")]
        [InlineData("Hello World", 6, 5, "Hello|World", "World")]
        public void ASpanReturnsExpectedWhenConsumingRegex(string str, int offset, int length, string regex, string expected)
        {
            var span = new TextSpan(str, new Position(offset, 1, offset + 1), length);

            var actual = span.ConsumeRegex(regex);

            Assert.Equal(expected, actual.Value);
        }

        [Theory]
        [InlineData("Hello World", 0, 11, "Help|Stop")] // Help partially matches but should fail nonetheless
        [InlineData("Prefix Hello World", 0, 11, "Hello|World")] // Would only match if prefix gets skiped (i.e. regex is not anchored on current position)
        public void ASpanReturnsExpectedFailureWhenRegexDoesNotMatch(string str, int offset, int length, string regex)
        {
            var span = new TextSpan(str, new Position(offset, 1, offset + 1), length);

            var actual = span.ConsumeRegex(regex);

            Assert.False(actual.HasValue);
        }
    }
}

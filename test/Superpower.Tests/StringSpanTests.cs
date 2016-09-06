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
            var span = default(StringSpan);
            Assert.Throws<InvalidOperationException>(() => span.ToStringValue());
        }

        [Theory]
        [InlineData("Hello", 0, 5, "Hello")]
        [InlineData("Hello", 1, 4, "ello")]
        [InlineData("Hello", 1, 3, "ell")]
        [InlineData("Hello", 0, 0, "")]
        public void ASpanIsEqualInValueToAMatchingString(string str, int offset, int length, string value)
        {
            var span = new StringSpan(str, new Position(offset, 1, offset + 1), length);
            Assert.True(span.EqualsValue(value));
        }

        [Theory]
        [InlineData("Hello", 0, 5, "HELLO")]
        [InlineData("Hello", 1, 4, "ELLO")]
        [InlineData("Hello", 1, 3, "ELL")]
        [InlineData("Hello", 0, 0, "")]
        public void ASpanIsEqualInValueIgnoringCaseToAMatchingUppsercaseString(string str, int offset, int length, string value)
        {
            var span = new StringSpan(str, new Position(offset, 1, offset + 1), length);
            Assert.True(span.EqualsValueIgnoreCase(value));
        }

        [Theory]
        [InlineData("Hello", 0, 5, "HELLO")]
        [InlineData("Hello", 1, 4, "Hell")]
        [InlineData("Hello", 1, 3, "fll")]
        public void ASpanIsNotEqualToADifferentString(string str, int offset, int length, string value)
        {
            var span = new StringSpan(str, new Position(offset, 1, offset + 1), length);
            Assert.False(span.EqualsValue(value));
        }
    }
}

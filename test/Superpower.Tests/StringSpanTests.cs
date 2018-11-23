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

        [Fact]
        public void IdenticalSpansAreEqual()
        {
            var source = "123";
            var t1 = new TextSpan(source, Position.Zero, 1);
            var t2 = new TextSpan(source, Position.Zero, 1);
            Assert.Equal(t1, t2);
        }

        [Fact]
        public void SpansFromDifferentSourcesAreNotEqual()
        {
            string source1 = "123", source2 = "1234".Substring(0, 3);
            var t1 = new TextSpan(source1, Position.Zero, 1);
            var t2 = new TextSpan(source2, Position.Zero, 1);
            Assert.NotEqual(t1, t2);
        }

        [Fact]
        public void DifferentLengthSpansAreNotEqual()
        {
            var source = "123";
            var t1 = new TextSpan(source, Position.Zero, 1);
            var t2 = new TextSpan(source, Position.Zero, 2);
            Assert.NotEqual(t1, t2);
        }

        [Fact]
        public void SpansAtDifferentPositionsAreNotEqual()
        {
            var source = "111";
            var t1 = new TextSpan(source, Position.Zero, 1);
            var t2 = new TextSpan(source, new Position(1, 1, 1), 1);
            Assert.NotEqual(t1, t2);
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
    }
}

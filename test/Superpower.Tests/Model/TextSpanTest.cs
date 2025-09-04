using System;
using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Model
{
    public class TextSpanTest
    {
        [Theory]
        [InlineData("hello", 0, 5, "hello")]
        [InlineData("hello", 1, 3, "ell")]
        [InlineData("hello", 2, 0, "")]
        [InlineData("The quick brown fox jumps over the lazy dog", 9, 7, " brown ")]
        public void AsReadOnlySpanWorks(string input, int start, int length, string expected)
        {
            var span = new TextSpan(input).Skip(start).First(length);
            var readOnlySpan = span.AsReadOnlySpan();
            Assert.Equal(expected, readOnlySpan.ToString());
        }

        [Fact]
        public void AsReadOnlySpanEnsureHasValue()
        {
            Assert.Throws<InvalidOperationException>(() => TextSpan.None.AsReadOnlySpan());
        }
    }
}

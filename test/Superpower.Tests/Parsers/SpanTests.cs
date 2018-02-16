using Superpower.Model;
using Superpower.Parsers;
using Xunit;

namespace Superpower.Tests.Parsers
{
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
    }
}
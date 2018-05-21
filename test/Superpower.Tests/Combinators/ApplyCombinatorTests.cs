using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class ApplyCombinatorTests
    {
        [Fact]
        public void ApplyOnParsedSpanCallsAppliedParser()
        {
            var input = new TextSpan("1234");
            var twodigits = Span.Length(2).Apply(Numerics.IntegerInt32);
            var result = twodigits(input);
            Assert.Equal(12, result.Value);
        }

        [Fact]
        public void AnAppliedParserMustConsumeAllInput()
        {
            var input = new TextSpan("1234");
            var twodigits = Span.Length(2).Apply(Character.Digit);
            var result = twodigits(input);
            Assert.False(result.HasValue);
            Assert.Equal("Syntax error (line 1, column 2): unexpected `2`.", result.ToString());
        }

        [Fact]
        public void AnAppliedParserIsNotCalledIfThePrecedingParseFails()
        {
            var input = new TextSpan("1234");
            var twodigits = Span.EqualTo("aa").Apply(Character.Digit);
            var result = twodigits(input);
            Assert.False(result.HasValue);
            Assert.Equal("Syntax error (line 1, column 1): unexpected `1`, expected `aa`.", result.ToString());
        }

        [Fact]
        public void ApplyOnParsedTokenCallsAppliedParser()
        {
            var input = StringAsCharTokenList.Tokenize("abcd");
            var aAs42 = Token.EqualTo('a').Apply(Character.AnyChar.Value(42));
            var result = aAs42(input);
            Assert.Equal(42, result.Value);
        }
        
        [Fact]
        public void AnAppliedParserMustConsumeTheWholeTokenSpan()
        {
            var input = StringAsCharTokenList.Tokenize("abcd");
            var just42 = Token.EqualTo('a').Apply(Parse.Return(42));
            var result = just42(input);
            Assert.False(result.HasValue);
            // The "invalid a" here is the token name, since we're using characters as tokens - in normal use
            // this would read more like "invalid URI: unexpected `:`".
            Assert.Equal("Syntax error (line 1, column 1): invalid a, unexpected `a`.", result.ToString());
        }
    }
}
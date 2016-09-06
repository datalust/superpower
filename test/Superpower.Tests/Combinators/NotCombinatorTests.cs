using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class NotCombinatorTests
    {
        [Fact]
        public void NotSucceedsIfLookaheadFails()
        {
            AssertParser.SucceedsWith(Parse.Not(Span.EqualTo("ab")).Then(_ => Character.EqualTo('a')), "ac", 'a');
        }

        [Fact]
        public void NotFailsIfLookaheadSucceeds()
        {
            AssertParser.FailsWithMessage(Parse.Not(Span.EqualTo("ab")).Then(_ => Character.EqualTo('a')), "ab", 
                "Parsing failure (line 1, column 1): unexpected successful parsing of `ab`.");
        }

        [Fact]
        public void TokenNotSucceedsIfLookaheadFails()
        {
            AssertParser.SucceedsWith(Parse.Not(Token.EqualTo('a').Then(_ => Token.EqualTo('b'))).Then(_ => Token.EqualTo('a')), "ac", 'a');
        }

        [Fact]
        public void TokenNotFailsIfLookaheadSucceeds()
        {
            AssertParser.FailsWithMessage(Parse.Not(Token.Sequence('a', 'b')).Then(_ => Token.EqualTo('a')), "ab",
                "Parsing failure (line 1, column 1): unexpected successful parsing of `ab`.");
        }
    }
}

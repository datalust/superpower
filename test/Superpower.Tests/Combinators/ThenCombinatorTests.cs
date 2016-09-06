using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class ThenCombinatorTests
    {
        [Fact]
        public void ThenFailsIfFirstParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Then(_ => Character.EqualTo('b')), "cb");
        }

        [Fact]
        public void ThenFailsIfSecondParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Then(_ => Character.EqualTo('b')), "ac");
        }

        [Fact]
        public void ThenSucceedsIfBothSucceedInSequence()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Then(_ => Character.EqualTo('b')), "ab", 'b');
        }

        [Fact]
        public void TokenThenFailsIfFirstParserFails()
        {
            AssertParser.Fails(Token.EqualTo('a').Then(_ => Token.EqualTo('b')), "cb");
        }

        [Fact]
        public void TokenThenFailsIfSecondParserFails()
        {
            AssertParser.Fails(Token.EqualTo('a').Then(_ => Token.EqualTo('b')), "ac");
        }

        [Fact]
        public void TokenThenSucceedsIfBothSucceedInSequence()
        {
            AssertParser.SucceedsWith(Token.EqualTo('a').Then(_ => Token.EqualTo('b')), "ab", 'b');
        }
    }
}

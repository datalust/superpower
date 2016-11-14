using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class ManyCombinatorTests
    {
        [Fact]
        public void ManySucceedsWithNone()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').Many(), "");
        }

        [Fact]
        public void ManySucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').Many(), "a");
        }

        [Fact]
        public void ManySucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').Many(), "aa");
        }

        [Fact]
        public void ManyFailsWithPartialItemMatch()
        {
            var ab = Character.EqualTo('a').Then(_ => Character.EqualTo('b'));
            var list = ab.Many();
            AssertParser.Fails(list, "ababa");
        }

        [Fact]
        public void TokenManySucceedsWithNone()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').Many(), "");
        }

        [Fact]
        public void TokenManySucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').Many(), "a");
        }

        [Fact]
        public void TokenManySucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').Many(), "aa");
        }

        [Fact]
        public void TokenManyFailsWithPartialItemMatch()
        {
            var ab = Token.EqualTo('a').Then(_ => Token.EqualTo('b'));
            var list = ab.Many();
            AssertParser.Fails(list, "ababa");
        }
    }
}

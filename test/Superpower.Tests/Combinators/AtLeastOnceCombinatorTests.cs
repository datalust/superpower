using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class AtLeastOnceCombinatorTests
    {
        [Fact]
        public void AtLeastOnceSucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').AtLeastOnce(), "a");
        }

        [Fact]
        public void AtLeastOnceSucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').AtLeastOnce(), "aa");
        }

        [Fact]
        public void AtLeastOnceFailsWithNone()
        {
            AssertParser.Fails(Character.EqualTo('a').AtLeastOnce(), "");
        }

        [Fact]
        public void TokenAtLeastOnceSucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').AtLeastOnce(), "a");
        }

        [Fact]
        public void TokenAtLeastOnceSucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').AtLeastOnce(), "aa");
        }

        [Fact]
        public void TokenAtLeastOnceFailsWithNone()
        {
            AssertParser.Fails(Token.EqualTo('a').AtLeastOnce(), "");
        }
    }
}

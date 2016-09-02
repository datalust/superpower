using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class AtLeastOnceCombinatorTests
    {
        [Fact]
        public void AtLeastOnceSucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').AtLeastOnce(), "a");
        }

        [Fact]
        public void AtLeastOnceSucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').AtLeastOnce(), "aa");
        }

        [Fact]
        public void AtLeastOnceFailsWithNone()
        {
            AssertParser.Fails(Parse.Char('a').AtLeastOnce(), "");
        }

        [Fact]
        public void TokenAtLeastOnceSucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Parse.Token('a').AtLeastOnce(), "a");
        }

        [Fact]
        public void TokenAtLeastOnceSucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Parse.Token('a').AtLeastOnce(), "aa");
        }

        [Fact]
        public void TokenAtLeastOnceFailsWithNone()
        {
            AssertParser.Fails(Parse.Token('a').AtLeastOnce(), "");
        }
    }
}

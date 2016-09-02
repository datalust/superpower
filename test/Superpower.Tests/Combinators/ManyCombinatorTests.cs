using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class ManyCombinatorTests
    {
        [Fact]
        public void ManySucceedsWithNone()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "");
        }

        [Fact]
        public void ManySucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "a");
        }

        [Fact]
        public void ManySucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Parse.Char('a').Many(), "aa");
        }

        [Fact]
        public void TokenManySucceedsWithNone()
        {
            AssertParser.SucceedsWithAll(Parse.Token('a').Many(), "");
        }

        [Fact]
        public void TokenManySucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Parse.Token('a').Many(), "a");
        }

        [Fact]
        public void TokenManySucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Parse.Token('a').Many(), "aa");
        }
    }
}

using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class RepeatCombinatorTests
    {
        [Fact]
        public void RepeatSucceedsWithNone()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').Repeat(0), "");
        }

        [Fact]
        public void RepeatSucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').Repeat(1), "a");
        }

        [Fact]
        public void RepeatSucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Character.EqualTo('a').Repeat(2), "aa");
        }

        [Fact]
        public void RepeatFailsWithTooFew()
        {
            AssertParser.Fails(Character.EqualTo('a').Repeat(3), "aa");
        }

        [Fact]
        public void TokenRepeatSucceedsWithNone()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').Repeat(0), "");
        }

        [Fact]
        public void TokenRepeatSucceedsWithOne()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').Repeat(1), "a");
        }

        [Fact]
        public void TokenRepeatSucceedsWithTwo()
        {
            AssertParser.SucceedsWithAll(Token.EqualTo('a').Repeat(2), "aa");
        }

        [Fact]
        public void TokenRepeatFailsWithTooFew()
        {
            AssertParser.Fails(Token.EqualTo('a').Repeat(3), "aa");
        }
    }
}

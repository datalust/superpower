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
            AssertParser.Fails(Parse.Token('a').Then(_ => Parse.Token('b')), "cb");
        }

        [Fact]
        public void TokenThenFailsIfSecondParserFails()
        {
            AssertParser.Fails(Parse.Token('a').Then(_ => Parse.Token('b')), "ac");
        }

        [Fact]
        public void TokenThenSucceedsIfBothSucceedInSequence()
        {
            AssertParser.SucceedsWith(Parse.Token('a').Then(_ => Parse.Token('b')), "ab", 'b');
        }
    }
}

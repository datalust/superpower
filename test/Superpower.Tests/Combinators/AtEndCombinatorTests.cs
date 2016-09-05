using Superpower.Tests.Support;
using Superpower.Model;
using Xunit;
using Superpower.Parsers;

namespace Superpower.Tests.Combinators
{
    public class AtEndCombinatorTests
    {
        [Fact]
        public void AtEndSucceedsAtTheEnd()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').AtEnd(), "a", 'a');
        }

        [Fact]
        public void AtEndFailsIfThereIsARemainder()
        {
            AssertParser.Fails(Character.EqualTo('a').AtEnd(), "ab");
        }

        [Fact]
        public void AtEndFailsIfThePrecedingParserFails()
        {
            AssertParser.Fails(Character.EqualTo('b').AtEnd(), "a");
        }

        [Fact]
        public void AtEndSucceedsIfThereIsNoInput()
        {
            AssertParser.SucceedsWith(Parse.Return('a').AtEnd(), "", 'a');
        }

        [Fact]
        public void TokenAtEndSucceedsAtTheEnd()
        {
            AssertParser.SucceedsWith(Parse.Token('a').AtEnd(), "a", 'a');
        }

        [Fact]
        public void TokenAtEndFailsIfThereIsARemainder()
        {
            AssertParser.Fails(Parse.Token('a').AtEnd(), "ab");
        }

        [Fact]
        public void TokenAtEndFailsIfThePrecedingParserFails()
        {
            AssertParser.Fails(Parse.Token('b').AtEnd(), "a");
        }

        [Fact]
        public void TokenAtEndSucceedsIfThereIsNoInput()
        {
            AssertParser.SucceedsWith(Parse.Return<char, Token<char>>(new Token<char>('a', StringSpan.Empty)).AtEnd(), "", 'a');
        }
    }
}

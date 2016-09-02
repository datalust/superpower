using Superpower.Tests.Support;
using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class AtEndCombinatorTests
    {
        [Fact]
        public void AtEndSucceedsAtTheEnd()
        {
            AssertParser.SucceedsWith(Parse.Char('a').AtEnd(), "a", 'a');
        }

        [Fact]
        public void AtEndFailsIfThereIsARemainder()
        {
            AssertParser.Fails(Parse.Char('a').AtEnd(), "ab");
        }

        [Fact]
        public void AtEndFailsIfThePrecedingParserFails()
        {
            AssertParser.Fails(Parse.Char('b').AtEnd(), "a");
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

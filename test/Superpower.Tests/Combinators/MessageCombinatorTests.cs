using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class MessageCombinatorTests
    {
        [Fact]
        public void FailedParsingProducesMessage()
        {
            AssertParser.FailsWithMessage(Character.EqualTo('a').Message("hello"), "b", "Parsing failure (line 1, column 1): hello.");
        }

        [Fact]
        public void TokenFailedParsingProducesMessage()
        {
            AssertParser.FailsWithMessage(Token.EqualTo('a').Message("hello"), "b", "Parsing failure (line 1, column 1): hello.");
        }
    }
}

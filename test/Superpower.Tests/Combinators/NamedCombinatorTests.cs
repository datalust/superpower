using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class NamedCombinatorTests
    {
        [Fact]
        public void FailedParsingProducesMessage()
        {
            AssertParser.FailsWithMessage(Character.EqualTo('a').Named("hello"), "b", "Syntax error (line 1, column 1): unexpected `b`, expected hello.");
        }

        [Fact]
        public void TokenFailedParsingProducesMessage()
        {
            AssertParser.FailsWithMessage(Token.EqualTo('a').Named("hello"), "b", "Syntax error (line 1, column 1): unexpected b `b`, expected hello.");
        }
    }
}

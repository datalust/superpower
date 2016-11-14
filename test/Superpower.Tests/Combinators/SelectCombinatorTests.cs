using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class SelectCombinatorTests
    {
        [Fact]
        public void SelectFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Select(_ => 42), "b");
        }

        [Fact]
        public void SelectTransformsPrecedingResult()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Select(_ => 42), "a", 42);
        }

        [Fact]
        public void TokenSelectFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Select(_ => 42), "b");
        }

        [Fact]
        public void TokenSelectTransformsPrecedingResult()
        {
            AssertParser.SucceedsWith(Token.EqualTo('a').Select(_ => 42), "a", 42);
        }
    }
}

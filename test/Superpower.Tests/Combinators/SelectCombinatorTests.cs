using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class SelectCombinatorTests
    {
        [Fact]
        public void SelectFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Parse.Char('a').Select(_ => 42), "b");
        }

        [Fact]
        public void SelectTransformsPrecedingResult()
        {
            AssertParser.SucceedsWith(Parse.Char('a').Select(_ => 42), "a", 42);
        }

        [Fact]
        public void TokenSelectFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Parse.Char('a').Select(_ => 42), "b");
        }

        [Fact]
        public void TokenSelectTransformsPrecedingResult()
        {
            AssertParser.SucceedsWith(Parse.Token('a').Select(_ => 42), "a", 42);
        }
    }
}

using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class ValueCombinatorTests
    {
        [Fact]
        public void ValueFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Value(42), "b");
        }

        [Fact]
        public void ValueTransformsPrecedingResult()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Value(42), "a", 42);
        }

        [Fact]
        public void TokenValueFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Value(42), "b");
        }

        [Fact]
        public void TokenValueTransformsPrecedingResult()
        {
            AssertParser.SucceedsWith(Parse.Token('a').Value(42), "a", 42);
        }
    }
}

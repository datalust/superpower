using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class WhereCombinatorTests
    {
        [Fact]
        public void WhereFailsIfPrecedingParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Where(_ => true), "b");
        }

        [Fact]
        public void WhereSucceedsWhenPredicateMatches()
        {
            AssertParser.SucceedsWith(Character.EqualTo('a').Where(a => a == 'a'), "a", 'a');
        }

        [Fact]
        public void WhereFailsWhenPredicateDoesNotMatch()
        {
            AssertParser.FailsWithMessage(
                Character.EqualTo('a').Where(a => a != 'a', "character should be an A"),
                "a",
                "Syntax error (line 1, column 1): character should be an A.");
        }
    }
}

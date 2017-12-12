using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class BetweenCombinatorTests
    {
        [Fact]
        public void BetweenFailsIfLeftParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Between(Character.EqualTo('('), Character.EqualTo(')')), "{a)");
        }

        [Fact]
        public void BetweenFailsIfRightParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Between(Character.EqualTo('('), Character.EqualTo(')')), "(a}");
        }

        [Fact]
        public void BetweenFailsIfMiddleParserFails()
        {
            AssertParser.Fails(Character.EqualTo('a').Between(Character.EqualTo('('), Character.EqualTo(')')), "(b)");
        }

        [Fact]
        public void BetweenSucceedsIfAllParsersSucceed()
        {
            AssertParser.SucceedsWith( Character.EqualTo( 'a' ).Between( Character.EqualTo( '(' ), Character.EqualTo( ')' ) ), "(a)", 'a' );
        }

        [Fact]
        public void TokenBetweenFailsIfLeftParserFails()
        {
            AssertParser.Fails( Token.EqualTo( 'a' ).Between( Token.EqualTo( '(' ), Token.EqualTo( ')' ) ), "{a)" );
        }

        [Fact]
        public void TokenBetweenFailsIfRightParserFails()
        {
            AssertParser.Fails( Token.EqualTo( 'a' ).Between( Token.EqualTo( '(' ), Token.EqualTo( ')' ) ), "(a}" );
        }

        [Fact]
        public void TokenBetweenFailsIfMiddleParserFails()
        {
            AssertParser.Fails( Token.EqualTo( 'a' ).Between( Token.EqualTo( '(' ), Token.EqualTo( ')' ) ), "(b)" );
        }

        [Fact]
        public void TokenBetweenSucceedsIfAllParsersSucceed()
        {
            AssertParser.SucceedsWith( Token.EqualTo( 'a' ).Between( Token.EqualTo( '(' ), Token.EqualTo( ')' ) ), "(a)", 'a' );
        }
    }
}
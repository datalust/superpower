using System;
using System.Linq;
using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class ChainCombinatorTests
    {
        [Fact]
        public void SuccessWithLongChains()
        {
            const int chainLength = 5000;
            string input = string.Join( "+",  Enumerable.Repeat("1", chainLength ) );
            AssertParser.SucceedsWith( Parse.Chain( Character.EqualTo('+'),
                Numerics.IntegerInt32, (opr,val1,val2) => val1 + val2 ), input, chainLength );
        }

        [Fact]
        public void TokenSuccessWithLongChains() {
            const int chainLength = 5000;
            string input = string.Join( "+", Enumerable.Repeat( "1", chainLength ) );

            AssertParser.SucceedsWith( Parse.Chain(
                Token.EqualTo( '+' ), Token.EqualTo( '1' ).Value( 1 ), ( opr, val1, val2 ) => val1 + val2 ), input, chainLength );
        }
  }
}

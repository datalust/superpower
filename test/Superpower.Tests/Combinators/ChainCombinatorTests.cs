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
            string input = string.Join("+", Enumerable.Repeat("1", chainLength));
            var chainParser = Parse.Chain(
                Character.EqualTo('+'),
                Numerics.IntegerInt32,
                (opr, val1, val2) => val1 + val2);

            AssertParser.SucceedsWith(chainParser, input, chainLength);
        }

        [Fact]
        public void TokenSuccessWithLongChains()
        {
            const int chainLength = 5000;
            string input = string.Join("+", Enumerable.Repeat("1", chainLength));

            var chainParser = Parse.Chain(
                Token.EqualTo('+'),
                Token.EqualTo('1').Value(1),
                (opr, val1, val2) => val1 + val2);

            AssertParser.SucceedsWith(chainParser, input, chainLength);
        }

        [Fact]
        public void ChainFailWithMultiTokenOperator()
        {
            // Addition is represented with operator '++'
            // If we only have one '+', ensure we get error
            var nPlusPlusN = Parse.Chain(
                Character.EqualTo('+').IgnoreThen(Character.EqualTo('+')),
                Numerics.IntegerInt32,
                (opr, val1, val2) => val1 + val2);

            AssertParser.FailsAt(nPlusPlusN, "1+1", 2);
        }

        [Fact]
        public void TokenChainFailWithMultiTokenOperator()
        {
            // Addition is represented with operator '++'
            // If we only have one '+', ensure we get error
            var nPlusPlusN = Parse.Chain(
                Token.EqualTo('+').IgnoreThen(Token.EqualTo('+')),
                Token.EqualTo('1').Value(1),
                (opr, val1, val2) => val1 + val2);

            AssertParser.FailsAt(nPlusPlusN, "1+1", 2);
        }
    }
}

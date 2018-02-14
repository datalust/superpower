using System.Linq;
using Superpower.Parsers;
using Superpower.Tests.SExpressionScenario;
using Superpower.Tokenizers;
using Xunit;

namespace Superpower.Tests.Tokenizers
{
    public class TokenizerBuilderTests
    {
        [Fact]
        public void SExpressionsCanBeTokenized()
        {
            var tokenizer = new TokenizerBuilder<SExpressionToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.EqualTo('('), SExpressionToken.LParen)
                .Match(Character.EqualTo(')'), SExpressionToken.RParen)
                .Match(Numerics.Integer, SExpressionToken.Number)
                .Match(Span.While(char.IsLetterOrDigit), SExpressionToken.Atom)
                .Build();

            var tokens = tokenizer.TryTokenize("abc (123 def)");
            Assert.True(tokens.HasValue);
            Assert.Equal(5, tokens.Value.Count());
        }
    }
}
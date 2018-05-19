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
                .Match(Numerics.Integer, SExpressionToken.Number, requireDelimiters: true)
                .Match(Character.Letter.IgnoreThen(Character.LetterOrDigit.AtLeastOnce()), SExpressionToken.Atom, requireDelimiters: true)
                .Ignore(Comment.ShellStyle)
                .Build();

            var tokens = tokenizer.TryTokenize("abc (123 def) # this is a comment");
            Assert.True(tokens.HasValue);
            Assert.Equal(5, tokens.Value.Count());
        }

        [Fact]
        public void KeywordsRequireDelimiters()
        {
            var tokenizer = new TokenizerBuilder<bool>()
                .Ignore(Span.WhiteSpace)
                .Match(Span.EqualTo("is"), true, requireDelimiters: true)
                .Match(Character.Letter.AtLeastOnce(), false, requireDelimiters: true)
                .Build();

            var tokens = tokenizer.TryTokenize("is isnot is notis ins not is");
            Assert.True(tokens.HasValue);
            Assert.Equal(7, tokens.Value.Count());
            Assert.Equal(3, tokens.Value.Count(v => v.Kind));
        }

        [Fact]
        public void PartiallyFailedTokenizationIsReported()
        {
            var tokenizer = new TokenizerBuilder<string>()
                .Match(Span.EqualTo("abc"), "abc")
                .Match(Span.EqualTo("def"), "def")
                .Build();

            var tokens = tokenizer.TryTokenize("abd");
            Assert.False(tokens.HasValue);
            var msg = tokens.ToString();
            Assert.Equal("Syntax error (line 1, column 3): unexpected `d`, expected `c`.", msg);
        }
    }
}
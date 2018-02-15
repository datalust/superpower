using System;
using System.CodeDom;
using System.Linq;
using Superpower.Tests.NumberListScenario;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests
{
    public class TokenizerTests
    {
        [Fact]
        public void TryTokenizeReportsFailures()
        {
            var tokenizer = new NumberListTokenizer();
            var result = tokenizer.TryTokenize("1 a");
            Assert.False(result.HasValue);
            Assert.Equal("unexpected `a`, expected digit", result.FormatErrorMessageFragment());
        }

        [Fact]
        public void TryTokenizeReportsCustomErrors()
        {
            var tokenizer = new NumberListTokenizer(useCustomErrors: true);
            var result = tokenizer.TryTokenize("1 a");
            Assert.False(result.HasValue);
            Assert.Equal("list must contain only numbers", result.FormatErrorMessageFragment());
        }

        [Fact]
        public void TokenizeThrowsOnFailure()
        {
            var tokenizer = new NumberListTokenizer();
            Assert.Throws<ParseException>(() => tokenizer.Tokenize("1 a"));
        }

        [Fact]
        public void TryTokenizeSucceedsIfTokenizationSucceeds()
        {
            var tokenizer = new NumberListTokenizer();
            var result = tokenizer.TryTokenize("1 23 456");
            Assert.True(result.HasValue);
        }

        [Fact]
        public void TokenizeReturnsAllProducedTokens()
        {
            var tokenizer = new NumberListTokenizer();
            var result = tokenizer.Tokenize("1 23 456");
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void TokenizationStateTracksTheLastProducedToken()
        {
            var tokenizer = new PreviousCheckingTokenizer();
            var input = new string('_', 6);
            var result = tokenizer.Tokenize(input);
            Assert.Equal(input.Length, result.Count());
        }
    }
}

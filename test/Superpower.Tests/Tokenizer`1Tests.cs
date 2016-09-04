using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Superpower.Tests.NumberListScenario;
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
            Assert.Equal(result.FormatErrorMessageFragment(), "unexpected `a`, expected digit");
        }

        [Fact]
        public void TryTokenizeReportsCustomErrors()
        {
            var tokenizer = new NumberListTokenizer(useCustomErrors: true);
            var result = tokenizer.TryTokenize("1 a");
            Assert.False(result.HasValue);
            Assert.Equal(result.FormatErrorMessageFragment(), "list must contain only numbers");
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
    }
}

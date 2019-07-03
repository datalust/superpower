using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Superpower.Benchmarks.NumberListScenario;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;
using Xunit;

namespace Superpower.Benchmarks
{
    [MemoryDiagnoser]
    public class TokenizerBuilderBenchmark
    {
        const int NumbersLength = 1000;
        static readonly string Numbers = string.Join(" ", Enumerable.Range(0, NumbersLength));

        static readonly Tokenizer<NumberListToken> BuilderTokenizer = new TokenizerBuilder<NumberListToken>()
            .Match(Numerics.Integer, NumberListToken.Number)
            .Ignore(Span.WhiteSpace)
            .Build();

        static void AssertComplete(TokenList<NumberListToken> numbers)
        {
            var tokens = numbers.ToArray();
            Assert.Equal(NumbersLength, tokens.Length);
            for (var i = 0; i < NumbersLength; ++i)
            {
                Assert.Equal(NumberListToken.Number, tokens[i].Kind);
                Assert.Equal(i.ToString(), tokens[i].ToStringValue());
            }
        }

        [Fact]
        public void Verify()
        {
            AssertComplete(HandCoded());
            AssertComplete(Builder());
        }

        [Fact]
        public void Benchmark()
        {
            BenchmarkRunner.Run<TokenizerBuilderBenchmark>();
        }

        [Benchmark(Baseline = true)]
        public TokenList<NumberListToken> HandCoded()
        {
            return NumberListTokenizer.Instance.Tokenize(Numbers);
        }

        [Benchmark]
        public TokenList<NumberListToken> Builder()
        {
            return BuilderTokenizer.Tokenize(Numbers);
        }
    }
}

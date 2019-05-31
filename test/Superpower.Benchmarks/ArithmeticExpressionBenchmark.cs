using System;
using System.Linq.Expressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sprache;
using Superpower.Benchmarks.ArithmeticExpressionScenario;
using Superpower.Model;
using Xunit;

namespace Superpower.Benchmarks
{
    [MemoryDiagnoser]
    public class ArithmeticExpressionBenchmark
    {
        // This benchmark includes construction of the input, and unwrapping of results, while
        // NumberListBenchmark does not.

        static readonly ArithmeticExpressionTokenizer Tokenizer = new ArithmeticExpressionTokenizer();
        const string Expression = "123 + 456 * 123 - 456 / 123 + 456 * 123 - 456 / 123 + 456 * 123 - 456 / 123 + 456 * 123 - 456 / 123 + 456 * 123 - 456";
        static readonly TokenList<ArithmeticExpressionToken> Tokens = Tokenizer.Tokenize(Expression);
        const int ExpectedValue = 280095;
        
        [Fact]
        public void Verify()
        {
            Assert.Equal(ExpectedValue, SpracheText().Compile()());
            Assert.Equal(ExpectedValue, SuperpowerTokenListParser().Compile()());
            Assert.Equal(ExpectedValue, SuperpowerComplete().Compile()());
        }

        [Fact]
        public void Benchmark()
        {
            BenchmarkRunner.Run<ArithmeticExpressionBenchmark>();
        }

        [Benchmark(Baseline=true)]
        public Expression<Func<int>> SpracheText()
        {
            return SpracheArithmeticExpressionParser.Lambda.Parse(Expression);
        }
        
        [Benchmark]
        public TokenList<ArithmeticExpressionToken> SuperpowerTokenizer()
        {
            return Tokenizer.Tokenize(Expression);
        } 
        
        [Benchmark]
        public Expression<Func<int>> SuperpowerTokenListParser()
        {
            return ArithmeticExpressionParser.Lambda.Parse(Tokens);
        }
        
        [Benchmark]
        public Expression<Func<int>> SuperpowerComplete()
        {
            return ArithmeticExpressionParser.Lambda.Parse(Tokenizer.Tokenize(Expression));
        }
    }
}

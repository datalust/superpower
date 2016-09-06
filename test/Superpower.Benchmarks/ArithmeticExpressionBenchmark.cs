using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Sprache;
using Superpower;
using Superpower.Parsers;
using Superpower.Model;
using Superpower.Benchmarks.ArithmeticExpressionScenario;

namespace Superpower.Benchmarks
{
    public class ArithmeticExpressionBenchmark
    {
        static readonly ArithmeticExpressionTokenizer Tokenizer = new ArithmeticExpressionTokenizer();
        static readonly string Expression = "123 + 456 * 123 - 456 / 123 + 456 * 123 - 456 / 123 + 456 * 123 - 456 / 123 + 456 * 123 - 456 / 123 + 456 * 123 - 456";

        [Benchmark(Baseline=true)]
        public void Sprache()
        {
            var expr = SpracheArithmeticExpressionParser.Lambda.Parse(Expression);
            GC.KeepAlive(expr);
        }
        
        [Benchmark]
        public void SuperpowerToken()
        {
            var expr = ArithmeticExpressionParser.Lambda.Parse(Tokenizer.Tokenize(Expression));
            GC.KeepAlive(expr);
        }
    }
}

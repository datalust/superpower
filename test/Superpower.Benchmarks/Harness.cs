using BenchmarkDotNet.Running;
using Xunit;

namespace Superpower.Benchmarks
{
    public class Harness
    {
        [Fact]
        public void NumberListBenchmark()
        {
            BenchmarkRunner.Run<NumberListBenchmark>();
        }

        [Fact]
        public void ArithmeticExpressionBenchmark()
        {
            BenchmarkRunner.Run<ArithmeticExpressionBenchmark>();
        }
    }
}

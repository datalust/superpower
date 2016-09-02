using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    }
}

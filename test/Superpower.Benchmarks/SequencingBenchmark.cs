using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Superpower.Parsers;
using Superpower.Model;
using Xunit;

// ReSharper disable ParameterOnlyUsedForPreconditionCheck.Local

namespace Superpower.Benchmarks
{
    [MemoryDiagnoser]
    public class SequencingBenchmark
    {
        static readonly string Numbers = "123";
        static readonly TextSpan Input = new TextSpan(Numbers);

        static void AssertValues((char First, char Second, char Third) numbers)
        {
            Assert.Equal('1', numbers.First);
            Assert.Equal('2', numbers.Second);
            Assert.Equal('3', numbers.Third);
        }

        [Fact]
        public void Verify()
        {
            AssertValues(ApplyThen().Value);
            AssertValues(ApplySequence().Value);
        }

        [Fact]
        public void Benchmark()
        {
            BenchmarkRunner.Run<SequencingBenchmark>();
        }

        static readonly TextParser<(char, char, char)> ThenParser =
            Character.Digit.Then(first => 
                Character.Digit.Then(second =>
                    Character.Digit.Then(third => Parse.Return((first, second, third)))));

        [Benchmark(Baseline = true)]
        public Result<(char, char, char)> ApplyThen()
        {
            return ThenParser(Input);
        }

        static readonly TextParser<(char, char, char)> SequenceParser =
            Parse.Sequence(
                Character.Digit,
                Character.Digit,
                Character.Digit)
                .Select(t => t); // Even up the work done

        [Benchmark]
        public Result<(char, char, char)> ApplySequence()
        {
            return SequenceParser(Input);
        }
    }
}

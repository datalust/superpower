using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Sprache;
using Superpower.Parsers;
using Superpower.Model;
using Superpower.Benchmarks.NumberListScenario;
using Xunit;

namespace Superpower.Benchmarks
{
    [MemoryDiagnoser]
    public class NumberListBenchmark
    {
        public const int NumbersLength = 1000;
        static readonly string Numbers = string.Join(" ", Enumerable.Range(0, NumbersLength));
        static readonly Input SpracheInput = new Input(Numbers);
        static readonly TextSpan SuperpowerTextSpan = new TextSpan(Numbers);

        static void AssertComplete(int[] numbers)
        {
            Assert.Equal(NumbersLength, numbers.Length);
            for (var i = 0; i < NumbersLength; ++i)
                Assert.Equal(i, numbers[i]);
        }

        [Fact]
        public void Verify()
        {
            AssertComplete(StringSplitAndInt32Parse());
            AssertComplete(SpracheText().Value);
            AssertComplete(SuperpowerText().Value);
            AssertComplete(SuperpowerToken().Value);
        }

        [Fact]
        public void Benchmark()
        {
            BenchmarkRunner.Run<NumberListBenchmark>();
        }

        [Benchmark(Baseline = true)]
        public int[] StringSplitAndInt32Parse()
        {
            var tokens = Numbers.Split(' ');
            var numbers = new int[tokens.Length];
            for(var i = 0; i < tokens.Length; ++i)
            {
                numbers[i] = int.Parse(tokens[i]);
            }

            return numbers;
        }

        static readonly Parser<int[]> SpracheParser =
            Sprache.Parse.Number.Token()
                .Select(int.Parse)
                .Many()
                .Select(n => n.ToArray());

        [Benchmark]
        public IResult<int[]> SpracheText()
        {
            return SpracheParser(SpracheInput);
        }

        static readonly TextParser<int[]> SuperpowerTextParser =
            Span.WhiteSpace.Optional()
                .IgnoreThen(Numerics.IntegerInt32)
                .Many()
                .AtEnd();

        [Benchmark]
        public Result<int[]> SuperpowerText()
        {
            return SuperpowerTextParser(SuperpowerTextSpan);
        }

        static readonly TokenListParser<NumberListToken, int[]> SuperpowerTokenListParser =
            Token.EqualTo(NumberListToken.Number)
                .Apply(Numerics.IntegerInt32) // Slower that int.Parse(), but worth benchmarking
                .Many()
                .AtEnd();

        [Benchmark]
        public TokenListParserResult<NumberListToken, int[]> SuperpowerToken()
        {
            return SuperpowerTokenListParser(NumberListTokenizer.Instance.Tokenize(Numbers));
        }
    }
}

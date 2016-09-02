using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using Sprache;
using Superpower;
using Superpower.Parsers;
using Superpower.Model;
using Superpower.Benchmarks.NumberListScenario;
using Xunit;

namespace Superpower.Benchmarks
{
    public class NumberListBenchmark
    {
        const int NumbersLength = 1000;
        static readonly string Numbers = string.Join(" ", Enumerable.Range(0, NumbersLength));

        [Benchmark(Baseline = true)]
        public void StringSplitAndInt32Parse()
        {
            var tokens = Numbers.Split(' ');
            var numbers = new int[tokens.Length];
            for(var i = 0; i < tokens.Length; ++i)
            {
                numbers[i] = int.Parse(tokens[i]);
            }
            Assert.Equal(NumbersLength, numbers.Length);
        }

        static readonly Parser<int[]> SpracheSimpleParser =
            Sprache.Parse.Number.Token()
                .Select(int.Parse)
                .Many()
                .Select(n => n.ToArray());

        [Benchmark]
        public void SpracheSimple()
        {
            var numbers = SpracheSimpleParser.Parse(Numbers);
            Assert.Equal(NumbersLength, numbers.Length);
        }

        static readonly CharParser<int[]> SuperpowerSimpleParser =
            Numerics.Integer
                .Then(n => Parse.Char(' ').Many().Select(_ => int.Parse(n.ToStringValue())))
                .Many();

        [Benchmark]
        public void SuperpowerSimple()
        {
            var numbers = SuperpowerSimpleParser(new StringSpan(Numbers));
            Assert.Equal(NumbersLength, numbers.Value.Length);
        }

        static readonly CharParser<int[]> SuperpowerCharParser =
            Numerics.IntegerInt32
                .Then(n => Parse.Char(' ').Many().Select(_ => n))
                .Many();

        [Benchmark]
        public void SuperpowerChar()
        {
            var numbers = SuperpowerCharParser(new StringSpan(Numbers));
            Assert.Equal(NumbersLength, numbers.Value.Length);
        }

        static readonly TokenParser<NumberListToken, int[]> SuperpowerTokenParser =
            Parse.Token(NumberListToken.Number)
                .Select(t => int.Parse(t.Value))
                .Many();

        static readonly NumberListTokenizer Tokenizer = new NumberListTokenizer();

        [Benchmark]
        public void SuperpowerToken()
        {
            var numbers = SuperpowerTokenParser(Tokenizer.Tokenize(Numbers));
            Assert.Equal(NumbersLength, numbers.Value.Length);
        }
    }
}
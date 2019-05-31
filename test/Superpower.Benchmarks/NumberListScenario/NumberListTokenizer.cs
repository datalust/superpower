using System.Collections.Generic;
using Superpower.Model;

namespace Superpower.Benchmarks.NumberListScenario
{
    public class NumberListTokenizer : Tokenizer<NumberListToken>
    {
        public static NumberListTokenizer Instance { get; } = new NumberListTokenizer();

        protected override IEnumerable<Result<NumberListToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                var ch = next.Value;
                if (ch >= '0' && ch <= '9')
                {
                    var start = next;
                    next = next.Remainder.ConsumeChar();
                    while (next.HasValue && next.Value >= '0' && next.Value <= '9')
                    {
                        next = next.Remainder.ConsumeChar();
                    }
                    yield return Result.Value(NumberListToken.Number, start.Location, next.Location);
                }
                else
                {
                    yield return Result.Empty<NumberListToken>(next.Location, new[] { "digit" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

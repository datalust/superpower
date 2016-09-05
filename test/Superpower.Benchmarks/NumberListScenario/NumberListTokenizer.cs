using System;
using System.Collections.Generic;
using Superpower.Model;
using Superpower.Parsers;

namespace Superpower.Benchmarks.NumberListScenario
{
    class NumberListTokenizer : Tokenizer<NumberListToken>
    {
        protected override IEnumerable<CharResult<NumberListToken>> Tokenize(StringSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                if (char.IsDigit(next.Value))
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.ConsumeChar();
                    yield return CharResult.Value(NumberListToken.Number, integer.Location, integer.Remainder);
                }
                else
                {
                    yield return CharResult.Empty<NumberListToken>(next.Location, new[] { "digit" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

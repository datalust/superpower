using System;
using System.Collections.Generic;
using Superpower.Model;
using Superpower.Parsers;

namespace Superpower.Benchmarks.NumberListScenario
{
    class NumberListTokenizer : Tokenizer<NumberListToken>
    {
        protected override IEnumerable<Token<NumberListToken>> Tokenize(StringSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                if (char.IsDigit(next.Value))
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.NextChar();
                    yield return new Token<NumberListToken>(NumberListToken.Number, integer.Value);
                }
                else
                {
                    throw new NotImplementedException("Tokenization errors not yet implemented.");
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

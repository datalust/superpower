using System;
using System.Collections.Generic;
using Superpower.Model;
using Superpower.Parsers;

namespace Superpower.Tests.NumberListScenario
{
    class NumberListTokenizer : Tokenizer<NumberListToken>
    {
        readonly bool _useCustomErrors;

        public NumberListTokenizer(bool useCustomErrors = false)
        {
            _useCustomErrors = useCustomErrors;
        }

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
                    next = integer.Remainder.NextChar();
                    yield return CharResult.Value(NumberListToken.Number, integer.Location, integer.Remainder);
                }
                else
                {
                    if (_useCustomErrors)
                    {
                        yield return CharResult.Empty<NumberListToken>(next.Location, "list must contain only numbers");
                    }
                    else
                    {
                        yield return CharResult.Empty<NumberListToken>(next.Location, new[] { "digit" });
                    }
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

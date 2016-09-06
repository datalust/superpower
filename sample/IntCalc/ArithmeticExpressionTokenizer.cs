using System.Collections.Generic;
using Superpower.Model;
using Superpower.Parsers;
using Superpower;

namespace IntCalc
{
    class ArithmeticExpressionTokenizer : Tokenizer<ArithmeticExpressionToken>
    {
        readonly Dictionary<char, ArithmeticExpressionToken> _operators = new Dictionary<char, ArithmeticExpressionToken>
        {
            ['+'] = ArithmeticExpressionToken.Plus,
            ['-'] = ArithmeticExpressionToken.Minus,
            ['*'] = ArithmeticExpressionToken.Times,
            ['/'] = ArithmeticExpressionToken.Divide,
            ['('] = ArithmeticExpressionToken.LParen,
            [')'] = ArithmeticExpressionToken.RParen,
        };

        protected override IEnumerable<CharResult<ArithmeticExpressionToken>> Tokenize(StringSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                ArithmeticExpressionToken charToken;

                if (char.IsDigit(next.Value))
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.ConsumeChar();
                    yield return CharResult.Value(ArithmeticExpressionToken.Number, integer.Location, integer.Remainder);
                }
                else if (_operators.TryGetValue(next.Value, out charToken))
                {
                    yield return CharResult.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else
                {
                    yield return CharResult.Empty<ArithmeticExpressionToken>(next.Location, new[] { "number", "operator" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

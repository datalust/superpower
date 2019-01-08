using System.Collections.Generic;
using Superpower.Model;
using Superpower.Parsers;

namespace Superpower.Tests.ArithmeticExpressionScenario
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

        protected override IEnumerable<Result<ArithmeticExpressionToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                var ch = next.Value;
                if (ch >= '0' && ch <= '9')
                {
                    var natural = Numerics.Natural(next.Location);
                    next = natural.Remainder.ConsumeChar();
                    yield return Result.Value(ArithmeticExpressionToken.Number, natural.Location, natural.Remainder);
                }
                else if (_operators.TryGetValue(ch, out var charToken))
                {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else
                {
                    yield return Result.Empty<ArithmeticExpressionToken>(next.Location, new[] { "number", "operator" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

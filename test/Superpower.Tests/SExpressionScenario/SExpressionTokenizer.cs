using Superpower;
using Superpower.Parsers;
using Superpower.Model;
using System.Collections.Generic;

namespace Superpower.Tests.SExpressionScenario
{
    class SExpressionTokenizer : Tokenizer<SExpressionToken>
    {
        protected override IEnumerable<Result<SExpressionToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                if (next.Value == '(')
                {
                    yield return Result.Value(SExpressionToken.LParen, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if (next.Value == ')')
                {
                    yield return Result.Value(SExpressionToken.RParen, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else if (next.Value >= '0' && next.Value <= '9')
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.ConsumeChar();

                    yield return Result.Value(SExpressionToken.Number, integer.Location, integer.Remainder);

                    if (next.HasValue && !char.IsPunctuation(next.Value) && !char.IsWhiteSpace(next.Value))
                    {
                        yield return Result.Empty<SExpressionToken>(next.Location, new[] {"whitespace", "punctuation"});
                    }
                }
                else
                {
                    var beginIdentifier = next.Location;
                    while (next.HasValue && char.IsLetterOrDigit(next.Value))
                    {
                        next = next.Remainder.ConsumeChar();
                    }

                    yield return Result.Value(SExpressionToken.Atom, beginIdentifier, next.Location);
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}
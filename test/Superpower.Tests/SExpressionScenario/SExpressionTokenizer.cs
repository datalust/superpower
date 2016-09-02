using Superpower;
using Superpower.Parsers;
using Superpower.Model;
using System.Collections.Generic;

namespace Superpower.Tests.SExpressionScenario
{
    class SExpressionTokenizer : Tokenizer<SExpressionToken>
    {
        protected override IEnumerable<Token<SExpressionToken>> Tokenize(StringSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                if (next.Value == '(')
                {
                    yield return new Token<SExpressionToken>(SExpressionToken.LParen, next.Location.First(1));
                    next = next.Remainder.NextChar();
                }
                else if (next.Value == ')')
                {
                    yield return new Token<SExpressionToken>(SExpressionToken.RParen, next.Location.First(1));
                    next = next.Remainder.NextChar();
                }
                else if (char.IsDigit(next.Value))
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.NextChar();

                    if (next.HasValue && !char.IsPunctuation(next.Value) && !char.IsWhiteSpace(next.Value))
                    {
                        // Yield token with error
                    }
                    else
                    {
                        yield return new Token<SExpressionToken>(SExpressionToken.Number, integer.Value);
                    }
                }
                else
                {
                    var beginIdentifier = next.Location;
                    while (next.HasValue && char.IsLetterOrDigit(next.Value))
                    {
                        next = next.Remainder.NextChar();
                    }

                    yield return new Token<SExpressionToken>(SExpressionToken.Atom, beginIdentifier.Until(next.Location));
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}
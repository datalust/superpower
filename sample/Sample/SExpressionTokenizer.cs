using Superpower;
using Superpower.Parsers;
using Superpower.Model;
using System.Collections.Generic;

namespace Sample
{
    class SExpressionTokenizer : Tokenizer<SExpressionToken>
    {
        public override IEnumerable<Token<SExpressionToken>> Tokenize(StringSpan span)
        {
            var next = EatWhitespace(span);
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
                    var beginNumber = next.Location;
                    while (next.HasValue && char.IsDigit(next.Value))
                    {
                        next = next.Remainder.NextChar();
                    }

                    if (next.HasValue && !char.IsPunctuation(next.Value) && !char.IsWhiteSpace(next.Value))
                    {
                        // Yield token with error
                    }
                    else
                    {
                        yield return new Token<SExpressionToken>(SExpressionToken.Number, beginNumber.Until(next.Location));
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

                next = EatWhitespace(next.Location);
            } while (next.HasValue);
        }

        static Result<char> EatWhitespace(StringSpan span)
        {
            var next = span.NextChar();
            while (next.HasValue && char.IsWhiteSpace(next.Value))
            {
                next = next.Remainder.NextChar();
            }
            return next;
        }
    }
}
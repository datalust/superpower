using System;
using System.Collections.Generic;
using Superpower.Model;
using Superpower.Util;

namespace Superpower
{
    public abstract class Tokenizer<TTokenKind>
    {
        public TokenList<TTokenKind> Tokenize(string source)
        {
            var result = TryTokenize(source);
            if (result.HasValue)
                return result.Value;

            throw new ParseException(result.ToString());
        }

        public CharResult<TokenList<TTokenKind>> TryTokenize(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var sourceSpan = new StringSpan(source);
            var remainder = sourceSpan;
            var results = new List<Token<TTokenKind>>();
            foreach (var result in Tokenize(sourceSpan))
            {
                if (!result.HasValue)
                    return CharResult.CastEmpty<TTokenKind, TokenList<TTokenKind>>(result);

                if (result.Remainder == remainder)
                    throw new ParseException($"Zero-width tokens are not supported; token {Presentation.FormatKind(result.Value)} at position {result.Location.Position}.");

                remainder = result.Remainder;
                results.Add(new Token<TTokenKind>(result.Value, result.Location.Until(result.Remainder)));
            }

            var value = new TokenList<TTokenKind>(results.ToArray());
            return CharResult.Value(value, sourceSpan, remainder);
        }

        protected abstract IEnumerable<CharResult<TTokenKind>> Tokenize(StringSpan stringSpan);

        protected static CharResult<char> SkipWhiteSpace(StringSpan span)
        {
            var next = span.ConsumeChar();
            while (next.HasValue && char.IsWhiteSpace(next.Value))
            {
                next = next.Remainder.ConsumeChar();
            }
            return next;
        }
    }
}
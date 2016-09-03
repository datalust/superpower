using System;
using System.Collections.Generic;
using Superpower.Model;

namespace Superpower
{
    public abstract class Tokenizer<TTokenKind>
    {
        public TokenList<TTokenKind> Tokenize(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var results = new List<Token<TTokenKind>>();
            foreach (var result in Tokenize(new StringSpan(source)))
            {
                if (!result.HasValue)
                    throw new ParseException(result.ToString());

                results.Add(new Token<TTokenKind>(result.Value, result.Location.Until(result.Remainder)));
            }

            return new TokenList<TTokenKind>(results.ToArray());
        }

        protected abstract IEnumerable<CharResult<TTokenKind>> Tokenize(StringSpan stringSpan);

        protected static CharResult<char> SkipWhiteSpace(StringSpan span)
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
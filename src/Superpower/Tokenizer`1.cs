using System;
using System.Collections.Generic;
using System.Linq;
using Superpower.Model;

namespace Superpower
{
    public abstract class Tokenizer<TTokenKind>
    {
        public TokenList<TTokenKind> Tokenize(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            return new TokenList<TTokenKind>(Tokenize(new StringSpan(source)).ToArray());
        }

        protected abstract IEnumerable<Token<TTokenKind>> Tokenize(StringSpan stringSpan);

        protected static Result<char> SkipWhiteSpace(StringSpan span)
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
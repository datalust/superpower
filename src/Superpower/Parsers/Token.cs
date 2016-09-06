using System;
using Superpower.Model;
using Superpower.Util;

namespace Superpower.Parsers
{
    public static class Token
    {
        public static TokenParser<TTokenKind, Token<TTokenKind>> EqualTo<TTokenKind>(TTokenKind token)
        {
            var expectations = new[] { Presentation.FormatKind(token) };

            return input =>
            {
                var next = input.ConsumeToken();
                if (!next.HasValue || !next.Value.Kind.Equals(token))
                    return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(input, expectations);

                return next;
            };
        }

        public static TokenParser<TTokenKind, Token<TTokenKind>[]> Sequence<TTokenKind>(params TTokenKind[] tokens)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));

            TokenParser<TTokenKind, Token<TTokenKind>[]> result = input => TokenResult.Value(new Token<TTokenKind>[tokens.Length], input, input);
            for (var i = 0; i < tokens.Length; ++i)
            {
                var token = EqualTo(tokens[i]);
                var index = i;
                result = result.Then(arr => token.Select(t => { arr[index] = t; return arr; }));
            }
            return result;
        }
    }
}

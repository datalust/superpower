using Superpower.Model;

namespace Superpower
{
    public static class Parse
    {
        public static CharParser<char> Char(char c)
        {
            return input =>
            {
                var next = input.NextChar();
                if (!next.HasValue || next.Value != c)
                    return Result.Empty<char>(input);

                return next;
            };
        }

        public static TokenParser<TTokenKind, Token<TTokenKind>> Token<TTokenKind>(TTokenKind token)
        {
            return input =>
            {
                var next = input.NextToken();
                if (!next.HasValue || !next.Value.Kind.Equals(token))
                    return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(input);

                return next;
            };
        }

        public static CharParser<T> Return<T>(T t)
        {
            return input => Result.Value(t, input, input);
        }

        public static TokenParser<TTokenKind, T> Return<TTokenKind, T>(T t)
        {
            return input => TokenResult.Value(t, input, input);
        }
    }
}

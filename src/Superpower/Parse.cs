using Superpower.Model;

namespace Superpower
{
    public static class Parse
    {
        public static CharParser<char> Char(char c)
        {
            var expectations = new[] { "`" + c + "`" };

            return input =>
            {
                var next = input.NextChar();
                if (!next.HasValue || next.Value != c)
                    return CharResult.Empty<char>(input, expectations);

                return next;
            };
        }

        public static TokenParser<TTokenKind, Token<TTokenKind>> Token<TTokenKind>(TTokenKind token)
        {
            var expectations = new[] { Presentation.FormatKind(token) };

            return input =>
            {
                var next = input.NextToken();
                if (!next.HasValue || !next.Value.Kind.Equals(token))
                    return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(input, expectations);

                return next;
            };
        }

        public static CharParser<T> Return<T>(T t)
        {
            return input => CharResult.Value(t, input, input);
        }

        public static TokenParser<TTokenKind, T> Return<TTokenKind, T>(T t)
        {
            return input => TokenResult.Value(t, input, input);
        }
    }
}

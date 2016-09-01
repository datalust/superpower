namespace Superpower.Model
{
    public static class TokenResult
    {
        public static TokenResult<TTokenKind, T> Empty<TTokenKind, T>(TokenList<TTokenKind> remainder)
        {
            return new TokenResult<TTokenKind, T>(remainder);
        }

        public static TokenResult<TTokenKind, T> Value<TTokenKind, T>(T value, TokenList<TTokenKind> location, TokenList<TTokenKind> remainder)
        {
            return new TokenResult<TTokenKind, T>(value, location, remainder);
        }
    }
}
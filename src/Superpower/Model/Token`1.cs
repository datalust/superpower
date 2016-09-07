namespace Superpower.Model
{
    public struct Token<TTokenKind>
    {
        public TTokenKind Kind { get; }
        public StringSpan Span { get; }
        public string ToStringValue() => Span.ToStringValue();
        public Position Position => Span.Position;
        public bool HasValue => Span != StringSpan.None;

        public Token(TTokenKind kind, StringSpan span)
        {
            Kind = kind;
            Span = span;
        }

        public static Token<TTokenKind> Empty { get; } = default(Token<TTokenKind>);

        public override string ToString()
        {
            if (!HasValue)
                return "(empty token)";

            return $"{Kind}@{Position}: {Span}";
        }
    }
}
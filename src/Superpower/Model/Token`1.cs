namespace Superpower.Model
{
    public struct Token<TTokenKind>
    {
        public TTokenKind Kind { get; }
        public StringSpan Span { get; }
        public string ToStringValue() => Span.ToStringValue();
        public Position Position => Span.Position;

        public Token(TTokenKind kind, StringSpan span)
        {
            Kind = kind;
            Span = span;
        }

        public static Token<TTokenKind> Empty { get; } = default(Token<TTokenKind>);

        public override string ToString()
        {
            if (Span == StringSpan.None)
                return "(empty token)";

            return $"{Kind}@{Position}: {Span}";
        }
    }
}
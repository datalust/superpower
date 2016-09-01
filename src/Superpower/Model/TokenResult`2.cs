using System;

namespace Superpower.Model
{
    public struct TokenResult<TTokenKind, T>
    {
        readonly T _value;

        public TokenList<TTokenKind> Location { get; }
        public TokenList<TTokenKind> Remainder { get; }
        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("TokenResult has no value.");
                return _value;
            }
        }

        public TokenResult(T value, TokenList<TTokenKind> location, TokenList<TTokenKind> remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
        }

        public TokenResult(TokenList<TTokenKind> remainder)
        {
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
        }

        public override string ToString()
        {
            if (Remainder == TokenList<TTokenKind>.Empty)
                return "(empty result)";

            if (HasValue)
                return "Value: " + _value;

            if (Remainder.IsAtEnd)
                return "Unexpected end of input";

            return $"Unexpected '{Remainder.NextToken()}'";
        }
    }
}

using System;
using System.Linq;

namespace Superpower.Model
{
    public struct TokenResult<TTokenKind, T>
    {
        readonly T _value;

        public TokenList<TTokenKind> Location { get; }
        public TokenList<TTokenKind> Remainder { get; }
        public bool HasValue { get; }
        public Position ErrorPosition { get; }
        public string ErrorMessage { get; }
        public TTokenKind[] Expectations { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("TokenResult has no value.");
                return _value;
            }
        }

        public bool IsPartial(TokenList<TTokenKind> @from) => ErrorPosition.HasValue || @from != Remainder;

        internal TokenResult(T value, TokenList<TTokenKind> location, TokenList<TTokenKind> remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            ErrorPosition = Position.Empty;
            ErrorMessage = null;
            Expectations = null;
        }

        internal TokenResult(TokenList<TTokenKind> remainder, Position errorPosition, string errorMessage, TTokenKind[] expectations)
        {
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
            ErrorPosition = errorPosition;
            ErrorMessage = errorMessage;
            Expectations = expectations;
        }        

        public override string ToString()
        {
            if (Remainder == TokenList<TTokenKind>.Empty)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Remainder.IsAtEnd)
            {
                var next = Remainder.NextToken().Value;
                var sourcePosition = ErrorPosition.HasValue ? ErrorPosition : next.Position;
                location = $" (line {sourcePosition.Line}, column {sourcePosition.Column})";
            }

            return $"Parsing failure{location}: {message}.";
        }

        string FormatErrorMessageFragment()
        {
            var message = ErrorMessage;
            if (message == null)
            {
                if (Remainder.IsAtEnd)
                {
                    message = "unexpected end of input";
                }
                else
                {
                    var next = Remainder.NextToken().Value;
                    var nextKind = FormatKind(next.Kind);
                    var nextValue = next.Value;
                    if (nextValue.Length > 12)
                        nextValue = nextValue.Substring(0, 9) + "...";
                    message = $"unexpected {nextKind} `{nextValue}`";
                }

                if (Expectations != null)
                {
                    var expected = FormatKind(Expectations.Last());
                    if (Expectations.Length > 1)
                        expected = $"{string.Join(", ", Expectations.Take(Expectations.Length - 1).Select(FormatKind))} or {expected}";
                    message += $", expected {expected}";
                }
            }
            return message;
        }

        string FormatKind(TTokenKind kind)
        {
            return kind.ToString().ToLower();
        }
    }
}

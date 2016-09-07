using System;
using Superpower.Display;
using Superpower.Util;

namespace Superpower.Model
{
    // Should probably be TokenParserResult<T> (or just a combined Result<TokenList<TTokenKind>,T> - but CharResult<T> is currently smaller)
    public struct TokenResult<TTokenKind, T>
    {
        readonly T _value;

        public TokenList<TTokenKind> Location { get; }
        public TokenList<TTokenKind> Remainder { get; }
        public bool HasValue { get; }
        public Position ErrorPosition { get; }
        public string ErrorMessage { get; }
        public string[] Expectations { get; }

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

        internal TokenResult(TokenList<TTokenKind> remainder, Position errorPosition, string errorMessage, string[] expectations)
        {
            // Errors don't really carry a location - it's always the remainder, which is the first item unable to
            // be successfully parsed.
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
                var next = Remainder.ConsumeToken().Value;
                var sourcePosition = ErrorPosition.HasValue ? ErrorPosition : next.Position;
                location = $" (line {sourcePosition.Line}, column {sourcePosition.Column})";
            }

            return $"Parsing failure{location}: {message}.";
        }

        public string FormatErrorMessageFragment()
        {
            if (ErrorMessage != null)
                return ErrorMessage;

            string message;
            if (Remainder.IsAtEnd)
            {
                message = "unexpected end of input";
            }
            else
            {
                var next = Remainder.ConsumeToken().Value;
                var appearance = Presentation.FormatAppearance(next.Kind, next.ToStringValue());
                message = $"unexpected {appearance}";
            }

            if (Expectations != null)
            {
                var expected = Friendly.List(Expectations);
                message += $", expected {expected}";
            }

            return message;
        }
    }
}

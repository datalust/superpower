using Superpower.Util;
using System;

namespace Superpower.Model
{
    // Should probably be CharParserResult<T>
    public struct CharResult<T>
    {
        readonly T _value;

        public StringSpan Location { get; }
        public StringSpan Remainder { get; }
        public bool HasValue { get; }

        // Just here to show symmetry with TokenResult`2.
        public Position ErrorPosition => Remainder.Position;
        public string ErrorMessage { get; }
        public string[] Expectations { get; }

        public bool IsPartial(StringSpan @from) => @from != Remainder;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Result has no value.");
                return _value;
            }
        }

        internal CharResult(T value, StringSpan location, StringSpan remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            ErrorMessage = null;
            Expectations = null;
        }

        internal CharResult(StringSpan remainder, string errorMessage, string[] expectations)
        {
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
            Expectations = expectations;
            ErrorMessage = errorMessage;
        }

        public override string ToString()
        {
            if (Remainder == StringSpan.None)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Remainder.IsAtEnd)
            {
                location = $" (line {Remainder.Position.Line}, column {Remainder.Position.Column})";
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
                var next = Remainder.ConsumeChar().Value;
                message = $"unexpected `{next}`";
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

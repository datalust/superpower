using System;

namespace Superpower.Model
{
    public struct Result<T>
    {
        readonly T _value;

        public StringSpan Location { get; }
        public StringSpan Remainder { get; }
        public bool HasValue { get; }

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Result has no value.");
                return _value;
            }
        }

        public Result(T value, StringSpan location, StringSpan remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
        }

        public Result(StringSpan remainder)
        {
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
        }

        public override string ToString()
        {
            if (Remainder == StringSpan.None)
                return "(empty result)";

            if (HasValue)
                return "Value: " + _value;

            if (Remainder.IsAtEnd)
                return "Unexpected end of input";

            return $"Unexpected '{Remainder.NextChar().Value}'";
        }
    }
}

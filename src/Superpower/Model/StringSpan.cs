using System;

namespace Superpower.Model
{
    public struct StringSpan : IEquatable<StringSpan>
    {
        public string Source { get; }
        public Position Position { get; }
        public int Length { get; }

        public StringSpan(string source)
            : this(source, Position.Zero, source.Length)
        {
        }

        public StringSpan(string source, Position position, int length)
        {
#if CHECKED
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "The length must be non-negative.");
            if (source.Length < position.Absolute + length)
                throw new ArgumentOutOfRangeException(nameof(length), "The token extends beyond the end of the input.");
#endif

            Source = source;
            Position = position;
            Length = length;
        }

        public static StringSpan None { get; } = default(StringSpan);

        public static StringSpan Empty { get; } = new StringSpan(string.Empty, Position.Zero, 0);

        public bool IsAtEnd
        {
            get
            {
                EnsureHasValue();
                return Length == 0;
            }
        }

        void EnsureHasValue()
        {
            if (Source == null)
                throw new InvalidOperationException("String span has no value.");
        }

        public CharResult<char> ConsumeChar()
        {
            EnsureHasValue();

            if (IsAtEnd)
                return CharResult.Empty<char>(this);

            var ch = Source[Position.Absolute];
            return CharResult.Value(ch, this, new StringSpan(Source, Position.Advance(ch), Length - 1));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is StringSpan))
                return false;

            return Equals((StringSpan)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Source?.GetHashCode() ?? 0) * 397) ^ Position.Absolute;
            }
        }

        public bool Equals(StringSpan other)
        {
            return string.Equals(Source, other.Source) && Position.Absolute == other.Position.Absolute;
        }

        public static bool operator ==(StringSpan lhs, StringSpan rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(StringSpan lhs, StringSpan rhs)
        {
            return !(lhs == rhs);
        }

        public StringSpan Until(StringSpan next)
        {
            var charCount = Length - next.Length;
            return First(charCount);
        }

        public StringSpan First(int length)
        {
#if CHECKED
            if (length > Length)
                throw new ArgumentOutOfRangeException(nameof(length), "Length exceeds the source span's length.");
#endif

            return new StringSpan(Source, Position, length);
        }

        public override string ToString()
        {
            if (Source == null)
                return "(empty source span)";

            return ToStringValue();
        }

        public string ToStringValue()
        {
            EnsureHasValue();
            return Source.Substring(Position.Absolute, Length);
        }

        public bool Equals(string otherValue)
        {
            if (otherValue == null) throw new ArgumentNullException(nameof(otherValue));
            EnsureHasValue();
            if (Length != otherValue.Length)
                return false;
            for (var i = 0; i < Length; ++i)
            {
                if (Source[Position.Absolute + i] != otherValue[i])
                    return false;
            }
            return true;
        }

        public bool EqualsIgnoreCase(string otherValue)
        {
            if (otherValue == null) throw new ArgumentNullException(nameof(otherValue));
            EnsureHasValue();
            if (Length != otherValue.Length)
                return false;
            for (var i = 0; i < Length; ++i)
            {
                if (char.ToUpperInvariant(Source[Position.Absolute + i]) != char.ToUpperInvariant(otherValue[i]))
                    return false;
            }
            return true;
        }
    }
}
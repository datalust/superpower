using Superpower.Model;
using System;

namespace Superpower.Parsers
{
    public class Span
    {
        public static CharParser<StringSpan> Length(int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));

            var expectations = new[] { "span of length " + length };
            return input =>
            {
                var remainder = input;
                for (var i = 0; i < length; ++i)
                {
                    var ch = remainder.ConsumeChar();
                    if (!ch.HasValue)
                        return CharResult.Empty<StringSpan>(ch.Remainder, expectations);
                    remainder = ch.Remainder;
                }
                return CharResult.Value(input.Until(remainder), input, remainder);
            };
        }

        public static CharParser<StringSpan> EqualTo(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var expectations = new[] { text };
            return input =>
            {
                var remainder = input;
                for (var i = 0; i < text.Length; ++i)
                {
                    var ch = remainder.ConsumeChar();
                    if (!ch.HasValue || ch.Value != text[i])
                        return CharResult.Empty<StringSpan>(ch.Remainder, expectations);
                    remainder = ch.Remainder;
                }
                return CharResult.Value(input.Until(remainder), input, remainder);
            };
        }

        public static CharParser<StringSpan> EqualToIgnoreCase(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var expectations = new[] { text };
            return input =>
            {
                var remainder = input;
                for (var i = 0; i < text.Length; ++i)
                {
                    var ch = remainder.ConsumeChar();
                    if (!ch.HasValue || char.ToUpperInvariant(ch.Value) != char.ToUpperInvariant(text[i]))
                        return CharResult.Empty<StringSpan>(ch.Remainder, expectations);
                    remainder = ch.Remainder;
                }
                return CharResult.Value(input.Until(remainder), input, remainder);
            };
        }

        public static CharParser<StringSpan> While(Func<char, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return input =>
            {
                var next = input.ConsumeChar();
                while (next.HasValue && predicate(next.Value))
                {
                    next = next.Remainder.ConsumeChar();
                }

                return CharResult.Value(input.Until(next.Location), input, next.Location);
            };
        }

        public static CharParser<StringSpan> Until(Func<char, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return While(ch => !predicate(ch));
        }

        public static CharParser<StringSpan> WhiteSpace { get; } = input =>
        {
            var next = input.ConsumeChar();
            while (next.HasValue && char.IsWhiteSpace(next.Value))
            {
                next = next.Remainder.ConsumeChar();
            }

            return CharResult.Value(input.Until(next.Location), input, next.Location);
        };
    }
}

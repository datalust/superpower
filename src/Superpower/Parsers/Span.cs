// Copyright 2016 Datalust, Superpower Contributors, Sprache Contributors
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  
//
//     http://www.apache.org/licenses/LICENSE-2.0  
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Superpower.Model;
using Superpower.Util;
using System;
using Superpower.Display;

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for spans of characters.
    /// </summary>
    public class Span
    {
        /// <summary>
        /// Parse a span of length <paramref name="length"/>/>.
        /// </summary>
        /// <param name="length">The number of characters to parse.</param>
        /// <returns>The parsed span.</returns>
        public static TextParser<TextSpan> Length(int length)
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
                    {
                        if (ch.Location == input)
                            return Result.Empty<TextSpan>(ch.Location, expectations);

                        var remaining = length - i;
                        return Result.Empty<TextSpan>(ch.Location, new[] { $"{remaining} more {Friendly.Pluralize("character", remaining)}" });
                    }
                    remainder = ch.Remainder;
                }
                return Result.Value(input.Until(remainder), input, remainder);
            };
        }

        /// <summary>
        /// Match a span equal to <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to match.</param>
        /// <returns>The matched text.</returns>
        public static TextParser<TextSpan> EqualTo(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var expectations = new[] { Presentation.FormatLiteral(text) };
            return input =>
            {
                var remainder = input;
                for (var i = 0; i < text.Length; ++i)
                {
                    var ch = remainder.ConsumeChar();
                    if (!ch.HasValue || ch.Value != text[i])
                    {
                        if (ch.Location == input)
                            return Result.Empty<TextSpan>(ch.Location, expectations);

                        return Result.Empty<TextSpan>(ch.Location, new[] { Presentation.FormatLiteral(text[i]) });
                    }
                    remainder = ch.Remainder;
                }
                return Result.Value(input.Until(remainder), input, remainder);
            };
        }

        /// <summary>
        /// Match a span equal to <paramref name="text"/>, ignoring invariant case.
        /// </summary>
        /// <param name="text">The text to match.</param>
        /// <returns>The matched text.</returns>
        public static TextParser<TextSpan> EqualToIgnoreCase(string text)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            var textUpper = text.ToUpperInvariant();

            var expectations = new[] { Presentation.FormatLiteral(text) };
            return input =>
            {
                var remainder = input;
                for (var i = 0; i < textUpper.Length; ++i)
                {
                    var ch = remainder.ConsumeChar();
                    if (!ch.HasValue || char.ToUpperInvariant(ch.Value) != textUpper[i])
                    {
                        if (ch.Location == input)
                            return Result.Empty<TextSpan>(ch.Location, expectations);

                        return Result.Empty<TextSpan>(ch.Location, new[] { Presentation.FormatLiteral(text[i]) });
                    }
                    remainder = ch.Remainder;
                }
                return Result.Value(input.Until(remainder), input, remainder);
            };
        }

        /// <summary>
        /// Match a span equal to a singe character <paramref name="ch"/>.
        /// </summary>
        /// <param name="ch">The character to match.</param>
        /// <returns>The matched text.</returns>
        public static TextParser<TextSpan> EqualTo(char ch)
        {
            var expectations = new[] { Presentation.FormatLiteral(ch) };
            return input =>
            {
                var result = input.ConsumeChar();
                if (!result.HasValue)
                    return Result.CastEmpty<char, TextSpan>(result);
                if (result.Value == ch)
                    return Result.Value(input.Until(result.Remainder), input, result.Remainder);
                return Result.Empty<TextSpan>(input, expectations);
            };
        }

        /// <summary>
        /// Match a span equal to a singe character <paramref name="ch"/>, ignoring invariant character case.
        /// </summary>
        /// <param name="ch">The character to match.</param>
        /// <returns>The matched text.</returns>
        public static TextParser<TextSpan> EqualToIgnoreCase(char ch)
        {
            var chToUpper = char.ToUpperInvariant(ch);
            var expectations = new[] { Presentation.FormatLiteral(ch) };
            return input =>
            {
                var result = input.ConsumeChar();
                if (!result.HasValue)
                    return Result.CastEmpty<char, TextSpan>(result);
                if (char.ToUpperInvariant(result.Value) == chToUpper)
                   return Result.Value(input.Until(result.Remainder), input, result.Remainder);
                return Result.Empty<TextSpan>(input, expectations);
            };
        }

        /// <summary>
        /// Parse until finding a character for which <paramref name="predicate"/> returns true.
        /// </summary>
        /// <param name="predicate">A predicate.</param>
        /// <returns>The matched text.</returns>
        public static TextParser<TextSpan> Until(Func<char, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return While(ch => !predicate(ch));
        }


        /// <summary>
        /// Parse until finding a character for which <paramref name="predicate"/> returns false.
        /// </summary>
        /// <param name="predicate">A predicate.</param>
        /// <returns>The matched text.</returns>
        public static TextParser<TextSpan> While(Func<char, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return input =>
            {
                var next = input.ConsumeChar();
                while (next.HasValue && predicate(next.Value))
                {
                    next = next.Remainder.ConsumeChar();
                }

                return Result.Value(input.Until(next.Location), input, next.Location);
            };
        }

        /// <summary>
        /// Parse until a non-whitespace character is encountered, returning the matched span of whitespace.
        /// </summary>
        public static TextParser<TextSpan> WhiteSpace { get; } = input =>
        {
            var next = input.ConsumeChar();
            while (next.HasValue && char.IsWhiteSpace(next.Value))
            {
                next = next.Remainder.ConsumeChar();
            }

            return Result.Value(input.Until(next.Location), input, next.Location);
        };
    }
}

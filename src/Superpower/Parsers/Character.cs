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
using System.Linq;
using Superpower.Display;

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for matching individual characters.
    /// </summary>
    public static class Character
    {
        static TextParser<char> Matching(Func<char, bool> predicate, string[] expectations)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (expectations == null) throw new ArgumentNullException(nameof(expectations));

            return input =>
            {
                var next = input.ConsumeChar();
                if (!next.HasValue || !predicate(next.Value))
                    return Result.Empty<char>(input, expectations);

                return next;
            };
        }

        /// <summary>
        /// Parse a single character matching <paramref name="predicate"/>.
        /// </summary>
        public static TextParser<char> Matching(Func<char, bool> predicate, string name)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return Matching(predicate, new[] { name });
        }

        /// <summary>
        /// Parse a single character except those matching <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">Characters not to match.</param>
        /// <param name="description">Description of characters that don't match.</param>
        /// <returns>A parser for characters except those matching <paramref name="predicate"/>.</returns>
        public static TextParser<char> Except(Func<char, bool> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (description == null) throw new ArgumentNullException(nameof(description));

            return Matching(c => !predicate(c), "any character except " + description);
        }

        /// <summary>
        /// Parse a single specified character.
        /// </summary>
        public static TextParser<char> EqualTo(char ch)
        {
            return Matching(parsed => parsed == ch, Presentation.FormatLiteral(ch));
        }

        /// <summary>
        /// Parse a single specified character, ignoring case differences.
        /// </summary>
        public static TextParser<char> EqualToIgnoreCase(char ch)
        {
            return Matching(parsed => char.ToUpper(parsed) == char.ToUpperInvariant(ch), Presentation.FormatLiteral(ch));
        }

        /// <summary>
        /// Parse any single character in <paramref name="chars"/>.
        /// </summary>
        public static TextParser<char> In(params char[] chars)
        {
            return Matching(chars.Contains, chars.Select(Presentation.FormatLiteral).ToArray());
        }

        /// <summary>
        /// Parse a single character except <paramref name="ch"/>.
        /// </summary>
        public static TextParser<char> Except(char ch)
        {
            return Except(parsed => parsed == ch, Presentation.FormatLiteral(ch));
        }

        /// <summary>
        /// Parse any single character except those in <paramref name="chars"/>.
        /// </summary>
        public static TextParser<char> ExceptIn(params char[] chars)
        {
            return Matching(c => !chars.Contains(c), "any character except " + Friendly.List(chars.Select(Presentation.FormatLiteral)));
        }

        /// <summary>
        /// Parse any character.
        /// </summary>
        public static TextParser<char> AnyChar { get; } = Matching(c => true, "any character");

        /// <summary>
        /// Parse a whitespace character.
        /// </summary>
        public static TextParser<char> WhiteSpace { get; } = Matching(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parse a digit.
        /// </summary>
        public static TextParser<char> Digit { get; } = Matching(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static TextParser<char> Letter { get; } = Matching(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static TextParser<char> LetterOrDigit { get; } = Matching(char.IsLetterOrDigit, new[] { "letter", "digit" });

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static TextParser<char> Lower { get; } = Matching(char.IsLower, "lowercase letter");

        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static TextParser<char> Upper { get; } = Matching(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static TextParser<char> Numeric { get; } = Matching(char.IsNumber, "numeric character");

        /// <summary>
        /// Parse a hexadecimal digit (0-9, a-f, A-F).
        /// </summary>
        public static TextParser<char> HexDigit { get; } = Matching(CharInfo.IsHexDigit, "hex digit");
    }
}


using Superpower.Model;
using Superpower.Util;
using System;
using System.Linq;

namespace Superpower.Parsers
{
    public static class Character
    {
        static CharParser<char> Matching(Func<char, bool> predicate, string[] expectations)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (expectations == null) throw new ArgumentNullException(nameof(expectations));

            return input =>
            {
                var next = input.ConsumeChar();
                if (!next.HasValue || !predicate(next.Value))
                    return CharResult.Empty<char>(input, expectations);

                return next;
            };
        }

        /// <summary>
        /// Parse a single character matching <paramref name="predicate"/>.
        /// </summary>
        public static CharParser<char> Matching(Func<char, bool> predicate, string name)
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
        public static CharParser<char> Except(Func<char, bool> predicate, string description)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            if (description == null) throw new ArgumentNullException(nameof(description));

            return Matching(c => !predicate(c), "any character except " + description);
        }

        /// <summary>
        /// Parse a single specified character.
        /// </summary>
        public static CharParser<char> EqualTo(char ch)
        {
            return Matching(parsed => parsed == ch, Presentation.FormatCharacter(ch));
        }

        /// <summary>
        /// Parse any single character in <paramref name="chars"/>.
        /// </summary>
        public static CharParser<char> In(params char[] chars)
        {
            return Matching(chars.Contains, chars.Select(Presentation.FormatCharacter).ToArray());
        }

        /// <summary>
        /// Parse a single character except <paramref name="ch"/>.
        /// </summary>
        public static CharParser<char> Except(char ch)
        {
            return Except(parsed => parsed == ch, Presentation.FormatCharacter(ch));
        }

        /// <summary>
        /// Parse any single character except those in <paramref name="chars"/>.
        /// </summary>
        public static CharParser<char> ExceptIn(params char[] chars)
        {
            return Matching(c => !chars.Contains(c), "any character except " + Presentation.List(chars.Select(Presentation.FormatCharacter)));
        }

        /// <summary>
        /// Parse any character.
        /// </summary>
        public static CharParser<char> AnyChar { get; } = Matching(c => true, "any character");

        /// <summary>
        /// Parse a whitespace character.
        /// </summary>
        public static CharParser<char> WhiteSpace { get; } = Matching(char.IsWhiteSpace, "whitespace");

        /// <summary>
        /// Parse a digit.
        /// </summary>
        public static CharParser<char> Digit { get; } = Matching(char.IsDigit, "digit");

        /// <summary>
        /// Parse a letter.
        /// </summary>
        public static CharParser<char> Letter { get; } = Matching(char.IsLetter, "letter");

        /// <summary>
        /// Parse a letter or digit.
        /// </summary>
        public static CharParser<char> LetterOrDigit { get; } = Matching(char.IsLetterOrDigit, new[] { "letter", "digit" });

        /// <summary>
        /// Parse a lowercase letter.
        /// </summary>
        public static CharParser<char> Lower { get; } = Matching(char.IsLower, "lowercase letter");

        /// <summary>
        /// Parse an uppercase letter.
        /// </summary>
        public static CharParser<char> Upper { get; } = Matching(char.IsUpper, "uppercase letter");

        /// <summary>
        /// Parse a numeric character.
        /// </summary>
        public static CharParser<char> Numeric { get; } = Matching(char.IsNumber, "numeric character");
    }
}


﻿// Copyright 2016 Datalust, Superpower Contributors, Sprache Contributors
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

using System;
using Superpower.Model;

namespace Superpower
{
    /// <summary>
    /// Helper methods for working with parsers.
    /// </summary>
    public static class ParserExtensions
    {
        /// <summary>
        /// Tries to parse the input without throwing an exception upon failure.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser</returns>
        /// <exception cref="ArgumentNullException">The parser or input is null.</exception>
        public static Result<T> TryParse<T>(this TextParser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == null) throw new ArgumentNullException(nameof(input));

            return parser(new TextSpan(input));
        }

        /// <summary>
        /// Tries to parse the input without throwing an exception upon failure.
        /// </summary>
        /// <typeparam name="TKind">The type of tokens consumed by the parser.</typeparam>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser</returns>
        /// <exception cref="ArgumentNullException">The parser or input is null.</exception>
        public static TokenListParserResult<TKind, T> TryParse<TKind, T>(this TokenListParser<TKind, T> parser, TokenList<TKind> input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == null) throw new ArgumentNullException(nameof(input));

            return parser(input);
        }

        /// <summary>
        /// Parses the specified input string.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser.</returns>
        /// <exception cref="ArgumentNullException">The parser or input is null.</exception>
        /// <exception cref="ParseException">It contains the details of the parsing error.</exception>
        public static T Parse<T>(this TextParser<T> parser, string input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == null) throw new ArgumentNullException(nameof(input));

            var result = parser.TryParse(input);

            if (result.HasValue)
                return result.Value;

            throw new ParseException(result.ToString(), result.ErrorPosition);
        }

        /// <summary>
        /// Parses the specified input.
        /// </summary>
        /// <typeparam name="TKind">The type of tokens consumed by the parser.</typeparam>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>The result of the parser.</returns>
        /// <exception cref="ArgumentNullException">The parser or input is null.</exception>
        /// <exception cref="ParseException">It contains the details of the parsing error.</exception>
        public static T Parse<TKind, T>(this TokenListParser<TKind, T> parser, TokenList<TKind> input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == null) throw new ArgumentNullException(nameof(input));

            var result = parser.TryParse(input);

            if (result.HasValue)
                return result.Value;

            throw new ParseException(result.ToString(), result.ErrorPosition);
        }

        /// <summary>
        /// Tests whether the parser matches the entire provided <see cref="TextSpan"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parser's result.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="input">The input.</param>
        /// <returns>True if the parser is a complete match for the input; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">The parser is null.</exception>
        /// <exception cref="ArgumentException">The input is <see cref="TextSpan.Empty"/>.</exception>
        public static bool IsMatch<T>(this TextParser<T> parser, TextSpan input)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (input == TextSpan.Empty) throw new ArgumentException("Input text span is empty.", nameof(input));

            var result = parser(input);
            return result.HasValue && result.Remainder.IsAtEnd;
        }
    }
}

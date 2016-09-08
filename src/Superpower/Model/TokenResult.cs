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

namespace Superpower.Model
{
    /// <summary>
    /// Helper methods for working with <see cref="TokenResult{TTokenKind,T}"/>.
    /// </summary>
    public static class TokenResult
    {
        /// <summary>
        /// Create a token result with no value, indicating a failure to parse any value.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <returns>An empty result.</returns>
        public static TokenResult<TTokenKind, T> Empty<TTokenKind, T>(TokenList<TTokenKind> remainder)
        {
            return new TokenResult<TTokenKind, T>(remainder, Position.Empty, null, null);
        }

        /// <summary>
        /// Create a token result with no value, indicating a failure to parse any value.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <param name="expectations">Expectations that could not be fulfilled.</param>
        /// <returns>An empty result.</returns>
        public static TokenResult<TTokenKind, T> Empty<TTokenKind, T>(TokenList<TTokenKind> remainder, string[] expectations)
        {
            return new TokenResult<TTokenKind, T>(remainder, Position.Empty, null, expectations);
        }

        /// <summary>
        /// Create a token result with no value, indicating a failure to parse any value.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <param name="errorMessage">An error message describing why the tokens could not be parsed.</param>
        /// <returns>An empty result.</returns>
        public static TokenResult<TTokenKind, T> Empty<TTokenKind, T>(TokenList<TTokenKind> remainder, string errorMessage)
        {
            return new TokenResult<TTokenKind, T>(remainder, Position.Empty, errorMessage, null);
        }

        /// <summary>
        /// Create a token result with no value, indicating a failure to parse any value.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <param name="errorPosition">A source position within an individual token where parsing failed. In this case the position will be within
        /// the first token in <paramref name="remainder"/>.</param>
        /// <param name="errorMessage">A message describing the problem.</param>
        /// <returns>An empty result.</returns>
        public static TokenResult<TTokenKind, T> Empty<TTokenKind, T>(TokenList<TTokenKind> remainder, Position errorPosition, string errorMessage)
        {
            return new TokenResult<TTokenKind, T>(remainder, errorPosition, errorMessage, null);
        }

        /// <summary>
        /// Create a token result with the provided value.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="location">The location where parsing began.</param>
        /// <param name="remainder">The first un-parsed location.</param>
        /// <returns></returns>
        public static TokenResult<TTokenKind, T> Value<TTokenKind, T>(T value, TokenList<TTokenKind> location, TokenList<TTokenKind> remainder)
        {
            return new TokenResult<TTokenKind, T>(value, location, remainder);
        }

        /// <summary>
        /// Convert an empty result of one type into another.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <typeparam name="T">The source type.</typeparam>
        /// <typeparam name="U">The destination type.</typeparam>
        /// <param name="tokenResult">The result to convert.</param>
        /// <returns>The converted result.</returns>
        public static TokenResult<TTokenKind,U> CastEmpty<TTokenKind, T, U>(TokenResult<TTokenKind, T> tokenResult)
        {
            return new TokenResult<TTokenKind, U>(tokenResult.Remainder, tokenResult.ErrorPosition, tokenResult.ErrorMessage, tokenResult.Expectations);
        }

        /// <summary>
        /// Combine two empty results.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <typeparam name="TTokenKind">The kind of token.</typeparam>
        /// <param name="first">The first value to combine.</param>
        /// <param name="second">The second value to combine.</param>
        /// <returns>A result of type <typeparamref name="T"/> carrying information from both results.</returns>
        public static TokenResult<TTokenKind, T> CombineEmpty<TTokenKind, T>(TokenResult<TTokenKind, T> first, TokenResult<TTokenKind, T> second)
        {
            if (first.Remainder != second.Remainder)
                return second;

            var expectations = first.Expectations;
            if (expectations == null)
                expectations = second.Expectations;
            else if (second.Expectations != null)
            {
                expectations = new string[first.Expectations.Length + second.Expectations.Length];
                var i = 0;
                for (; i < first.Expectations.Length; ++i)
                    expectations[i] = first.Expectations[i];
                for (var j = 0; j < second.Expectations.Length; ++i, ++j)
                    expectations[i] = second.Expectations[j];
            }

            return new TokenResult<TTokenKind, T>(second.Remainder, second.ErrorPosition, first.ErrorMessage, expectations);
        }
    }
}
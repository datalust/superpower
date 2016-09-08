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
    /// Helper methods for working with <see cref="CharResult{T}"/>.
    /// </summary>
    public static class CharResult
    {
        /// <summary>
        /// An empty result indicating no value could be parsed.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <returns>A result.</returns>
        public static CharResult<T> Empty<T>(StringSpan remainder)
        {
            return new CharResult<T>(remainder, null, null);
        }

        /// <summary>
        /// An empty result indicating no value could be parsed.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <param name="expectations">Literal descriptions of expectations not met.</param>
        /// <returns>A result.</returns>
        public static CharResult<T> Empty<T>(StringSpan remainder, string[] expectations)
        {
            return new CharResult<T>(remainder, null, expectations);
        }

        /// <summary>
        /// An empty result indicating no value could be parsed.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <param name="errorMessage">Error message to present.</param>
        /// <returns>A result.</returns>
        public static CharResult<T> Empty<T>(StringSpan remainder, string errorMessage)
        {
            return new CharResult<T>(remainder, errorMessage, null);
        }

        /// <summary>
        /// A result carrying a successfully-parsed value.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="location">The location corresponding to the beginning of the parsed span.</param>
        /// <param name="remainder">The start of un-parsed input.</param>
        /// <returns>A result.</returns>
        public static CharResult<T> Value<T>(T value, StringSpan location, StringSpan remainder)
        {
            return new CharResult<T>(value, location, remainder);
        }

        /// <summary>
        /// Convert an empty result of one type into another.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <typeparam name="U">The target type.</typeparam>
        /// <param name="result">The value to convert.</param>
        /// <returns>A result of type <typeparamref name="U"/> carrying the same information as <paramref name="result"/>.</returns>
        public static CharResult<U> CastEmpty<T, U>(CharResult<T> result)
        {
            return new CharResult<U>(result.Remainder, result.ErrorMessage, result.Expectations);
        }

        /// <summary>
        /// Combine two empty results.
        /// </summary>
        /// <typeparam name="T">The source type.</typeparam>
        /// <param name="first">The first value to combine.</param>
        /// <param name="second">The second value to combine.</param>
        /// <returns>A result of type <typeparamref name="T"/> carrying information from both results.</returns>
        public static CharResult<T> CombineEmpty<T>(CharResult<T> first, CharResult<T> second)
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

            return new CharResult<T>(second.Remainder, second.ErrorMessage, expectations);
        }
    }
}

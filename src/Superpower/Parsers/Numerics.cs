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

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for numeric patterns.
    /// </summary>
    public static class Numerics
    {
        static readonly string[] ExpectedDigit = { "digit" };

        /// <summary>
        /// A string of digits.
        /// </summary>
        public static TextParser<TextSpan> Integer { get; } = input =>
        {
            var next = input.ConsumeChar();
            if (!next.HasValue || !char.IsDigit(next.Value))
                return Result.Empty<TextSpan>(input, ExpectedDigit);

            TextSpan remainder;
            do
            {
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            return Result.Value(input.Until(remainder), input, remainder);
        };

        /// <summary>
        /// A string of digits, converted into an <see cref="int"/>.
        /// </summary>
        public static TextParser<int> IntegerInt32 { get; } = input =>
        {
            var next = input.ConsumeChar();
            if (!next.HasValue || !char.IsDigit(next.Value))
                return Result.Empty<int>(input, ExpectedDigit);

            TextSpan remainder;
            var val = 0;
            do
            {
                val = 10 * val + (next.Value - '0');
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            return Result.Value(val, input, remainder);
        };
    }
}
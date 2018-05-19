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
    //* Fairly large amount of duplication/repetition here, due to the lack
    //* of generics over numbers in C#.
    public static class Numerics
    {
        static readonly string[] ExpectedDigit = { "digit" };
        static readonly string[] ExpectedSignOrDigit = { "sign", "digit" };

        /// <summary>
        /// A string of digits.
        /// </summary>
        public static TextParser<TextSpan> Natural { get; } = input =>
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
        /// A string of digits, converted into a <see cref="uint"/>.
        /// </summary>
        public static TextParser<uint> NaturalUInt32 { get; } = input =>
        {
            var next = input.ConsumeChar();
            
            if (!next.HasValue || !char.IsDigit(next.Value))
                return Result.Empty<uint>(input, ExpectedDigit);

            TextSpan remainder;
            var val = 0u;
            do
            {
                val = 10 * val + (uint)(next.Value - '0');
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));
            
            return Result.Value(val, input, remainder);
        };

        /// <summary>
        /// A string of digits, converted into a <see cref="ulong"/>.
        /// </summary>
        public static TextParser<ulong> NaturalUInt64 { get; } = input =>
        {
            var next = input.ConsumeChar();
            
            if (!next.HasValue || !char.IsDigit(next.Value))
                return Result.Empty<ulong>(input, ExpectedDigit);

            TextSpan remainder;
            var val = 0ul;
            do
            {
                val = 10 * val + (ulong)(next.Value - '0');
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));
            
            return Result.Value(val, input, remainder);
        };

        /// <summary>
        /// A string of digits with an optional +/- sign.
        /// </summary>
        public static TextParser<TextSpan> Integer { get; } = input =>
        {
            var next = input.ConsumeChar();
            
            if (!next.HasValue)
                return Result.Empty<TextSpan>(input, ExpectedSignOrDigit);
            
            if (next.Value == '-' || next.Value == '+')
                next = next.Remainder.ConsumeChar();

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
        /// A string of digits with an optional +/- sign, converted into an <see cref="int"/>.
        /// </summary>
        public static TextParser<int> IntegerInt32 { get; } = input =>
        {
            var negative = false;
            
            var next = input.ConsumeChar();

            if (!next.HasValue)
                return Result.Empty<int>(input, ExpectedSignOrDigit);
            
            if (next.Value == '-')
            {
                negative = true;
                next = next.Remainder.ConsumeChar();
            }
            else if (next.Value == '+')
            {
                next = next.Remainder.ConsumeChar();
            }
            
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

            if (negative)
                val = -val;
            
            return Result.Value(val, input, remainder);
        };

        /// <summary>
        /// A string of digits with an optional +/- sign, converted into an <see cref="long"/>.
        /// </summary>
        public static TextParser<long> IntegerInt64 { get; } = input =>
        {
            var negative = false;
            
            var next = input.ConsumeChar();

            if (!next.HasValue)
                return Result.Empty<long>(input, ExpectedSignOrDigit);
            
            if (next.Value == '-')
            {
                negative = true;
                next = next.Remainder.ConsumeChar();
            }
            else if (next.Value == '+')
            {
                next = next.Remainder.ConsumeChar();
            }
            
            if (!next.HasValue || !char.IsDigit(next.Value))
                return Result.Empty<long>(input, ExpectedDigit);

            TextSpan remainder;
            var val = 0L;
            do
            {
                val = 10 * val + (next.Value - '0');
                remainder = next.Remainder;
                next = remainder.ConsumeChar();
            } while (next.HasValue && char.IsDigit(next.Value));

            if (negative)
                val = -val;
            
            return Result.Value(val, input, remainder);
        };
        
        /// <summary>
        /// Matches decimal numbers, for example <code>-1.23</code>.
        /// </summary>
        public static TextParser<TextSpan> Decimal { get; } =
            Integer
                .Then(n => Character.EqualTo('.').IgnoreThen(Natural).OptionalOrDefault()
                    .Select(f => f == TextSpan.None ? n : new TextSpan(n.Source, n.Position, n.Length + f.Length + 1)));

        /// <summary>
        /// Matches <code>0x</code>-prefixed hexadecimal numbers.
        /// </summary>
        public static TextParser<TextSpan> HexNatural { get; } =
            Span.MatchedBy(Span.EqualTo("0x")
                .IgnoreThen(Character.Digit
                    .Or(Character.Matching(ch => ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F', "a-f"))
                    .Named("hex digit")
                    .AtLeastOnce()));
    }
}
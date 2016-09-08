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

using System;
using Superpower.Display;
using Superpower.Util;

namespace Superpower.Model
{
    /// <summary>
    /// The result of parsing a character stream.
    /// </summary>
    /// <typeparam name="T">The type of the value being parsed.</typeparam>
    /// <typeparam name="TTokenKind">The kind of token being parsed.</typeparam>
    public struct TokenResult<TTokenKind, T>
    {
        readonly T _value;

        /// <summary>
        /// The location in the stream where the parsing began.
        /// </summary>
        public TokenList<TTokenKind> Location { get; }

        /// <summary>
        /// The first un-parsed location in the stream.
        /// </summary>
        public TokenList<TTokenKind> Remainder { get; }

        /// <summary>
        /// True if the result carries a successfully-parsed value; otherwise, false.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// The position of the first un-parsed location.
        /// </summary>
        public Position ErrorPosition { get; }

        /// <summary>
        /// A provided error message, or null.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// A list of expectations that were unmet, or null.
        /// </summary>
        public string[] Expectations { get; }

        /// <summary>
        /// The parsed value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("TokenResult has no value.");
                return _value;
            }
        }

        internal bool IsPartial(TokenList<TTokenKind> @from) => ErrorPosition.HasValue || @from != Remainder;

        internal TokenResult(T value, TokenList<TTokenKind> location, TokenList<TTokenKind> remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            ErrorPosition = Position.Empty;
            ErrorMessage = null;
            Expectations = null;
        }

        internal TokenResult(TokenList<TTokenKind> remainder, Position errorPosition, string errorMessage, string[] expectations)
        {
            // Errors don't really carry a location - it's always the remainder, which is the first item unable to
            // be successfully parsed.
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
            ErrorPosition = errorPosition;
            ErrorMessage = errorMessage;
            Expectations = expectations;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Remainder == TokenList<TTokenKind>.Empty)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Remainder.IsAtEnd)
            {
                var next = Remainder.ConsumeToken().Value;
                var sourcePosition = ErrorPosition.HasValue ? ErrorPosition : next.Position;
                location = $" (line {sourcePosition.Line}, column {sourcePosition.Column})";
            }

            return $"Parsing failure{location}: {message}.";
        }

        /// <summary>
        /// If the result is empty, format the fragment of text describing the error.
        /// </summary>
        /// <returns>The error fragment.</returns>
        public string FormatErrorMessageFragment()
        {
            if (ErrorMessage != null)
                return ErrorMessage;

            string message;
            if (Remainder.IsAtEnd)
            {
                message = "unexpected end of input";
            }
            else
            {
                var next = Remainder.ConsumeToken().Value;
                var appearance = Presentation.FormatAppearance(next.Kind, next.ToStringValue());
                message = $"unexpected {appearance}";
            }

            if (Expectations != null)
            {
                var expected = Friendly.List(Expectations);
                message += $", expected {expected}";
            }

            return message;
        }
    }
}

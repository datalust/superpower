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
using System.Collections.Generic;
using Superpower.Display;
using Superpower.Util;

namespace Superpower.Model
{
    /// <summary>
    /// The result of parsing from a token list.
    /// </summary>
    /// <typeparam name="T">The type of the value being parsed.</typeparam>
    /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
    public struct TokenListParserResult<TKind, T> : IEquatable<TokenListParserResult<TKind, T>>
    {
        readonly T _value;

        /// <summary>
        /// If the result has a value, this carries the location of the value in the token
        /// list. If the result is an error, it's the location of the error.
        /// </summary>
        public TokenList<TKind> Location { get; }

        /// <summary>
        /// The first un-parsed location in the list.
        /// </summary>
        public TokenList<TKind> Remainder { get; }

        /// <summary>
        /// True if the result carries a successfully-parsed value; otherwise, false.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// If the result is an error, the source-level position of the error; otherwise, <see cref="Position.Empty"/>.
        /// </summary>
        public Position ErrorPosition
        {
            get
            {
                if (HasValue)
                    return Position.Empty;
                
                if (SubTokenErrorPosition.HasValue)
                    return SubTokenErrorPosition;
                
                if (!Remainder.IsAtEnd)
                    return Remainder.ConsumeToken().Value.Position;

                return Location.ComputeEndOfInputPosition();
            }
        }

        /// <summary>
        /// If the result is an error, the source-level position of the error; otherwise, <see cref="Position.Empty"/>.
        /// </summary>
        public Position SubTokenErrorPosition { get; }

        /// <summary>
        /// A provided error message, or null.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// A list of expectations that were unmet, or null.
        /// </summary>
        public string[]? Expectations { get; }

        /// <summary>
        /// The parsed value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException($"{nameof(TokenListParserResult)} has no value.");
                return _value;
            }
        }

        internal bool IsPartial(TokenList<TKind> from) => SubTokenErrorPosition.HasValue || from != Remainder;

        internal bool Backtrack { get; set; }

        internal TokenListParserResult(T value, TokenList<TKind> location, TokenList<TKind> remainder, bool backtrack)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            SubTokenErrorPosition = Position.Empty;
            ErrorMessage = null;
            Expectations = null;
            Backtrack = backtrack;
        }

        internal TokenListParserResult(TokenList<TKind> location, TokenList<TKind> remainder, Position errorPosition, string? errorMessage, string[]? expectations, bool backtrack)
        {
            Location = location;
            Remainder = remainder;
            _value = default!; // Default value is not observable.
            HasValue = false;
            SubTokenErrorPosition = errorPosition;
            ErrorMessage = errorMessage;
            Expectations = expectations;
            Backtrack = backtrack;
        }
        
        internal TokenListParserResult(TokenList<TKind> remainder, Position errorPosition, string? errorMessage, string[]? expectations, bool backtrack)
        {
            Location = Remainder = remainder;
            _value = default!; // Default value is not observable.
            HasValue = false;
            SubTokenErrorPosition = errorPosition;
            ErrorMessage = errorMessage;
            Expectations = expectations;
            Backtrack = backtrack;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Remainder == TokenList<TKind>.Empty)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Remainder.IsAtEnd)
            {
                // Since the message notes `end of input`, don't report line/column here.
                var sourcePosition = SubTokenErrorPosition.HasValue ? SubTokenErrorPosition : Remainder.ConsumeToken().Value.Position;
                location = $" (line {sourcePosition.Line}, column {sourcePosition.Column})";
            }

            return $"Syntax error{location}: {message}.";
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(TokenListParserResult<TKind, T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) &&
                   Location.Equals(other.Location) &&
                   Remainder.Equals(other.Remainder) &&
                   HasValue == other.HasValue &&
                   SubTokenErrorPosition.Equals(other.SubTokenErrorPosition) &&
                   ErrorMessage == other.ErrorMessage &&
                   Equals(Expectations, other.Expectations) &&
                   Backtrack == other.Backtrack;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj) => obj is TokenListParserResult<TKind, T> other && Equals(other);

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EqualityComparer<T>.Default.GetHashCode(_value!);
                hashCode = (hashCode * 397) ^ Location.GetHashCode();
                hashCode = (hashCode * 397) ^ Remainder.GetHashCode();
                hashCode = (hashCode * 397) ^ HasValue.GetHashCode();
                hashCode = (hashCode * 397) ^ SubTokenErrorPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ (ErrorMessage != null ? ErrorMessage.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Expectations != null ? Expectations.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Backtrack.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(TokenListParserResult<TKind, T> left, TokenListParserResult<TKind, T> right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(TokenListParserResult<TKind, T> left, TokenListParserResult<TKind, T> right) => !left.Equals(right);

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

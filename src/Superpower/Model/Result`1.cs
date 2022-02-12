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

using Superpower.Util;
using System;
using System.Collections.Generic;

namespace Superpower.Model
{
    /// <summary>
    /// The result of parsing from a text span.
    /// </summary>
    /// <typeparam name="T">The type of the value being parsed.</typeparam>
    public struct Result<T> : IEquatable<Result<T>>
    {
        readonly T _value;

        /// <summary>
        /// If the result is a value, the location in the input corresponding to the
        /// value. If the result is an error, it's the location of the error.
        /// </summary>
        public TextSpan Location { get; }

        /// <summary>
        /// The first un-parsed location in the input.
        /// </summary>
        public TextSpan Remainder { get; }

        /// <summary>
        /// True if the result carries a successfully-parsed value; otherwise, false.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// If the result is an error, the source-level position of the error; otherwise, <see cref="Position.Empty"/>.
        /// </summary>
        public Position ErrorPosition => HasValue ? Position.Empty : Location.Position;

        /// <summary>
        /// A provided error message, or null.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// A list of expectations that were unmet, or null.
        /// </summary>
        public string[]? Expectations { get; }

        internal bool IsPartial(TextSpan from) => from != Remainder;

        internal bool Backtrack { get; set; }

        /// <summary>
        /// The parsed value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException($"{nameof(Result)} has no value.");
                return _value;
            }
        }

        internal Result(T value, TextSpan location, TextSpan remainder, bool backtrack)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            ErrorMessage = null;
            Expectations = null;
            Backtrack = backtrack;
        }

        internal Result(TextSpan location, TextSpan remainder, string? errorMessage, string[]? expectations, bool backtrack)
        {
            Location = location;
            Remainder = remainder;
            _value = default!; // Default value is not observable.
            HasValue = false;
            Expectations = expectations;
            ErrorMessage = errorMessage;
            Backtrack = backtrack;
        }

        internal Result(TextSpan remainder, string? errorMessage, string[]? expectations, bool backtrack)
        {
            Location = Remainder = remainder;
            _value = default!; // Default value is not observable.
            HasValue = false;
            Expectations = expectations;
            ErrorMessage = errorMessage;
            Backtrack = backtrack;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Remainder == TextSpan.None)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Location.IsAtEnd)
            {
                location = $" (line {Location.Position.Line}, column {Location.Position.Column})";
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
        public bool Equals(Result<T> other)
        {
            return EqualityComparer<T>.Default.Equals(_value, other._value) &&
                   Location.Equals(other.Location) &&
                   Remainder.Equals(other.Remainder) &&
                   HasValue == other.HasValue &&
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
        public override bool Equals(object? obj) => obj is Result<T> other && Equals(other);

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
        public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);

        /// <summary>
        /// If the result is empty, format the fragment of text describing the error.
        /// </summary>
        /// <returns>The error fragment.</returns>
        public string FormatErrorMessageFragment()
        {
            if (ErrorMessage != null)
                return ErrorMessage;
            
            string message;
            if (Location.IsAtEnd)
            {
                message = "unexpected end of input";
            }
            else
            {
                var next = Location.ConsumeChar().Value;
                message = $"unexpected {Display.Presentation.FormatLiteral(next)}";
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

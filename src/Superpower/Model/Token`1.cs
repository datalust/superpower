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

namespace Superpower.Model
{
    /// <summary>
    /// A token.
    /// </summary>
    /// <typeparam name="TKind">The type of the token's kind.</typeparam>
    public struct Token<TKind> : IEquatable<Token<TKind>>
    {
        /// <summary>
        /// The kind of the token.
        /// </summary>
        public TKind Kind { get; }

        /// <summary>
        /// The string span containing the value of the token.
        /// </summary>
        public TextSpan Span { get; }

        /// <summary>
        /// Get the string value of the token.
        /// </summary>
        /// <returns>The token as a string.</returns>
        public string ToStringValue() => Span.ToStringValue();

        /// <summary>
        /// The position of the token within the source string.
        /// </summary>
        public Position Position => Span.Position;

        /// <summary>
        /// True if the token has a value.
        /// </summary>
        public bool HasValue => Span != TextSpan.None;

        /// <summary>
        /// Construct a token.
        /// </summary>
        /// <param name="kind">The kind of the token.</param>
        /// <param name="span">The span holding the token's value.</param>
        public Token(TKind kind, TextSpan span)
        {
            Kind = kind;
            Span = span;
        }

        /// <summary>
        /// A token with no value.
        /// </summary>
        public static Token<TKind> Empty { get; } = default;

        /// <inheritdoc/>
        public override string ToString()
        {
            if (!HasValue)
                return "(empty token)";

            return $"{Kind}@{Position}: {Span}";
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Token<TKind> other) => EqualityComparer<TKind>.Default.Equals(Kind, other.Kind) && Span.Equals(other.Span);

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj) => obj is Token<TKind> other && Equals(other);

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
                return (EqualityComparer<TKind>.Default.GetHashCode(Kind!) * 397) ^ Span.GetHashCode();
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
        public static bool operator ==(Token<TKind> left, Token<TKind> right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Token<TKind> left, Token<TKind> right) => !left.Equals(right);
    }
}
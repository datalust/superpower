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

namespace Superpower.Model
{
    /// <summary>
    /// A position within a stream of character input.
    /// </summary>
    public readonly struct Position : IEquatable<Position>
    {
        /// <summary>
        /// The zero-based absolute index of the position.
        /// </summary>
        public int Absolute { get; }

        /// <summary>
        /// The one-based line number.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The one-based column number.
        /// </summary>
        public int Column { get; }

        /// <summary>
        /// Construct a position.
        /// </summary>
        /// <param name="absolute">The absolute position.</param>
        /// <param name="line">The line number.</param>
        /// <param name="column">The column number.</param>
        public Position(int absolute, int line, int column)
        {
#if CHECKED
            if (absolute < 0) throw new ArgumentOutOfRangeException(nameof(line), "Absolute positions start at 0.");
            if (line < 1) throw new ArgumentOutOfRangeException(nameof(line), "Line numbering starts at 1.");
            if (column < 1) throw new ArgumentOutOfRangeException(nameof(column), "Column numbering starts at 1.");
#endif
            Absolute = absolute;
            Line = line;
            Column = column;
        }

        /// <summary>
        /// The position corresponding to the zero index.
        /// </summary>
        public static Position Zero { get; } = new Position(0, 1, 1);

        /// <summary>
        /// A position with no value.
        /// </summary>
        public static Position Empty { get; } = default;

        /// <summary>
        /// True if the position has a value.
        /// </summary>
        public bool HasValue => Line > 0;

        /// <summary>
        /// Advance over <paramref name="overChar"/>, advancing line and column numbers
        /// as appropriate.
        /// </summary>
        /// <param name="overChar">The character being advanced over.</param>
        /// <returns>The updated position.</returns>
        public Position Advance(char overChar)
        {
            if (overChar == '\n')
                return new Position(Absolute + 1, Line + 1, 1);

            return new Position(Absolute + 1, Line, Column + 1);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Absolute} (line {Line}, column {Column})";
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Position other) => Absolute == other.Absolute && Line == other.Line && Column == other.Column;

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// true if <paramref name="obj">obj</paramref> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj) => obj is Position other && Equals(other);

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
                var hashCode = Absolute;
                hashCode = (hashCode * 397) ^ Line;
                hashCode = (hashCode * 397) ^ Column;
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
        public static bool operator ==(Position left, Position right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Position left, Position right) => !left.Equals(right);
    }
}
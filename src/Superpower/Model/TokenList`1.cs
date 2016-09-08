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
using System.Collections;
using System.Collections.Generic;

namespace Superpower.Model
{
    /// <summary>
    /// A list of <see cref="Token{TTokenKind}"/>
    /// </summary>
    /// <typeparam name="TTokenKind">The kind of tokens held in the list.</typeparam>
    public struct TokenList<TTokenKind> : IEquatable<TokenList<TTokenKind>>, IEnumerable<Token<TTokenKind>>
    {
        readonly Token<TTokenKind>[] _tokens;

        /// <summary>
        /// The position of the token list in the token stream.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Construct a token list containing <paramref name="tokens"/>.
        /// </summary>
        /// <param name="tokens">The tokens in the list.</param>
        public TokenList(Token<TTokenKind>[] tokens)
            : this(tokens, 0)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
        }

        TokenList(Token<TTokenKind>[] tokens, int position)
        {
#if CHECKED // Called on every advance or backtrack
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            if (position > tokens.Length) throw new ArgumentOutOfRangeException(nameof(position), "Position is past end + 1.");
#endif

            Position = position;
            _tokens = tokens;
        }

        /// <summary>
        /// A token list with no value.
        /// </summary>
        public static TokenList<TTokenKind> Empty { get; } = default(TokenList<TTokenKind>);

        /// <summary>
        /// True if the token list contains no tokens.
        /// </summary>
        public bool IsAtEnd
        {
            get
            {
                EnsureHasValue();
                return Position == _tokens.Length;
            }
        }

        void EnsureHasValue()
        {
            if (_tokens == null)
                throw new InvalidOperationException("Token list has no value.");
        }

        /// <summary>
        /// Consume a token from the start of the list, returning a result with the token and remainder.
        /// </summary>
        /// <returns></returns>
        public TokenResult<TTokenKind, Token<TTokenKind>> ConsumeToken()
        {
            EnsureHasValue();

            if (IsAtEnd)
                return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(this);

            var token = _tokens[Position];
            return TokenResult.Value(token, this, new TokenList<TTokenKind>(_tokens, Position + 1));
        }

        /// <inheritdoc/>
        public IEnumerator<Token<TTokenKind>> GetEnumerator()
        {
            EnsureHasValue();

            for (var position = Position; position < _tokens.Length; ++position)
                yield return _tokens[position];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is TokenList<TTokenKind>))
                return false;

            return Equals((TokenList<TTokenKind>)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((_tokens?.GetHashCode() ?? 0) * 397) ^ Position;
            }
        }

        /// <summary>
        /// Compare two token lists using identity semantics - same list, same position.
        /// </summary>
        /// <param name="other">The other token list.</param>
        /// <returns>True if the token lists are the same.</returns>
        public bool Equals(TokenList<TTokenKind> other)
        {
            return Equals(_tokens, other._tokens) && Position == other.Position;
        }

        /// <summary>
        /// Compare two token lists using identity semantics.
        /// </summary>
        /// <param name="lhs">The first token list.</param>
        /// <param name="rhs">The second token list.</param>
        /// <returns>True if the token lists are the same.</returns>
        public static bool operator ==(TokenList<TTokenKind> lhs, TokenList<TTokenKind> rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Compare two token lists using identity semantics.
        /// </summary>
        /// <param name="lhs">The first token list.</param>
        /// <param name="rhs">The second token list.</param>
        /// <returns>True if the token lists are the different.</returns>
        public static bool operator !=(TokenList<TTokenKind> lhs, TokenList<TTokenKind> rhs)
        {
            return !(lhs == rhs);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (_tokens == null)
                return "Token stream (empty)";

            return "Token stream";
        }
    }
}
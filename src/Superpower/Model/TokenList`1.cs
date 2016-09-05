using System;
using System.Collections;
using System.Collections.Generic;

namespace Superpower.Model
{
    public struct TokenList<TTokenKind> : IEquatable<TokenList<TTokenKind>>, IEnumerable<Token<TTokenKind>>
    {
        readonly Token<TTokenKind>[] _tokens;

        public int Position { get; }

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

        public static TokenList<TTokenKind> Empty { get; } = default(TokenList<TTokenKind>);

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

        public TokenResult<TTokenKind, Token<TTokenKind>> ConsumeToken()
        {
            EnsureHasValue();

            if (IsAtEnd)
                return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(this);

            var token = _tokens[Position];
            return TokenResult.Value(token, this, new TokenList<TTokenKind>(_tokens, Position + 1));
        }

        // Usability method, not called during parsing.
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

        public override bool Equals(object obj)
        {
            if (!(obj is TokenList<TTokenKind>))
                return false;

            return Equals((TokenList<TTokenKind>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_tokens?.GetHashCode() ?? 0) * 397) ^ Position;
            }
        }

        public bool Equals(TokenList<TTokenKind> other)
        {
            return string.Equals(_tokens, other._tokens) && Position == other.Position;
        }

        public static bool operator ==(TokenList<TTokenKind> lhs, TokenList<TTokenKind> rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(TokenList<TTokenKind> lhs, TokenList<TTokenKind> rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            if (_tokens == null)
                return "Token stream (empty)";

            return "Token stream";
        }
    }
}
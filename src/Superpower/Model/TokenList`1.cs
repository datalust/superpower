using System;
using System.Linq;

namespace Superpower.Model
{
    public struct TokenList<TTokenKind> : IEquatable<TokenList<TTokenKind>>
    {
        readonly Token<TTokenKind>[] _tokens;
        readonly int _position;

        public TokenList(string source, Tokenizer<TTokenKind> tokenizer)
            : this(tokenizer.Tokenize(new StringSpan(source)).ToArray(), 0)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (tokenizer == null) throw new ArgumentNullException(nameof(tokenizer));
        }

        TokenList(Token<TTokenKind>[] tokens, int position)
        {
#if CHECKED
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));
            if (position > tokens.Length) throw new ArgumentOutOfRangeException(nameof(position), "Position is past end + 1.");
#endif

            _position = position;
            _tokens = tokens;
        }

        public static TokenList<TTokenKind> Empty { get; } = default(TokenList<TTokenKind>);

        public bool IsAtEnd
        {
            get
            {
                EnsureHasValue();
                return _position == _tokens.Length;
            }
        }

        void EnsureHasValue()
        {
            if (_tokens == null)
                throw new InvalidOperationException("Token list has no value.");
        }

        public TokenResult<TTokenKind, Token<TTokenKind>> NextToken()
        {
            EnsureHasValue();

            if (IsAtEnd)
                return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(this);

            var token = _tokens[_position];
            return TokenResult.Value(token, this, new TokenList<TTokenKind>(_tokens, _position + 1));
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
                return ((_tokens?.GetHashCode() ?? 0) * 397) ^ _position;
            }
        }

        public bool Equals(TokenList<TTokenKind> other)
        {
            return string.Equals(_tokens, other._tokens) && _position == other._position;
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
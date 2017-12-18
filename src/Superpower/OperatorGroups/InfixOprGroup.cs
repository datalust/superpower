using System;
using System.IO;
using System.Linq;
using Superpower.Parsers;

namespace Superpower.OperatorGroups
{
    /// <summary>
    /// Definition of binary infix operators
    /// </summary>
    /// <typeparam name="TKind">Type of tokens parsed</typeparam>
    /// <typeparam name="T">Type of result output by parser</typeparam>
    /// <typeparam name="U">Type of binary operators which tokens are transformed into</typeparam>
    public class InfixOprGroup<TKind, T, U>
        : IOperatorGroup<TKind, T>
    {
        private readonly InfixOprAssociativity _associativity;
        private readonly TKind[] _tokens;
        private readonly Func<TKind, U> _tokenToOperator;
        private readonly Func<U, T, T, T> _resultBuilder;

        /// <summary>
        /// Create an <see cref="InfixOprGroup{TKind,T,U}"/> representing a group of infix
        /// operators with same <see cref="Precedence"/>.
        /// </summary>
        /// <param name="precedence">Operator precedence of all operators specified</param>
        /// <param name="associativity">Specification if operators are left or right associative</param>
        /// <param name="tokenToOperator">Function for transforming token of type <typeparamref name="TKind"/>
        /// into binary operator of type <typeparamref name="U"/></param>
        /// <param name="resultBuilder">Function for creating result of type <typeparamref name="T"/>
        /// given an operator of type <typeparamref name="U"/> and two arguments of type
        /// <typeparamref name="T"/></param>
        /// <param name="tokens">Tokens representing operators in this operator group</param>
        public InfixOprGroup(
            int precedence,
            InfixOprAssociativity associativity,
            Func<TKind, U> tokenToOperator,
            Func<U, T, T, T> resultBuilder,
            params TKind[] tokens)
        {
            Precedence = precedence;
            _associativity = associativity;
            _tokens = tokens;
            _tokenToOperator = tokenToOperator;
            _resultBuilder = resultBuilder;

            // Validate that token mapping works
            foreach ( var token in tokens ) {
                try {
                    tokenToOperator(token);
                }
                catch {
                    throw new InvalidDataException($"Token {token} is not mapped correctly");
                }
            }
        }

        /// <inheritdoc />
        public int Precedence { get; }

        /// <inheritdoc />
        public TokenListParser<TKind, T> BuildParser(TokenListParser<TKind, T> operandParser)
        {
            var oprParsers = _tokens
                .Select(token => Token.EqualTo(token).Value(_tokenToOperator(token)))
                .Aggregate(Combinators.Or);

            return
                _associativity == InfixOprAssociativity.Left ?
                    // Do not use the Parse.Chain operator - it can cause stack overflow
                    Parse.Chain(oprParsers, operandParser, _resultBuilder) :
                    Parse.ChainRight(oprParsers, operandParser, _resultBuilder);
        }
    }
}

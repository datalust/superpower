using System;
using System.IO;
using System.Linq;
using Superpower.Parsers;

namespace Superpower.OperatorGroups
{
    /// <summary>
    /// Definition of group of unary prefix operators
    /// </summary>
    /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
    /// <typeparam name="T">The type of value being parsed.</typeparam>
    /// <typeparam name="U">The type of the operator.</typeparam>
    public class PrefixOprGroup<TKind, T, U> : IOperatorGroup<TKind, T>
    {
        private readonly TKind[] _tokens;
        private readonly Func<TKind, U> _tokenToOperator;
        private readonly Func<U, T, T> _resultBuilder;

        /// <summary>
        /// Create <see cref="PrefixOprGroup{TKind,T,U}"/> representing a group of prefix
        /// operators with same <see cref="Precedence"/>
        /// </summary>
        /// <param name="precedence">Operator precedence of all operators specified</param>
        /// <param name="tokenToOperator">Function for transforming token of type <typeparamref name="TKind"/>
        /// into unary operator of type <typeparamref name="U"/></param>
        /// <param name="resultBuilder">Function for creating result of type <typeparamref name="T"/>
        /// given an operator of type <typeparamref name="U"/> and one arguments of type
        /// <typeparamref name="T"/></param>
        /// <param name="tokens">Tokens representing operators in this operator group</param>
        public PrefixOprGroup(
            int precedence,
            Func<TKind, U> tokenToOperator,
            Func<U, T, T> resultBuilder,
            params TKind[] tokens)
        {
            Precedence = precedence;
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

            TokenListParser<TKind, U> operatorParser = _tokens
                .Select(token => Token.EqualTo(token).Value(_tokenToOperator(token)))
                .Aggregate((p1, p2) => p1.Or(p2));

            return operatorParser
                .Many()
                .Then(oprs => operandParser.Select(operand =>
                    oprs.Aggregate(operand, (o, opr) => _resultBuilder(opr, o))));
        }
    }
}
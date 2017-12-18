using System;
using System.Collections.Generic;
using System.IO;

namespace Superpower.OperatorPrecedenceParser
{
    /// <summary>
    /// Definition of binary infix operators
    /// </summary>
    /// <typeparam name="TKind">Type of tokens parsed</typeparam>
    /// <typeparam name="T">Type of result output by parser</typeparam>
    /// <typeparam name="U">Type of binary operators which tokens are transformed into</typeparam>
    internal class InfixOprDef<TKind, T, U> : IExprOprDef<TKind>
    {

        /// <summary>
        /// Create an <see cref="InfixOprDef{TKind,T,U}"/> representing a group of infix
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
        public InfixOprDef(
            int precedence,
            InfixOprAssociativity associativity,
            Func<TKind, U> tokenToOperator,
            Func<U, T, T, T> resultBuilder,
            params TKind[] tokens)
        {
            Precedence = precedence;
            Tokens = tokens;
            Associativity = associativity;

            TokenToOperator = tokenToOperator;
            ResultBuilder = resultBuilder;

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

        /// <summary>
        /// Method for mapping two operands into a single operand using provided operator
        /// </summary>
        public Func<U, T, T, T> ResultBuilder { get; set; }

        /// <summary>
        /// Indication if operator is left of right associative
        /// </summary>
        public InfixOprAssociativity Associativity { get; set; }

        /// <summary>
        /// Mapping from tokens in <see cref="Tokens"/> to operator of type <typeparamref name="U"/>
        /// </summary>

        public Func<TKind, U> TokenToOperator { get; set; }


        /// <inheritdoc />
        public IReadOnlyList<TKind> Tokens { get; set; }


        /// <inheritdoc />
        public int Precedence { get; }
    }
}

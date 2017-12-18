using System;
using System.Collections.Generic;
using System.IO;

namespace Superpower.OperatorPrecedenceParser
{
    /// <summary>
    /// Definition of group of unary prefix operators
    /// </summary>
    /// <typeparam name="TKind">Type of tokens parsed</typeparam>
    /// <typeparam name="T">Type of result output by parser</typeparam>
    /// <typeparam name="U">Type of binary operators which tokens are transformed into</typeparam>
    internal class PrefixOprDef<TKind, T, U> : IExprOprDef<TKind>
    {

        /// <summary>
        /// Create <see cref="PrefixOprDef{TKind,T,U}"/> representing a group of prefix
        /// operators with same <see cref="Precedence"/>
        /// </summary>
        /// <param name="precedence">Operator precedence of all operators specified</param>
        /// <param name="tokenToOperator">Function for transforming token of type <typeparamref name="TKind"/>
        /// into unary operator of type <typeparamref name="U"/></param>
        /// <param name="resultBuilder">Function for creating result of type <typeparamref name="T"/>
        /// given an operator of type <typeparamref name="U"/> and one arguments of type
        /// <typeparamref name="T"/></param>
        /// <param name="tokens">Tokens representing operators in this operator group</param>
        public PrefixOprDef(
            int precedence,
            Func<TKind, U> tokenToOperator,
            Func<U, T, T> resultBuilder,
            params TKind[] tokens)
        {
            Precedence = precedence;
            Tokens = tokens;
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
        /// Method for mapping an operand into a single operand using provided operator
        /// </summary>
        public Func<U, T, T> ResultBuilder { get; set; }

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
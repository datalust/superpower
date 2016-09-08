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

using Superpower.Model;
using Superpower.Util;
using System;
using Superpower.Display;

namespace Superpower
{
    /// <summary>
    /// General parsing helper methods.
    /// </summary>
    public static class Parse
    {
        /// <summary>
        /// Parse a sequence of operands connected by left-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        public static CharParser<T> Chain<T, TOperator>(
            CharParser<TOperator> @operator,
            CharParser<T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainOperatorRest(first, @operator, operand, apply));
        }

        static CharParser<T> ChainOperatorRest<T, TOperator>(
            T firstOperand,
            CharParser<TOperator> @operator,
            CharParser<T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return @operator.Then(opvalue =>
                operand.Then(operandValue =>
                    ChainOperatorRest(apply(opvalue, firstOperand, operandValue), @operator, operand, apply)))
                    .Or(Return(firstOperand));
        }

        /// <summary>
        /// Parse a sequence of operands connected by right-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        public static CharParser<T> ChainRight<T, TOperator>(
            CharParser<TOperator> @operator,
            CharParser<T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainRightOperatorRest(first, @operator, operand, apply));
        }

        static CharParser<T> ChainRightOperatorRest<T, TOperator>(
            T lastOperand,
            CharParser<TOperator> @operator,
            CharParser<T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return @operator.Then(opvalue =>
                operand.Then(operandValue =>
                    ChainRightOperatorRest(operandValue, @operator, operand, apply)).Then(r => Return(apply(opvalue, lastOperand, r))))
                    .Or(Return(lastOperand));
        }

        /// <summary>
        /// Parse a sequence of operands connected by left-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TTokenKind">The kind of token being parsed.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        public static TokenParser<TTokenKind, T> Chain<TTokenKind, T, TOperator>(
            TokenParser<TTokenKind, TOperator> @operator,
            TokenParser<TTokenKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainOperatorRest(first, @operator, operand, apply));
        }

        static TokenParser<TTokenKind, T> ChainOperatorRest<TTokenKind, T, TOperator>(
            T firstOperand,
            TokenParser<TTokenKind, TOperator> @operator,
            TokenParser<TTokenKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return @operator.Then(opvalue =>
                operand.Then(operandValue =>
                    ChainOperatorRest(apply(opvalue, firstOperand, operandValue), @operator, operand, apply)))
                    .Or(Return<TTokenKind, T>(firstOperand));
        }

        /// <summary>
        /// Parse a sequence of operands connected by right-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TTokenKind">The kind of token being parsed.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        public static TokenParser<TTokenKind, T> ChainRight<TTokenKind, T, TOperator>(
            TokenParser<TTokenKind, TOperator> @operator,
            TokenParser<TTokenKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainRightOperatorRest(first, @operator, operand, apply));
        }

        static TokenParser<TTokenKind, T> ChainRightOperatorRest<TTokenKind, T, TOperator>(
            T lastOperand,
            TokenParser<TTokenKind, TOperator> @operator,
            TokenParser<TTokenKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return @operator.Then(opvalue =>
                operand.Then(operandValue =>
                    ChainRightOperatorRest(operandValue, @operator, operand, apply)).Then(r => Return<TTokenKind, T>(apply(opvalue, lastOperand, r))))
                    .Or(Return<TTokenKind, T>(lastOperand));
        }
        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in a regular expression.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the negation of the given parser.</returns>
        public static CharParser<Unit> Not<T>(CharParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);

                if (result.HasValue)
                    return CharResult.Empty<Unit>(input, $"unexpected successful parsing of `{input.Until(result.Remainder)}`");

                return CharResult.Value(Unit.Value, input, input);
            };
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in a regular expression.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <typeparam name="TTokenKind">The kind of token being parsed.</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the negation of the given parser.</returns>
        public static TokenParser<TTokenKind, Unit> Not<TTokenKind, T>(TokenParser<TTokenKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);

                if (result.HasValue)
                {
                    // This is usually a success case for Not(), so the allocations here are a bit of a pity.

                    var current = input.ConsumeToken();
                    var last = result.Remainder.ConsumeToken();
                    if (current.HasValue)
                    {
                        var span = last.HasValue ?
                            current.Value.Span.Source.Substring(current.Value.Position.Absolute, last.Value.Position.Absolute - current.Value.Position.Absolute) :
                            current.Value.Span.Source.Substring(current.Value.Position.Absolute);
                        return TokenResult.Empty<TTokenKind, Unit>(input, $"unexpected successful parsing of {Presentation.FormatLiteral(Friendly.Clip(span, 12))}");
                    }

                    return TokenResult.Empty<TTokenKind, Unit>(input, "unexpected successful parsing");
                }

                return TokenResult.Value(Unit.Value, input, input);
            };
        }

        /// <summary>
        /// Lazily construct a parser, so that circular dependencies are possible.
        /// </summary>
        /// <param name="reference">A function creating the parser, when required.</param>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <returns>A parser that lazily evaluates <paramref name="reference"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is null.</exception>
        public static CharParser<T> Ref<T>(Func<CharParser<T>> reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            CharParser<T> parser = null;

            return i =>
            {
                if (parser == null)
                    parser = reference();

                return parser(i);
            };
        }

        /// <summary>
        /// Lazily construct a parser, so that circular dependencies are possible.
        /// </summary>
        /// <param name="reference">A function creating the parser, when required.</param>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="TTokenKind">The kind of token being parsed.</typeparam>
        /// <returns>A parser that lazily evaluates <paramref name="reference"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is null.</exception>
        public static TokenParser<TTokenKind, T> Ref<TTokenKind, T>(Func<TokenParser<TTokenKind, T>> reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            TokenParser<TTokenKind, T> parser = null;

            return i =>
            {
                if (parser == null)
                    parser = reference();

                return parser(i);
            };
        }

        /// <summary>
        /// Construct a parser with a fixed value.
        /// </summary>
        /// <param name="value">The value returned by the parser.</param>
        /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
        /// <returns>The parser.</returns>
        public static CharParser<T> Return<T>(T value)
        {
            return input => CharResult.Value(value, input, input);
        }

        /// <summary>
        /// Construct a parser with a fixed value.
        /// </summary>
        /// <param name="value">The value returned by the parser.</param>
        /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
        /// <typeparam name="TTokenKind">The kind of token being parsed.</typeparam>
        /// <returns>The parser.</returns>
        public static TokenParser<TTokenKind, T> Return<TTokenKind, T>(T value)
        {
            return input => TokenResult.Value(value, input, input);
        }
    }
}

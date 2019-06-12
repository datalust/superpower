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
        /// Parse a sequence of similar operands connected by left-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        /// <seealso cref="Combinators.Chain{TResult,TOperator,TOperand}"/>
        public static TextParser<T> Chain<T, TOperator>(
            TextParser<TOperator> @operator,
            TextParser<T> operand,
            Func<TOperator, T, T, T> apply)
        {
            return operand.Chain(@operator, operand, apply);
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
        public static TextParser<T> ChainRight<T, TOperator>(
            TextParser<TOperator> @operator,
            TextParser<T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainRightOperatorRest(first, @operator, operand, apply));
        }

        static TextParser<T> ChainRightOperatorRest<T, TOperator>(
            T lastOperand,
            TextParser<TOperator> @operator,
            TextParser<T> operand,
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
        /// Parse a sequence of similar operands connected by left-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        /// <seealso cref="Combinators.Chain{TKind, TResult,TOperator,TOperand}"/>
        public static TokenListParser<TKind, T> Chain<TKind, T, TOperator>(
            TokenListParser<TKind, TOperator> @operator,
            TokenListParser<TKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            return operand.Chain(@operator, operand, apply);
        }

        /// <summary>
        /// Parse a sequence of operands connected by right-associative operators.
        /// </summary>
        /// <typeparam name="T">The type being parsed.</typeparam>
        /// <typeparam name="TOperator">The type of the operator.</typeparam>
        /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
        /// <param name="operator">A parser matching operators.</param>
        /// <param name="operand">A parser matching operands.</param>
        /// <param name="apply">A function combining an operator and two operands into the result.</param>
        /// <returns>The result of calling <paramref name="apply"/> successively on pairs of operands.</returns>
        public static TokenListParser<TKind, T> ChainRight<TKind, T, TOperator>(
            TokenListParser<TKind, TOperator> @operator,
            TokenListParser<TKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return operand.Then(first => ChainRightOperatorRest(first, @operator, operand, apply));
        }

        static TokenListParser<TKind, T> ChainRightOperatorRest<TKind, T, TOperator>(
            T lastOperand,
            TokenListParser<TKind, TOperator> @operator,
            TokenListParser<TKind, T> operand,
            Func<TOperator, T, T, T> apply)
        {
            if (@operator == null) throw new ArgumentNullException(nameof(@operator));
            if (operand == null) throw new ArgumentNullException(nameof(operand));
            if (apply == null) throw new ArgumentNullException(nameof(apply));
            return @operator.Then(opvalue =>
                operand.Then(operandValue =>
                    ChainRightOperatorRest(operandValue, @operator, operand, apply)).Then(r => Return<TKind, T>(apply(opvalue, lastOperand, r))))
                    .Or(Return<TKind, T>(lastOperand));
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in a regular expression.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the negation of the given parser.</returns>
        public static TextParser<Unit> Not<T>(TextParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);

                if (result.HasValue)
                    return Result.Empty<Unit>(input, $"unexpected successful parsing of `{input.Until(result.Remainder)}`");

                return Result.Value(Unit.Value, input, input);
            };
        }

        /// <summary>
        /// Constructs a parser that will fail if the given parser succeeds,
        /// and will succeed if the given parser fails. In any case, it won't
        /// consume any input. It's like a negative look-ahead in a regular expression.
        /// </summary>
        /// <typeparam name="T">The result type of the given parser.</typeparam>
        /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
        /// <param name="parser">The parser to wrap</param>
        /// <returns>A parser that is the negation of the given parser.</returns>
        public static TokenListParser<TKind, Unit> Not<TKind, T>(TokenListParser<TKind, T> parser)
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
                        return TokenListParserResult.Empty<TKind, Unit>(input, $"unexpected successful parsing of {Presentation.FormatLiteral(Friendly.Clip(span, 12))}");
                    }

                    return TokenListParserResult.Empty<TKind, Unit>(input, "unexpected successful parsing");
                }

                return TokenListParserResult.Value(Unit.Value, input, input);
            };
        }

        /// <summary>
        /// Lazily construct a parser, so that circular dependencies are possible.
        /// </summary>
        /// <param name="reference">A function creating the parser, when required.</param>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <returns>A parser that lazily evaluates <paramref name="reference"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is null.</exception>
        public static TextParser<T> Ref<T>(Func<TextParser<T>> reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            TextParser<T> parser = null;

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
        /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
        /// <returns>A parser that lazily evaluates <paramref name="reference"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="reference"/> is null.</exception>
        public static TokenListParser<TKind, T> Ref<TKind, T>(Func<TokenListParser<TKind, T>> reference)
        {
            if (reference == null) throw new ArgumentNullException(nameof(reference));

            TokenListParser<TKind, T> parser = null;

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
        public static TextParser<T> Return<T>(T value)
        {
            return input => Result.Value(value, input, input);
        }

        /// <summary>
        /// Construct a parser with a fixed value.
        /// </summary>
        /// <param name="value">The value returned by the parser.</param>
        /// <typeparam name="T">The type of <paramref name="value"/>.</typeparam>
        /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
        /// <returns>The parser.</returns>
        public static TokenListParser<TKind, T> Return<TKind, T>(T value)
        {
            return input => TokenListParserResult.Value(value, input, input);
        }

        /// <summary>
        /// Construct a parser applies two parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, (T, U)> Sequence<TKind, T, U>(
            TokenListParser<TKind, T> parser1,
            TokenListParser<TKind, U> parser2)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, T, (T, U)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, U, (T, U)>(ru);

                return TokenListParserResult.Value((rt.Value, ru.Value), input, ru.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser applies three parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <typeparam name="V">The type of the third value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <param name="parser3">The third parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, (T, U, V)> Sequence<TKind, T, U, V>(
            TokenListParser<TKind, T> parser1,
            TokenListParser<TKind, U> parser2,
            TokenListParser<TKind, V> parser3)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));
            if (parser3 == null) throw new ArgumentNullException(nameof(parser3));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, T, (T, U, V)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, U, (T, U, V)>(ru);

                var rv = parser3(ru.Remainder);
                if (!rv.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, V, (T, U, V)>(rv);

                return TokenListParserResult.Value((rt.Value, ru.Value, rv.Value), input, rv.Remainder);
            };
        }
                
        /// <summary>
        /// Construct a parser applies four parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <typeparam name="V">The type of the third value parsed.</typeparam>
        /// <typeparam name="W">The type of the fourth value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <param name="parser3">The third parser to apply.</param>
        /// <param name="parser4">The fourth parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, (T, U, V, W)> Sequence<TKind, T, U, V, W>(
            TokenListParser<TKind, T> parser1,
            TokenListParser<TKind, U> parser2,
            TokenListParser<TKind, V> parser3,
            TokenListParser<TKind, W> parser4)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));
            if (parser3 == null) throw new ArgumentNullException(nameof(parser3));
            if (parser4 == null) throw new ArgumentNullException(nameof(parser4));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, T, (T, U, V, W)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, U, (T, U, V, W)>(ru);

                var rv = parser3(ru.Remainder);
                if (!rv.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, V, (T, U, V, W)>(rv);

                var rw = parser4(rv.Remainder);
                if (!rw.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, W, (T, U, V, W)>(rw);

                return TokenListParserResult.Value((rt.Value, ru.Value, rv.Value, rw.Value), input, rw.Remainder);
            };
        }
        
        /// <summary>
        /// Construct a parser applies five parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <typeparam name="V">The type of the third value parsed.</typeparam>
        /// <typeparam name="W">The type of the fourth value parsed.</typeparam>
        /// <typeparam name="X">The type of the fifth value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <param name="parser3">The third parser to apply.</param>
        /// <param name="parser4">The fourth parser to apply.</param>
        /// <param name="parser5">The fifth parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, (T, U, V, W, X)> Sequence<TKind, T, U, V, W, X>(
            TokenListParser<TKind, T> parser1,
            TokenListParser<TKind, U> parser2,
            TokenListParser<TKind, V> parser3,
            TokenListParser<TKind, W> parser4,
            TokenListParser<TKind, X> parser5)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));
            if (parser3 == null) throw new ArgumentNullException(nameof(parser3));
            if (parser4 == null) throw new ArgumentNullException(nameof(parser4));
            if (parser5 == null) throw new ArgumentNullException(nameof(parser5));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, T, (T, U, V, W, X)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, U, (T, U, V, W, X)>(ru);

                var rv = parser3(ru.Remainder);
                if (!rv.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, V, (T, U, V, W, X)>(rv);

                var rw = parser4(rv.Remainder);
                if (!rw.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, W, (T, U, V, W, X)>(rw);

                var rx = parser5(rw.Remainder);
                if (!rx.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, X, (T, U, V, W, X)>(rx);

                return TokenListParserResult.Value((rt.Value, ru.Value, rv.Value, rw.Value, rx.Value), input, rx.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser applies two parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<(T, U)> Sequence<T, U>(
            TextParser<T> parser1,
            TextParser<U> parser2)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return Result.CastEmpty<T, (T, U)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return Result.CastEmpty<U, (T, U)>(ru);

                return Result.Value((rt.Value, ru.Value), input, ru.Remainder);
            };
        }
        
        /// <summary>
        /// Construct a parser applies three parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <typeparam name="V">The type of the third value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <param name="parser3">The third parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<(T, U, V)> Sequence<T, U, V>(
            TextParser<T> parser1,
            TextParser<U> parser2,
            TextParser<V> parser3)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));
            if (parser3 == null) throw new ArgumentNullException(nameof(parser3));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return Result.CastEmpty<T, (T, U, V)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return Result.CastEmpty<U, (T, U, V)>(ru);

                var rv = parser3(ru.Remainder);
                if (!rv.HasValue)
                    return Result.CastEmpty<V, (T, U, V)>(rv);

                return Result.Value((rt.Value, ru.Value, rv.Value), input, rv.Remainder);
            };
        }
        
        /// <summary>
        /// Construct a parser applies four parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <typeparam name="V">The type of the third value parsed.</typeparam>
        /// <typeparam name="W">The type of the fourth value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <param name="parser3">The third parser to apply.</param>
        /// <param name="parser4">The fourth parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<(T, U, V, W)> Sequence<T, U, V, W>(
            TextParser<T> parser1,
            TextParser<U> parser2,
            TextParser<V> parser3,
            TextParser<W> parser4)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));
            if (parser3 == null) throw new ArgumentNullException(nameof(parser3));
            if (parser4 == null) throw new ArgumentNullException(nameof(parser4));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return Result.CastEmpty<T, (T, U, V, W)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return Result.CastEmpty<U, (T, U, V, W)>(ru);

                var rv = parser3(ru.Remainder);
                if (!rv.HasValue)
                    return Result.CastEmpty<V, (T, U, V, W)>(rv);

                var rw = parser4(rv.Remainder);
                if (!rw.HasValue)
                    return Result.CastEmpty<W, (T, U, V, W)>(rw);

                return Result.Value((rt.Value, ru.Value, rv.Value, rw.Value), input, rw.Remainder);
            };
        }
        
        /// <summary>
        /// Construct a parser applies five parsers in order and returns a tuple of their results.
        /// </summary>
        /// <typeparam name="T">The type of the first value parsed.</typeparam>
        /// <typeparam name="U">The type of the second value parsed.</typeparam>
        /// <typeparam name="V">The type of the third value parsed.</typeparam>
        /// <typeparam name="W">The type of the fourth value parsed.</typeparam>
        /// <typeparam name="X">The type of the fifth value parsed.</typeparam>
        /// <param name="parser1">The first parser to apply.</param>
        /// <param name="parser2">The second parser to apply.</param>
        /// <param name="parser3">The third parser to apply.</param>
        /// <param name="parser4">The fourth parser to apply.</param>
        /// <param name="parser5">The fifth parser to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<(T, U, V, W, X)> Sequence<T, U, V, W, X>(
            TextParser<T> parser1,
            TextParser<U> parser2,
            TextParser<V> parser3,
            TextParser<W> parser4,
            TextParser<X> parser5)
        {
            if (parser1 == null) throw new ArgumentNullException(nameof(parser1));
            if (parser2 == null) throw new ArgumentNullException(nameof(parser2));
            if (parser3 == null) throw new ArgumentNullException(nameof(parser3));
            if (parser4 == null) throw new ArgumentNullException(nameof(parser4));
            if (parser5 == null) throw new ArgumentNullException(nameof(parser5));

            return input =>
            {
                var rt = parser1(input);
                if (!rt.HasValue)
                    return Result.CastEmpty<T, (T, U, V, W, X)>(rt);

                var ru = parser2(rt.Remainder);
                if (!ru.HasValue)
                    return Result.CastEmpty<U, (T, U, V, W, X)>(ru);

                var rv = parser3(ru.Remainder);
                if (!rv.HasValue)
                    return Result.CastEmpty<V, (T, U, V, W, X)>(rv);

                var rw = parser4(rv.Remainder);
                if (!rw.HasValue)
                    return Result.CastEmpty<W, (T, U, V, W, X)>(rw);

                var rx = parser5(rw.Remainder);
                if (!rx.HasValue)
                    return Result.CastEmpty<X, (T, U, V, W, X)>(rx);

                return Result.Value((rt.Value, ru.Value, rv.Value, rw.Value, rx.Value), input, rx.Remainder);
            };
        }
    }
}

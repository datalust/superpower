using Superpower.Model;
using Superpower.Util;
using System;
using Superpower.Display;

namespace Superpower
{
    public static class Parse
    {
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

        public static CharParser<T> Return<T>(T t)
        {
            return input => CharResult.Value(t, input, input);
        }

        public static TokenParser<TTokenKind, T> Return<TTokenKind, T>(T t)
        {
            return input => TokenResult.Value(t, input, input);
        }
    }
}

using Superpower.Model;
using Superpower.Util;
using System;

namespace Superpower
{
    public static class Parse
    {
        public static CharParser<char> Char(char c)
        {
            var expectations = new[] { "`" + c + "`" };

            return input =>
            {
                var next = input.ConsumeChar();
                if (!next.HasValue || next.Value != c)
                    return CharResult.Empty<char>(input, expectations);

                return next;
            };
        }

        public static TokenParser<TTokenKind, Token<TTokenKind>> Token<TTokenKind>(TTokenKind token)
        {
            var expectations = new[] { Presentation.FormatKind(token) };

            return input =>
            {
                var next = input.ConsumeToken();
                if (!next.HasValue || !next.Value.Kind.Equals(token))
                    return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(input, expectations);

                return next;
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
    }
}

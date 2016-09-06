using Superpower.Parsers;
using System;
using System.Linq.Expressions;

namespace Superpower.Tests.ArithmeticExpressionScenario
{
    class ArithmeticExpressionParser
    {
        static TokenParser<ArithmeticExpressionToken, ExpressionType> Operator(ArithmeticExpressionToken op, ExpressionType opType)
        {
            return Token.EqualTo(op).Value(opType);
        }

        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Add = Operator(ArithmeticExpressionToken.Plus, ExpressionType.AddChecked);
        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Subtract = Operator(ArithmeticExpressionToken.Minus, ExpressionType.SubtractChecked);
        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Multiply = Operator(ArithmeticExpressionToken.Times, ExpressionType.MultiplyChecked);
        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Divide = Operator(ArithmeticExpressionToken.Divide, ExpressionType.Divide);

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Constant =
             Token.EqualTo(ArithmeticExpressionToken.Number)
                .Apply(Numerics.IntegerInt32)
                .Select(n => (Expression)Expression.Constant(n));

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Factor =
            (from lparen in Token.EqualTo(ArithmeticExpressionToken.LParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Token.EqualTo(ArithmeticExpressionToken.RParen)
             select expr)
            .Or(Constant);

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Operand =
            (from sign in Token.EqualTo(ArithmeticExpressionToken.Minus)
             from factor in Factor
             select (Expression)Expression.Negate(factor))
            .Or(Factor).Named("expression");

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Term = Parse.Chain(Multiply.Or(Divide), Operand, Expression.MakeBinary);

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Expr = Parse.Chain(Add.Or(Subtract), Term, Expression.MakeBinary);

        public static readonly TokenParser<ArithmeticExpressionToken, Expression<Func<int>>> Lambda =
            Expr
                .AtEnd()
                .Select(body => Expression.Lambda<Func<int>>(body));
    }
}

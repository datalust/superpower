using Superpower.Parsers;
using System;
using System.Linq.Expressions;

namespace Superpower.Tests.ArithmeticExpressionScenario
{
    class ArithmeticExpressionParser
    {
        static TokenParser<ArithmeticExpressionToken, ExpressionType> Operator(ArithmeticExpressionToken op, ExpressionType opType)
        {
            return Parse.Token(op).Value(opType);
        }

        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Add = Operator(ArithmeticExpressionToken.Plus, ExpressionType.AddChecked);
        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Subtract = Operator(ArithmeticExpressionToken.Minus, ExpressionType.SubtractChecked);
        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Multiply = Operator(ArithmeticExpressionToken.Times, ExpressionType.MultiplyChecked);
        static readonly TokenParser<ArithmeticExpressionToken, ExpressionType> Divide = Operator(ArithmeticExpressionToken.Divide, ExpressionType.Divide);

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Constant =
             Parse.Token(ArithmeticExpressionToken.Number)
                .Apply(Numerics.IntegerInt32)
                .Select(n => (Expression)Expression.Constant(n));

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Factor =
            (from lparen in Parse.Token(ArithmeticExpressionToken.LParen)
             from expr in Parse.Ref(() => Expr)
             from rparen in Parse.Token(ArithmeticExpressionToken.RParen)
             select expr)
            .Or(Constant);

        static readonly TokenParser<ArithmeticExpressionToken, Expression> Operand =
            (from sign in Parse.Token(ArithmeticExpressionToken.Minus)
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

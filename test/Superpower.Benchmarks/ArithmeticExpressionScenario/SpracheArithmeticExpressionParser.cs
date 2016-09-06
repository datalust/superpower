using System;
using System.Linq;
using Sprache;
using System.Linq.Expressions;

namespace Superpower.Benchmarks.ArithmeticExpressionScenario
{
    static class SpracheArithmeticExpressionParser
    {
        static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        {
            return Sprache.Parse.String(op).Token().Return(opType);
        }

        static readonly Parser<ExpressionType> Add = Operator("+", ExpressionType.AddChecked);
        static readonly Parser<ExpressionType> Subtract = Operator("-", ExpressionType.SubtractChecked);
        static readonly Parser<ExpressionType> Multiply = Operator("*", ExpressionType.MultiplyChecked);
        static readonly Parser<ExpressionType> Divide = Operator("/", ExpressionType.Divide);

        static readonly Parser<Expression> Constant =
             Sprache.Parse.Decimal
             .Select(x => Expression.Constant(int.Parse(x)))
             .Named("number");

        static readonly Parser<Expression> Factor =
            (from lparen in Sprache.Parse.Char('(')
             from expr in Sprache.Parse.Ref(() => Expr)
             from rparen in Sprache.Parse.Char(')')
             select expr)
             .XOr(Constant);

        static readonly Parser<Expression> Operand =
            ((from sign in Sprache.Parse.Char('-')
              from factor in Factor
              select Expression.Negate(factor)
             ).XOr(Factor)).Named("expression").Token();

        static readonly Parser<Expression> Term = Sprache.Parse.XChainOperator(Multiply.XOr(Divide), Operand, Expression.MakeBinary);

        static readonly Parser<Expression> Expr = Sprache.Parse.XChainOperator(Add.XOr(Subtract), Term, Expression.MakeBinary);

        public static readonly Parser<Expression<Func<int>>> Lambda =
            Expr.End().Select(body => Expression.Lambda<Func<int>>(body));
    }
}

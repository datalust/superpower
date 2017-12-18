using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Superpower.OperatorGroups;
using Superpower.Parsers;

namespace Superpower.Tests.BoolExpressionScenario
{
    class BoolExpressionParser
    {
        /// <summary>
        /// Mapping from token representing operator to expression operator
        /// </summary>
        private static readonly Dictionary<BoolExpressionToken, ExpressionType> TokenToOprMap =
            new Dictionary<BoolExpressionToken, ExpressionType> {
                { BoolExpressionToken.Plus, ExpressionType.AddChecked },
                { BoolExpressionToken.Minus, ExpressionType.SubtractChecked },
                { BoolExpressionToken.Times, ExpressionType.Multiply },
                { BoolExpressionToken.Divide, ExpressionType.Divide },
                { BoolExpressionToken.Lt, ExpressionType.LessThan },
                { BoolExpressionToken.Gt, ExpressionType.GreaterThan },
                { BoolExpressionToken.And, ExpressionType.And },
                { BoolExpressionToken.Or, ExpressionType.Or },
                { BoolExpressionToken.Not, ExpressionType.Not }
            };

        /// <summary>
        /// Define a simple integer constant
        /// </summary>
        static readonly TokenListParser<BoolExpressionToken, Expression> Constant =
            Token.EqualTo(BoolExpressionToken.Number)
                .Apply(Numerics.IntegerInt32)
                .Select(n => (Expression) Expression.Constant(n));

        /// <summary>
        /// The variable naming convention in the example is, that variables starting with 'b'
        /// are Boolean variables, all other variables are integer variables (should start with 'i')
        /// </summary>
        private static readonly TokenListParser<BoolExpressionToken, Expression> Variable =
            Token.EqualTo(BoolExpressionToken.Variable)
                .Select(v => (Expression) Expression.Variable(
                    v.ToStringValue()[0] == 'b' ? typeof( bool ) : typeof( Int32 ),
                    v.ToStringValue()));

        static readonly TokenListParser<BoolExpressionToken, Expression> Factor =
            ( from lparen in Token.EqualTo(BoolExpressionToken.LParen)
                from expr in Parse.Ref(() => Expr)
                from rparen in Token.EqualTo(BoolExpressionToken.RParen)
                select expr )
            .Or(Constant).Or(Variable);

        private static readonly TokenListParser<BoolExpressionToken, Expression> Expr =
            new List<IOperatorGroup<BoolExpressionToken, Expression>> {
                new InfixOprGroup<BoolExpressionToken, Expression, ExpressionType>(
                    1, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                    BoolExpressionToken.Or),
                new InfixOprGroup<BoolExpressionToken, Expression, ExpressionType>(
                    2, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                    BoolExpressionToken.And),
                new InfixOprGroup<BoolExpressionToken, Expression, ExpressionType>(
                    3, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                    BoolExpressionToken.Lt, BoolExpressionToken.Gt),
                new InfixOprGroup<BoolExpressionToken, Expression, ExpressionType>(
                    4, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                    BoolExpressionToken.Plus, BoolExpressionToken.Minus),
                new InfixOprGroup<BoolExpressionToken, Expression, ExpressionType>(
                    5, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                    BoolExpressionToken.Times, BoolExpressionToken.Divide),
                new PrefixOprGroup<BoolExpressionToken,Expression,ExpressionType>(
                    6, t => TokenToOprMap[t], (oprType, expr)=>Expression.MakeUnary(oprType,expr,null),
                    BoolExpressionToken.Not)
            }.DefineParser(Factor);

        public static readonly TokenListParser<BoolExpressionToken, Expression<Func<bool>>> Lambda =
            Expr
                .AtEnd()
                .Select(body => Expression.Lambda<Func<bool>>(body));
    }
}

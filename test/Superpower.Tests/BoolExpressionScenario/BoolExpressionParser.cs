using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Superpower.OperatorPrecedenceParser;
using Superpower.Parsers;
using Xunit.Sdk;

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
                { BoolExpressionToken.Not, ExpressionType.Not },
                { BoolExpressionToken.In, (ExpressionType)(-1) }
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

        static readonly TokenListParser<BoolExpressionToken, Expression> ExprOperand =
            Constant.Or(Variable);

        /// <summary>
        /// Method use to create a new list of expression
        /// </summary>
        private static readonly Func<IEnumerable<Expression>, IEnumerable<BoolExpressionToken>, Expression> _setBuilder
            =
            (args, seps) =>
                Expression.ListInit(
                    Expression.New(typeof( List<int> )),
                    args.Select(e => Expression.ElementInit(typeof( List<int> ).GetMethod("Add"), e)));

        /// <summary>
        /// Representation of IN. "x in {1,2}" becomes List&lt;object&gt;(){1,2}.Contains(x)
        /// </summary>
        private static readonly Expression<Func<IEnumerable<int>, int, bool>> _inFunc =
            (set, elem) => set.Contains(elem);

        private static readonly Func<ExpressionType, Expression, Expression, Expression> x =
            (opr, elem, set) => Expression.Invoke(_inFunc, set, elem);

        private static readonly IExprDef<BoolExpressionToken>[] ExprDefs = {
            // OR
            new InfixOprDef<BoolExpressionToken, Expression, ExpressionType>(
                1, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                BoolExpressionToken.Or),
            // AND
            new InfixOprDef<BoolExpressionToken, Expression, ExpressionType>(
                2, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                BoolExpressionToken.And),
            // LT, GT
            new InfixOprDef<BoolExpressionToken, Expression, ExpressionType>(
                3, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                BoolExpressionToken.Lt, BoolExpressionToken.Gt),
            // PLUS, MINUS
            new InfixOprDef<BoolExpressionToken, Expression, ExpressionType>(
                4, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                BoolExpressionToken.Plus, BoolExpressionToken.Minus),
            // TIMES, DIVIDE
            new InfixOprDef<BoolExpressionToken, Expression, ExpressionType>(
                5, InfixOprAssociativity.Left, t => TokenToOprMap[t], Expression.MakeBinary,
                BoolExpressionToken.Times, BoolExpressionToken.Divide),
            // UNARY NOT, UNARY MINUS
            new PrefixOprDef<BoolExpressionToken,Expression,ExpressionType>(
                6, t => TokenToOprMap[t], (oprType, expr)=>Expression.MakeUnary(oprType,expr,null),
                BoolExpressionToken.Not, BoolExpressionToken.Minus),
            // IN
            new InfixOprDef<BoolExpressionToken, Expression, ExpressionType>(
                7, InfixOprAssociativity.Left, t => TokenToOprMap[t], x,
                BoolExpressionToken.In),
            // Parenthesis ( EXPR )
            new ExprSetDef<BoolExpressionToken,Expression>(
                BoolExpressionToken.LParen, BoolExpressionToken.RParen ),
            // Set (lists) of expressions { e1, e2, e3 }
            new ExprSetDef<BoolExpressionToken,Expression>(
                BoolExpressionToken.LCrl, BoolExpressionToken.RCrl, new [] {BoolExpressionToken.Comma},
                true, _setBuilder ),
            // IF-x-THEN-y-ELSE-z ==  x ? y : z
            // IF-x-THEN-y        == x -> y == not x or y (imply operator)
            new IfThenElseDef<BoolExpressionToken,Expression>(
                BoolExpressionToken.If, BoolExpressionToken.Then, BoolExpressionToken.Else,
                Expression.Condition, (c,t) => Expression.Or(Expression.Not(c), t) )
        };

        public static readonly TokenListParser<BoolExpressionToken, Expression> Expr =
            OperatorPrecedenceParser<BoolExpressionToken, Expression, ExpressionType>.DefineParser(
                ExprDefs, ExprOperand).AtEnd();
    }
}

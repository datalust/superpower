using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.BoolExpressionScenario
{
    public class BoolExpressionScenarioTests
    {
        [Theory]
        [InlineData("1 + 2 * 2 > 0", "((1 + (2 * 2)) > 0)")]
        [InlineData("3 * 2 / 1 < 4 / 3 + 1", "(((3 * 2) / 1) < ((4 / 3) + 1))")]
        [InlineData("3 * i1 / i2 > i3", "(((3 * i1) / i2) > i3)")]
        [InlineData("i1 + i2 + i3 + i4 + i5 > 0 ", "(((((i1 + i2) + i3) + i4) + i5) > 0)")]
        [InlineData("b1 & !b2 & b3", "((b1 And Not(b2)) And b3)")]
        [InlineData( "!b1 | !b2 & !b3", "(Not(b1) Or (Not(b2) And Not(b3)))" )]
        [InlineData( "!(b1 | !b2)", "Not((b1 Or Not(b2)))" )]
        [InlineData( "!!b1", "Not(Not(b1))" )]
        public void EnsureExpectedOperatorPrecedence(string exprString, string expected)
        {
            var tokenizer = new BoolExpressionTokenizer();
            var expression = BoolExpressionParser.Expr(tokenizer.Tokenize(exprString));
            Assert.True(expression.HasValue);
            Assert.Equal(expected, expression.Value.ToString());
        }

        [Theory]
        [InlineData( "if i1 + i2 > i3 then b1 else b2", "IIF(((i1 + i2) > i3), b1, b2)" )]
        [InlineData( "if if b1 then b2 else b3 then b4 else b5", "IIF(IIF(b1, b2, b3), b4, b5)" )]
        [InlineData( "if if b1 then b2 else b3 then b4", "(Not(IIF(b1, b2, b3)) Or b4)" )]
        [InlineData( "if b1 then b2", "(Not(b1) Or b2)" )]
        public void IfThenElseConstructs( string exprString, string expected )
        {
            var tokenizer = new BoolExpressionTokenizer();
            var expression = BoolExpressionParser.Expr( tokenizer.Tokenize( exprString ) );
            Assert.True( expression.HasValue );
            Assert.Equal( expected, expression.Value.ToString() );
        }

        [Theory]
        [InlineData("if b1", "Syntax error: unexpected end of input, expected `then`." )]
        [InlineData( "if b1 then then", "Syntax error (line 1, column 12): unexpected keyword `then`, expected number or variable." )]
        [InlineData( "if b1 then else", "Syntax error (line 1, column 12): unexpected keyword `else`, expected number or variable." )]
        [InlineData( "b1 then b2", "Syntax error (line 1, column 4): unexpected keyword `then`." )]
        public void IfThenElseConstructsErrors( string exprString, string expectedMessage)
        {
            var tokenizer = new BoolExpressionTokenizer();
            var result = BoolExpressionParser.Expr.TryParse( tokenizer.Tokenize( exprString ) );
            var resultMsg = result.ToString();

            Assert.False( result.HasValue);
            Assert.Equal( expectedMessage, resultMsg );
        }

        [Theory]
        [InlineData( "i1 in {1,2}", "Invoke((set, elem) => set.Contains(elem), new List`1() {Void Add(Int32)(1), Void Add(Int32)(2)}, i1)" )]
        public void Collections( string exprString, string expected ) {

            Func<IEnumerable<Expression>,IEnumerable<BoolExpressionToken>,Expression> resultBuilder = (args, seps) =>
                Expression.ListInit(
                    Expression.New(typeof( List<Expression> )),
                    args );

            var tokenizer = new BoolExpressionTokenizer();
            var expression = BoolExpressionParser.Expr( tokenizer.Tokenize( exprString ) );
            Assert.True( expression.HasValue );
            Assert.Equal( expected, expression.Value.ToString() );
        }
    }
}

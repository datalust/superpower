using Xunit;

namespace Superpower.Tests.BoolExpressionScenario
{
    public class BoolExpressionScenarioTests
    {
        [Theory]
        [InlineData("1 + 2 * 2 > 0", "() => ((1 + (2 * 2)) > 0)")]
        [InlineData("3 * 2 / 1 < 4 / 3 + 1", "() => (((3 * 2) / 1) < ((4 / 3) + 1))")]
        [InlineData("3 * i1 / i2 > i3", "() => (((3 * i1) / i2) > i3)")]
        [InlineData("i1 + i2 + i3 + i4 + i5 > 0 ", "() => (((((i1 + i2) + i3) + i4) + i5) > 0)")]
        [InlineData("b1 & !b2 & b3", "() => ((b1 And Not(b2)) And b3)")]
        [InlineData( "!b1 | !b2 & !b3", "() => (Not(b1) Or (Not(b2) And Not(b3)))" )]
        [InlineData( "!(b1 | !b2)", "() => Not((b1 Or Not(b2)))" )]
        [InlineData( "!!b1", "() => Not(Not(b1))" )]
        public void EnsureExpectedOperatorPrecedence(string exprString, string expected)
        {
            var tokenizer = new BoolExpressionTokenizer();
            var expression = BoolExpressionParser.Lambda(tokenizer.Tokenize(exprString));
            Assert.True(expression.HasValue);
            Assert.Equal(expected, expression.Value.ToString());
        }
    }
}

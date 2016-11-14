using Superpower.Display;
using Superpower.Model;

namespace Superpower.Tests.ArithmeticExpressionScenario
{
    public enum ArithmeticExpressionToken
    {
        None,

        Number,

        [Token(Category = "operator", Example = "+")]
        Plus,

        [Token(Category = "operator", Example = "-")]
        Minus,

        [Token(Category = "operator", Example = "*")]
        Times,

        [Token(Category = "operator", Example = "-")]
        Divide,

        [Token(Example = "(")]
        LParen,

        [Token(Example = ")")]
        RParen,

        [Token(Category = "keyword", Example = "zero")]
        Zero,

        [Token(Category = "keyword", Description = "literal one")]
        One,

        [Token(Description = "literal two")]
        Two
    }
}

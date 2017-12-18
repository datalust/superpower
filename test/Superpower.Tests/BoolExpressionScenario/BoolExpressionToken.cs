using Superpower.Display;

namespace Superpower.Tests.BoolExpressionScenario
{
    public enum BoolExpressionToken
    {
        None,

        Number,

        Variable,

        [Token(Category = "operator", Example = "+")]
        Plus,

        [Token(Category = "operator", Example = "-")]
        Minus,

        [Token(Category = "operator", Example = "*")]
        Times,

        [Token(Category = "operator", Example = "-")]
        Divide,

        [Token( Category = "operator", Example = "<" )]
        Lt,

        [Token( Category = "operator", Example = ">" )]
        Gt,

        [Token( Category = "operator", Example = "&" )]
        And,

        [Token( Category = "operator", Example = "|" )]
        Or,

        [Token( Category = "operator", Example = "!" )]
        Not,

        [Token(Example = "(")]
        LParen,

        [Token(Example = ")")]
        RParen
    }
}

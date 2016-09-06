using System.ComponentModel;

namespace Superpower.Benchmarks.ArithmeticExpressionScenario
{
    enum ArithmeticExpressionToken
    {
        None,
        Number,
        Plus,
        Minus,

        [Description("multiplication operator")]
        Times,

        Divide,
        LParen,

        [Description("closing parenthesis")]
        RParen
    }
}

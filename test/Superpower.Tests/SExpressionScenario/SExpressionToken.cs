using System.ComponentModel;

namespace Superpower.Tests.SExpressionScenario
{
    enum SExpressionToken
    {
        None,
        Atom,
        Number,

        [Description("open parenthesis")]
        LParen,

        [Description("closing parenthesis")]
        RParen
    }
}
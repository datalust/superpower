using Superpower.Display;
using Superpower.Model;

namespace Superpower.Tests.SExpressionScenario
{
    enum SExpressionToken
    {
        None,
        Atom,
        Number,

        [Token(Description = "open parenthesis")]
        LParen,

        [Token(Description = "closing parenthesis")]
        RParen
    }
}
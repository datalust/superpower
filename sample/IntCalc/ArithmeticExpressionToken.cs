using System.ComponentModel;

namespace IntCalc
{
    enum ArithmeticExpressionToken
    {
        None,

        Number,

        [Description("addition operator")]
        Plus,

        [Description("subtraction operator")]
        Minus,

        [Description("multiplication operator")]
        Times,

        [Description("division operator")]
        Divide,

        [Description("opening parenthesis")]
        LParen,

        [Description("closing parenthesis")]
        RParen
    }
}

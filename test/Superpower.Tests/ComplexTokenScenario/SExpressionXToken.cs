using Superpower.Display;

namespace Superpower.Tests.ComplexTokenScenario
{
    [Token(Category = "S-Expression Token")]
    class SExpressionXToken
    {
        public SExpressionXToken(SExpressionType type)
        {
            Type = type;
        }
        public SExpressionXToken(int number)
        {
            Type = SExpressionType.Number;
            Number = number;
        }
        public SExpressionType Type { get; set; }
        public int Number { get; set; }
    }
}

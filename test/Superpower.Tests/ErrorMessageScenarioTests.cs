using Superpower.Tests.SExpressionScenario;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests
{
    public class ErrorMessageScenarioTests
    {
        [Fact]
        public void ErrorMessagesFromAppliedCharacterParsersPropagate()
        {
            var number = Parse.Token(SExpressionToken.Number)
                  .Apply(t => Parse.Char('1').Then(_ => Parse.Char('x')));
            
            var numbers = number.AtLeastOnce().AtEnd();
            
            AssertParser.FailsWithMessage(numbers, " 123", new SExpressionTokenizer(), "Parsing failure (line 1, column 3): invalid number, unexpected `2`, expected `x`.");
        }
    }
}

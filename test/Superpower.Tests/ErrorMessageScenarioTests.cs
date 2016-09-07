using Superpower.Parsers;
using Superpower.Tests.ArithmeticExpressionScenario;
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
            var number = Token.EqualTo(SExpressionToken.Number)
                  .Apply(t => Character.EqualTo('1').Then(_ => Character.EqualTo('x')));
            
            var numbers = number.AtEnd();
            
            AssertParser.FailsWithMessage(numbers, "123", new SExpressionTokenizer(),
                "Parsing failure (line 1, column 2): invalid number, unexpected `2`, expected `x`.");
        }

        [Fact]
        public void ErrorMessageFromPartialItemsPropagate()
        {
            var atom = Token.EqualTo(SExpressionToken.Atom);
            var number = Token.EqualTo(SExpressionToken.Number);

            var alternating = number.Then(n => atom).AtEnd();

            AssertParser.FailsWithMessage(alternating, "123 123", new SExpressionTokenizer(),
                "Parsing failure (line 1, column 5): unexpected number `123`, expected atom.");
        }

        [Fact]
        public void ErrorMessageFromLastPartialItemPropagates()
        {
            var atom = Token.EqualTo(SExpressionToken.Atom);
            var number = Token.EqualTo(SExpressionToken.Number);

            var alternating = number.Then(n => atom).Many().AtEnd();

            AssertParser.FailsWithMessage(alternating, "123 abc 123 123", new SExpressionTokenizer(),
                "Parsing failure (line 1, column 13): unexpected number `123`, expected atom.");
        }

        [Fact]
        public void ErrorMessageFromIncompleteItemPropagates()
        {
            var atom = Token.EqualTo(SExpressionToken.Atom);
            var number = Token.EqualTo(SExpressionToken.Number);

            var alternating = number.Then(n => atom).AtEnd();

            AssertParser.FailsWithMessage(alternating, "123", new SExpressionTokenizer(),
                "Parsing failure: unexpected end of input, expected atom.");
        }

        [Fact]
        public void DroppedClosingParenthesisProducesMeaningfulError()
        {
            AssertParser.FailsWithMessage(ArithmeticExpressionParser.Lambda, "1 + (2 * 3", new ArithmeticExpressionTokenizer(),
                "Parsing failure: unexpected end of input, expected `)`.");
        }

        [Fact]
        public void MissingOperandProducesMeaningfulError()
        {
            AssertParser.FailsWithMessage(ArithmeticExpressionParser.Lambda, "1 + * 3", new ArithmeticExpressionTokenizer(),
                 "Parsing failure (line 1, column 5): unexpected operator `*`, expected expression.");
        }
    }
}

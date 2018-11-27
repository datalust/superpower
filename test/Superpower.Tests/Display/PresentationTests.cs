using Superpower.Display;
using Superpower.Tests.SExpressionScenario;
using Xunit;
using Superpower.Parsers;

namespace Superpower.Tests.Display
{
    public class PresentationTests
    {
        [Fact]
        public void AnUnadornedEnumMemberIsLowercasedForDisplay()
        {
            var display = Presentation.FormatExpectation(SExpressionToken.Number);
            Assert.Equal("number", display);
        }

        [Fact]
        public void DescriptionAttributeIsInterrogatedForDisplay()
        {
            var display = Presentation.FormatExpectation(SExpressionToken.LParen);
            Assert.Equal("open parenthesis", display);
        }
        [Fact]
        public void ProperNameIsDisplayedWhenNonGraphicalCausesFailure()
        {
            var result=Character.EqualTo('a').TryParse("\x2007");
           
            Assert.Equal("Syntax error (line 1, column 1): unexpected `figure space`, expected `a`.", result.ToString());
        }
        [Fact]
        public void ProperNameIsDisplayedWhenNonGraphicalIsFailed()
        {
            var result=Character.EqualTo('\x2007').TryParse("a");
            Assert.Equal("Syntax error (line 1, column 1): unexpected `a`, expected figure space.",result.ToString());
        }
    }
}

using Superpower.Display;
using Superpower.Tests.SExpressionScenario;
using Xunit;

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
    }
}

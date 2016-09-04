using Superpower.Tests.SExpressionScenario;
using Superpower.Util;
using Xunit;

namespace Superpower.Tests.Util
{
    public class PresentationTests
    {
        [Fact]
        public void AnUnadornedEnumMemberIsLowercasedForDisplay()
        {
            var display = Presentation.FormatKind(SExpressionToken.Number);
            Assert.Equal("number", display);
        }

        [Fact]
        public void DescriptionAttributeIsInterrogatedForDisplay()
        {
            var display = Presentation.FormatKind(SExpressionToken.LParen);
            Assert.Equal("open parenthesis", display);
        }
    }
}

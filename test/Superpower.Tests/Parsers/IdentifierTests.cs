using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Parsers
{
    public class IdentifierTests
    {
        [Fact]
        public void CStyleIdentifiersAreMatched()
        {
            var input = new TextSpan("C_Style!");
            var r = Identifier.CStyle(input);
            Assert.Equal("C_Style", r.Value.ToStringValue());
        }

        [Fact]
        public void CStyleIdentifiersMayStartWithLeadingUnderscore()
        {
            var input = new TextSpan("_cStyle1!");
            var r = Identifier.CStyle(input);
            Assert.Equal("_cStyle1", r.Value.ToStringValue());
        }
        
        [Theory]
        [InlineData("0", false)]
        [InlineData("__", true)]
        [InlineData("A0", true)]
        [InlineData("ab", true)]
        [InlineData("a_b", true)]
        [InlineData("_b", true)]
        [InlineData("1CStyle", false)]
        public void CStyleIdentifiersAreRecognized(string input, bool isMatch)
        {
            AssertParser.FitsTheory(Identifier.CStyle, input, isMatch);
        }
    }
}

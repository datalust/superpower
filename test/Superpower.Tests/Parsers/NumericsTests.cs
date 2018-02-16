using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Parsers
{
    public class NumericsTests
    {
        [Theory]
        [InlineData("0", true)]
        [InlineData("01", true)]
        [InlineData("910", true)]
        [InlineData("-1", true)]
        [InlineData("+1", true)]
        [InlineData("1.1", false)]
        [InlineData("a", false)]
        [InlineData("", false)]
        public void IntegersAreRecognized(string input, bool isMatch)
        {
            if (isMatch)
                AssertParser.SucceedsWithAll(Numerics.Integer, input);
            else
                AssertParser.Fails(Numerics.Integer.AtEnd(), input);
        }
        
        [Theory]
        [InlineData("0", true)]
        [InlineData("01", true)]
        [InlineData("910", true)]
        [InlineData("-1", false)]
        [InlineData("+1", false)]
        [InlineData("1.1", false)]
        [InlineData("a", false)]
        [InlineData("", false)]
        public void NaturalNumbersAreRecognized(string input, bool isMatch)
        {
            if (isMatch)
                AssertParser.SucceedsWithAll(Numerics.Natural, input);
            else
                AssertParser.Fails(Numerics.Natural.AtEnd(), input);
        }
    }
}
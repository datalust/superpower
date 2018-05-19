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
            AssertParser.FitsTheory(Numerics.Integer, input, isMatch);
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
            AssertParser.FitsTheory(Numerics.Natural, input, isMatch);
        }
        
        [Theory]
        [InlineData("0", false)]
        [InlineData("a", false)]
        [InlineData("0x-1", false)]
        [InlineData("0x910", true)]
        [InlineData("0x0", true)]
        [InlineData("0xa", true)]
        [InlineData("0xA", true)]
        [InlineData("0x0123456789abcdef", true)]
        [InlineData("0xg", false)]
        [InlineData("", false)]
        public void HexNaturalNumbersAreRecognized(string input, bool isMatch)
        {
            AssertParser.FitsTheory(Numerics.HexNatural, input, isMatch);
        }
        
        [Theory]
        [InlineData("0", true)]
        [InlineData("01", true)]
        [InlineData("910", true)]
        [InlineData("-1", true)]
        [InlineData("+1", true)]
        [InlineData("1.1", true)]
        [InlineData("-1.1", true)]
        [InlineData("a", false)]
        [InlineData("", false)]
        [InlineData("123.456", true)]
        [InlineData("123.+456", false)]
        [InlineData("123.", false)]
        [InlineData(".456", false)]
        [InlineData("-.456", false)]
        public void DecimalNumbersAreRecognized(string input, bool isMatch)
        {
            AssertParser.FitsTheory(Numerics.Decimal, input, isMatch);
        }
    }
}

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
        [InlineData("\u0669\u0661\u0660", false)] // 910 in Arabic
        [InlineData("9\u0661\u0660", false)] // 9 in Latin then 10 in Arabic
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
        [InlineData("\u0669\u0661\u0660", false)] // 910 in Arabic
        [InlineData("9\u0661\u0660", false)] // 9 in Latin then 10 in Arabic
        public void NaturalNumbersAreRecognized(string input, bool isMatch)
        {
            AssertParser.FitsTheory(Numerics.Natural, input, isMatch);
        }
        
        [Theory]
        [InlineData("0", true)]
        [InlineData("-1", false)]
        [InlineData("910", true)]
        [InlineData("0x123", false)]
        [InlineData("a", true)]
        [InlineData("A", true)]
        [InlineData("0123456789abcdef", true)]
        [InlineData("g", false)]
        [InlineData("", false)]
        [InlineData("\u0669\u0661\u0660", false)] // 910 in Arabic
        [InlineData("9\u0661\u0660", false)] // 9 in Latin then 10 in Arabic
        public void HexDigitsAreRecognized(string input, bool isMatch)
        {
            AssertParser.FitsTheory(Numerics.HexDigits, input, isMatch);
        }
        
        [Theory]
        [InlineData("0", 0)]
        [InlineData("a", 0xa)]
        [InlineData("910", 0x910)]
        [InlineData("A", 0xA)]
        [InlineData("012345678", 0x12345678)]
        [InlineData("9abcdef", 0x9abcdef)]
        public void HexDigitsAreParsed(string input, uint value)
        {
            var parsed = Numerics.HexDigitsUInt32.Parse(input);
            Assert.Equal(value, parsed);
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

        [Fact]
        public void DecimalNumbersAreParsed()
        {
            var parsed = Numerics.DecimalDecimal.Parse("-123.456");
            Assert.Equal(-123.456m, parsed);
        }

        [Fact]
        public void DecimalDoublesAreParsed()
        {
            var parsed = Numerics.DecimalDouble.Parse("-123.456");
            Assert.Equal(-123.456, parsed);
        }
    }
}

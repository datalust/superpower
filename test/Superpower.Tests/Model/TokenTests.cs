using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Model
{
    public class TokenTests
    {
        [Fact]
        public void IdenticalTokensAreEqual()
        {
            var first = new Token<char>('c', TextSpan.Empty);
            var second = new Token<char>('c', TextSpan.Empty);

            var result1 = first.Equals(second);
            var result2 = first.Equals((object)second);
            var result3 = first == second;

            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public void TokensWithDifferentKindsAreNotEqual()
        {
            var first = new Token<char>('c', TextSpan.Empty);
            var second = new Token<char>('d', TextSpan.Empty);

            var result1 = first.Equals(second);
            var result2 = first.Equals((object)second);
            var result3 = first == second;
            var result4 = first != second;

            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
            Assert.True(result4);
        }

        [Fact]
        public void TokensWithDifferentSpansAreNotEqual()
        {
            var first = new Token<char>('c', TextSpan.Empty);
            var second = new Token<char>('c', new TextSpan("abc"));

            var result1 = first.Equals(second);
            var result2 = first.Equals((object)second);
            var result3 = first == second;
            var result4 = first != second;

            Assert.False(result1);
            Assert.False(result2);
            Assert.False(result3);
            Assert.True(result4);
        }

        [Fact]
        public void IdenticalTokensHaveTheSameHashCode()
        {
            var first = new Token<char>('c', TextSpan.Empty);
            var second = new Token<char>('c', TextSpan.Empty);

            var hashCode1 = first.GetHashCode();
            var hashCode2 = second.GetHashCode();

            Assert.Equal(hashCode1, hashCode2);
        }
    }
}

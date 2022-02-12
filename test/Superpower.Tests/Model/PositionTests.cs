using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Model
{
    public class PositionTests
    {
        [Fact]
        public void IdenticalPositionsAreEqual()
        {
            var first = new Position(10, 20, 30);
            var second = new Position(10, 20, 30);

            var result1 = first.Equals(second);
            var result2 = first.Equals((object)second);
            var result3 = first == second;

            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public void PositionsWithDifferentAbsolutesAreNotEqual()
        {
            var first = new Position(10, 20, 30);
            var second = new Position(20, 20, 30);

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
        public void PositionsWithDifferentLinesAreNotEqual()
        {
            var first = new Position(10, 20, 30);
            var second = new Position(10, 30, 30);

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
        public void PositionsWithDifferentColumnsAreNotEqual()
        {
            var first = new Position(10, 20, 30);
            var second = new Position(10, 20, 40);

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
        public void IdenticalPositionsHaveTheSameHashCode()
        {
            var first = new Position(10, 20, 30);
            var second = new Position(10, 20, 30);

            var hashCode1 = first.GetHashCode();
            var hashCode2 = second.GetHashCode();

            Assert.Equal(hashCode1, hashCode2);
        }
    }
}

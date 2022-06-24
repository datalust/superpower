using System;
using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Model
{
    public class ResultTests
    {
        [Fact]
        public void IdenticalResultsAreEqual()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);
            var second= new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);

            var result1 = first.Equals(second);
            var result2 = first.Equals((object)second);
            var result3 = first == second;

            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public void ResultsWithDifferentLocationsAreNotEqual()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new Result<char>(new TextSpan("source"), TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);

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
        public void ResultsWithDifferentRemaindersAreNotEqual()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new Result<char>(TextSpan.Empty, new TextSpan("source"), "errorMessage", Array.Empty<string>(), true);

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
        public void ResultsWithDifferentErrorMessagesAreNotEqual()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage1", Array.Empty<string>(), true);
            var second = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage2", Array.Empty<string>(), true);

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
        public void ResultsWithDifferentExpectationsAreNotEqual()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", null, true);

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
        public void ResultsWithDifferentBacktracksAreNotEqual()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), false);

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
        public void IdenticalResultsHaveTheSameHashCode()
        {
            var first = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new Result<char>(TextSpan.Empty, TextSpan.Empty, "errorMessage", Array.Empty<string>(), true);

            var hashCode1 = first.GetHashCode();
            var hashCode2 = second.GetHashCode();

            Assert.Equal(hashCode1, hashCode2);
        }
    }
}

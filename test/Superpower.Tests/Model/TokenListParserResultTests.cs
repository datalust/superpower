using System;
using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Model
{
    public class TokenListParserResultTests
    {
        [Fact]
        public void IdenticalTokenListParserResultsAreEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);

            var result1 = first.Equals(second);
            var result2 = first.Equals((object)second);
            var result3 = first == second;

            Assert.True(result1);
            Assert.True(result2);
            Assert.True(result3);
        }

        [Fact]
        public void TokenListParserResultsWithDifferentLocationsAreNotEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(new TokenList<int>(Array.Empty<Token<int>>()), TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);

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
        public void TokenListParserResultsWithDifferentRemaindersAreNotEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, new TokenList<int>(Array.Empty<Token<int>>()), Position.Empty, "errorMessage", Array.Empty<string>(), true);

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
        public void TokenListParserResultsWithDifferentPositionsAreNotEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, new Position(10, 5, 3), "errorMessage", Array.Empty<string>(), true);

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
        public void TokenListParserResultsWithDifferentErrorMessagesAreNotEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage1", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage2", Array.Empty<string>(), true);

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
        public void TokenListParserResultsWithDifferentExpectationsAreNotEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", null, true);

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
        public void TokenListParserResultsWithDifferentBacktracksAreNotEqual()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), false);

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
        public void IdenticalTokenListParserResultsHaveTheSameHashCode()
        {
            var first = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);
            var second = new TokenListParserResult<int, int>(TokenList<int>.Empty, TokenList<int>.Empty, Position.Empty, "errorMessage", Array.Empty<string>(), true);

            var hashCode1 = first.GetHashCode();
            var hashCode2 = second.GetHashCode();

            Assert.Equal(hashCode1, hashCode2);
        }
    }
}

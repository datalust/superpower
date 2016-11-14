using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class TryCombinatorTests
    {
        [Fact]
        public void TryFailureConsumesNoInput()
        {
            var tryAb = Character.EqualTo('a').Then(_ => Character.EqualTo('b')).Try();
            var result = tryAb.TryParse("ac");
            Assert.False(result.HasValue);
            Assert.True(result.Backtrack);
        }

        [Fact]
        public void TrySuccessIsTransparent()
        {
            var tryAb = Character.EqualTo('a').Then(_ => Character.EqualTo('b')).Try();
            var result = tryAb.TryParse("ab");
            Assert.True(result.HasValue);
            Assert.True(result.Remainder.IsAtEnd);
        }

        [Fact]
        public void TryItemMakesManyBacktrack()
        {
            var ab = Character.EqualTo('a').Then(_ => Character.EqualTo('b'));
            var list = ab.Try().Many();
            AssertParser.SucceedsWithMany(list, "ababa", "bb".ToCharArray());
        }

        [Fact]
        public void TryAlternativeMakesOrBacktrack()
        {
            var tryAOrAB = Character.EqualTo('a').Then(_ => Character.EqualTo('b')).Try().Or(Character.EqualTo('a'));
            AssertParser.SucceedsWith(tryAOrAB, "a", 'a');
        }

        [Fact]
        public void TokenTryFailureBacktracks()
        {
            var tryAb = Token.EqualTo('a').Then(_ => Token.EqualTo('b')).Try();
            var result = tryAb.TryParse(StringAsCharTokenList.Tokenize("ac"));
            Assert.False(result.HasValue);
            Assert.True(result.Backtrack);
        }

        [Fact]
        public void TokenTrySuccessIsTransparent()
        {
            var tryAb = Token.EqualTo('a').Then(_ => Token.EqualTo('b')).Try();
            var result = tryAb.TryParse(StringAsCharTokenList.Tokenize("ab"));
            Assert.True(result.HasValue);
            Assert.True(result.Remainder.IsAtEnd);
        }

        [Fact]
        public void TokenTryItemMakesManyBacktrack()
        {
            var ab = Token.EqualTo('a').Then(_ => Token.EqualTo('b'));
            var list = ab.Try().Many();
            AssertParser.SucceedsWithMany(list, "ababa", "bb".ToCharArray());
        }

        [Fact]
        public void TokenTryAlternativeMakesOrBacktrack()
        {
            var tryAOrAB = Token.EqualTo('a').Then(_ => Token.EqualTo('b')).Try().Or(Token.EqualTo('a'));
            AssertParser.SucceedsWith(tryAOrAB, "a", 'a');
        }
    }
}

using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class SequenceCombinatorTests
    {
        [Fact]
        public void Sequence2Succeeds()
        {
            var p = Parse.Sequence(Character.Digit, Character.AnyChar);
            AssertParser.SucceedsWithAll(Span.MatchedBy(p), "1w");
        }

        [Fact]
        public void Sequence3Succeeds()
        {
            var p = Parse.Sequence(Character.Digit, Character.AnyChar, Character.Upper);
            AssertParser.SucceedsWithAll(Span.MatchedBy(p), "1wU");
        }

        [Fact]
        public void Sequence4Succeeds()
        {
            var p = Parse.Sequence(Character.Digit, Character.AnyChar, Character.Upper, Character.Letter);
            AssertParser.SucceedsWithAll(Span.MatchedBy(p), "1wUh");
        }

        [Fact]
        public void Sequence5Succeeds()
        {
            var p = Parse.Sequence(Character.Digit, Character.AnyChar, Character.Upper, Character.Letter, Character.Lower);
            AssertParser.SucceedsWithAll(Span.MatchedBy(p), "1wUhh");
        }
        
        [Fact]
        public void Sequence3ReportsCorrectErrorPosition()
        {
            var p = Parse.Sequence(Character.Digit, Character.AnyChar, Character.Upper);
            AssertParser.FailsAt(Span.MatchedBy(p), "1w1g", 2);
        }
        
        [Fact]
        public void Sequence2TokenSucceeds()
        {
            // Issue - explicit tuple argument types are needed here; see:
            // https://github.com/dotnet/csharplang/issues/258
            // Keeping this instance as an example, but using the "cleaner" .Item1, .Item2 syntax below.
            var p = Parse.Sequence(Token.EqualTo('1'), Token.EqualTo('w'))
                .Select(((Token<char> a, Token<char> b) t) => new []{t.a, t.b});

            AssertParser.SucceedsWithAll(p, "1w");
        }

        [Fact]
        public void Sequence3TokenSucceeds()
        {
            var p = Parse.Sequence(Token.EqualTo('1'), Token.EqualTo('w'), Token.EqualTo('U'))
                .Select(t => new []{t.Item1, t.Item2, t.Item3});
            AssertParser.SucceedsWithAll(p, "1wU");
        }

        [Fact]
        public void Sequence4TokenSucceeds()
        {
            var p = Parse.Sequence(Token.EqualTo('1'), Token.EqualTo('w'), Token.EqualTo('U'), Token.EqualTo('h'))
                .Select(t => new []{t.Item1, t.Item2, t.Item3, t.Item4});
            AssertParser.SucceedsWithAll(p, "1wUh");
        }

        [Fact]
        public void Sequence5TokenSucceeds()
        {
            var p = Parse.Sequence(Token.EqualTo('1'), Token.EqualTo('w'), Token.EqualTo('U'), Token.EqualTo('h'), Token.EqualTo('h'))
                .Select(t => new []{t.Item1, t.Item2, t.Item3, t.Item4, t.Item5});
            AssertParser.SucceedsWithAll(p, "1wUhh");
        }
        
        [Fact]
        public void Sequence3TokenReportsCorrectErrorPosition()
        {
            var p = Parse.Sequence(Token.EqualTo('1'), Token.EqualTo('w'), Token.EqualTo('U'))
                .Select(t => new []{t.Item1, t.Item2, t.Item3});
            AssertParser.FailsAt(p, "1w1g", 2);
        }
    }
}

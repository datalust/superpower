using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tests.Support;
using System.Linq;
using Xunit;

namespace Superpower.Tests.Combinators
{
    public class OneOfCombinatorTests
    {
        [Fact]
        public void OneOf2Succeeds()
        {
            var p = Parse.OneOf(Character.Digit, Character.AnyChar);
            AssertParser.SucceedsWithAll(Span.MatchedBy(p), "1");
            AssertParser.SucceedsWithAll(Span.MatchedBy(p), "w");
        }

        [Fact]
        public void OneOf2TokenSucceeds()
        {
            var p = Parse.OneOf(Token.EqualTo('1'), Token.EqualTo('w'));
            AssertParser.SucceedsWith(p, "1", '1');
            AssertParser.SucceedsWith(p, "w", 'w');
        }

        [Fact]
        public void OneOfReportsCorrectError()
        {
            var names = new[] { "one", "two" };
            TextParser<TextSpan> p = Parse.OneOf(names.Select(Span.EqualTo).ToArray());
            AssertParser.SucceedsWith(p.Select(t => t.ToStringValue()), "one", "one");
            AssertParser.SucceedsWith(p.Select(t => t.ToStringValue()), "two", "two");
            AssertParser.FailsWithMessage(p.Select(t => t.ToStringValue()), "four", "Syntax error (line 1, column 1): unexpected `f`, expected `one` or `two`.");
        }
    }
}

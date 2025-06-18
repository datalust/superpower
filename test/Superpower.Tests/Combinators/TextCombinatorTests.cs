using Superpower.Parsers;
using Superpower.Tests.Support;
using Xunit;

namespace Superpower.Tests.Combinators;

public class TextCombinatorTests
{
    [Fact]
    public void TextSucceedsWithAnyCharArrayInput()
    {
        AssertParser.SucceedsWith(Character.AnyChar.Many().Text(), "ab", "ab");
    }

    [Fact]
    public void TextSucceedsWithTextSpanInput()
    {
        AssertParser.SucceedsWith(Span.Length(2).Text(), "ab", "ab");
    }
}

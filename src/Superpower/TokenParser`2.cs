using Superpower.Model;

namespace Superpower
{
    public delegate TokenResult<TTokenKind, T> TokenParser<TTokenKind, T>(TokenList<TTokenKind> input);
}
using System.Linq;
using Superpower.Model;

namespace Superpower.Tests.Support
{
    static class StringAsCharTokenList
    {
        public static TokenList<char> Tokenize(string tokens)
        {
            var items = tokens.ToCharArray()
                .Select((ch, i) => new Token<char>(ch, new StringSpan(tokens, new Position(i, 1, 1), 1)))
                .ToArray();

            return new TokenList<char>(items);
        }
    }
}
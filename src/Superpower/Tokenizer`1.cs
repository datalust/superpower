using System.Collections.Generic;
using Superpower.Model;

namespace Superpower
{
    public abstract class Tokenizer<TTokenKind>
    {
        public abstract IEnumerable<Token<TTokenKind>> Tokenize(StringSpan stringSpan);
    }
}
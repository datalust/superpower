using System.Collections.Generic;
using Superpower.Model;
using Xunit;

namespace Superpower.Tests.Support
{
    public class PreviousCheckingTokenizer : Tokenizer<int>
    {
        protected override IEnumerable<Result<int>> Tokenize(TextSpan span, TokenizationState<int> state)
        {
            Assert.NotNull(state);            
            Assert.Null(state.Previous);
            var next = span.ConsumeChar();
            yield return Result.Value(0, next.Location, next.Remainder);
            
            for (var i = 1; i < span.Length; ++i)
            {
                Assert.NotNull(state.Previous);
                Assert.Equal(i - 1, state.Previous.Value.Kind);
                next = next.Remainder.ConsumeChar();
                yield return Result.Value(i, next.Location, next.Remainder);
            }
        }
    }
}
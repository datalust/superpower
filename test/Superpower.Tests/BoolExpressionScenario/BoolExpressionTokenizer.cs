using System.Collections.Generic;
using Superpower.Model;
using Superpower.Parsers;

namespace Superpower.Tests.BoolExpressionScenario
{
    class BoolExpressionTokenizer : Tokenizer<BoolExpressionToken>
    {
        private readonly Dictionary<char, BoolExpressionToken> _operators = new Dictionary<char, BoolExpressionToken> {
            ['+'] = BoolExpressionToken.Plus,
            ['-'] = BoolExpressionToken.Minus,
            ['*'] = BoolExpressionToken.Times,
            ['/'] = BoolExpressionToken.Divide,
            ['('] = BoolExpressionToken.LParen,
            [')'] = BoolExpressionToken.RParen,
            ['{'] = BoolExpressionToken.LCrl,
            ['}'] = BoolExpressionToken.RCrl,
            [','] = BoolExpressionToken.Comma,
            ['&'] = BoolExpressionToken.And,
            ['|'] = BoolExpressionToken.Or,
            ['>'] = BoolExpressionToken.Gt,
            ['<'] = BoolExpressionToken.Lt,
            ['!'] = BoolExpressionToken.Not
        };

        private readonly Dictionary<string, BoolExpressionToken> _keywords = new Dictionary<string, BoolExpressionToken> {
            ["if"] = BoolExpressionToken.If,
            ["then"] = BoolExpressionToken.Then,
            ["else"] = BoolExpressionToken.Else,
            ["in"] = BoolExpressionToken.In
        };

        protected override IEnumerable<Result<BoolExpressionToken>> Tokenize(TextSpan span)
        {
            var next = SkipWhiteSpace(span);
            if (!next.HasValue)
                yield break;

            do
            {
                BoolExpressionToken charToken;

                if (char.IsDigit(next.Value))
                {
                    var integer = Numerics.Integer(next.Location);
                    next = integer.Remainder.ConsumeChar();
                    yield return Result.Value(BoolExpressionToken.Number, integer.Location, integer.Remainder);
                }
                else if (char.IsLetter(next.Value)) {
                    var start = next.Location;
                    while ( next.HasValue && char.IsLetterOrDigit(next.Value) ) {
                        next = next.Remainder.ConsumeChar();
                    }

                    var str = next.Location.Source.Substring(
                        start.Position.Absolute,
                        next.Location.Position.Absolute - start.Position.Absolute);

                    yield return _keywords.TryGetValue(str, out var token) ?
                        Result.Value(token, start, next.Location) :
                        Result.Value(BoolExpressionToken.Variable, start, next.Location);
                }
                else if (_operators.TryGetValue(next.Value, out charToken)) {
                    yield return Result.Value(charToken, next.Location, next.Remainder);
                    next = next.Remainder.ConsumeChar();
                }
                else {
                    yield return Result.Empty<BoolExpressionToken>(next.Location, new[] { "number", "variable", "operator" });
                }

                next = SkipWhiteSpace(next.Location);
            } while (next.HasValue);
        }
    }
}

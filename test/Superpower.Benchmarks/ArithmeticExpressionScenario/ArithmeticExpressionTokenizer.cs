namespace Superpower.Benchmarks.ArithmeticExpressionScenario;

class ArithmeticExpressionTokenizer : Tokenizer<ArithmeticExpressionToken>
{
	readonly Dictionary<char, ArithmeticExpressionToken> _operators = new()
	{
		['+'] = ArithmeticExpressionToken.Plus,
		['-'] = ArithmeticExpressionToken.Minus,
		['*'] = ArithmeticExpressionToken.Times,
		['/'] = ArithmeticExpressionToken.Divide,
		['('] = ArithmeticExpressionToken.LParen,
		[')'] = ArithmeticExpressionToken.RParen,
	};

	protected override IEnumerable<Result<ArithmeticExpressionToken>> Tokenize(TextSpan span)
	{
		var next = SkipWhiteSpace(span);
		if (!next.HasValue)
			yield break;

		do
		{

			var ch = next.Value;
			if (ch is >= '0' and <= '9')
			{
				var integer = Numerics.Integer(next.Location);
				next = integer.Remainder.ConsumeChar();
				yield return Result.Value(ArithmeticExpressionToken.Number, integer.Location, integer.Remainder);
			}
			else if (_operators.TryGetValue(ch, out ArithmeticExpressionToken charToken))
			{
				yield return Result.Value(charToken, next.Location, next.Remainder);
				next = next.Remainder.ConsumeChar();
			}
			else
			{
				yield return Result.Empty<ArithmeticExpressionToken>(next.Location, ["number", "operator"]);
			}

			next = SkipWhiteSpace(next.Location);
		} while (next.HasValue);
	}
}

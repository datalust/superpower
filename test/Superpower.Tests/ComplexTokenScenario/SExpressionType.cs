using Superpower.Display;

namespace Superpower.Tests.ComplexTokenScenario;

enum SExpressionType
{
	None,
	Atom,
	Number,

	[Token(Description = "open parenthesis")]
	LParen,

	[Token(Description = "closing parenthesis")]
	RParen
}

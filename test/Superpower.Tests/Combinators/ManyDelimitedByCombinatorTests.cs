using Superpower.Tests.Support;

namespace Superpower.Tests.Combinators;

public class ManyDelimitedByCombinatorTests
{
	[Fact]
	public void AnEndDelimiterCanBeSpecified()
	{
		AssertParser.SucceedsWith(
			Token.EqualTo('a').Value('a')
				.ManyDelimitedBy(Token.EqualTo('b'), end: Token.EqualTo('c')),
			"ababac",
			['a', 'a', 'a']);
	}
}
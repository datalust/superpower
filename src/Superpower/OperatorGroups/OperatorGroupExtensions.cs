using System.Collections.Generic;
using System.Linq;

namespace Superpower.OperatorGroups
{
    /// <summary>
    /// Extension methods used for defining parsers based on collections of
    /// <see cref="IEnumerable{IOperatorGroup}"/>
    /// </summary>
    public static class OperatorGroupExtensions
    {
        /// <summary>
        /// Create <see cref="TokenListParser{TKind,T}"/> representing
        /// operators defined in <paramref name="operatorGroups"/>
        /// </summary>
        /// <typeparam name="TKind">The kind of token being parsed.</typeparam>
        /// <typeparam name="T">The type being parsed.</typeparam>
        public static TokenListParser<TKind, T> DefineParser<TKind, T>(
            this IEnumerable<IOperatorGroup<TKind, T>> operatorGroups,
            TokenListParser<TKind, T> operandParser)
        {
            return operatorGroups
                .OrderByDescending(g => g.Precedence)
                .Aggregate(operandParser, (p, g) => g.BuildParser(p));
        }
    }
}

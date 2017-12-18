namespace Superpower.OperatorGroups
{
    /// <summary>
    /// Definition of a group of operators with a given operator precedence. Operators can either be
    /// prefix or infix operators, used to combine expressions where operator precedence is important.
    /// </summary>
    /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
    /// <typeparam name="T">The type of value being parsed.</typeparam>
    public interface IOperatorGroup<TKind, T>
    {
        /// <summary>
        /// Precedence of operators in group. The higher the precedence, the "harder" the
        /// operator binds.
        /// </summary>
        int Precedence { get; }

        /// <summary>
        /// <para>
        /// Method for building a new <see cref="TokenListParser{TToken,TResult}"/> parser
        /// representing operators in group.
        /// </para>
        /// <para>The resulting parser will match one or more operands combined with the operators in
        /// the group, or just the <paramref name="operandParser"/> itself.
        /// </para>
        /// </summary>
        TokenListParser<TKind, T> BuildParser(TokenListParser<TKind, T> operandParser);
    }
}
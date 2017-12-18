using System;
using System.Collections.Generic;
using System.Linq;

namespace Superpower.OperatorPrecedenceParser
{
    /// <summary>
    /// Definition of a set of expressions
    /// </summary>
    public class ExprSetDef<TKind, T> : IExprDef<TKind>
    {
        /// <summary>
        /// <para>
        /// Construct a new <see cref="ExprSetDef{TKind,T}"/> representing
        /// a set of expression. </para>
        /// <para>The most simple form for set is the singleton set that does not
        /// have any <paramref name="separators"/> - this could e.g. use to define
        /// an expression in parenthesis.</para>
        /// </summary>
        /// <param name="setStart">Token defining the start of a set</param>
        /// <param name="setEnd">Token defining the end of the set</param>
        /// <param name="separators">A collection of allowed separators between elements in the set.
        /// If this is set to null, then only singleton sets are accepted.</param>
        /// <param name="allowEmptySet">If set to true, will allow the empty set.</param>
        /// <param name="resultBuilder">Function for creating result of type <typeparamref name="T"/> given
        /// list of operands and list of separators. If set to null, then
        /// <paramref name="separators"/> must be empty/null, and the function used will
        /// be the identity function, simply returning the singleton operand as the result.
        /// </param>
        /// <param name="canBuild">Optional function to validate that operands and separators can
        /// build into a result of type <typeparamref name="T"/>. If this function is given,
        /// the function is called before trying to build a result. If the function returns any string
        /// but null, it is considered an error message, which is reported in a parse exception.</param>
        public ExprSetDef(
            TKind setStart,
            TKind setEnd,
            IEnumerable<TKind> separators = null,
            bool allowEmptySet = false,
            Func<IEnumerable<T>, IEnumerable<TKind>, T> resultBuilder = null,
            Func<IEnumerable<T>, IEnumerable<TKind>, string> canBuild = null)
        {
            SetStart = setStart;
            SetEnd = setEnd;
            AllowEmptySet = allowEmptySet;
            CanBuild = canBuild ?? ( (operands, seps) => null );
            Separators = separators?.ToArray() ?? new TKind[0];

            if ( resultBuilder == null && Separators.Count > 0 ) {
                throw new ArgumentException(
                    "A set where separators are defined must also define a function for mapping operands into a result.");
            }

            ResultBuilder = resultBuilder ?? ( (operands, seps) => operands.First() );
        }

        /// <summary>
        /// Return true if <paramref name="token"/> is either defined in
        /// <see cref="Separators"/> or as <see cref="SetEnd"/>
        /// </summary>
        public bool IsSeparatorOrEnd(TKind token) =>
            SetEnd.Equals(token) || Separators.Contains(token);

        /// <summary>
        /// Return true if <paramref name="token"/> is either defined in
        /// <see cref="Separators"/> or as <see cref="SetStart"/>
        /// </summary>
        public bool IsSeparatorOrStart(TKind token) =>
            SetStart.Equals(token) || Separators.Contains(token);

        /// <summary>
        /// Token defining the start of the set
        /// </summary>
        public TKind SetStart { get; }

        /// <summary>
        /// Token defining the end of the set
        /// </summary>
        public TKind SetEnd { get; }

        /// <summary>
        /// If set to true, will allow the empty set
        /// </summary>
        public bool AllowEmptySet { get; }

        /// <summary>
        /// Function to validate that operands and separators can
        /// build into a result of type <typeparamref name="T"/>. This function
        /// is called before trying to build a result. If the function returns any string
        /// but null, it is considered an error message, which is reported in a parse exception.
        /// </summary>
        public Func<IEnumerable<T>, IEnumerable<TKind>, string> CanBuild { get; }

        /// <summary>
        /// Tokens used to separate operands
        /// </summary>
        public IReadOnlyList<TKind> Separators { get; }

        /// <summary>
        /// Function for creating result of type <typeparamref name="T"/> based on
        /// a collection of input operands and a set of separators.
        /// </summary>
        public Func<IEnumerable<T>, IEnumerable<TKind>, T> ResultBuilder { get; }

        /// <inheritdoc />
        public int Precedence => 0;
    }
}

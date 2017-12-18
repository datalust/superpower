using System.Collections.Generic;

namespace Superpower.OperatorPrecedenceParser
{
    /// <summary>
    /// Marker interface used to describe a valid part of an expression, used
    /// when defining a operator precedence parser with the <see cref="OperatorPrecedenceParser{TKind,T,U}"/>
    /// class.
    /// </summary>
    /// <typeparam name="TKind">Type of tokens parsed</typeparam>
    internal interface IExprDef<TKind>
    {
        /// <summary>
        /// Precedence of expressions represented by this definition. For
        /// operators, the precedence define how "hard" the operator binds.
        /// </summary>
        int Precedence { get; }
    }

    /// <summary>
    /// Part of an expression representing a prefix or infix operator
    /// </summary>
    internal interface IExprOprDef<TKind> : IExprDef<TKind>
    {
        /// <summary>
        /// Tokens representing operators in this group
        /// </summary>
        IReadOnlyList<TKind> Tokens { get; set; }
    }
}
using System;

namespace Superpower.OperatorPrecedenceParser
{
    /// <summary>
    /// <para>Definition of If-Then-Else expressions, where the else part is optional.</para>
    /// <para>The Else part will bind to the nearest If part, in case If-Then-Else expressions
    /// are nested.</para>
    /// </summary>
    /// <typeparam name="TKind">Type of tokens parsed</typeparam>
    /// <typeparam name="T">Type of result output by parser</typeparam>
    public class IfThenElseDef<TKind, T> : IExprDef<TKind>
    {
        /// <summary>
        /// Construct a new <see cref="IfThenElseDef{TKind,T}"/> representing expression written
        /// as IF condition THEN when-true ELSE when-false
        /// </summary>
        /// <param name="ifToken">Token to appear before condition</param>
        /// <param name="thenToken">Token to appear before when-true</param>
        /// <param name="elseToken">Token to appear before when-false</param>
        /// <param name="buildIfThenElseResult"></param>
        /// <param name="buildIfThenResult"></param>
        public IfThenElseDef(
            TKind ifToken,
            TKind thenToken,
            TKind elseToken,
            Func<T, T, T, T> buildIfThenElseResult,
            Func<T, T, T> buildIfThenResult)
        {

            IfToken = ifToken;
            ThenToken = thenToken;
            ElseToken = elseToken;
            BuildIfThenElseResult = buildIfThenElseResult;
            BuildIfThenResult = buildIfThenResult;
        }

        /// <summary>
        /// Function for building If-Then-Else expression from condition,
        /// when-true and when-false
        /// </summary>
        public Func<T, T, T, T> BuildIfThenElseResult { get; }

        /// <summary>
        /// Function for building If-Then expression from condition
        /// and when-true
        /// </summary>
        public Func<T, T, T> BuildIfThenResult { get; }

        /// <summary>
        /// Token representing beginning of IF part of expression
        /// </summary>
        public TKind IfToken { get; }

        /// <summary>
        /// Token representing beginning of THEN part of expression
        /// </summary>
        public TKind ThenToken { get; }

        /// <summary>
        /// Token representing beginning of ELSE part of expression
        /// </summary>
        public TKind ElseToken { get; }

        /// <inheritdoc />
        public int Precedence { get; } = 0;
    }
}

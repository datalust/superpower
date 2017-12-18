using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Superpower.Display;
using Superpower.Model;

namespace Superpower.OperatorPrecedenceParser
{

    /// <summary>
    /// <para>An operator precedence parser is a bottom-up parser accepting simple grammars describing
    /// expressions. This type of parser can be used for expressions with grammars where no production
    /// rule leads to empty input, and where no production rule has two adjacent non-terminal symbols.
    /// </para>
    /// <para>
    /// This implementation allow a grammar to be represented by <see cref="IExprDef{TKind}"/>
    /// constructs, providing support for infix operators, prefix operators, sets of
    /// expressions (including singleton sets, such as parenthesis) and if-then-else
    /// constructs.</para>
    /// <para>The parser is implemented using a stack, with shift/reduce actions. As such, this
    /// type of parser is not prone to stack-overflow exceptions when parsing
    /// deeply nested expressions.
    /// </para>
    /// <para>The same token can be used to define both a prefix and an infix operator (e.g.
    /// a MINUS token representing infix subtraction and prefix negation), but any token
    /// used to define a set or if-then-else construct cannot also be used
    /// to define a prefix/infix operator.</para>
    /// </summary>
    /// <typeparam name="TKind">Type of input token</typeparam>
    /// <typeparam name="T">Resulting types</typeparam>
    /// <typeparam name="U">Type of operator used to combine the resulting operands of type <typeparamref name="T"/>
    /// into new operands when handling prefix and infix operators</typeparam>
    internal class OperatorPrecedenceParser<TKind, T, U>
    {
        private static readonly TokenListParserResult<TKind, T>
            EmptyParseResult = new TokenListParserResult<TKind, T>();

        private static readonly TKind EmptyToken = default(TKind);
        private static readonly IExprDef<TKind> EmptyExprDef = default(IExprDef<TKind>);

        private readonly TokenListParser<TKind, T> _operandParser;

        private readonly Dictionary<TKind, PrefixOprDef<TKind, T, U>> _prefixOperators;
        private readonly Dictionary<TKind, InfixOprDef<TKind, T, U>> _infixOperators;
        private readonly Dictionary<TKind, ExprSetDef<TKind, T>> _setDefinitions;
        private readonly Dictionary<TKind, IfThenElseDef<TKind, T>> _iteDefinitions;

        /// <summary>
        /// Definition of what has been read from input
        /// </summary>
        private enum ResultType
        {
            Error,
            EndOfInput,
            Operand,
            Operator,
            Set,
            IfThenElse
        }

        /// <summary>
        /// Definition of shift vs reduce operation
        /// </summary>
        private enum ShiftReduce
        {
            Shift,
            Reduce
        }

        /// <summary>
        /// Data read from input
        /// </summary>
        private struct NextResult
        {
            public NextResult(
                ResultType resultType,
                TKind token,
                IExprDef<TKind> exprDef,
                TokenListParserResult<TKind, T> parseResult)
            {

                ResultType = resultType;
                Token = token;
                ExprDef = exprDef;
                ParseResult = parseResult;
            }

            public ResultType ResultType { get; }

            public TKind Token { get; }

            public IExprDef<TKind> ExprDef { get; }

            public TokenListParserResult<TKind, T> ParseResult { get; }
        }

        /// <summary>
        /// Definition of what is expected next. Used to detect errors in
        /// input.
        /// </summary>
        private struct ExpectedNext
        {
            public bool ExpectOperand { get; set; }

            public Stack<ExprSetDef<TKind, T>> OpenSet { get; set; }

            public Stack<(TKind token, IfThenElseDef<TKind, T> iteDef)> OpenIte { get; set; }
        }

        /// <summary>
        /// Construct an operator precedence parser capable of parsing expressions described
        /// by <paramref name="exprDefs"/>. The operands for the parser are defined
        /// by the <paramref name="operandParser"/>. Note that the <paramref name="operandParser"/>
        /// should not accept the empty token set.
        /// </summary>
        public static TokenListParser<TKind, T> DefineParser(
            IEnumerable<IExprDef<TKind>> exprDefs,
            TokenListParser<TKind, T> operandParser)
        {
            var parser = new OperatorPrecedenceParser<TKind, T, U>(exprDefs, operandParser);

            return input => parser.Parse(input);
        }

        private OperatorPrecedenceParser(
            IEnumerable<IExprDef<TKind>> exprDefs,
            TokenListParser<TKind, T> operandParser)
        {

            _operandParser = operandParser;

            var exprDefsList = exprDefs.ToList();

            _infixOperators = exprDefsList
                .OfType<InfixOprDef<TKind, T, U>>()
                .SelectMany(opr => opr.Tokens.Select(t => ( t, opr )))
                .ToDictionary(t => t.Item1, t => t.Item2);

            _prefixOperators = exprDefsList
                .OfType<PrefixOprDef<TKind, T, U>>()
                .SelectMany(opr => opr.Tokens.Select(t => (t, opr)))
                .ToDictionary(t => t.Item1, t => t.Item2);

            _setDefinitions = exprDefsList
                .OfType<ExprSetDef<TKind, T>>()
                .SelectMany(set => set.Separators.Concat(new[] { set.SetStart, set.SetEnd }).Select(t => (t, set)))
                .ToDictionary(t => t.Item1, t => t.Item2);

            _iteDefinitions = exprDefsList
                .OfType<IfThenElseDef<TKind, T>>()
                .SelectMany(ite => new[] { (ite.IfToken, ite), (ite.ThenToken, ite), (ite.ElseToken, ite) })
                .ToDictionary(t => t.Item1, t => t.Item2);

            // We allow same token to represent infix/prefix operator, set and if-then-else tokens must be used only once
            var tokenOverlap =
                _setDefinitions.Keys.Concat(_iteDefinitions.Keys)
                    .Concat(_infixOperators.Keys.Union(_prefixOperators.Keys))
                    .GroupBy(t => t)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToArray();
            if ( tokenOverlap.Any() ) {
                throw new ArgumentException(
                    $"Tokens used in sets/if-then-else expression definitions cannot be used more than once: {string.Join(",", tokenOverlap)}");
            }
        }

        /// <summary>
        /// Parse expression
        /// </summary>
        public TokenListParserResult<TKind, T> Parse(TokenList<TKind> input)
        {
            var operatorStack = new Stack<(TKind token, IExprDef<TKind> exprDef)>();
            var operandStack = new Stack<T>();

            var next = input.ConsumeToken();
            var expectedNext = new ExpectedNext {
                ExpectOperand = true,
                OpenSet = new Stack<ExprSetDef<TKind, T>>(),
                OpenIte = new Stack<(TKind, IfThenElseDef<TKind, T>)>()
            };

            do {
                var nextResult = ReadNext(ref next, ref expectedNext);

                switch ( nextResult.ResultType ) {

                    case ResultType.Error:
                        return nextResult.ParseResult;

                    case ResultType.EndOfInput:
                        ReduceWhile(operatorStack, operandStack, oprStack => oprStack.Count > 0);

                        return TokenListParserResult.Value(operandStack.Pop(), input, next.Location);

                    case ResultType.Operand:
                        operandStack.Push(nextResult.ParseResult.Value);
                        break;

                    case ResultType.Operator:
                        ReduceWhile(operatorStack, operandStack,
                            oprStack => ShiftOrReduce(oprStack, nextResult.ExprDef) == ShiftReduce.Reduce);
                        Shift(operatorStack, nextResult);

                        break;

                    case ResultType.Set:
                        var setDef = (ExprSetDef<TKind, T>) nextResult.ExprDef;
                        // Start of new set
                        if ( setDef.SetStart.Equals(nextResult.Token) ) {
                            Shift(operatorStack, nextResult);
                        }
                        // Finding a separator, we reduce last element in set
                        else if ( setDef.Separators.Contains(nextResult.Token) ) {
                            ReduceWhile(operatorStack, operandStack,
                                oprStack => !setDef.IsSeparatorOrStart(oprStack.Peek().token));
                            Shift(operatorStack, nextResult);
                        }
                        // End of set
                        else {
                            // If we get the empty token, then the set is empty (and we have not gotten a start operator)
                            if ( nextResult.Token.Equals(EmptyToken) ) {
                                operandStack.Push(setDef.ResultBuilder(new T[0], new TKind[0]));
                                break;
                            }

                            // Set is not empty, reduce last operand
                            ReduceWhile(operatorStack, operandStack,
                                oprStack => !setDef.IsSeparatorOrStart(oprStack.Peek().token));

                            var operands = new List<T>();
                            var separators = new List<TKind>();

                            while ( !setDef.SetStart.Equals(operatorStack.Peek().token) ) {
                                operands.Add(operandStack.Pop());
                                separators.Add(operatorStack.Pop().token);
                            }

                            // There is one more operand than separator.
                            operands.Add(operandStack.Pop());

                            // Pop set start
                            PopExpected(operatorStack, setDef.SetStart);

                            operands.Reverse();
                            separators.Reverse();

                            // Check that parsed operands and separators are valid
                            var error = setDef.CanBuild(operands, separators);
                            if ( error != null ) {
                                return TokenListParserResult.Empty<TKind, T>(next.Location, error);
                            }

                            operandStack.Push(setDef.ResultBuilder(operands, separators));
                        }

                        break;

                    case ResultType.IfThenElse:
                        var iteDef = (IfThenElseDef<TKind, T>) nextResult.ExprDef;
                        // When we get THEN / ELSE we reduce as much as possible (to next IF / THEN)
                        if ( iteDef.ThenToken.Equals(nextResult.Token) ) {
                            ReduceWhile(operatorStack, operandStack,
                                oprStack => !iteDef.IfToken.Equals(oprStack.Peek().token));
                        }
                        else if ( iteDef.ElseToken.Equals(nextResult.Token) ) {
                            ReduceWhile(operatorStack, operandStack,
                                oprStack => !iteDef.ThenToken.Equals(oprStack.Peek().token));
                        }

                        Shift(operatorStack, nextResult);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            } while ( true );
        }

        /// <summary>
        /// Keep reducing while <paramref name="condition"/> is satisfied
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ReduceWhile(
            Stack<(TKind token, IExprDef<TKind> exprDef)> operatorStack,
            Stack<T> operandStack,
            Func<Stack<(TKind token, IExprDef<TKind> exprDef)>, bool> condition)
        {

            while ( condition(operatorStack) ) {
                Reduce(operatorStack, operandStack);
            }
        }

        /// <summary>
        /// Reduce top-most operator on <paramref name="operatorStack"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Reduce(Stack<(TKind token, IExprDef<TKind> exprDef)> operatorStack, Stack<T> operandStack)
        {
            var topOpr = operatorStack.Pop();

            // Infix operator
            if ( topOpr.exprDef is InfixOprDef<TKind, T, U> infixOpr ) {
                var right = operandStack.Pop();
                var left = operandStack.Pop();

                operandStack.Push(infixOpr.ResultBuilder(infixOpr.TokenToOperator(topOpr.token), left, right));
                return;
            }

            // Prefix operator
            if ( topOpr.exprDef is PrefixOprDef<TKind, T, U> prefixOpr ) {
                operandStack.Push(prefixOpr.ResultBuilder(prefixOpr.TokenToOperator(topOpr.token), operandStack.Pop()));
                return;
            }

            // If-Then-Else
            if ( topOpr.exprDef is IfThenElseDef<TKind, T> iteDef ) {
                T whenFalse = default(T);
                bool haveElsePart = false;

                if ( topOpr.token.Equals(iteDef.ElseToken) ) {
                    haveElsePart = true;
                    whenFalse = operandStack.Pop();

                    PopExpected(operatorStack, iteDef.ThenToken, "Operand for THEN part has not been reduced");
                }

                PopExpected(operatorStack, iteDef.IfToken, "Operand for IF part has not been reduced");

                var whenTrue = operandStack.Pop();
                var condition = operandStack.Pop();

                operandStack.Push(haveElsePart ?
                    iteDef.BuildIfThenElseResult(condition, whenTrue, whenFalse) :
                    iteDef.BuildIfThenResult(condition, whenTrue));
                return;
            }

            throw new InvalidOperationException(
                "Reduce is called when top of operator stack does not contain a prefix/infix/if-then-else operator");
        }

        /// <summary>
        /// Shift operator to <paramref name="operatorStack"/>
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Shift(Stack<(TKind, IExprDef<TKind>)> operatorStack, NextResult nextResult)
        {
            operatorStack.Push((nextResult.Token, nextResult.ExprDef));
        }

        /// <summary>
        /// Figure out if we should shift or reduce, according to operators on stack
        /// and last operator read.
        /// </summary>
        private ShiftReduce ShiftOrReduce(Stack<(TKind token, IExprDef<TKind> exprDef)> operatorStack,
            IExprDef<TKind> exprDef)
        {
            if ( operatorStack.Count == 0 ) {
                return ShiftReduce.Shift;
            }

            var top = operatorStack.Peek();

            // Operator have same precedence. Left associative infix operators are reduced,
            // all others are shifted
            if ( top.exprDef.Precedence == exprDef.Precedence ) {
                return ( top.exprDef as InfixOprDef<TKind, T, U> )?.Associativity == InfixOprAssociativity.Left ?
                    ShiftReduce.Reduce : ShiftReduce.Shift;
            }

            return top.exprDef.Precedence > exprDef.Precedence ? ShiftReduce.Reduce : ShiftReduce.Shift;
        }

        /// <summary>
        /// <para>
        /// Read next value from input. Both the <paramref name="next"/> and
        /// <paramref name="expectedNext"/> are updated to reflect that we have
        /// consumed tokens from input.
        /// </para>
        /// <para>This method ensures, that all operators and operands put on the stack
        /// indeed follow the grammar. This way, the main parse loop only need to handle
        /// when to shift/reduce.</para>
        /// </summary>
        private NextResult ReadNext(
            ref TokenListParserResult<TKind, Token<TKind>> next,
            ref ExpectedNext expectedNext)
        {

            // We accept the following
            // EXPR            ::= (OPERAND | SET) [INFIX-OPR EXPR]
            //                   | PREFIX-OPR EXPR
            //                   | IF EXPR THEN EXPR [ELSE EXPR]
            // SET             ::= SET_START [EXPR (SET_SEPARATOR EXPR)*] SET_END
            //
            // There are two main cases to handle
            // 1: We expect an operand. In this case
            //    - Accept prefix operator
            //    - Accept start of new set
            //    - Accept IF in If-Then-Else
            //    - Accept operand
            //    - Do not accept end-of-input
            //    - Do not accept THEN / ELSE in If-Then-Else
            // 2: We do not expect operand. In this case
            //    - Accept infix operator
            //    - Accept separator in current set / end of current set
            //    - Accept end-of-input (given there are no open sets or open IF)
            //    - Accept THEN (given there is an open IF)
            //    - Accept ELSE (given there is an open THEN)
            //    - Do not accept prefix operator
            //    - Do not accept start of new set
            //    - Do not accept IF in If-Then-Else

            return expectedNext.ExpectOperand ?
                ReadNextOperand(ref next, ref expectedNext) :
                ReadNextOperator(ref next, ref expectedNext);
        }

        private void PopExpected(Stack<(TKind token, IExprDef<TKind>)> stack, TKind expectedToken, string msg = null)
        {
            if ( stack.Count == 0 ) {
                throw new InvalidOperationException($"Operator stack is empty, expected {expectedToken}. {msg}");
            }

            if ( !expectedToken.Equals(stack.Pop().token) ) {
                throw new InvalidOperationException(
                    $"Invalid token at top of operator stack, expecting {expectedToken}. {msg}");
            }
        }

        private NextResult ReadNextOperand(
            ref TokenListParserResult<TKind, Token<TKind>> next,
            ref ExpectedNext expectedNext)
        {

            var token = next.HasValue ? next.Value.Kind : EmptyToken;

            // Accept prefix operator
            if ( next.HasValue &&
                 _prefixOperators.TryGetValue(next.Value.Kind, out var prefixOprDef) ) {
                next = next.Remainder.ConsumeToken();
                return new NextResult(ResultType.Operator, token, prefixOprDef, EmptyParseResult);
            }

            // Accept start of new set
            if ( next.HasValue &&
                 _setDefinitions.TryGetValue(next.Value.Kind, out var setDef) &&
                 setDef.SetStart.Equals(next.Value.Kind) ) {
                next = next.Remainder.ConsumeToken();

                // Check if set is empty
                if ( next.HasValue && setDef.SetEnd.Equals(next.Value.Kind) && setDef.AllowEmptySet ) {
                    expectedNext.ExpectOperand = false;
                    next = next.Remainder.ConsumeToken();
                    // Return of the empty token means that set is empty
                    return new NextResult(ResultType.Set, EmptyToken, setDef, EmptyParseResult);
                }

                expectedNext.OpenSet.Push(setDef);

                return new NextResult(ResultType.Set, token, setDef, EmptyParseResult);
            }

            // Accept IF
            if ( next.HasValue &&
                 _iteDefinitions.TryGetValue(next.Value.Kind, out var iteDef) &&
                 iteDef.IfToken.Equals(next.Value.Kind) ) {
                expectedNext.OpenIte.Push((iteDef.IfToken, iteDef));
                next = next.Remainder.ConsumeToken();

                return new NextResult(ResultType.IfThenElse, token, iteDef, EmptyParseResult);
            }

            // Accept operand
            var operand = _operandParser(next.Location);
            expectedNext.ExpectOperand = false;
            next = operand.Remainder.ConsumeToken();

            return new NextResult(operand.HasValue ? ResultType.Operand : ResultType.Error, EmptyToken, EmptyExprDef,
                operand);
        }

        private NextResult ReadNextOperator(
            ref TokenListParserResult<TKind, Token<TKind>> next,
            ref ExpectedNext expectedNext)
        {

            var openSet = expectedNext.OpenSet.Count > 0 ? expectedNext.OpenSet.Peek() : null;
            var openIte = expectedNext.OpenIte.Count > 0 ? expectedNext.OpenIte.Peek() : (EmptyToken, null);

            var token = next.HasValue ? next.Value.Kind : EmptyToken;

            // Accept infix operator
            if ( next.HasValue &&
                 _infixOperators.TryGetValue(next.Value.Kind, out var infixOprDef) ) {
                next = next.Remainder.ConsumeToken();
                expectedNext.ExpectOperand = true;

                return new NextResult(ResultType.Operator, token, infixOprDef, EmptyParseResult);
            }

            // Accept separator / end of current open set
            if ( next.HasValue &&
                 openSet?.IsSeparatorOrEnd(token) == true ) {
                next = next.Remainder.ConsumeToken();

                if ( openSet.SetEnd.Equals(token) ) {
                    expectedNext.OpenSet.Pop();
                }
                else {
                    // After separator, there must be an operand
                    expectedNext.ExpectOperand = true;
                }

                return new NextResult(ResultType.Set, token, openSet, EmptyParseResult);
            }

            // If we get THEN when another THEN is open, then we can close the open THEN
            if ( next.HasValue &&
                 openIte.iteDef?.ThenToken.Equals(token) == true &&
                 openIte.iteDef.ThenToken.Equals(openIte.token) ) {
                expectedNext.OpenIte.Pop();
                openIte = expectedNext.OpenIte.Count > 0 ? expectedNext.OpenIte.Peek() : (EmptyToken, null);
            }

            // Accept THEN of current open IF
            if ( next.HasValue &&
                 openIte.iteDef?.ThenToken.Equals(token) == true &&
                 openIte.iteDef.IfToken.Equals(openIte.token) ) {
                next = next.Remainder.ConsumeToken();
                expectedNext.ExpectOperand = true;
                // Replace open IF with open THEN
                expectedNext.OpenIte.Pop();
                expectedNext.OpenIte.Push((openIte.iteDef.ThenToken, openIte.iteDef));
                return new NextResult(ResultType.IfThenElse, token, openIte.iteDef, EmptyParseResult);
            }

            // Accept ELSE of current open IF-THEN
            if ( next.HasValue &&
                 openIte.iteDef?.ElseToken.Equals(token) == true &&
                 openIte.iteDef.ThenToken.Equals(openIte.token) ) {
                next = next.Remainder.ConsumeToken();
                // Close matching open IF-THEN
                if ( !openIte.iteDef.ThenToken.Equals(expectedNext.OpenIte.Pop().token) ) {
                    throw new InvalidOperationException("ELSE did not have matching IF-THEN on ite stack");
                }

                expectedNext.ExpectOperand = true;
                return new NextResult(ResultType.IfThenElse, token, openIte.iteDef, EmptyParseResult);
            }

            // Ensure we have closed all open sets
            if ( openSet != null ) {
                return new NextResult(ResultType.Error, EmptyToken, EmptyExprDef,
                    TokenListParserResult.Empty<TKind, T>(next.Location,
                        new[] { Presentation.FormatExpectation(openSet.SetEnd) }));
            }

            // Ensure we have closed all open IF (no missing THEN). Consider case where we have "IF IF x THEN" (outer IF not closed)
            while ( expectedNext.OpenIte.Count > 0 ) {
                openIte = expectedNext.OpenIte.Pop();
                // If we have open IF, report missing THEN
                if ( openIte.token.Equals(openIte.iteDef.IfToken) ) {
                    return new NextResult(ResultType.Error, EmptyToken, EmptyExprDef,
                        TokenListParserResult.Empty<TKind, T>(next.Location,
                            new[] { Presentation.FormatExpectation(openIte.iteDef.ThenToken) }));
                }
            }

            // All done
            return new NextResult(ResultType.EndOfInput, EmptyToken, EmptyExprDef, EmptyParseResult);
        }
    }
}

// Copyright 2016 Datalust, Superpower Contributors, Sprache Contributors
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at  
//
//     http://www.apache.org/licenses/LICENSE-2.0  
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using Superpower.Display;
using Superpower.Model;
using Superpower.Util;

namespace Superpower
{
    /// <summary>
    /// Functions that construct more complex parsers by combining simpler ones.
    /// </summary>
    public static class Combinators
    {
        /// <summary>
        /// Apply the character parser <paramref name="valueParser"/> to the span represented by the parsed token.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="valueParser">A function that determines which character parser to apply.</param>
        /// <returns>A parser that returns the result of parsing the token value.</returns>
        public static TokenListParser<TKind, U> Apply<TKind, U>(this TokenListParser<TKind, Token<TKind>> parser, Func<Token<TKind>, TextParser<U>> valueParser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (valueParser == null) throw new ArgumentNullException(nameof(valueParser));
            return input =>
            {
                var rt = parser(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, Token<TKind>, U>(rt);

                var uParser = valueParser(rt.Value);
                var uResult = uParser.AtEnd()(rt.Value.Span);
                if (!uResult.HasValue)
                {
                    var message = $"invalid {Presentation.FormatExpectation(rt.Value.Kind)}, {uResult.FormatErrorMessageFragment()}";
                    return new TokenListParserResult<TKind, U>(input, uResult.Remainder.Position, message, null);
                }

                return TokenListParserResult.Value(uResult.Value, rt.Location, rt.Remainder);
            };
        }

        /// <summary>
        /// Apply the character parser <paramref name="valueParser"/> to the span represented by the parsed token.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="valueParser">A character parser to apply.</param>
        /// <returns>A parser that returns the result of parsing the token value.</returns>
        public static TokenListParser<TKind, U> Apply<TKind, U>(this TokenListParser<TKind, Token<TKind>> parser, TextParser<U> valueParser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (valueParser == null) throw new ArgumentNullException(nameof(valueParser));

            return parser.Apply(rt => valueParser);
        }

        /// <summary>
        /// Construct a parser that succeeds only if the source is at the end of input.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T> AtEnd<T>(this TextParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);
                if (!result.HasValue)
                    return result;

                if (result.Remainder.IsAtEnd)
                    return result;

                return Result.Empty<T>(result.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that succeeds only if the source is at the end of input.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T> AtEnd<TKind, T>(this TokenListParser<TKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);
                if (!result.HasValue)
                    return result;

                if (result.Remainder.IsAtEnd)
                    return result;

                return TokenListParserResult.Empty<TKind, T>(result.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that matches one or more instances of applying <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T[]> AtLeastOnce<TKind, T>(this TokenListParser<TKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            return parser.Then(first => parser.Many().Select(rest => ArrayEnumerable.Cons(first, rest)));
        }

        /// <summary>
        /// Construct a parser that matches one or more instances of applying <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T[]> AtLeastOnce<T>(this TextParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            return parser.Then(first => parser.Many().Select(rest => ArrayEnumerable.Cons(first, rest)));
        }

        /// <summary>
        /// Construct a parser that matches one or more instances of applying <paramref name="parser"/>, delimited by <paramref name="delimiter"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="delimiter">The parser that matches the delimiters.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T[]> AtLeastOnceDelimitedBy<TKind, T, U>(this TokenListParser<TKind, T> parser, TokenListParser<TKind, U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return parser.Then(first => delimiter.IgnoreThen(parser).Many().Select(rest => ArrayEnumerable.Cons(first, rest)));
        }

        /// <summary>
        /// Construct a parser that matches one or more instances of applying <paramref name="parser"/>, delimited by <paramref name="delimiter"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="delimiter">The parser that matches the delimiters.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T[]> AtLeastOnceDelimitedBy<T, U>(this TextParser<T> parser, TextParser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return parser.Then(first => delimiter.IgnoreThen(parser).Many().Select(rest => ArrayEnumerable.Cons(first, rest)));
        }

        /// <summary>
        /// Construct a parser that matches <paramref name="first"/>, discards the resulting value, then returns the result of <paramref name="second"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, U> IgnoreThen<TKind, T, U>(this TokenListParser<TKind, T> first, TokenListParser<TKind, U> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, T, U>(rt);

                var ru = second(rt.Remainder);
                if (!ru.HasValue)
                    return ru;

                return TokenListParserResult.Value(ru.Value, input, ru.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that matches <paramref name="first"/>, discards the resulting value, then returns the result of <paramref name="second"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<U> IgnoreThen<T, U>(this TextParser<T> first, TextParser<U> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return Result.CastEmpty<T, U>(rt);

                var ru = second(rt.Remainder);
                if (!ru.HasValue)
                    return ru;

                return Result.Value(ru.Value, input, ru.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that matches <paramref name="parser"/> zero or more times.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        /// <remarks>Many will fail if any item partially matches this. To modify this behavior use <see cref="Try{TKind,T}(TokenListParser{TKind,T})"/>.</remarks>
        public static TokenListParser<TKind, T[]> Many<TKind, T>(this TokenListParser<TKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = new List<T>();
                var @from = input;
                var r = parser(input);
                while (r.HasValue)
                {
                    if (@from == r.Remainder) // Broken parser, not a failed parsing.
                        throw new ParseException($"Many() cannot be applied to zero-width parsers; value {r.Value} at position {r.Location.Position}.");

                    result.Add(r.Value);
                    @from = r.Remainder;
                    r = parser(r.Remainder);
                }

                if (r.IsPartial(@from))
                    return TokenListParserResult.CastEmpty<TKind, T, T[]>(r);

                return TokenListParserResult.Value(result.ToArray(), input, r.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that matches <paramref name="parser"/> zero or more times.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        /// <remarks>Many will fail if any item partially matches this. To modify this behavior use <see cref="Try{T}(TextParser{T})"/>.</remarks>
        public static TextParser<T[]> Many<T>(this TextParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = new List<T>();
                var @from = input;
                var r = parser(input);
                while (r.HasValue)
                {
                    if (@from == r.Remainder) // Broken parser, not a failed parsing.
                        throw new ParseException($"Many() cannot be applied to zero-width parsers; value {r.Value} at position {r.Location.Position}.");

                    result.Add(r.Value);

                    @from = r.Remainder;
                    r = parser(r.Remainder);
                }

                if (r.IsPartial(@from))
                    return Result.CastEmpty<T, T[]>(r);

                return Result.Value(result.ToArray(), input, r.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that matches <paramref name="parser"/> zero or more times, delimited by <paramref name="delimiter"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="delimiter">The parser that matches the delimiters.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T[]> ManyDelimitedBy<TKind, T, U>(this TokenListParser<TKind, T> parser, TokenListParser<TKind, U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return parser.Then(first => delimiter.IgnoreThen(parser).Many().Select(rest => ArrayEnumerable.Cons(first, rest)))
                .OptionalOrDefault(new T[0]);
        }

        /// <summary>
        /// Construct a parser that matches <paramref name="parser"/> zero or more times, delimited by <paramref name="delimiter"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="delimiter">The parser that matches the delimiters.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T[]> ManyDelimitedBy<T, U>(this TextParser<T> parser, TextParser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return parser.Then(first => delimiter.IgnoreThen(parser).Many().Select(rest => ArrayEnumerable.Cons(first, rest)))
                .OptionalOrDefault(new T[0]);
        }

        /// <summary>
        /// Construct a parser that fails with error message <paramref name="errorMessage"/> when <paramref name="parser"/> fails.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T> Message<TKind, T>(this TokenListParser<TKind, T> parser, string errorMessage)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue)
                    return result;

                return TokenListParserResult.Empty<TKind, T>(result.Remainder, result.ErrorPosition, errorMessage);
            };
        }

        /// <summary>
        /// Construct a parser that fails with error message <paramref name="errorMessage"/> when <paramref name="parser"/> fails.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T> Message<T>(this TextParser<T> parser, string errorMessage)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue)
                    return result;

                return Result.Empty<T>(result.Remainder, errorMessage);
            };
        }

        /// <summary>
        /// Construct a parser that returns <paramref name="name"/> as its "expectation" if <paramref name="parser"/> fails.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="name">The name given to <paramref name="parser"/>.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T> Named<TKind, T>(this TokenListParser<TKind, T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue || result.Remainder != input)
                    return result;

                // result.IsSubTokenError?
                if (result.ErrorPosition.HasValue)
                {
                    return TokenListParserResult.Empty<TKind, T>(result.Remainder, result.ErrorPosition, result.FormatErrorMessageFragment());
                }

                return TokenListParserResult.Empty<TKind, T>(result.Remainder, new[] { name });
            };
        }

        /// <summary>
        /// Construct a parser that returns <paramref name="name"/> as its "expectation" if <paramref name="parser"/> fails.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="name">The name given to <paramref name="parser"/>.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T> Named<T>(this TextParser<T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue || result.Remainder != input)
                    return result;

                return Result.Empty<T>(result.Remainder, new[] { name });
            };
        }

        /// <summary>
        /// Construct a parser that matches zero or one instance of <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T?> Optional<TKind, T>(this TokenListParser<TKind, T> parser)
            where T : struct
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Select(t => (T?)t).Or(Parse.Return<TKind, T?>(null));
        }

        /// <summary>
        /// Construct a parser that matches zero or one instance of <paramref name="parser"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T?> Optional<T>(this TextParser<T> parser)
            where T : struct
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Select(t => (T?)t).Or(Parse.Return<T?>(null));
        }

        /// <summary>
        /// Construct a parser that matches zero or one instance of <paramref name="parser"/>, returning <paramref name="defaultValue"/> when
        /// no match is possible.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T> OptionalOrDefault<TKind, T>(this TokenListParser<TKind, T> parser, T defaultValue = default(T))
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Or(Parse.Return<TKind, T>(defaultValue));
        }

        /// <summary>
        /// Construct a parser that matches zero or one instance of <paramref name="parser"/>, returning <paramref name="defaultValue"/> when
        /// no match is possible.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T> OptionalOrDefault<T>(this TextParser<T> parser, T defaultValue = default(T))
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Or(Parse.Return(defaultValue));
        }

        /// <summary>
        /// Construct a parser that tries first the <paramref name="lhs"/> parser, and if it fails, applies <paramref name="rhs"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="lhs">The first parser to try.</param>
        /// <param name="rhs">The second parser to try.</param>
        /// <returns>The resulting parser.</returns>
        /// <remarks>Or will fail if the first item partially matches this. To modify this behavior use <see cref="Try{TKind,T}(TokenListParser{TKind,T})"/>.</remarks>
        public static TokenListParser<TKind, T> Or<TKind, T>(this TokenListParser<TKind, T> lhs, TokenListParser<TKind, T> rhs)
        {
            if (lhs == null) throw new ArgumentNullException(nameof(lhs));
            if (rhs == null) throw new ArgumentNullException(nameof(rhs));

            return input =>
            {
                var first = lhs(input);
                if (first.HasValue || first.IsPartial(input))
                    return first;

                var second = rhs(input);
                if (second.HasValue)
                    return second;

                return TokenListParserResult.CombineEmpty(first, second);
            };
        }

        /// <summary>
        /// Construct a parser that tries first the <paramref name="lhs"/> parser, and if it fails, applies <paramref name="rhs"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="lhs">The first parser to try.</param>
        /// <param name="rhs">The second parser to try.</param>
        /// <returns>The resulting parser.</returns>
        /// <remarks>Or will fail if the first item partially matches this. To modify this behavior use <see cref="Try{T}(TextParser{T})"/>.</remarks>
        public static TextParser<T> Or<T>(this TextParser<T> lhs, TextParser<T> rhs)
        {
            if (lhs == null) throw new ArgumentNullException(nameof(lhs));
            if (rhs == null) throw new ArgumentNullException(nameof(rhs));

            return input =>
            {
                var first = lhs(input);
                if (first.HasValue || first.IsPartial(input))
                    return first;

                var second = rhs(input);
                if (second.HasValue)
                    return second;

                return Result.CombineEmpty(first, second);
            };
        }

        /// <summary>
        /// Construct a parser that takes the result of <paramref name="parser"/> and converts it value using <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="selector">A mapping from the first result to the second.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, U> Select<TKind, T, U>(this TokenListParser<TKind, T> parser, Func<T, U> selector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return parser.Then(rt => Parse.Return<TKind, U>(selector(rt)));
        }

        /// <summary>
        /// Construct a parser that takes the result of <paramref name="parser"/> and converts it value using <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="selector">A mapping from the first result to the second.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<U> Select<T, U>(this TextParser<T> parser, Func<T, U> selector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return parser.Then(rt => Parse.Return(selector(rt)));
        }

        /// <summary>
        /// The LINQ query comprehension pattern.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="selector">A mapping from the first result to the second parser.</param>
        /// <param name="projector">Function mapping the results of the first two parsers onto the final result.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<V> SelectMany<T, U, V>(
            this TextParser<T> parser,
            Func<T, TextParser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        /// <summary>
        /// The LINQ query comprehension pattern.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="selector">A mapping from the first result to the second parser.</param>
        /// <param name="projector">Function mapping the results of the first two parsers onto the final result.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, V> SelectMany<TKind, T, U, V>(
            this TokenListParser<TKind, T> parser,
            Func<T, TokenListParser<TKind, U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        /// <summary>
        /// Construct a parser that applies <paramref name="first"/>, provides the value to <paramref name="second"/> and returns the result.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, U> Then<TKind, T, U>(this TokenListParser<TKind, T> first, Func<T, TokenListParser<TKind, U>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return TokenListParserResult.CastEmpty<TKind, T, U>(rt);

                var ru = second(rt.Value)(rt.Remainder);
                if (!ru.HasValue)
                    return ru;

                return TokenListParserResult.Value(ru.Value, input, ru.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that applies <paramref name="first"/>, provides the value to <paramref name="second"/> and returns the result.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="first">The first parser.</param>
        /// <param name="second">The second parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<U> Then<T, U>(this TextParser<T> first, Func<T, TextParser<U>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return Result.CastEmpty<T, U>(rt);

                var ru = second(rt.Value)(rt.Remainder);
                if (!ru.HasValue)
                    return ru;

                return Result.Value(ru.Value, input, ru.Remainder);
            };
        }

        /// <summary>
        /// Construct a parser that tries one parser, and backtracks if unsuccessful so that no input
        /// appears to have been consumed by subsequent checks against the result.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T> Try<TKind, T>(this TokenListParser<TKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var rt = parser(input);
                if (rt.HasValue)
                    return rt;

                // Need to preserve expecations iff there was no match.
                return TokenListParserResult.Empty<TKind, T>(input);
            };
        }

        /// <summary>
        /// Construct a parser that tries one parser, and backtracks if unsuccessful so that no input
        /// appears to have been consumed by subsequent checks against the result.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T> Try<T>(this TextParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var rt = parser(input);
                if (rt.HasValue)
                    return rt;

                // Need to preserve expecations iff there was no match.
                return Result.Empty<T>(input);
            };
        }

        /// <summary>
        /// Construct a parser that applies the first, and returns <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, U> Value<TKind, T, U>(this TokenListParser<TKind, T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.IgnoreThen(Parse.Return<TKind, U>(value));
        }

        /// <summary>
        /// Construct a parser that applies the first, and returns <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <typeparam name="U">The type of the resulting value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="value">The value to return.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<U> Value<T, U>(this TextParser<T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.IgnoreThen(Parse.Return(value));
        }

        /// <summary>
        /// Construct a parser that evaluates the result of a previous parser and fails if <paramref name="predicate"/> returns false
        /// for the result.
        /// </summary>
        /// <typeparam name="TKind">The kind of the tokens being parsed.</typeparam>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="predicate">The predicate to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TokenListParser<TKind, T> Where<TKind, T>(this TokenListParser<TKind, T> parser, Func<T, bool> predicate)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return input =>
            {
                var rt = parser(input);
                if (!rt.HasValue)
                    return rt;

                if (predicate(rt.Value))
                    return rt;

                return TokenListParserResult.Empty<TKind, T>(input, "unsatisfied condition");
            };
        }

        /// <summary>
        /// Construct a parser that evaluates the result of a previous parser and fails if <paramref name="predicate"/> returns false
        /// for the result.
        /// </summary>
        /// <typeparam name="T">The type of value being parsed.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="predicate">The predicate to apply.</param>
        /// <returns>The resulting parser.</returns>
        public static TextParser<T> Where<T>(this TextParser<T> parser, Func<T, bool> predicate)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return input =>
            {
                var rt = parser(input);
                if (!rt.HasValue)
                    return rt;

                if (predicate(rt.Value))
                    return rt;

                return Result.Empty<T>(input, "unsatisfied condition");
            };
        }
    }
}

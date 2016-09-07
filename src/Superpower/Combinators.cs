using System;
using System.Collections.Generic;
using System.Linq;
using Superpower.Model;
using Superpower.Util;

namespace Superpower
{
    public static class Combinators
    {
        public static TokenParser<TTokenKind, U> Apply<TTokenKind, U>(this TokenParser<TTokenKind, Token<TTokenKind>> parser, Func<Token<TTokenKind>, CharParser<U>> valueParser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (valueParser == null) throw new ArgumentNullException(nameof(valueParser));
            return input =>
            {
                var rt = parser(input);
                if (!rt.HasValue)
                    return TokenResult.CastEmpty<TTokenKind, Token<TTokenKind>, U>(rt);

                var uParser = valueParser(rt.Value);
                var uResult = uParser.AtEnd()(rt.Value.Span);
                if (!uResult.HasValue)
                {
                    var message = $"invalid {Presentation.FormatKind(rt.Value.Kind)}, {uResult.FormatErrorMessageFragment()}";
                    return new TokenResult<TTokenKind, U>(input, uResult.Remainder.Position, message, null);
                }

                return TokenResult.Value(uResult.Value, rt.Location, rt.Remainder);
            };
        }

        public static TokenParser<TTokenKind, U> Apply<TTokenKind, U>(this TokenParser<TTokenKind, Token<TTokenKind>> parser, CharParser<U> valueParser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (valueParser == null) throw new ArgumentNullException(nameof(valueParser));

            return parser.Apply(rt => valueParser);
        }

        public static CharParser<T> AtEnd<T>(this CharParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);
                if (!result.HasValue)
                    return result;

                if (result.Remainder.IsAtEnd)
                    return result;

                return CharResult.Empty<T>(result.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T> AtEnd<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var result = parser(input);
                if (!result.HasValue)
                    return result;

                if (result.Remainder.IsAtEnd)
                    return result;

                return TokenResult.Empty<TTokenKind, T>(result.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T[]> AtLeastOnce<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            return parser.Then(first => parser.Many().Select(rest => new[] { first }.Concat(rest).ToArray()));
        }

        public static CharParser<T[]> AtLeastOnce<T>(this CharParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            return parser.Then(first => parser.Many().Select(rest => new[] { first }.Concat(rest).ToArray()));
        }

        public static TokenParser<TTokenKind, U> IgnoreThen<TTokenKind, T, U>(this TokenParser<TTokenKind, T> first, TokenParser<TTokenKind, U> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return TokenResult.CastEmpty<TTokenKind, T, U>(rt);

                return second(rt.Remainder);
            };
        }

        public static CharParser<U> IgnoreThen<T, U>(this CharParser<T> first, CharParser<U> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return CharResult.CastEmpty<T, U>(rt);

                return second(rt.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T[]> Many<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
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
                    return TokenResult.CastEmpty<TTokenKind, T, T[]>(r);

                return TokenResult.Value(result.ToArray(), input, r.Remainder);
            };
        }

        public static CharParser<T[]> Many<T>(this CharParser<T> parser)
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
                    return CharResult.CastEmpty<T, T[]>(r);

                return CharResult.Value(result.ToArray(), input, r.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T[]> ManyDelimitedBy<TTokenKind, T, U>(this TokenParser<TTokenKind, T> parser, TokenParser<TTokenKind, U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return parser.Then(first => delimiter.IgnoreThen(parser).Many().Select(rest =>
            {
                var all = new T[rest.Length + 1];
                all[0] = first;
                Array.Copy(rest, 0, all, 1, rest.Length);
                return all;
            }));
        }

        public static CharParser<T[]> ManyDelimitedBy<T, U>(this CharParser<T> parser, CharParser<U> delimiter)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (delimiter == null) throw new ArgumentNullException(nameof(delimiter));

            return parser.Then(first => delimiter.IgnoreThen(parser).Many().Select(rest =>
            {
                var all = new T[rest.Length + 1];
                all[0] = first;
                Array.Copy(rest, 0, all, 1, rest.Length);
                return all;
            }));
        }

        public static TokenParser<TTokenKind, T> Message<TTokenKind, T>(this TokenParser<TTokenKind, T> parser, string errorMessage)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue)
                    return result;

                return TokenResult.Empty<TTokenKind, T>(result.Remainder, result.ErrorPosition, errorMessage);
            };
        }

        public static CharParser<T> Message<T>(this CharParser<T> parser, string errorMessage)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (errorMessage == null) throw new ArgumentNullException(nameof(errorMessage));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue)
                    return result;

                return CharResult.Empty<T>(result.Remainder, errorMessage);
            };
        }

        public static TokenParser<TTokenKind, T> Named<TTokenKind, T>(this TokenParser<TTokenKind, T> parser, string name)
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
                    return TokenResult.Empty<TTokenKind, T>(result.Remainder, result.ErrorPosition, result.FormatErrorMessageFragment());
                }

                return TokenResult.Empty<TTokenKind, T>(result.Remainder, new[] { name });
            };
        }

        public static CharParser<T> Named<T>(this CharParser<T> parser, string name)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (name == null) throw new ArgumentNullException(nameof(name));

            return input =>
            {
                var result = parser(input);
                if (result.HasValue || result.Remainder != input)
                    return result;

                return CharResult.Empty<T>(result.Remainder, new[] { name });
            };
        }

        public static TokenParser<TTokenKind, T?> Optional<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
            where T : struct
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Select(t => (T?)t).Or(Parse.Return<TTokenKind, T?>(null));
        }

        public static CharParser<T?> Optional<T>(this CharParser<T> parser)
            where T : struct
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Select(t => (T?)t).Or(Parse.Return<T?>(null));
        }

        public static TokenParser<TTokenKind, T> OptionalOrDefault<TTokenKind, T>(this TokenParser<TTokenKind, T> parser, T defaultValue = default(T))
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Or(Parse.Return<TTokenKind, T>(defaultValue));
        }

        public static CharParser<T> OptionalOrDefault<T>(this CharParser<T> parser, T defaultValue = default(T))
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.Or(Parse.Return(defaultValue));
        }

        public static TokenParser<TTokenKind, T> Or<TTokenKind, T>(this TokenParser<TTokenKind, T> lhs, TokenParser<TTokenKind, T> rhs)
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

                return TokenResult.CombineEmpty(first, second);
            };
        }

        public static CharParser<T> Or<T>(this CharParser<T> lhs, CharParser<T> rhs)
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

                return CharResult.CombineEmpty(first, second);
            };
        }

        public static TokenParser<TTokenKind, U> Select<TTokenKind, T, U>(this TokenParser<TTokenKind, T> parser, Func<T, U> selector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return parser.Then(rt => Parse.Return<TTokenKind, U>(selector(rt)));
        }

        public static CharParser<U> Select<T, U>(this CharParser<T> parser, Func<T, U> selector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return parser.Then(rt => Parse.Return(selector(rt)));
        }

        public static CharParser<V> SelectMany<T, U, V>(
            this CharParser<T> parser,
            Func<T, CharParser<U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        public static TokenParser<TTokenKind, V> SelectMany<TTokenKind, T, U, V>(
            this TokenParser<TTokenKind, T> parser,
            Func<T, TokenParser<TTokenKind, U>> selector,
            Func<T, U, V> projector)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            if (projector == null) throw new ArgumentNullException(nameof(projector));

            return parser.Then(t => selector(t).Select(u => projector(t, u)));
        }

        public static TokenParser<TTokenKind, U> Then<TTokenKind, T, U>(this TokenParser<TTokenKind, T> first, Func<T, TokenParser<TTokenKind, U>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return TokenResult.CastEmpty<TTokenKind, T, U>(rt);

                var ru = second(rt.Value)(rt.Remainder);
                if (!ru.HasValue)
                    return ru;

                return TokenResult.Value<TTokenKind, U>(ru.Value, input, ru.Remainder);
            };
        }

        public static CharParser<U> Then<T, U>(this CharParser<T> first, Func<T, CharParser<U>> second)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (second == null) throw new ArgumentNullException(nameof(second));

            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return CharResult.CastEmpty<T, U>(rt);

                var ru = second(rt.Value)(rt.Remainder);
                if (!ru.HasValue)
                    return ru;

                return CharResult.Value(ru.Value, input, ru.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T> Try<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var rt = parser(input);
                if (rt.HasValue)
                    return rt;

                return TokenResult.Empty<TTokenKind, T>(input);
            };
        }

        public static CharParser<T> Try<T>(this CharParser<T> parser)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return input =>
            {
                var rt = parser(input);
                if (rt.HasValue)
                    return rt;

                return CharResult.Empty<T>(input);
            };
        }

        public static TokenParser<TTokenKind, U> Value<TTokenKind, T, U>(this TokenParser<TTokenKind, T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.IgnoreThen(Parse.Return<TTokenKind, U>(value));
        }

        public static CharParser<U> Value<T, U>(this CharParser<T> parser, U value)
        {
            if (parser == null) throw new ArgumentNullException(nameof(parser));

            return parser.IgnoreThen(Parse.Return(value));
        }

        public static TokenParser<TTokenKind, T> Where<TTokenKind, T>(this TokenParser<TTokenKind, T> parser, Func<T, bool> predicate)
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

                return TokenResult.Empty<TTokenKind, T>(input, "unsatisfied condition");
            };
        }

        public static CharParser<T> Where<T>(this CharParser<T> parser, Func<T, bool> predicate)
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

                return CharResult.Empty<T>(input, "unsatisfied condition");
            };
        }
    }
}

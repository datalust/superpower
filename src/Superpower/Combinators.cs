using System;
using System.Collections.Generic;
using System.Linq;
using Superpower.Model;

namespace Superpower
{
    public static class Combinators
    {
        public static TokenParser<TTokenKind, U> Apply<TTokenKind, U>(this TokenParser<TTokenKind, Token<TTokenKind>> parser, Func<Token<TTokenKind>, CharParser<U>> valueParser)
        {
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
            return parser.Then(first => parser.Many().Select(rest => new[] { first }.Concat(rest).ToArray()));
        }

        public static CharParser<T[]> AtLeastOnce<T>(this CharParser<T> parser)
        {
            return parser.Then(first => parser.Many().Select(rest => new[] { first }.Concat(rest).ToArray()));
        }

        public static TokenParser<TTokenKind, T[]> Many<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
        {
            return input =>
            {
                var result = new List<T>();
                var @from = input;
                var r = parser(input);
                while (r.HasValue)
                {
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
            return input =>
            {
                var result = new List<T>();
                var @from = input;
                var r = parser(input);
                while (r.HasValue)
                {
                    result.Add(r.Value);
                    @from = r.Remainder;
                    r = parser(r.Remainder);
                }

                if (r.IsPartial(@from))
                    return CharResult.CastEmpty<T, T[]>(r);

                return CharResult.Value(result.ToArray(), input, r.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T> Or<TTokenKind, T>(this TokenParser<TTokenKind, T> lhs, TokenParser<TTokenKind, T> rhs)
        {
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
            return parser.Then(rt => Parse.Return<TTokenKind, U>(selector(rt)));
        }

        public static CharParser<U> Select<T, U>(this CharParser<T> parser, Func<T, U> selector)
        {
            return parser.Then(rt => Parse.Return(selector(rt)));
        }

        public static TokenParser<TTokenKind, U> Then<TTokenKind, T, U>(this TokenParser<TTokenKind, T> t, Func<T, TokenParser<TTokenKind, U>> u)
        {
            return input =>
            {
                var rt = t(input);
                if (!rt.HasValue)
                    return TokenResult.CastEmpty<TTokenKind, T, U>(rt);

                return u(rt.Value)(rt.Remainder);
            };
        }

        public static CharParser<U> Then<T, U>(this CharParser<T> first, Func<T, CharParser<U>> second)
        {
            return input =>
            {
                var rt = first(input);
                if (!rt.HasValue)
                    return CharResult.CastEmpty<T, U>(rt);

                return second(rt.Value)(rt.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T> Try<TTokenKind, T>(this TokenParser<TTokenKind, T> parser)
        {
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
            return input =>
            {
                var rt = parser(input);
                if (rt.HasValue)
                    return rt;

                return CharResult.Empty<T>(input);
            };
        }

        public static TokenParser<TTokenKind, U> Value<TTokenKind, T, U>(this TokenParser<TTokenKind, T> parser, U u)
        {
            return parser.Then(_ => Parse.Return<TTokenKind, U>(u));
        }

        public static CharParser<U> Value<T, U>(this CharParser<T> parser, U u)
        {
            return parser.Then(_ => Parse.Return(u));
        }
    }
}

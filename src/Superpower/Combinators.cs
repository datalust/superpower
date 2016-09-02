using System;
using System.Collections.Generic;
using System.Linq;
using Superpower.Model;

namespace Superpower
{
    public static class Combinators
    {
        public static TokenParser<TTokenKind, U> Apply<TTokenKind, U>(this TokenParser<TTokenKind, Token<TTokenKind>> t, Func<Token<TTokenKind>, CharParser<U>> u)
        {
            return input =>
            {
                var rt = t(input);
                if (!rt.HasValue)
                    return TokenResult.Empty<TTokenKind, U>(rt.Remainder);

                var uParser = u(rt.Value);
                var uResult = uParser.AtEnd()(rt.Value.Span);
                if (!uResult.HasValue)
                    return TokenResult.Empty<TTokenKind, U>(rt.Remainder);

                return TokenResult.Value(uResult.Value, rt.Location, rt.Remainder);
            };
        }

        public static TokenParser<TTokenKind, U> Apply<TTokenKind, U>(this TokenParser<TTokenKind, Token<TTokenKind>> t, CharParser<U> u)
        {
            return t.Apply(rt => u);
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

                return Result.Empty<T>(result.Remainder);
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

        public static TokenParser<TTokenKind, T[]> AtLeastOnce<TTokenKind, T>(this TokenParser<TTokenKind, T> t)
        {
            return t.Then(first => t.Many().Select(rest => new[] { first }.Concat(rest).ToArray()));
        }

        public static TokenParser<TTokenKind, T[]> Many<TTokenKind, T>(this TokenParser<TTokenKind, T> t)
        {
            return input =>
            {
                var result = new List<T>();
                var r = t(input);
                while (r.HasValue)
                {
                    result.Add(r.Value);
                    r = t(r.Remainder);
                }
                return TokenResult.Value(result.ToArray(), input, r.Remainder);
            };
        }

        public static CharParser<T[]> Many<T>(this CharParser<T> t)
        {
            return input =>
            {
                var result = new List<T>();
                var r = t(input);
                while (r.HasValue)
                {
                    result.Add(r.Value);
                    r = t(r.Remainder);
                }
                return Result.Value(result.ToArray(), input, r.Remainder);
            };
        }

        public static TokenParser<TTokenKind, T> Or<TTokenKind, T>(this TokenParser<TTokenKind, T> lhs, TokenParser<TTokenKind, T> rhs)
        {
            return input =>
            {
                var first = lhs(input);
                if (first.HasValue)
                    return first;

                return rhs(input);
            };
        }

        public static CharParser<T> Or<T>(this CharParser<T> lhs, CharParser<T> rhs)
        {
            return input =>
            {
                var first = lhs(input);
                if (first.HasValue)
                    return first;

                return rhs(input);
            };
        }

        public static TokenParser<TTokenKind, U> Select<TTokenKind, T, U>(this TokenParser<TTokenKind, T> t, Func<T, U> u)
        {
            return t.Then(rt => Parse.Return<TTokenKind, U>(u(rt)));
        }

        public static CharParser<U> Select<T, U>(this CharParser<T> t, Func<T, U> u)
        {
            return t.Then(rt => Parse.Return(u(rt)));
        }

        public static TokenParser<TTokenKind, U> Then<TTokenKind, T, U>(this TokenParser<TTokenKind, T> t, Func<T, TokenParser<TTokenKind, U>> u)
        {
            return input =>
            {
                var rt = t(input);
                if (!rt.HasValue)
                    return TokenResult.Empty<TTokenKind, U>(rt.Remainder);

                return u(rt.Value)(rt.Remainder);
            };
        }

        public static CharParser<U> Then<T, U>(this CharParser<T> t, Func<T, CharParser<U>> u)
        {
            return input =>
            {
                var rt = t(input);
                if (!rt.HasValue)
                    return Result.Empty<U>(rt.Remainder);

                return u(rt.Value)(rt.Remainder);
            };
        }

        public static TokenParser<TTokenKind, U> Value<TTokenKind, T, U>(this TokenParser<TTokenKind, T> t, U u)
        {
            return t.Then(_ => Parse.Return<TTokenKind, U>(u));
        }

        public static CharParser<U> Value<T, U>(this CharParser<T> t, U u)
        {
            return t.Then(_ => Parse.Return(u));
        }
    }
}
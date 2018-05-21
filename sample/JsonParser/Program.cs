using System;
using System.Text.RegularExpressions;
using Superpower;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace JsonParser
{
    enum JsonToken
    {
        LBracket,
        RBracket,
        LSquareBracket,
        RSquareBracket,
        Colon,
        Comma,
        String,
        Number,
        True,
        False,
        Null,
    }

    static class JsonTextParsers
    {
        public static TextParser<string> String { get; } =
            from _ in Character.EqualTo('"')
            from chars in Character.Except('\\').Try()
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(Character.EqualTo('\\')
                        .Or(Character.EqualTo('"').Value('"'))
                        .Or(Character.EqualTo('/').Value('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Or(Span.Length(4).Apply(Numerics.HexUInt32).Select(cc => (char)cc))))                
                .Many()
            from __ in Character.EqualTo('"')
            select new string(chars);

        public static TextParser<double> Number { get; } =
            from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
            from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
            from frac in Character.EqualTo('.')
                .IgnoreThen(Numerics.Natural)
                .Select(n => double.Parse("0." + n.ToStringValue()))
                .OptionalOrDefault()
            from exp in Character.EqualToIgnoreCase('e')
                .IgnoreThen(Character.EqualTo('+').Value(1.0)
                    .Or(Character.EqualTo('-').Value(-1.0))
                    .OptionalOrDefault(1.0))
                .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
            select (whole + frac) * sign * Math.Pow(10, exp);
    }

    static class JsonTokenizer
    {
        public static Tokenizer<JsonToken> Instance { get; } = Create();

        static Tokenizer<JsonToken> Create()
        {
            return new TokenizerBuilder<JsonToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.EqualTo('{'), JsonToken.LBracket)
                .Match(Character.EqualTo('}'), JsonToken.RBracket)
                .Match(Character.EqualTo(':'), JsonToken.Colon)
                .Match(Character.EqualTo(','), JsonToken.Comma)
                .Match(Character.EqualTo('['), JsonToken.LSquareBracket)
                .Match(Character.EqualTo(']'), JsonToken.RSquareBracket)
                .Match(JsonTextParsers.String, JsonToken.String)
                .Match(JsonTextParsers.Number, JsonToken.Number)
                .Match(Span.EqualTo("true"),   JsonToken.True)
                .Match(Span.EqualTo("false"),  JsonToken.False)
                .Match(Span.EqualTo("null"),   JsonToken.Null)
                .Build();
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Superpower;
using Superpower.Model;
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

    static class JsonTokenizer
    {
        public static Tokenizer<JsonToken> Instance { get; } = Create();

        static TextParser<Unit> JsonStringToken { get; } =
            from open in Character.EqualTo('"')
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Character.EqualTo('"')
            select Unit.Value;

        static TextParser<Unit> JsonNumberToken { get; } =
            from sign in Character.EqualTo('-').OptionalOrDefault()
            from first in Character.Digit
            from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
            select Unit.Value;

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
                .Match(JsonStringToken,        JsonToken.String)
                .Match(JsonNumberToken,        JsonToken.Number, requireDelimiters: true)
                .Match(Span.EqualTo("true"),   JsonToken.True, requireDelimiters: true)
                .Match(Span.EqualTo("false"),  JsonToken.False, requireDelimiters: true)
                .Match(Span.EqualTo("null"),   JsonToken.Null, requireDelimiters: true)
                .Build();
        }
    }

    static class JsonTextParsers
    {
        public static TextParser<string> String { get; } =
            from open in Character.EqualTo('"')
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
            from close in Character.EqualTo('"')
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

    static class JsonParser
    {
        static TokenListParser<JsonToken, object> JsonString { get; } =
            Token.EqualTo(JsonToken.String)
                .Apply(JsonTextParsers.String)
                .Select(s => (object)s);
        
        static TokenListParser<JsonToken, object> JsonNumber { get; } =
            Token.EqualTo(JsonToken.Null)
                .Apply(JsonTextParsers.Number)
                .Select(n => (object)n);

        static TokenListParser<JsonToken, object> JsonObject { get; } =
            from open in Token.EqualTo(JsonToken.LBracket)
            from properties in JsonString
                .Then(name => Token.EqualTo(JsonToken.Colon)
                    .IgnoreThen(Parse.Ref(() => JsonValue)
                        .Select(value => KeyValuePair.Create((string)name, value))))
                .ManyDelimitedBy(Token.EqualTo(JsonToken.Comma))
            from close in Token.EqualTo(JsonToken.RBracket)
            select (object)new Dictionary<string, object>(properties);

        static TokenListParser<JsonToken, object> JsonArray { get; } =
            from open in Token.EqualTo(JsonToken.LSquareBracket)
            from values in Parse.Ref(() => JsonValue)
                .ManyDelimitedBy(Token.EqualTo(JsonToken.Comma))
            from close in Token.EqualTo(JsonToken.RSquareBracket)
            select (object)values;

        static TokenListParser<JsonToken, object> JsonTrue { get; } =
            Token.EqualTo(JsonToken.True).Value((object)true);
        
        static TokenListParser<JsonToken, object> JsonFalse { get; } =
            Token.EqualTo(JsonToken.False).Value((object)false);    

        static TokenListParser<JsonToken, object> JsonNull { get; } =
            Token.EqualTo(JsonToken.Null).Value((object)null);

        static TokenListParser<JsonToken, object> JsonValue { get; } =
            JsonString
                .Or(JsonNumber)
                .Or(JsonObject)
                .Or(JsonArray)
                .Or(JsonTrue)
                .Or(JsonFalse)
                .Or(JsonNull)
                .Named("JSON value");

        public static bool TryParse(string json, out object value, out string error)
        {
            var tokens = JsonTokenizer.Instance.TryTokenize(json);
            if (!tokens.HasValue)
            {
                value = null;
                error = tokens.ToString();
                return false;
            }

            var parsed = JsonValue.TryParse(tokens.Value);
            if (!parsed.HasValue)
            {
                value = null;
                error = parsed.ToString();
                return false;
            }

            value = parsed.Value;
            error = null;
            return true;
        }
    }

    static class Program
    {
        static void Main()
        {
            var line = Console.ReadLine();
            while (line != null)
            {
                if (JsonParser.TryParse(line, out var value, out var error))
                    Console.WriteLine("Parsed: " + (value ?? "<null>"));
                else
                    Console.WriteLine("Error: " + error);

                line = Console.ReadLine();
            }
        }
    }
}
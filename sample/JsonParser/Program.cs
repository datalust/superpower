using System;
using System.Collections.Generic;
using Superpower;
using Superpower.Display;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

// This is an example JSON parser. It correctly and completely implements the
// language spec at https://json.org (or should), but the goal isn't to use
// this "for real" - there are no tests, after all! :-)
//
// The goal of the example is to demonstrate how a reasonably-efficient parser
// with end-user-quality error reporting can be built using Superpower. Getting
// everything "just perfect" does take some fine tuning, but the result is still
// readable.

namespace JsonParser
{
    // The parser is token-based. This enum lists the various kinds
    // of element that make up a JSON document. Check out the
    // the JSON spec linked above: you'll see that the tokens kinds map very
    // closely to what you see in the boxes of the syntax diagrams.
    enum JsonToken
    {
        // In general, it's better to be more, rather than less-specific
        // when it comes to choosing what kinds of tokens to generate,
        // because conditional rules in the parser - like "match this
        // kind of token" - can be written more simply if the tokens are
        // descriptive.
        [Token(Example = "{")]
        LBracket,

        // The `Token` attribute lets a little more information be
        // associated with the token. In error messages, the token will
        // normally be described by lower-casing the enum variant name:
        // "unexpected rbracket" - the `Example` property will turn this
        // into "unexpected `}`".
        [Token(Example = "}")]
        RBracket,
        
        // Notice that the tokens describe the characters and clumps of
        // characters in the language - it's a "bracket", at this level, not
        // an "array start".
        [Token(Example = "[")]
        LSquareBracket,
        
        [Token(Example = "]")]
        RSquareBracket,
        
        [Token(Example = ":")]
        Colon,
        
        [Token(Example = ",")]
        Comma,
        
        String,
        
        Number,
        
        // Although JSON doesn't have an "identifier" or "keyword"
        // concept that groups `true`, `false`, and `null`, it's useful
        // for the tokenizer to be very permissive - it's more informative
        // to generate an error later at the parsing stage, e.g.
        // _unexpected identifier `flase`_, instead of failing at the
        // tokenization stage where all we'd have is _unexpected `l`_.
        Identifier,
    }

    // The tokenizer here is assembled using `TokenizerBuilder`. The `Instance`
    // property is the place to start reading, but it has to be ordered below
    // the two "recognizers" that it depends upon, or else they'll be uninitialized
    // when we try to put the tokenizer together.
    static class JsonTokenizer
    {
        // This is a "recognizer" that matches - just like a regular expression would -
        // a block of input text that resembles a JSON string. Notice that it's very
        // permissive - it's only concerned with finding the start and end of the string,
        // and doesn't try verifying the correctness of escape sequences and so-on.
        //
        // The `Unit` type is just a way of expressing `Void` in a C#-friendly way: the
        // recognizer doesn't need to return a value, just match the text.
        //
        // The parser uses built-ins like `Character.EqualTo()` and `Span.EqualTo()`.
        // There's a whole range of simple parsers like these in the `Parsers` namespace.
        //
        // Also in here, you encounter a couple of Superpower's less-obvious combinators:
        //
        //  * `Try()` causes the attempted match of `\"` to backtrack; in order to report
        //    errors accurately, Superpower fails fast when a parser partially matches its
        //    input, as `Span.EqualTo("\\\"")` will when it finds other escape sequences
        //    such as `\n`. When we backtrack, we can try parsing the `\` again with the
        //    following `Character.Except('"')` parser.
        //  * `Value()` here is just being used cast the span parser and the character
        //    parser to compatible types (since we don't care about the values matched
        //    by either of them.
        //  * `IgnoreMany()` is an optimization - we could have used `Many()` here, only
        //    that would allocate an array to return its value - `IgnoreMany()` just
        //    drops the items that it matches.
        static TextParser<Unit> JsonStringToken { get; } =
            from open in Character.EqualTo('"')
            from content in Span.EqualTo("\\\"").Value(Unit.Value).Try()
                .Or(Character.Except('"').Value(Unit.Value))
                .IgnoreMany()
            from close in Character.EqualTo('"')
            select Unit.Value;

        // Like the string parser, the number version is permissive - it's just looking 
        // for a chunk of input that looks something like a JSON number, and not
        // necessarily a valid one.
        static TextParser<Unit> JsonNumberToken { get; } =
            from sign in Character.EqualTo('-').OptionalOrDefault()
            from first in Character.Digit
            from rest in Character.Digit.Or(Character.In('.', 'e', 'E', '+', '-')).IgnoreMany()
            select Unit.Value;

        // Here's the tokenizer. Working through the input text, the rules are
        // tried in top-to-bottom order until one of them matches. When a rule
        // matches, that rule's token will be produced, and then the tokenizer
        // starts again with the remaining input.
        //
        // The number and identifier tokens are marked as requiring delimiters
        // (all of the other tokens/whitespace are implicitly considered to
        // be delimiters), so that, say, "123abc" isn't inadvertently tokenized
        // as a number, "123", followed by the identifier "abc". The `requireDelimiters`
        // flag deals with this case.
        //
        // One important note about the tokenizer - it's not the place to detect or report
        // errors, except when it's unavoidable. Better errors can be generated later
        // in the parsing process. That's why we accept anything that looks remotely
        // like an identifier, and only check during parsing whether it's a
        // `true`, `false`, `null`, or some invalid junk.
        public static Tokenizer<JsonToken> Instance { get; } = 
            new TokenizerBuilder<JsonToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Character.EqualTo('{'), JsonToken.LBracket)
                .Match(Character.EqualTo('}'), JsonToken.RBracket)
                .Match(Character.EqualTo(':'), JsonToken.Colon)
                .Match(Character.EqualTo(','), JsonToken.Comma)
                .Match(Character.EqualTo('['), JsonToken.LSquareBracket)
                .Match(Character.EqualTo(']'), JsonToken.RSquareBracket)
                .Match(JsonStringToken,        JsonToken.String)
                .Match(JsonNumberToken,        JsonToken.Number, requireDelimiters: true)
                .Match(Identifier.CStyle,      JsonToken.Identifier, requireDelimiters: true)
                .Build();
    }

    // By this point, we have everything we need to break a JSON document down
    // into a stream of tokens resembling `[LBracket, String("foo"), Colon, ...]`.
    //
    // From this point there are actually two jobs remaining to do.
    // 
    // We'll need to assemble the structure of the JSON document, with arrays,
    // objects, and so-on, and we'll also have to decode strings and
    // numbers into their .NET representations - `string`s and `double`s.
    //
    // Before we assemble the document, we'll jump to the latter task: the
    // `JsonTextParsers` class contains two standalone character-driven parsers
    // that can handle the (non-trivial) string and number formats from
    // the JSON spec.
    static class JsonTextParsers
    {
        // Text parsers like these use the same `TextParser<T>` type that the
        // recognizers in the tokenizer use. This might get a little confusing,
        // but it's also rather handy: these parsers could just as easily do
        // double duty as recognizers in the tokenizer - we don't do that here
        // because we want the tokenizer to be more permissive (and there's
        // some redundant work done for things like decoding escape
        // sequences and allocating the .NET `string` object that the tokenizer
        // can avoid).
        //
        // Most of the parsers and combinators here are self-explanatory, but
        // we also encounter a `Span` parser being used in conjunction with
        // `Apply()` to glue together some pre-built parsers to deal with the
        // four-character unicode character code escape (like "\u0056"). This part
        // could be written a few different ways, along the lines of:
        //
        // `Character.HexDigit.Repeat(4).Select(chs => (char)int.Parse(new string(chs), ...`
        //
        // In general, using Superpower parsers over methods like `int.Parse()` will
        // be less likely to throw exceptions, and should report better error info
        // just in case some invalid input slips through.
        //
        // `Named()` is used here to insert text into "expectations"; rather than
        // reporting "expected `\`, `"`, `/`, `b`, f`.." etc., the error message
        // generated for an invalid escape sequence reads "expected escape sequence".
        public static TextParser<string> String { get; } =
            from open in Character.EqualTo('"')
            from chars in Character.ExceptIn('"', '\\')
                .Or(Character.EqualTo('\\')
                    .IgnoreThen(
                        Character.EqualTo('\\')
                        .Or(Character.EqualTo('"'))
                        .Or(Character.EqualTo('/'))
                        .Or(Character.EqualTo('b').Value('\b'))
                        .Or(Character.EqualTo('f').Value('\f'))
                        .Or(Character.EqualTo('n').Value('\n'))
                        .Or(Character.EqualTo('r').Value('\r'))
                        .Or(Character.EqualTo('t').Value('\t'))
                        .Or(Character.EqualTo('u').IgnoreThen(
                                Span.MatchedBy(Character.HexDigit.Repeat(4))
                                    .Apply(Numerics.HexDigitsUInt32)
                                    .Select(cc => (char)cc)))
                        .Named("escape sequence")))                
                .Many()
            from close in Character.EqualTo('"')
            select new string(chars);

        // The number parser here works with some pretty ugly `double`-based
        // maths; it's tricky enough that in a real-world parser, it would
        // be worth considering some imperative code to do this (see the way
        // that `Numerics.NaturalInt32` and similar parsers work, for examples).
        public static TextParser<double> Number { get; } =
            from sign in Character.EqualTo('-').Value(-1.0).OptionalOrDefault(1.0)
            from whole in Numerics.Natural.Select(n => double.Parse(n.ToStringValue()))
            from frac in Character.EqualTo('.')
                .IgnoreThen(Numerics.Natural)
                .Select(n => double.Parse(n.ToStringValue()) * Math.Pow(10, -n.Length))
                .OptionalOrDefault()
            from exp in Character.EqualToIgnoreCase('e')
                .IgnoreThen(Character.EqualTo('+').Value(1.0)
                    .Or(Character.EqualTo('-').Value(-1.0))
                    .OptionalOrDefault(1.0))
                .Then(expsign => Numerics.Natural.Select(n => double.Parse(n.ToStringValue()) * expsign))
                .OptionalOrDefault()
            select (whole + frac) * sign * Math.Pow(10, exp);
    }

    // Here we are at the meat of the parser! You already know, from the
    // sections above, just about everything required to completely understand
    // this one.
    static class JsonParser
    {
        // For simplicity, we use `object` as the stand-in for every
        // possible JSON value type. There's quite a lot of casting:
        // unfortunately, for performance reasons, Superpower uses a
        // parser design that doesn't allow for variance, so you need
        // to create a parser that returns `object` here, even though
        // one that returns `string` should, in theory, be compatible.
        static TokenListParser<JsonToken, object> JsonString { get; } =
            Token.EqualTo(JsonToken.String)
                .Apply(JsonTextParsers.String)
                .Select(s => (object)s);
        
        static TokenListParser<JsonToken, object> JsonNumber { get; } =
            Token.EqualTo(JsonToken.Number)
                .Apply(JsonTextParsers.Number)
                .Select(n => (object)n);

        // The grammar is recursive - values can be objects, which contain
        // values, which can be objects... In order to reflect this circularity,
        // the parser below uses `Parse.Ref()` to refer lazily to the `JsonValue`
        // parser, which won't be constructed until after the runtime initializes
        // the `JsonObject` parser.
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
            Token.EqualToValue(JsonToken.Identifier, "true").Value((object)true);
        
        static TokenListParser<JsonToken, object> JsonFalse { get; } =
            Token.EqualToValue(JsonToken.Identifier, "false").Value((object)false);    

        static TokenListParser<JsonToken, object> JsonNull { get; } =
            Token.EqualToValue(JsonToken.Identifier, "null").Value((object)null);

        static TokenListParser<JsonToken, object> JsonValue { get; } =
            JsonString
                .Or(JsonNumber)
                .Or(JsonObject)
                .Or(JsonArray)
                .Or(JsonTrue)
                .Or(JsonFalse)
                .Or(JsonNull)
                .Named("JSON value");

        static TokenListParser<JsonToken, object> JsonDocument { get; } = JsonValue.AtEnd();

        // `TryParse` is just a helper method. It's useful to write one of these, where
        // the tokenization and parsing phases remain distinct, because it's often very
        // handy to place a breakpoint between the two steps to check out what the
        // token list looks like.
        public static bool TryParse(string json, out object value, out string error)
        {
            var tokens = JsonTokenizer.Instance.TryTokenize(json);
            if (!tokens.HasValue)
            {
                value = null;
                error = tokens.ToString();
                return false;
            }

            var parsed = JsonDocument.TryParse(tokens.Value);
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

    // The driver program accepts a JSON value (string, number, array, object...)
    // on each line and prints the result of parsing, or an error message.
    // Try typing in some valid and some malformed input! The error messages should
    // always be informative; if you find a case where they're not, please raise
    // an issue :).
    static class Program
    {
        static void Main()
        {
            var line = Console.ReadLine();
            while (line != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (JsonParser.TryParse(line, out var value, out var error))
                    {
                        Print(value);
                    }
                    else
                    {
                        Console.WriteLine("Error: " + error);
                    }
                }

                line = Console.ReadLine();
            }
        }

        static void Print(object value, int indent = 0)
        {
            switch (value)
            {
                case null:
                    Indent(indent, "Null");
                    break;
                case true:
                    Indent(indent, "True");
                    break;
                case false:
                    Indent(indent, "False");
                    break;
                case double n:
                    Indent(indent, $"Number: {n}");
                    break;
                case string s:
                    Indent(indent, $"String: {s}");
                    break;
                case object[] a:
                    Indent(indent, "Array:");
                    foreach (var el in a)
                        Print(el, indent + 2);
                    break;
                case Dictionary<string, object> o:
                    Indent(indent, "Object:");
                    foreach (var p in o)
                    {
                        Indent(indent + 2, p.Key);
                        Print(p.Value, indent + 4);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        static void Indent(int amount, string text)
        {
            Console.WriteLine($"{new string(' ', amount)}{text}");
        }
    }
}
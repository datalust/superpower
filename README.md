# Superpower [![Build status](https://ci.appveyor.com/api/projects/status/7bj6if6tyc68urpy?svg=true)](https://ci.appveyor.com/project/datalust/superpower)  [![Join the chat at https://gitter.im/datalust/superpower](https://img.shields.io/gitter/room/datalust/superpower.svg)](https://gitter.im/datalust/superpower) [![NuGet Version](https://img.shields.io/nuget/vpre/Superpower.svg?style=flat)](https://www.nuget.org/packages/Superpower/) [![Stack Overflow](https://img.shields.io/badge/stackoverflow-superpower-orange.svg)](http://stackoverflow.com/questions/tagged/superpower)

A [parser combinator](https://en.wikipedia.org/wiki/Parser_combinator) library based on [Sprache](https://github.com/sprache/Sprache). Superpower generates friendlier error messages through its support for token-based parsers.

![Logo](https://raw.githubusercontent.com/datalust/superpower/dev/asset/Superpower-White-200px.png)

### Usage

Superpower is embedded directly into your program code, without the need for any additional tools or build-time code generation tasks.

The simplest parsers consume characters directly from the source text: 

```csharp
// Parse any number of capital 'A's in a row
var parseA = Character.EqualTo('A').AtLeastOnce();
```

The `Character.EqualTo()` method is a built-in parser. The `AtLeastOnce()` method is a _combinator_, that builds a more complex parser for a sequence of `'A'` characters out of the simple parser for a single `'A'`.

Superpower includes a library of simple parsers and combinators from which sophisticated parsers can be built:

```csharp
TextParser<string> identifier =
    from first in Character.Letter
    from rest in Character.LetterOrDigit.Or(Character.EqualTo('_')).Many()
    select first + new string(rest);

var id = identifier.Parse("abc123");

Assert.Equal("abc123", id);
```

Parsers are highly modular, so smaller parsers can be built and tested independently of the larger parsers built from them.

### Tokenization

A token-driven parser consumes elements from a list of tokens. The type used to represent the kinds of tokens consumed by a parser is generic, but currently Superpower has deeper support for `enum` tokens and using them is recommended.

```csharp
public enum ArithmeticExpressionToken
{
    None,
    Number,
    Plus,
```

Token-driven parsing occurs in two distinct steps:

 1. Tokenization, using a class derived from `Tokenizer<TKind>`, then
 2. Parsing, using a function of type `TokenListParser<TKind>`.

```csharp
var expression = "1 * (2 + 3)";

// 1.
var tokenizer = new ArithmeticExpressionTokenizer();
var tokenList = tokenizer.Tokenize(expresssion);

// 2.
var parser = ArithmeticExpressionParser.Lambda; // parser built with combinators
var expressionTree = parser.Parse(tokenList);

// Use the result
var eval = expressionTree.Compile();
Console.WriteLine(eval()); // -> 5
```

#### Writing tokenizers

A simple arithmetic expression tokenizer is shown below:

```csharp
class ArithmeticExpressionTokenizer : Tokenizer<ArithmeticExpressionToken>
{
    readonly Dictionary<char, ArithmeticExpressionToken> _operators =
        new Dictionary<char, ArithmeticExpressionToken>
        {
            ['+'] = ArithmeticExpressionToken.Plus,
            ['-'] = ArithmeticExpressionToken.Minus,
            ['*'] = ArithmeticExpressionToken.Times,
            ['/'] = ArithmeticExpressionToken.Divide,
            ['('] = ArithmeticExpressionToken.LParen,
            [')'] = ArithmeticExpressionToken.RParen,
        };

    protected override IEnumerable<Result<ArithmeticExpressionToken>> Tokenize(TextSpan span)
    {
        var next = SkipWhiteSpace(span);
        if (!next.HasValue)
            yield break;

        do
        {
            ArithmeticExpressionToken charToken;

            if (char.IsDigit(next.Value))
            {
                var integer = Numerics.Integer(next.Location);
                next = integer.Remainder.ConsumeChar();
                yield return Result.Value(ArithmeticExpressionToken.Number,
                    integer.Location, integer.Remainder);
            }
            else if (_operators.TryGetValue(next.Value, out charToken))
            {
                yield return Result.Value(charToken, next.Location, next.Remainder);
                next = next.Remainder.ConsumeChar();
            }
            else
            {
                yield return Result.Empty<ArithmeticExpressionToken>(next.Location,
                    new[] { "number", "operator" });
            }

            next = SkipWhiteSpace(next.Location);
        } while (next.HasValue);
    }
}
```

The tokenizer itself can use `TextParser<T>` parsers as recognizers, as in the `Numerics.Integer` example above.

#### Writing token list parsers

Token parsers are defined in the same manner as text parsers, but consume tokens from a token list rather than characters from a string:

```csharp
class ArithmeticExpressionParser
{
    static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Add =
        Token.EqualTo(ArithmeticExpressionToken.Plus).Value(ExpressionType.AddChecked);
        
    static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Subtract =
        Token.EqualTo(ArithmeticExpressionToken.Minus).Value(ExpressionType.SubtractChecked);
        
    static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Multiply =
        Token.EqualTo(ArithmeticExpressionToken.Times).Value(ExpressionType.MultiplyChecked);
        
    static readonly TokenListParser<ArithmeticExpressionToken, ExpressionType> Divide = 
        Token.EqualTo(ArithmeticExpressionToken.Divide).Value(ExpressionType.Divide);

    static readonly TokenListParser<ArithmeticExpressionToken, Expression> Constant =
            Token.EqualTo(ArithmeticExpressionToken.Number)
            .Apply(Numerics.IntegerInt32)
            .Select(n => (Expression)Expression.Constant(n));

    static readonly TokenListParser<ArithmeticExpressionToken, Expression> Factor =
        (from lparen in Token.EqualTo(ArithmeticExpressionToken.LParen)
            from expr in Parse.Ref(() => Expr)
            from rparen in Token.EqualTo(ArithmeticExpressionToken.RParen)
            select expr)
        .Or(Constant);

    static readonly TokenListParser<ArithmeticExpressionToken, Expression> Operand =
        (from sign in Token.EqualTo(ArithmeticExpressionToken.Minus)
            from factor in Factor
            select (Expression)Expression.Negate(factor))
        .Or(Factor).Named("expression");

    static readonly TokenListParser<ArithmeticExpressionToken, Expression> Term =
        Parse.Chain(Multiply.Or(Divide), Operand, Expression.MakeBinary);

    static readonly TokenListParser<ArithmeticExpressionToken, Expression> Expr =
        Parse.Chain(Add.Or(Subtract), Term, Expression.MakeBinary);

    public static readonly TokenListParser<ArithmeticExpressionToken, Expression<Func<int>>> Lambda =
        Expr.AtEnd().Select(body => Expression.Lambda<Func<int>>(body));
}
```

### Error messages

The [error scenario tests](https://github.com/datalust/superpower/blob/dev/test/Superpower.Tests/ErrorMessageScenarioTests.cs) demonstrate some of the error message formatting capabilities of Superpower. Check out the parsers referenced in the tests for some examples.

```csharp
ArithmeticExpressionParser.Lambda.Parse(new ArithmeticExpressionTokenizer().Tokenize("1 + * 3"));
     // -> Syntax error (line 1, column 5): unexpected operator `*`, expected expression.
```

To improve the error reporting for a particular token type, apply the `[Token]` attribute:

```csharp
public enum ArithmeticExpressionToken
{
    None,

    Number,

    [Token(Category = "operator", Example = "+")]
    Plus,
```

### Performance

Superpower is built with performance as a priority. Less frequent backtracking, combined with the avoidance of allocations and indirect dispatch, mean that Superpower can be quite a bit faster than Sprache.

Recent benchmark for parsing a long arithmetic expression:

```ini
Host Process Environment Information:
BenchmarkDotNet.Core=v0.9.9.0
OS=Windows
Processor=?, ProcessorCount=8
Frequency=2533306 ticks, Resolution=394.7411 ns, Timer=TSC
CLR=CORE, Arch=64-bit ? [RyuJIT]
GC=Concurrent Workstation
dotnet cli version: 1.0.0-preview2-003121

Type=ArithmeticExpressionBenchmark  Mode=Throughput  

```

|          Method |      Median |     StdDev | Scaled | Scaled-SD |
|---------------- |----------- |---------- |------ |--------- |
|         Sprache | 283.8618 &micro;s | 10.0276 &micro;s |   1.00 |      0.00 |
| Superpower (Token) |  81.1563 &micro;s |  2.8775 &micro;s |   0.29 |      0.01 |

Benchmarks and results are included in the repository.

### Examples

Superpower is introduced, with a worked example, in [this blog post](https://nblumhardt.com/2016/09/superpower/).

**Example** parsers to learn from:

* [_DateTimeTextParser_](https://github.com/datalust/superpower/tree/dev/sample/DateTimeTextParser) shows how Superpower's text parsers work, parsing ISO-8601 date-times
* [_IntCalc_](https://github.com/datalust/superpower/tree/dev/sample/IntCalc) is a simple arithmetic expresion parser (`1 + 2 * 3`) included in the repository, demonstrating how Superpower token parsing works
* [_Plotty_](https://github.com/SuperJMN/Plotty) implements a logo-like drawing language

**Real-world** projects built with Superpower:

* [_Serilog.Filters.Expressions_](https://github.com/serilog/serilog-filters-expressions) uses Superpower to implement a filtering language for structured log events
* The query language of [Seq](https://getseq.net) is implemented using Superpower

_Have an example we can add to this list? [Let us know](https://github.com/datalust/superpower/issues/new)._

### Getting help

Please post issues [to the issue tracker](https://github.com/datalust/superpower/issues), visit our [Gitter chat](https://gitter.im/datalust/superpower), or tag your [question on StackOverflow](http://stackoverflow.com/questions/tagged/superpower) with `superpower`.

_The repository's title arose out of a talk_ "Parsing Text: the Programming Superpower You Need at Your Fingertips" _given at [DDD Brisbane](http://dddbrisbane.com/) 2015._

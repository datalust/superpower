# Superpower [![Build status](https://ci.appveyor.com/api/projects/status/7bj6if6tyc68urpy?svg=true)](https://ci.appveyor.com/project/datalust/superpower)  [![Join the chat at https://gitter.im/datalust/superpower](https://img.shields.io/gitter/room/datalust/superpower.svg)](https://gitter.im/datalust/superpower) [![NuGet Version](https://img.shields.io/nuget/vpre/Superpower.svg?style=flat)](https://www.nuget.org/packages/Superpower/) [![Stack Overflow](https://img.shields.io/badge/stackoverflow-superpower-orange.svg)](http://stackoverflow.com/questions/tagged/superpower)

A [parser combinator](https://en.wikipedia.org/wiki/Parser_combinator) library based on
[Sprache](https://github.com/sprache/Sprache). Superpower generates friendlier error messages through its support for
token-driven parsers.

![Logo](https://raw.githubusercontent.com/datalust/superpower/dev/asset/Superpower-White-200px.png)

### What is Superpower?

The job of a parser is to take a sequence of characters as input, and produce a data structure that's easier
for a program to analyze, manipulate, or transform. From this point of view, a parser is just a function from `string`
to `T` - where `T` might be anything from a simple number, a list of fields in a data format, or the abstract syntax
tree of some kind of programming language.

Just like other kinds of functions, parsers can be built by hand, from scratch. This is-or-isn't a lot of fun, depending
on the complexity of the parser you need to build (and how you plan to spend your next few dozen nights and weekends).

Superpower is a library for writing parsers in a declarative style that mirrors
the structure of the target grammar. Parsers built with Superpower are fast, robust, and report precise and
informative errors when invalid input is encountered.

### Usage

Superpower is embedded directly into your C# program, without the need for any additional tools or build-time code
generation tasks.

```shell
dotnet add package Superpower
```

The simplest _text parsers_ consume characters directly from the source text: 

```csharp
// Parse any number of capital 'A's in a row
var parseA = Character.EqualTo('A').AtLeastOnce();
```

The `Character.EqualTo()` method is a built-in parser. The `AtLeastOnce()` method is a _combinator_, that builds a more
complex parser for a sequence of `'A'` characters out of the simple parser for a single `'A'`.

Superpower includes a library of simple parsers and combinators from which more sophisticated parsers can be built:

```csharp
TextParser<string> identifier =
    from first in Character.Letter
    from rest in Character.LetterOrDigit.Or(Character.EqualTo('_')).Many()
    select first + new string(rest);

var id = identifier.Parse("abc123");

Assert.Equal("abc123", id);
```

Parsers are highly modular, so smaller parsers can be built and tested independently of the larger parsers that use
them.

### Tokenization

Along with text parsers that consume input character-by-character, Superpower supports _token parsers_.

A token parser consumes elements from a list of tokens. A token is a fragment of the input text, tagged with the
kind of item that fragment represents - usually specified using an `enum`:

```csharp
public enum ArithmeticExpressionToken
{
    None,
    Number,
    Plus,
```

A major benefit of driving parsing from tokens, instead of individual characters, is that errors can be reported in
terms of tokens - _unexpected identifier \`frm\`, expected keyword \`from\`_ - instead of the cryptic _unexpected `m`_.

Token-driven parsing takes place in two distinct steps:

 1. Tokenization, using a class derived from `Tokenizer<TKind>`, then
 2. Parsing, using a function of type `TokenListParser<TKind>`.

```csharp
var expression = "1 * (2 + 3)";

// 1.
var tokenizer = new ArithmeticExpressionTokenizer();
var tokenList = tokenizer.Tokenize(expression);

// 2.
var parser = ArithmeticExpressionParser.Lambda; // parser built with combinators
var expressionTree = parser.Parse(tokenList);

// Use the result
var eval = expressionTree.Compile();
Console.WriteLine(eval()); // -> 5
```

#### Assembling tokenizers with `TokenizerBuilder<TKind>`

The job of a _tokenizer_ is to split the input into a list of tokens - numbers, keywords, identifiers, operators -
while discarding irrelevant trivia such as whitespace or comments.

Superpower provides the `TokenizerBuilder<TKind>` class to quickly assemble tokenizers from _recognizers_,
text parsers that match the various kinds of tokens required by the grammar.

A simple arithmetic expression tokenizer is shown below:

```csharp
var tokenizer = new TokenizerBuilder<ArithmeticExpressionToken>()
    .Ignore(Span.WhiteSpace)
    .Match(Character.EqualTo('+'), ArithmeticExpressionToken.Plus)
    .Match(Character.EqualTo('-'), ArithmeticExpressionToken.Minus)
    .Match(Character.EqualTo('*'), ArithmeticExpressionToken.Times)
    .Match(Character.EqualTo('/'), ArithmeticExpressionToken.Divide)
    .Match(Character.EqualTo('('), ArithmeticExpressionToken.LParen)
    .Match(Character.EqualTo(')'), ArithmeticExpressionToken.RParen)
    .Match(Numerics.Natural, ArithmeticExpressionToken.Number)
    .Build();
```

Tokenizers constructed this way produce a list of tokens by repeatedly attempting to match recognizers 
against the input in top-to-bottom order.

#### Writing tokenizers by hand

Tokenizers can alternatively be written by hand; this can provide the most flexibility, performance, and control,
at the expense of more complicated code. A handwritten arithmetic expression tokenizer is included in the test suite,
and a more complete example can be found [here](https://github.com/serilog/serilog-filters-expressions/blob/dev/src/Serilog.Filters.Expressions/Filters/Expressions/Parsing/FilterExpressionTokenizer.cs).

#### Writing token list parsers

Token parsers are defined in the same manner as text parsers, using combinators to build up more sophisticated parsers
out of simpler ones.

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

The [error scenario tests](https://github.com/datalust/superpower/blob/dev/test/Superpower.Tests/ErrorMessageScenarioTests.cs)
demonstrate some of the error message formatting capabilities of Superpower. Check out the parsers referenced in the 
tests for some examples.

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

Superpower is built with performance as a priority. Less frequent backtracking, combined with the avoidance of
allocations and indirect dispatch, mean that Superpower can be quite a bit faster than Sprache.

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

**Tips:** if you find you need more throughput: 1) consider a hand-written tokenizer, and 2) avoid the use of LINQ comprehensions and instead use chained combinators like `Then()` and especially `IgnoreThen()` - these allocate fewer delegates (closures) during parsing.

### Examples

Superpower is introduced, with a worked example, in [this blog post](https://nblumhardt.com/2016/09/superpower/).

**Example** parsers to learn from:

* [_JsonParser_](https://github.com/datalust/superpower/tree/dev/sample/JsonParser/Program.cs) is a complete, annotated
 example implementing the [JSON spec](https://json.org) with good error reporting
* [_DateTimeTextParser_](https://github.com/datalust/superpower/tree/dev/sample/DateTimeTextParser) shows how Superpower's text parsers work, parsing ISO-8601 date-times
* [_IntCalc_](https://github.com/datalust/superpower/tree/dev/sample/IntCalc) is a simple arithmetic expresion parser (`1 + 2 * 3`) included in the repository, demonstrating how Superpower token parsing works
* [_Plotty_](https://github.com/SuperJMN/Plotty) implements an instruction set for a RISC virtual machine
* [_tcalc_](https://github.com/nblumhardt/tcalc) is an example expression language that computes durations (`1d / 12m`)

**Real-world** projects built with Superpower:

* [_Serilog.Expressions_](https://github.com/serilog/serilog-expressions) uses Superpower to implement an expression and templating language for structured log events
* The query language of [Seq](https://datalust.co/seq) is implemented using Superpower
* `seqcli` [extraction patterns](https://github.com/datalust/seqcli#extraction-patterns) use Superpower for plain-text log parsing

_Have an example we can add to this list? [Let us know](https://github.com/datalust/superpower/issues/new)._

### Getting help

Please post issues [to the issue tracker](https://github.com/datalust/superpower/issues), visit our [Gitter chat](https://gitter.im/datalust/superpower), or tag your [question on StackOverflow](http://stackoverflow.com/questions/tagged/superpower) with `superpower`.

_The repository's title arose out of a talk_ "Parsing Text: the Programming Superpower You Need at Your Fingertips" _given at [DDD Brisbane](http://dddbrisbane.com/) 2015._

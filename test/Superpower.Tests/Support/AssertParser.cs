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
using System.Linq;
using Xunit;
using Superpower;
using Superpower.Model;

namespace Superpower.Tests.Support
{
    static class AssertParser
    {
        public static void SucceedsWithOne<T>(CharParser<IEnumerable<T>> parser, string input, T expectedResult)
        {
            Succeeds(parser, input, t =>
            {
                Assert.Equal(1, t.Count());
                Assert.Equal(expectedResult, t.Single());
            });
        }

        public static void SucceedsWithMany<T>(CharParser<IEnumerable<T>> parser, string input, IEnumerable<T> expectedResult)
        {
            Succeeds(parser, input, t => Assert.True(t.SequenceEqual(expectedResult)));
        }

        public static void SucceedsWithAll(CharParser<IEnumerable<char>> parser, string input)
        {
            SucceedsWithMany(parser, input, input.ToCharArray());
        }

        public static void Succeeds<T>(CharParser<T> parser, string input, Action<T> resultAssertion)
        {
            var t = parser.Parse(input);
            resultAssertion(t);
        }

        public static void SucceedsWith<T>(CharParser<T> parser, string input, T value)
        {
            var t = parser.Parse(input);
            Assert.Equal(value, t);
        }

        public static void Fails<T>(CharParser<T> parser, string input)
        {
            FailsWith(parser, input, f => { });
        }

        public static void FailsAt<T>(CharParser<T> parser, string input, int position)
        {
            FailsWith(parser, input, f => Assert.Equal(position, f.Remainder.Position.Absolute));
        }

        public static void FailsWith<T>(CharParser<T> parser, string input, Action<CharResult<T>> resultAssertion)
        {
            var result = parser.TryParse(input);

            if (result.HasValue)
                Assert.False(result.HasValue, $"Expected failure but succeeded with {result.Value}.");

            resultAssertion(result);
        }

        public static void SucceedsWithOne<T>(TokenParser<char, IEnumerable<T>> parser, string input, T expectedResult)
        {
            Succeeds(parser, input, t =>
            {
                Assert.Equal(1, t.Count());
                Assert.Equal(expectedResult, t.Single());
            });
        }

        public static void SucceedsWithMany<T>(TokenParser<char, IEnumerable<T>> parser, string input, IEnumerable<T> expectedResult)
        {
            Succeeds(parser, input, t => Assert.True(t.SequenceEqual(expectedResult)));
        }

        public static void SucceedsWithAll(TokenParser<char, IEnumerable<char>> parser, string input)
        {
            SucceedsWithMany(parser, input, input.ToCharArray());
        }

        public static void Succeeds<T>(TokenParser<char, T> parser, string input, Action<T> resultAssertion)
        {
            var t = parser.Parse(Tokenize(input));
            resultAssertion(t);
        }

        public static void SucceedsWith(TokenParser<char, Token<char>> parser, string input, char value)
        {
            Succeeds(parser, input, tok =>
            {
                Assert.Equal(value, tok.Kind);
            });
        }

        public static void Fails<T>(TokenParser<char, T> parser, string input)
        {
            FailsWith(parser, input, f => { });
        }

        public static void FailsAt<T>(TokenParser<char, T> parser, string input, int position)
        {
            FailsWith(parser, input, f => Assert.Equal(position, f.Remainder.Position));
        }

        public static void FailsWith<T>(TokenParser<char, T> parser, string input, Action<TokenResult<char, T>> resultAssertion)
        {
            var result = parser.TryParse(Tokenize(input));

            if (result.HasValue)
                Assert.False(result.HasValue, $"Expected failure but succeeded with {result.Value}.");

            resultAssertion(result);
        }

        static TokenList<char> Tokenize(string tokens)
        {
            var items = tokens.ToCharArray()
                .Select((ch, i) => new Token<char>(ch, new StringSpan(tokens, new Position(i, 1, 1), 1)))
                .ToArray();

            return new TokenList<char>(items);
        }
    }
}

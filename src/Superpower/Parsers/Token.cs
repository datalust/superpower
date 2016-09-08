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
using Superpower.Display;
using Superpower.Model;

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for matching individual tokens.
    /// </summary>
    public static class Token
    {
        /// <summary>
        /// Parse a token of the kind <typeparamref name="TTokenKind"/>.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of tokens being matched.</typeparam>
        /// <param name="token">The value to match.</param>
        /// <returns>The matched token.</returns>
        public static TokenParser<TTokenKind, Token<TTokenKind>> EqualTo<TTokenKind>(TTokenKind token)
        {
            var expectations = new[] { Presentation.FormatExpectation(token) };

            return input =>
            {
                var next = input.ConsumeToken();
                if (!next.HasValue || !next.Value.Kind.Equals(token))
                    return TokenResult.Empty<TTokenKind, Token<TTokenKind>>(input, expectations);

                return next;
            };
        }

        /// <summary>
        /// Parse a sequence of tokens of the kind <typeparamref name="TTokenKind"/>.
        /// </summary>
        /// <typeparam name="TTokenKind">The kind of tokens being matched.</typeparam>
        /// <param name="tokens">The values to match, in order.</param>
        /// <returns>The matched tokens.</returns>
        public static TokenParser<TTokenKind, Token<TTokenKind>[]> Sequence<TTokenKind>(params TTokenKind[] tokens)
        {
            if (tokens == null) throw new ArgumentNullException(nameof(tokens));

            TokenParser<TTokenKind, Token<TTokenKind>[]> result = input => TokenResult.Value(new Token<TTokenKind>[tokens.Length], input, input);
            for (var i = 0; i < tokens.Length; ++i)
            {
                var token = EqualTo(tokens[i]);
                var index = i;
                result = result.Then(arr => token.Select(t => { arr[index] = t; return arr; }));
            }
            return result;
        }
    }
}

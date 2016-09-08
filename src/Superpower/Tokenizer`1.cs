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
using Superpower.Display;
using Superpower.Model;

namespace Superpower
{
    /// <summary>
    /// Base class for tokenizers, types whose instances convert strings into lists of tokens.
    /// </summary>
    /// <typeparam name="TTokenKind">The kind of tokens produced.</typeparam>
    public abstract class Tokenizer<TTokenKind>
    {
        /// <summary>
        /// Tokenize <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source to tokenize.</param>
        /// <returns>The list of tokens or an error.</returns>
        /// <exception cref="ParseException">Tokenization failed.</exception>
        public TokenList<TTokenKind> Tokenize(string source)
        {
            var result = TryTokenize(source);
            if (result.HasValue)
                return result.Value;

            throw new ParseException(result.ToString());
        }

        /// <summary>
        /// Tokenize <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source to tokenize.</param>
        /// <returns>A result with the list of tokens or an error.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        /// <exception cref="ParseException">The tokenizer could not correctly perform tokenization.</exception>
        public CharResult<TokenList<TTokenKind>> TryTokenize(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var sourceSpan = new StringSpan(source);
            var remainder = sourceSpan;
            var results = new List<Token<TTokenKind>>();
            foreach (var result in Tokenize(sourceSpan))
            {
                if (!result.HasValue)
                    return CharResult.CastEmpty<TTokenKind, TokenList<TTokenKind>>(result);

                if (result.Remainder == remainder) // Broken parser, not a failed parsing.
                    throw new ParseException($"Zero-width tokens are not supported; token {Presentation.FormatExpectation(result.Value)} at position {result.Location.Position}.");

                remainder = result.Remainder;
                var token = new Token<TTokenKind>(result.Value, result.Location.Until(result.Remainder));
                Previous = token;
                results.Add(token);
            }

            var value = new TokenList<TTokenKind>(results.ToArray());
            return CharResult.Value(value, sourceSpan, remainder);
        }

        /// <summary>
        /// The previous token parsed.
        /// </summary>
        protected Token<TTokenKind> Previous { get; private set; }

        /// <summary>
        /// Subclasses should override to perform tokenization.
        /// </summary>
        /// <param name="stringSpan">The input span to tokenize.</param>
        /// <returns>A list of parsed tokens.</returns>
        protected abstract IEnumerable<CharResult<TTokenKind>> Tokenize(StringSpan stringSpan);

        /// <summary>
        /// Advance until the first non-whitespace character is encountered.
        /// </summary>
        /// <param name="span">The span to advance from.</param>
        /// <returns>A result with the first non-whitespace character.</returns>
        protected static CharResult<char> SkipWhiteSpace(StringSpan span)
        {
            var next = span.ConsumeChar();
            while (next.HasValue && char.IsWhiteSpace(next.Value))
            {
                next = next.Remainder.ConsumeChar();
            }
            return next;
        }
    }
}
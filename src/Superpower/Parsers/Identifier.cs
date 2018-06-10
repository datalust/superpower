// Copyright 2018 Datalust, Superpower Contributors, Sprache Contributors
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

using Superpower.Model;

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for matching identifiers in various styles.
    /// </summary>
    public static class Identifier
    {
        /// <summary>
        /// Parse a <code>C_Style</code> identifier.
        /// </summary>
        public static TextParser<TextSpan> CStyle { get; } =
            Span.MatchedBy(
                Character.Letter.Or(Character.EqualTo('_'))
                    .IgnoreThen(Character.LetterOrDigit.Or(Character.EqualTo('_')).Many()));
    }
}

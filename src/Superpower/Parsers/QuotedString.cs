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

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for matching strings in various styles.
    /// </summary>
    public static class QuotedString
    {
        static readonly TextParser<char> SqlStringContentChar =
            Span.EqualTo("''").Value('\'').Try().Or(Character.ExceptIn('\'', '\r', '\n'));

        static readonly TextParser<char> CStringContentChar =
            Span.EqualTo("\\\"").Value('"').Try().Or(Character.ExceptIn('"', '\\', '\r', '\n'));

        /// <summary>
        /// A <code>'SQL-style'</code> string. Single quote delimiters, with embedded single quotes
        /// escaped by '' doubling.
        /// </summary>
        public static TextParser<string> SqlStyle { get; } =
            Character.EqualTo('\'')
                .IgnoreThen(SqlStringContentChar.Many())
                .Then(s => Character.EqualTo('\'').Value(new string(s)));

        /// <summary>
        /// A <code>"C-style"</code> string. Double quote delimiters, with ability to escape
        /// characters by using <code>\"</code>.
        /// </summary>
        public static TextParser<string> CStyle { get; } =
            Character.EqualTo('"')
                .IgnoreThen(CStringContentChar.Many())
                .Then(s => Character.EqualTo('"').Value(new string(s)));
    }
}

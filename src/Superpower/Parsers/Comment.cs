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

using Superpower.Model;

namespace Superpower.Parsers
{
    /// <summary>
    /// Parsers for matching comments in various styles
    /// </summary>
    public static class Comment
    {
        /// <summary>
        /// Parses a comment that begins with a specified pattern and continues to the end of the line.
        /// </summary>
        /// <remarks>
        /// The comment span does not include the end-of-line characters that terminate it.
        /// </remarks>
        /// <param name="beginComment">Recognizes the beginning of the comment.</param>
        /// <returns>The span covered by the comment.</returns>
        public static TextParser<TextSpan> ToEndOfLine(TextParser<TextSpan> beginComment)
        {            
            return i =>
            {
                var begin = beginComment(i);
                if (!begin.HasValue)
                    return begin;

                var remainder = begin.Remainder;
                while (!remainder.IsAtEnd)
                {
                    var ch = remainder.ConsumeChar();
                    if (ch.Value == '\r' || ch.Value == '\n')
                        break;

                    remainder = ch.Remainder;
                }

                return Result.Value(i.Until(remainder), i, remainder);
            };
        }
        
        /// <summary>
        /// Parses a C++ style comment, beginning with a double forward slash `//`
        /// and continuing to the end of the line.
        /// </summary>
        public static TextParser<TextSpan> CPlusPlusStyle { get; } = ToEndOfLine(Span.EqualTo("//"));
        
        /// <summary>
        /// Parses a SQL style comment, beginning with a double dash `--`
        /// and continuing to the end of the line.
        /// </summary>
        public static TextParser<TextSpan> SqlStyle { get; } = ToEndOfLine(Span.EqualTo("--"));
        
        /// <summary>
        /// Parses a shell style comment, beginning with a pound/hash `#` sign
        /// and continuing to the end of the line.
        /// </summary>
        public static TextParser<TextSpan> ShellStyle { get; } = ToEndOfLine(Span.EqualTo("#"));

        /// <summary>
        /// Parses a C-style multiline comment beginning with `/*` and ending with `*/`.
        /// </summary>
        public static TextParser<TextSpan> CStyle
        {
            get
            {
                var beginComment = Span.EqualTo("/*");
                var endComment = Span.EqualTo("*/");
                return i =>
                {
                    var begin = beginComment(i);
                    if (!begin.HasValue)
                        return begin;

                    var content = begin.Remainder;
                    while (!content.IsAtEnd)
                    {
                        var end = endComment(content);
                        if (end.HasValue)
                            return Result.Value(i.Until(end.Remainder), i, end.Remainder);
                            
                        content = content.ConsumeChar().Remainder;
                    }

                    return endComment(content); // Will fail, because we're at the end-of-input.
                };

            }
        }
    }
}
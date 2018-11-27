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

using System.Collections.Generic;
using System.Reflection;
using Superpower.Util;

namespace Superpower.Display
{
    static class Presentation
    {
        static string FormatKind(object kind)
        {
            return kind.ToString().ToLower();
        }

        static TokenAttribute TryGetTokenAttribute<TKind>(TKind kind)
        {
            var kindTypeInfo = typeof(TKind).GetTypeInfo();
            if (kindTypeInfo.IsEnum)
            {
                var field = kindTypeInfo.GetDeclaredField(kind.ToString());
                if (field != null)
                {
                    return field.GetCustomAttribute<TokenAttribute>();
                }
            }

            return null;
        }

        public static string FormatExpectation<TKind>(TKind kind)
        {
            var description = TryGetTokenAttribute(kind);
            if (description != null)
            {
                if (description.Description != null)
                    return description.Description;
                if (description.Example != null)
                    return FormatLiteral(description.Example);
            }

            return FormatKind(kind);
        }

        public static string FormatAppearance<TKind>(TKind kind, string value)
        {
            var clipped = FormatLiteral(Friendly.Clip(value, 12));

            var description = TryGetTokenAttribute(kind);
            if (description != null)
            {
                if (description.Category != null)
                    return $"{description.Category} {clipped}";

                if (description.Example != null)
                    return clipped;
            }

            return $"{FormatKind(kind)} {clipped}";
        }
        public static string FormatLiteral(char literal)
        {
            switch (literal)
            {
     
               

                //Unicode Category: Space Separators
                case '\x0020': return "space";
                case '\x00A0': return "no-break space";
                case '\x1680': return "ogham space mark";
                case '\x2000': return "en quad";
                case '\x2001': return "em quad";
                case '\x2002': return "en space";
                case '\x2003': return "em space";
                case '\x2004': return "three-per-em space";
                case '\x2005': return "four-per-em space";
                case '\x2006': return "siz-per-em space";
                case '\x2007': return "figure space";
                case '\x2008': return "punctuation space";
                case '\x2009': return "thin space";
                case '\x200A': return "hair space";
                case '\x202F': return "narrow no-break space";
                case '\x205F': return "medium mathematical space";
                case '\x3000': return "ideographic space";

                //Line Separator
                case '\x2028': return "line separator";

                //Paragraph Separator
                case '\x2029': return "paragraph separator";
                
                //Ascii Non-Graphical
                case '\r': return "carriage return";
                case '\n': return "line feed";
                case '\t': return "tab";
                case '\0': return "NUL";


                default:
                    return "`" + literal + "`";
            }
        }

        public static string FormatLiteral(string literal)
        {
           
            return "`" + literal + "`";
        }
    }
}

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
        public static string FormatLiteral(char literal) => FormatLiteral(literal, true);
        public static string FormatLiteral(char literal,bool wrapDefault)
        {
            string result;
            switch (literal)
            {
                //Unicode Category: Space Separators
                case '\x0020': result= "space"; break;
                case '\x00A0': result= "no-break space"; break;
                case '\x1680': result= "ogham space mark"; break;
                case '\x2000': result= "en quad"; break;
                case '\x2001': result= "em quad"; break;
                case '\x2002': result= "en space"; break;
                case '\x2003': result= "em space"; break;
                case '\x2004': result= "three-per-em space"; break;
                case '\x2005': result= "four-per-em space"; break;
                case '\x2006': result= "siz-per-em space"; break;
                case '\x2007': result= "figure space"; break;
                case '\x2008': result= "punctuation space"; break;
                case '\x2009': result= "thin space"; break;
                case '\x200A': result= "hair space"; break;
                case '\x202F': result= "narrow no-break space"; break;
                case '\x205F': result= "medium mathematical space"; break;
                case '\x3000': result= "ideographic space"; break;

                //Line Separator
                case '\x2028': result= "line separator"; break;

                //Paragraph Separator
                case '\x2029': result= "paragraph separator"; break;
                
                //Unicode C0 Control Codes (ascii equivalent) 
                case '\x0000': result= "NUL"; break;
                case '\x0001': result= "start of heading"; break;
                case '\x0002': result= "start of text"; break;
                case '\x0003': result= "end of text"; break;
                case '\x0004': result= "end of transmission"; break;
                case '\x0005': result= "enquiry"; break;
                case '\x0006': result= "acknoledge"; break;
                case '\x0007': result= "bell"; break;
                case '\x0008': result= "backspace"; break;
                case '\x0009': result= "tab"; break; //\t
                case '\x000A': result= "line feed"; break; //\n
                case '\x000B': result= "vertical tab"; break;
                case '\x000C': result= "form feed"; break;
                case '\x000D': result= "carriage result="; break;
                case '\x000E': result= "shift in"; break;
                case '\x000F': result= "shift out"; break;
                case '\x0010': result= "data link escape"; break;
                case '\x0011': result= "device ctrl 1"; break;
                case '\x0012': result= "device ctrl 2"; break;
                case '\x0013': result= "device ctrl 3"; break;
                case '\x0014': result= "device ctrl 4"; break;
                case '\x0015': result= "not acknoledge"; break;
                case '\x0016': result= "synchronous idle"; break;
                case '\x0017': result= "end transmission block"; break;
                case '\x0018': result= "cancel"; break;
                case '\x0019': result= "end of medium"; break;
                case '\x001A': result= "substitute"; break;
                case '\x001B': result= "escape"; break;
                case '\x001C': result= "file separator"; break;
                case '\x001D': result= "group separator"; break;
                case '\x001E': result= "record separator"; break;
                case '\x001F': result= "unit separator"; break;
                case '\x007F': result= "delete"; break;

                default: result = literal.ToString(); break;
            }
            return wrapDefault?"`" + result + "`":result;

        }

        public static string FormatLiteral(string literal)
        {
           
            return "`" + literal + "`";
        }
    }
}

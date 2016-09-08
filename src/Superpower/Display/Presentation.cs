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

        static TokenAttribute TryGetTokenAttribute<TTokenKind>(TTokenKind kind)
        {
            var kindTypeInfo = typeof(TTokenKind).GetTypeInfo();
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

        public static string FormatExpectation<TTokenKind>(TTokenKind kind)
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

        public static string FormatAppearance<TTokenKind>(TTokenKind kind, string value)
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
                case '\r':
                    return "carriage return";
                case '\n':
                    return "line feed";
                case '\t':
                    return "tab";
                case '\0':
                    return "NUL";
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

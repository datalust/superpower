using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Superpower.Util
{
    static class Presentation
    {
        public static string FormatKind<TTokenKind>(TTokenKind kind)
        {
            var kindTypeInfo = typeof (TTokenKind).GetTypeInfo();
            if (kindTypeInfo.IsEnum)
            {
                var field = kindTypeInfo.GetDeclaredField(kind.ToString());
                if (field != null)
                {
                    var description = field.GetCustomAttribute<DescriptionAttribute>();
                    if (description != null)
                        return description.Description;
                }

                return kind.ToString().ToLower();
            }

            return kind.ToString().ToLower();
        }

        public static string FormatCharacter(char ch)
        {
            // Should check for unprintable/awkward chars here like `\n` and display appropriately.
            return "`" + ch + "`";
        }

        public static string List(IEnumerable<string> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var list = items.Last();
            var count = items.Count();
            if (count > 1)
                list = $"{string.Join(", ", items.Take(count - 1))} or {list}";
            return list;
        }

        public static string Clip(string value, int maxLength)
        {
            if (value.Length > maxLength)
                return value.Substring(0, maxLength - 3) + "...";
            return value;
        }
    }
}

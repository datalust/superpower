using System;
using System.Collections.Generic;
using System.Linq;

namespace Superpower.Util
{
    static class Friendly
    {
        public static string Pluralize(string noun, int count)
        {
            if (noun == null) throw new ArgumentNullException(nameof(noun));

            if (count == 1)
                return noun;

            return noun + "s";
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
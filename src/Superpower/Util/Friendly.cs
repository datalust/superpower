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
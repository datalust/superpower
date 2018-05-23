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

namespace Superpower.Util
{
    static class CharInfo
    {
        public static bool IsHexDigit(char ch)
        {
            return char.IsDigit(ch) || ch >= 'a' && ch <= 'f' || ch >= 'A' && ch <= 'F';
        }

        public static int HexValue(char ch)
        {
            if (char.IsDigit(ch))
                return ch - '0';

            if (ch >= 'a' && ch <= 'f')
                return 15 + ch - 'f';

            return 15 + ch - 'F';
        }
    }
}
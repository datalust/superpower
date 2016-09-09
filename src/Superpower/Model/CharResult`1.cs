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

using Superpower.Util;
using System;

namespace Superpower.Model
{
    /// <summary>
    /// The result of parsing a character stream.
    /// </summary>
    /// <typeparam name="T">The type of the value being parsed.</typeparam>
    public struct CharResult<T>
    {
        readonly T _value;

        /// <summary>
        /// The location in the stream where the parsing began.
        /// </summary>
        public StringSpan Location { get; }

        /// <summary>
        /// The first un-parsed location in the stream.
        /// </summary>
        public StringSpan Remainder { get; }

        /// <summary>
        /// True if the result carries a successfully-parsed value; otherwise, false.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// The position of the first un-parsed location.
        /// </summary>
        public Position ErrorPosition => Remainder.Position;

        /// <summary>
        /// A provided error message, or null.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// A list of expectations that were unmet, or null.
        /// </summary>
        public string[] Expectations { get; }

        internal bool IsPartial(StringSpan @from) => @from != Remainder;

        /// <summary>
        /// The parsed value.
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new InvalidOperationException("Result has no value.");
                return _value;
            }
        }

        internal CharResult(T value, StringSpan location, StringSpan remainder)
        {
            Location = location;
            Remainder = remainder;
            _value = value;
            HasValue = true;
            ErrorMessage = null;
            Expectations = null;
        }

        internal CharResult(StringSpan remainder, string errorMessage, string[] expectations)
        {
            Location = Remainder = remainder;
            _value = default(T);
            HasValue = false;
            Expectations = expectations;
            ErrorMessage = errorMessage;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Remainder == StringSpan.None)
                return "(Empty result.)";

            if (HasValue)
                return $"Successful parsing of {Value}.";

            var message = FormatErrorMessageFragment();
            var location = "";
            if (!Remainder.IsAtEnd)
            {
                location = $" (line {Remainder.Position.Line}, column {Remainder.Position.Column})";
            }

            return $"Syntax error{location}: {message}.";
        }

        /// <summary>
        /// If the result is empty, format the fragment of text describing the error.
        /// </summary>
        /// <returns>The error fragment.</returns>
        public string FormatErrorMessageFragment()
        {
            if (ErrorMessage != null)
                return ErrorMessage;
            
            string message;
            if (Remainder.IsAtEnd)
            {
                message = "unexpected end of input";
            }
            else
            {
                var next = Remainder.ConsumeChar().Value;
                message = $"unexpected `{next}`";
            }

            if (Expectations != null)
            {
                var expected = Friendly.List(Expectations);
                message += $", expected {expected}";
            }

            return message;
        }
    }
}

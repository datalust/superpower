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
using System.Reflection;

namespace Superpower.Display;

static class Presentation
{
	static string FormatKind(object kind)
	{
		return kind.ToString()!.ToLower();
	}

	static TokenAttribute? TryGetTokenAttribute(Type type)
	{
		return type.GetTypeInfo().GetCustomAttribute<TokenAttribute>();
	}

	static TokenAttribute? TryGetTokenAttribute<TKind>(TKind kind)
	{
		var kindTypeInfo = typeof(TKind).GetTypeInfo();
		if (kindTypeInfo.IsEnum)
		{
			var field = kindTypeInfo.GetDeclaredField(kind!.ToString()!);
			if (field != null)
			{
				return field.GetCustomAttribute<TokenAttribute>() ?? TryGetTokenAttribute(typeof(TKind));
			}
		}

		return TryGetTokenAttribute(typeof(TKind));
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

		return FormatKind(kind!);
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

		return $"{FormatKind(kind!)} {clipped}";
	}
	public static string FormatLiteral(char literal) => literal switch
	{
		//Unicode Category: Space Separators
		'\x00A0' => "U+00A0 no-break space",
		'\x1680' => "U+1680 ogham space mark",
		'\x2000' => "U+2000 en quad",
		'\x2001' => "U+2001 em quad",
		'\x2002' => "U+2002 en space",
		'\x2003' => "U+2003 em space",
		'\x2004' => "U+2004 three-per-em space",
		'\x2005' => "U+2005 four-per-em space",
		'\x2006' => "U+2006 six-per-em space",
		'\x2007' => "U+2007 figure space",
		'\x2008' => "U+2008 punctuation space",
		'\x2009' => "U+2009 thin space",
		'\x200A' => "U+200A hair space",
		'\x202F' => "U+202F narrow no-break space",
		'\x205F' => "U+205F medium mathematical space",
		'\x3000' => "U+3000 ideographic space",
		//Line Separator
		'\x2028' => "U+2028 line separator",
		//Paragraph Separator
		'\x2029' => "U+2029 paragraph separator",
		//Unicode C0 Control Codes (ASCII equivalent) 
		'\x0000' => "NUL",//\0
		'\x0001' => "U+0001 start of heading",
		'\x0002' => "U+0002 start of text",
		'\x0003' => "U+0003 end of text",
		'\x0004' => "U+0004 end of transmission",
		'\x0005' => "U+0005 enquiry",
		'\x0006' => "U+0006 acknowledge",
		'\x0007' => "U+0007 bell",
		'\x0008' => "U+0008 backspace",
		'\x0009' => "tab",//\t
		'\x000A' => "line feed",//\n
		'\x000B' => "U+000B vertical tab",
		'\x000C' => "U+000C form feed",
		'\x000D' => "carriage return",//\r
		'\x000E' => "U+000E shift in",
		'\x000F' => "U+000F shift out",
		'\x0010' => "U+0010 data link escape",
		'\x0011' => "U+0011 device ctrl 1",
		'\x0012' => "U+0012 device ctrl 2",
		'\x0013' => "U+0013 device ctrl 3",
		'\x0014' => "U+0014 device ctrl 4",
		'\x0015' => "U+0015 not acknowledge",
		'\x0016' => "U+0016 synchronous idle",
		'\x0017' => "U+0017 end transmission block",
		'\x0018' => "U+0018 cancel",
		'\x0019' => "U+0019 end of medium",
		'\x0020' => "space",
		'\x001A' => "U+001A substitute",
		'\x001B' => "U+001B escape",
		'\x001C' => "U+001C file separator",
		'\x001D' => "U+001D group separator",
		'\x001E' => "U+001E record separator",
		'\x001F' => "U+001F unit separator",
		'\x007F' => "U+007F delete",
		_ => "`" + literal + "`",
	};

	public static string FormatLiteral(string literal)
	{
		return $"`{literal}`";
	}
}

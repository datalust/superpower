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

namespace Superpower.Model;

/// <summary>
/// The result of parsing from a text span.
/// </summary>
/// <typeparam name="T">The type of the value being parsed.</typeparam>
public readonly record struct Result<T>
{
	/// <summary>
	/// If the result is a value, the location in the input corresponding to the
	/// value. If the result is an error, it's the location of the error.
	/// </summary>
	public TextSpan Location { get; }

	/// <summary>
	/// The first un-parsed location in the input.
	/// </summary>
	public TextSpan Remainder { get; }

	/// <summary>
	/// True if the result carries a successfully-parsed value; otherwise, false.
	/// </summary>
	public bool HasValue { get; }

	/// <summary>
	/// If the result is an error, the source-level position of the error; otherwise, <see cref="Position.Empty"/>.
	/// </summary>
	public readonly Position ErrorPosition => HasValue ? Position.Empty : Location.Position;

	/// <summary>
	/// A provided error message, or null.
	/// </summary>
	public string? ErrorMessage { get; }

	/// <summary>
	/// A list of expectations that were unmet, or null.
	/// </summary>
	public string[]? Expectations { get; }

	internal readonly bool IsPartial(TextSpan from) => from != Remainder;

	internal bool Backtrack { get; }

	/// <summary>
	/// Returns a new <see cref="Result{T}"/> instance with the specified backtrack setting.
	/// </summary>
	/// <param name="backtrack">A boolean value indicating whether backtracking is enabled.  If <see langword="true"/>, backtracking is enabled;
	/// otherwise, it is disabled.</param>
	/// <returns>A new <see cref="Result{T}"/> instance with the updated backtrack setting,  or the current instance if the
	/// backtrack setting is unchanged.</returns>
	public Result<T> WithBacktrack(bool backtrack)
	{
		if (backtrack == Backtrack)
			return this;
		return new Result<T>(Location, Remainder, ErrorMessage, Expectations, backtrack);
	}

	/// <summary>
	/// The parsed value.
	/// </summary>
	public readonly T Value
	{
		get
		{
			if (!HasValue)
				throw new InvalidOperationException($"{nameof(Result)} has no value.");
			return field;
		}
	}

	internal Result(T value, TextSpan location, TextSpan remainder, bool backtrack)
	{
		Location = location;
		Remainder = remainder;
		Value = value;
		HasValue = true;
		ErrorMessage = null;
		Expectations = null;
		Backtrack = backtrack;
	}

	internal Result(TextSpan location, TextSpan remainder, string? errorMessage, string[]? expectations, bool backtrack)
	{
		Location = location;
		Remainder = remainder;
		Value = default!; // Default value is not observable.
		HasValue = false;
		Expectations = expectations;
		ErrorMessage = errorMessage;
		Backtrack = backtrack;
	}

	internal Result(TextSpan remainder, string? errorMessage, string[]? expectations, bool backtrack)
	{
		Location = Remainder = remainder;
		Value = default!; // Default value is not observable.
		HasValue = false;
		Expectations = expectations;
		ErrorMessage = errorMessage;
		Backtrack = backtrack;
	}

	/// <inheritdoc />
	public override readonly string ToString()
	{
		if (Remainder == TextSpan.None)
			return "(Empty result.)";

		if (HasValue)
			return $"Successful parsing of {Value}.";

		var message = FormatErrorMessageFragment();
		var location = "";
		if (!Location.IsAtEnd)
		{
			location = $" (line {Location.Position.Line}, column {Location.Position.Column})";
		}

		return $"Syntax error{location}: {message}.";
	}

	/// <summary>
	/// If the result is empty, format the fragment of text describing the error.
	/// </summary>
	/// <returns>The error fragment.</returns>
	public readonly string FormatErrorMessageFragment()
	{
		if (ErrorMessage != null)
			return ErrorMessage;

		string message;
		if (Location.IsAtEnd)
		{
			message = "unexpected end of input";
		}
		else
		{
			var next = Location.ConsumeChar().Value;
			message = $"unexpected {Display.Presentation.FormatLiteral(next)}";
		}

		if (Expectations != null)
		{
			var expected = Friendly.List(Expectations);
			message += $", expected {expected}";
		}

		return message;
	}
}

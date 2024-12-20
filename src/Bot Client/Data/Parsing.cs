﻿using System.Text.RegularExpressions;
using System.Text;

namespace Bot.Data;
internal static partial class Parsing
{
	/// <summary>
	/// Replaces any unparsed unicode identifiers with their appropriate symbols (i.e. <c>\u0041' to 'A'</c>).
	/// </summary>
	public static string FromEscapedUnicode(this string unparsed)
	{
		bool ContainsIdentifier = false, ContainsEscape = false;

		if (!unparsed.Contains('\\')) return unparsed;

		int Position = 0;
		StringBuilder Result = new();
		foreach (Match match in UnicodeIdentifierRegex().Matches(unparsed))
		{
			ContainsIdentifier = true;
			Result.Append(unparsed, Position, match.Index - Position);
			Position = match.Index + match.Length;
			Result.Append((char)Convert.ToInt32(match.Groups[1].ToString(), 16));
		}
		Result.Append(unparsed, Position, unparsed.Length - Position);

		unparsed = ContainsIdentifier ? Result.ToString() : unparsed;
		Result.Clear();
		Position = 0;

		foreach (Match match in EscapeCharacterRegex().Matches(unparsed))
		{
			ContainsEscape = true;
			Result.Append(unparsed, Position, match.Index - Position);
			Position = match.Index + match.Length;
			Result.Append(match.ValueSpan[1] switch
			{
				'b' => "\b",
				'f' => "\f",
				'n' => "\n",
				'r' => "\r",
				't' => "\t",
				'"' => "\"",
				'\\' => "\\",
				_ => throw new InvalidOperationException()
			});
		}

		return ContainsEscape ? Result.ToString() : unparsed;
	}

	/// <summary>
	/// Converts the contents of this string into an escaped version, avoiding markdown formatting issues.
	/// </summary>
	public static string ToEscapedMarkdown(this string unparsed)
	{
		StringBuilder Parsed = new(unparsed.Length + 20);

		foreach (char c in unparsed)
		{
			switch (c)
			{
				default:
					Parsed.Append(c);
					break;

				case '*' or '_' or '#' or '-' or '~' or '`' or '(' or ')' or '[' or ']' or '>':
					Parsed.Append('\\');
					Parsed.Append(c);
					break;
			}
		}

		return Parsed.ToString();
	}

	[GeneratedRegex(@"\\u([0-9A-Fa-f]{4})")]
	private static partial Regex UnicodeIdentifierRegex();

	[GeneratedRegex(@"\\(b|f|n|r|t|""|\\)")]
	private static partial Regex EscapeCharacterRegex();
}

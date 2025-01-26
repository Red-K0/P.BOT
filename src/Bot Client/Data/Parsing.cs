using System.Text.RegularExpressions;
using System.Text;

namespace Bot.Data;

/// <summary>
/// Contains extensions for parsing and handling formatted text, such as markdown.
/// </summary>
internal static partial class Parsing
{
	/// <summary>
	/// Used for parsing markdown strings efficiently.
	/// </summary>
	private static readonly StringBuilder _markdownBuilder = new(128);

	[GeneratedRegex(@"\\(b|f|n|r|t|""|\\)")]
	private static partial Regex EscapeCharacterRegex();

	/// <summary>
	/// Replaces any unparsed unicode identifiers with their appropriate symbols (i.e. <c>\u0041' to 'A'</c>).
	/// </summary>
	public static string FromEscapedUnicode(this string unparsed)
	{
		bool containsIdentifier = false, containsEscape = false;

		if (!unparsed.Contains('\\')) return unparsed;

		int position = 0;
		StringBuilder result = new();
		foreach (Match match in UnicodeIdentifierRegex().Matches(unparsed))
		{
			containsIdentifier = true;
			result.Append(unparsed, position, match.Index - position);
			position = match.Index + match.Length;
			result.Append((char)Convert.ToInt32(match.Groups[1].ToString(), 16));
		}
		result.Append(unparsed, position, unparsed.Length - position);

		unparsed = containsIdentifier ? result.ToString() : unparsed;
		result.Clear();
		position = 0;

		foreach (Match match in EscapeCharacterRegex().Matches(unparsed))
		{
			containsEscape = true;
			result.Append(unparsed, position, match.Index - position);
			position = match.Index + match.Length;
			result.Append(match.ValueSpan[1] switch
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

		return containsEscape ? result.ToString() : unparsed;
	}

	/// <summary>
	/// Converts the contents of a string into a markdown-safe escaped version, avoiding formatting issues.
	/// </summary>
	public static string ToEscapedMarkdown(this string unparsed)
	{
		_markdownBuilder.Clear();

		_markdownBuilder.EnsureCapacity(unparsed.Length * 2);

		foreach (char c in unparsed)
		{
			switch (c)
			{
				default:
					_markdownBuilder.Append(c);
					break;

				case '*' or '_' or '#' or '-' or '~' or '`' or '(' or ')' or '[' or ']' or '>':
					_markdownBuilder.Append('\\');
					_markdownBuilder.Append(c);
					break;
			}
		}

		return _markdownBuilder.ToString();
	}

	[GeneratedRegex(@"\\u([0-9A-Fa-f]{4})")]
	private static partial Regex UnicodeIdentifierRegex();
}

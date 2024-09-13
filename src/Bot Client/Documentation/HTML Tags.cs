using System.Text.RegularExpressions;

namespace Bot.Documentation;
internal static partial class Tags
{
	[GeneratedRegex(@"\uEEEC")]
	public static partial Regex MemberTagRegex();

	/// <summary>
	/// Formats the final HTML document to have proper spacing for code readability.
	/// </summary>
	public static string IndentTags(string str)
	{
		return str
		.Replace("\0", null)
		.Replace("<details>", "\r\n\t\t<details>").Replace("</details>", "\r\n\t\t</details>")
		.Replace("<summary>", "\r\n\t\t\t<summary>\r\n\t\t\t\t").Replace("</summary>", "\r\n\t\t\t</summary>\r\n\t\t\t")
		+ "\r\n\t</body>\r\n</html>";
	}

	/// <summary>
	/// Represents the safe replacement character for its equivalent XML entity.
	/// </summary>
	public const string LT = "\uEEEA", GT = "\uEEEB";

	/// <summary>
	/// Part of the <c>code</c> tag pair, used for font application.
	/// </summary>
	public const string Code = $"{LT}code{GT}", cCode = $"{LT}/code{GT}";

	/// <summary>
	/// Part of the <c>blue</c> tag pair, used for highlighting.
	/// </summary>
	public const string Blue = $"{LT}blue{GT}", cBlue = $"{LT}/blue{GT}";

	/// <summary>
	/// Part of the <c>cyan</c> tag pair, used for highlighting.
	/// </summary>
	public const string Cyan = $"{LT}cyan{GT}", cCyan = $"{LT}/cyan{GT}";

	/// <summary>
	/// Part of the <c>details</c> tag pair, used for formatting.
	/// </summary>
	public const string Details = $"{LT}details{GT}", cDetails = $"{LT}/details{GT}";

	/// <summary>
	/// Part of the <c>summary</c> tag pair, used for formatting.
	/// </summary>
	public const string Summary = $"{LT}summary{GT}", cSummary = $"{LT}/summary{GT}";

	/// <summary>
	/// Indicate the start and end of a member name section, used in <see cref="Generator.GetMemberSummaryPairs"/>
	/// </summary>
	public const string MemberOpen = "\uEEEC", MemberClose = "\uEEED";

	/// <summary>
	/// Indicate the start and end of a summary section, used in <see cref="Generator.GetMemberSummaryPairs"/>
	/// </summary>
	public const string SummaryOpen = "\uEEEE", SummaryClose = "\uEEEF";
}

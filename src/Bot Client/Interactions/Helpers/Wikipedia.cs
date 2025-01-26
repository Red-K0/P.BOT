using System.Text.RegularExpressions;
namespace Bot.Interactions.Helpers;

/// <summary>
/// Contains methods for interacting with the Wikipedia API, and handling requests.
/// </summary>
internal static partial class Wikipedia
{
	/// <summary>
	/// The URL and preset parameters for the Wikipedia content API.
	/// </summary>
	private const string _root = "https://en.wikipedia.org/w/api.php?action=query&generator=prefixsearch&redirects=1&gpslimit=1&explaintext=0&format=json&prop=extracts&gpssearch=";

	[GeneratedRegex(@":\d\.\d|\[\d+\]")]
	private static partial Regex CitationRegex();

	/// <summary>
	/// Sends the request responsible for retrieving a Wikipedia page, and formats it appropriately.
	/// </summary>
	/// <param name="searchTerm">The term to retrieve a page for.</param>
	/// <param name="full">Whether to attempt to extract a term's full page.</param>
	public static async Task<string> GetPage(string searchTerm, bool full)
	{
		string page = await NetClient.GetStringAsync(_root + searchTerm + (full ? "" : "&exintro"));

		string title = page[(page.IndexOf("title\"") + 8)..];
		title = title.Remove(title.IndexOf('"'));

		if (!page.Contains("xtract")) return $"Wikipedia does not have a definition for '{searchTerm}'.";

		// While the response is truncated to 4000 characters later, 5000 gives some leeway for formatting.
		if (page.Length >= 5000) page = page[..5000];

		page = page[(page.IndexOf("xtract") + 9)..^5].FromEscapedUnicode().Replace(":\n", ".\n");

		CitationRegex().Replace(page, _ => "");

		page = $"{(full ? "##" : "###")} [{title.FromEscapedUnicode()}](https://en.wikipedia.org/wiki/{System.Web.HttpUtility.UrlEncode(title.Replace(' ', '_'))})\n{page}";

		if (full) FormatHeaders(ref page);

		return page.Length >= 4000 ? $"{page[..4000]}..." : page;
	}

	/// <summary>
	/// Applies varying types of formatting to page headers.
	/// </summary>
	/// <param name="page">The page to format.</param>
	private static void FormatHeaders(ref string page)
	{
		// HEADERS_REMOVE_EMPTY
		// The following section is to remove empty headers that look like the following:
		//
		// == Header1 ==
		//
		//
		// == Header2 ==
		//
		// This is usually caused by a table or some other element that can't be communicated in text.
		// The 'References' and 'External Links' headers are also common culrpits, and are therefore removed.

		while (page.Contains("\n\n\n")) page = page.Replace("\n\n\n", "\n\n");

		if (page.Contains("\n== References")) page = page.Remove(page.IndexOf("\n== References"));

		int emptyHeaderEndIndex;
		while ((emptyHeaderEndIndex = page.IndexOf("==\n\n== ")) != -1)
		{
			bool equalsTrigger = false;
			int emptyHeaderStartIndex = 0;

			for (int i = emptyHeaderEndIndex - 1; i >= 0; i--)
			{
				if (!equalsTrigger)
				{
					if (page[i] == '=') equalsTrigger = true;
				}
				else if (page[i] != '=')
				{
					emptyHeaderStartIndex = i + 1;
					break;
				}
			}

			ReadOnlySpan<char> response = page.AsSpan();
			page = string.Concat(response[..emptyHeaderStartIndex], response[(emptyHeaderEndIndex + 4)..]);
		}

		// HEADERS_USE_MARKDOWN
		// The following section is to apply markdown formatting to page responses, such as the following:
		//
		// "== Header1 ==" to "### Header1"
		//
		// "=== Header2 ===" to "**Header2**"

		page = page
			.Replace( "\n===", "\n**")
			.Replace(  "\n==",  "###")
			.Replace( " ==\n",   "\n")
			.Replace(" ===\n", "**\n")
			.Replace("**\n\n", "**\n");
	}
}
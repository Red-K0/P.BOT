using System.Text.RegularExpressions;
namespace Bot.Interactions.Helpers;

/// <summary>
/// Contains the Wikipedia API constant, as well as the method responsible for page retrieval.
/// </summary>
internal static partial class Wikipedia
{
	/// <summary>
	/// The URL and preset parameters for the Wikipedia content API.
	/// </summary>
	private const string WIKI_API = "https://en.wikipedia.org/w/api.php?action=query&generator=prefixsearch&redirects=1&gpslimit=1&explaintext=0&format=json&prop=extracts&gpssearch=";

	/// <summary>
	/// Sends the request responsible for retrieving the page, and formats it properly.
	/// </summary>
	public static async Task<string> GetPage(string searchTerm, bool full)
	{
		string Page = await NetClient.GetStringAsync(WIKI_API + searchTerm + (full ? "" : "&exintro"));

		string Title = Page[(Page.IndexOf("title\"") + 8)..];
		Title = Title.Remove(Title.IndexOf('"'));

		if (!Page.Contains("xtract")) return $"Wikipedia does not have a definition for '{searchTerm}'.";

		// While the response is truncated to 4000 characters later, 5000 gives some leeway for formatting.
		if (Page.Length >= 5000) Page = Page[..5000];

		Page = Page[(Page.IndexOf("xtract") + 9)..^5].FromEscapedUnicode().Replace(":\n", ".\n");

		CitationRegex().Replace(Page, _ => "");

		Page = $"{(full ? "##" : "###")} [{Title.FromEscapedUnicode()}](https://en.wikipedia.org/wiki/{System.Web.HttpUtility.UrlEncode(Title.Replace(' ', '_'))})\n{Page}";

		if (full) FormatHeaders(ref Page);

		return Page.Length >= 4000 ? $"{Page[..4000]}..." : Page;
	}

	/// <summary>
	/// Applies varying types of formatting to page headers.
	/// </summary>
	public static void FormatHeaders(ref string page)
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

		int EmptyHeaderEndingIndex;
		while ((EmptyHeaderEndingIndex = page.IndexOf("==\n\n== ")) != -1)
		{
			bool EqualsTrigger = false;
			int EmptyHeaderStartIndex = 0;

			for (int i = EmptyHeaderEndingIndex - 1; i >= 0; i--)
			{
				if (!EqualsTrigger)
				{
					if (page[i] == '=') EqualsTrigger = true;
				}
				else if (page[i] != '=')
				{
					EmptyHeaderStartIndex = i + 1;
					break;
				}
			}

			ReadOnlySpan<char> ResponseSpan = page.AsSpan();
			page = string.Concat(ResponseSpan[..EmptyHeaderStartIndex], ResponseSpan[(EmptyHeaderEndingIndex + 4)..]);
		}

		// HEADERS_USE_MARKDOWN
		// The following section is to apply markdown formatting to page responses, such as the following:
		//
		// "== Header1 ==" to "### Header1"
		//
		// "=== Header2 ===" to "**Header2**"

		page = page
			.Replace("\n===", "\n**")
			.Replace("\n==", "###")
			.Replace(" ==\n", "\n")
			.Replace(" ===\n", "**\n")
			.Replace("**\n\n", "**\n");
	}

	[GeneratedRegex(@":\d\.\d|\[\d+\]")]
	private static partial Regex CitationRegex();
}
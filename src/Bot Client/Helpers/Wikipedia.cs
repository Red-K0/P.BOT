// The helper class supporting the /wikidefine command.

using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using PBot.Caches;

namespace PBot.Commands.Helpers;

/// <summary>
/// Contains the Wikipedia API constant, as well as the method responsible for page retrieval.
/// </summary>
internal static partial class Wikipedia
{
	/// <summary>
	/// The URL and preset parameters for the Wikipedia content API.
	/// </summary>
	private const string WIKI_API = "https://en.wikipedia.org/w/api.php?action=query&generator=prefixsearch&redirects&gpslimit=1&explaintext=0&format=json&prop=extracts&gpssearch=";

	/// <summary>
	/// Sends the request responsible for retrieving the page, and formats it properly.
	/// </summary>
	public static async Task<string> GetPage(string searchTerm, bool longFormat)
	{
		string Query = WIKI_API + searchTerm + (longFormat ? "" : "&exintro");

		string Response = await NetClient.GetStringAsync(Query);

		if (!Response.Contains("xtract")) return $"Wikipedia does not have a definition for '{searchTerm}'.";
		if (Response.Length >= 4000) Response = $"{Response[..4000]}...";

		Response = Response[(Response.IndexOf("xtract") + 9)..^5].ToParsedUnicode();

		CitationRegex().Replace(Response, _ => "");

		while (Response.Contains("\n\n\n")) Response = Response.Replace("\n\n\n", "\n\n");

		if (!longFormat) Response = Response.Remove(Response.IndexOf('\n'));

		return Response;
	}

	[GeneratedRegex(@":\d\.\d|\[\d+\]")]
	private static partial Regex CitationRegex();
}
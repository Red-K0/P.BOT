// The helper class supporting the /wikidefine command.

namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains the Wikipedia API constant, as well as the method responsible for page retrieval.
/// </summary>
public static class Wikipedia
{
	/// <summary>
	/// The URL and preset parameters for the Wikipedia content API.
	/// </summary>
	private const string WIKI_API = "https://en.wikipedia.org/w/api.php?action=query&generator=prefixsearch&redirects=1&gpslimit=1&explaintext=0&format=json&prop=extracts&";

	/// <summary>
	/// Sends the request responsible for retrieving the page, and formats it properly.
	/// </summary>
	public static async Task<string> GetPage(string search_term, bool long_format)
	{
		string Query = WIKI_API + $"gpssearch={search_term}{(long_format ? "" : "&exintro=1")}";
		string FailResponse = $"Wikipedia does not have a definition for '{search_term}'.";

		string Response = await client_h.GetStringAsync(Query);
		Response = Response[(Response.IndexOf(",\"extract\":\"") + 12)..];

		// If there's a response, this won't fail.
		if (Response.Contains("}}}}")) Response = Response.Remove(Response.IndexOf("}}}}") - 1); else return FailResponse;
		if (string.IsNullOrWhiteSpace(Response)) return FailResponse;
		if (!long_format && Response.Contains("\\n")) Response = Response.Remove(Response.IndexOf("\\n"));

		// The next section exists solely to parse the response and return actually useful text.
		// Fuck Wikipedia and whoever wrote the code responsible for returning this horrible mess.

		// Replace inconsistently spaced newlines with inline newlines, and then replace all inline occurrences.
		Response = Response.Replace(" \\n", "\\n");
		Response = Response.Replace("\\n", "\n");

		// Limit the response to 4096 characters if it's longer, appending a '...' in place of the final 3 characters as clarification.
		if (Response.Length > 4096) Response = Response[..4093] + "...";

		// Convert the response to an array of chars.
		char[] CharArray = Response.ToCharArray();

		// Fix for escape characters in raw text, such as "\u2014" instead of '—' (U+2014 | Em Dash).
		for (int i = 0; i < CharArray.Length - 1; i++)
		{
			// If a raw text unicode identifier is present (\u****).
			if (CharArray[i] == '\\' && CharArray[i + 1] == 'u')
			{
				// Get the chars present after "\u", and merge them into one identifier.
				string Identifier = CharArray[i + 2].ToString()
								  + CharArray[i + 3].ToString()
								  + CharArray[i + 4].ToString()
								  + CharArray[i + 5].ToString();

				// Convert the Identifier into a char value, and replace the '\' w
				// ith it.
				CharArray[i] = Convert.ToChar(int.Parse(Identifier, System.Globalization.NumberStyles.HexNumber));

				// Replace the 'u' and identifier characters with null '\0' characters.
				CharArray[i + 1] = '\0';
				CharArray[i + 2] = '\0';
				CharArray[i + 3] = '\0';
				CharArray[i + 4] = '\0';
				CharArray[i + 5] = '\0';
			}
		}

		// Horrible hack to make the conversion faster.
		// Stores the array in the response variable temporarily to remove null characters.
		Response = new(CharArray);
		Response = Response.Replace("\0", "");

		// Copy char array to string array.
		string[] StringArray = new string[Response.Length];
		for (int i = 0; i < Response.Length; i++) { StringArray[i] = Response[i].ToString(); }

		// Format the text properly.
		for (int i = 0; i < StringArray.Length - 1; i++)
		{
			// Fix for the '[1]' bug, where a citation replaces a necessary newline.
			// Avoids scenarios such as the following: "This paragraph ends here.[1]This one starts here."
			if (StringArray[i] == "." && StringArray[i + 1] is not (" " or " " or "." or ":" or "\\") && StringArray[i + 1][0] - 30 < 10 && StringArray[i + 2] != ".")
			{
				StringArray[i] = ".\n\n";
			}

			// Remove random citation notes, such as ':1.1'.
			if (StringArray[i] == ":" && StringArray[i + 1] is not (" " or "\\" or "\n") && (StringArray[i - 1][0] - 30) > 10)
			{
				StringArray[i] = "\0";
				StringArray[i + 1] = "\0";
				StringArray[i + 2] = "\0";
				StringArray[i + 3] = "\0";
				StringArray[i + 4] = "\0";
			}
		}

		// Stores the newly formatted array in the response variable.
		Response = string.Concat(StringArray);

		// If the long format was not requested, cuts off the text at the first (proper) newline, otherwise attempts to clean up excess newlines.
		if (!long_format && Response.Contains('\n')) try { Response = Response.Remove(Response.IndexOf('\n')); } catch { }
		else try { while (Response.Contains("\n\n\n")) Response = Response.Replace("\n\n\n", "\n\n"); } catch { }

		return Response;
	}
}
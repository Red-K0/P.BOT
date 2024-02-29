// Get Commands Extension File
// This file contains commands that are relevant to the bot's getter functions, local and online.
// An example of these commands is the GetAvatar() command, which gets a user's avatar.
// Commands in this file rely frequently on web requests, and the related mess there.

using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;
using static P_BOT.Embeds;

namespace P_BOT.Command_Processing;
public sealed partial class SlashCommand
{
	#region Attributes
	[SlashCommand("getavatar", "Get a user's avatar.")]
	public partial Task GetAvatar
	(
		[SlashCommandParameter(Name = "user", Description = "The ID of the account to pull an avatar from. Works for non-server members as well.")]
		User user,

		[SlashCommandParameter(Name = "format", Description = "The format of the image result, the GIF and Lottie formats are currently unsupported.")]
		ImageFormat format = ImageFormat.Png
	);
	#endregion

	/// <summary>
	/// Command task. Gets the avatar of the <paramref name="user"/>, in a specific format if specified in <paramref name="format"/>.
	/// </summary>
	public async partial Task GetAvatar(User user, ImageFormat format)
	{
		user ??= Context.User;
		MessageProperties msg_prop = Generate
		(
			user.HasAvatar ?
			$"Sure, [here]({user.GetAvatarUrl(format)}) is <@{user.Id}>'s avatar " :
			$"Sorry, <@{user.Id}> does not currently have an avatar set, [here]({user.DefaultAvatarUrl}) is the default discord avatar",

			CreateAuthorObject($"{user.Username}'s Avatar", user.GetAvatarUrl(ImageFormat.Png).ToString()),
			DateTimeOffset.UtcNow,
			ReplyTo: Context.User.Id,
			ImageURL: new(user.GetAvatarUrl(format).ToString())
		);

		await RespondAsync(InteractionCallback.Message(new() { Embeds = msg_prop.Embeds, AllowedMentions = AllowedMentionsProperties.None }));
	}

	#region Attributes
	[SlashCommand("define", "Define a given term.")]
	public partial Task GetDefinition
	(
		[SlashCommandParameter(Name = "term", Description = "The term to reply with a definition for.")]
		Definition.Choices term
	);
	#endregion

	/// <summary>
	/// Command task. Gets the definition of the term specified in the <paramref name="term"/> parameter.
	/// </summary>
	public async partial Task GetDefinition(Definition.Choices term)
	{
		Definition.Values.TryGetValue(term, out string? definition);
		MessageProperties msg_prop = Generate
		(
			definition,
			CreateAuthorObject("PPP Encyclopedia", ASSETS + "Define&20Icon.png"),
			DateTimeOffset.UtcNow,
			ReplyTo: Context.User.Id
		);

		await RespondAsync(InteractionCallback.Message(new() { Embeds = msg_prop.Embeds }));
	}

	#region Attributes
	[SlashCommand("translate", "Translate a given input from one language to another.")]
	public partial Task GetTranslation
	(
		[SlashCommandParameter(Name = "text", Description = "The text to translate, limited to 623 characters.", MaxLength = 623)]
		string input,

		[SlashCommandParameter(Name = "original_language", Description = "The language of the original text, defaults to English (en).")]
		Translation.Choices source_lang = Translation.Choices.en,

		[SlashCommandParameter(Name = "target_language", Description = "The language to translate the text to, defaults to Japanese (ja).")]
		Translation.Choices target_lang = Translation.Choices.ja
	);

	#endregion

	/// <summary>
	/// Translates a given <see cref="string"/> from the <paramref name="source_lang"/> to the <paramref name="target_lang"/>, and responds with the output.
	/// </summary>
	public async partial Task GetTranslation(string input, Translation.Choices source_lang, Translation.Choices target_lang)
	{
		// Make sure the interaction doesn't time out
		await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

		MessageProperties msg_prop = Generate
		(
			await Translation.Process(input, source_lang, target_lang),
			CreateAuthorObject("Translation Processed", ASSETS + "Translator%20Icon.png"),
			DateTime.Now,
			CreateFooterObject($"Translation requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			0x72767D
		);
		await Context.Interaction.SendFollowupMessageAsync(new() { Embeds = msg_prop.Embeds });
	}

	#region Attributes & Constants
	/// <summary>
	/// The URL and preset parameters for the Wikipedia content API.
	/// </summary>
	private const string WIKI_API = "https://en.wikipedia.org/w/api.php?action=query&generator=prefixsearch&redirects=1&gpslimit=1&explaintext=0&format=json&prop=extracts&";

	[SlashCommand("wikidefine", "Define a given term via Wikipedia.")]
	public partial Task GetWikiResult
	(
		[SlashCommandParameter(Name = "search_term", Description = "The term to find a page for if possible.")]
		string search_term,

		[SlashCommandParameter(Name = "full_page", Description = "Should the full page's contents be fetched?")]
		bool long_format = false
	);
	#endregion

	/// <summary>
	/// Searches for a Wikipedia page similar to the given <paramref name="search_term"/>, and gets its content if a page is found.
	/// </summary>
	public async partial Task GetWikiResult(string search_term, bool long_format)
	{
		// Make sure the interaction doesn't time out
		await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

		bool NoResponse = false;

		string Query = WIKI_API + $"gpssearch={search_term}{(long_format ? "" : "&exintro=1")}";
		string Response = await client_h.GetStringAsync(Query);
		Response = Response[(Response.IndexOf(",\"extract\":\"") + 12)..];

		// If there's a response, this won't fail.
		if (Response.Contains("}}}}")) { Response = Response.Remove(Response.IndexOf("}}}}") - 1); } else { NoResponse = true; goto FailCheckpoint; }
		if (!long_format && Response.Contains("\\n")) Response = Response.Remove(Response.IndexOf("\\n"));

		// The next section exists solely to parse the response and return actually useful text.
		// Fuck Wikipedia and whoever wrote the code responsible for returning this horrible mess.
		#region Wikipedia Response Formatting

		// Replace inconsistently spaced newlines with inline newlines, and then replace all inline occurrences.
		Response = Response.Replace(" \\n", "\\n");
		Response = Response.Replace("\\n",   "\n");

		// Limit the response to 4096 characters if it's longer, appending a '...' in place of the final 3 characters as clarification.
		if (Response.Length > 4096) Response = Response[..4093] + "...";

		// Convert the response to an array of chars.
		char[] CharArray = Response.ToCharArray();

		// Fix for escape characters in raw text, such as "\u2014" instead of '—' (U+2014 | Em Dash).
		for (int i = 0; i < CharArray.Length -1; i++)
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
			if (StringArray[i] == "." && StringArray[i + 1] is not (" " or " " or "." or ":" or "\\") && StringArray[i + 1][0]-30 < 10 && StringArray[i + 2] != ".")
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
		if   (!long_format && Response.Contains('\n')) try { Response = Response.Remove(Response.IndexOf('\n')); } catch { }
		else try { while (Response.Contains("\n\n\n")) Response = Response.Replace("\n\n\n", "\n\n"); } catch { }

		#endregion

	FailCheckpoint:
		MessageProperties msg_prop = Generate
		(
			NoResponse ? $"Wikipedia does not have a definition for '{search_term}'." : Response,
			CreateAuthorObject("Wikipedia", ASSETS + "Wikipedia%20Icon.png"),
			DateTime.Now,
			CreateFooterObject($"Definition requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			0x72767D
		);

		await Context.Interaction.SendFollowupMessageAsync(new() { Embeds = msg_prop.Embeds });
	}
}
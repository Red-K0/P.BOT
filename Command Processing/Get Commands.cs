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
		const ulong BOT_ID = 1169031557848252516;

		#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
		#endif

		user ??= Context.User;
		MessageProperties msg_prop = (user.Id == BOT_ID) ?
		Generate
		(
			$"Sure, [here]({ASSETS}/Bot%20Icon.png) is my avatar, if you require it in a format other than PNG, please contact <@1124777547687788626>.",
			CreateAuthorObject($"{user.Username}'s Avatar", user.GetAvatarUrl(ImageFormat.Png).ToString()),
			DateTimeOffset.UtcNow,
			CreateFooterObject($"Avatar requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			ReplyTo: Context.User.Id,
			ImageURL: new($"{ASSETS}/Bot%20Icon.png"),
			CallerID: user.Id
		) :
		Generate
		(
			user.HasAvatar ?
			$"Sure, [here]({user.GetAvatarUrl(format)}) is <@{user.Id}>'s avatar." :
			$"Sorry, <@{user.Id}> does not currently have an avatar set, [here]({user.DefaultAvatarUrl}) is the default discord avatar.",

			CreateAuthorObject($"{user.Username}'s Avatar", user.GetAvatarUrl(ImageFormat.Png).ToString()),
			DateTimeOffset.UtcNow,
			ReplyTo: Context.User.Id,
			ImageURL: new(user.GetAvatarUrl(format).ToString()),
			CallerID: user.Id
		);

		await RespondAsync(InteractionCallback.Message(msg_prop.ToInteraction()));

		#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetAvatar Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
		#endif
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
		#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
		#endif

		Definition.Values.TryGetValue(term, out string? definition);
		MessageProperties msg_prop = Generate
		(
			definition,
			CreateAuthorObject("PPP Encyclopedia", ASSETS + "Define&20Icon.png"),
			DateTimeOffset.UtcNow,
			CreateFooterObject($"Definition requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			STD_COLOR,
			ReplyTo: Context.User.Id
		);

		await RespondAsync(InteractionCallback.Message(msg_prop.ToInteraction()));

		#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetDefinition Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
		#endif
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
		#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
		#endif

		// Make sure the interaction doesn't time out
		await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());

		MessageProperties msg_prop = Generate
		(
			await Translation.Process(input, source_lang, target_lang),
			CreateAuthorObject("Translation Processed", ASSETS + "Translator%20Icon.png"),
			DateTime.Now,
			CreateFooterObject($"Translation requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			STD_COLOR
		);

		await Context.Interaction.SendFollowupMessageAsync(msg_prop.ToInteraction());

		#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetTranslation Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
		#endif
	}

	#region Attributes
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
		#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
		#endif

		// Make sure the interaction doesn't time out
		await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
		MessageProperties msg_prop = Generate
		(
			await Wikipedia.GetPage(search_term, long_format),
			CreateAuthorObject("Wikipedia", ASSETS + "Wikipedia%20Icon.png"),
			DateTime.Now,
			CreateFooterObject($"Definition requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			STD_COLOR
		);

		await Context.Interaction.SendFollowupMessageAsync(msg_prop.ToInteraction());

		#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetWikiResult Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
		#endif
	}
}
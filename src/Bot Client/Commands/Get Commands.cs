// Get Commands Extension File
// This file contains commands that are relevant to the bot's getter functions, local and online.
// An example of these commands is the GetAvatar() command, which gets a user's avatar.
// Commands in this file rely frequently on web requests, and the related mess there.

using Microsoft.IdentityModel.Tokens;
using NetCord.Services.ApplicationCommands;
using PBot.Commands.Helpers;
using static PBot.Caches.Members;
using static PBot.Embeds;
namespace PBot.Commands;

public sealed partial class SlashCommands
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
			$"Sure, [here]({GetAssetURL("Bot Icon.png")}) is my avatar, if you require it in a format other than PNG, please contact <@1124777547687788626>.",
			CreateAuthor($"{user.GetDisplayName()}'s Avatar", user.GetAvatarUrl(ImageFormat.Png).ToString()),
			DateTimeOffset.UtcNow,
			CreateFooter($"Avatar requested by {Context.User.GetDisplayName()}", Context.User.GetAvatarUrl().ToString()),
			replyTo: Context.User.Id,
			imageURLs: [GetAssetURL("Bot Icon.png")],
			refID: user.Id
		) :
		Generate
		(
			user.HasAvatar ?
			$"Sure, [here]({user.GetAvatarUrl(format)}) is <@{user.Id}>'s avatar." :
			$"Sorry, <@{user.Id}> does not currently have an avatar set, [here]({user.DefaultAvatarUrl}) is the default discord avatar.",

			CreateAuthor($"{user.GetDisplayName()}'s Avatar", user.GetAvatarUrl(ImageFormat.Png).ToString()),
			DateTimeOffset.UtcNow,
			replyTo: Context.User.Id,
			imageURLs: [user.GetAvatarUrl(format).ToString()],
			refID: user.Id
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
		await RespondAsync(InteractionCallback.Message(Definition.Entries.GetValueOrDefault(term)!.ToInteraction()));

#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetDefinition Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}

#if false // No Translation Server available

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
			CreateFooterObject($"Translation requested by {Context.User.GetDisplayName()}", Context.User.GetAvatarUrl().ToString()),
			STD_COLOR
		);

		await Context.Interaction.SendFollowupMessageAsync(msg_prop.ToInteraction());

#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetTranslation Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}
#endif

	#region Attributes
	[SlashCommand("user", "Displays data relevant to a specified user.")]
	public partial Task GetUser
	(
		[SlashCommandParameter(Name = "user", Description = "The user to display data for.")]
		User user
	);
	#endregion

	/// <summary>
	/// Dumps user info.
	/// </summary>
	public async partial Task GetUser(User user)
	{
		Member Member = List[user.Id];
		GuildUser User = Member.Data.User;

		string DisplayName = Member.DisplayName;

		string AKAString = "`AKA` "; bool DisplayAKA = false;
		if (User.GetDisplayName() != DisplayName)
		{
			AKAString += $"{User.GetDisplayName()}, ";
			DisplayAKA = true;
		}
		if (User.GlobalName != DisplayName && !string.IsNullOrWhiteSpace(User.GlobalName))
		{
			AKAString += User.GlobalName;
			DisplayAKA = true;
		}
		else
		{
			AKAString = AKAString[..^2];
		}

		MessageProperties msg_prop = Generate
		(
			DisplayAKA ? AKAString.ToEscapedMarkdown() : "",
			null,
			null,
			CreateFooter($"User requested by {Context.User.GetDisplayName()}", Context.User.GetAvatarUrl().ToString()),
			-1,
			Context.User.Id,
			null,
			Member.Data.User.GetAvatar(),
			DisplayName,
			null,
			false,
			#region Fields
			[
			CreateField("User", $"<@{User.Id}>", true),
			CreateField("Tag", User.Discriminator == 0 ? "None" : $"#{User.Discriminator}", true),
			CreateField("ID", User.Id.ToString(), true),
			CreateField("Joined", $"<t:{EpochTime.GetIntDate(User.JoinedAt.DateTime.ToUniversalTime())}>", true),
			CreateField("Verified", User.Verified.GetValueOrDefault().ToString(), true),
			CreateField("PPP Founder", Member.IsFounder ? "True" : "False", true),
			CreateField("Timed Out Until", User.TimeOutUntil == null ? "No Timeout" : $"<t:{EpochTime.GetIntDate(User.TimeOutUntil!.Value.DateTime.ToUniversalTime())}>", true),
			CreateField("Muted", User.Muted ? "True" : "False", true),
			CreateField("Deafened", User.Deafened ? "True" : "False", true),
			CreateField("Invite Code", Member.Data.SourceInviteCode != "false" ? Member.Data.SourceInviteCode : "None", true),
			CreateField("Invited by", Member.Data.InviterId != null ? $"<@{Member.Data.InviterId}>" : "Unknown", true),
			CreateField("Personal Role", Member.PersonalRole == 0 ? "None" : $"<@&{Member.PersonalRole}>", true),
			CreateField("Personal Color", Member.PersonalRoleColor != -1 ? $"#{Member.PersonalRoleColor:X6}" : "None", true),
			CreateField("Accent Color", User.AccentColor == null ? "None" : $"#{User.AccentColor:X6}", true),
			CreateField("Boosting Since", User.GuildBoostStart != null ? $"<t:{EpochTime.GetIntDate(User.GuildBoostStart!.Value.DateTime.ToUniversalTime())}>" : "Not Boosting", true),
			CreateField(inline: false),
			CreateField("Accolades", GetUserAccolades(Member), false),
			],
		#endregion
			User.Id
		);
		await RespondAsync(InteractionCallback.Message(msg_prop.ToInteraction()));
	}

	#region Attributes
	[SlashCommand("wikidefine", "Define a given term via Wikipedia.")]
	public partial Task GetWikiResult
	(
		[SlashCommandParameter(Name = "search_term", Description = "The term to find a page for if possible.")]
		string searchTerm,

		[SlashCommandParameter(Name = "full_page", Description = "Should the full page's contents be fetched?")]
		bool longFormat = false
	);
	#endregion

	/// <summary>
	/// Searches for a Wikipedia page similar to the given <paramref name="searchTerm"/>, and gets its content if a page is found.
	/// </summary>
	public async partial Task GetWikiResult(string searchTerm, bool longFormat)
	{
#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		// Make sure the interaction doesn't time out
		await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
		MessageProperties msg_prop = Generate
		(
			await Wikipedia.GetPage(searchTerm, longFormat),
			CreateAuthor("Wikipedia", GetAssetURL("Wikipedia Icon.png")),
			DateTime.Now,
			CreateFooter($"Definition requested by {Context.User.GetDisplayName()}", Context.User.GetAvatarUrl().ToString()),
			STD_COLOR
		);

		await Context.Interaction.SendFollowupMessageAsync(msg_prop.ToInteraction());

#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetWikiResult Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}
}
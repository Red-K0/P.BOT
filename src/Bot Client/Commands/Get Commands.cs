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
		ImageFormat? format = null
	);
	#endregion

	/// <summary>
	/// Command task. Gets the avatar of the <paramref name="user"/>, in a specific format if specified in <paramref name="format"/>.
	/// </summary>
	public async partial Task GetAvatar(User user, ImageFormat? format)
	{
#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		const ulong BOT_ID = 1169031557848252516;
		string AvatarUrl = user.Id == BOT_ID ? GetAssetURL("Bot Icon.png") : user.GetAvatar(format);
		await RespondAsync(InteractionCallback.Message(((user.Id == BOT_ID) ?
		Generate
		(
			$"Sure, [here]({AvatarUrl}) is my avatar, if you require it in a format other than PNG, please contact <@1124777547687788626>.",
			CreateAuthor("My Current Avatar", AvatarUrl),
			DateTimeOffset.UtcNow,
			CreateFooter($"Avatar requested by {Context.User.GetDisplayName()}", Context.User.GetAvatar()),
			replyTo: Context.User.Id,
			imageURLs: [AvatarUrl],
			refID: user.Id
		) :
		Generate
		(
			$"Sure, [here]({AvatarUrl}) is <@{user.Id}>'s avatar.",
			CreateAuthor($"{user.GetDisplayName()}'s Avatar", AvatarUrl),
			DateTimeOffset.UtcNow,
			replyTo: Context.User.Id,
			imageURLs: [AvatarUrl],
			refID: user.Id
		)).ToInteraction()));

#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetAvatar Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}

	#region Attributes
	[SlashCommand("library", "Get the library entry of a given title.")]
	public partial Task GetTitle
	(
		[SlashCommandParameter(Name = "title", Description = "The title to display.")]
		Library.Titles title
	);
	#endregion

	/// <summary>
	/// Command task. Gets the library entry of the title specified in the <paramref name="title"/> parameter.
	/// </summary>
	public async partial Task GetTitle(Library.Titles title)
	{
		await RespondAsync(InteractionCallback.Message(Library.Entries.GetValueOrDefault(title)!.ToInteraction()));
	}

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
		Member? PPP = List.GetValueOrDefault(user.Id);
		GuildUser User = PPP?.Data.User ?? await Client.Rest.GetGuildUserAsync(GuildID, user.Id);
		GuildUserInfo Info = PPP?.Data ?? await User.GetInfoAsync();

		string DisplayName = PPP?.DisplayName ?? user.GetDisplayName();

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
			CreateFooter($"User requested by {Context.User.GetDisplayName()}", Context.User.GetAvatar()),
			-1,
			Context.User.Id,
			null,
			User.GetAvatar(),
			DisplayName,
			null,
			false,
			[
				CreateField(           "User",  User.                                                                                      ToString()   ),
				CreateField(            "Tag",  User.     Discriminator  ==     0 ?    "None" :   $"#{User.                          Discriminator:D4} "),
				CreateField(             "ID",  User.                Id           .                                                        ToString()   ),
				CreateField(         "Joined",  User.    JoinedAt.Ticks  ==     0 ?   "Never" : $"<t:{User.              JoinedAt.ToUnixTimeSeconds()}>"),
				CreateField(       "Verified", (User.          Verified  ?? false).                                                        ToString()   ),
				CreateField(        "Founder", (PPP?.         IsFounder  ?? false).                                                        ToString()   ),
				CreateField("Timed Out Until",  User.      TimeOutUntil  ==  null ?   "Never" : $"<t:{User.    TimeOutUntil.Value.ToUnixTimeSeconds()}>"),
				CreateField(          "Muted",  User.             Muted           .                                                        ToString()   ),
				CreateField(       "Deafened",  User.          Deafened           .                                                        ToString()   ),
				CreateField(    "Invite Code",  Info.  SourceInviteCode  ??            "None"                                                           ),
				CreateField(     "Invited by",  Info.         InviterId  ==  null ? "Unknown" :  $"<@{Info.                              InviterId   }>"),
				CreateField(  "Personal Role", (PPP?.      PersonalRole) ==  null ?    "None" : $"<@&{PPP!.                           PersonalRole   }>"),
				CreateField( "Personal Color", (PPP?. PersonalRoleColor) ==  null ?    "None" :   $"#{PPP!.                      PersonalRoleColor:X6} "),
				CreateField(   "Accent Color",  User.       AccentColor  ==  null ?    "None" :   $"#{User.                            AccentColor:X6} "),
				CreateField( "Boosting Since",  User.   GuildBoostStart  ==  null ?   "Never" : $"<t:{User. GuildBoostStart.Value.ToUnixTimeSeconds()}>"),
				CreateField(  noInline: true),
				CreateField(      "Accolades",    GetUserAccolades(PPP), true),
			],
			User.Id
		);
		await RespondAsync(InteractionCallback.Message(msg_prop.ToInteraction()));
	}

	#region Attributes
	[SlashCommand("define", "Define a given term via Wikipedia.")]
	public partial Task GetWiki
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
	public async partial Task GetWiki(string searchTerm, bool longFormat)
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
			CreateFooter($"Definition requested by {Context.User.GetDisplayName()}", Context.User.GetAvatar()),
			STD_COLOR
		);

		await Context.Interaction.SendFollowupMessageAsync(msg_prop.ToInteraction());

#if DEBUG_COMMAND
		Messages.Logging.AsVerbose($"GetWikiResult Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}
}
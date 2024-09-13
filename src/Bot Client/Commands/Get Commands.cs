// Get Commands Extension File
// This file contains commands that are relevant to the bot's getter functions, local and online.
// An example of these commands is the GetAvatar() command, which gets a user's avatar.
// Commands in this file rely frequently on web requests, and the related mess there.

using System.Text;
using NetCord.Services.ApplicationCommands;
using Bot.Commands.Helpers;
using static Bot.Caches.Members;
using static Bot.Backend.Embeds;
namespace Bot.Commands;

public sealed partial class SlashCommands
{
	#region Attributes
	[SlashCommand("getavatar", "Get a user's avatar.")]
	public partial Task GetAvatar
	(
		[SlashCommandParameter(Name = "user", Description = "The account to pull an avatar from. Works for non-server members as well.")]
		User user,

		[SlashCommandParameter(Name = "format", Description = "The format of the image result, the GIF and Lottie formats are only supported for animated avatars.")]
		ImageFormat? format = null
	);
	#endregion

	/// <summary>Command task. Gets the avatar of the <paramref name="user"/>, in a specific format if specified in <paramref name="format"/>.</summary>
	/// <param name="user">The user to pull an avatar from. Works for non-server members as well.</param>
	/// <param name="format">The format of the image result, the GIF and Lottie formats are only supported for animated avatars.</param>
	public async partial Task GetAvatar(User user, ImageFormat? format)
	{
		string AvatarUrl = (user.Id == Client.Id) ? GetAssetURL("Bot Icon.png") : user.GetAvatar(format);

		await RespondAsync(InteractionCallback.Message(Generate(CreateProperties(
			$"Sure, [here]({AvatarUrl}) is <@{user.Id}>'s avatar.",
			CreateAuthor($"{user.GetDisplayName()}'s Avatar", AvatarUrl),
			DateTimeOffset.UtcNow,
			CreateFooter($"Avatar requested by {Context.User.GetDisplayName()}", Context.User.GetAvatar()),
			refId: user.Id
		))));
	}

	#region Attributes
	[SlashCommand("library", "Get the library entry of a given title.")]
	public partial Task GetTitle
	(
		[SlashCommandParameter(Name = "title", Description = "The title to display.")]
		Library.Titles title
	);
	#endregion

	/// <summary>Command task. Gets the library entry of the title specified in the <paramref name="title"/> parameter.</summary>
	/// <param name="title">The title to display.</param>
	public async partial Task GetTitle(Library.Titles title)
	{
		await RespondAsync(InteractionCallback.Message(Library.Entries.GetValueOrDefault(title)!));
	}

	#region Attributes
	private static readonly CompositeFormat RawServerMember = CompositeFormat.Parse("""
	```
	-- User Fields --
	Accent Color: {0}
	Avatar Decoration Hash: {1}
	Avatar Hash: {2}
	Banner Hash: {3}
	Created At: {4}
	Default Avatar URL: {5}
	Discriminator: {6}
	Email: {7}
	Flags: {8}
	Global Name: {9}
	Has Avatar: {10}
	Has Avatar Decoration: {11}
	Has Banner: {12}
	ID: {13}
	Is Bot: {14}
	Is System User: {15}
	Locale: {16}
	MFA Enabled: {17}
	Premium Type: {18}
	Public Flags: {19}
	Username: {20}
	Verified: {21}
	
	-- Guild User Fields --
	Deafened: {22}
	Guild Avatar Hash: {23}
	Guild Boost Start: {24}
	Guild Flags: {25}
	Guild ID: {26}
	Has Guild Avatar: {27}
	Hoisted Role ID: {28}
	Is Pending: {29}
	Joined At: {30}
	Muted: {31}
	Nickname: {32}
	Role IDs: {33}
	Time Out Until: {34}

	-- Guild Info Fields --
	Inviter ID: {35}
	Join Source: {36}
	Invite Code: {37}

	-- Server Data --
	Display Name: {38}
	Is Founder: {39}
	Personal Role ID: {40}
	Last Sent Attachment Size: {41}
	Last Sent Message Content: {42}
	Is Last Sent Message Heading: {43}
	Spam Filter Trigger Count: {44}
	```
	""");
	private static readonly CompositeFormat RawStandardUser = CompositeFormat.Parse("""
	```
	-- User Fields --
	Accent Color: {0}
	Avatar Decoration Hash: {1}
	Avatar Hash: {2}
	Banner Hash: {3}
	Created At: {4}
	Default Avatar URL: {5}
	Discriminator: {6}
	Email: {7}
	Flags: {8}
	Global Name: {9}
	Has Avatar: {10}
	Has Avatar Decoration: {11}
	Has Banner: {12}
	ID: {13}
	Is Bot: {14}
	Is System User: {15}
	Locale: {16}
	MFA Enabled: {17}
	Premium Type: {18}
	Public Flags: {19}
	Username: {20}
	Verified: {21}
	```
	""");

	[SlashCommand("user", "Displays data relevant to a specified user.")]
	public partial Task GetUser
	(
		[SlashCommandParameter(Name = "user", Description = "The user to display data for.")]
		User user,

		[SlashCommandParameter(Name = "raw_dump", Description = "Whether the command should dump all user data as plaintext.")]
		bool raw = false
	);
	#endregion

	/// <summary>Command task. Displays data relevant to a specified <paramref name="user"/>.</summary>
	/// <param name="user">The user to display data for.</param>
	/// <param name="raw">Whether the command should dump all user data as plaintext.</param>
	public async partial Task GetUser(User user, bool raw)
	{
		if (raw)
		{
			await RespondAsync(InteractionCallback.Message(List.TryGetValue(user.Id, out Member? Raw)
			? string.Format(null, RawServerMember,
			user.AccentColor?.RawValue, user.AvatarDecorationData?.Hash, user.AvatarHash, user.BannerHash,  user.CreatedAt,
			user.DefaultAvatarUrl,      user.Discriminator,              user.Email,      user.Flags,       user.GlobalName,
			user.HasAvatar,             user.HasAvatarDecoration,        user.HasBanner,  user.Id,          user.IsBot,
			user.IsSystemUser,          user.Locale,                     user.MfaEnabled, user.PremiumType, user.PublicFlags,
			user.Username,              user.Verified,

			// Guild User Fields
			Raw.Info.User.Deafened,      Raw.Info.User.GuildAvatarHash, Raw.Info.User.GuildBoostStart,
			Raw.Info.User.GuildFlags,    Raw.Info.User.GuildId,         Raw.Info.User.HasGuildAvatar,
			Raw.Info.User.HoistedRoleId, Raw.Info.User.IsPending,       Raw.Info.User.JoinedAt,
			Raw.Info.User.Muted,         Raw.Info.User.Nickname,        string.Join(" | ", Raw.Info.User.RoleIds), Raw.Info.User.TimeOutUntil,

			// Guild Info Fields
			Raw.Info.InviterId, Raw.Info.JoinSourceType, Raw.Info.SourceInviteCode,

			// Server Data
			Raw.DisplayName,            Raw.IsFounder,       Raw.PersonalRole?.Id,
			Raw.SpamLastAttachmentSize, Raw.SpamLastMessage, Raw.SpamLastMessageHeading, Raw.SpamSameMessageCount
			)

			: string.Format(null, RawStandardUser,
			user.AccentColor?.RawValue, user.AvatarDecorationData?.Hash, user.AvatarHash, user.BannerHash,  user.CreatedAt,
			user.DefaultAvatarUrl,      user.Discriminator,              user.Email,      user.Flags,       user.GlobalName,
			user.HasAvatar,             user.HasAvatarDecoration,        user.HasBanner,  user.Id,          user.IsBot,
			user.IsSystemUser,          user.Locale,                     user.MfaEnabled, user.PremiumType, user.PublicFlags,
			user.Username,              user.Verified
			)));
			return;
		}

		string DisplayName = user.GetDisplayName();
		string[] Field = ["Never", "False", "False", "Never", "False", "False", "None", "Unknown", "None", "None", "None", "Never", "> None"];

		if (List.TryGetValue(user.Id, out Member? Member))
		{
			                                              Field[00] = $"<t:{Member.Info.User.JoinedAt.ToUnixTimeSeconds()}>";
			if (Member.Info.User.Verified        != null) Field[01] = Member.Info.User.Verified.ToString()!;
			                                              Field[02] = Member.IsFounder.ToString();
			if (Member.Info.User.TimeOutUntil    != null) Field[03] = $"<t:{Member.Info.User.TimeOutUntil.Value.ToUnixTimeSeconds()}>";
			                                              Field[04] = Member.Info.User.Muted.ToString();
			                                              Field[05] = Member.Info.User.Deafened.ToString();
			if (Member.Info.SourceInviteCode     != null) Field[06] = Member.Info.SourceInviteCode;
			if (Member.Info.InviterId            != null) Field[07] = $"<@{Member.Info.InviterId}>";
			if (Member.PersonalRole              != null)
			{
				                                          Field[08] = $"<@&{Member.PersonalRole.Id}>";
				                                          Field[09] = $"#{Member.PersonalRole.Color.RawValue:X6}";
			}
			if (Member.Info.User.AccentColor     != null) Field[10] = $"#{Member.Info.User.AccentColor:X6}";
			if (Member.Info.User.GuildBoostStart != null) Field[11] = $"<t:{Member.Info.User.GuildBoostStart.Value.ToUnixTimeSeconds()}>";
			                                              Field[12] = GetUserAccolades(Member);

			DisplayName = Member.Info.User.Nickname ?? DisplayName;
		}

		bool DisplayAka = false; string AkaString = "`AKA` ";
		if (user.Username   != DisplayName) { DisplayAka = true; AkaString += $"{user.Username}, "; }
		if (user.GlobalName != DisplayName) { DisplayAka = true; AkaString += $"{user.GlobalName}"; } else { AkaString = AkaString[..^2]; }

		await RespondAsync(InteractionCallback.Message(Generate(CreateProperties(
			DisplayAka ? AkaString.ToEscapedMarkdown() : "",
			null,
			DateTime.Now,
			CreateFooter($"User requested by {Context.User.GetDisplayName()}", Context.User.GetAvatar()),
			null,
			user.GetAvatar(),
			DisplayName,
			null,
			[
				CreateField(           "User",                       user.ToString()),
				CreateField(            "Tag",     user.Discriminator.ToString("X4")),
				CreateField(             "ID",                    user.Id.ToString()),
				CreateField(         "Joined", Field[00]), CreateField(    "Verified", Field[01]), CreateField(       "Founder", Field[02]),
				CreateField("Timed Out Until", Field[03]), CreateField(       "Muted", Field[04]), CreateField(      "Deafened", Field[05]),
				CreateField(    "Invite Code", Field[06]), CreateField(  "Invited By", Field[07]), CreateField( "Personal Role", Field[08]),
				CreateField( "Personal Color", Field[09]), CreateField("Accent Color", Field[10]), CreateField("Boosting Since", Field[11]),
				CreateField(  noInline: true),
				CreateField(      "Accolades", Field[12]),
			],
			user.Id
		))));
	}

	#region Attributes
	[SlashCommand("define", "Define a given term via Wikipedia.")]
	public partial Task GetWiki
	(
		[SlashCommandParameter(Name = "search_term", Description = "The term to find a page for if possible.")]
		string term,

		[SlashCommandParameter(Name = "full_page", Description = "Should the full page's contents be fetched?")]
		bool fullPage = false
	);
	#endregion

	/// <summary>Command task. Searches for a Wikipedia page similar to the given <paramref name="term"/>, and gets its content if a page is found.</summary>
	/// <param name="term">The term to find a page for if possible.</param>
	/// <param name="fullPage">Should the full page's contents be fetched?</param>
	public async partial Task GetWiki(string term, bool fullPage)
	{
		// Make sure the interaction doesn't time out
		await RespondAsync(InteractionCallback.DeferredMessage());

		await FollowupAsync(Generate(CreateProperties(
			await Wikipedia.GetPage(term, fullPage),
			CreateAuthor("Wikipedia", GetAssetURL("Wikipedia Icon.png")),
			DateTime.Now,
			CreateFooter($"Definition requested by {Context.User.GetDisplayName()}", Context.User.GetAvatar()),
			color: STD_COLOR
		)));
	}
}
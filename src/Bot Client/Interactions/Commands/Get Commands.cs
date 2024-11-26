using System.Text;
using Bot.Interactions.Helpers;
using NetCord.Services.ApplicationCommands;
using static Bot.Backend.Members;
namespace Bot.Interactions;

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
		string AvatarUrl = (user.Id == Client.Id) ? Common.GetAssetURL("Icons/Bot Icon.png") : $"{user.GetAvatar(format)}";

		await RespondAsync(InteractionCallback.Message(new()
		{
			Embeds = [new()
			{
				Author = new()
				{
					IconUrl = AvatarUrl,
					Name = $"{user.GetDisplayName()}'s Avatar"
				},
				Color = Common.GetColor(user.Id),
				Description = $"Sure, [here]({AvatarUrl}) is <@{user.Id}>'s avatar.",
				Image = AvatarUrl,
				Timestamp = DateTimeOffset.UtcNow,
				Url = AvatarUrl,
			}]
		}));
	}

	#region Attributes
	[SlashCommand("library", "Get the library entry of a given title.")]
	public partial Task GetTitle
	(
		[SlashCommandParameter(Name = "title", Description = "The title to display.")]
		string title
	);
	#endregion

	/// <summary>Command task. Gets the library entry of the title specified in the <paramref name="title"/> parameter.</summary>
	/// <param name="title">The title to display.</param>
	public async partial Task GetTitle(string title)
	{
		await RespondAsync(InteractionCallback.Message(new()
		{
			Embeds = [Library.Entries[title][0]],
			Components = [new ActionRowProperties([new ButtonProperties("NextPage", "2", ButtonStyle.Primary)])]
		}));
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
			await RespondAsync(InteractionCallback.Message(MemberList.TryGetValue(user.Id, out Member? Raw)
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
		bool MemberFound = MemberList.TryGetValue(user.Id, out Member? Member);

		string AkaString = "`AKA` ";
		if (user.Username != DisplayName) { AkaString += $"{user.Username}, "; }
		if ((user.GlobalName ?? DisplayName) != DisplayName) { AkaString += $"{user.GlobalName}"; }
		else { AkaString = AkaString.Contains(',') ? AkaString[..AkaString.IndexOf(',')] : ""; }

		await RespondAsync(InteractionCallback.Message(new()
		{
			Embeds = [new()
			{
				Author = null,
				Color = Common.GetColor(user.Id),
				Description = $"### {user}{(AkaString.Length == 0 ? "" : "\n")}{AkaString}",
				Fields = [
				new() { Name = "ID", Value = $"`{user.Id}`", Inline = true },
				new() { Name = "Joined", Value = !MemberFound ? "Never" : Member!.Info.User.JoinedAt.ToString(), Inline = true },

				new(),
				new() { Name = "Timeout Until", Value = Member?.Info.User.TimeOutUntil == null ? "No timeout" : $"<t:{Member.Info.User.TimeOutUntil.Value.ToUnixTimeSeconds()}>", Inline = true },
				new() { Name = "Muted",         Value =                           !MemberFound ?      "False" : Member!.Info.User.Muted.ToString(), Inline = true },
				new() { Name = "Deafened",      Value =                           !MemberFound ?      "False" : Member!.Info.User.Deafened.ToString(), Inline = true },
				new() { Name = "Invite Code",   Value =  Member?.Info.SourceInviteCode == null ?       "None" : $"`{Member.Info.SourceInviteCode}`", Inline = true },
				new() { Name = "Special Color", Value =           Member?.PersonalRole == null ?       "None" : $"`#{Member.PersonalRole.Color.RawValue:X6}`", Inline = true },
				new() { Name = "Accent Color",  Value =  Member?.Info.User.AccentColor == null ?       "None" : $"#{Member.Info.User.AccentColor:X6}", Inline = true },

				new(),
				new() { Name = "Invited By",     Value =            Member?.Info.InviterId == null ?       "Unknown" : $"<@{Member.Info.InviterId}>", Inline = true },
				new() { Name = "Boosting Since", Value = Member?.Info.User.GuildBoostStart == null ? "Never boosted" : $"<t:{Member.Info.User.GuildBoostStart.Value.ToUnixTimeSeconds()}>", Inline = true },

				new(),
				new() { Name = "Accolades", Value =!MemberFound ? "" : await GetUserAccolades(Member!), Inline = true },
				],
				Footer = new()
				{
					Text = $"Requested by {Context.User.GetDisplayName()}",
					IconUrl = Context.User.GetAvatar()
				},
				Image = null,
				Thumbnail = user.GetAvatar(),
				Timestamp = DateTime.Now,
				Title = null,
				Url = null,
			}]
		}));
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

		await FollowupAsync(new()
		{
			Embeds = [new() {
				Author = new()
				{
					IconUrl = Common.GetAssetURL("Icons/Wikipedia Icon.png"),
					Name = "Wikipedia"
				},
				Color = new(Common.STD_COLOR),
				Description = await Wikipedia.GetPage(term, fullPage),
				Footer = new()
				{
					IconUrl = Context.User.GetAvatar(),
					Text = $"Definition requested by {Context.User.GetDisplayName()}"
				},
				Timestamp = DateTime.Now
			}]
		});
	}
}
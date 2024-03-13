// SlashCommand definition file
// This file contains the definition for the SlashCommand class, as well as commands relevant to the bot's internal functions.
// An example of these commands are the SystemsCheck() command, which queries the current state of the bot.
// Commands in this file should be minimal and concise, as well as being able to run entirely locally.

using Microsoft.IdentityModel.Tokens;
using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;
using static P_BOT.UserManagement;

namespace P_BOT.Command_Processing;

/// <summary>
/// Contains the commands used by P.BOT and their associated tasks.
/// </summary>
public sealed partial class SlashCommand : ApplicationCommandModule<SlashCommandContext>
{
	/// <summary>
	/// The URL to the GitHub Assets folder.
	/// </summary>
	private const string ASSETS = "https://raw.githubusercontent.com/Red-K0/P.BOT/master/Assets/";

	/// <summary>
	/// The hex code for standard embed gray.
	/// </summary>
	private const int STD_COLOR = 0x72767D;

	#region Attributes
	[SlashCommand("syscheck", "Check if the system's online.")]
	public partial Task SystemsCheck();
	#endregion

	/// <summary>
	/// Command task. Checks if P.BOT's system is active.
	/// </summary>
	public async partial Task SystemsCheck()
	{
		const string response =
		"""
		
		System is active and running.
		
		""";
		await RespondAsync(InteractionCallback.Message(response));
	}

	#region Attributes
	[SlashCommand("toggle", "Toggle the status of a text-based command module.")]
	public partial Task ToggleModule
	(
		[SlashCommandParameter(Name = "module", Description = "The module to turn on, or off.")]
		Options.Modules module
	);
	#endregion

	/// <summary>
	/// Toggles the state of the module specified in the <paramref name="module"/> parameter.
	/// </summary>
	public async partial Task ToggleModule(Options.Modules module)
	{
		bool result = false;
		switch (module)
		{
			case Options.Modules.DnDTextModule: Options.DnDTextModule ^= true; result = Options.DnDTextModule; break;
		}
		await RespondAsync(InteractionCallback.Message(result ? $"The module '{module}' has been successfully enabled." : $"The module '{module}' has been successfully disabled."));
	}

	#region Attributes
	[SlashCommand("user", "Displays data relevant to a specified user.")]
	public partial Task DumpUserInfo
	(
		[SlashCommandParameter(Name = "user", Description = "The user to display data for.")]
		User user
	);
	#endregion

	/// <summary>
	/// Dumps user info.
	/// </summary>
	public async partial Task DumpUserInfo(User user) // TODO | Proper details
	{
		_ = MemberList.TryGetValue(user.Id, out UserObject @User);

		string Roles = "";
		foreach (ulong Accolade in User.Server.Roles.Where(IsEventRole))
		{
			Roles += Accolade switch
			{
				SixMonthAnniversary => $"\n<@&{SixMonthAnniversary}> \n- Attended the 6-Month Server Anniversary",
				SecretSanta2023 => $"\n<@&{SecretSanta2023}> \n- Sent a Secret Santa Gift in 2023",
				HyakkanoEnjoyer => $"\n<@&{HyakkanoEnjoyer}> \n- Read the first 50 Chapters of 100 Girlfriends",
				_ => ""
			};
		}

		string NitroType = User.PremiumType switch
		{
			UserManagement.PremiumType.NitroClassic => "Classic",
			UserManagement.PremiumType.Nitro => "Standard",
			UserManagement.PremiumType.NitroBasic => "Basic",
			_ => "None"
		};

		string? DisplayName = User.Server.Nickname?.Length != 0 ? User.Server.Nickname : User.GlobalName?.Length != 0 ? User.GlobalName : User.Username;

		string AKAString = "`AKA` "; bool DisplayAKA = false;
		if (User.Username != DisplayName)
		{
			AKAString += $"{User.Username}, ";
			DisplayAKA = true;
		}
		if (User.GlobalName != DisplayName && !string.IsNullOrWhiteSpace(User.GlobalName))
		{
			AKAString += User.GlobalName;
			DisplayAKA = true;
		}
		else
		{
			AKAString = "`AKA` "[..^2];
		}

		MessageProperties msg_prop = Embeds.Generate
		(
			DisplayAKA ? Parsing.MDLiteral("`AKA` ") : "",
			null,
			null,
			Embeds.CreateFooterObject($"User requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			-1,
			Context.User.Id,
			null,
			(User.Customization.AvatarHash != null) ?
			ImageUrl.UserAvatar(User.ID, User.Customization.AvatarHash, ImageFormat.Png).ToString() : ImageUrl.DefaultUserAvatar(User.ID).ToString(),
			DisplayName,
			null,
			false,
			#region Fields
			[
			Embeds.CreateFieldObject("User", $"<@{User.ID}>", true),
			Embeds.CreateFieldObject("Tag", User.Discriminator == 0 ? "None" : $"#{User.Discriminator}", true),
			Embeds.CreateFieldObject("ID", User.ID.ToString(), true),
			Embeds.CreateFieldObject("Joined", $"<t:{EpochTime.GetIntDate(User.Server.JoinedAt.ToUniversalTime())}>", true),
			Embeds.CreateFieldObject("Verified", User.Server.Verified.ToString(), true),
			Embeds.CreateFieldObject("PPP Founder", IsFounder(User.ID) ? "True" : "False", true),
			Embeds.CreateFieldObject("Timed Out Until", User.Server.MutedUntil < DateTime.Now ? "No Timeout" : $"<t:{EpochTime.GetIntDate(User.Server.MutedUntil.ToUniversalTime())}>", true),
			Embeds.CreateFieldObject("Muted", User.Server.IsVCMuted ? "True" : "False", true),
			Embeds.CreateFieldObject("Deafened", User.Server.IsVCDeafened ? "True" : "False", true),
			Embeds.CreateFieldObject("Invite Code", User.Invite.Code != "false" ? User.Invite.Code : "None", true),
			Embeds.CreateFieldObject("Invited by", User.Invite.SenderID != 0 ? $"<@{User.Invite.SenderID}>" : "Unknown", true),
			Embeds.CreateFieldObject("Personal Role", User.Customization.PersonalRole == 0 ? "None" : $"<@&{User.Customization.PersonalRole}>", true),
			Embeds.CreateFieldObject("Personal Role Color", User.Customization.PersonalRoleColor == -1 ? "None" : $"#{User.Customization.PersonalRoleColor:X6}", true),
			Embeds.CreateFieldObject("Nitro Type", NitroType, true),
			Embeds.CreateFieldObject("Boosting Since", User.Server.NitroSince != DateTime.MinValue ? $"<t:{EpochTime.GetIntDate(User.Server.NitroSince.ToUniversalTime())}>" : "Not Boosting", true),
			Embeds.CreateFieldObject(Inline: false),
			Embeds.CreateFieldObject("Accolades", Roles.Length != 0 ? Roles : "None", false),
			],
		#endregion
			User.ID
		);
		await RespondAsync(InteractionCallback.Message(msg_prop.ToInteraction()));
	}
}
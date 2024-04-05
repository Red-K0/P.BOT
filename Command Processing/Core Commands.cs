// SlashCommand definition file
// This file contains the definition for the SlashCommand class, as well as commands relevant to the bot's internal functions.
// An example of these commands are the SystemsCheck() command, which queries the current state of the bot.
// Commands in this file should be minimal and concise, as well as being able to run entirely locally.

using Microsoft.IdentityModel.Tokens;
using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;
using static P_BOT.Caches.Members;
using static P_BOT.Embeds;
namespace P_BOT.Command_Processing;

/// <summary>
/// Contains the commands used by P.BOT and their associated tasks.
/// </summary>
public sealed partial class SlashCommand : ApplicationCommandModule<SlashCommandContext>
{
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
		Member Member = List[user.Id];
		GuildUser User = Member.Data.User;

		string index = (Array.IndexOf([.. List.Keys], Member.Data.User.Id) + 1).ToString();
		index += index.Last() switch
		{
			'1' => "st",
			'2' => "nd",
			'3' => "rd",
			  _ => "th"
		};

		string Roles = $">>> <@&{(Member.IsFounder ? BaseFounderRole : Cultist)}>\n- The {index} member of the PPP.";
		foreach (ulong role in User.RoleIds.Where(IsAccolade))
		{
			Roles += role switch
			{
				SixMonthAnniversary => $"\n<@&{SixMonthAnniversary}>\n- Attended the 6-Month Server Anniversary",
				    SecretSanta2023 => $"\n<@&{SecretSanta2023}>\n- Sent a Secret Santa Gift in 2023",
				    HyakkanoEnjoyer => $"\n<@&{HyakkanoEnjoyer}>\n- Read the first 50 Chapters of 100 Girlfriends",
				    CelesteSpeedrun => $"\n<@&{CelesteSpeedrun}>\n- Finish a celeste speedrun in under 40 minutes",
				_ => ""
			};
		}

		string? DisplayName = User.Nickname?.Length != 0 ? User.Nickname : User.GlobalName?.Length != 0 ? User.GlobalName : User.Username;

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
			AKAString = AKAString[..^2];
		}

		MessageProperties msg_prop = Generate
		(
			DisplayAKA ? AKAString.ToEscapedMarkdown()	: "",
			null,
			null,
			CreateFooter($"User requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
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
			CreateField("Accolades", Roles.Length != 0 ? Roles : "None", false),
			],
		#endregion
			User.Id
		);
		await RespondAsync(InteractionCallback.Message(msg_prop.ToInteraction()));
	}
}
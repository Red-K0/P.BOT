// SlashCommand definition file
// This file contains the definition for the SlashCommand class, as well as commands relevant to the bot's internal functions.
// An example of these commands are the SystemsCheck() command, which queries the current state of the bot.
// Commands in this file should be minimal and concise, as well as being able to run entirely locally.

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
	[SlashCommand("userdata", "Gets a specific user")]
	public partial Task DumpUserInfo
	(
		[SlashCommandParameter(MinValue = 1)]
		int number
	);
	#endregion

	/// <summary>
	/// Dumps user info.
	/// </summary>
	public async partial Task DumpUserInfo(int number) // TODO | Proper details
	{
		if (number > UserList.Length) { await RespondAsync(InteractionCallback.Message($"There are only {UserList.Length} members in the PPP.")); return; }
		UserObject User = UserList[number - 1];

		EmbedFieldProperties[] FieldArray =
		[
			Embeds.CreateFieldObject("User", $"<@{User.ID}>", true),
			Embeds.CreateFieldObject("ID", User.ID.ToString(), true),
			Embeds.CreateFieldObject(Inline: false),

			Embeds.CreateFieldObject("Joined", $"<t:{Microsoft.IdentityModel.Tokens.EpochTime.GetIntDate(User.Server.JoinedAt.ToUniversalTime())}>", true),
			Embeds.CreateFieldObject("Verified", User.Server.Verified.ToString(), true),
			Embeds.CreateFieldObject(Inline: false),

			Embeds.CreateFieldObject("Invited by", User.Invite.SenderID != 0 ? $"<@{User.Invite.SenderID}>" : "Unknown", true),
			Embeds.CreateFieldObject("Invite link", User.Invite.Code != "false" ? $"https://discord.gg/{User.Invite.Code}" : "None", true),
			Embeds.CreateFieldObject(Inline: false),

			Embeds.CreateFieldObject("Personal Role", User.Customization.PersonalRole == 0 ? "None" : $"<@&{User.Customization.PersonalRole}>")
		];
		MessageProperties msg_prop = Embeds.Generate
		(
			$"AKA - {User.Username}" + (User.GlobalName?.Length != 0 ? $", {User.GlobalName}" : ""),
			null,
			null,
			Embeds.CreateFooterObject($"User requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			-1,
			Context.User.Id,
			null,
			ImageUrl.UserAvatar(User.ID, User.Customization.AvatarHash, ImageFormat.Png).ToString(),
			User.Server.Nickname?.Length != 0 ? User.Server.Nickname : User.GlobalName?.Length !=0 ? User.GlobalName : User.Username,
			null,
			false,
			FieldArray,
			User.ID
		);
		await RespondAsync(InteractionCallback.Message(new() { Embeds = msg_prop.Embeds }));
	}
}
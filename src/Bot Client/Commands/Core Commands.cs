// SlashCommand definition file
// This file contains the definition for the SlashCommand class, as well as commands relevant to the bot's internal functions.
// An example of these commands are the SystemsCheck() command, which queries the current state of the bot.
// Commands in this file should be minimal and concise, as well as being able to run entirely locally.

using NetCord.Services.ApplicationCommands;

namespace PBot.Commands;

/// <summary>
/// Contains the slash commands used by P.BOT and their associated tasks.
/// </summary>
[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "Does not apply here.")]
public sealed partial class SlashCommands : ApplicationCommandModule<SlashCommandContext>
{
	#region Attributes

	[SlashCommand("syscheck", "Check if the system's online.")]
	public partial Task SystemsCheck();

	#endregion Attributes

	/// <summary>
	/// Command task. Checks if P.BOT's system is active.
	/// </summary>
	public async partial Task SystemsCheck()
	{
		Process currentProcess = Process.GetCurrentProcess();
		currentProcess.Refresh();

		await RespondAsync(InteractionCallback.Message($"""
		Time since client start: `{(DateTime.UtcNow - currentProcess.StartTime.ToUniversalTime()).TotalSeconds}`
		RAM in use: `{currentProcess.WorkingSet64 / 1024 / 1024}MB`
		"""));
	}

	#region Attributes

	[SlashCommand("movechannel", "Move a group of users from one voice channel to another.")]
	public partial Task MoveVoiceUsers
	(

		[SlashCommandParameter(Name = "from", Description = "The channel to move users from.")]
		VoiceGuildChannel originChannel,

		[SlashCommandParameter(Name = "to", Description = "The channel to move users to.")]
		VoiceGuildChannel destinationChannel,

		[SlashCommandParameter(Name = "role", Description = "Optional. Only moves members with a specific role if set.")]
		Role? role = null
	);

	#endregion Attributes

	/// <summary>
	/// Command task. Moves a group of users from one voice channel to another.
	/// </summary>
	public async partial Task MoveVoiceUsers(VoiceGuildChannel originChannel, VoiceGuildChannel destinationChannel, Role? role)
	{
		await RespondAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

		// Nothing to be done here.
		if (originChannel == destinationChannel)
		{
			await FollowupAsync(new()
			{
				Content = "Successfully moved 0 users. What did you think would happen?",
				Flags = MessageFlags.Ephemeral
			});
			return;
		}

		Guild Guild = GetGuild(GuildID);

		// Prevent users without the appropriate permissions from using the command.
		if (!(await Client.Rest.GetGuildUserAsync(GuildID, Context.User.Id)).GetPermissions(Guild).HasFlag(Permissions.MoveUsers))
		{
			await FollowupAsync(new()
			{
				Content = "Sorry, this command can only be used by members with the \"Move users\" permission.",
				Flags = MessageFlags.Ephemeral
			});
			return;
		}

		bool SpecificRole = (role != null);

		int MoveCount = 0;
		foreach (VoiceState state in Guild.VoiceStates.Values)
		{
			GuildUser User = state.User ?? await Client.Rest.GetGuildUserAsync(GuildID, state.UserId);

			if (state.ChannelId == originChannel.Id && (!SpecificRole || User.RoleIds.Contains(role!.Id)))
			{
				await User.ModifyAsync(o => o.ChannelId = destinationChannel.Id);
				MoveCount++;
			}
		}

		await FollowupAsync(new()
		{
			Content = MoveCount > 0 ? $"Successfully moved {MoveCount} users." : "Sorry, but no users were found in the specified channel.",
			Flags = MessageFlags.Ephemeral
		});
	}
}
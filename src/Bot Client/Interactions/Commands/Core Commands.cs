using NetCord.Services.ApplicationCommands;

namespace Bot.Interactions;

/// <summary>
/// Contains the bot's slash commands and their associated tasks.
/// </summary>
public sealed partial class SlashCommands : ApplicationCommandModule<SlashCommandContext>
{
	#region Attributes

	[SlashCommand("syscheck", "Check if the system's online.")]
	public partial Task SystemsCheck();

	#endregion Attributes

	/// <summary>
	/// Command task. Checks if the bot's command system is active.
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

		// Prevent users without the appropriate permissions from using the command.
		if (!(await Client.Rest.GetGuildUserAsync(GuildID, Context.User.Id)).GetPermissions(Core.Guild).HasFlag(Permissions.MoveUsers))
		{
			await FollowupAsync(new()
			{
				Content = "Sorry, this command can only be used by members with the \"Move users\" permission.",
				Flags = MessageFlags.Ephemeral
			});
			return;
		}

		bool specificRole = role != null;

		int moveCount = 0;
		foreach (VoiceState state in Core.Guild.VoiceStates.Values)
		{
			GuildUser user = state.User ?? await Client.Rest.GetGuildUserAsync(GuildID, state.UserId);

			if (state.ChannelId == originChannel.Id && (!specificRole || user.RoleIds.Contains(role!.Id)))
			{
				await user.ModifyAsync(o => o.ChannelId = destinationChannel.Id);
				moveCount++;
			}
		}

		await FollowupAsync(new()
		{
			Content = moveCount > 0 ? $"Successfully moved {moveCount} users." : "Sorry, but no users were found in the specified channel.",
			Flags = MessageFlags.Ephemeral
		});
	}
}
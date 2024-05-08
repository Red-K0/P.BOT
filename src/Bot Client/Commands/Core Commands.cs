// SlashCommand definition file
// This file contains the definition for the SlashCommand class, as well as commands relevant to the bot's internal functions.
// An example of these commands are the SystemsCheck() command, which queries the current state of the bot.
// Commands in this file should be minimal and concise, as well as being able to run entirely locally.

using NetCord.Services.ApplicationCommands;
using static PBot.Commands.TextCommands;
namespace PBot.Commands;

/// <summary>
/// Contains the slash commands used by P.BOT and their associated tasks.
/// </summary>
public sealed partial class SlashCommands : ApplicationCommandModule<SlashCommandContext>
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
		Process currentProcess = Process.GetCurrentProcess();
		string timeSinceStart = (DateTime.Now - currentProcess.StartTime).ToString();

		currentProcess.Refresh();

		await RespondAsync(InteractionCallback.Message($"""
			Time since client start: `{timeSinceStart.Remove(timeSinceStart.IndexOf('.'))}`
			RAM in use: `{currentProcess.WorkingSet64 / 1024 / 1024}MB`
			"""));
	}
}
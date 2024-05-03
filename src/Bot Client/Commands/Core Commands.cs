﻿// SlashCommand definition file
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
		const string response =
		"""
		
		System is active and running.
		
		""";
		await RespondAsync(InteractionCallback.Message(response));
	}

	#region Attributes
	[SlashCommand("toggle", "Toggle the status of a text command module.")]
	public partial Task ToggleModule
	(
		[SlashCommandParameter(Name = "module", Description = "The module to turn on, or off.")]
		Modules module
	);
	#endregion

	/// <summary>
	/// Toggles the state of the module specified in the <paramref name="module"/> parameter.
	/// </summary>
	public async partial Task ToggleModule(Modules module)
	{
		switch (module)
		{
			case Modules.ProbabilityModule: State ^= 0b0000_0001; break;
		}
		await RespondAsync(InteractionCallback.Message(IsActive(Modules.ProbabilityModule) ? $"The module '{module}' has been successfully enabled." : $"The module '{module}' has been successfully disabled."));
	}
}
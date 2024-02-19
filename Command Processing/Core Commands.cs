// SlashCommand definition file
// This file contains the definition for the SlashCommand class, as well as commands relevant to the bot's internal functions.
// An example of these commands are the SystemsCheck() command, which queries the current state of the bot.
// Commands in this file should be minimal and concise, as well as being able to run entirely locally.

using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;

namespace P_BOT.Command_Processing;

/// <summary> Contains the commands used by P.BOT and their associated tasks. </summary>
public sealed partial class SlashCommand : ApplicationCommandModule<SlashCommandContext>
{
	#region Attributes
	[SlashCommand("syscheck", "Check if the system's online.")]
	public partial Task SystemsCheck();
	#endregion

	/// <summary> Command task. Checks if P.BOT's system is active. </summary>
	public partial Task SystemsCheck()
	{
		const string response =
		"""
		
		System is active and running.
		
		""";
		return RespondAsync(InteractionCallback.Message(response));
	}

	#region Attributes
	[SlashCommand("toggle", "Toggle the status of a text-based command module.")]
	public partial Task ToggleModule
	(
		[SlashCommandParameter(Name = "module", Description = "The module to turn on, or off.")]
		Options.Modules module
	);
	#endregion

	/// <summary> Toggles the state of the module specified in the <paramref name="module"/> parameter. </summary>
	public partial Task ToggleModule(Options.Modules module)
	{
		bool result = false;
		switch (module)
		{
			case Options.Modules.DnDTextModule: Options.DnDTextModule ^= true; result = Options.DnDTextModule; break;
		}
		return RespondAsync(InteractionCallback.Message(result ? $"The module '{module}' has been successfully enabled." : $"The module '{module}' has been successfully disabled."));
	}
}
// The helper class supporting the /toggle command.

using NetCord.Services.ApplicationCommands;
namespace PBot.Processing.Helpers;

#pragma warning disable CA2211 // Non-constant fields should not be visible
/// <summary>
/// Contains methods and variables for storage and modification of bot settings.
/// </summary>
public static class Options
{
	/// <summary>
	/// The current status of the module.
	/// </summary>
	public static bool DnDTextModule;

	/// <summary>
	/// A list of optional modules used by P.BOT.
	/// </summary>
	public enum Modules
	{
		/// <summary>
		/// The module responsible for rolling dice, as well as message parsing related to so.
		/// </summary>
		[SlashCommandChoice(Name = "Probability Module")] DnDTextModule
	}
}
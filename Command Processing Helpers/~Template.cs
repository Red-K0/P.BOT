// Helper class files contain methods, enumerations, fields, and other
// info relevant to a specific command, or set of commands. The below
// template is the standard on how they should be written and formatted
// to ensure ease of maintainability and readability.

// ----------------------- TEMPLATE STARTS HERE -----------------------

// The helper class supporting the *** commands.

using NetCord.Services.ApplicationCommands;
namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains ***
/// </summary>
public static class CommandHelperClass
{
	/// <summary>
	/// 
	/// </summary>
	public static async type MainMethod()
	{

	}

	/// <summary>
	/// 
	/// </summary>
	public enum Choices
	{
		/// <summary>  </summary>
		[SlashCommandChoice(Name = "")] A,
		/// <summary>  </summary>
		[SlashCommandChoice(Name = "")] B,
		/// <summary>  </summary>
		[SlashCommandChoice(Name = "")] C
	}
}
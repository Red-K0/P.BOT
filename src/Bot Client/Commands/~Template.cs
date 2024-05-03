// Extension files contain the methods that define a command, alongside
// its parameters and attributes. Commands are laid out in the format
// provided below, to maintain consistency and ease of readability for
// future additions / modifications. The #Attributes region preceding
// every command contains the partial method definition containing its
// attributes, to prevent clutter of the actual method code.

// ----------------------- TEMPLATE STARTS HERE -----------------------

// *** Commands Extension File
// This file contains commands that are relevant to the bot's ***
// An example of these commands is the ***
// Commands in this file ***

using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;

namespace P_BOT.Command_Processing;
public sealed partial class SlashCommand
{
	#region Attributes
	[SlashCommand("", "")]
	public async partial Task CommandMethod
	(
		[SlashCommandParameter(Name = "", Description = "")]
		type parameter,

		[SlashCommandParameter(Name = "", Description = "")]
		type optional_parameter = defaultvalue
	);
	#endregion

	/// <summary>
	/// 
	/// </summary>
	public async partial Task CommandMethod(type parameter, type optional_parameter)
	{

	}
}
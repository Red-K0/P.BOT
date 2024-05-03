// Text Commands Extension File
// This file contains commands that are relevant to the bot's text-based functions.
// An example of these commands is the .rl command, which performs a basic dice roll.
// Commands in this file rely on selector functions, and are called during message parsing.

using NetCord.Services.ApplicationCommands;

namespace PBot.Commands;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA2211 // Non-constant fields should not be visible
/// <summary>
/// Contains the text commands used by P.BOT and their associated tasks.
/// </summary>
public static class TextCommands
{
	public static byte State = 1;
	public enum Modules : byte
	{
		[SlashCommandChoice(Name = "Probability Module")] ProbabilityModule = 1 << 0,
	}

	public static bool IsActive(Modules module) => (State & (byte)module) != 0;

	public static void Parse(Message message)
	{
		switch (message.Content[1])
		{
			case 'r': if ((State & 0b0000_0001) != 0) Helpers.ProbabilityStateMachine.Run(message); return;
		}
	}
}

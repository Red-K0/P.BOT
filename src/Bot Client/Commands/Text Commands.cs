// Text Commands Extension File
// This file contains commands that are relevant to the bot's text-based functions.
// An example of these commands is the .rl command, which performs a basic dice roll.
// Commands in this file rely on selector functions, and are called during message parsing.

namespace PBot.Commands;

/// <summary>
/// Contains the text commands used by P.BOT and their associated tasks.
/// </summary>
public static class TextCommands
{
	/// <summary>
	/// Parses a given message, running the associated text command if a valid prefix is found.
	/// </summary>
	public static void Parse(Message message)
	{
		switch (message.Content[1])
		{
			case 'r': Helpers.ProbabilityStateMachine.Run(message); return;
		}
	}
}

namespace Bot.Interactions;

/// <summary>
/// Contains the text commands used by P.BOT and their associated tasks.
/// </summary>
internal static class TextCommands
{
	/// <summary>
	/// Parses a given message, running the associated text command if a valid prefix is found.
	/// </summary>
	public static async Task Parse(Message message)
	{
		switch (message.Content[1])
		{
			case 'r': await Helpers.ProbabilityStateMachine.Run(message); return;
		}
	}
}
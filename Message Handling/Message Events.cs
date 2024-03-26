using static P_BOT.Messages.Functions;
namespace P_BOT.Messages;

/// <summary>
/// Contains methods responsible for handling message events.
/// </summary>
internal static class Events
{
	/// <summary>
	/// Processes new messages.
	/// </summary>
	public static async ValueTask MessageCreated(Message message)
	{
		const ulong IGNORE_ID = 1169031557848252516;

		// Core Message Parsing
		if (!message.Author.IsBot)
		{
			Logging.AddMessage(message);
			SpamFilter(message);
		}

		if (message.Content.Contains("https://discord.com/channels/") && message.Author.Id != IGNORE_ID)
		{
			ParseMessageLink(message);
		}

		// Modular Message Parsing
		if (Command_Processing.Helpers.Options.DnDTextModule && message.Content.StartsWith('.'))
		{
			Command_Processing.Helpers.ProbabilityStateMachine.Run(message);
		}

		await ValueTask.CompletedTask;
	}

	/// <summary>
	/// Logs message deletions.
	/// </summary>
	public static async ValueTask MessageDeleted(MessageDeleteEventArgs message)
	{
		Logging.AsDiscord($"The message with ID '{message.MessageId}' was deleted.");
		await Task.CompletedTask;
	}

	/// <summary>
	/// Logs message edits and updates.
	/// </summary>
	public static async ValueTask MessageUpdated(Message message)
	{
		if (message.Content != null)
		{
			Logging.AsDiscord($"The message with ID '{message.Id}' was updated by {message.Author.Username} with the new content:");
		}
		await Task.CompletedTask;
	}

	/// <summary>
	/// Logs reactions to messages.
	/// </summary>
	public static async ValueTask ReactionAdded(MessageReactionAddEventArgs message)
	{
		// Starboard Handling
		if (message.Emoji.Name == "⭐" && !(await File.ReadAllLinesAsync(Pages.STARBOARD)).Contains(message.MessageId.ToString()))
		{
			AddToStarBoard(message);
		}
		await ValueTask.CompletedTask;
	}
}
namespace P_BOT;
using static MessageFunctions;

/// <summary> Contains methods responsible for handling message events in the console. </summary>
internal static partial class MessageLogging
{
	private const ulong AUTHORID_SYSTEM = 0;
	private const ulong AUTHORID_CLIENT = 1;

	/// <summary> Logs messages from the system. </summary>
	public static async ValueTask System(LogMessage message)
	{
		// Prevents partial message exceptions in the log
		if (message.Exception is { StackTrace: string stackTrace } && stackTrace.Contains(ERRORLOG_NOWRITE))
		{
			return;
		}
		LogAsSystem(ref message);
		await Task.CompletedTask;
	}

	/// <summary> Logs message deletions. </summary>
	public static async ValueTask Delete(MessageDeleteEventArgs message)
	{
		LogAsClient($"The message with ID '{message.MessageId}' was deleted.");
		await Task.CompletedTask;
	}

	/// <summary> Logs message edits and updates. </summary>
	public static async ValueTask Update(Message message)
	{
		if (message.Content != null)
		{
			LogAsClient($"The message with ID '{message.Id}' was updated by {message.Author.Username} with the new content:");
		}
		await Task.CompletedTask;
	}

	/// <summary> Logs new messages. </summary>
	public static async ValueTask Create(Message message)
	{
		// Core Message Parsing
		if (!message.Author.IsBot)
		{
			LogMessage(message);
			SpamFilter(message);
		}

		if (message.Content.Contains("https://discord.com/channels/") && message.Author.Id != BOT_ID)
		{
			ParseMessageLink(message);
		}

		// Modular Message Parsing
		if (Command_Processing.Helpers.Options.DnDTextModule && message.Content.StartsWith('.'))
		{
			Command_Processing.Helpers.RollsModule.LogicSelect(message);
		}

		await ValueTask.CompletedTask;
	}

	/// <summary> Logs reactions to messages. </summary>
	public static async ValueTask ReactAdd(MessageReactionAddEventArgs message)
	{
		// Starboard Handling
		if (message.Emoji.Name == "⭐" && !(await File.ReadAllLinesAsync(MEMORY_STARRED_MESSAGES)).Contains(message.MessageId.ToString()))
		{
			AddToStarBoard(message);
		}
		await ValueTask.CompletedTask;
	}
}
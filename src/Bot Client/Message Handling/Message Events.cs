using static PBot.Messages.Functions;
using static PBot.Messages.Logging;
namespace PBot.Messages;

/// <summary>
/// Contains methods responsible for handling message events.
/// </summary>
internal static class Events
{
	/// <summary>
	/// Used by <see cref="Filter(Message)"/> to pass a message to <see cref="MessageDeleted(MessageDeleteEventArgs)"/>, bypassing the cache.
	/// </summary>
	public static Message? DeletedSpamMessage;

	/// <summary>
	/// Maps the <see cref="client"/>'s events to their appropriate response method.
	/// </summary>
	public static void MapClientHandlers()
	{
		client.Log                += LogNetworkMessage;
		client.MessageCreate      += MessageCreated;
		client.MessageDelete      += MessageDeleted;
		client.MessageUpdate      += MessageUpdated;
		client.MessageReactionAdd += ReactionAdded;
	}

	/// <summary>
	/// Processes new messages.
	/// </summary>
	public static async ValueTask MessageCreated(Message message)
	{
		// Massive performance save.
		// While this leads to a repeated reference, it skips a lot of unnecessary computation.
		if (message.Author.IsBot) { Caches.Messages.Add(message); return; }

		if (await Filter(message)) return;

		Caches.Messages.Add(message);

		if (message.Content.Contains(SERVER_LINK)) ParseLinks(message);

		// TODO: New command processor

		LogCreatedMessage(message);
	}

	/// <summary>
	/// Logs message deletions.
	/// </summary>
	public static async ValueTask MessageDeleted(MessageDeleteEventArgs deleteArgs)
	{
		// If DeletedSpamMessage isn't null, this event was a result of the message filter, so just use the value passed there.
		// This removes the cost of caching the message and retrieving it.
		Message? message = DeletedSpamMessage ?? Caches.Messages.Get(deleteArgs.MessageId);
		if (DeletedSpamMessage != null) DeletedSpamMessage = null;

		// If this check passes, message isn't null.
		if (message?.Author.IsBot != true)
		{
			await LogDeletedMessage(message!);
			return;
		}

		WriteAsID($"The message with ID '{deleteArgs.MessageId}' was deleted, but not cached.", SpecialId.Discord);
	}

	/// <summary>
	/// Logs message edits and updates.
	/// </summary>
	public static async ValueTask MessageUpdated(Message message)
	{
		// Don't process bot messages.
		// Roo expensive computationally, as a lot of bots rely on frequent edits.
		if (message.Author.IsBot) return;

		Message? OriginalMessage = Caches.Messages.Get(message.Id);
		Caches.Messages.Edit(message);

		if (OriginalMessage != null) await LogUpdatedMessage(OriginalMessage, message);
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
	}
}

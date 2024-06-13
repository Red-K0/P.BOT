using PBot.Commands;
using static PBot.Messages.Functions;
using static PBot.Messages.Logging;

namespace PBot;

internal static partial class Events
{
	/// <summary>
	/// Used by <see cref="Filter(Message)"/> to pass a message to <see cref="MessageDelete(MessageDeleteEventArgs)"/>, bypassing the cache.
	/// </summary>
	private static Message? DeletedSpamMessage;

	/// <summary>
	/// Processes new messages.
	/// </summary>
	public static async ValueTask MessageCreate(Message message)
	{
		// Massive performance save.
		// While this leads to a repeated reference, it skips a lot of unnecessary computation.
		if (message.Author.IsBot) { Caches.Messages.Recent.AddOrUpdate(message.Id, message); return; }

		// If a command runs, do nothing else.
		if (message.Content.Length > 3 && message.Content[0] == '.')
		{
			await TextCommands.Parse(message);
			return;
		}

		if (await Filter(message)) return;

		Caches.Messages.Recent.AddOrUpdate(message.Id, message);

		if (message.Content.Contains(GuildURL)) await ParseLinks(message);

		LogCreatedMessage(message);
	}

	/// <summary>
	/// Logs message deletions.
	/// </summary>
	public static async ValueTask MessageDelete(MessageDeleteEventArgs deleteArgs)
	{
		// If DeletedSpamMessage isn't null, this event was a result of the message filter, so just use the value passed there.
		// This removes the cost of caching the message and retrieving it.
		Message? message = DeletedSpamMessage;

		if (message is null)
		{
			Caches.Messages.Recent.TryGet(deleteArgs.MessageId, out message);
		}
		else
		{
			DeletedSpamMessage = null;
		}

		// If this check passes, message isn't null.
		if (message?.Author.IsBot == false)
		{
			await LogDeletedMessage(message!);
			return;
		}

		WriteAsID($"The message with ID '{deleteArgs.MessageId}' was deleted, but not cached.", SpecialId.Discord);
	}

	/// <summary>
	/// Logs message edits and updates.
	/// </summary>
	public static async ValueTask MessageUpdate(IPartialMessage partialMessage)
	{
		// Ignore partial messages
		if (partialMessage is not Message message) return;

		// Don't process bot messages.
		// Too expensive computationally, as a lot of bots rely on frequent edits.
		if (message.Author.IsBot) return;

		Caches.Messages.Recent.AddOrUpdate(message.Id, message);

		if (Caches.Messages.Recent.TryGet(message.Id, out Message? OriginalMessage)) await LogUpdatedMessage(OriginalMessage, message);
	}

	/// <summary>
	/// Sets <see cref="DeletedSpamMessage"/> for faster access from the spam filter.
	/// </summary>
	public static void PipeToDeleteHandler(Message message) => DeletedSpamMessage = message;

	/// <summary>
	/// Logs reactions to messages.
	/// </summary>
	public static async ValueTask ReactionAdded(MessageReactionAddEventArgs message)
	{
		// Starboard Handling
		if (message.Emoji.Name == "⭐" && !await Files.FileContains(Files.Names.Starboard, message.MessageId.ToString(), true)) await AddToStarBoard(message);
	}
}
using PBot.Commands;
using static PBot.Messages.Functions;
using static PBot.Messages.Logging;

namespace PBot;

internal static partial class Events
{
	/// <summary>
	/// Used by <see cref="Filter(Message)"/> to pass a message to <see cref="MessageDelete(MessageDeleteEventArgs)"/>, bypassing the cache.
	/// </summary>
	public static Message? DeletedSpamMessage { private get; set; }

	/// <summary>
	/// Processes new messages.
	/// </summary>
	public static async ValueTask MessageCreate(Message message)
	{
		// Massive performance save.
		if (message.Author.IsBot) { Caches.Messages.IgnoreID(message.Id); return; }

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
		// The message was a bot message, no reason to process it.
		if (Caches.Messages.IgnoreCache.Contains(deleteArgs.MessageId)) return;

		// If DeletedSpamMessage isn't null, this event was a result of the message filter, so just use the value passed there.
		// This removes the cost of caching the message and retrieving it.
		Message? message = DeletedSpamMessage;

		if (message == null)
		{
			if (!Caches.Messages.Recent.TryGet(deleteArgs.MessageId, out message))
			{
				WriteAsID($"The message with ID '{deleteArgs.MessageId}' was deleted, but not cached.", SpecialId.Discord);
				return;
			}
		}
		else
		{
			DeletedSpamMessage = null;
		}

		await LogDeletedMessage(message);
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

		if (Caches.Messages.Recent.TryGet(message.Id, out Message? OriginalMessage)) await LogUpdatedMessage(OriginalMessage, message);

		Caches.Messages.Recent.AddOrUpdate(message.Id, message);
	}

	/// <summary>
	/// Logs reactions to messages.
	/// </summary>
	public static async ValueTask ReactionAdded(MessageReactionAddEventArgs message)
	{
		switch (message.Emoji.Name)
		{
			case "⭐":
				if (!await Files.FileContains(Files.Names.Starboard, message.MessageId.ToString(), true)) await AddToStarBoard(message);

				break;
		}
		// Starboard Handling
	}
}
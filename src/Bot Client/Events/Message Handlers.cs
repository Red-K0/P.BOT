using Bot.Interactions;
using static Bot.Data.Cache;
using static Bot.Messages.Functions;
using static Bot.Messages.Logging;

namespace Bot.Backend;

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
		bool OctoconTrigger = false;

		if (message.Author.IsBot)
		{
			// Massive performance save.
			IgnoreMessageID(message.Id);

			// To filter messages sent by octocon.
			if (message.ApplicationId == 1162755699433029723) await Filter(message);

			return;
		}

		// Octocon support
		if (message.Content.Length > 1 && message.Content[1] == ':')
		{
			IgnoreMessageID(message.Id);
			OctoconTrigger = true;
		}

		// If a command runs, do nothing else.
		if (message.Content.Length > 3 && message.Content[0] == '.')
		{
			await TextCommands.Parse(message);
			return;
		}

		if (await Filter(message)) return;

		RecentMessages.AddOrUpdate(message.Id, message);

		if (message.Content.Contains(GuildURL)) await ParseLinks(message);

		LogCreatedMessage(message, OctoconTrigger);
	}

	/// <summary>
	/// Logs message deletions.
	/// </summary>
	public static async ValueTask MessageDelete(MessageDeleteEventArgs deleteArgs)
	{
		// The message was a bot message, no reason to process it.
		if (IgnoredMessageIDs.Contains(deleteArgs.MessageId)) return;

		// If DeletedSpamMessage isn't null, this event was a result of the message filter, so just use the value passed there.
		// This removes the cost of caching the message and retrieving it.
		Message? message = DeletedSpamMessage;

		if (message == null)
		{
			if (!RecentMessages.TryGet(deleteArgs.MessageId, out message))
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
	public static async ValueTask MessageUpdate(Message message)
	{
		// Don't process bot messages.
		// Too expensive computationally, as a lot of bots rely on frequent edits.
		if (message.Author.IsBot) return;

		if (RecentMessages.TryGet(message.Id, out Message? OriginalMessage)) await LogUpdatedMessage(OriginalMessage, message);

		RecentMessages.AddOrUpdate(message.Id, message);
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
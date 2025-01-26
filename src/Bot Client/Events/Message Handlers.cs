using static Bot.Messages.Functions;
using static Bot.Messages.Logging;
using static Bot.Data.Cache;

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
	/// <param name="message">The message to process.</param>
	public static async ValueTask MessageCreate(Message message)
	{
		bool octoconTrigger = false;

		if (message.Author.IsBot)
		{
			// Massive performance save.
			AddToIgnoreList(message.Id);

			// To filter messages sent by octocon.
			if (message.ApplicationId == 1162755699433029723) await Filter(message);

			return;
		}

		// Octocon support
		if (message.Content.Length > 1 && message.Content[1] == ':')
		{
			AddToIgnoreList(message.Id);
			octoconTrigger = true;
		}

		// If a command runs, do nothing else.
		if (message.Content.Length > 3 && message.Content[0] == '.')
		{
			switch (message.Content[1])
			{
				case 'r':

					string rollString = message.Content[(message.Content.IndexOf(' ') + 1)..];
					int splitIndex = rollString.IndexOf('d');
					int modIndex = rollString.IndexOf(' ');

					await message.ReplyAsync(Interactions.Helpers.DiceRoller.Run(
						uint.Parse(rollString[..splitIndex]),
						uint.Parse(rollString[(splitIndex + 1)..(modIndex == -1 ? rollString.Length : modIndex)]),
						modIndex == -1 ? 0 : uint.Parse(rollString[(modIndex + 1)..])));
					break;
			}
			return;
		}

		if (await Filter(message)) return;

		RecentMessages[message.Id] = message;

		if (message.Content.Contains(GuildURL)) await ParseLinks(message);

		LogCreatedMessage(message, octoconTrigger);
	}

	/// <summary>
	/// Logs message deletions.
	/// </summary>
	/// <param name="deleteArgs">The arguments relating to the deletion.</param>
	public static async ValueTask MessageDelete(MessageDeleteEventArgs deleteArgs)
	{
		// The message was a bot message, no reason to process it.
		if (MessageIgnoreList.Contains(deleteArgs.MessageId)) return;

		// If DeletedSpamMessage isn't null, this event was a result of the message filter, so just use the value passed there.
		// This removes the cost of caching the message and retrieving it.
		Message? message = DeletedSpamMessage;

		if (message == null)
		{
			if (!RecentMessages.TryGetValue(deleteArgs.MessageId, out message))
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
	/// <param name="message">The updated message to log.</param>
	public static async ValueTask MessageUpdate(Message message)
	{
		// Don't process bot messages.
		// Too expensive computationally, as a lot of bots rely on frequent edits.
		if (message.Author.IsBot) return;

		if (RecentMessages.TryGetValue(message.Id, out Message? OriginalMessage)) await LogUpdatedMessage(OriginalMessage, message);

		RecentMessages[message.Id] = message;
	}

	/// <summary>
	/// Logs reactions to messages.
	/// </summary>
	/// <param name="message">The arguments relating to the reaction.</param>
	public static async ValueTask ReactionAdded(MessageReactionAddEventArgs message)
	{
		switch (message.Emoji.Name)
		{
			// Starboard Handling
			case "⭐":
				if (!await Files.FileContains(Files.Names.Starboard, message.MessageId.ToString(), true)) await AddToStarBoard(message);
				break;
		}
	}
}
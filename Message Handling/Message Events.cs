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

		Cache.Add(message);

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
		Message? DeletedMessage = Cache.Get(message.MessageId);
		string Output = $"The message with ID '{message.MessageId}' was deleted, but was not cached.";
		EmbedAuthorProperties? Author = Embeds.CreateAuthorObject("Message Deleted", Embeds.GetAssetURL("Message Deleted Icon.png"));
		MessageProperties msg_prop;

		if (DeletedMessage != null)
		{
			Output = $"The message with ID '{message.MessageId}' was deleted. It was originally sent by {DeletedMessage.Author.Username} at {DeletedMessage.CreatedAt}, and had the contents:\n" +
			         $"{DeletedMessage.Content}";

			EmbedFooterProperties? Footer = Embeds.CreateFooterObject($"Original sent by '{DeletedMessage.Author.Username}' with ID: {message.MessageId}.");

			msg_prop = Embeds.Generate
			(
				DeletedMessage.Content,
				Author,
				DateTime.Now,
				Footer,
				0xda373c
			);
		}
		else
		{
			msg_prop = Embeds.Generate("The deleted message was not cached.", Author, DateTime.Now, new() { Text = $"ID: {message.MessageId}" }, 0xda373c);
		}

		Logging.AsDiscord(Output);
		await client.Rest.SendMessageAsync(1222917190043570207, msg_prop);
	}

	/// <summary>
	/// Logs message edits and updates.
	/// </summary>
	public static async ValueTask MessageUpdated(Message message)
	{
		if (message.Content != null)
		{
			Logging.AsDiscord
			(
			$"""
			The message with ID '{message.Id}' was updated by {message.Author.Username} with the new content:
			{message.Content}
			"""
			);
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
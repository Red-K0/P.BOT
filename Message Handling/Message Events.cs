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
		Caches.Messages.Add(message);

		if (message.Author.IsBot) return;

		SpamFilter(message);
		if (message.Content.Contains(SERVER_LINK)) ParseMessageLink(message);

		// Modular Message Parsing
		if (Command_Processing.Helpers.Options.DnDTextModule && message.Content.StartsWith('.'))
			Command_Processing.Helpers.ProbabilityStateMachine.Run(message);

		Logging.AddMessage(message);
		await ValueTask.CompletedTask;
	}

	/// <summary>
	/// Logs message deletions.
	/// </summary>
	public static async ValueTask MessageDeleted(MessageDeleteEventArgs message)
	{
		Message? DeletedMessage = Caches.Messages.Get(message.MessageId);
		if ((DeletedMessage?.Author.IsBot) != false) return;

		int AttachmentCount = DeletedMessage.Attachments.Count;
		string FooterAttachmentMessage = $"{AttachmentCount} Attachment" + (AttachmentCount == 1 ? "" : "s");

		MessageProperties msg_prop = Embeds.Generate(
			DeletedMessage,
			SERVER_LINK + DeletedMessage.Channel!.Id.ToString(),
			Embeds.CreateFooter(FooterAttachmentMessage + (AttachmentCount > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : "")),
			title: $"Message Deleted {(DeletedMessage.Channel!.TryGetName(out string? Name) ? $"in {Name}" : "")}"
		);

		Logging.AsDiscord($"The message with ID '{message.MessageId}' was deleted. It was originally sent by {DeletedMessage.Author.Username} at {DeletedMessage.CreatedAt}, and had the contents:\n" +
							DeletedMessage.Content);

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
using static PBot.Messages.Functions;
namespace PBot.Messages;

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
		if (DeletedMessage?.Author.IsBot != false) return;

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
		Message? OriginalMessage = Caches.Messages.Get(message.Id);
		if (OriginalMessage?.Author.IsBot != false) return;
		Caches.Messages.Edit(message);

		MessageProperties emsg_prop = Embeds.Generate(
			message,
			$"{SERVER_LINK}{message.Channel!.Id}/{message.Id}",
			title: $"Message Edited {(message.Channel!.TryGetName(out string? Name) ? $"in {Name}" : "")}"
		);

		await client.Rest.SendMessageAsync(1222917190043570207, emsg_prop);

		int AttachmentCount = OriginalMessage.Attachments.Count;
		string FooterAttachmentMessage = $"{AttachmentCount} Attachment" + (AttachmentCount == 1 ? "" : "s");

		MessageProperties omsg_prop = Embeds.Generate(
			OriginalMessage.Content,
			Embeds.CreateAuthor("Original Message:"),
			OriginalMessage.CreatedAt,
			Embeds.CreateFooter(FooterAttachmentMessage + (AttachmentCount > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : "")),
			-1,
			0,
			OriginalMessage.Attachments.GetImageURLs(),
			refID: OriginalMessage.Author.Id
		);

		await client.Rest.SendMessageAsync(1222917190043570207, omsg_prop);
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

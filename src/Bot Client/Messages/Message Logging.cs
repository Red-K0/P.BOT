using Bot.Interactions;

namespace Bot.Messages;

/// <summary>
/// Contains methods responsible for logging messages to the console.
/// </summary>
internal static class Logging
{
	/// <summary>
	/// The ID of the latest logged user.
	/// </summary>
	private static ulong LastAuthorID;

	/// <summary>
	/// The ID of the latest deleted user.
	/// </summary>
	private static ulong LastDeletedAuthorID;

	/// <summary>
	/// The ID of the channel to send logs to.
	/// </summary>
	private const ulong LOG_CHANNEL = 1222917190043570207;

	/// <summary>
	/// Logs messages from the system.
	/// </summary>
	public static async ValueTask LogNetworkMessage(LogMessage message)
	{
		WriteAsID(message.Message, SpecialId.Network);
		await Task.CompletedTask;
	}

	/// <summary>
	/// Logs new discord messages.
	/// </summary>
	public static void LogCreatedMessage(Message message, bool octoconTrigger)
	{
		string Message = (LastAuthorID != message.Author.Id) ?
		$"\n{message.CreatedAt.ToString()[..^7]} {message.Author,-22} - {message.Author.GetDisplayName()}\n" : "";

		if (message.ReferencedMessage?.Content != null) Message += $"↳ {message.ReferencedMessage.Content.Replace("\n", "\n  ")}\n";

		Message += octoconTrigger ? message.Content[2..] : message.Content;

		Console.WriteLine(Message); LastAuthorID = message.Author.Id;
	}

	/// <summary>
	/// Logs deleted discord messages.
	/// </summary>
	public static async ValueTask LogDeletedMessage(Message message)
	{
		int AttachmentCount = message.Attachments.Count;
		string FooterAttachmentMessage = $"{AttachmentCount} Attachment" + (AttachmentCount == 1 ? "" : "s");

		EmbedProperties embed = message.ToEmbedSet().First()
			.WithUrl(GuildURL + message.Channel!.Id)
			.WithFooter(new() { Text = FooterAttachmentMessage + (AttachmentCount > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : "") })
			.WithTitle($"Message Deleted {(message.Channel!.TryGetName(out string? Name) ? $"in {Name}" : "")}");

		if (LastDeletedAuthorID == message.Author.Id) embed = embed.WithAuthor(null);

		await Client.Rest.SendMessageAsync(LOG_CHANNEL, new()
		{
			Embeds = Embeds.CreateImageSet(($"{GuildURL}{message.Channel!.Id}", message.Attachments.GetImageURLs())).Prepend(embed)
		});

		WriteAsID($"The message with ID '{message.Id}' was deleted. It was sent by {message.Author.GetDisplayName()} @ {message.CreatedAt.ToString()[..^7]} with contents:\n{message.Content}", SpecialId.Discord);

		LastDeletedAuthorID = message.Author.Id;
	}

	/// <summary>
	/// Logs edited discord messages.
	/// </summary>
	public static async ValueTask LogUpdatedMessage(Message originalMessage, Message editedMessage)
	{
		await Client.Rest.SendMessageAsync(LOG_CHANNEL, new()
		{
			Embeds = editedMessage.ToEmbedSet(
			$"{GuildURL}{editedMessage.Channel!.Id}/{editedMessage.Id}",
			title: $"Message Edited {((editedMessage.Channel?.TryGetName(out string? Name) ?? false) ? $"in {Name}" : null)}"
		)});

		int AttachmentCount = originalMessage.Attachments.Count;

		await Client.Rest.SendMessageAsync(LOG_CHANNEL, new() {
			Embeds = Embeds.CreateImageSet(("text", originalMessage.Attachments.GetImageURLs())).Prepend(new()
			{
				Description = originalMessage.Content,
				Author = new() { Name = "Original Message:" },
				Timestamp = originalMessage.CreatedAt,
				Footer = new() { Text = $"{AttachmentCount} Attachment(s)" + (AttachmentCount > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : null) },
				Color = Common.GetColor(originalMessage.Author.Id),
			})
		});
	}

	/// <summary>
	/// Writes a message to the console using a specified <see cref="SpecialId"/>.
	/// </summary>
	public static void WriteAsID(string message, SpecialId id)
	{
		string IdString = "\n" + id switch
		{
			SpecialId.Network => $"{VirtualTerminalSequences.DarkGreen}Network",
			SpecialId.Discord => $"{VirtualTerminalSequences.DarkBlue}Discord",
			SpecialId.Verbose => $"{VirtualTerminalSequences.DarkRed}Command",
			_ => "Unknown"
		};

		Console.WriteLine($"{IdString[((LastAuthorID != (ulong)id) ? 0 : 1)..]}:{VirtualTerminalSequences.White} {message}");
		LastAuthorID = (ulong)id;
	}

	/// <summary>
	/// An enum of IDs used by the bot during logging.
	/// </summary>
	public enum SpecialId : ulong
	{
		Network = 0,
		Discord = 1,
		Verbose = 2,
	}
}
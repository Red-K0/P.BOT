namespace PBot.Messages;

/// <summary>
/// Contains methods responsible for logging messages to the console.
/// </summary>
internal static class Logging
{
	/// <summary>
	/// The ID of the latest logged user.
	/// </summary>
	private static ulong LastAuthor;

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
	public static void LogCreatedMessage(Message message)
	{
		string Message = (LastAuthor != message.Author.Id) ?
		$"\n{message.CreatedAt.ToString()[..^7]} {message.Author,-22} - {message.Author.GetDisplayName()}\n" : "";

		if (message.ReferencedMessage?.Content != null)
		{
			Message += $"↳ {message.ReferencedMessage.Content.Replace("\n", "\n  ")}\n{message.Content}";
		}
		else
		{
			Message += message.Content;
		}

		Console.WriteLine(Message); LastAuthor = message.Author.Id;
	}

	/// <summary>
	/// Logs deleted discord messages.
	/// </summary>
	public static async ValueTask LogDeletedMessage(Message message)
	{
		int AttachmentCount = message.Attachments.Count;
		string FooterAttachmentMessage = $"{AttachmentCount} Attachment" + (AttachmentCount == 1 ? "" : "s");

		MessageProperties msg_prop = Embeds.Generate(
			message,
			GuildURL + message.Channel!.Id.ToString(),
			Embeds.CreateFooter(FooterAttachmentMessage + (AttachmentCount > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : "")),
			title: $"Message Deleted {(message.Channel!.TryGetName(out string? Name) ? $"in {Name}" : "")}"
		);

		await Client.Rest.SendMessageAsync(LOG_CHANNEL, msg_prop);

		WriteAsID(
		$"The message with ID '{message.Id}' was deleted. It was sent by {message.Author.GetDisplayName()} @ {message.CreatedAt.ToString()[..^7]}" +
		$" with contents:\n{message.Content}", SpecialId.Discord);
	}

	/// <summary>
	/// Logs edited discord messages.
	/// </summary>
	public static async ValueTask LogUpdatedMessage(Message originalMessage,Message editedMessage)
	{
		MessageProperties emsg_prop = Embeds.Generate(
			editedMessage,
			$"{GuildURL}{editedMessage.Channel!.Id}/{editedMessage.Id}",
			title: $"Message Edited {(editedMessage.Channel!.TryGetName(out string? Name) ? $"in {Name}" : "")}"
		);

		await Client.Rest.SendMessageAsync(LOG_CHANNEL, emsg_prop);

		int AttachmentCount = originalMessage.Attachments.Count;
		string FooterAttachmentMessage = $"{AttachmentCount} Attachment" + (AttachmentCount == 1 ? "" : "s");

		MessageProperties omsg_prop = Embeds.Generate(
			originalMessage.Content,
			Embeds.CreateAuthor("Original Message:"),
			originalMessage.CreatedAt,
			Embeds.CreateFooter(FooterAttachmentMessage + (AttachmentCount > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : "")),
			-1,
			0,
			originalMessage.Attachments.GetImageURLs(),
			refID: originalMessage.Author.Id
		);

		await Client.Rest.SendMessageAsync(LOG_CHANNEL, omsg_prop);
	}

	/// <summary>
	/// Writes a message to the console using a specified <see cref="SpecialId"/>.
	/// </summary>
	public static void WriteAsID(string message, SpecialId id)
	{
		string IdString = "\n" + id switch
		{
			SpecialId.Network => $"{PHelper.Green}Network",
			SpecialId.Discord => $"{PHelper.Blue}Discord",
			SpecialId.Verbose => $"{PHelper.Red}Command",
			_ => "Unknown"
		};

		Console.WriteLine($"{IdString[((LastAuthor != (ulong)id) ? 0 : 1)..]}:{PHelper.None} {message}");
		LastAuthor = (ulong)id;
	}

	public enum SpecialId : ulong
	{
		Network = 0,
		Discord = 1,
		Verbose = 2,
	}
}

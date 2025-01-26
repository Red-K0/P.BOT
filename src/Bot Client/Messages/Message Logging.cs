using Bot.Interactions;

namespace Bot.Messages;

/// <summary>
/// Contains methods responsible for logging messages to the console.
/// </summary>
internal static class Logging
{
	/// <summary>
	/// The ID of the channel to send logs to.
	/// </summary>
	private const ulong _logChannel = 1222917190043570207;

	/// <summary>
	/// The ID of the last logged user.
	/// </summary>
	private static ulong _lastAuthorID;

	/// <summary>
	/// The ID of the last deleted user.
	/// </summary>
	private static ulong _lastDeletedAuthorID;

	/// <summary>
	/// Logs messages from the system.
	/// </summary>
	/// <param name="message">The message to log.</param>
	public static async ValueTask LogNetworkMessage(LogMessage message)
	{
		WriteAsID(message.Message, SpecialId.Network);
		await Task.CompletedTask;
	}

	/// <summary>
	/// Logs new discord messages.
	/// </summary>
	/// <param name="message">The message to log.</param>
	/// <param name="octoconTrigger">Whether this message was a trigger for the Octocon bot.</param>
	public static void LogCreatedMessage(Message message, bool octoconTrigger)
	{
		string text = (_lastAuthorID != message.Author.Id) ? $"\n{message.CreatedAt.ToString()[..^7]} {message.Author, -22} - {message.Author.GetDisplayName()}\n" : "";

		if (message.ReferencedMessage?.Content != null) text += $"↳ {message.ReferencedMessage.Content.Replace("\n", "\n  ")}\n";

		text += octoconTrigger ? message.Content[2..] : message.Content;

		Console.WriteLine(text);

		_lastAuthorID = message.Author.Id;
	}

	/// <summary>
	/// Logs deleted discord messages.
	/// </summary>
	/// <param name="message">The deleted message.</param>
	public static async ValueTask LogDeletedMessage(Message message)
	{
		int count = message.Attachments.Count;
		string footerText = $"{count} Attachment" + (count == 1 ? "" : "s");

		EmbedProperties embed = message.ToEmbedSet().First()
			.WithUrl(GuildURL + message.Channel!.Id)
			.WithFooter(new() { Text = footerText + (count > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : "") })
			.WithTitle($"Message Deleted {(message.Channel!.TryGetName(out string? Name) ? $"in {Name}" : "")}");

		if (_lastDeletedAuthorID == message.Author.Id) embed = embed.WithAuthor(null);

		await Client.Rest.SendMessageAsync(_logChannel, new()
		{
			Embeds = Embeds.CreateImageSet($"{GuildURL}{message.Channel!.Id}", message.Attachments.GetImageURLs()).Prepend(embed)
		});

		WriteAsID($"The message with ID '{message.Id}' was deleted. It was sent by {message.Author.GetDisplayName()} @ {message.CreatedAt.ToString()[..^7]} with contents:\n{message.Content}", SpecialId.Discord);

		_lastDeletedAuthorID = message.Author.Id;
	}

	/// <summary>
	/// Logs edited discord messages.
	/// </summary>
	/// <param name="originalMessage">The message's original state.</param>
	/// <param name="editedMessage">The message's edited state.</param>
	public static async ValueTask LogUpdatedMessage(Message originalMessage, Message editedMessage)
	{
		if (originalMessage.Content == editedMessage.Content) return;

		await Client.Rest.SendMessageAsync(_logChannel, new()
		{
			Embeds = editedMessage.ToEmbedSet(
			$"{GuildURL}{editedMessage.Channel!.Id}/{editedMessage.Id}",
			title: $"Message Edited {((editedMessage.Channel?.TryGetName(out string? Name) ?? false) ? $"in {Name}" : null)}"
		)});

		int count = originalMessage.Attachments.Count;

		await Client.Rest.SendMessageAsync(_logChannel, new() {
			Embeds = Embeds.CreateImageSet("text", originalMessage.Attachments.GetImageURLs()).Prepend(new()
			{
				Description = originalMessage.Content,
				Author = new() { Name = "Original Message:" },
				Timestamp = originalMessage.CreatedAt,
				Footer = new() { Text = $"{count} Attachment(s)" + (count > 4 ? " (Attachments not displayed can be seen in fullscreen mode)" : null) },
				Color = Common.GetColor(originalMessage.Author.Id),
			})
		});
	}

	/// <summary>
	/// Writes a message to the console using a specified <see cref="SpecialId"/>.
	/// </summary>
	/// <param name="message">The message to write to the console.</param>
	/// <param name="id">The ID to use an associated color for.</param>
	public static void WriteAsID(string message, SpecialId id)
	{
		string nameString = "\n" + id switch
		{
			SpecialId.Network => $"{Ansi.DarkGreen}Network",
			SpecialId.Discord => $"{Ansi.DarkBlue}Discord",
			SpecialId.Verbose => $"{Ansi.DarkRed}Command",
			_ => "Unknown"
		};

		Console.WriteLine($"{nameString[((_lastAuthorID != (ulong)id) ? 0 : 1)..]}:{Ansi.White} {message}");

		_lastAuthorID = (ulong)id;
	}

	/// <summary>
	/// A list of IDs used by the bot during logging.
	/// </summary>
	public enum SpecialId : ulong
	{
		Network = 0,
		Discord = 1,
		Verbose = 2,
	}
}
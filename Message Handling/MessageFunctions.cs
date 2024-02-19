namespace P_BOT;

/// <summary> Contains methods and variables used for basic message functionality and parsing. </summary>
internal static class MessageFunctions
{
	#region Variables
	/// <summary> The ID of the last logged user. </summary>
	public static ulong? LastAuthor = 0;

	/// <summary> The number of messages starred by P.BOT. </summary>
	private static int StarCount = Convert.ToInt32(DataBackend.ReadMemory(1, DataBackend.Pages.Counter));

	#region Spam Filter
	/// <summary> The <see cref="Attachment.FileName"/> of the last <see cref="Attachment"/> sent. </summary>
	private static string LastAttachment = "";

	/// <summary> The <see cref="RestMessage.Content"/> of the last <see cref="RestMessage"/> sent. </summary>
	private static string LastContent = "";

	/// <summary> The number of identical <see cref="Attachment"/> objects sent. </summary>
	private static int AttachmentFilterLimit;

	/// <summary> The number of identical <see cref="RestMessage.Content"/> objects sent. </summary>
	private static int MessageFilterLimit;
	#endregion

	#endregion

	/// <summary> Parses the contents of a given <see cref="MessageReactionAddEventArgs"/> and adds the result to the starboard. </summary>
	/// <param name="message"> The <see cref="MessageReactionAddEventArgs"/> containing the message to add to the starboard. </param>
	public static async void AddToStarBoard(MessageReactionAddEventArgs message)
	{
		RestMessage Message = await client.Rest.GetMessageAsync(message.ChannelId, message.MessageId);
		MessageProperties msg_prop = new();
		if (Message.ReferencedMessage != null)
		{
			RestMessage Reply = client.Rest.GetMessageAsync(Message.ReferencedMessage.ChannelId, Message.ReferencedMessage.Id).Result;
			EmbedAuthorProperties Author = new()
			{
				Name = $"Replying to: {Message.ReferencedMessage.Author.Username}",
				IconUrl = Message.ReferencedMessage.Author.GetAvatarUrl().ToString()
			};

			_ = msg_prop.AddEmbeds
			(
				EmbedHelpers.ToEmbed(Reply,
				$"{SERVER_LINK}{Message.ReferencedMessage.ChannelId}/{Message.ReferencedMessage.Id}"
			).Embeds!.First().WithAuthor(Author));
		}
		_ = msg_prop.AddEmbeds(EmbedHelpers.ToEmbed
		(
			Message,
			$"{SERVER_LINK}{message.ChannelId}/{message.MessageId}",
			$"Message starred by {message.User!.Username}",
			message.User.GetAvatarUrl().ToString(),
			message.MessageId,
			$"Starboard Entry #{StarCount}"
		).Embeds!);
		StarCount++;

		DataBackend.AppendMemory(DataBackend.Pages.StarredMessageList, message.MessageId.ToString());
		DataBackend.WriteMemory(1, DataBackend.Pages.Counter, StarCount.ToString());

		_ = await client.Rest.SendMessageAsync(SERVER_STARBOARD, msg_prop.WithContent("# " + new string('⭐', Math.Clamp(Message.Attachments.Count, 1, 10))));
	}

	/// <summary> Logs a given <paramref name="message"/> in the console, using <see cref="GetAnnotation(string)"/>. </summary>
	/// <param name="message"> The <see cref="Message"/> object to log. </param>
	public static void LogMessage(in Message message)
	{
		if (!string.IsNullOrWhiteSpace(message.Content))
		{
			if (LastAuthor != message.Author.Id)
			{
				Console.WriteLine($"\n{message.CreatedAt} {message.Author,-22} - {message.Author.Username}");
			}

			string Annotation = GetAnnotation(message.Content);
			Console.WriteLine($"{message.Content} {(string.IsNullOrWhiteSpace(Annotation) ? '\0' : "< ")}{Annotation}");
			LastAuthor = message.Author.Id;
		}
	}

	/// <summary> Parses a given <paramref name="message"/> to check for message links, and displays their content if possible. </summary>
	/// <param name="message"> The <see cref="Message"/> object to check for and parse links in. </param>
	public static void ParseMessageLink(in Message message)
	{
		//HACK | The '49's below could pose a compatibility issue in the future. If this breaks for no reason later, you know why.
		string Scan = message.Content; string CurrentScan; ulong ChannelID; ulong MessageID; RestMessage LinkedMessage;
		int LinkCount = (Scan.Length - Scan.Replace(SERVER_LINK, "").Length) / SERVER_LINK.Length;
		for (int i = 0; i < LinkCount; i++)
		{
			CurrentScan = Scan[(Scan.IndexOf(SERVER_LINK) + 49)..];
			if (CurrentScan.Contains(' '))
			{
				CurrentScan = CurrentScan.Remove(CurrentScan.IndexOf(' '));
			}

			if (CurrentScan.Contains('\n'))
			{
				CurrentScan = CurrentScan.Remove(CurrentScan.IndexOf('\n'));
			}

			try { ChannelID = ulong.Parse(CurrentScan.Remove(CurrentScan.IndexOf('/'))); } catch (Exception) { return; }
			try { MessageID = ulong.Parse(CurrentScan[(CurrentScan.IndexOf('/') + 1)..].Replace('/','\0')); } catch (Exception) { return; }
			LinkedMessage = client.Rest.GetMessageAsync(ChannelID, MessageID, null).Result;

			MessageProperties msg_prop = EmbedHelpers.ToEmbed(
				LinkedMessage,
				$"{SERVER_LINK}{message.ChannelId}/{message.Id}",
				$"Message linked by {message.Author.Username}",
				message.Author.GetAvatarUrl().ToString(),
				message.Id
			);
			_ = client.Rest.SendMessageAsync(message.ChannelId, msg_prop);
			Scan = Scan.Remove(Scan.IndexOf(SERVER_LINK), 49 + CurrentScan.Length);
		}
	}

	/// <summary> Monitors and deletes messages to avoid spam. </summary>
	/// <param name="message"> The <see cref="RestMessage"/> object to filter. </param>
	public static void SpamFilter(in Message message)
	{
		bool Filter = false;

		if (message.Content == LastContent) { Filter = true;  MessageFilterLimit++; }
									   else { Filter = false; MessageFilterLimit = Math.Max(MessageFilterLimit - 1, 0); }
		LastContent = message.Content;

		KeyValuePair<ulong, Attachment>[] Attachments = [.. message.Attachments];
		for (int i = 0; i < Attachments.Length; i++)
		{
			if (Attachments[i].Value.FileName == "image.png" || Attachments[i].Value.FileName == "unknown.png") continue;

			if (Attachments[i].Value.FileName == LastAttachment) { Filter = true;  AttachmentFilterLimit++; }
															else { Filter = false; AttachmentFilterLimit = Math.Max(AttachmentFilterLimit - 1, 0); }
			LastAttachment = Attachments[i].Value.FileName;
		}

		if ((AttachmentFilterLimit >= 3 || MessageFilterLimit >= 5) && Filter)
		{
			string  response = AttachmentFilterLimit switch
			{
				3 => $"<@{message.Author.Id}> please avoid spamming.",
				4 => $"<@{message.Author.Id}> spamming interrupts others and the flow of chat, please avoid so.",
				5 => $"<@{message.Author.Id}> avoid spamming, final warning.",
				6 => "<@&1147933921829470399> spam limit reached, timeout necessary.",
				_ => ""
			};
					response = MessageFilterLimit switch
			{
				5 => $"<@{message.Author.Id}> please avoid spamming.",
				6 => $"<@{message.Author.Id}> spamming interrupts others and the flow of chat, please avoid so.",
				7 => $"<@{message.Author.Id}> avoid spamming, final warning.",
				8 => "<@&1147933921829470399> spam limit reached, timeout necessary.",
				_ => response
			};

			client.Rest.SendMessageAsync(message.ChannelId, response);
			client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);
		}
	}

	/// <summary> Responsible for setting text color in P.BOT's console log and annotating it. </summary>
	/// <param name="content"> The <see cref="Message"/> object to highlight in the console. </param>
	private static string GetAnnotation(string content)
	{
		//Message Highlighting
		if (content.Contains(SERVER_LINK))
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			return "Discord Message Link";
		}

		if (content.StartsWith('.'))
		{
			Console.ForegroundColor = Command_Processing.Helpers.Options.DnDTextModule ? ConsoleColor.Green : ConsoleColor.Red;
			return $"Call to DnDTextModule {(Command_Processing.Helpers.Options.DnDTextModule ? "(Processed)" : "(Ignored)")}";
		}

		return "";
	}

	/// <summary> Writes the string <paramref name="content"/> to the console in the color specified by <paramref name="color"/>. </summary>
	/// <param name="content"> The <see cref="string"/> to write to the console. </param>
	/// <param name="color"> The color to write the <paramref name="content"/> in. </param>
	public static void WriteColor(string content, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.Write(content);
		Console.ForegroundColor = ConsoleColor.Gray;
	}
}
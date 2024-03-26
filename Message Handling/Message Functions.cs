namespace P_BOT.Messages;

/// <summary>
/// Contains methods and variables used for basic message functionality and parsing.
/// </summary>
internal static class Functions
{
	#region Filter Fields
	/// <summary>
	/// The <see cref="Attachment.FileName"/> of the last <see cref="Attachment"/> sent.
	/// </summary>
	private static string LastAttachment = "";

	/// <summary>
	/// The <see cref="RestMessage.Content"/> of the last <see cref="RestMessage"/> sent.
	/// </summary>
	private static string LastContent = "";

	/// <summary>
	/// The number of identical <see cref="Attachment"/> objects sent.
	/// </summary>
	private static int AttachmentFilterLimit;

	/// <summary>
	/// The number of identical <see cref="RestMessage.Content"/> objects sent.
	/// </summary>
	private static int MessageFilterLimit;
	#endregion

	/// <summary> Parses the contents of a given <see cref="MessageReactionAddEventArgs"/> and adds the result to the starboard. </summary>
	/// <param name="message"> The <see cref="MessageReactionAddEventArgs"/> containing the message to add to the starboard. </param>
	public static async void AddToStarBoard(MessageReactionAddEventArgs message)
	{
#if DEBUG_EVENTS
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		const ulong STARBOARD = 1133836713194696744;

		int StarCount = Convert.ToInt32(Pages.Read(Pages.Files.Counters, 1).Result);
		RestMessage Message = await client.Rest.GetMessageAsync(message.ChannelId, message.MessageId);
		MessageProperties msg_prop = new();

		if (Message.ReferencedMessage != null)
		{
			RestMessage Reply = client.Rest.GetMessageAsync(Message.ReferencedMessage.ChannelId, Message.ReferencedMessage.Id).Result;
			EmbedAuthorProperties Author = Embeds.CreateAuthorObject
			(
				$"Replying to: {Message.ReferencedMessage.Author.Username}",
				Message.ReferencedMessage.Author.GetAvatarUrl().ToString()
			);

			msg_prop.AddEmbeds
			(
				Embeds.Generate(Reply,
				$"{SERVER_LINK}{Message.ReferencedMessage.ChannelId}/{Message.ReferencedMessage.Id}"
			).Embeds!.First().WithAuthor(Author));
		}

		msg_prop.AddEmbeds(Embeds.Generate
		(
			Message,
			$"{SERVER_LINK}{message.ChannelId}/{message.MessageId}",
			$"Message starred by {message.User!.Username}",
			message.User.GetAvatarUrl().ToString(),
			message.MessageId,
			$"Starboard Entry #{StarCount}"
		).Embeds!);

		StarCount++;
		Pages.Append(Pages.Files.Starboard, message.MessageId.ToString());
		Pages.Write(Pages.Files.Counters, 1, StarCount.ToString());

		await client.Rest.SendMessageAsync(STARBOARD, msg_prop.WithContent("# " + new string('⭐', Math.Clamp(Message.Attachments.Count, 1, 10))));

#if DEBUG_EVENTS
		Logging.AsVerbose($"Starboard Processed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}

	/// <summary> Parses a given <paramref name="message"/> to check for message links, and displays their content if possible. </summary>
	/// <param name="message"> The <see cref="Message"/> object to check for and parse links in. </param>
	public static async void ParseMessageLink(Message message)
	{
#if DEBUG_EVENTS
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		//HACK | The '49's below could pose a compatibility issue in the future. If this breaks for no reason later, you know why.
		string Scan = message.Content; string CurrentScan; RestMessage LinkedMessage; ulong ChannelID;
		int LinkCount = (Scan.Length - Scan.Replace(SERVER_LINK, "").Length) / SERVER_LINK.Length;
		for (int i = 0; i < LinkCount; i++)
		{
			CurrentScan = Scan[(Scan.IndexOf(SERVER_LINK) + 49)..];

			if (CurrentScan.Contains(' ')) CurrentScan = CurrentScan.Remove(CurrentScan.IndexOf(' '));
			if (CurrentScan.Contains('\n')) CurrentScan = CurrentScan.Remove(CurrentScan.IndexOf('\n'));

			if (CurrentScan.Contains('/'))
			{
				if (!ulong.TryParse(CurrentScan.Remove(CurrentScan.IndexOf('/')), out ChannelID))
				{
					return;
				}
			}
			else
			{
				return;
			}

			if (!ulong.TryParse(CurrentScan[(CurrentScan.IndexOf('/') + 1)..].Replace('/', '\0'), out ulong MessageID))
			{
				return;
			}

			LinkedMessage = await client.Rest.GetMessageAsync(ChannelID, MessageID, null);

			MessageProperties msg_prop = Embeds.Generate(
				LinkedMessage,
				$"{SERVER_LINK}{message.ChannelId}/{message.Id}",
				$"Message linked by {message.Author.Username}",
				message.Author.GetAvatarUrl().ToString(),
				message.Id
			);
			await client.Rest.SendMessageAsync(message.ChannelId, msg_prop);
			Scan = Scan.Remove(Scan.IndexOf(SERVER_LINK) + 49 + CurrentScan.Length);
		}

#if DEBUG_EVENTS
		Logging.AsVerbose($"Link Parsed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}

	/// <summary> Monitors and deletes messages to avoid spam. </summary>
	/// <param name="message"> The <see cref="RestMessage"/> object to filter. </param>
	public static async void SpamFilter(Message message)
	{
		ulong ID = message.Author.Id;
		bool filter = false;
		string response;

		if (message.Content.StartsWith("# ") && message.Content.Equals(message.Content, StringComparison.Ordinal))
		{
			if (MessageFilterLimit < 4)
			{
				MessageFilterLimit = 4;
			}
			else
			{
				MessageFilterLimit++;
			}

			filter = true;
		}

		if (message.Content == LastContent) { filter = true; MessageFilterLimit++; }
		else if (!filter) { MessageFilterLimit = Math.Max(MessageFilterLimit - 1, 0); }
		LastContent = message.Content;

		foreach (KeyValuePair<ulong, Attachment> Attachment in (KeyValuePair<ulong, Attachment>[])([.. message.Attachments]))
		{
			if (Attachment.Value.FileName is "image.png" or "unknown.png")
			{
				continue;
			}

			if (Attachment.Value.FileName == LastAttachment) { filter = true; AttachmentFilterLimit++; }
			else { filter = false; AttachmentFilterLimit = Math.Max(AttachmentFilterLimit - 1, 0); }
			LastAttachment = Attachment.Value.FileName;
		}

		if ((AttachmentFilterLimit >= 3 || MessageFilterLimit >= 5) && filter)
		{
			response = AttachmentFilterLimit switch
			{
				3 => $"<@{ID}> please avoid spamming.",
				4 => $"<@{ID}> spamming interrupts others and the flow of chat, please avoid so.",
				5 => $"<@{ID}> avoid spamming, final warning.",
				6 => "<@&1147933921829470399> spam limit reached, timeout necessary.",
				_ => ""
			};
			response = MessageFilterLimit switch
			{
				5 => $"<@{ID}> please avoid spamming.",
				6 => $"<@{ID}> spamming interrupts others and the flow of chat, please avoid so.",
				7 => $"<@{ID}> avoid spamming, final warning.",
				8 => "<@&1147933921829470399> spam limit reached, timeout necessary.",
				_ => response
			};

			if (!string.IsNullOrWhiteSpace(response))
			{
				_ = await client.Rest.SendMessageAsync(message.ChannelId, response);
			}
			await client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);
		}
	}
}
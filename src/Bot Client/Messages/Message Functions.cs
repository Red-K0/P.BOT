namespace PBot.Messages;

/// <summary>
/// Contains methods and variables used for basic message functionality and parsing.
/// </summary>
internal static class Functions
{
	/// <summary> Parses the contents of a given <see cref="MessageReactionAddEventArgs"/> and adds the result to the starboard. </summary>
	/// <param name="message"> The <see cref="MessageReactionAddEventArgs"/> containing the message to add to the starboard. </param>
	public static async void AddToStarBoard(MessageReactionAddEventArgs message)
	{
#if DEBUG_EVENTS
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		const ulong STARBOARD = 1133836713194696744;

		string StarCount = await Pages.Read(Pages.Files.Counters, 1);
		RestMessage Message = await Client.Rest.GetMessageAsync(message.ChannelId, message.MessageId);
		MessageProperties msg_prop = new();

		if (Message.ReferencedMessage != null)
		{
			msg_prop.AddEmbeds
			(
				Embeds.Generate(Client.Rest.GetMessageAsync(Message.ReferencedMessage.ChannelId, Message.ReferencedMessage.Id).Result,
				$"{GuildURL}{Message.ReferencedMessage.ChannelId}/{Message.ReferencedMessage.Id}"
			).Embeds!.First().WithAuthor(Embeds.CreateAuthor
			(
				$"Replying to: {Message.ReferencedMessage.Author.GetDisplayName()}",
				Message.ReferencedMessage.Author.GetAvatar()
			)));
		}

		msg_prop.AddEmbeds(Embeds.Generate
		(
			Message,
			$"{GuildURL}{message.ChannelId}/{message.MessageId}",
			Embeds.CreateFooter($"Message starred by {message.User!.GetDisplayName()}", message.User!.GetAvatar().ToString()),
			message.MessageId,
			$"Starboard Entry #{StarCount}"
		).Embeds!);

		await Client.Rest.SendMessageAsync(STARBOARD, msg_prop.ToChecked().WithContent(new string('⭐', Math.Clamp(Message.Attachments.Count, 1, 10)) + "_ _"));

		Pages.Append(Pages.Files.Starboard, message.MessageId.ToString());
		Pages.Write(Pages.Files.Counters, 1, (Convert.ToInt32(StarCount) + 1).ToString());

#if DEBUG_EVENTS
		Logging.AsVerbose($"Starboard Processed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}

	/// <summary>
	/// Parses a given <paramref name="message"/> to check for message links, and displays their content if possible.
	/// </summary>
	/// <param name="message"> The <see cref="Message"/> object to check for and parse links in. </param>
	public static async void ParseLinks(Message message)
	{
#if DEBUG_EVENTS
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		//HACK: The '49's below could pose a compatibility issue in the future. If this breaks for no reason later, you know why.
		string Scan = message.Content.Replace("https://", " https://"); RestMessage? LinkedMessage; int i = 0;
		int LinkCount = (Scan.Length - Scan.Replace(GuildURL, "").Length) / GuildURL.Length;
		do
		{
			i++;
			if ((LinkedMessage = await Scan.GetMessage()) == null) goto InvalidLink;

			await Client.Rest.SendMessageAsync(message.ChannelId, Embeds.Generate
			(
				LinkedMessage,
				$"{GuildURL}{message.ChannelId}/{message.Id}",
				Embeds.CreateFooter($"Message linked by {message.Author.GetDisplayName()}", message.Author.GetAvatar().ToString()),
				message.Id
			).ToChecked());

		InvalidLink:
			if (i != LinkCount) Scan = Scan[(Scan.IndexOf(GuildURL) + GuildURL.Length)..];
		}
		while (i < LinkCount);

#if DEBUG_EVENTS
		Logging.AsVerbose($"Link Parsed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}

	/// <summary>
	/// Compares a given message's attributes to other messages by the same user, and deletes it if the filter's criteria are met.
	/// </summary>
	/// <param name="message">The <see cref="Message"/> object to perform comparison on.</param>
	public static async Task<bool> Filter(Message message)
	{
		Caches.Members.Member Member = Caches.Members.List[message.Author.Id];

		if (message.Content.StartsWith("# "))
		{
			if (Member.SpamLastMessageHeading)
			{
				return await FilterHit();
			}
			else
			{
				Member.SpamLastMessageHeading = true;
			}
		}
		else
		{
			Member.SpamLastMessageHeading = false;
		}

		if (message.Content == Member.SpamLastMessage)
		{
			// Double penalize link spam.
			if (message.Content.Contains("://")) Member.SpamSameMessageCount++;
			if (Member.SpamSameMessageCount++ > 1) return await FilterHit();
		}
		else
		{
			if (Member.SpamSameMessageCount != 0) Member.SpamSameMessageCount--;
			Member.SpamLastMessage = message.Content;
		}

		// If there are no attachments, save state and exit early.
		if (!message.Attachments.Any()) goto NoAttachments;

		if (message.Attachments.First().Value.Size == Member.SpamLastAttachmentSize)
		{
			if (Member.SpamSameMessageCount++ > 1) return await FilterHit();
		}
		else
		{
			if (Member.SpamSameMessageCount != 0) Member.SpamSameMessageCount--;
			Member.SpamLastAttachmentSize = message.Attachments.First().Value.Size;
		}

	NoAttachments:
		Caches.Members.List[message.Author.Id] = Member;
		return false;

		async Task<bool> FilterHit()
		{
			// This passes the message to the deleted message handler directly.
			// More information on this is in the handler's code.
			Events.DeletedSpamMessage = message;
			await Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);

			if (Member.SpamSameMessageCount > 5)
			{
				await Client.Rest.ModifyGuildUserAsync(GuildID, message.Author.Id, u => u.WithTimeOutUntil(new(DateTime.Now.AddMinutes(30))));
				Member.SpamSameMessageCount = 0;
				goto UpdateMember;
			}

			await Client.Rest.SendMessageAsync(message.ChannelId, Member.SpamSameMessageCount switch
			{
				3 => $"<@{message.Author.Id}> please avoid spamming.",
				4 => $"<@{message.Author.Id}> spamming interrupts others and the flow of chat, please avoid so.",
				5 => $"<@{message.Author.Id}> avoid spamming, final warning.",
				_ => $"<@{message.Author.Id}> please avoid overuse of # formatting."
			});

		UpdateMember:
			Caches.Members.List[message.Author.Id] = Member;
			return true;
		}
	}
}

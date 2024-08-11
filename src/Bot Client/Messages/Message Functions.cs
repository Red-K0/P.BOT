namespace PBot.Messages;

/// <summary>
/// Contains methods and variables used for basic message functionality and parsing.
/// </summary>
internal static class Functions
{
	/// <summary> Parses the contents of a given <see cref="MessageReactionAddEventArgs"/> and adds the result to the starboard. </summary>
	/// <param name="message"> The <see cref="MessageReactionAddEventArgs"/> containing the message to add to the starboard. </param>
	public static async Task AddToStarBoard(MessageReactionAddEventArgs message)
	{
		RestMessage Message = await Client.Rest.GetMessageAsync(message.ChannelId, message.MessageId);
		InteractionMessageProperties StarMessage = new();
		RestMessage? RepliedTo = Message.ReferencedMessage;

		if (RepliedTo != null) StarMessage.AddEmbeds(
			Client.Rest.GetMessageAsync(RepliedTo.ChannelId, RepliedTo.Id).Result.ToEmbed($"{GuildURL}{RepliedTo.ChannelId}/{RepliedTo.Id}")
			.Embeds!.First().WithAuthor(Embeds.CreateAuthor($"Replying to: {RepliedTo.Author.GetDisplayName()}", RepliedTo.Author.GetAvatar()))
		);

		StarMessage.AddEmbeds(
			Message.ToEmbed(
				$"{GuildURL}{message.ChannelId}/{message.MessageId}",
				Embeds.CreateFooter($"Message starred by {message.User!.GetDisplayName()}", message.User!.GetAvatar()),
				$"Starboard Entry #{await Files.ReadCounter(Files.CounterLines.Starboard, 1)}"
			).Embeds!
		);

		// Starboard Channel
		await Client.Rest.SendMessageAsync(1133836713194696744, StarMessage.ToChecked().ToMessage());
	}

	/// <summary>
	/// Parses a given <paramref name="message"/> to check for message links, and displays their content if possible.
	/// </summary>
	/// <param name="message"> The <see cref="Message"/> object to check for and parse links in. </param>
	public static async Task ParseLinks(Message message)
	{
		string Scan = message.Content.Replace("https://", " https://"); RestMessage? LinkedMessage; int i = 0;
		ulong LastAuthorID = 0; int LinkCount = (Scan.Length - Scan.Replace(GuildURL, "").Length) / GuildURL.Length;
		ulong[] ParsedLinks = new ulong[LinkCount];
		do
		{
			i++;
			if ((LinkedMessage = await Scan.GetMessage()) != null && !ParsedLinks.Contains(LinkedMessage.Id))
			{
				EmbedProperties embed = LinkedMessage.ToEmbed().Embeds!.First().WithUrl($"{GuildURL}{message.ChannelId}/{message.Id}");

				if (i == LinkCount)
				{
					embed = embed.WithFooter(Embeds.CreateFooter($"Message linked by {message.Author.GetDisplayName()}", message.Author.GetAvatar()));
				}
				else
				{
					embed = embed.WithTimestamp(null);
				}

				await Client.Rest.SendMessageAsync(
					message.ChannelId,
					Embeds.Generate(
						LastAuthorID == LinkedMessage.Author.Id ? embed.WithAuthor(null) : embed,
						LinkedMessage.Attachments.GetImageURLs(),
						embed.Url).ToChecked().ToMessage()
				);

				ParsedLinks[i] = LinkedMessage.Id;
				LastAuthorID = LinkedMessage.Author.Id;
			}

			if (i != LinkCount) Scan = Scan[(Scan.IndexOf(GuildURL) + GuildURL.Length)..];
		}
		while (i < LinkCount);
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
		if (message.Attachments.Any())
		{
			if (message.Attachments.First().Value.Size == Member.SpamLastAttachmentSize)
			{
				if (Member.SpamSameMessageCount++ > 1) return await FilterHit();
			}
			else
			{
				if (Member.SpamSameMessageCount != 0) Member.SpamSameMessageCount--;
				Member.SpamLastAttachmentSize = message.Attachments.First().Value.Size;
			}
		}

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
			}
			else
			{
				await Client.Rest.SendMessageAsync(message.ChannelId, Member.SpamSameMessageCount switch
				{
					3 => $"<@{message.Author.Id}> please avoid spamming.",
					4 => $"<@{message.Author.Id}> spamming interrupts others and the flow of chat, please avoid doing so.",
					5 => $"<@{message.Author.Id}> avoid spamming, final warning.",
					_ => $"<@{message.Author.Id}> please avoid overuse of # formatting."
				});
			}

			Caches.Members.List[message.Author.Id] = Member;
			return true;
		}
	}
}

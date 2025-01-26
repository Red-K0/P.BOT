using Bot.Interactions;

namespace Bot.Messages;

/// <summary>
/// Contains methods and variables used for basic message functionality and parsing.
/// </summary>
internal static class Functions
{
	/// <summary> Parses the contents of a given <see cref="MessageReactionAddEventArgs"/> and adds the result to the starboard. </summary>
	/// <param name="args"> The <see cref="MessageReactionAddEventArgs"/> containing the message to add to the starboard. </param>
	public static async Task AddToStarBoard(MessageReactionAddEventArgs args)
	{
		RestMessage message = await Client.Rest.GetMessageAsync(args.ChannelId, args.MessageId);
		MessageProperties starMessage = new();

		if (message.ReferencedMessage != null)
		{
			starMessage.AddEmbeds(Client.Rest.GetMessageAsync(message.ReferencedMessage.ChannelId, message.ReferencedMessage.Id).Result
			.ToEmbedSet($"{GuildURL}{message.ReferencedMessage.ChannelId}/{message.ReferencedMessage.Id}")
			.First()
			.WithAuthor(new() { IconUrl = message.ReferencedMessage.Author.GetAvatar(), Name = $"Replying to: {message.ReferencedMessage.Author.GetDisplayName()}"}));
		}

		starMessage.AddEmbeds(message.ToEmbedSet(
			$"{GuildURL}{args.ChannelId}/{args.MessageId}",
			new() { IconUrl = args.User!.GetAvatar(), Text = $"Message starred by {args.User!.GetDisplayName()}" },
			title: $"Starboard Entry #{await Files.GetCounter(Files.CounterLines.Starboard, 1)}"));

		// Starboard Channel
		await Client.Rest.SendMessageAsync(1133836713194696744, starMessage);
	}

	/// <summary>
	/// Gets the relevant <see cref="Message"/> object from a Discord message URL.
	/// </summary>
	/// <param name="link">The URL to find a message object from.</param>
	public static async Task<(RestMessage? Message, bool Restricted)> GetMessageFromLink(string link)
	{
		if (link.Contains(GuildURL) && link.Length > (link.IndexOf(GuildURL) + GuildURL.Length))
		{
			link = link[(link.IndexOf(GuildURL) + GuildURL.Length)..];

			if (link.Contains(' ')) link = link.Remove(link.IndexOf(' '));
			if (link.Contains('\n')) link = link.Remove(link.IndexOf('\n'));

			if (ulong.TryParse(link.Remove(link.LastIndexOf('/')), out ulong Channel) && ulong.TryParse(link[(link.LastIndexOf('/') + 1)..], out ulong Message))
			{
				return (await Client.Rest.GetMessageAsync(Channel, Message), ((TextGuildChannel)await Client.Rest.GetChannelAsync(Channel)).ParentId == 1165525787580051526);
			}
		}

		return (null, false);
	}

	/// <summary>
	/// Parses a given <paramref name="message"/> to check for message links, and displays their content if possible.
	/// </summary>
	/// <param name="message"> The <see cref="Message"/> object to check for and parse links in. </param>
	public static async Task ParseLinks(Message message)
	{
		string scan = message.Content.Replace("https://", " https://"); (RestMessage? Message, bool Restricted) linkedMessage; int i = 0;
		ulong lastAuthorID = 0; int linkCount = (scan.Length - scan.Replace(GuildURL, "").Length) / GuildURL.Length;
		ulong[] parsedLinks = new ulong[linkCount];
		do
		{
			i++;
			if ((linkedMessage = await GetMessageFromLink(scan)).Message != null && !parsedLinks.Contains(linkedMessage.Message.Id))
			{
				EmbedProperties embed = linkedMessage.Message.ToEmbedSet(url: $"{GuildURL}{message.ChannelId}/{message.Id}", censor: linkedMessage.Restricted).First();

				embed = i == linkCount
					? embed.WithFooter(new() { Text = $"Message linked by {message.Author.GetDisplayName()}", IconUrl = message.Author.GetAvatar() })
					: embed.WithTimestamp(null);

				await Client.Rest.SendMessageAsync(
					message.ChannelId,
					new()
					{
						Embeds = Embeds.CreateImageSet(embed.Url, linkedMessage.Message.Attachments.GetImageURLs().Skip(1))
						.Prepend(lastAuthorID == linkedMessage.Message.Author.Id ? embed.WithAuthor(null) : embed)
					}
				);

				parsedLinks[i - 1] = linkedMessage.Message.Id;
				lastAuthorID = linkedMessage.Message.Author.Id;
			}

			if (i != linkCount) scan = scan[(scan.IndexOf(GuildURL) + GuildURL.Length)..];
		}
		while (i < linkCount);
	}

	/// <summary>
	/// Compares a given message's attributes to other messages by the same user, and deletes it if the filter's criteria are met.
	/// </summary>
	/// <param name="message">The <see cref="Message"/> object to perform comparison on.</param>
	public static async Task<bool> Filter(Message message)
	{
		Members.Member member = Members.MemberList[message.Author.Id];

		ReadOnlySpan<char> content = message.Content.AsSpan();

		if (content.StartsWith("# "))
		{
			if (member.SpamLastMessageHeading)
			{
				return await FilterHit();
			}
			else
			{
				member.SpamLastMessageHeading = true;
			}
		}
		else
		{
			member.SpamLastMessageHeading = false;
		}

		if (content == member.LastMessage)
		{
			// Double penalize link spam.
			if (content.Contains("://", StringComparison.Ordinal)) member.SpamSameMessageCount++;
			if (member.SpamSameMessageCount++ > 1) return await FilterHit();
		}
		else
		{
			if (member.SpamSameMessageCount != 0) member.SpamSameMessageCount--;
			member.LastMessage = message.Content;
		}

		if (content.Contains("lost", StringComparison.OrdinalIgnoreCase) && content.Contains("game", StringComparison.OrdinalIgnoreCase))
		{
			return await FilterHit(true);
		}

		// If there are no attachments, save state and exit early.
		if (message.Attachments.Any())
		{
			if (message.Attachments[0].Size == member.LastAttachmentSize)
			{
				if (member.SpamSameMessageCount++ > 1) return await FilterHit();
			}
			else
			{
				if (member.SpamSameMessageCount != 0) member.SpamSameMessageCount--;
				member.LastAttachmentSize = message.Attachments[0].Size;
			}
		}

		Members.MemberList[message.Author.Id] = member;
		return false;

		async Task<bool> FilterHit(bool gameLost = false)
		{
			// This passes the message to the deleted message handler directly.
			// More information on this is in the handler's code.
			Events.DeletedSpamMessage = message;
			await Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);

			if (gameLost)
			{
				if ((Interactions.Helpers.DiceRoller.Next() & 0b0110) == 0b0110) await Client.Rest.SendMessageAsync(message.ChannelId, $"<@{message.Author.Id}>, shame on you.");
				return false;
			}

			if (member.SpamSameMessageCount > 5)
			{
				await Client.Rest.ModifyGuildUserAsync(GuildID, message.Author.Id, u => u.WithTimeOutUntil(new(DateTime.Now.AddMinutes(30))));
				member.SpamSameMessageCount = 0;
			}
			else
			{
				await Client.Rest.SendMessageAsync(message.ChannelId, member.SpamSameMessageCount switch
				{
					3 => $"<@{message.Author.Id}> please avoid spamming.",
					4 => $"<@{message.Author.Id}> spamming interrupts others and the flow of chat, please avoid doing so.",
					5 => $"<@{message.Author.Id}> avoid spamming, final warning.",
					_ => $"<@{message.Author.Id}> please avoid overuse of # formatting."
				});
			}

			Members.MemberList[message.Author.Id] = member;
			return true;
		}
	}
}

using Bot.Interactions.Helpers;
using Bot.Interactions;

namespace Bot.Messages;

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
		MessageProperties StarMessage = new();
		RestMessage? RepliedTo = Message.ReferencedMessage;

		if (RepliedTo != null)
		{
			StarMessage.AddEmbeds(Client.Rest.GetMessageAsync(RepliedTo.ChannelId, RepliedTo.Id).Result.ToEmbedSet($"{GuildURL}{RepliedTo.ChannelId}/{RepliedTo.Id}")
			.First().WithAuthor(new()
			{
				IconUrl = RepliedTo.Author.GetAvatar(),
				Name = $"Replying to: {RepliedTo.Author.GetDisplayName()}"
			}));
		}

		StarMessage.AddEmbeds(Message.ToEmbedSet(
			$"{GuildURL}{message.ChannelId}/{message.MessageId}",
			new() { IconUrl = message.User!.GetAvatar(), Text = $"Message starred by {message.User!.GetDisplayName()}" },
			title: $"Starboard Entry #{await Files.GetCounter(Files.CounterLines.Starboard, 1)}"
		));

		// Starboard Channel
		await Client.Rest.SendMessageAsync(1133836713194696744, StarMessage);
	}

	/// <summary>
	/// Gets the relevant <see cref="Message"/> object from a Discord message URL.
	/// </summary>
	public static async Task<RestMessage?> GetMessageFromLink(string link)
	{
		if (link.Contains(GuildURL) && link.Length > (link.IndexOf(GuildURL) + GuildURL.Length))
		{
			link = link[(link.IndexOf(GuildURL) + GuildURL.Length)..];

			if (link.Contains(' ')) link = link.Remove(link.IndexOf(' '));
			if (link.Contains('\n')) link = link.Remove(link.IndexOf('\n'));

			if (ulong.TryParse(link.Remove(link.LastIndexOf('/')), out ulong Channel) && ulong.TryParse(link[(link.LastIndexOf('/') + 1)..], out ulong Message))
			{
				return await Client.Rest.GetMessageAsync(Channel, Message);
			}
		}

		return null;
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
			if ((LinkedMessage = await GetMessageFromLink(Scan)) != null && !ParsedLinks.Contains(LinkedMessage.Id))
			{
				EmbedProperties embed = LinkedMessage.ToEmbedSet().First().WithUrl($"{GuildURL}{message.ChannelId}/{message.Id}");

				embed = i == LinkCount
					? embed.WithFooter(new() { Text = $"Message linked by {message.Author.GetDisplayName()}", IconUrl = message.Author.GetAvatar() })
					: embed.WithTimestamp(null);

				await Client.Rest.SendMessageAsync(
					message.ChannelId,
					new()
					{
						Embeds = Embeds.CreateImageSet((embed.Url, LinkedMessage.Attachments.GetImageURLs()))
						.Prepend(LastAuthorID == LinkedMessage.Author.Id ? embed.WithAuthor(null) : embed)
					}
				);

				ParsedLinks[i - 1] = LinkedMessage.Id;
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
		Members.Member Member = Members.MemberList[message.Author.Id];

		ReadOnlySpan<char> Content = message.Content.AsSpan();

		if (Content.StartsWith("# "))
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

		if (Content == Member.SpamLastMessage)
		{
			// Double penalize link spam.
			if (Content.Contains("://", StringComparison.Ordinal)) Member.SpamSameMessageCount++;
			if (Member.SpamSameMessageCount++ > 1) return await FilterHit();
		}
		else
		{
			if (Member.SpamSameMessageCount != 0) Member.SpamSameMessageCount--;
			Member.SpamLastMessage = message.Content;
		}

		if (Content.Contains("lost", StringComparison.OrdinalIgnoreCase) && Content.Contains("game", StringComparison.OrdinalIgnoreCase))
		{
			return await FilterHit(true);
		}

		// If there are no attachments, save state and exit early.
		if (message.Attachments.Any())
		{
			if (message.Attachments[0].Size == Member.SpamLastAttachmentSize)
			{
				if (Member.SpamSameMessageCount++ > 1) return await FilterHit();
			}
			else
			{
				if (Member.SpamSameMessageCount != 0) Member.SpamSameMessageCount--;
				Member.SpamLastAttachmentSize = message.Attachments[0].Size;
			}
		}

		Members.MemberList[message.Author.Id] = Member;
		return false;

		async Task<bool> FilterHit(bool gameLost = false)
		{
			// This passes the message to the deleted message handler directly.
			// More information on this is in the handler's code.
			Events.DeletedSpamMessage = message;
			await Client.Rest.DeleteMessageAsync(message.ChannelId, message.Id);

			if (gameLost)
			{
				if ((ProbabilityStateMachine.Next() & 0b0110) == 0b0110) await Client.Rest.SendMessageAsync(message.ChannelId, $"<@{message.Author.Id}>, shame on you.");
				return false;
			}

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

			Members.MemberList[message.Author.Id] = Member;
			return true;
		}
	}
}

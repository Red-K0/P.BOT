﻿#region Boilerplate (DO NOT TOUCH)
#pragma warning disable CA2211

namespace P_BOT;
#endregion

/// <summary> Contains methods and variables used for basic message functionality and parsing. </summary>
public static class MessageFunctions
{
	#region Variables
	/// <summary> The ID of the last logged user. </summary>
	public static ulong? LastAuthor = 0;

	/// <summary> The number of messages starred by P.BOT. </summary>
	public static int StarCount = Convert.ToInt32(DiskData.ReadMemory(1, DiskData.Pages.Counter));
	#endregion

	/// <summary> Logs a given <paramref name="message"/> in the console, using <see cref="HighlightMessage(in Message, out string)"/>. </summary>
	public static void LogMessage(in Message message)
	{
		if (LastAuthor != message.Author.Id)
		{
			Console.WriteLine($"\n{message.CreatedAt} {message.Author,-22} - {message.Author.Username}");
		}

		HighlightMessage(message, out string Annotation);

		if (!string.IsNullOrWhiteSpace(message.Content))
		{
			Console.WriteLine($"{message.Content} {(string.IsNullOrWhiteSpace(Annotation) ? '\0' : "< ")}{Annotation}");
		}

		LastAuthor = message.Author.Id;

		Console.ForegroundColor = ConsoleColor.Gray;
	}

	private static void HighlightMessage(in Message message, out string Annotation)
	{
		Annotation = "";
		//Message Highlighting
		if (message.Content.Contains(SERVER_LINK))
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Annotation = "Discord Message Link";
		}
		if (message.Content.StartsWith('.'))
		{
			Console.ForegroundColor = Options.DnDTextModule ? ConsoleColor.Green : ConsoleColor.Red;
			Annotation = $"Call to DnDTextModule {(Options.DnDTextModule ? "(Processed)" : "(Ignored)")}";
		}
	}

	/// <summary> Parses a given <paramref name="message"/> to check for message links, and displays their content if possible. </summary>
	public static void ParseMessageLink(in Message message, in GatewayClient client)
	{
		//HACK | The '49's below could pose a compatibility issue in the future. If this breaks for no reason later, you know why.
		if (message.Author.Id != BOT_ID)
		{
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

				try { ChannelID = ulong.Parse(CurrentScan.Remove(CurrentScan.IndexOf('/'))); } catch (ArgumentOutOfRangeException) { return; }
				try { MessageID = ulong.Parse(CurrentScan[(CurrentScan.IndexOf('/') + 1)..]); } catch (ArgumentOutOfRangeException) { return; }
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
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static async void AddToStarBoard(MessageReactionAddEventArgs message)
	{
		RestMessage StarredMessage = await client.Rest.GetMessageAsync(message.ChannelId, message.MessageId).ConfigureAwait(true);
		MessageProperties msg_prop = new();
		if (StarredMessage.ReferencedMessage != null)
		{
			RestMessage Reply = client.Rest.GetMessageAsync(StarredMessage.ReferencedMessage.ChannelId, StarredMessage.ReferencedMessage.Id).Result;
			EmbedAuthorProperties Author = new()
			{
				Name = $"Replying to: {StarredMessage.ReferencedMessage.Author.Username}",
				IconUrl = StarredMessage.ReferencedMessage.Author.GetAvatarUrl().ToString()
			};

			_ = msg_prop.AddEmbeds(
				EmbedHelpers.ToEmbed(Reply,
				$"{SERVER_LINK}{StarredMessage.ReferencedMessage.ChannelId}/{StarredMessage.ReferencedMessage.Id}"
			).Embeds!.First().WithAuthor(Author));
		}
		_ = msg_prop.AddEmbeds(EmbedHelpers.ToEmbed(
			StarredMessage,
			$"{SERVER_LINK}{message.ChannelId}/{message.MessageId}",
			$"Message starred by {message.User!.Username}",
			message.User.GetAvatarUrl().ToString(),
			message.MessageId,
			$"Starboard Entry #{StarCount}"
		).Embeds!);
		StarCount++;

		DiskData.AppendMemory(DiskData.Pages.StarredMessageList, message.MessageId.ToString());
		DiskData.WriteMemory(1, DiskData.Pages.Counter, StarCount.ToString());

		if (msg_prop.Embeds!.Count() > 10)
		{
			msg_prop.Embeds = msg_prop.Embeds!.Take(10);
			msg_prop.Content = "# " + new string('⭐', Math.Clamp(StarredMessage.Attachments.Count, 1, 10)) + "\n(Some attachments were trimmed due to the 10 attachment limit)";
		}
		else
		{
			msg_prop.Content = "# " + new string('⭐', Math.Clamp(StarredMessage.Attachments.Count, 1, 10));
		}
		_ = await client.Rest.SendMessageAsync(SERVER_STARBOARD, msg_prop).ConfigureAwait(true);
	}

	/// <summary> Writes the string <paramref name="content"/> to the console in the color specified by <paramref name="color"/>. </summary>
	public static void WriteColor(string content, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.Write(content);
		Console.ForegroundColor = ConsoleColor.Gray;
	}
}
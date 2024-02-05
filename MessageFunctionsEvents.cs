namespace P_BOT;
using static MessageFunctions;

/// <summary> Contains methods responsible for handling message events in the console. </summary>
public static class MessageLogging
{
	#region Constants
	private const ulong AUTHORID_SYSTEM = 0;
	private const ulong AUTHORID_CLIENT = 1;
	#endregion

	/// <summary>  </summary>
	public static async ValueTask System(LogMessage message)
	{
		if (LastAuthor != AUTHORID_SYSTEM)
		{
			Console.Write('\n');
		}

		WriteColor("System: ", ConsoleColor.Green);
		Console.WriteLine(message);

		LastAuthor = AUTHORID_SYSTEM;
		await Task.CompletedTask.ConfigureAwait(true);
	}

	/// <summary>  </summary>
	public static async ValueTask Delete(MessageDeleteEventArgs message)
	{
		if (LastAuthor != AUTHORID_CLIENT)
		{
			Console.Write('\n');
		}

		WriteColor("Client: ", ConsoleColor.Red);
		Console.WriteLine($"The message with ID '{message.MessageId}' was deleted.");

		LastAuthor = AUTHORID_CLIENT;
		await Task.CompletedTask.ConfigureAwait(true);
	}

	/// <summary>  </summary>
	public static async ValueTask Update(Message message)
	{
		if (LastAuthor != AUTHORID_CLIENT)
		{
			Console.Write('\n');
		}

		WriteColor("Client: ", ConsoleColor.Red);
		Console.WriteLine($"The message with ID '{message.Id}' was updated by {message.Author.Username} with the new content:");
		WriteColor($"{message.Content}\n", ConsoleColor.DarkGray);
		
		LastAuthor = AUTHORID_CLIENT;
		await Task.CompletedTask.ConfigureAwait(true);
	}

	/// <summary>  </summary>
	public static async ValueTask Create(Message message)
	{
		// Core Message Parsing
		if (!message.Author.IsBot)
		{
			MessageFunctions.LogMessage(message);
		}

		if (message.Content.Contains(@"https://discord.com/channels/") && message.Author.Id != 1169031557848252516)
		{
			MessageFunctions.ParseMessageLink(message);
		}

		// Modular Message Parsing
		if (Options.DnDTextModule && message.Content.StartsWith('.'))
		{
			RollMath.LogicSelect(message);
		}

		await ValueTask.CompletedTask.ConfigureAwait(true);
	}

	/// <summary>  </summary>
	public static async ValueTask ReactAdd(MessageReactionAddEventArgs message)
	{
		// Starboard Handling
		if (message.Emoji.Name == "⭐" && (await File.ReadAllLinesAsync(MEMORY_STARRED_MESSAGES).ConfigureAwait(false)).Contains(message.MessageId.ToString()))
		{
			MessageFunctions.AddToStarBoard(message);
		}
		await ValueTask.CompletedTask.ConfigureAwait(true);
	}
}
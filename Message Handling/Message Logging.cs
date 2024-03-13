namespace P_BOT.Messages;

/// <summary>
/// Contains methods responsible for logging messages to the console.
/// </summary>
internal static class Logging
{
	/// <summary>
	/// The ID of the last logged user.
	/// </summary>
	private static ulong? LastAuthor = 0;

	/// <summary>
	/// Logs messages from the system.
	/// </summary>
	public static async ValueTask Log(LogMessage message)
	{
		const string EMPTY_TRACE = "\"at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector)\"";
		const string EMPTY_ERROR = "Value cannot be null.";

		// Prevents partial message exceptions in the log
		if (message.Exception is { StackTrace: string stackTrace   } &&  (stackTrace.Contains(EMPTY_TRACE) || stackTrace.Contains(EMPTY_ERROR))) return;
		if (message.Exception is {    Message: string errorMessage } && errorMessage.Contains(EMPTY_ERROR)) return;

		AsNetwork(ref message);
		await Task.CompletedTask;
	}

	/// <summary> Logs a given <paramref name="message"/> to the console. </summary>
	/// <param name="message"> The <see cref="Message"/> object to log. </param>
	public static void AddMessage(in Message message)
	{
		if (!string.IsNullOrWhiteSpace(message.Content))
		{
			if (LastAuthor != message.Author.Id)
			{
				Console.WriteLine($"\n{message.CreatedAt} {message.Author,-22} - {message.Author.Username}");
			}

			string Annotation = GetAnnotation(message.Content);
			Console.WriteLine($"{message.Content} {(string.IsNullOrWhiteSpace(Annotation) ? "" : "< ")}{Annotation}");
			Console.ForegroundColor = ConsoleColor.Gray;
			LastAuthor = message.Author.Id;
		}

		static string GetAnnotation(string content)
		{
			if (content.StartsWith(".r"))
			{
				Console.ForegroundColor = Command_Processing.Helpers.Options.DnDTextModule ? ConsoleColor.Green : ConsoleColor.Red;
				return $"Call to DnDTextModule {(Command_Processing.Helpers.Options.DnDTextModule ? "(Processed)" : "(Ignored)")}";
			}

			return "";
		}
	}

	#region Special ID Writes
	private const ulong NETWORK_ID = 0, DISCORD_ID = 1, VERBOSE_ID = 2;
	private static void AsNetwork(ref LogMessage message)
	{
		if (LastAuthor != NETWORK_ID)
		{
			Console.Write('\n');
		}

		WriteColor("Network: ", ConsoleColor.Green);
		Console.WriteLine(message.Message);

		LastAuthor = NETWORK_ID;
	}
	public  static void AsDiscord(string message)
	{
		if (LastAuthor != DISCORD_ID)
		{
			Console.Write('\n');
		}
		WriteColor("Discord: ", ConsoleColor.Blue);
		Console.WriteLine(message);

		LastAuthor = DISCORD_ID;
	}
	public  static void AsVerbose(string message)
	{
		if (LastAuthor != VERBOSE_ID)
		{
			Console.Write('\n');
		}
		WriteColor("Command: ", ConsoleColor.Red);
		Console.WriteLine(message);

		LastAuthor = VERBOSE_ID;
	}

	private static void WriteColor(string content, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.Write(content);
		Console.ForegroundColor = ConsoleColor.Gray;
	}
	#endregion
}
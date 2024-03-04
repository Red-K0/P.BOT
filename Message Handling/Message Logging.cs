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
		if (message.Exception is { StackTrace: string stackTrace } && (stackTrace.Contains(EMPTY_TRACE) || stackTrace.Contains(EMPTY_ERROR)))
		{
			return;
		}
		AsSystem(ref message);
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
	private const ulong SYSTEM_ID = 0, CLIENT_ID = 1;
	private static void AsSystem(ref LogMessage message)
	{
		if (LastAuthor != SYSTEM_ID)
		{
			Console.Write('\n');
		}

		WriteColor("System: ", ConsoleColor.Green);
		Console.WriteLine(message.Message);

		LastAuthor = SYSTEM_ID;
	}
	public  static void AsClient(string message)
	{
		if (LastAuthor != CLIENT_ID)
		{
			Console.Write('\n');
		}
		WriteColor("Client: ", ConsoleColor.Red);
		Console.WriteLine(message);

		LastAuthor = CLIENT_ID;
	}
	private static void WriteColor(string content, ConsoleColor color)
	{
		Console.ForegroundColor = color;
		Console.Write(content);
		Console.ForegroundColor = ConsoleColor.Gray;
	}
	#endregion
}
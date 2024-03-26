using P_BOT.Command_Processing.Helpers;
namespace P_BOT.Messages;

/// <summary>
/// Contains methods responsible for logging messages to the console.
/// </summary>
internal static class Logging
{
	/// <summary>
	/// The ID of the latest logged user.
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
			string LogMessage = message.Content;

			if (LastAuthor != message.Author.Id)
			{
				Console.WriteLine($"\n{message.CreatedAt} {message.Author,-22} - {message.Author.Username}");
			}

			while (LogMessage.Contains(" http") || LogMessage.StartsWith("http"))
			{
				int LinkStart = LogMessage.IndexOf(" http") + 1;
				int LinkEnd = LogMessage.IndexOf(' ', LinkStart);

				if (LinkEnd == -1) LinkEnd = LogMessage.Length + 5;

				LogMessage = LogMessage.Insert(LinkStart, PBOT_C.BrightBlue).Insert(LinkEnd, PBOT_C.None);
			}

			GetAnnotation(ref LogMessage);
			Console.WriteLine(LogMessage);
			LastAuthor = message.Author.Id;
		}

		static void GetAnnotation(ref string content) => content += content.StartsWith(".r") ?
			$" < {(Options.DnDTextModule ? PBOT_C.Green : PBOT_C.Red)}Call to DnDTextModule {(Options.DnDTextModule ? "(Processed)" : "(Ignored)")}{PBOT_C.None}"
			: "";
	}

	#region Special ID Writes
	private const ulong NETWORK_ID = 0, DISCORD_ID = 1, VERBOSE_ID = 2;

	/// <summary>
	/// Logs network client responses.
	/// </summary>
	/// <param name="message"> The text to log. </param>
	private static void AsNetwork(ref LogMessage message)
	{
		if (LastAuthor != NETWORK_ID)
		{
			Console.Write('\n');
		}
		Console.WriteLine($"{PBOT_C.Green}Network:{PBOT_C.None} {message.Message}");
		LastAuthor = NETWORK_ID;
	}

	/// <summary>
	/// Logs discord responses.
	/// </summary>
	/// <param name="message"> The text to log. </param>
	public static void AsDiscord(string message)
	{
		if (LastAuthor != DISCORD_ID)
		{
			Console.Write('\n');
		}
		Console.WriteLine($"{PBOT_C.Blue}Discord:{PBOT_C.None} {message}");
		LastAuthor = DISCORD_ID;
	}

	/// <summary>
	/// Logs verbose output.
	/// </summary>
	/// <param name="message"> The text to log. </param>
	public  static void AsVerbose(string message)
	{
		if (LastAuthor != VERBOSE_ID)
		{
			Console.Write('\n');
		}
		Console.WriteLine($"{PBOT_C.Red}Command:{PBOT_C.None} {message}");
		LastAuthor = VERBOSE_ID;
	}
	#endregion
}
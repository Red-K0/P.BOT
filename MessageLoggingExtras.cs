namespace P_BOT;
using static MessageFunctions;

public static partial class MessageLogging
{
	private static void LogAsSystem(ref LogMessage message) // ref used to avoid unnecessary overhead
	{
		if (LastAuthor != AUTHORID_SYSTEM)
		{
			Console.Write('\n');
		}

		WriteColor("System: ", ConsoleColor.Green);
		Console.WriteLine(message);

		LastAuthor = AUTHORID_SYSTEM;
	}

	private static void LogAsClient(string message)
	{
		if (LastAuthor != AUTHORID_CLIENT)
		{
			Console.Write('\n');
		}
		WriteColor("Client: ", ConsoleColor.Red);
		Console.WriteLine(message);

		LastAuthor = AUTHORID_CLIENT;
	}
}

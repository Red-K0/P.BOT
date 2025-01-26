using static Bot.Messages.Logging;

Console.OutputEncoding = System.Text.Encoding.Unicode;
Console.CursorVisible = false;
Ansi.EnableSequences();

AppDomain app = AppDomain.CurrentDomain;

app.ProcessExit += static (_, _) => Ansi.DisableSequences();

app.UnhandledException += static async (_, e) =>
{
	Console.Clear();
	WriteAsID($"An unhandled exception occurred at {DateTime.UtcNow}, with message: {((Exception)e.ExceptionObject).Message}", SpecialId.Network);
	await Restart();
};

await Start();

await Task.Delay(Timeout.Infinite);

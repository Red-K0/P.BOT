using static Bot.Messages.Logging;
Console.OutputEncoding = System.Text.Encoding.Unicode;
VirtualTerminalSequences.Enable();
Console.CursorVisible = false;

Bot.Documentation.Generator.Generate();
Files.EnsurePathsExist();

AppDomain current = AppDomain.CurrentDomain;

current.ProcessExit += static (_, _) => VirtualTerminalSequences.Disable();

current.UnhandledException += static async (_, e) =>
{
	Console.Clear();
	WriteAsID($"An unhandled '{e.ExceptionObject.GetType()}' occurred at {DateTime.UtcNow}", SpecialId.Network);
	await Restart();
};

await Start();

await Task.Delay(Timeout.Infinite);

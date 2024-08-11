using static PBot.Messages.Logging;

await Documentation.Generate();

VirtualTerminalSequences.Enable();
Console.OutputEncoding = System.Text.Encoding.Unicode;
Console.CursorVisible = false;

AppDomain current = AppDomain.CurrentDomain; // Load handler after client to prevent infinite loop.

current.ProcessExit += (_, _) => VirtualTerminalSequences.Disable(); // Restore input and output modes after exit.

current.UnhandledException += async (_, e) =>
{
	Console.Clear();
	WriteAsID($"An unhandled '{e.ExceptionObject.GetType()}' occurred at {DateTime.UtcNow}", SpecialId.Network);
	await Restart();
};

await Start();

await Task.Delay(Timeout.Infinite);

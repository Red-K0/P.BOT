using static PBot.Messages.Logging;

if (!PHelper.EnableVirtualAndHideCursor()) throw new OperationCanceledException("'phlpr.dll' failed to initlialize the console.");
Console.OutputEncoding = System.Text.Encoding.Unicode;

await Start();

AppDomain current = AppDomain.CurrentDomain; // Load handler after client and phlpr.dll to prevent infinite loop.
current.UnhandledException += async (_, e) =>
{
	Console.Clear();
	WriteAsID($"An unhandled '{e.ExceptionObject.GetType()}' occured at {DateTime.Now}", SpecialId.Network);
	await Restart();
};

await Task.Delay(Timeout.Infinite);
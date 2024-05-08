using static PBot.Messages.Logging;

if (!PHelper.EnableVirtualAndHideCursor()) throw new OperationCanceledException("VT_Sequences (phlpr.dll) failed to initlialize the console.");
Console.OutputEncoding = System.Text.Encoding.Unicode;

Start();

AppDomain current = AppDomain.CurrentDomain; // Load handler after client and phlpr.dll to prevent infinite loop.
current.UnhandledException += async (object sender, UnhandledExceptionEventArgs e) =>
{
	Console.Clear();
	WriteAsID($"An unhandled '{e.ExceptionObject.GetType()}' occured at {DateTime.Now.ToLongTimeString()} on {DateTime.Now.ToLongDateString()}", SpecialId.Network);
	await Restart();
};

await Task.Delay(Timeout.Infinite);
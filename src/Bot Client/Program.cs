using static PBot.Messages.Logging;

if (!PHelper.EnableVirtualAndHideCursor()) throw new OperationCanceledException("VT_Sequences (phlpr.dll) failed to initlialize the console.");

AppDomain current = AppDomain.CurrentDomain; // Load handler after phlpr.dll to prevent infinite loop.
current.UnhandledException += async (object sender, UnhandledExceptionEventArgs e) =>
{
	Console.Clear();
	WriteAsID($"An unhandled '{e.ExceptionObject.GetType()}' occured at {DateTime.Now.ToLongTimeString()} on {DateTime.Now.ToLongDateString()}", SpecialId.Network);
	await Restart();
};

Console.OutputEncoding = System.Text.Encoding.Unicode;

Start();

await Task.Delay(Timeout.Infinite);
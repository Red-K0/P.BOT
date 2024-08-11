using static Windows.Win32.System.Console.CONSOLE_MODE;
using static Windows.Win32.System.Console.STD_HANDLE;
using System.Runtime.InteropServices;
using static Windows.Win32.PInvoke;
using Windows.Win32.System.Console;

namespace PBot;

/// <summary>
/// Contains constants and method calls related to unmanaged code.
/// </summary>
internal static class VirtualTerminalSequences
{
	static readonly SafeHandle OutHandle = GetStdHandle_SafeHandle(STD_OUTPUT_HANDLE);
	static readonly SafeHandle InHandle  = GetStdHandle_SafeHandle(STD_INPUT_HANDLE);

	#region Control Codes
	/// <summary>
	/// Sets the console foreground color.
	/// </summary>
	public const string
	DarkGray = "\e[30m", Black       = "\e[90m",
	Red      = "\e[31m", DarkRed     = "\e[91m",
	Green    = "\e[32m", DarkGreen   = "\e[92m",
	Yellow   = "\e[33m", DarkYellow  = "\e[93m",
	Blue     = "\e[34m", DarkBlue    = "\e[94m",
	Magenta  = "\e[35m", DarkMagenta = "\e[95m",
	Cyan     = "\e[36m", DarkCyan    = "\e[96m",
	White    = "\e[37m", Gray        = "\e[97m";

	/// <summary>
	/// Sets the console background color.
	/// </summary>
	public const string
	DarkGrayB = "\e[40m", BlackB       = "\e[100m",
	RedB      = "\e[41m", DarkRedB     = "\e[101m",
	GreenB    = "\e[42m", DarkGreenB   = "\e[102m",
	YellowB   = "\e[43m", DarkYellowB  = "\e[103m",
	BlueB     = "\e[44m", DarkBlueB    = "\e[104m",
	MagentaB  = "\e[45m", DarkMagentaB = "\e[105m",
	CyanB     = "\e[46m", DarkCyanB    = "\e[106m",
	WhiteB    = "\e[47m", GrayB        = "\e[107m";
	#endregion

public static void Enable()
{
	GetConsoleMode(OutHandle, out CONSOLE_MODE OriginalOut);
	GetConsoleMode(InHandle,  out CONSOLE_MODE OriginalIn);
	SetConsoleMode(OutHandle, OriginalOut | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
	SetConsoleMode(InHandle,  OriginalIn  | ENABLE_VIRTUAL_TERMINAL_INPUT);
}

public static void Disable()
{
	GetConsoleMode(OutHandle, out CONSOLE_MODE OriginalOut);
	GetConsoleMode(InHandle,  out CONSOLE_MODE OriginalIn);
	SetConsoleMode(OutHandle, OriginalOut &~ ENABLE_VIRTUAL_TERMINAL_PROCESSING);
	SetConsoleMode(InHandle,  OriginalIn  &~ ENABLE_VIRTUAL_TERMINAL_INPUT);
}
}

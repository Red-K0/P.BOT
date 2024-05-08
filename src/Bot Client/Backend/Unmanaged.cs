using System.Runtime.InteropServices;

namespace PBot;

/// <summary>
/// Contains constants and method calls related to unmanaged code in 'phlpr.dll'.
/// </summary>
internal static unsafe partial class PHelper
{
	#region Control Codes
	/// <summary>
	/// Indicates the start of a virtual terminal sequence.
	/// </summary>
	public const string CSI = "\x1b[";

	/// <summary>
	/// Sets the console color.
	/// </summary>
	public const string
	BrightBlack    = CSI + "30m", Black    = CSI + "90m",
	BrightRed      = CSI + "31m", Red      = CSI + "91m",
	BrightGreen    = CSI + "32m", Green    = CSI + "92m",
	BrightYellow   = CSI + "33m", Yellow   = CSI + "93m",
	BrightBlue     = CSI + "34m", Blue     = CSI + "94m",
	BrightMagenta  = CSI + "35m", Magenta  = CSI + "95m",
	BrightCyan     = CSI + "36m", Cyan     = CSI + "96m",
	None           = CSI + "37m", White    = CSI + "97m",
	bBrightBlack   = CSI + "40m", bBlack   = CSI + "100m",
	bBrightRed     = CSI + "41m", bRed     = CSI + "101m",
	bBrightGreen   = CSI + "42m", bGreen   = CSI + "102m",
	bBrightYellow  = CSI + "43m", bYellow  = CSI + "103m",
	bBrightBlue    = CSI + "44m", bBlue    = CSI + "104m",
	bBrightMagenta = CSI + "45m", bMagenta = CSI + "105m",
	bBrightCyan    = CSI + "46m", bCyan    = CSI + "106m",
	bBrightWhite   = CSI + "47m", bWhite   = CSI + "107m";
	#endregion

	/// <summary> Unmanaged method imported via 'phlpr.dll', enables the use of virtual terminal sequences globally, and hides the cursor. </summary>
	/// <returns> True if successful, otherwise returns false. </returns>
	[LibraryImport("phlpr.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool EnableVirtualAndHideCursor();
}

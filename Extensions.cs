using System.Runtime.InteropServices;
namespace P_BOT;

/// <summary>
/// Convenience class for simple type conversions.
/// </summary>
internal static class TypeExtensions
{
	/// <summary>
	/// Converts the value of this instance to its equivalent <see cref="InteractionMessageProperties"/> representation.
	/// </summary>
	public static InteractionMessageProperties ToInteraction(this MessageProperties Obj) => new()
	{
		AllowedMentions = Obj.AllowedMentions,
		Attachments = Obj.Attachments,
		Components = Obj.Components,
		Content = Obj.Content,
		Embeds = Obj.Embeds,
		Flags = Obj.Flags,
		Tts = Obj.Tts
	};

	public static string GetAvatar(this User user) => user.HasAvatar ? user.GetAvatarUrl().ToString() : user.DefaultAvatarUrl.ToString();

	public static string ToParsedUnicode(this string Unparsed)
	{
		if (Unparsed == null) return "";
		char[] CharArray = Unparsed.ToCharArray();

		// Fix for escape characters in raw text, such as "\u2014" instead of '—' (U+2014 | Em Dash).
		for (int i = 0; i < CharArray.Length - 1; i++)
		{
			// If a raw text unicode identifier is present (\u****).
			if (CharArray[i] == '\\' && CharArray[i + 1] == 'u')
			{
				// Get the chars present after "\u", and merge them into one identifier.
				string Identifier = CharArray[i + 2].ToString()
				                  + CharArray[i + 3].ToString()
				                  + CharArray[i + 4].ToString()
				                  + CharArray[i + 5].ToString();

				// Convert the Identifier into a char value, and replace the '\' w
				// ith it.
				CharArray[i] = Convert.ToChar(int.Parse(Identifier, System.Globalization.NumberStyles.HexNumber));

				// Replace the 'u' and identifier characters with null '\0' characters.
				CharArray[i + 1] = '\0';
				CharArray[i + 2] = '\0';
				CharArray[i + 3] = '\0';
				CharArray[i + 4] = '\0';
				CharArray[i + 5] = '\0';
			}
		}

		Unparsed = new(CharArray);
		return Unparsed.Replace("\0", "");
	}

	public static string ToEscapedMarkdown(this string Unparsed) =>
		Unparsed.Replace("\\", "\\\\").Replace("_", "\\_").Replace("-", "\\-").Replace("*", "\\*").Replace("~", "\\~");
}

/// <summary>
/// Contains constants and method calls related to unmanaged code in 'PBOT_C.dll'.
/// </summary>
internal static partial class PBOT_C
{
	/// <summary> Unmanaged method imported via 'PBOT_C.dll', enables the use of virtual terminal sequences globally. </summary>
	/// <returns> True if successful, otherwise returns false. </returns>
	[LibraryImport("PBOT_C.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool EnableVirtual();

	#region Control Codes
	public const string SCI = "\x1b[";
	public const string
	BrightBlack   = SCI + "30m", Black   = SCI + "90m",
	BrightRed     = SCI + "31m", Red     = SCI + "91m",
	BrightGreen   = SCI + "32m", Green   = SCI + "92m",
	BrightYellow  = SCI + "33m", Yellow  = SCI + "93m",
	BrightBlue    = SCI + "34m", Blue    = SCI + "94m",
	BrightMagenta = SCI + "35m", Magenta = SCI + "95m",
	BrightCyan    = SCI + "36m", Cyan    = SCI + "96m",
	None          = SCI + "37m", White   = SCI + "97m",

	bBrightBlack   = SCI + "40m", bBlack   = SCI + "100m",
	bBrightRed     = SCI + "41m", bRed     = SCI + "101m",
	bBrightGreen   = SCI + "42m", bGreen   = SCI + "102m",
	bBrightYellow  = SCI + "43m", bYellow  = SCI + "103m",
	bBrightBlue    = SCI + "44m", bBlue    = SCI + "104m",
	bBrightMagenta = SCI + "45m", bMagenta = SCI + "105m",
	bBrightCyan    = SCI + "46m", bCyan    = SCI + "106m",
	bBrightWhite   = SCI + "47m", bWhite   = SCI + "107m";
	#endregion
}
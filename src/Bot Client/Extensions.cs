using System.Runtime.InteropServices;
namespace PBot;

/// <summary>
/// Convenience class for simple type conversions.
/// </summary>
internal static class TypeExtensions
{
	/// <summary>
	/// Converts the value of this instance to its equivalent <see cref="InteractionMessageProperties"/> representation.
	/// </summary>
	public static InteractionMessageProperties ToInteraction(this MessageProperties obj) => new()
	{
		AllowedMentions = obj.AllowedMentions,
		Attachments = obj.Attachments,
		Components = obj.Components,
		Content = obj.Content,
		Embeds = obj.Embeds,
		Flags = obj.Flags,
		Tts = obj.Tts
	};

	/// <summary>
	/// Converts a set of attachments to an array of the attachments' URLs.
	/// </summary>
	public static string?[]? GetImageURLs(this IReadOnlyDictionary<ulong, Attachment> attachments)
	{
		IEnumerable<string?> ImageURLs = [];
		foreach (KeyValuePair<ulong, Attachment> attachment in attachments) ImageURLs = ImageURLs.Append(attachment.Value.Url);
		return ImageURLs != null ? ImageURLs.Any() ? ImageURLs.ToArray() : null : null;
	}

	/// <summary>
	/// Attempts to get the channel's name, returning null if it doesn't have one.
	/// </summary>
	public static bool TryGetName(this TextChannel channel, out string? name)
	{
		if (channel is INamedChannel Channel)
		{
			name = Channel.Name;
			return true;
		}
		name = null;
		return false;
	}

	/// <summary>
	/// Caps the embed count of this instance to 10 embeds maximum.
	/// </summary>
	public static MessageProperties ToChecked(this MessageProperties obj)
	{
		if (obj.Embeds!.Count() > 10) obj.Embeds = obj.Embeds!.Take(10);
		obj.Embeds = obj.Embeds!.Where(i => i != null);
		return obj;
	}

	/// <summary>
	/// Gets the URL of a user's avatar if they have one, otherwise returning their default avatar URL.
	/// </summary>
	public static string GetAvatar(this User user) => user.HasAvatar ? user.GetAvatarUrl().ToString() : user.DefaultAvatarUrl.ToString();

	/// <summary>
	/// Gets the URL of a user's avatar if they have one, otherwise returning their default avatar URL.
	/// </summary>
	public static string GetAvatar(this GuildUser user) => user.HasAvatar ? user.GetAvatarUrl().ToString() : user.DefaultAvatarUrl.ToString();

	/// <summary>
	/// Replaces any unparsed unicode identifiers with their appropriate symbols (i.e. <c>\u0041' -> 'A'</c>).
	/// </summary>
	public static string ToParsedUnicode(this string unparsed)
	{
		if (unparsed == null) return "";
		char[] CharArray = unparsed.ToCharArray();

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

		unparsed = new(CharArray);
		return unparsed.Replace("\0", "");
	}

	/// <summary>
	/// Converts the contents of this string into an escaped version, avoiding markdown formatting issues.
	/// </summary>
	public static string ToEscapedMarkdown(this string unparsed) =>
			unparsed.Replace("\\", "\\\\").Replace("_", "\\_").Replace("-", "\\-").Replace("*", "\\*").Replace("~", "\\~");
}

/// <summary>
/// Contains constants and method calls related to unmanaged code in 'phlpr.dll'.
/// </summary>
internal static partial class PHelper
{
	/// <summary> Unmanaged method imported via 'phlpr.dll', enables the use of virtual terminal sequences globally. </summary>
	/// <returns> True if successful, otherwise returns false. </returns>
	[LibraryImport("phlpr.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool EnableVirtual();

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
}

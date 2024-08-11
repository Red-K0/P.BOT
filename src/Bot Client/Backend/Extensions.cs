using System.Text.RegularExpressions;
using System.Text;

namespace PBot;

/// <summary>
/// Convenience class for simple type conversions.
/// </summary>
internal static partial class Extensions
{
	/// <summary>
	/// Converts the value of this instance to its equivalent <see cref="InteractionMessageProperties"/> representation.
	/// </summary>
	public static MessageProperties ToMessage(this InteractionMessageProperties obj) => new()
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
		return ImageURLs.Any() ? ImageURLs.ToArray() : null;
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
	public static InteractionMessageProperties ToChecked(this InteractionMessageProperties obj)
	{
		if (obj.Embeds!.Count() > 10) obj.Embeds = obj.Embeds!.Take(10);
		obj.Embeds = obj.Embeds!.Where(i => i != null);
		return obj;
	}

	/// <summary>
	/// Gets the URL of a user's avatar if they have one, otherwise returning their default avatar URL.
	/// </summary>
	public static string GetAvatar(this User user, ImageFormat? format = null)
	{
		if (Caches.Members.List.TryGetValue(user.Id, out Caches.Members.Member? member) && member.Info.User.HasGuildAvatar)
		{
			return member.Info.User.GetGuildAvatarUrl(format)!.ToString();
		}
		else if (user.HasAvatar)
		{
			return user.GetAvatarUrl(format)!.ToString();
		}
		else
		{
			return user.DefaultAvatarUrl.ToString();
		}
	}

	/// <summary>
	/// Gets the URL of a user's avatar if they have one, otherwise returning their default avatar URL.
	/// </summary>
	public static string GetAvatar(this GuildUser user, ImageFormat? format = null)
	{
		if (user.HasGuildAvatar) return user.GetGuildAvatarUrl(format)!.ToString();
		else if (user.HasAvatar) return user.GetAvatarUrl(format)!.ToString();
		else return user.DefaultAvatarUrl.ToString();
	}

	/// <summary>
	/// Replaces any unparsed unicode identifiers with their appropriate symbols (i.e. <c>\u0041' -> 'A'</c>).
	/// </summary>
	public static string ToParsedUnicode(this string unparsed)
	{
		if (!unparsed.Contains('\\')) return unparsed;

		int Position = 0;
		StringBuilder Result = new();
		foreach (Match match in UnicodeIdentifierRegex().Matches(unparsed))
		{
			Result.Append(unparsed, Position, match.Index - Position);
			Position = match.Index + match.Length;
			Result.Append((char)Convert.ToInt32(match.Groups[1].ToString(), 16));
		}
		Result.Append(unparsed, Position, unparsed.Length - Position);

		unparsed = Result.ToString();
		Result.Clear();
		Position = 0;

		foreach (Match match in EscapeCharacterRegex().Matches(unparsed))
		{
			Result.Append(unparsed, Position, match.Index - Position);
			Position = match.Index + match.Length;
			Result.Append(match.ValueSpan[1] switch
			{
				 'b' => "\b",
				 'f' => "\f",
				 'n' => "\n",
				 'r' => "\r",
				 't' => "\t",
				 '"' => "\"",
				'\\' => "\\",
				_ => throw new InvalidOperationException()
			});
		}

		return Result.ToString();
	}

	/// <summary>
	/// Converts the contents of this string into an escaped version, avoiding markdown formatting issues.
	/// </summary>
	public static string ToEscapedMarkdown(this string unparsed) =>
			unparsed.Replace("\\", "\\\\").Replace("_", "\\_").Replace("-", "\\-").Replace("*", "\\*").Replace("~", "\\~");

	/// <summary>
	/// Gets the relevant <see cref="Message"/> object from a Discord message URL.
	/// </summary>
	public static async Task<RestMessage?> GetMessage(this string link)
	{
		if (link.Contains(GuildURL) && link.Length > (link.IndexOf(GuildURL) + GuildURL.Length))
		{
			link = link[(link.IndexOf(GuildURL) + GuildURL.Length)..];

			if (link.Contains(' ')) link = link.Remove(link.IndexOf(' '));
			if (link.Contains('\n')) link = link.Remove(link.IndexOf('\n'));

			if (ulong.TryParse(link.Remove(link.LastIndexOf('/')), out ulong Channel) && ulong.TryParse(link[(link.LastIndexOf('/') + 1)..], out ulong Message))
			{
				return await Client.Rest.GetMessageAsync(Channel, Message);
			}
		}

		return null;
	}

	/// <summary>
	/// Gets a <see cref="User"/>'s displayed discord name using the cached member list.
	/// </summary>
	public static string GetDisplayName(this User user) =>
		Caches.Members.List.TryGetValue(user.Id, out Caches.Members.Member? member) ? member.DisplayName : user.GlobalName ?? user.Username;

	[GeneratedRegex(@"\\u([0-9A-Fa-f]{4})")]
	private static partial Regex UnicodeIdentifierRegex();

	[GeneratedRegex(@"\\(b|f|n|r|t|""|\\)")]
	private static partial Regex EscapeCharacterRegex();
}
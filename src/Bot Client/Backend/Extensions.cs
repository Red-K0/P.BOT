namespace PBot;

/// <summary>
/// Convenience class for simple type conversions.
/// </summary>
internal static class Extensions
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
	/// Converts a <see cref="User"/> object to a <see cref="GuildUser"/> compatible object. The values appeneded to the object are always set to their defaults.
	/// </summary>
	public static GuildUser ToGuildUser(this User user) => new(new()
	{
		Deafened = default,
		GuildAvatarHash = default,
		GuildBoostStart = default,
		GuildFlags = default,
		HoistedRoleId = default,
		IsPending = default,
		JoinedAt = default,
		Muted = default,
		Nickname = default,
		Permissions = default,
		RoleIds = [],
		TimeOutUntil = default,
		User = new()
		{
			AccentColor = user.AccentColor,
			AvatarDecorationHash = user.AvatarDecorationHash,
			AvatarHash = user.AvatarHash,
			BannerHash = user.BannerHash,
			Discriminator = user.Discriminator,
			Email = user.Email,
			MfaEnabled = user.MfaEnabled,
			Flags = user.Flags,
			GlobalName = user.GlobalName,
			GuildUser = null,
			Id = user.Id,
			IsBot = user.IsBot,
			IsSystemUser = user.IsSystemUser,
			Locale = user.Locale,
			PremiumType = user.PremiumType,
			PublicFlags = user.PublicFlags,
			Username = user.Username,
			Verified = user.Verified,
		}
	}, GuildID, Client.Rest);

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
	public static string GetAvatar(this User user, ImageFormat? format = null) => user.HasAvatar ? user.GetAvatarUrl(format).ToString() : user.DefaultAvatarUrl.ToString();

	/// <summary>
	/// Gets the URL of a user's avatar if they have one, otherwise returning their default avatar URL.
	/// </summary>
	public static string GetAvatar(this GuildUser user, ImageFormat? format = null) => user.HasAvatar ? user.GetAvatarUrl(format).ToString() : user.DefaultAvatarUrl.ToString();

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

		return new string(CharArray).Replace("\0", "");
	}

	/// <summary>
	/// Converts the contents of this string into an escaped version, avoiding markdown formatting issues.
	/// </summary>
	public static string ToEscapedMarkdown(this string unparsed) =>
			unparsed.Replace("\\", "\\\\").Replace("_", "\\_").Replace("-", "\\-").Replace("*", "\\*").Replace("~", "\\~");

	/// <summary>
	/// Gets the relevant <see cref="Message"/> object from a Discord message URL.
	/// </summary>
	public static async Task<RestMessage?> GetMessage(this string messageLink)
	{
		if (!messageLink.Contains(GuildURL) || messageLink.Length <= (messageLink.IndexOf(GuildURL) + 49)) return null;
		string CurrentScan = messageLink[(messageLink.IndexOf(GuildURL) + 49)..];

		if (CurrentScan.Contains(' '))  CurrentScan = CurrentScan.Remove(CurrentScan.IndexOf(' '));
		if (CurrentScan.Contains('\n')) CurrentScan = CurrentScan.Remove(CurrentScan.IndexOf('\n'));

		if (!ulong.TryParse(CurrentScan.Remove(CurrentScan.LastIndexOf('/')),  out ulong ChannelID)) return null;
		if (!ulong.TryParse(CurrentScan[(CurrentScan.LastIndexOf('/') + 1)..], out ulong MessageID)) return null;

		return await Client.Rest.GetMessageAsync(ChannelID, MessageID);
	}

	/// <summary>
	/// Gets a <see cref="User"/>'s displayed discord name using the cached member list.
	/// </summary>
	public static string GetDisplayName(this User user) =>
		Caches.Members.List.TryGetValue(user.Id, out Caches.Members.Member? member) ? member.DisplayName : user.GlobalName ?? user.Username;
}

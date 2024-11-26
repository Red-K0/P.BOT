using static Bot.Backend.Members;
namespace Bot.Backend;

/// <summary>
/// Convenience class for simple type conversions.
/// </summary>
internal static class Extensions
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
	public static IEnumerable<string?> GetImageURLs(this IReadOnlyList<Attachment> attachments)
	{
		IEnumerable<string?> ImageURLs = [];
		foreach (Attachment attachment in attachments) ImageURLs = ImageURLs.Append(attachment.Url);
		return ImageURLs;
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
	public static string GetAvatar(this User user, ImageFormat? format = null) => MemberList.TryGetValue(user.Id, out Member? member) && member.Info.User.HasGuildAvatar
			? member.Info.User.GetGuildAvatarUrl(format)!.ToString()
			: user.HasAvatar ? user.GetAvatarUrl(format)!.ToString() : user.DefaultAvatarUrl.ToString();

	/// <summary>
	/// Gets the URL of a user's avatar if they have one, otherwise returning their default avatar URL.
	/// </summary>
	public static string GetAvatar(this GuildUser user, ImageFormat? format = null) => user.HasGuildAvatar
			? user.GetGuildAvatarUrl(format)!.ToString()
			: user.HasAvatar ? user.GetAvatarUrl(format)!.ToString() : user.DefaultAvatarUrl.ToString();

	/// <summary>
	/// Gets a <see cref="User"/>'s displayed discord name using the cached member list.
	/// </summary>
	public static string GetDisplayName(this User user) => MemberList.TryGetValue(user.Id, out Member? member) ? member.DisplayName : user.GlobalName ?? user.Username;
}
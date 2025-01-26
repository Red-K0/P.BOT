using static Bot.Backend.Members;

namespace Bot.Backend;

/// <summary>
/// Convenience class for simple but repetitive operations.
/// </summary>
internal static class Extensions
{
	/// <summary>
	/// Returns an <see cref="IEnumerable{T}"/> of type <see cref="string"/>, representing the attachment set's URLs.
	/// </summary>
	/// <param name="attachments">The attachment set to enumerate.</param>
	public static IEnumerable<string> GetImageURLs(this IEnumerable<Attachment> attachments) => attachments.Select(static s => s.Url);

	/// <summary>
	/// Gets the name of the specified channel.
	/// </summary>
	/// <param name="channel">The channel to get the name of.</param>
	/// <param name="name">When this method returns, contains the name of the channel, if it has one; otherwise <see langword="null"/>. This parameter is passed uninitialized.</param>
	/// <returns><see langword="true"/> if the <see cref="TextChannel"/> has a name; otherwise, <see langword="false"/>.</returns>
	public static bool TryGetName(this TextChannel channel, [NotNullWhen(true)] out string? name)
	{
		if (channel is INamedChannel namedChannel)
		{
			name = namedChannel.Name;
			return true;
		}
		name = null;
		return false;
	}

	/// <summary>
	/// Returns a user's display avatar.
	/// </summary>
	public static string GetAvatar(this User user, ImageFormat? format = null) => (MemberList.TryGetValue(user.Id, out Member? member) && member.Info.User.HasGuildAvatar
			? member.Info.User.GetGuildAvatarUrl(format)!
			: user.HasAvatar
				? user.GetAvatarUrl(format)!
				: user.DefaultAvatarUrl).ToString();

	/// <inheritdoc cref="GetAvatar(User, ImageFormat?)"/>
	public static string GetAvatar(this GuildUser user, ImageFormat? format = null) => (user.HasGuildAvatar
			? user.GetGuildAvatarUrl(format)!
			: user.HasAvatar
				? user.GetAvatarUrl(format)!
				: user.DefaultAvatarUrl).ToString();

	/// <summary>
	/// Returns a user's display name.
	/// </summary>
	public static string GetDisplayName(this User user) => MemberList.TryGetValue(user.Id, out Member? member) ? member.DisplayName : user.GlobalName ?? user.Username;
}
namespace Bot.Backend;
internal static partial class Events
{
	/// <summary>
	/// Updates the <see cref="Members.MemberList"/> dictionary when a user is added or modified.
	/// </summary>
	public static async ValueTask GuildUserUpdate(GuildUser guildUser) => Members.MemberList[guildUser.Id] = new(await guildUser.GetInfoAsync());

	/// <summary>
	/// Removes users from the Caches.Content.List dictionary when a member is modified.
	/// </summary>
	public static async ValueTask GuildUserRemove(GuildUserRemoveEventArgs guildUser) => await Task.Run(() => Members.MemberList.Remove(guildUser.User.Id));
}
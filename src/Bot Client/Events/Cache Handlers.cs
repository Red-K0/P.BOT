namespace Bot.Backend;
internal static partial class Events
{
	/// <summary>
	/// Adds/updates the <see cref="Members.MemberList"/> when a user is added or modified.
	/// </summary>
	public static async ValueTask GuildUserUpdate(GuildUser guildUser) => Members.MemberList[guildUser.Id] = new(await guildUser.GetInfoAsync());

	/// <summary>
	/// Removes users from the <see cref="Members.MemberList"/> when a member leaves the guild.
	/// </summary>
	public static async ValueTask GuildUserRemove(GuildUserRemoveEventArgs guildUser) { Members.MemberList.Remove(guildUser.User.Id); await Task.CompletedTask; }
}
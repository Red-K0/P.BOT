using PBot.Caches;

namespace PBot;

internal static partial class Events
{
	// Users

	/// <summary>
	/// Updates the <see cref="Caches.Members.List"/> dictionary when a user is added or modified.
	/// </summary>
	public static async ValueTask GuildUserUpdate(GuildUser guildUser) => Members.List[guildUser.Id] = new(await guildUser.GetInfoAsync());

	/// <summary>
	/// Removes users from the Caches.Members.List dictionaryy when a member is modified.
	/// </summary>
	public static async ValueTask GuildUserRemove(GuildUserRemoveEventArgs guildUser) => await Task.Run(() => Members.List.Remove(guildUser.User.Id));

	// Roles

	/// <summary>
	/// Updates the <see cref="Caches.Members.Roles"/> dictionary when roles are created or updated.
	/// </summary>
	public static async ValueTask RoleUpdate(Role role) => await Task.Run(() => Members.Roles[role.Id] = role);

	/// <summary>
	/// Updates the <see cref="Caches.Members.Roles"/> dictionary when roles are deleted.
	/// </summary>
	public static async ValueTask RoleDelete(RoleDeleteEventArgs role) => await Task.Run(() => Members.Roles.Remove(role.RoleId));
}
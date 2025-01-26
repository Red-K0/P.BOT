namespace Bot.Data;

/// <summary>
/// Contains the bot's message cache and its relevant methods.
/// </summary>
internal static class Cache
{
	/// <summary>
	/// The index of the last assigned <see cref="MessageIgnoreList"/> slot.
	/// </summary>
	private static int _ignoreIndex;

	/// <summary>
	/// A short array of messages to be ignored by <see cref="Events.MessageDelete(MessageDeleteEventArgs)"/> to prevent clutter.
	/// </summary>
	public static readonly ulong[] MessageIgnoreList = [0, 0, 0, 0, 0];

	/// <summary>
	/// The message cache, indexed by message IDs.
	/// </summary>
	public static readonly Dictionary<ulong, Message> RecentMessages = new(capacity: 10000);

	/// <summary>
	/// Adds a message ID to the <see cref="MessageIgnoreList"/> list.
	/// </summary>
	public static void AddToIgnoreList(ulong id)
	{
		MessageIgnoreList[_ignoreIndex] = id;
		_ignoreIndex += _ignoreIndex == 4 ? -4 : 1;
	}
}

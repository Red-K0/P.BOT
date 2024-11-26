using BitFaster.Caching.Lru;
namespace Bot.Data;

/// <summary>
/// Contains the recent messages cache and its relevant methods.
/// </summary>
internal static class Cache
{
	/// <summary>
	/// Contains all messages in the bot's cache.
	/// </summary>
	public static ConcurrentLru<ulong, Message> RecentMessages { get; } = new(capacity: 10000);

	/// <summary>
	/// The index of the last assigned <see cref="IgnoredMessageIDs"/> slot.
	/// </summary>
	private static sbyte IgnoredMessageListPointer = -1;

	/// <summary>
	/// A short array of messages to be ignored by <see cref="Events.MessageDelete(MessageDeleteEventArgs)"/> to prevent clutter.
	/// </summary>
	public static readonly ulong[] IgnoredMessageIDs = [0, 0, 0, 0, 0];

	/// <summary>
	/// Adds a message ID to the <see cref="IgnoredMessageIDs"/>.
	/// </summary>
	public static void IgnoreMessageID(ulong id)
	{
		if (IgnoredMessageListPointer == 4) IgnoredMessageListPointer = -1;
		IgnoredMessageIDs[++IgnoredMessageListPointer] = id;
	}
}

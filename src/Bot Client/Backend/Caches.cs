namespace PBot.Caches;

/// <summary>
/// Contains the recent messages cache and its relevant methods.
/// </summary>
internal static class Messages
{
	/// <summary>
	/// The maximum number of entries in the <see cref="Recent"/> list.
	/// </summary>
	const int RECENT_CACHE_SIZE_MAX = 100;

	/// <summary>
	/// Contains the most recent messages in the bot's cache.
	/// </summary>
	public static LinkedList<KeyValuePair<ulong, Message>> Recent = [];

	/// <summary>
	/// Iterates through the <see cref="Recent"/> list in reverse order, returning the <see cref="Message"/> corresponding to the given <paramref name="ID"/> if it's found, returning <see langword="null"/> otherwise.
	/// </summary>
	public static Message? Get(ulong ID)
	{
		KeyValuePair<ulong, Message> item;
		for (int i = Recent.Count - 1; i > -1; i--)
		{
			item = Recent.ElementAt(i);
			if (item.Key == ID) return item.Value;
		}
		return null;
	}

	/// <summary>
	/// Adds the given <see cref="Message"/> to the <see cref="Recent"/> list, removing the oldest message if the list size is at <see cref="RECENT_CACHE_SIZE_MAX"/>.
	/// </summary>
	public static void Add(Message message)
	{
		if (Recent.Count > RECENT_CACHE_SIZE_MAX) Recent.RemoveFirst();
		Recent.AddLast(value: new(message.Id, message));
	}

	/// <summary>
	/// Updates a message in the cache if it's found, otherwise creating it as a new entry.
	/// </summary>
	public static void Edit(Message message)
	{
		KeyValuePair<ulong, Message> item;
		for (int i = Recent.Count - 1; i > 0; i--)
		{
			item = Recent.ElementAt(i);
			if (item.Key == message.Id)
			{
				Recent.Remove(item);
				Recent.AddLast(value: new(message.Id, message));
				return;
			}
		}
		Add(message);
	}
}

/// <summary>
/// Contains the cahced members list, role list, and their relevant methods.
/// </summary>
internal static partial class Members
{
	/// <summary>
	/// The bot's internal member list, generated from the <c>members-search</c> endpoint.
	/// </summary>
	public static Dictionary<ulong, Member> List = [];

	/// <summary>
	/// The bot's internal role list, generated with <see cref="RestClient.GetGuildRolesAsync(ulong, RequestProperties?)"/>.
	/// </summary>
	public static IReadOnlyDictionary<ulong, Role> Roles = client.Rest.GetGuildRolesAsync(1131100534250680433).Result;

	/// <summary>
	/// Initializes the class with data from the <c>members-search</c> endpoint.
	/// </summary>
	public static void Load()
	{
		IEnumerable<KeyValuePair<ulong, Member>> TempList = [];
		foreach (var item in client.Rest.SearchGuildUsersAsync(1131100534250680433).ToBlockingEnumerable()) TempList = TempList.Append(new KeyValuePair<ulong, Member>(item.User.Id, new(item)));
		List = TempList.Reverse().ToDictionary();
	}

	/// <summary>
	/// Checks if the role referenced by the ID is an event role.
	/// </summary>
	/// <param name="ID"> The ID to check. </param>
	public static bool IsAccolade(ulong ID) => Accolades.Contains(ID);
}
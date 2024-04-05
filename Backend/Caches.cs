namespace P_BOT.Caches;

internal static class Messages
{
	const int RECENT_CACHE_SIZE_MAX = 100;
	public static LinkedList<KeyValuePair<ulong, Message>> Recent = [];

	/// <summary>
	/// Adds the given <see cref="Message"/> to the <see cref="Recent"/> list, removing the oldest message if the list size is at <see cref="RECENT_CACHE_SIZE_MAX"/>.
	/// </summary>
	public static void Add(Message message)
	{
		if (Recent.Count > RECENT_CACHE_SIZE_MAX) Recent.RemoveFirst();
		Recent.AddLast(value: new(message.Id, message));
	}

	/// <summary>
	/// Iterates through the <see cref="Recent"/> list in reverse order, returning the <see cref="Message"/> corresponding to the given <paramref name="ID"/> if it's found, returning <see langword="null"/> otherwise.
	/// </summary>
	public static Message? Get(ulong ID)
	{
		KeyValuePair<ulong, Message> item;
		for (int i = Recent.Count - 1; i > 0; i--)
		{
			item = Recent.ElementAt(i);
			if (item.Key == ID) return item.Value;
		}
		return null;
	}
}

internal static partial class Members
{
	/// <summary>
	/// The bot's internal member list, generated from the <c>members-search</c> endpoint.
	/// </summary>
	public static Dictionary<ulong, Member> List = [];
	public static IReadOnlyDictionary<ulong, Role> Roles = client.Rest.GetGuildRolesAsync(1131100534250680433).Result;

	/// <summary>
	/// Initializes the class with data from the <c>members-search</c> endpoint.
	/// </summary>
	public static async void Load()
	{
		IEnumerable<KeyValuePair<ulong, Member>> TempList = [];
		await foreach (var item in client.Rest.SearchGuildUsersAsync(1131100534250680433)) TempList = TempList.Append(new KeyValuePair<ulong, Member>(item.User.Id, new(item)));
		List = TempList.Reverse().ToDictionary();
	}

	/// <summary>
	/// Checks if the role referenced by the ID is an event role.
	/// </summary>
	/// <param name="ID"> The ID to check. </param>
	public static bool IsAccolade(ulong ID) => Accolades.Contains(ID);
}
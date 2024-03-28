namespace P_BOT.Messages;

internal static class Cache
{
	const int RECENT_CACHE_SIZE_MAX = 100;
	public static LinkedList<KeyValuePair<ulong, Message>> Recent = [];

	public static void Add(Message message)
	{
		if (Recent.Count > RECENT_CACHE_SIZE_MAX) Recent.RemoveFirst();
		Recent.AddLast(value: new(message.Id, message));
	}
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

namespace P_BOT.Command_Processing.Helpers;

/// <summary> Contains methods and variables related to the post system. </summary>
internal static class PostFunctions
{
	/// <summary> Gets the ExternalID tied to the given <paramref name="InternalID"/>. </summary>
	public static ulong ToExternal(ulong InternalID) => Convert.ToUInt64(DataBackend.ReadMemory((int)InternalID, DataBackend.Pages.PostDatabaseIDList));
	/// <summary> Gets the InternalID tied to the given <paramref name="ExternalID"/>. </summary>
	public static ulong ToInternal(ulong ExternalID) => (ulong)Array.FindIndex(File.ReadAllLines(MEMORY_POSTDATABASE_IDLIST), s => s.Equals(ExternalID));

	/// <summary> Stores both the internal and external PostIDs side-by-side in the <see cref="DataBackend.Pages.PostDatabaseIDList"/> page. </summary>
	/// <param name="InternalPostID"> The internal ID of the post. </param>
	/// <returns> The external ID of the post (Discord Message ID). </returns>
	public static ulong StoreID(ulong InternalPostID)
	{
		// Reserve the ID.
		DataBackend.WriteMemory(3, DataBackend.Pages.Counter, InternalPostID.ToString());

		// Gets the message ID of the last sent message in the feed channel, almost always the post created by this method.
		RestMessage ExternalPostID = client.Rest.GetMessagesAsync(SERVER_POSTFEED, new() { Limit = 1 }).ToBlockingEnumerable().First();

		// Store the Internal ID and its relevant external ID in the ID List.
		DataBackend.AppendMemory(DataBackend.Pages.PostDatabaseIDList, $"{ExternalPostID}");

		return ExternalPostID.Id;
	}
}
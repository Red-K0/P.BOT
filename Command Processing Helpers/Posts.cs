// The helper class supporting the /post command.

namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains methods and variables related to the post system.
/// </summary>
public static class Posts
{
	/// <summary>
	/// The channel to send posts and other data to.
	/// </summary>
	public const ulong CHANNEL = 1208068312487960606;

	/// <summary>
	/// Gets the ExternalID tied to the given <paramref name="InternalID"/>.
	/// </summary>
	public static async Task<ulong> ToExternal(ulong InternalID) =>
		Convert.ToUInt64(await DataBackend.ReadMemory(DataBackend.Pages.PDB_ID, (int)InternalID));

	/// <summary>
	/// Gets the InternalID tied to the given <paramref name="ExternalID"/>.
	/// </summary>
	public static async Task<ulong> ToInternal(ulong ExternalID) =>
		(ulong)Array.FindIndex(await File.ReadAllLinesAsync(DataBackend.PDB_ID), s => s.Equals(ExternalID));

	/// <summary> Stores both the internal and external PostIDs side-by-side in the <see cref="DataBackend.Pages.PDB_ID"/> page. </summary>
	/// <param name="InternalPostID"> The internal ID of the post. </param>
	/// <returns> The external ID of the post (Discord Message ID). </returns>
	public static ulong StoreID(ulong InternalPostID)
	{
		// Reserve the ID.
		DataBackend.WriteMemory(DataBackend.Pages.Counters, 3, InternalPostID.ToString());

		// Gets the message ID of the last sent message in the feed channel, almost always the post created by this method.
		RestMessage ExternalPostID = client.Rest.GetMessagesAsync(CHANNEL, new() { Limit = 1 }).ToBlockingEnumerable().First();

		// Store the Internal ID and its relevant external ID in the ID List.
		DataBackend.AppendMemory(DataBackend.Pages.PDB_ID, $"{ExternalPostID}");

		return ExternalPostID.Id;
	}
}
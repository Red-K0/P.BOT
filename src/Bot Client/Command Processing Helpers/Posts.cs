// The helper class supporting the /post command.

namespace PBot.Command_Processing.Helpers;

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
	/// Gets the ExternalID tied to the given <paramref name="ID"/>.
	/// </summary>
	public static async Task<ulong> ToExternal(ulong ID) => Convert.ToUInt64(await Pages.Read(Pages.Files.PDB_ID, (int)ID));

	/// <summary>
	/// Gets the InternalID tied to the given <paramref name="ID"/>.
	/// </summary>
	public static async Task<ulong> ToInternal(ulong ID) => (ulong)Array.FindIndex(await File.ReadAllLinesAsync(Pages.PDB_ID), s => s.Equals(ID));

	/// <summary> Stores both the internal and external PostIDs side-by-side in the <see cref="Pages.Files.PDB_ID"/> page. </summary>
	/// <param name="internalPostID"> The internal ID of the post. </param>
	/// <returns> The external ID of the post (Discord Message ID). </returns>
	public static ulong StoreID(ulong internalPostID)
	{
		// Reserve the ID.
		Pages.Write(Pages.Files.Counters, 3, internalPostID.ToString());

		// Gets the message ID of the last sent message in the feed channel, almost always the post created by this method.
		RestMessage ExternalPostID = client.Rest.GetMessagesAsync(CHANNEL, new() { Limit = 1 }).ToBlockingEnumerable().First();

		// Store the Internal ID and its relevant external ID in the ID List.
		Pages.Append(Pages.Files.PDB_ID, ExternalPostID.ToString());

		return ExternalPostID.Id;
	}
}
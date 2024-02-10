namespace P_BOT;

/// <summary> Contains methods and constants used for persistent data storage. </summary>
internal static class DataBackend
{
	/// <summary> Reads the content at a specific <paramref name="Line"/>, from the specified <paramref name="Page"/>. </summary>
	/// <param name="Line"> The number of the line to read the contents of. </param>
	/// <param name="Page"> The memory page to read the <paramref name="Line"/> from. </param>
	public static string ReadMemory(int Line, Pages Page) => File.ReadAllLines(PageSwitch(Page))[Line];

	/// <summary> Appends the given <paramref name="NewLine"/> to the end of the <paramref name="Page"/> specified. </summary>
	/// <param name="Page"> The memory page to append the <paramref name="NewLine"/> to. </param>
	/// <param name="NewLine"> The <see cref="string"/> to append to the <paramref name="Page"/>. </param>
	public static void AppendMemory(Pages Page, string NewLine)
	{
		string Path = PageSwitch(Page);
		File.WriteAllLines(Path, [.. File.ReadAllLines(Path), NewLine]);
	}

	/// <summary> Overwrites the data at <paramref name="Line"/> with the given <paramref name="NewValue"/>, at the specified <paramref name="Page"/>. </summary>
	/// <param name="Line"> The number of the line to overwrite with the given <paramref name="NewValue"/>. </param>
	/// <param name="Page"> The memory page to modify. </param>
	/// <param name="NewValue"> The <see cref="string"/> to overwrite the given <paramref name="Line"/> with. </param>
	public static void WriteMemory(int Line, Pages Page, string NewValue)
	{
		string Path = PageSwitch(Page);
		string[] STR = [.. File.ReadAllLines(Path)];
		STR.SetValue(NewValue, Line);
		File.WriteAllLines(Path, STR);
	}

	/// <summary> Gets the path of a given <paramref name="Page"/>. </summary>
	/// <param name="Page"> The page to fetch the path to. </param>
	private static string PageSwitch(Pages Page)
	{
		return Page switch
		{
			Pages.Counter => MEMORY_COUNTERS,
			Pages.SecretSantaReceivers => MEMORY_SECRETSANTA_RECIEVER,
			Pages.SecretSantaSenders => MEMORY_SECRETSANTA_SENDER,
			Pages.StarredMessageList => MEMORY_STARRED_MESSAGES,
			Pages.PostDatabaseIDList => MEMORY_POSTDATABASE_IDLIST,
			_ => throw new ArgumentException("How did this happen?"),
		};
	}

	/// <summary> A list of memory pages used by P.BOT. </summary>
	public enum Pages
	{
		/// <summary> Contains data related to basic operation, such as incremental counters. </summary>
		Counter,
		/// <summary> Contains a list of names assigned to receive gifts. </summary>
		SecretSantaReceivers,
		/// <summary> Contains a list of names to receive gifts, as well as their senders. </summary>
		SecretSantaSenders,
		/// <summary> Contains a list of starred messages. </summary>
		StarredMessageList,
		/// <summary> Contains a list of info related to post IDs. </summary>
		PostDatabaseIDList
	}
}
namespace P_BOT;

/// <summary>
/// Contains methods and constants used for persistent data storage.
/// </summary>
internal static class DataBackend
{
	#region Path Constants
	/// <summary>
	/// The main path containing all the memory files..BOT.
	/// </summary>
	private const string MAIN = @"F:\!PBOT\~Data\";

	/// <summary>
	/// Contains data related to basic operation, such as incremental counters.
	/// </summary>
	public  const string COUNTERS = MAIN + "Counters.txt";

	/// <summary>
	/// Contains a list of starred messages.
	/// </summary>
	public  const string STARBOARD = MAIN + "Starred Message List.txt";

	/// <summary>
	/// Contains a list of info related to post IDs.
	/// </summary>
	public  const string PDB_ID = MAIN + @"PostDB\ID.txt";
	#endregion

	/// <summary> Reads the content at a specific <paramref name="Line"/>, from the specified <paramref name="Page"/>. </summary>
	/// <param name="Page"> The memory page to read the <paramref name="Line"/> from. </param>
	/// <param name="Line"> The number of the line to read the contents of. </param>
	public static async Task<string> ReadMemory(Pages Page, int Line) => (await File.ReadAllLinesAsync(PageSwitch(Page)))[Line];

	/// <summary> Appends the given <paramref name="NewLine"/> to the end of the <paramref name="Page"/> specified. </summary>
	/// <param name="Page"> The memory page to append the <paramref name="NewLine"/> to. </param>
	/// <param name="NewLine"> The <see cref="string"/> to append to the <paramref name="Page"/>. </param>
	public static async void AppendMemory(Pages Page, string NewLine)
	{
		string Path = PageSwitch(Page);
		await File.WriteAllLinesAsync(Path, [.. File.ReadAllLines(Path), NewLine]);
	}

	/// <summary> Overwrites the data at <paramref name="Line"/> with the given <paramref name="NewValue"/>, at the specified <paramref name="Page"/>. </summary>
	/// <param name="Page"> The memory page to modify. </param>
	/// <param name="Line"> The number of the line to overwrite with the given <paramref name="NewValue"/>. </param>
	/// <param name="NewValue"> The <see cref="string"/> to overwrite the given <paramref name="Line"/> with. </param>
	public static async void WriteMemory(Pages Page, int Line, string NewValue)
	{
		string Path = PageSwitch(Page);
		string[] STR = [.. File.ReadAllLines(Path)];
		STR.SetValue(NewValue, Line);
		await File.WriteAllLinesAsync(Path, STR);
	}

	/// <summary> Gets the path of a given <paramref name="Page"/>. </summary>
	/// <param name="Page"> The page to fetch the path to. </param>
	private static string PageSwitch(Pages Page)
	{
		return Page switch
		{
			Pages.Counters => COUNTERS,
			Pages.Starboard => STARBOARD,
			Pages.PDB_ID => PDB_ID,
			_ => throw new ArgumentException("You forgot to register the new memory file."),
		};
	}

	/// <summary>
	/// A list of memory pages used by P.BOT.
	/// </summary>
	public enum Pages
	{
		/// <summary> Contains data related to basic operation, such as incremental counters. </summary>
		Counters,
		/// <summary> Contains a list of starred messages. </summary>
		Starboard,
		/// <summary> Contains a list of info related to post IDs. </summary>
		PDB_ID
	}
}
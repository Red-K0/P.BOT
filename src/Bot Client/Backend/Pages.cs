namespace PBot;

/// <summary>
/// Contains methods and constants used for persistent data storage.
/// </summary>
internal static class Pages
{
	#region Path Constants
	/// <summary>
	/// The main path containing all the memory files used by P.BOT.
	/// </summary>
	private const string MAIN = @"F:\!PBOT\src\Bot Client\~Data\";

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

	/// <summary> Reads the content at a specific <paramref name="line"/>, from the specified <paramref name="page"/>. </summary>
	/// <param name="page"> The memory page to read the <paramref name="line"/> from. </param>
	/// <param name="line"> The number of the line to read the contents of. </param>
	public static async Task<string> Read(Files page, int line)
	{
#if DEBUG_DISK
		Messages.Logging.AsVerbose($"The value at line {Line} in page {Page} was read.");
#endif
		return (await File.ReadAllLinesAsync(Switch(page)))[line];
	}

	/// <summary> Appends the given <paramref name="newLine"/> to the end of the <paramref name="page"/> specified. </summary>
	/// <param name="page"> The memory page to append the <paramref name="newLine"/> to. </param>
	/// <param name="newLine"> The <see cref="string"/> to append to the <paramref name="page"/>. </param>
	public static async void Append(Files page, string newLine)
	{
		await File.WriteAllLinesAsync(Switch(page), [.. File.ReadAllLines(Switch(page)), newLine]);

#if DEBUG_DISK
		Messages.Logging.AsVerbose($"The value \"{NewLine}\" was appended to the end of the page {Page}.");
#endif
	}

	/// <summary> Overwrites the data at <paramref name="line"/> with the given <paramref name="newValue"/>, at the specified <paramref name="page"/>. </summary>
	/// <param name="page"> The memory page to modify. </param>
	/// <param name="line"> The number of the line to overwrite with the given <paramref name="newValue"/>. </param>
	/// <param name="newValue"> The <see cref="string"/> to overwrite the given <paramref name="line"/> with. </param>
	public static async void Write(Files page, int line, string newValue)
	{
		string Path = Switch(page);
		string[] STR = [.. File.ReadAllLines(Path)];
		STR.SetValue(newValue, line);
		await File.WriteAllLinesAsync(Path, STR);

#if DEBUG_DISK
		Messages.Logging.AsVerbose($"The value \"{NewValue}\" was written to the page {Page} at line {Line}");
#endif
	}

	/// <summary> Gets the path of a given <paramref name="page"/>. </summary>
	/// <param name="page"> The page to fetch the path to. </param>
	private static string Switch(Files page)
	{
#if DEBUG_DISK
		string ReturnValue = Page switch
		{
			Pages.Counters => COUNTERS,
			Pages.Starboard => STARBOARD,
			Pages.PDB_ID => PDB_ID,
			_ => throw new ArgumentException("You forgot to register the new memory file."),
		};

		Messages.Logging.AsVerbose($"Page {Page} at path \"{ReturnValue}\" was switched to successfully.");
		return ReturnValue;
#else
		return page switch
		{
			Files.Counters => COUNTERS,
			Files.Starboard => STARBOARD,
			Files.PDB_ID => PDB_ID,
			_ => throw new ArgumentException("You forgot to register the new memory file."),
		};
#endif
	}

	/// <summary>
	/// A list of memory pages used by P.BOT.
	/// </summary>
	public enum Files
	{
		/// <summary> Contains data related to basic operations, such as incremental counters. </summary>
		Counters,
		/// <summary> Contains a list of starred messages. </summary>
		Starboard,
		/// <summary> Contains a list of info related to post IDs. </summary>
		PDB_ID
	}
}
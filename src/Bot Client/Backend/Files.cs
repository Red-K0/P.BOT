using System.Collections.Frozen;
using static Bot.Backend.Files.Names;
namespace Bot.Backend;

/// <summary>
/// Contains methods and constants used for persistent data storage.
/// </summary>
internal static class Files
{
	/// <summary>
	/// Whether the specified file contains the given data.
	/// </summary>
	/// <param name="file"> The file to search for a match in. </param>
	/// <param name="data"> The data to search for. </param>
	/// <param name="append"> Whether to append the data if it isn't found. </param>
	public static async Task<bool> FileContains(Names file, string data, bool append = false)
	{
		if ((await File.ReadAllLinesAsync(Paths[file])).Contains(data))
		{
			return true;
		}
		else
		{
			if (append) await File.AppendAllTextAsync(Paths[file], $"\n{data}");
			return false;
		}
	}

	/// <summary>
	/// Reads the value of a counter.
	/// </summary>
	/// <param name="line"> The counter to read the value of. </param>
	/// <param name="mod"> The value to modify the counter by after reading. </param>
	public static async Task<int> ReadCounter(CounterLines line, int mod = 0)
	{
		string  Path = Paths[Counters];
		string[] STR = [.. await File.ReadAllLinesAsync(Path)];
		int   Return = Convert.ToInt32((await File.ReadAllLinesAsync(Path))[(int)line]);

		STR.SetValue((Return + mod).ToString(), (int)line);

		await File.WriteAllLinesAsync(Path, STR);

		return Return;
	}

	#region IO Constants

	/// <summary>
	/// The main path containing all the memory files used by P.BOT.
	/// </summary>
	private const string MAIN = @"F:\!PBOT\src\Bot Client\~Data\";

	/// <summary>
	/// A list of files used by P.BOT for storage.
	/// </summary>
	public enum Names
	{
		/// <summary> Contains data related to basic operations, such as incremental counters. </summary>
		Counters,

		/// <summary> Contains a list of starred messages. </summary>
		Starboard,
	}

	/// <summary>
	/// A list of names for lines in the <see cref="Names.Counters"/> file.
	/// </summary>
	public enum CounterLines
	{
		Starboard = 1,
		    Posts = 3,
	}

	/// <summary>
	/// The bot's internal list of file paths, indexed by their names.
	/// </summary>
	public static readonly FrozenDictionary<Names, string> Paths = new Dictionary<Names, string>() {
		{  Counters, MAIN +             "Counters.txt" },
		{ Starboard, MAIN + "Starred Message List.txt" }
	}.ToFrozenDictionary();

	#endregion IO Constants
}
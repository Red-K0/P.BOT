using System.Collections.Frozen;
using Microsoft.Extensions.Configuration;
using static Bot.Data.Files.Names;
namespace Bot.Data;

/// <summary>
/// Contains methods and constants used for persistent data storage.
/// </summary>
internal static class Files
{
	/// <summary>
	/// Allows the fetching of private data from user secrets.
	/// </summary>
	private static readonly IConfigurationRoot Secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

	/// <summary>
	/// Ensures all files required for data storage are present and formatted correctly.
	/// </summary>
	public static void EnsurePathsExist()
	{
		if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);

		foreach (KeyValuePair<Names, string> FileName in Paths)
		{
			if (File.Exists(FileName.Value)) continue;

			File.Create(FileName.Value);

			switch (FileName.Key)
			{
				case Counters:
					File.WriteAllText(FileName.Value, """
							Starboard Count
							0
							Post Count
							0
							""");
					break;
			}
		}
	}

	/// <summary>
	/// Extracts a value from the project secrets, returning null if the identifier does not exist.
	/// </summary>
	public static string? GetSecret(string id) => Secrets[id];

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
	/// <param name="increment"> The value to modify the counter by after reading. </param>
	public static async Task<ulong> GetCounter(CounterLines line, ulong increment = 0)
	{
		string[] Lines = await File.ReadAllLinesAsync(Paths[Counters]);
		ulong Return = Convert.ToUInt64(Lines[(int)line]);

		Lines[(int)line] = (Return + increment).ToString();

		await File.WriteAllLinesAsync(Paths[Counters], Lines);

		return Return;
	}

	public static async Task<FrozenDictionary<string, string>> GetDictionary(Names file)
	{
		string[] Lines = await File.ReadAllLinesAsync(Paths[file]);
		Dictionary<string, string> Dictionary = [];

		foreach (string line in Lines)
		{
			int CommaIndex = line.IndexOf(','), EndIndex = line.IndexOf(';');

			if (CommaIndex == -1) throw new InvalidDataException($"line \"{line}\" does not match the dictionary format.");

			Dictionary.Add(line[..CommaIndex].Trim(), line[(CommaIndex + 1)..(EndIndex == -1 ? line.Length : EndIndex)].Trim());
		}

		return Dictionary.ToFrozenDictionary();
	}

	#region IO Constants

	/// <summary>
	/// The main path containing all the memory files used by P.BOT.
	/// </summary>
	public static readonly string DataPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Bot Data\";

	/// <summary>
	/// The bot's internal list of file paths, indexed by their names.
	/// </summary>
	public static readonly FrozenDictionary<Names, string> Paths = new Dictionary<Names, string>() {
		{     Counters, DataPath +             "Counters.txt" },
		{    Starboard, DataPath + "Starred Message List.txt" },
		{ FounderRoles, DataPath +        "Founder Roles.txt" },
		{    Accoaldes, DataPath +            "Accolades.txt" },
	}.ToFrozenDictionary();

	/// <summary>
	/// A list of files used by P.BOT for storage.
	/// </summary>
	public enum Names
	{
		/// <summary> Contains a list of event role IDs, alongside their descriptions. </summary>
		Accoaldes,

		/// <summary> Contains data related to basic operations, such as incremental counters. </summary>
		Counters,

		/// <summary> Contains a list of starred messages. </summary>
		Starboard,

		/// <summary> Contains a list of founder IDs, alongside their role IDs. </summary>
		FounderRoles
	}

	/// <summary>
	/// A list of names for lines in the <see cref="Counters"/> file.
	/// </summary>
	public enum CounterLines
	{
		Starboard = 1,
		Posts = 3,
		CultistID = 5,
		FounderID = 7,
	}

	#endregion IO Constants
}
using Microsoft.Extensions.Configuration;
using static Bot.Data.Files.Names;
using System.Collections.Frozen;

namespace Bot.Data;

/// <summary>
/// Contains methods and constants used for persistent data storage.
/// </summary>
internal static class Files
{
	/// <summary>
	/// Ensures all files required for data storage are present and formatted correctly.
	/// </summary>
	static Files()
	{
		Root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Bot Data\";

		_paths = new Dictionary<Names, string>() {
		{    Accoaldes, Root +            "Accolades.txt" },
		{     Counters, Root +             "Counters.txt" },
		{ FounderRoles, Root +        "Founder Roles.txt" },
		{    Starboard, Root + "Starred Message List.txt" },
		{     Webhooks, Root +             "Webhooks.txt" }
		}.ToFrozenDictionary();

		if (!Directory.Exists(Root)) Directory.CreateDirectory(Root);

		foreach (KeyValuePair<Names, string> FileName in _paths)
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
	/// The bot's filepath list, indexed by name.
	/// </summary>
	private static readonly FrozenDictionary<Names, string> _paths;

	/// <summary>
	/// The main path for all bot data files.
	/// </summary>
	public static readonly string Root;

	/// <summary>
	/// Allows the fetching of private data from user secrets.
	/// </summary>
	private static readonly IConfigurationRoot Secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

	/// <summary>
	/// Checks if the specified file contains the given data.
	/// </summary>
	/// <param name="file">The file to search for a match in.</param>
	/// <param name="data">The data to search for.</param>
	/// <param name="append">Whether to append the data if it isn't found.</param>
	public static async Task<bool> FileContains(Names file, string data, bool append = false)
	{
		if ((await File.ReadAllLinesAsync(_paths[file])).Contains(data))
		{
			return true;
		}
		else if (append)
		{
			await File.AppendAllTextAsync(_paths[file], $"\n{data}");
		}
		return false;
	}

	/// <summary>
	/// Reads the value of a counter.
	/// </summary>
	/// <param name="line">The counter to read the value of.</param>
	/// <param name="increment">The value to modify the counter by after reading.</param>
	public static async Task<ulong> GetCounter(CounterLines line, ulong increment = 0)
	{
		string[] Lines = await File.ReadAllLinesAsync(_paths[Counters]);
		ulong Return = Convert.ToUInt64(Lines[(int)line]);

		Lines[(int)line] = (Return + increment).ToString();

		await File.WriteAllLinesAsync(_paths[Counters], Lines);

		return Return;
	}

	/// <summary>
	/// Reads a file's contents, returning them as a <see cref="FrozenDictionary{TKey, TValue}"/> object of type <see cref="string"/>, <see cref="string"/>.
	/// </summary>
	/// <param name="file">The file to read.</param>
	public static async Task<FrozenDictionary<string, string>> GetDictionary(Names file)
	{
		string[] Lines = await File.ReadAllLinesAsync(_paths[file]);
		Dictionary<string, string> Dictionary = [];

		foreach (string line in Lines)
		{
			int CommaIndex = line.IndexOf(','), EndIndex = line.IndexOf(';');

			Dictionary.Add(line[..CommaIndex].Trim(), line[(CommaIndex + 1)..(EndIndex == -1 ? line.Length : EndIndex)].Trim());
		}

		return Dictionary.ToFrozenDictionary();
	}

	/// <summary>
	/// Gets the value associated with a specified secret.
	/// </summary>
	/// <param name="secret">The channel to get the value for.</param>
	public static string? TryGetSecret(string secret) => Secrets[secret];

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

	/// <summary>
	/// The bot's filename list.
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
		FounderRoles,

		/// <summary> Contains a list of webhook URLs. </summary>
		Webhooks
	}
}
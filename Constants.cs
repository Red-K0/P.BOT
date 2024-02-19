using Microsoft.Extensions.Configuration;
using P_BOT.Command_Processing;
using P_BOT.Command_Processing.Helpers;
using System.Text;

namespace P_BOT;

/// <summary> Contains the various constants used throughout the bot. </summary>
internal static class Constants
{
	#region Bot

	/// <summary> P.BOT's main client. </summary>
	public static GatewayClient client = new
	(
		new BotToken(new ConfigurationBuilder().AddUserSecrets<Program>().Build().GetSection("Discord")["Token"]!),
		new GatewayClientConfiguration() { Intents = GatewayIntents.All }
	);

	/// <summary> P.BOT's HTTP client, used for external network requests. </summary>
	public static HttpClient client_h = new();

	/// <summary> The bot's User ID. Not so secret. </summary>
	public const ulong BOT_ID = 1169031557848252516;
	#endregion

	#region Server

	/// <summary> The server's Guild ID. </summary>
	public const ulong SERVER_ID = 1131100534250680433;

	/// <summary> The start of the server's URL. </summary>
	public const string SERVER_LINK = "https://discord.com/channels/1131100534250680433/";

	/// <summary> The starboard channel's Channel ID. </summary>
	public const ulong SERVER_STARBOARD = 1133836713194696744;

	/// <summary> The channel to send posts and other data to. </summary>
	public const ulong SERVER_POSTFEED = 1208068312487960606;
	#endregion

	#region Error Strings

	//DnD Text Module
	public static readonly CompositeFormat ERROR_DND_FORMAT = CompositeFormat.Parse(
	"""
	Sorry, the parameters for your roll `{0}` couldn't be parsed, please input your roll in the format `.{1} [Number]d[Number] [Number]`, where:
	- The first 'Number' is the number of die to roll
	- The second is the number of faces per die
	- The third is the modifier to place on each roll
	For example:
	```
	.{1} 4d20	  -	Rolls 4 die with 20 sides
	.{1} 4d4 1	 -	Rolls 4 die with 4 sides, adding a 1 to each
	.{1} 4d5 -2	-	Rolls 4 die with 5 sides, subtracting a 2 from each
	```
	""");

	public const string ERROR_CODE_NODEV = "Unimplemented.";
	public const string ERROR_DEFINEFAIL = "Sorry, the given term couldn't be defined.";
	public const string ERRORLOG_NOWRITE = "\"at System.Linq.Enumerable.ToDictionary[TSource,TKey,TElement](IEnumerable`1 source, Func`2 keySelector, Func`2 elementSelector)\"";
	#endregion

	#region URL Locations
	/// <summary> The URL of the translation API used by <see cref="SlashCommand.Translate(string, Translation.Options, Translation.Options)"/>. </summary>
	public const string URL_TRANSLATE = "https://655.mtis.workers.dev/translate";

	// TODO | Work on this
	public const string URL_WIKIPEDIA = "https://en.wikipedia.org/w/api.php?action=query&generator=prefixsearch&redirects=1&gpslimit=1&explaintext=0&format=json&prop=extracts&";

	/// <summary> The URL of the translation icon used by <see cref="SlashCommand.Translate(string, Translation.Options, Translation.Options)"/> </summary>
	public const string URL_TLICON = "https://i.ibb.co/GV149Px/iamvector-download-1.png";

	/// <summary> The URL of the rulebook emoji used by discord. Used by <see cref="SlashCommand.GetDefinition(Define.DefineChoices)"/>. </summary>
	public const string URL_RULESICON = "https://emoji.discadia.com/emojis/2ae224b9-e6d5-40c2-aa41-791939fbd113.PNG";

	public const string URL_ANONUSER = "https://i.ibb.co/qBQXBg0/svgviewer-png-output.png";
	#endregion

	#region Memory Locations
	public const string MEMORY = @"F:\!PBOT\~Data\";
	public const string MEMORY_COUNTERS = MEMORY + "Counters.txt";
	public const string MEMORY_SECRETSANTA_RECIEVER = MEMORY + "Secret Santa Recipient List.txt";
	public const string MEMORY_SECRETSANTA_SENDER = MEMORY + "Secret Santa Sender List.txt";
	public const string MEMORY_STARRED_MESSAGES = MEMORY + "Starred Message List.txt";
	public const string MEMORY_POSTDATABASE_IDLIST = MEMORY + @"PostDB\ID.txt";
	#endregion
}
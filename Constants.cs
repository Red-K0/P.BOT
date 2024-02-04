using System.Text;
using Microsoft.Extensions.Configuration;


namespace P_BOT;

/// <summary> Contains the various constants used throughout the bot. </summary>
internal static class Constants
{
	/// <summary> Secret configuration. </summary>
	private static readonly IConfiguration config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

	#region Bot
	/// <summary> The intents passed to the <see cref="GatewayClient"/>. </summary>
	public const GatewayIntents BOT_INTENTS = GatewayIntents.All | GatewayIntents.MessageContent;

	/// <summary> P.BOT's main client. </summary>
	public static GatewayClient client = new(new Token(0, config.GetSection("Bot")["Token"]), new GatewayClientConfiguration() { Intents = BOT_INTENTS });

	/// <summary> P.BOT's HTTP client </summary>
	public static HttpClient client_h = new();

	/// <summary> The bot's User ID. Not so secret. </summary>
	public const ulong BOT_ID = 700796664276844612;
	#endregion

	#region Server
	/// <summary> The start of the server's URL. </summary>
	public const string SERVER_LINK = "https://discord.com/channels/1131100534250680433/";

	/// <summary> The server's Guild ID. </summary>
	public const ulong SERVER_ID = 1131100534250680433;

	/// <summary> The starboard channel's Channel ID. </summary>
	public const ulong SERVER_STARBOARD = 1133836713194696744;
	#endregion

	#region Commands

	#region Ping
	public const string CMD_NTPING_NAME = "syscheck";
	public const string CMD_NTPING_DESC = "Check if the system's online";
	#endregion

	#region Secret Santa
	public const string CMD_SSANTA_NAME = "secretsanta";
	public const string CMD_SSANTA_DESC = "Merry Christmas";
	#endregion

	#region GetAvatar
	public const string CMD_AVATAR_NAME = "getavatar";
	public const string CMD_AVATAR_DESC = "Get a user's avatar";

	public const string CMD_AVATAR_PR1N = "user";
	public const string CMD_AVATAR_PR1D = "The UID to pull an avatar from";
	public const string CMD_AVATAR_PR2N = "format";
	public const string CMD_AVATAR_PR2D = "The image file type (GIF and Lottie are currently unsupported)";
	#endregion

	#region Define
	public const string CMD_DEFINE_NAME = "define";
	public const string CMD_DEFINE_DESC = "Define a given term";

	public const string CMD_DEFINE_PR1N = "term";
	public const string CMD_DEFINE_PR1D = "The term to define";
	#endregion

	#region Poll
	public const string CMD_STPOLL_NAME = "poll";
	public const string CMD_STPOLL_DESC = "Start a poll (WIP)";
	#endregion

	#region Module Toggle
	public const string CMD_TOGGLE_NAME = "togglemodule";
	public const string CMD_TOGGLE_DESC = "Toggle the status of a module";

	public const string CMD_TOGGLE_PR1N = "module";
	public const string CMD_TOGGLE_PR1D = "The name of the module to modify";
	#endregion

	#region Translate
	public const string CMD_HTTPTL_NAME = "translate";
	public const string CMD_HTTPTL_DESC = "Translate (WIP)";

	public const string CMD_HTTPTL_PR1N = "input";
	public const string CMD_HTTPTL_PR1D = "The text to translate";

	public const string CMD_HTTPTL_PR2N = "original_language";
	public const string CMD_HTTPTL_PR2D = "The language of the original text";

	public const string CMD_HTTPTL_PR3N = "target_language";
	public const string CMD_HTTPTL_PR3D = "The language to translate the text to";

	#endregion

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
	#endregion

	#region URL Locations
	/// <summary> The URL of the translation API used by <see cref="SlashCommands.Translate(string, Translation.SupportedLanguages, Translation.SupportedLanguages)"/>. </summary>
	public const string URL_TRANSLATE = "https://655.mtis.workers.dev/translate";
	
	/// <summary> The URL of the translation icon used by <see cref="SlashCommands.Translate(string, Translation.SupportedLanguages, Translation.SupportedLanguages)"/> </summary>
	public const string URL_TLICON = "https://i.ibb.co/GV149Px/iamvector-download-1.png";
	
	/// <summary> The URL of the rulebook emoji used by discord. Used by <see cref="SlashCommands.Define(DefineData.DefineChoices)"/>. </summary>
	public const string URL_RULESICON = "https://emoji.discadia.com/emojis/2ae224b9-e6d5-40c2-aa41-791939fbd113.PNG";
	#endregion

	#region Memory Locations
	public const string MEMORY_COUNTERS = @"F:\!PBOT\memory\Counters.txt";
	public const string MEMORY_SECRETSANTA_RECIEVER = @"F:\!PBOT\memory\Secret Santa Recipient List.txt";
	public const string MEMORY_SECRETSANTA_SENDER = @"F:\!PBOT\memory\Secret Santa Sender List.txt";
	public const string MEMORY_STARRED_MESSAGES = @"F:\!PBOT\memory\Starred Message List.txt";
	#endregion
}
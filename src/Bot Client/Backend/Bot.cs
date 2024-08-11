namespace PBot;

using Microsoft.Extensions.Configuration;
using NetCord.Gateway.Voice;
using PBot.Commands.Helpers;
using static GatewayIntents;

/// <summary>
/// Contains methods for initializing and preparing the bot's client.
/// </summary>
internal static class Bot
{
	/// <summary>
	/// Allows the fetching of private data from user secrets.
	/// </summary>
	private static readonly IConfigurationRoot Secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

	/// <summary>
	/// Extracts a value from the project secrets, returning null if the identifier does not exist.
	/// </summary>
	public static string? GetSecret(string id) => Secrets[id];


	/// <summary>
	/// Main client, used for communicating with Discord.
	/// </summary>
	public static readonly GatewayClient Client = new(new BotToken(GetSecret("Token")!), new GatewayClientConfiguration()
	{ Intents = MessageContent | Guilds | GuildUsers | GuildMessages | GuildMessageReactions | GuildVoiceStates });

	/// <summary>
	/// HTTP client, used for external network requests.
	/// </summary>
	public static readonly HttpClient NetClient = new();


	/// <summary>
	/// Gets a guild from the <see cref="Client"/>'s cache from using its ID.
	/// </summary>
	public static Guild GetGuild(ulong id) => Client.Cache.Guilds[id];

	/// <summary>
	/// The current guild's ID, fetched via <see cref="GetSecret(string)"/>.
	/// </summary>
	public static readonly ulong GuildID = Convert.ToUInt64(GetSecret("ServerID"));

	/// <summary>
	/// The base guild URL.
	/// </summary>
	public static readonly string GuildURL = $"https://discord.com/channels/{GuildID}/";


	/// <summary>
	/// Stops the client, releases its resources, and restarts the bot client.
	/// </summary>
	public static async Task Restart()
	{
		await Client.CloseAsync();
		Client.Dispose();

		Process.Start(Process.GetCurrentProcess().StartInfo);
		Environment.Exit(-1);
	}

	/// <summary>
	/// Starts the bot's client, initializes event handlers, and prepares caches.
	/// </summary>
	public static async Task Start()
	{
		await Events.MapClientHandlers();
		await Client.StartAsync();
		await Client.ReadyAsync;
		Caches.Members.Load();

		ProbabilityStateMachine.InitXShift128();
	}
}
namespace PBot;

using Microsoft.Extensions.Configuration;
using PBot.Commands.Helpers;
using PBot.Messages;
using static GatewayIntents;

/// <summary>
/// Contains methods for initializing and preparing the bot's client.
/// </summary>
public static class Bot
{
	/// <summary>
	/// Allows the fetching of private data from user secrets.
	/// </summary>
	private static readonly IConfigurationSection Secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build().GetSection("Secrets");

	/// <summary>
	/// The current guild's ID, fetched via <see cref="GetSecret(string)"/>.
	/// </summary>
	public static readonly ulong GuildID = Convert.ToUInt64(GetSecret("ServerID"));

	/// <summary>
	/// The base guild URL.
	/// </summary>
	public static readonly string GuildURL = $"https://discord.com/channels/{GuildID}/";

	/// <summary>
	/// Extracts a value from the project secrets, returning null if the identifier does not exist.
	/// </summary>
	public static string? GetSecret(string id) => Secrets[id];

	/// <summary>
	/// P.BOT's main client.
	/// </summary>
	public static readonly GatewayClient Client = new(new BotToken(GetSecret("Token")!), new GatewayClientConfiguration()
	{ Intents = MessageContent | Guilds | GuildUsers | GuildMessages | GuildMessageReactions | GuildModeration });

	/// <summary>
	/// P.BOT's HTTP client, used for external network requests.
	/// </summary>
	public static readonly HttpClient ClientH = new();

	/// <summary>
	/// Starts the bot's client, initializes event handlers, and prepares caches.
	/// </summary>
	public static async void Start()
	{
		await Events.MapClientHandlers();
		await Client.StartAsync();
		await Client.ReadyAsync;
		Caches.Members.Load();

		ProbabilityStateMachine.InitXShift128();
	}

	/// <summary>
	/// Stops the client, releases its resources, and restarts the bot client.
	/// </summary>
	public static async Task Restart()
	{
		await Client.CloseAsync();
		Client.Dispose();

		Process.Start(Environment.ProcessPath!, Environment.GetCommandLineArgs());
		Process.GetCurrentProcess().Kill();
	}
}

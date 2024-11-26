using Bot.Interactions.Helpers;
using static NetCord.Gateway.GatewayIntents;
namespace Bot.Backend;

/// <summary>
/// Contains methods for initializing and preparing the bot's client.
/// </summary>
internal static class Core
{
	/// <summary>
	/// Main client, used for communicating with Discord.
	/// </summary>
	public static readonly GatewayClient Client = new(new BotToken(Files.GetSecret("Token")!), new GatewayClientConfiguration()
	{ Intents = MessageContent | Guilds | GuildUsers | GuildMessages | GuildMessageReactions | GuildVoiceStates });

	/// <summary>
	/// HTTP client, used for external network requests.
	/// </summary>
	public static readonly HttpClient NetClient = new();


	/// <summary>
	/// Gets the bot's main guild.
	/// </summary>
	public static Guild Guild { get; set; } = null!;

	/// <summary>
	/// The current guild's ID, fetched via <see cref="Files.GetSecret(string)"/>.
	/// </summary>
	public static readonly ulong GuildID = Convert.ToUInt64(Files.GetSecret("ServerID"));

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

		ProbabilityStateMachine.LoadMersenneTwister();
	}
}
using static NetCord.Gateway.GatewayIntents;

namespace Bot.Backend;

/// <summary>
/// Contains methods for initializing and preparing the bot.
/// </summary>
internal static class Core
{
	/// <summary>
	/// Main client, used for communicating with Discord.
	/// </summary>
	public static readonly GatewayClient Client = new(new BotToken(Files.TryGetSecret("Token")!), new()
	{ Intents = MessageContent | Guilds | GuildUsers | GuildMessages | GuildMessageReactions | GuildVoiceStates });

	/// <summary>
	/// HTTP client, used for external network requests.
	/// </summary>
	public static readonly HttpClient NetClient = new();

	/// <summary>
	/// The bot's guild.
	/// </summary>
	public static Guild Guild { get; set; } = null!;

	/// <summary>
	/// The guild's ID, fetched via <see cref="Files.TryGetSecret(string)"/>.
	/// </summary>
	public static readonly ulong GuildID = Convert.ToUInt64(Files.TryGetSecret("ServerID"));

	/// <summary>
	/// The guild's base URL.
	/// </summary>
	public static readonly string GuildURL = $"https://discord.com/channels/{GuildID}/";

	/// <summary>
	/// Stops the client, releases its resources, and restarts the bot.
	/// </summary>
	public static async Task Restart()
	{
		await Client.CloseAsync();
		Client.Dispose();

		Process.Start(Process.GetCurrentProcess().StartInfo);
		Environment.Exit(-1);
	}

	/// <summary>
	/// Initializes the bot's event handlers and starts the client.
	/// </summary>
	public static async Task Start()
	{
		await Events.MapClientHandlers();
		await Client.StartAsync();
	}
}
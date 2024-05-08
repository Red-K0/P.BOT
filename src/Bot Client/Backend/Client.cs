namespace PBot;

using Microsoft.Extensions.Configuration;
using PBot.Commands.Helpers;
using PBot.Messages;

/// <summary>
/// Contains methods for initializing and preparing the bot's client.
/// </summary>
public static class Client
{
	/// <summary>
	/// Allows the fetching of private data from user secrets.
	/// </summary>
	public static readonly IConfigurationRoot secrets = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

	/// <summary>
	/// P.BOT's main client.
	/// </summary>
	public static readonly GatewayClient client = new(new BotToken(secrets.GetSection("Discord")["Token"]!), new GatewayClientConfiguration()
	{
		Intents = GatewayIntents.All
	});

	/// <summary>
	/// P.BOT's HTTP client, used for external network requests.
	/// </summary>
	public static readonly HttpClient client_h = new();

	/// <summary>
	/// The start of the server URL.
	/// </summary>
	public const string SERVER_LINK = "https://discord.com/channels/1131100534250680433/";

	/// <summary>
	/// Starts the bot's client, initializes event handlers, and prepares caches.
	/// </summary>
	public static async void Start()
	{
		await Events.MapClientHandlers();
		await client.StartAsync();
		await client.ReadyAsync;
		Caches.Members.Load();

		ProbabilityStateMachine.InitXShift128();
	}

	/// <summary>
	/// Stops the client, releases its resources, and restarts the bot client.
	/// </summary>
	public static async Task Restart()
	{
		await client.CloseAsync();
		client.Dispose();

		Process.Start(Environment.ProcessPath!, Environment.GetCommandLineArgs());
		Process.GetCurrentProcess().Kill();
	}
}

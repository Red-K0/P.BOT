namespace PBot;

using Microsoft.Extensions.Configuration;
using NetCord.Services.ApplicationCommands;
using PBot.Messages;
using static PBot.Messages.Logging;

/// <summary>
/// Contains methods for initializing and preparing the bot's client.
/// </summary>
public static class Client
{
	/// <summary>
	/// P.BOT's main client.
	/// </summary>
	public static readonly GatewayClient client = new
	(
		new BotToken(new ConfigurationBuilder().AddUserSecrets<Program>().Build().GetSection("Discord")["Token"]!),
		new GatewayClientConfiguration() { Intents = GatewayIntents.All }
	);

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
		Events.MapClientHandlers();
		await client.StartAsync();
		await client.ReadyAsync;
		Caches.Members.Load();
	}

	/// <summary>
	/// Starts the bot's interaction handler and assigns it to the client.
	/// </summary>
	public static async void StartInteractionHandler()
	{
		ApplicationCommandService<SlashCommandContext> cmdSrv = new();
		cmdSrv.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);
		await cmdSrv.CreateCommandsAsync(client.Rest, client.Id);

		client.InteractionCreate += async interaction =>
		{
			try
			{
				if (interaction is SlashCommandInteraction sCmd && await cmdSrv.ExecuteAsync(new SlashCommandContext(sCmd, client)) is IFailResult fRes)
				{
					WriteAsID(fRes.Message, SpecialId.Network);
				}
			}
			catch { }
		};
	}
}

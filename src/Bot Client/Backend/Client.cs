using NetCord.Services.ApplicationCommands;
using PBot.Messages;
using static PBot.Messages.Logging;

/// <summary>
/// Contains methods for initializing and preparing the bot's client.
/// </summary>
internal static class Client
{
	/// <summary>
	/// Starts the bot's client, initializes event handlers, and prepares caches.
	/// </summary>
	public static async void Start()
	{
		Events.MapClientHandlers();
		await client.StartAsync();
		await client.ReadyAsync;
		PBot.Caches.Members.Load();
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
			if (interaction is SlashCommandInteraction sCmd && await cmdSrv.ExecuteAsync(new SlashCommandContext(sCmd, client)) is IFailResult fRes)
			{
				WriteAsID(fRes.Message, SpecialId.Network);
			}
		};
	}
}

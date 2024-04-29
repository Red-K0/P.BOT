using Microsoft.Extensions.Configuration;

internal static class Constants
{
	/// <summary>
	/// P.BOT's main client.
	/// </summary>
	public static GatewayClient client = new
	(
		new BotToken(new ConfigurationBuilder().AddUserSecrets<Program>().Build().GetSection("Discord")["Token"]!),
		new GatewayClientConfiguration() { Intents = GatewayIntents.All }
	);

	/// <summary>
	/// P.BOT's HTTP client, used for external network requests.
	/// </summary>
	public static HttpClient client_h = new();

	/// <summary>
	/// The start of the server URL.
	/// </summary>
	public const string SERVER_LINK = "https://discord.com/channels/1131100534250680433/";
}
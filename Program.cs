using NetCord.Services.ApplicationCommands;
using P_BOT.Messages;

#region Event Handlers
client.Log += Logging.Log;
client.MessageCreate += Events.MessageCreated;
client.MessageDelete += Events.MessageDeleted;
client.MessageUpdate += Events.MessageUpdated;
client.MessageReactionAdd += Events.ReactionAdded;
#endregion

#region Startup
Console.CursorVisible = false;
Console.OutputEncoding = System.Text.Encoding.Unicode;

await client.StartAsync();
await client.ReadyAsync;

UserManagement.InitMembersSearch();

ApplicationCommandService<SlashCommandContext> applicationCommandService = new();
applicationCommandService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);

await applicationCommandService.CreateCommandsAsync(client.Rest, client.Id);
client.InteractionCreate += async interaction =>
{
	if (interaction is SlashCommandInteraction slashCommandInteraction)
	{
		_ = await applicationCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, client));
	}
};

await Task.Delay(Timeout.Infinite);
#endregion
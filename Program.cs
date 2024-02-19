using NetCord.Services.ApplicationCommands;
using P_BOT;
using P_BOT.Command_Processing.Helpers;

#region Events

// System Message Logging
client.Log += MessageLogging.System;

// When a message is deleted
client.MessageDelete += MessageLogging.Delete;

// When a message is edited
client.MessageUpdate += MessageLogging.Update;

// When a message is reacted to
client.MessageReactionAdd += MessageLogging.ReactAdd;

// When a message is sent
client.MessageCreate += MessageLogging.Create;

#endregion

#region Startup Sequence

Console.CursorVisible = false;
Console.OutputEncoding = System.Text.Encoding.Unicode;

await client.StartAsync();
await client.ReadyAsync;

#region Slash Command Handler

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
#endregion

await Task.Delay(Timeout.Infinite);

#endregion

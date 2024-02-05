using NetCord.Services.ApplicationCommands;
using P_BOT;

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

await client.StartAsync().ConfigureAwait(true);
await client.ReadyAsync.ConfigureAwait(true);

#region Slash Command Handler

#pragma warning disable IL2026
ApplicationCommandService<SlashCommandContext> applicationCommandService = new();
applicationCommandService.AddModules(System.Reflection.Assembly.GetEntryAssembly()!);
#pragma warning restore

await applicationCommandService.CreateCommandsAsync(client.Rest, client.ApplicationId).ConfigureAwait(true);
client.InteractionCreate += async interaction =>
{
#pragma warning disable CA1031
	if (interaction is SlashCommandInteraction slashCommandInteraction)
	{
		try { _ = await applicationCommandService.ExecuteAsync(new SlashCommandContext(slashCommandInteraction, client)).ConfigureAwait(true); }

		catch (Exception ex) { await interaction.SendResponseAsync(InteractionCallback.Message($"Error: {ex.Message}")).ConfigureAwait(true); }
	}
#pragma warning restore CA1031
};
#endregion

await Task.Delay(-1).ConfigureAwait(true);
client.Dispose();

#endregion

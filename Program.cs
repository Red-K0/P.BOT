﻿using NetCord.Services.ApplicationCommands;
using P_BOT.Messages;

#if DEBUG
Stopwatch Timer = Stopwatch.StartNew();
#endif

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

#if DEBUG
Logging.AsVerbose($"Startup Complete [{Timer.ElapsedMilliseconds}ms]");
Timer.Reset();
#endif

await Task.Delay(Timeout.Infinite);
#endregion
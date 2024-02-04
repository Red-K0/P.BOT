using NetCord.Services.ApplicationCommands;
using P_BOT;

const ulong AUTHORID_SYSTEM = 0;
const ulong AUTHORID_CLIENT = 1;

#region Events

// System Message Logging
client.Log += message =>
{
	if (MessageFunctions.LastAuthor != AUTHORID_SYSTEM)
	{
		Console.Write('\n');
	}

	MessageFunctions.LastAuthor = AUTHORID_SYSTEM;
	MessageFunctions.WriteColor("System: ", ConsoleColor.Green);
	Console.WriteLine(message);
	return ValueTask.CompletedTask;
};

client.MessageDelete += message =>
{
	if (MessageFunctions.LastAuthor != AUTHORID_CLIENT)
	{
		Console.Write('\n');
	}

	MessageFunctions.LastAuthor = AUTHORID_CLIENT;
	MessageFunctions.WriteColor("Client: ", ConsoleColor.Red);
	Console.WriteLine($"The message with ID '{message.MessageId}' was deleted.");
	return default;
};

client.MessageUpdate += message =>
{
	if (MessageFunctions.LastAuthor != AUTHORID_CLIENT)
	{
		Console.Write('\n');
	}

	MessageFunctions.LastAuthor = AUTHORID_CLIENT;
	MessageFunctions.WriteColor("Client: ", ConsoleColor.Red);
	Console.WriteLine($"The message with ID '{message.Id}' was updated by {message.Author.Username} with the new content:");
	MessageFunctions.WriteColor($"{message.Content}\n", ConsoleColor.DarkGray);
	return default;
};

client.MessageReactionAdd += message =>
{
#pragma warning disable CA1849 // Call async methods when in an async method
	if (message.Emoji.Name == "⭐" && !File.ReadAllLines(MEMORY_STARRED_MESSAGES).Contains(message.MessageId.ToString()))
	{
		MessageFunctions.AddToStarBoard(message);
	}
	return default;
#pragma warning restore CA1849 // Call async methods when in an async method
};

// When a message is sent
client.MessageCreate += message =>
{
	// Core Message Parsing
	if (!message.Author.IsBot)
	{
		MessageFunctions.LogMessage(message);
	}

	if (message.Content.Contains(@"https://discord.com/channels/") && message.Author.Id != 1169031557848252516)
	{
		MessageFunctions.ParseMessageLink(message, client);
	}

	// Modular Message Parsing
	if (Options.DnDTextModule && message.Content.StartsWith('.'))
	{
		RollMath.LogicSelect(message);
	}

	return ValueTask.CompletedTask;
};

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

using NetCord.Services.ApplicationCommands;
using static Bot.Messages.Logging;

namespace Bot.Backend;

/// <summary>
/// Contains methods responsible for handling message events.
/// </summary>
internal static partial class Events
{
	private static readonly ApplicationCommandService<SlashCommandContext> CommandService = new();

	/// <summary>
	/// Maps the <see cref="Client"/>'s events to their appropriate response method.
	/// </summary>
	public static async Task MapClientHandlers()
	{
		CommandService.AddModule<Commands.SlashCommands>();
		await CommandService.CreateCommandsAsync(Client.Rest, Client.Id);

		Client.Log += LogNetworkMessage;
		Client.InteractionCreate += InteractionCreate;

		Client.GuildUserAdd  += GuildUserUpdate; Client.GuildUserUpdate += GuildUserUpdate; Client.GuildUserRemove += GuildUserRemove;
		Client.RoleCreate    +=      RoleUpdate; Client.RoleUpdate      +=      RoleUpdate; Client.RoleDelete      +=      RoleDelete;
		Client.MessageCreate +=   MessageCreate; Client.MessageUpdate   +=   MessageUpdate; Client.MessageDelete   +=   MessageDelete;

		Client.MessageReactionAdd += ReactionAdded;
	}

	/// <summary>
	/// Processes received interactions.
	/// </summary>
	public static async ValueTask InteractionCreate(Interaction interaction)
	{
		try
		{
			if (interaction is SlashCommandInteraction SlashCommand && await CommandService.ExecuteAsync(new(SlashCommand, Client)) is IFailResult Result)
			{
				WriteAsID("Slash Command failed with message - " + Result.Message + "\n" + $"""
					            Channel: {SlashCommand.Channel}
					                 At: {SlashCommand.CreatedAt}
					         Command ID: {SlashCommand.Id}
					            User ID: {SlashCommand.User.Id}
					""", SpecialId.Network);
			}
		}
		catch (Exception ex)
		{
			WriteAsID(ex.Message, SpecialId.Network);
		}
	}
}
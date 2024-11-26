using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.ComponentInteractions;
using static Bot.Messages.Logging;

namespace Bot.Backend;

/// <summary>
/// Contains methods responsible for handling message events.
/// </summary>
internal static partial class Events
{
	private static readonly ApplicationCommandService<SlashCommandContext> CommandService = new();
	private static readonly ComponentInteractionService<ButtonInteractionContext> ComponentService = new();

	/// <summary>
	/// Maps the <see cref="Client"/>'s events to their appropriate response method.
	/// </summary>
	public static async Task MapClientHandlers()
	{
		CommandService.AddModule<Interactions.SlashCommands>();
		ComponentService.AddModule<Interactions.Helpers.Library>();
		await CommandService.CreateCommandsAsync(Client.Rest, Client.Id);

		Client.Log += LogNetworkMessage;
		Client.InteractionCreate += InteractionCreate;

		Client.GuildUserAdd  += GuildUserUpdate; Client.GuildUserUpdate += GuildUserUpdate; Client.GuildUserRemove += GuildUserRemove;
		Client.MessageCreate +=   MessageCreate; Client.MessageUpdate   +=   MessageUpdate; Client.MessageDelete   +=   MessageDelete;

		Client.GuildCreate += static async (e) => { Core.Guild = e.Guild!; await Members.LoadDictionaries(); };

		Client.MessageReactionAdd += ReactionAdded;
	}

	/// <summary>
	/// Processes received interactions.
	/// </summary>
	public static async ValueTask InteractionCreate(Interaction interaction)
	{
		try
		{
			switch (interaction)
			{
				case SlashCommandInteraction SlashCommand:
					if (await CommandService.ExecuteAsync(new(SlashCommand, Client)) is IFailResult CommandResult)
					{
						WriteAsID("Slash Command failed with message - " + CommandResult.Message + "\n" + $"""
										Channel: {SlashCommand.Channel}
											 At: {SlashCommand.CreatedAt}
									 Command ID: {SlashCommand.Id}
										User ID: {SlashCommand.User.Id}
							""", SpecialId.Network);
					}
					break;
				case ButtonInteraction Button:
					if (await ComponentService.ExecuteAsync(new ButtonInteractionContext(Button, Client)) is IFailResult ButtonResult)
					{
						WriteAsID("Button Interaction failed with message - " + ButtonResult.Message + "\n" + $"""
										Channel: {Button.Channel}
											 At: {Button.CreatedAt}
									 Command ID: {Button.Id}
										User ID: {Button.User.Id}
							""", SpecialId.Network);
					}
					break;
			}
		}
		catch (Exception ex)
		{
			WriteAsID(ex.Message, SpecialId.Network);
		}
	}
}
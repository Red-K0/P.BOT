using NetCord.Services.ComponentInteractions;
using NetCord.Services.ApplicationCommands;
using static Bot.Messages.Logging;

namespace Bot.Backend;

/// <summary>
/// Contains methods responsible for handling gateway events.
/// </summary>
internal static partial class Events
{
	private static readonly ApplicationCommandService<SlashCommandContext> _commandService = new();
	private static readonly ComponentInteractionService<ModalInteractionContext> _modalService = new();
	private static readonly ComponentInteractionService<ButtonInteractionContext> _buttonService = new();

	/// <summary>
	/// Maps the <see cref="Client"/>'s events to their appropriate response method.
	/// </summary>
	public static async Task MapClientHandlers()
	{
		_commandService.AddModule<Interactions.SlashCommands>();
		_modalService.AddModule<Interactions.Helpers.Webhooks>();
		_buttonService.AddModule<Interactions.Helpers.Library>();

		await _commandService.CreateCommandsAsync(Client.Rest, Client.Id);

		Client.Log += LogNetworkMessage;
		Client.InteractionCreate += InteractionCreate;

		Client.GuildUserAdd  += GuildUserUpdate; Client.GuildUserUpdate += GuildUserUpdate; Client.GuildUserRemove += GuildUserRemove;
		Client.MessageCreate +=   MessageCreate; Client.MessageUpdate   +=   MessageUpdate; Client.MessageDelete   +=   MessageDelete;

		Client.GuildCreate += static async args => { Core.Guild = args.Guild!; await Members.LoadMemberList(); };

		Client.MessageReactionAdd += ReactionAdded;
	}

	/// <summary>
	/// Processes received interactions by identifying their type, and passing them to the correct service.
	/// </summary>
	/// <param name="interaction">The interaction to handle.</param>
	public static async ValueTask InteractionCreate(Interaction interaction)
	{
		switch (interaction)
		{
			case SlashCommandInteraction command:
				await _commandService.ExecuteAsync(new(command, Client));
				break;

			case ButtonInteraction button:
				await _buttonService.ExecuteAsync(new(button, Client));
				break;

			case ModalInteraction modal:
				await _modalService.ExecuteAsync(new(modal, Client));
				break;
		}
	}
}
using NetCord.Services.ApplicationCommands;

namespace Bot.Interactions;
public sealed partial class SlashCommands
{
	/// <summary>
	/// Command Task. Creates a modal to display the post UI.
	/// </summary>
	[SlashCommand("post", "Displays the post creation interface.")]
	public async Task CreatePost()
	{
		await RespondAsync(InteractionCallback.Modal(new("PostModal", "Placeholder", [
			new TextInputProperties("Login", TextInputStyle.Short, "Login"),
			new TextInputProperties("Content", TextInputStyle.Paragraph, "Content")
		])));
	}
}

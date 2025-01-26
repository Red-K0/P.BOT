using NetCord.Services.ComponentInteractions;
using System.Collections.Frozen;

namespace Bot.Interactions.Helpers;

/// <summary>
/// Contains methods relevant to webhook interaction and handling.
/// </summary>
public class Webhooks : ComponentInteractionModule<ModalInteractionContext>
{
	/// <summary>
	/// Populates <see cref="Clients"/> with IDs and Tokens acquired from disk.
	/// </summary>
	static Webhooks() => Clients = Files.GetDictionary(Files.Names.Webhooks).Result
			.Select(p => new KeyValuePair<string, (ulong, string)>(p.Key, (ulong.Parse(p.Value[..p.Value.IndexOf('/')]), p.Value[(p.Value.IndexOf('/') + 1)..])))
			.ToFrozenDictionary();

	/// <summary>
	/// Contains a list of Webhook data pairs, indexed by their login names.
	/// </summary>
	public static readonly FrozenDictionary<string, (ulong ID, string Token)> Clients;

	/// <summary>
	/// Interaction Task. Uses information returned from a modal to create a post.
	/// </summary>
	[ComponentInteraction("PostModal")]
	public async Task CreatePost()
	{
		if (Clients.TryGetValue(((TextInput)Context.Components[0]).Value, out (ulong ID, string Token) webhook))
		{
			if (((TextInput)Context.Components[1]).Value.Length < 2000)
			{
				await Client.Rest.ExecuteWebhookAsync(webhook.ID, webhook.Token, new() { Content = ((TextInput)Context.Components[1]).Value });

				await RespondAsync(InteractionCallback.Message(new() { Content = "Post created successfully.", Flags = MessageFlags.Ephemeral }));
			}
			else
			{
				string response = $"""
					Your post was longer than 2000 characters, which is over Discord's character limit. Please shortern it to fit within a message, or split it across multiple.

					Here's most of the characters for your input, so you don't have to write it out again:
					```
					{((TextInput)Context.Components[1]).Value}
					""";

				await RespondAsync(InteractionCallback.Message(new() { Content = $"{response.Remove(1996)}\n```" }));
			}
		}
		else
		{
			await RespondAsync(InteractionCallback.Message(new()
			{
				Content = $"""
				The password `{((TextInput)Context.Components[0]).Value}` doesn't correspond to any known feed. Check your capitalization, numbers, so on.

				PS. Here's your input, so you don't have to write it out again:
				```
				{((TextInput)Context.Components[1]).Value}
				```
				""",
				Flags = MessageFlags.Ephemeral
			}));
		}
	}
}

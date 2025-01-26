using NetCord.Services.ComponentInteractions;
using System.Collections.Frozen;

namespace Bot.Interactions.Helpers;

/// <summary>
/// Contains methods and data involving the <see cref="SlashCommands.GetTitle(string)"/> command.
/// </summary>
public class Library : ComponentInteractionModule<ButtonInteractionContext>
{
	/// <summary>
	/// Precomputes embeds for library entries, sourced from the bot's data folder.
	/// </summary>
	static Library()
	{
		string[] titles, metadata; EmbedProperties[] pages;

		Entries = Directory.EnumerateDirectories(Files.Root + "Library\\").Select(s =>
		{
			metadata = File.ReadAllLines(s + @"\metadata.txt");
			titles = File.ReadAllLines(s + @"\titles.txt");
			pages = new EmbedProperties[titles.Length];

			for (int i = 0; i < pages.Length; i++)
			{
				pages[i] = new() {
					Author = new() { Name = titles[i] },
					Description = File.ReadAllText(s + $@"\{i + 1}.txt"),
					Footer = new() { Text = $"{i + 1} - {metadata[0]}" }
				};
			}

			return new KeyValuePair<string, EmbedProperties[]>(metadata[0], pages);
		}).ToFrozenDictionary();
	}

	/// <summary>
	/// Contains the precomputed embeds for all available library entries.
	/// </summary>
	public static FrozenDictionary<string, EmbedProperties[]> Entries { get; }

	/// <summary>
	/// Interaction Task. Modifies an embed to display the next page of a contained book.
	/// </summary>
	[ComponentInteraction("NextPage")]
	public async Task NextPage()
	{
		int PageNumber = int.Parse(Context.Message.Embeds[0].Footer!.Text[..Context.Message.Embeds[0].Footer!.Text.IndexOf(' ')]);

		EmbedProperties[] Entry = Entries[Context.Message.Embeds[0].Footer!.Text[(Context.Message.Embeds[0].Footer!.Text.IndexOf('-') + 2)..]];

		await RespondAsync(InteractionCallback.ModifyMessage(m => m
			.WithEmbeds([Entry[PageNumber]])
			.WithComponents([new ActionRowProperties((PageNumber + 1) == Entry.Length
			? [new ButtonProperties("LastPage", PageNumber.ToString(), ButtonStyle.Primary)]
			: [new ButtonProperties("LastPage", PageNumber.ToString(), ButtonStyle.Primary), new ButtonProperties("NextPage", (PageNumber + 2).ToString(), ButtonStyle.Primary)]
		)])));
	}

	/// <summary>
	/// Interaction Task. Modifies an embed to display the previous page of a contained book.
	/// </summary>
	[ComponentInteraction("LastPage")]
	public async Task LastPage()
	{
		int PageNumber = int.Parse(Context.Message.Embeds[0].Footer!.Text[..Context.Message.Embeds[0].Footer!.Text.IndexOf(' ')]);

		await RespondAsync(InteractionCallback.ModifyMessage(m => m
			.WithEmbeds([Entries[Context.Message.Embeds[0].Footer!.Text[(Context.Message.Embeds[0].Footer!.Text.IndexOf('-') + 2)..]][PageNumber - 2]])
			.WithComponents([new ActionRowProperties(PageNumber == 2
			? [new ButtonProperties("NextPage", PageNumber.ToString(), ButtonStyle.Primary)]
			: [new ButtonProperties("LastPage", (PageNumber - 2).ToString(), ButtonStyle.Primary), new ButtonProperties("NextPage", PageNumber.ToString(), ButtonStyle.Primary)]
		)])));
	}
}
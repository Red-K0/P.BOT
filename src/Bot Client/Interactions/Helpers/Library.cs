using NetCord.Services.ComponentInteractions;
namespace Bot.Interactions.Helpers;

/// <summary>
/// Contains data involving dictionaries and the <see cref="SlashCommands.GetTitle(string)"/> command.
/// </summary>
public class Library : ComponentInteractionModule<ButtonInteractionContext>
{
	/// <summary>
	/// Precomputes embeds for library entries, sourced from the bot's data folder.
	/// </summary>
	static Library()
	{
		string[] Titles, Metadata;
		EmbedProperties[] Pages;

		foreach (string directory in Directory.EnumerateDirectories(Files.DataPath + "Library\\"))
		{
			Metadata = File.ReadAllLines(directory + @"\metadata.txt");
			Titles = File.ReadAllLines(directory + @"\titles.txt");
			Pages = new EmbedProperties[Titles.Length];

			for (int i = 0; i < Pages.Length; i++)
			{
				Pages[i] = new() {
					Author = new() { Name = Titles[i] },
					Description = File.ReadAllText(directory + $@"\{i + 1}.txt"),
					Footer = new() { Text = $"{i + 1} - {Metadata[0]}" }
				};
			}

			Entries.Add(Metadata[0], Pages);
			return;
		}
	}

	/// <summary>
	/// Contains the precomputed embeds for all available library entries.
	/// </summary>
	public static Dictionary<string, EmbedProperties[]> Entries { get; } = [];

	/// <summary>
	/// WIP
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
	/// WIP
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
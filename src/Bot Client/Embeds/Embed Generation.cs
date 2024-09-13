using static Bot.Caches.Members;

namespace Bot.Backend;

/// <summary>
/// Contains functions for the creation of embeds and their components.
/// </summary>
internal static partial class Embeds
{
	/// <summary>
	/// The URL to the GitHub Assets folder.
	/// </summary>
	private const string ASSETS = "https://raw.githubusercontent.com/Red-K0/P.BOT/master/src/Bot%20Client/Assets/";

	/// <summary>
	/// The hex code for standard embed gray.
	/// </summary>
	public const int STD_COLOR = 0x72767D;

	/// <summary>
	/// Generates a random 24-bit integer from the current time in ticks.
	/// </summary>
	public static Color RandomColor { get => new(Environment.TickCount & 0xFFFFFF); }

	/// <summary> Combines the specified parameters to generate an embed. </summary>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static InteractionMessageProperties Generate(EmbedProperties embed, string?[]? imageURLs = null, string? titleURL = null, bool ephemeral = false)
	{
		InteractionMessageProperties Properties = new()
		{
			Embeds = [embed],
			Flags = ephemeral ? MessageFlags.Ephemeral : null
		};

		if (imageURLs?.Length > 1)
		{
			Properties.AddEmbeds(imageURLs.Skip(1).Take(8).Select(url => new EmbedProperties() { Image = url, Url = titleURL, Color = embed.Color }));
		}

		return Properties;
	}

	/// <summary> Combines the specified parameters with parameters extracted from a given <see cref="RestMessage"/> to generate an embed. </summary>
	/// <param name="message"> The <see cref="RestMessage"/> object to extract parameters from. </param>
	/// <param name="url"> A link to an address of a webpage. When set, the <paramref name="title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="footer"> Contains the <see cref="EmbedFooterProperties"/> to be used in the embed. </param>
	/// <param name="title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="url"/>, has a 256 character limit. </param>
	/// <param name="thumbnail"> The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static InteractionMessageProperties ToEmbed(this RestMessage message, string? url = null, EmbedFooterProperties? footer = null,
	string? title = null, string? thumbnail = null)
	{
		string?[]? URLs = message.Attachments.Count != 0 ? new string[message.Attachments.Count] : null;
		foreach ((Attachment attachment, int i) in message.Attachments.Values.Select((v, i) => (v, i))) URLs![i] = attachment.Url;
		return Generate(CreateProperties(
			message.Content,
			CreateAuthor(message.Author.GetDisplayName(), message.Author.GetAvatar()),
			message.CreatedAt,
			footer,
			URLs?[0],
			thumbnail,
			title,
			url,
			null,
			message.Author.Id
		), URLs);
	}

	/// <summary>
	/// Creates a <see cref="MessageProperties"/> object suitable for <see cref="Commands.SlashCommands.GetTitle(Commands.Helpers.Library.Titles)"/>.
	/// </summary>
	public static InteractionMessageProperties Title(string title, EmbedFieldProperties[] contents, ulong authorId = 0) => new()
	{
		Embeds = [CreateProperties(title: title, fields: contents, refId: authorId)],
	};

	public static EmbedProperties CreateProperties(string? description = null, EmbedAuthorProperties? author = null, DateTimeOffset? timestamp = null,
	EmbedFooterProperties? footer = null, string? image = null, string? thumbnail = null, string? title = null,
	string? url = null, EmbedFieldProperties[]? fields = null, ulong refId = 0, int? color = null) => new()
	{
		Author = author,
		Color = color != null ? new(color.Value) : List.GetValueOrDefault(refId)?.PersonalRole?.Color ?? RandomColor,
		Description = description,
		Footer = footer,
		Image = image,
		Timestamp = timestamp,
		Title = title,
		Thumbnail = new(thumbnail),
		Url = url,
		Fields = fields
	};
}
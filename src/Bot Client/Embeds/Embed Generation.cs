using static PBot.Caches.Members;
namespace PBot;

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

	/// <summary> Combines the specified parameters to generate an embed. </summary>
	/// <param name="description"> The section of the embed where the main text is contained, limited to 4096 characters. </param>
	/// <param name="authorObject"> Contains the <see cref="EmbedAuthorProperties"/> to be used in the embed. </param>
	/// <param name="timestamp"> Displays time in a format similar to a message timestamp. Located next to the <paramref name="footerObject"/>. </param>
	/// <param name="footerObject"> Contains the <see cref="EmbedFooterProperties"/> to be used in the embed. </param>
	/// <param name="RGB"> The hex code indicating an embed's accent color. Defaults to a random value when not set or -1. </param>
	/// <param name="replyTo"> The ID of the message being replied to with the embed. </param>
	/// <param name="imageURLs"> An array of URIs for the attachments included in the embed, displayed below the <paramref name="description"/> element. </param>
	/// <param name="thumbnailURL"> The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed. </param>
	/// <param name="title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="titleURL"/>, has a 256 character limit. </param>
	/// <param name="titleURL"> A link to an address of a webpage. When set, the <paramref name="title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="ephemral"> Creates an ephemeral message when set to true.</param>
	/// <param name="fieldObjects"> Contains an array of <see cref="EmbedField"/>s to include in the embed </param>
	/// <param name="refID"> The ID of the user responsible for the embed's creation. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties Generate(string? description = null, EmbedAuthorProperties? authorObject = null, DateTimeOffset? timestamp = null,
	EmbedFooterProperties? footerObject = null, int? RGB = null, ulong replyTo = 0, string?[]? imageURLs = null, string? thumbnailURL = null, string? title = null,
	string? titleURL = null, bool ephemral = false, EmbedFieldProperties[]? fieldObjects = null, ulong refID = 0)
	{
		MessageProperties msg_prop = new()
		{
			Embeds = [new EmbedProperties()
			{
				Author = authorObject,
				Color = new(RGB ?? ((refID != 0 && List.TryGetValue(refID, out Member? Member)) ? Member?.PersonalRoleColor ?? Environment.TickCount & 0xFFFFFF : Environment.TickCount & 0xFFFFFF)),
				Description = description,
				Footer = footerObject,
				Image = (imageURLs == null) ? null : new(imageURLs[0]),
				Timestamp = timestamp,
				Title = title,
				Thumbnail = new(thumbnailURL),
				Url = titleURL,
				Fields = fieldObjects
			}],
			MessageReference = (replyTo != 0) ? new(replyTo) : null,
			Flags = ephemral ? MessageFlags.Ephemeral : null
		};

		if (imageURLs == null || imageURLs.Length == 1) return msg_prop;

		EmbedProperties[] ExtraAttachments = new EmbedProperties[imageURLs.Length];

		for (int i = 1; i < Math.Clamp(imageURLs.Length, 1, 9); i++)
		{
			ExtraAttachments[i] = new EmbedProperties()
			{
				Image = imageURLs[i],
				Url = titleURL
			};
		}

		return msg_prop.AddEmbeds(ExtraAttachments);
	}

	/// <summary> Combines the specified parameters with parameters extracted from a given <see cref="RestMessage"/> to generate an embed. </summary>
	/// <param name="targetMessage"> The <see cref="RestMessage"/> object to extract parameters from. </param>
	/// <param name="titleURL"> A link to an address of a webpage. When set, the <paramref name="title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="footerObject"> Contains the <see cref="EmbedFooterProperties"/> to be used in the embed. </param>
	/// <param name="replyTo"> The ID of the message being replied to with the embed. </param>
	/// <param name="title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="titleURL"/>, has a 256 character limit. </param>
	/// <param name="thumbnailURL"> The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties Generate(RestMessage targetMessage, string titleURL = "", EmbedFooterProperties? footerObject = null, ulong? replyTo = null,
	string? title = null, string? thumbnailURL = null)
	{
		string?[]? ImageURLs = (targetMessage.Attachments.Count == 0) ? null : new string[targetMessage.Attachments.Count];
		Attachment[] Attachments = [.. targetMessage.Attachments.Values];

		for (int i = 0; i < targetMessage.Attachments.Count; i++) ImageURLs![i] = Attachments[i].Url;

		return Generate(
			targetMessage.Content,
			CreateAuthor(targetMessage.Author.GetDisplayName(), targetMessage.Author.GetAvatar()),
			targetMessage.CreatedAt,
			footerObject,
			-1,
			replyTo ?? 0,
			ImageURLs,
			thumbnailURL,
			title,
			titleURL,
			refID: targetMessage.Author.Id
		);
	}

	/// <summary>
	/// Creates a <see cref="MessageProperties"/> object suitable for <see cref="Commands.SlashCommands.GetTitle(Commands.Helpers.Library.Titles)"/>.
	/// </summary>
	public static MessageProperties Generate(string title, EmbedFieldProperties[] contents, ulong authorID = 0) => new()
	{
		Embeds = [new EmbedProperties()
		{
			Color = new(authorID != 0 ? List[authorID].PersonalRoleColor ?? Environment.TickCount & 0xFFFFFF : Environment.TickCount & 0xFFFFFF),
			Fields = contents,
			Title = title,
		}]
	};
}
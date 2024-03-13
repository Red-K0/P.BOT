using static P_BOT.UserManagement;

namespace P_BOT;

/// <summary>
/// Contains functions for the creation of embeds.
/// </summary>
internal static partial class Embeds
{
	/// <summary> Combines the specified parameters to generate an embed. </summary>
	/// <param name="Description"> The section of the embed where the main text is contained, limited to 4096 characters. </param>
	/// <param name="AuthorObject"> Contains the <see cref="EmbedAuthor"/> information to be used in the embed. </param>
	/// <param name="Timestamp"> Displays time in a format similar to a message timestamp. Located next to the <paramref name="FooterObject"/>. </param>
	/// <param name="FooterObject"> Contains the <see cref="EmbedFooter"/> information to be used in the embed. </param>
	/// <param name="RGB"> The hex code indicating an embed's accent color. Defaults to a random value when not set or -1. </param>
	/// <param name="ReplyTo"> The ID of the message being replied to with the embed. </param>
	/// <param name="ImageURL"> The URL of the image included in the embed, displayed as a large-sized image located below the <paramref name="Description"/> element. </param>
	/// <param name="ThumbnailURL"> The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed. </param>
	/// <param name="Title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="TitleURL"/>, has a 256 character limit. </param>
	/// <param name="TitleURL"> A link to an address of a webpage. When set, the <paramref name="Title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="Ephemeral"> Creates an ephemeral message when set to true.</param>
	/// <param name="FieldObjects"> Contains an array of <see cref="EmbedField"/>s to include in the embed </param>
	/// <param name="CallerID"> The ID of the user responsible for the embed's creation. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties Generate(string? Description = null, EmbedAuthorProperties? AuthorObject = null, DateTimeOffset? Timestamp = null, EmbedFooterProperties? FooterObject = null,int RGB = -1, ulong ReplyTo = 0, string? ImageURL = null, string? ThumbnailURL = null, string? Title = null, string? TitleURL = null, bool Ephemeral = false, EmbedFieldProperties[]? FieldObjects = null, ulong CallerID = 0)
	{
		if (RGB == -1 && CallerID != 0) RGB = UserList[GetIndexOfUser(CallerID)].Customization.PersonalRoleColor;

		EmbedProperties embed_prop = new()
		{
			Author = AuthorObject,
			Color = CreateColorObject(RGB),
			Description = Description,
			Footer = FooterObject,
			Image = new(ImageURL),
			Timestamp = Timestamp,
			Title = Title,
			Thumbnail = new(ThumbnailURL),
			Url = TitleURL,
			Fields = FieldObjects
		};
		return new()
		{
			Embeds = new[] { embed_prop },
			MessageReference = (ReplyTo != 0) ? new(ReplyTo) : null,
			Flags = Ephemeral ? MessageFlags.Ephemeral : null
		};
	}

	/// <summary> Combines the specified parameters with parameters extracted from a given <see cref="RestMessage"/> to generate an embed. </summary>
	/// <param name="TargetMessage"> The <see cref="RestMessage"/> object to extract parameters from. </param>
	/// <param name="TitleURL"> A link to an address of a webpage. When set, the <paramref name="Title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="Footer"> The text at the bottom of the embed, limited to 2048 characters. </param>
	/// <param name="FooterIconURL"> The URL of the image to display in the footer block, to the left of the <paramref name="Footer"/> text. </param>
	/// <param name="ReplyTo"> The ID of the message being replied to with the embed. </param>
	/// <param name="Title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="TitleURL"/>, has a 256 character limit. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties Generate(RestMessage TargetMessage, string TitleURL, string? Footer = null, string? FooterIconURL = null, ulong? ReplyTo = null, string? Title = null)
	{
		MessageProperties msg_prop = new();
		for (int i = 0; i < Math.Clamp(TargetMessage.Attachments.Count, 1, 10); i++)
		{
			_ = msg_prop.AddEmbeds(Generate(
			TargetMessage.Content,
			CreateAuthorObject(TargetMessage.Author.Username, TargetMessage.Author.GetAvatarUrl().ToString()),
			TargetMessage.CreatedAt,
			CreateFooterObject(Footer, FooterIconURL),
			ReplyTo: ReplyTo ?? 0,
			ImageURL: TargetMessage.Attachments.Any() ? TargetMessage.Attachments.Values.ToArray()[i].Url : null,
			Title: Title,
			TitleURL: TitleURL,
			CallerID: TargetMessage.Author.Id
			).Embeds!);
		}

		if (msg_prop.Embeds!.Count() > 10)
		{
			msg_prop.Embeds = msg_prop.Embeds!.Take(10);
		}

		return msg_prop;
	}
}
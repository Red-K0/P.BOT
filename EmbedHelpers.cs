﻿using static P_BOT.EmbedComponent;
namespace P_BOT;

/// <summary> Contains functions for the creation of embeds. </summary>
internal static class EmbedHelpers
{
	/// <summary>
	/// Combines the specified parameters into an embed. You can only have up to 10 embeds per message, and the total text of all embeds must be less than or equal to 6000 characters.
	/// </summary>
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
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties CreateEmbed(
	string? Description = null, EmbedAuthorProperties? AuthorObject = null, DateTimeOffset? Timestamp = null, EmbedFooterProperties? FooterObject = null,
	int RGB = -1, ulong ReplyTo = 0, string? ImageURL = null, string? ThumbnailURL = null, string? Title = null,
	string? TitleURL = null, bool Ephemeral = false)
	{
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
			Url = TitleURL
		};
		MessageProperties msg_prop = new()
		{
			Embeds = new[] { embed_prop },
			MessageReference = (ReplyTo != 0) ? new(ReplyTo) : null,
			Flags = Ephemeral ? MessageFlags.Ephemeral : null
		};

		return msg_prop;
	}

	public static MessageProperties CeateEmbed(
	string? Description = null, EmbedAuthorProperties? AuthorObject = null, DateTimeOffset? Timestamp = null, EmbedFooterProperties? FooterObject = null,
	int RGB = -1, ulong ReplyTo = 0, string? ImageURL = null, string? ThumbnailURL = null, string? Title = null,
	string? TitleURL = null, bool Ephemeral = false)
	{
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
			Url = TitleURL
		};
		MessageProperties msg_prop = new()
		{
			Embeds = new[] { embed_prop },
			MessageReference = (ReplyTo != 0) ? new(ReplyTo) : null,
			Flags = Ephemeral ? MessageFlags.Ephemeral : null
		};

		return msg_prop;
	}


	/// <summary>
	/// Similar to <see cref="CreateEmbed(string?, EmbedAuthorProperties?, DateTimeOffset?, EmbedFooterProperties?, int, ulong, string?, string?, string?, string?, bool)"/>, instead filling in most parameters automatically from a given <see cref="RestMessage"/>.
	/// </summary>
	/// <param name="TargetMessage"> The <see cref="RestMessage"/> object to extract properties from. </param>
	/// <param name="TitleURL"> A link to an address of a webpage. When set, the <paramref name="Title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="Footer"> The text at the bottom of the embed, limited to 2048 characters. </param>
	/// <param name="FooterIconURL"> The URL of the image to display in the footer block, to the left of the <paramref name="Footer"/> text. </param>
	/// <param name="ReplyTo"> The ID of the message being replied to with the embed. </param>
	/// <param name="Title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="TitleURL"/>, has a 256 character limit. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties ToEmbed( RestMessage TargetMessage, string TitleURL, string? Footer = null, string? FooterIconURL = null, ulong? ReplyTo = null, string? Title = null)
	{
		MessageProperties msg_prop = new();
		for (int i = 0; i < Math.Clamp(TargetMessage.Attachments.Count, 1, 10); i++)
		{
			_ = msg_prop.AddEmbeds(CreateEmbed(
			TargetMessage.Content,
			CreateAuthorObject(TargetMessage.Author.Username, TargetMessage.Author.GetAvatarUrl().ToString()),
			TargetMessage.CreatedAt,
			CreateFooterObject(Footer, FooterIconURL),
			ReplyTo: ReplyTo ?? 0,
			ImageURL: TargetMessage.Attachments.Any() ? TargetMessage.Attachments.Values.ToArray()[i].Url : null,
			Title: Title,
			TitleURL: TitleURL
			).Embeds!);
		}
		return msg_prop;
	}
}
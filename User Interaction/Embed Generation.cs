﻿using static P_BOT.Members;
namespace P_BOT;

/// <summary>
/// Contains functions for the creation of embeds and their components.
/// </summary>
internal static partial class Embeds
{
	/// <summary>
	/// The URL to the GitHub Assets folder.
	/// </summary>
	private const string ASSETS = "https://raw.githubusercontent.com/Red-K0/P.BOT/master/Assets/";

	/// <summary>
	/// The hex code for standard embed gray.
	/// </summary>
	public const int STD_COLOR = 0x72767D;

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
	/// <param name="RefID"> The ID of the user responsible for the embed's creation. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties Generate(string? Description = null, EmbedAuthorProperties? AuthorObject = null, DateTimeOffset? Timestamp = null,
	EmbedFooterProperties? FooterObject = null, int RGB = -1, ulong ReplyTo = 0, string? ImageURL = null, string? ThumbnailURL = null, string? Title = null,
	string? TitleURL = null, bool Ephemeral = false, EmbedFieldProperties[]? FieldObjects = null, ulong RefID = 0) => new()
	{
		Embeds = [(new EmbedProperties()
		{
			Author = AuthorObject,
			Color = new((RGB == -1 && RefID != 0) ? List.GetValueOrDefault(RefID).Customization.PersonalRoleColor : (RGB == -1) ? Environment.TickCount & 0xFFFFFF : RGB),
			Description = Description,
			Footer = FooterObject,
			Image = new(ImageURL),
			Timestamp = Timestamp,
			Title = Title,
			Thumbnail = new(ThumbnailURL),
			Url = TitleURL,
			Fields = FieldObjects
		})],
		MessageReference = (ReplyTo != 0) ? new(ReplyTo) : null,
		Flags = Ephemeral ? MessageFlags.Ephemeral : null
	};

	/// <summary> Combines the specified parameters with parameters extracted from a given <see cref="RestMessage"/> to generate an embed. </summary>
	/// <param name="TargetMessage"> The <see cref="RestMessage"/> object to extract parameters from. </param>
	/// <param name="TitleURL"> A link to an address of a webpage. When set, the <paramref name="Title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="Footer"> The text at the bottom of the embed, limited to 2048 characters. </param>
	/// <param name="FooterIconURL"> The URL of the image to display in the footer block, to the left of the <paramref name="Footer"/> text. </param>
	/// <param name="ReplyTo"> The ID of the message being replied to with the embed. </param>
	/// <param name="Title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="TitleURL"/>, has a 256 character limit. </param>
	/// <param name="ThumbnailURL"> The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static MessageProperties Generate(RestMessage TargetMessage, string TitleURL, string? Footer = null, string? FooterIconURL = null,
	ulong? ReplyTo = null, string? Title = null, string? ThumbnailURL = null)
	{
		MessageProperties msg_prop = new();
		for (int i = 0; i < Math.Clamp(TargetMessage.Attachments.Count, 1, 10); i++)
		{
			_ = msg_prop.AddEmbeds(Generate(
			TargetMessage.Content,
			CreateAuthorObject(TargetMessage.Author.Username, TargetMessage.Author.GetAvatarUrl().ToString()),
			TargetMessage.CreatedAt,
			CreateFooterObject(Footer, FooterIconURL),
			-1,
			ReplyTo ?? 0,
			TargetMessage.Attachments.Any() ? TargetMessage.Attachments.Values.ToArray()[i].Url : null,
			ThumbnailURL,
			Title,
			TitleURL,
			RefID: TargetMessage.Author.Id
			).Embeds!);
		}

		if (msg_prop.Embeds!.Count() > 10)
		{
			msg_prop.Embeds = msg_prop.Embeds!.Take(10);
		}

		return msg_prop;
	}
}
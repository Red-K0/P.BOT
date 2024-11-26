﻿namespace Bot.Interactions;
internal static class Generation
{
	/// <summary> Combines the specified parameters with parameters extracted from a given <see cref="RestMessage"/> to generate an embed. </summary>
	/// <param name="message"> The <see cref="RestMessage"/> object to extract parameters from. </param>
	/// <param name="url"> A link to an address of a webpage. When set, the <paramref name="title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped. </param>
	/// <param name="footer"> Contains the <see cref="EmbedFooterProperties"/> to be used in the embed. </param>
	/// <param name="thumbnail"> The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed. </param>
	/// <param name="title"> The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="url"/>, has a 256 character limit. </param>
	/// <returns> <see cref="MessageProperties"/> containing the created embed. </returns>
	public static IEnumerable<EmbedProperties> ToEmbedSet(this RestMessage message, string? url = null, EmbedFooterProperties? footer = null, string? thumbnail = null, string? title = null)
	{
		string?[] URLs = message.Attachments.Count != 0 ? new string[message.Attachments.Count] : [];

		for (int i = 0; i < URLs.Length; i++) URLs[i] = message.Attachments.Values.ElementAt(i).Url;

		return Embeds.CreateImageSet((url!, URLs[1..]), message.Author.Id).Prepend(new()
		{
			Author = new()
			{
				Name = message.Author.GetDisplayName(),
				IconUrl = message.Author.GetAvatar()
			},
			Color = Common.GetColor(message.Author.Id),
			Description = message.Content,
			Fields = null,
			Footer = footer,
			Image = URLs.Length == 0 ? null : URLs[0],
			Thumbnail = thumbnail,
			Timestamp = message.CreatedAt,
			Title = title,
			Url = url,
		});
	}
}
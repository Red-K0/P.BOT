namespace Bot.Interactions;

/// <summary>
/// Contains methods for standardizing embed generation across the codebase.
/// </summary>
internal static class Generation
{
	/// <summary>
	/// Combines the specified parameters with parameters extracted from a given <see cref="RestMessage"/> to generate an embed.
	/// </summary>
	/// <param name="message">The <see cref="RestMessage"/> object to extract parameters from.</param>
	/// <param name="url">A link to an address of a webpage. When set, the <paramref name="title"/> becomes a clickable link, directing to the URL. Additionally, embeds of the same URL are grouped.</param>
	/// <param name="footer">Contains the <see cref="EmbedFooterProperties"/> to be used in the embed.</param>
	/// <param name="thumbnail">The URL of thumbnail of the embed, displayed as a medium-sized image in the top right corner of the embed.</param>
	/// <param name="title">The text that is placed above the description, usually highlighted. Also directs to a URL if one is given in <paramref name="url"/>, has a 256 character limit.</param>
	/// <param name="censor">If the message contents should be censored.</param>
	/// <returns><see cref="MessageProperties"/> containing the created embed.</returns>
	public static IEnumerable<EmbedProperties> ToEmbedSet(this RestMessage message, string? url = null, EmbedFooterProperties? footer = null, string? thumbnail = null,
	string? title = null, bool censor = false) => Embeds.CreateImageSet(url, message.Attachments.GetImageURLs().Skip(1), message.Author.Id).Prepend(new()
	{
		Author = new()
		{
			Name = message.Author.GetDisplayName(),
			IconUrl = message.Author.GetAvatar()
		},
		Color = Common.GetColor(message.Author.Id),
		Description = string.Concat(message.Embeds.Select(p => $"{p.Description}\n\n")) + ((censor && message.Content != "")
			? $"||{message.Content.Replace("|", "\\|")}||"
			: message.Content),
		Fields = null,
		Footer = footer,
		Image = message.Attachments.Count != 0 ? message.Attachments[0].Url : null,
		Thumbnail = thumbnail,
		Timestamp = message.CreatedAt,
		Title = title,
		Url = url,
	});
}
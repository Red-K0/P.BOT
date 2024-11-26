namespace Bot.Interactions;
internal static class Embeds
{
	public static IEnumerable<EmbedProperties> CreateImageSet((string? baseURL, IEnumerable<string?> URLs) images, ulong id = 0)
	{
		Color Color = Common.GetColor(id);

		List<EmbedProperties> Embeds = [];
		foreach (string? url in images.URLs)
		{
			if (url == null) continue;

			Embeds.Add(new() { Image = url, Url = images.baseURL, Color = Color });
		}

		return Embeds;
	}
}

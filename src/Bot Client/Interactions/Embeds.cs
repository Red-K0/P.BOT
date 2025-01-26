namespace Bot.Interactions;

/// <summary>
/// Contains methods for assisting in the creation of embeds.
/// </summary>
internal static class Embeds
{
	/// <summary>
	/// Creates a set of <see cref="EmbedProperties"/> based off a set of URLs, that show up as combined images when embedded.
	/// </summary>
	/// <param name="baseUrl">The base URL to apply to every embed in the set.</param>
	/// <param name="urls">The set of URLs to process into image embeds.</param>
	/// <param name="id">The user to base the set's color on.</param>
	public static IEnumerable<EmbedProperties> CreateImageSet(string? baseUrl, IEnumerable<string?> urls, ulong id = 0)
	{
		Color Color = Common.GetColor(id);

		return urls
			.Where(u => u != null)
			.Select(u => new EmbedProperties() { Image = u, Url = baseUrl, Color = Color });
	}
}

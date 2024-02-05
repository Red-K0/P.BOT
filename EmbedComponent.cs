namespace P_BOT;

/// <summary> Contains methods to contain and simplify the creation of <see cref="Embed"/> components. </summary>
internal static class EmbedComponent
{
	#region Color Object
	/// <summary> Creates a <see cref="Color"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="R"> A <see cref="byte"/> value, specifying the red component of the color.</param>
	/// <param name="G"> A <see cref="byte"/> value, specifying the green component of the color.</param>
	/// <param name="B"> A <see cref="byte"/> value, specifying the blue component of the color.</param>
	public static Color CreateColorObject(byte R, byte G, byte B) => new(R, G, B);

	/// <summary> Creates a <see cref="Color"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="RGB"> An <see cref="int"/> value, bounded between <c>0</c> and <c>0xFFFFFF</c>. When set to <c>-1</c>, randomizes the color. </param>
	public static Color CreateColorObject(int RGB = -1)
	{
		if (RGB == -1)
		{
			RGB = FastRandom.Next24(Environment.TickCount);
		}

		// RGB Value Bounding
		RGB = Math.Max(0x000000, RGB);
		RGB = Math.Min(0xFFFFFF, RGB);
		return new(RGB);
	}
	#endregion

	/// <summary> Creates an <see cref="EmbedAuthorProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="AuthorText"> The <see cref="string"/> to display, used for the <see cref="EmbedAuthorProperties.Name"/> property. </param>
	/// <param name="IconURL"> The <see cref="string"/> containing the URL of the image to display to the left of the text, used for the <see cref="EmbedAuthorProperties.IconUrl"/> property. </param>
	/// <param name="TextURL"> The <see cref="string"/> containing the URL to set <paramref name="AuthorText"/> as a hyperlink towards, used for the <see cref="EmbedAuthorProperties.Url"/> property.</param>
	/// <returns></returns>
	public static EmbedAuthorProperties CreateAuthorObject(string? AuthorText = null, string? IconURL = null, string? TextURL = null) => new()
	{
		Name = AuthorText,
		IconUrl = IconURL,
		Url = TextURL
	};

	/// <summary> Creates an <see cref="EmbedAuthorProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="FooterText"> The <see cref="string"/> to display, used for the <see cref="EmbedFooterProperties.Text"/> property. </param>
	/// <param name="IconURL"> The <see cref="string"/> containing the URL of the image to display to the left of the text, used for the <see cref="EmbedFooterProperties.IconUrl"/> property. </param>
	public static EmbedFooterProperties CreateFooterObject(string? FooterText = null, string? IconURL = null) => new()
	{
		Text = FooterText,
		IconUrl = IconURL,
	};
}
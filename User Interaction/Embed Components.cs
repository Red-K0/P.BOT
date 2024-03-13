namespace P_BOT;

/// <summary>
/// Contains methods to contain and simplify the creation of <see cref="Embed"/> components.
/// </summary>
internal static partial class Embeds
{
	/// <summary> Creates an <see cref="EmbedAuthorProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="AuthorText"> The <see cref="string"/> to display, used for the <see cref="EmbedAuthorProperties.Name"/> property. </param>
	/// <param name="IconURL"> The <see cref="string"/> containing the URL of the image to display to the left of the text, used for the <see cref="EmbedAuthorProperties.IconUrl"/> property. </param>
	/// <param name="TextURL"> The <see cref="string"/> containing the URL to set <paramref name="AuthorText"/> as a hyperlink towards, used for the <see cref="EmbedAuthorProperties.Url"/> property.</param>
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

	/// <summary> Creates an <see cref="EmbedFieldProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="Name"> The <see cref="string"/> to display at the field title, used for the <see cref="EmbedFieldProperties.Name"/> property. </param>
	/// <param name="Value"> The <see cref="string"/> to display in the field, used for the <see cref="EmbedFieldProperties.Value"/> property. </param>
	/// <param name="Inline"> Whether or not to display the field inline. </param>
	public static EmbedFieldProperties CreateFieldObject(string? Name = null, string? Value = null, bool Inline = false) => new()
	{
		Inline = Inline,
		Name = Name,
		Value = Value
	};
}
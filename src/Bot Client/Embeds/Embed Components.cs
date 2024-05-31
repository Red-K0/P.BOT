namespace PBot;

internal static partial class Embeds
{
	/// <summary> Creates an <see cref="EmbedAuthorProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="authorText"> The <see cref="string"/> to display, used for the <see cref="EmbedAuthorProperties.Name"/> property. </param>
	/// <param name="iconURL"> The <see cref="string"/> containing the URL of the image to display to the left of the text, used for the <see cref="EmbedAuthorProperties.IconUrl"/> property. </param>
	/// <param name="textURL"> The <see cref="string"/> containing the URL to set <paramref name="authorText"/> as a hyperlink towards, used for the <see cref="EmbedAuthorProperties.Url"/> property.</param>
	public static EmbedAuthorProperties CreateAuthor(string? authorText = null, string? iconURL = null, string? textURL = null) => new()
	{
		Name = authorText,
		IconUrl = iconURL,
		Url = textURL
	};

	/// <summary> Creates an <see cref="EmbedAuthorProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="footerText"> The <see cref="string"/> to display, used for the <see cref="EmbedFooterProperties.Text"/> property. </param>
	/// <param name="iconURL"> The <see cref="string"/> containing the URL of the image to display to the left of the text, used for the <see cref="EmbedFooterProperties.IconUrl"/> property. </param>
	public static EmbedFooterProperties CreateFooter(string? footerText = null, string? iconURL = null) => new()
	{
		Text = footerText,
		IconUrl = iconURL,
	};

	/// <summary> Creates an <see cref="EmbedFieldProperties"/> object for use in an <see cref="Embed"/>. </summary>
	/// <param name="name"> The <see cref="string"/> to display at the field title, used for the <see cref="EmbedFieldProperties.Name"/> property. </param>
	/// <param name="value"> The <see cref="string"/> to display in the field, used for the <see cref="EmbedFieldProperties.Value"/> property. </param>
	/// <param name="noInline"> Whether or not to display the field inline. </param>
	public static EmbedFieldProperties CreateField(string? name = null, string? value = null, bool noInline = false) => new()
	{
		Inline = !noInline,
		Name = name,
		Value = value
	};

	/// <summary>
	/// Gets the GitHub hosted URL of the given asset.
	/// </summary>
	/// <param name="fileName"> The name of the hosted file. </param>
	public static string GetAssetURL(string fileName) => ASSETS + fileName.Replace(" ", "%20");
}

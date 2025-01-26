namespace Bot.Interactions;
internal static class Common
{
	/// <summary>
	/// The URL to the bot's assets folder on GitHub.
	/// </summary>
	private const string _root = "https://raw.githubusercontent.com/Red-K0/P.BOT/refs/heads/master/assets/";

	/// <summary>
	/// The hex code for the bot's default color code.
	/// </summary>
	public const int DefaultColor = 0x72767D;

	/// <summary>
	/// Generates a 24-bit integer from the current time in ticks.
	/// </summary>
	public static Color RandomColor => new(Environment.TickCount & 0xFFFFFF);

	/// <summary>
	/// Gets the GitHub hosted URL of the given asset.
	/// </summary>
	/// <param name="fileName">The name of the hosted asset.</param>
	public static string GetAssetURL(string fileName) => _root + Uri.EscapeDataString(fileName).Replace("%2F", "/");

	/// <summary>
	/// Gets the personal color of the user associated with the given ID, returning a random color if no personal color is found.
	/// </summary>
	/// <param name="id">The ID of the user to look up a color for.</param>
	public static Color GetColor(ulong id) => Members.MemberList.GetValueOrDefault(id)?.PersonalRole?.Color ?? RandomColor;
}

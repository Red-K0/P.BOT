namespace Bot.Interactions;
internal static class Common
{
	/// <summary>
	/// The URL to the GitHub Assets folder.
	/// </summary>
	private const string ASSETS = "https://raw.githubusercontent.com/Red-K0/P.BOT/refs/heads/master/assets/";

	/// <summary>
	/// The hex code for standard embed gray.
	/// </summary>
	public const int STD_COLOR = 0x72767D;

	/// <summary>
	/// Generates a random 24-bit integer from the current time in ticks.
	/// </summary>
	public static Color RandomColor { get => new(Environment.TickCount & 0xFFFFFF); }

	/// <summary>
	/// Gets the GitHub hosted URL of the given asset.
	/// </summary>
	/// <param name="fileName"> The name of the hosted file. </param>
	public static string GetAssetURL(string fileName) => ASSETS + Uri.EscapeDataString(fileName).Replace("%2F", "/");

	/// <summary>
	/// Gets the color associated with a user based on their ID, returning a random color if not associated.
	/// </summary>
	/// <param name="id">The ID of the user to lookup a color for.</param>
	public static Color GetColor(ulong id) => Members.MemberList.GetValueOrDefault(id)?.PersonalRole?.Color ?? RandomColor;
}

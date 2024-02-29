namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains methods and variables for storage and modification of bot settings.
/// </summary>
public static class Options
{
	/// <summary>
	/// A list of optional modules used by P.BOT.
	/// </summary>
	public enum Modules
	{
		/// <summary>
		/// The module responsible for rolling dice, as well as message parsing related to so.
		/// </summary>
		DnDTextModule
	}
	/// <summary>
	/// The current status of the module.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2211:Non-constant fields should not be visible", Justification = "<Pending>")]
	public static bool DnDTextModule;
}
// The helper class supporting the .r* commands.

using System.Globalization;
using System.Text;
namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains constants, variables and the function responsible for probability calculations and roll results.
/// </summary>
public static class ProbabilityStateMachine
{
	#region Constants
	/// <summary>
	/// The format string to pass for the error message in this module.
	/// </summary>
	private static readonly CompositeFormat ERROR_MESSAGE = CompositeFormat.Parse("""
	Sorry, the parameters for your roll '{0}' couldn't be parsed, please input your roll in the format `.[Prefix] [Number]d[Number] [Number]`, where:
	- The prefix is one of rl, rk, or rm for a low-detail roll, high-detail roll, or roll analysis, respectively.
	- The first 'Number' is the number of die to roll
	- The second is the number of faces per die
	- The third is the modifier to place on each roll
	For example:
	```
	.rl 4d20	  -	Rolls 4 die with 20 sides
	.rk 4d4 1	 -	Rolls 4 die with 4 sides, adding a 1 to each, giving a detailed result
	.rm 4d5 -2	-	Computes 4 die with 5 sides, subtracting a 2 from each, and displaying probabilities
	```
	""");

	/// <summary>
	/// The string to set <see cref="Prefix"/> to, in order to trigger an error report.
	/// </summary>
	private const string ERROR_TRIGGER = "An error occurred during roll processing.";

	/// <summary>
	/// The literal used as a separator in the nXn roll format.
	/// </summary>
	private const char ROLL_SEPERATOR = 'd';

	/// <summary>
	/// <c>U+0020 SPACE [SP]</c>
	/// </summary>
	private const char SPACE_CHAR = ' ';

	/// <summary>
	/// The prefix used to trigger a low-detail roll.
	/// </summary>
	private const string PREFIX_LDETAIL = "rl ";

	/// <summary>
	/// The prefix used to trigger a high-detail roll.
	/// </summary>
	private const string PREFIX_HDETAIL = "rk ";

	/// <summary>
	/// The prefix used to trigger a roll analysis.
	/// </summary>
	private const string PREFIX_ANALYZE = "rm ";
	#endregion

	#region Fields
	/// <summary> The instance of <see cref="Random"/> used for rolling die. </summary>
	private static readonly Random RNG = new();

	/// <summary> The message currently being processed. </summary>
	private static RestMessage? CurrentMessage;

	/// <summary> The string containing the prefix for the current roll type. </summary>
	private static string Prefix = "";

	/// <summary> The temporary string variable, used in extra operations. </summary>
	private static string TempString = "";

	/// <summary> The main string variable, used in main operations. </summary>
	private static string MainString = "";

	/// <summary> The array which holds the results of individual rolls. </summary>
	private static int[] Rolls = [];

	/// <summary> Stores how many rolls resulted in their maximum value. </summary>
	private static int JesusCount;

	/// <summary> Stores how many times to roll the die. </summary>
	private static int RollCount;

	/// <summary> Stores the number of faces the die possesses. </summary>
	private static int FaceCount;

	/// <summary> Stores how many rolls resulted in their minimum value. </summary>
	private static int MinCount;

	/// <summary> Stores the sum of all rolls. </summary>
	private static int Result;

	/// <summary> Stores the current roll modifier. </summary>
	private static int Mod;
	#endregion

	/// <summary> Applies the appropriate logic to use based on the given <paramref name="message"/>. </summary>
	/// <param name="message"> The message to operate with. </param>
	public static async void Run(Message message)
	{
		try
		{
			CurrentMessage = message;

			// Temporary variable assignments, these don't mean anything to the final result, optimization.
			Prefix = CurrentMessage.Content[1..3].ToLowerInvariant();
			MainString = CurrentMessage!.Content.Trim()[4..];
			TempString = MainString[(Mod + 1)..];
			Mod = MainString.IndexOf(ROLL_SEPERATOR);

			// Parse how many die to roll, and prepare the appropriate array size.
			RollCount = int.Parse(MainString[..Mod]);
			Rolls = new int[RollCount];

			// Check for presence of roll modifier at NdN X<---
			if (TempString.Contains(SPACE_CHAR))
			{
				Mod = int.Parse(MainString[(MainString.IndexOf(SPACE_CHAR) + 1)..]);
				MainString = TempString.Remove(TempString.IndexOf(SPACE_CHAR));
			}
			else
			{
				MainString = MainString[(Mod + 1)..];
			}

			// Parse how many faces the die should have, and prepare MainString to accept the output values.
			FaceCount = int.Parse(MainString);
			MainString = "Rolled `";

			// Dice rolling loop
			for (int i = 0; i < RollCount; i++)
			{
				Rolls[i] = RNG.Next(FaceCount - 1) + 1 + Mod;
				if (Rolls[i] != FaceCount + Mod && Rolls[i] != 1 + Mod)
				{
					// If the roll is a normal roll. (Not maximum or 1 + Mod)
					MainString += $"{Rolls[i]}, ";
				}
				else if (Rolls[i] != 1 + Mod)
				{
					// If the roll is a critical (Not 1 + Mod)
					MainString += (Prefix == PREFIX_HDETAIL) ? $"{Rolls[i]}*, " : $"**{Rolls[i]}**, ";
					JesusCount++;
				}
				else
				{
					// If the roll is a failure (Failure of other cases)
					MainString += (Prefix == PREFIX_HDETAIL) ? $"{Rolls[i]}!, " : $"~~{Rolls[i]}~~, ";
					MinCount++;
				}
			}
			Result = Rolls.Sum();
		}
		catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
		{
			// Stores Prefix in TempString, and sets Prefix to a value guaranteed to throw the report	 handler.
			TempString = Prefix;
			Prefix = ERROR_TRIGGER;
		}

		switch (Prefix)
		{
			default:
				if (Prefix == ERROR_TRIGGER) Prefix = TempString; // Restores Prefix to its original value.
				MessageProperties response = new()
				{
					MessageReference = new(CurrentMessage!.Id),
					Content = string.Format(CultureInfo.InvariantCulture, ERROR_MESSAGE, CurrentMessage.Content, Prefix)
				};
				await client.Rest.SendMessageAsync(CurrentMessage!.ChannelId, response);
				break;

			case PREFIX_LDETAIL:
				MainString = MainString.Replace("`", "||");
				await CurrentMessage!.ReplyAsync($"{MainString[..^2]}|| with a total of: {Result}");
				break;

			case PREFIX_HDETAIL or PREFIX_ANALYZE:
				MainString = (Prefix == PREFIX_HDETAIL)
				?  $"""
				{MainString[..^2]}` with a total of: {Result}
				
				Roll Data:
				- Number of Max Value Rolls: {JesusCount}
				- Number of Min Value Rolls: {MinCount}
				- Average Value of All Rolls: {Rolls.Average()}

				"""
				:"";

				MainString += $"""
				Roll Analysis:
				- Maximum Possible Roll: {(FaceCount + Mod) * RollCount}
				- Minimum Possible Roll: {(1 + Mod) * RollCount}
				- Chance of Perfect Roll: {(1 / Math.Pow(FaceCount, RollCount) * 100).ToString("0." + new string('#', 24))}%
				""";

				await CurrentMessage!.ReplyAsync(MainString);
				break;
		}

		// Reset all values to prepare for next call.
		JesusCount = RollCount = FaceCount = MinCount = Result = Mod = default;
		Prefix = TempString = MainString = "";
		Rolls = [];
	}
}
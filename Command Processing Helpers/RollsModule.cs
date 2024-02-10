using System.Globalization;
namespace P_BOT.Command_Processing.Helpers;

/// <summary> Contains methods and variables used for dice probability calculations and roll result output. </summary>
internal static class RollsModule
{
	#region Constants
	/// <summary> The literal used as a separator in the nXn roll format. </summary>
	private const char ROLL_SEPERATOR = 'd';

	/// <summary> <c>U+0020 SPACE [SP]</c> </summary>
	private const char SPACE_CHAR = ' ';

	/// <summary> The prefix used to trigger a basic roll using <see cref="BasicDieLogic()"/>. </summary>
	private const string ROLLPREFIX_BASIC = "D ";

	/// <summary> The prefix used to trigger an advanced roll using <see cref="AdvancedDieLogic()"/>. </summary>
	private const string ROLLPREFIX_ADVANCED = "DX ";

	/// <summary> The prefix used to trigger a roll analysis using <see cref="AnalyticalDieLogic()"/>. </summary>
	private const string ROLLPREFIX_ANALYSIS = "CX ";
	#endregion

	#region Variables

	/// <summary> The instance of <see cref="Random"/> used for rolling die. </summary>
	private static readonly Random RNG = new();

	/// <summary> The message currently being processed. </summary>
	private static RestMessage? CurrentMessage;

	/// <summary> The string that contains the results to output. </summary>
	private static string Output = "";

	/// <summary> The temporary string variable, used in extra operations. </summary>
	private static string Temp = "";

	/// <summary> The main string variable, used in main operations. </summary>
	private static string str = "";

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

	#region Methods

	/// <summary> Resets all variables to an initialized state. </summary>
	private static void Reset()
	{
		JesusCount = 0;
		RollCount = 0;
		FaceCount = 0;
		Rolls = [];
		MinCount = 0;
		Output = "";
		Result = 0;
		Temp = "";
		str = "";
		Mod = 0;
	}

	/// <summary> Determines which set of logic to use based on the given <paramref name="message"/>. </summary>
	/// <param name="message"> The message to use as a selector. </param>
	public static void LogicSelect(in Message message)
	{
		CurrentMessage = message;
		str = message.Content[1..].Trim().ToUpperInvariant();

		//Basic roll
		if (str.StartsWith(ROLLPREFIX_BASIC))
		{
			BasicDieLogic();
		}

		//Complex roll
		else
		if (str.StartsWith(ROLLPREFIX_ADVANCED))
		{
			AdvancedDieLogic();
		}

		//Check roll
		else
		if (str.StartsWith(ROLLPREFIX_ANALYSIS))
		{
			AnalyticalDieLogic();
		}
	}

	/// <summary> Prints the output of a roll. </summary>
	/// <param name="Detailed"> Whether the print was the result of a call to <see cref="AdvancedDieLogic()"/> or not. </param>
	private static void PrintOutput(bool Detailed)
	{
		_ = Detailed
			? CurrentMessage!.ReplyAsync($"""
				{Output[..^2]}` with a total of: {Result}
					
				Roll Data:
				- Number of Max Value Rolls: {JesusCount}
				- Number of Min Value Rolls: {MinCount}
				- Average Value of All Rolls: {Rolls.Average()}

				Statistical Data:
				- Maximum Possible Roll: {(FaceCount + Mod) * RollCount}
				- Minimum Possible Roll: {(1 + Mod) * RollCount}
				- Chance of Perfect Roll: {(1 / Math.Pow(FaceCount, RollCount) * 100).ToString("0." + new string('#', 24))}%
				""")
			: CurrentMessage!.ReplyAsync($"""
				Roll Analysis:
				- Maximum Possible Roll: {(FaceCount + Mod) * RollCount}
				- Minimum Possible Roll: {(1 + Mod) * RollCount}
				- Chance of Perfect Roll: {(1 / Math.Pow(FaceCount, RollCount) * 100).ToString("0." + new string('#', 24))}%
				""");
	}

	#endregion

	#region Roll Logic

	/// <summary> Performs a dice roll operation based on the given parameters and responds with a result. Does not call <see cref="PrintOutput(bool)"/>. </summary>
	private static void BasicDieLogic()
	{
		try
		{
			str = CurrentMessage!.Content[1..].Trim()[2..];
			Temp = str[(str.IndexOf(ROLL_SEPERATOR) + 1)..];
			RollCount = int.Parse(str[..str.IndexOf(ROLL_SEPERATOR)]);

			if (Temp.Contains(SPACE_CHAR))
			{
				Mod = int.Parse(str[(str.IndexOf(SPACE_CHAR) + 1)..]);
				str = Temp.Remove(Temp.IndexOf(SPACE_CHAR));
			}
			else
			{
				str = str[(str.IndexOf(ROLL_SEPERATOR) + 1)..];
			}

			FaceCount = int.Parse(str); RollDie(false);
			Output = Output.Replace("`", "||");
			_ = CurrentMessage!.ReplyAsync($"{Output[..^2]}|| with a total of: {Result}"); Reset();
		}
		catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
		{
			MessageProperties response = new()
			{
				MessageReference = new(CurrentMessage!.Id),
				Content = string.Format(CultureInfo.InvariantCulture, ERROR_DND_FORMAT, CurrentMessage.Content, "dx")
			};
			_ = client.Rest.SendMessageAsync(CurrentMessage!.ChannelId, response);
		}
		finally
		{
			Reset();
		}
	}

	/// <summary> Performs a dice roll operation based on the given parameters and responds with a result, providing additional statistics about the roll as well. </summary>
	private static void AdvancedDieLogic()
	{
		try
		{
			str = CurrentMessage!.Content[1..].Trim()[3..];
			Temp = str[(str.IndexOf(ROLL_SEPERATOR) + 1)..];
			RollCount = int.Parse(str[..str.IndexOf(ROLL_SEPERATOR)]);

			if (Temp.Contains(SPACE_CHAR))
			{
				Mod = int.Parse(str[(str.IndexOf(SPACE_CHAR) + 1)..]);
				str = Temp.Remove(Temp.IndexOf(SPACE_CHAR));
			}
			else
			{
				str = str[(str.IndexOf(ROLL_SEPERATOR) + 1)..];
			}

			FaceCount = int.Parse(str); RollDie(true);
			PrintOutput(true);
		}
		catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
		{
			MessageProperties response = new()
			{
				MessageReference = new(CurrentMessage!.Id),
				Content = string.Format(CultureInfo.InvariantCulture, ERROR_DND_FORMAT, CurrentMessage.Content, "dx")
			};
			_ = client.Rest.SendMessageAsync(CurrentMessage!.ChannelId, response);
		}
		finally
		{
			Reset();
		}
	}

	/// <summary> Performs an analysis of a possible dice roll based on the given parameters and responds with the resulting statistics. </summary>
	private static void AnalyticalDieLogic()
	{
		str = CurrentMessage!.Content[1..].Trim()[3..];
		Temp = str[(str.IndexOf(ROLL_SEPERATOR) + 1)..];
		RollCount = int.Parse(str[..str.IndexOf(ROLL_SEPERATOR)]);

		if (Temp.Contains(SPACE_CHAR)) // Check for presence of roll modifier at NdN X<---
		{
			Mod = int.Parse(str[(str.IndexOf(SPACE_CHAR) + 1)..]);
			str = Temp.Remove(Temp.IndexOf(SPACE_CHAR));
		}
		else
		{
			str = str[(str.IndexOf(ROLL_SEPERATOR) + 1)..];
		}

		FaceCount = int.Parse(str);
		PrintOutput(false); Reset();
	}

	/// <summary> Rolls die using the variables available. </summary>
	/// <param name="Advanced"> Whether the call was initiated from <see cref="AdvancedDieLogic()"/> or not. </param>
	private static void RollDie(bool Advanced)
	{
		Rolls = new int[RollCount];
		Output = "Rolled `";

		for (int i = 0; i < RollCount; i++) // Dice rolling loop
		{
			Rolls[i] = RNG.Next(FaceCount - 1) + 1 + Mod;
			if (Rolls[i] != FaceCount + Mod && Rolls[i] != 1 + Mod) // If the roll is a normal roll. (Not maximum or 1 + Mod)
			{
				Output += $"{Rolls[i]}, ";
			}
			else if (Rolls[i] != 1 + Mod) // If the roll is a critical (Not 1 + Mod)
			{
				Output += Advanced ? $"{Rolls[i]}*, " : $"**{Rolls[i]}**, ";
				JesusCount++;
			}
			else // If the roll is a failure (Failure of other cases)
			{
				Output += Advanced ? $"{Rolls[i]}!, " : $"~~{Rolls[i]}~~, ";
				MinCount++;
			}
		}
		Result = Rolls.Sum();
	}

	#endregion
}
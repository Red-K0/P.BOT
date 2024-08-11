// The helper class supporting the .r* commands.

using System.Text;

namespace PBot.Commands.Helpers;

/// <summary>
/// Contains constants, variables and the function responsible for probability calculations and roll results.
/// </summary>
internal static class ProbabilityStateMachine
{
	#region Format Strings

	/// <summary>
	/// The message to format and print if the roll command is invalid.
	/// </summary>
	private static readonly CompositeFormat ErrMessage = CompositeFormat.Parse("""
	Sorry, the parameters for your roll '{0}' couldn't be parsed, please input your roll in the format `.[Prefix] [Number]d[Number] [Number]`, where:
	- The prefix is rk, or rm for a high-detail roll or roll analysis, respectively.
	- The first 'Number' is the number of die to roll
	- The second is the number of faces per die
	- The third is the modifier to place on each roll
	For example:
	```
	.rk 4d4 1	 -	Rolls 4 die with 4 sides, adding a 1 to each, giving a detailed result
	.rm 4d5 -2	-	Computes 4 die with 5 sides, subtracting a 2 from each, and displaying probabilities
	```
	""");

	/// <summary>
	/// The message to format and append per standard roll result.
	/// </summary>
	private static readonly CompositeFormat StndString = CompositeFormat.Parse("{0}, ");

	/// <summary>
	/// The message to format and append per critical success.
	/// </summary>
	private static readonly CompositeFormat CritString = CompositeFormat.Parse("{0}*, ");

	/// <summary>
	/// The message to format and append per critical failure.
	/// </summary>
	private static readonly CompositeFormat FailString = CompositeFormat.Parse("{0}!, ");

	#endregion Format Strings

	private static uint w, x, y, z;

	/// <summary> Applies the appropriate logic to use based on the given <paramref name="message"/>. </summary>
	/// <param name="message"> The message to operate with. </param>
	public static async Task Run(Message message)
	{
		try
		{
			int FaceCount, Mod;
			if (message.Content[4..].Contains(' '))
			{
				FaceCount = int.Parse(message.Content[(message.Content.IndexOf('d') + 1)..(message.Content[4..].IndexOf(' ') + 4)]);
				Mod = int.Parse(message.Content[(message.Content[4..].IndexOf(' ') + 5)..]) + 1;
			}
			else
			{
				FaceCount = int.Parse(message.Content[(message.Content.IndexOf('d') + 1)..]);
				Mod = 1;
			}
			int RollCount = int.Parse(message.Content.Remove(message.Content.IndexOf('d'))[4..]);

			StringBuilder Response = new("Rolled `");
			string FinalResponse = "";

			if (message.Content[2] == 'k')
			{
				uint a = w, b = x, c = y, d = z, e;
				int JesusCount = 0, MinCount = 0, NormalizedFaceCount = Math.Abs(FaceCount - 1);
				int[] Rolls = new int[RollCount > 4096 ? 1 : RollCount];
				long Result = 0;

				Stopwatch Timer = Stopwatch.StartNew();

				if (RollCount > 4096)
				{
					Response.Length--;

					int CurrentResult;

					// Mersenne Twister
					for (int i = 0; i < RollCount; i++)
					{
						e = b ^ (b << 11); b = c; c = d; d = a;
						if ((CurrentResult = (((int)(a = a ^ (a >> 19) ^ e ^ (e >> 8)) & 0x7FFFFFFF) % (NormalizedFaceCount + 1)) + Mod) != Mod)
						{
							if (CurrentResult == NormalizedFaceCount + Mod)
							{
								// If the roll is a critical.
								JesusCount++;
							}
						}
						else
						{
							// If the roll is a failure.
							MinCount++;
						}
						Result += CurrentResult;
					}
				}
				else
				{
					// Mersenne Twister
					for (int i = 0; i < RollCount; i++)
					{
						e = b ^ (b << 11); b = c; c = d; d = a;
						if ((Rolls[i] = (((int)(a = a ^ (a >> 19) ^ e ^ (e >> 8)) & 0x7FFFFFFF) % (NormalizedFaceCount + 1)) + Mod) != Mod)
						{
							if (Rolls[i] != NormalizedFaceCount + Mod)
							{
								// If the roll is a normal roll.
								Response.AppendFormat(null, StndString, Rolls[i]);
							}
							else
							{
								// If the roll is a critical.
								Response.AppendFormat(null, CritString, Rolls[i]);
								JesusCount++;
							}
						}
						else
						{
							// If the roll is a failure.
							Response.AppendFormat(null, FailString, Rolls[i]);
							MinCount++;
						}
					}
				}

				Timer.Stop();

				w = a; x = b; y = c; z = d;
				Response.Length -= 2;
				Mod--;

				if (Response.Length > 1536)
				{
					Response.Length = 1536;
					Response.Append("...`");
				}
				else
				{
					Response.Append('`');
				}

				FinalResponse = $"""
					{(Response.ToString() == "Rolled ``" ? $"Rolled {RollCount} die" : Response)} with a total of: {(FaceCount > 0 ? '\0' : '-')}{(RollCount > 4096 ? Result : Rolls.Sum())}

					Roll Data:
					- Number of Max Value Rolls: {JesusCount}
					- Number of Min Value Rolls: {MinCount}
					- Average Value of All Rolls: {(RollCount > 4096 ? Rolls.Average() : Rolls.Sum() / RollCount)}
					- Calculated in {Timer.Elapsed.TotalMilliseconds}ms
					""";
			}

			FinalResponse += $"""

			Roll Analysis:
			- Maximum Possible Roll: {(long)(FaceCount + Mod) * RollCount}
			- Minimum Possible Roll: {(1 + Mod) * (FaceCount > 0 ? RollCount : -RollCount)}
			- Chance of Perfect Roll: {(1 / Math.Pow(FaceCount, RollCount) * 100).ToString("0." + new string('#', 24))}%
			""";

			await message.ReplyAsync(FinalResponse);
		}
		catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException or OverflowException)
		{
			await Client.Rest.SendMessageAsync(message.ChannelId, new()
			{
				MessageReference = new(message.Id),
				Content = string.Format(null, ErrMessage, message.Content)
			});
		}
	}

	/// <summary>
	/// Initializes the xShift128 algorithm based off the current time in ticks.
	/// </summary>
	public static void InitXShift128()
	{
		x = (uint)DateTime.Now.Ticks;
		y = (1812433253 * x) + 1;
		z = (1812433253 * y) + 1;
		w = (1812433253 * z) + 1;
	}
}
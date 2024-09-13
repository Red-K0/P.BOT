// The helper class supporting the .r* commands.

using System.Text;

namespace Bot.Commands.Helpers;

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
	private static readonly CompositeFormat RollString = CompositeFormat.Parse("{0}, ");

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

	/// <summary>
	/// Initializes the xShift128 algorithm based off the current time in ticks.
	/// </summary>
	public static void LoadMersenneTwister()
	{
		x = (uint)DateTime.Now.Ticks;
		y = (1812433253 * x) + 1;
		z = (1812433253 * y) + 1;
		w = (1812433253 * z) + 1;
	}

	/// <summary> Parses a given <paramref name="content"/>, extracting its roll parameters.</summary>
	/// <param name="content"> The content to process for parameters. </param>
	/// <returns> A <see cref="uint"/> <see cref="ValueTuple{T1, T2, T3}"/> tuple, where T1 is the number of rolls, T2 the number of faces, and T3 the modifier to apply to every roll. </returns>
	public static (uint, uint, uint) ParseParameters(ReadOnlySpan<char> content)
	{
		uint RollCount, FaceCount, Mod;
		try
		{
			int SeparatorIndex = content.IndexOf('d');
			if (content[4..].Contains(' '))
			{
				FaceCount = uint.Parse(content[(SeparatorIndex + 1)..(content[4..].IndexOf(' ') + 4)]);
				Mod = uint.Parse(content[(content[4..].IndexOf(' ') + 5)..]) + 1;
			}
			else
			{
				FaceCount = uint.Parse(content[(SeparatorIndex + 1)..]);
				Mod = 1;
			}
			RollCount = uint.Parse(content[3..SeparatorIndex]);
			return (RollCount, FaceCount, Mod);
		}
		catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
		{
			return (0, 0, 0);
		}
	}

	/// <summary> Performs the approriate calculations based off the passed message, replying to it. </summary>
	/// <param name="message"> The message to operate with. </param>
	public static async Task Run(Message message)
	{
		(uint RollCount, uint FaceCount, uint Mod) = ParseParameters(message.Content.AsSpan());

		if (RollCount == 0)
		{
			await message.ReplyAsync(string.Format(null, ErrMessage, message.Content));
			return;
		}

		FaceCount--;

		bool LargeRoll = RollCount > 2048;

		Span<uint> Rolls = stackalloc uint[(int)(LargeRoll ? 0 : RollCount)];

		ulong Result = 0;
		double ElapsedTime;
		if (LargeRoll)
		{
			ElapsedTime = Next(RollCount & uint.MaxValue - 1, FaceCount, Mod, out Result);
			if ((RollCount & 1) == 0) Result += (Next() % FaceCount) + Mod;
		}
		else
		{
			ElapsedTime = NextArray(in Rolls, FaceCount + 1, Mod);
			if ((RollCount & 1) == 1) Rolls[^1] = (Next() % (FaceCount + 1)) + Mod;
		}

		uint JesusCount = 0; uint MinCount = 0;

		string FinalResponse;

		if (!LargeRoll)
		{
			StringBuilder Response = new("Rolled `");
			uint MaxResult = FaceCount + Mod;

			for (int i = 0; i < RollCount; i++)
			{
				if (Rolls[i] != Mod)
				{
					if (Rolls[i] != MaxResult)
					{
						Response.AppendFormat(null, RollString, Rolls[i]);
					}
					else
					{
						Response.AppendFormat(null, CritString, Rolls[i]);
						JesusCount++;
					}
				}
				else
				{
					Response.AppendFormat(null, FailString, Rolls[i]);
					MinCount++;
				}
			}

			Response.Length -= 2;

			if (Response.Length > 1536)
			{
				Response.Length = 1536;
				Response.Append("...`");
			}
			else
			{
				Response.Append('`');
			}

			for (int i = 0; i < Rolls.Length; i++) Result += Rolls[i];

			FinalResponse = Response.ToString();
		}
		else
		{
			FinalResponse = $"Rolled {RollCount} die";
		}

		FinalResponse += $" with a total of: {Result}\n";

		if (!LargeRoll) FinalResponse += $"""

		Roll Data:
		- Number of Max Value Rolls: {JesusCount}
		- Number of Min Value Rolls: {MinCount}
		- Average Value of All Rolls: {Result / RollCount}
		""";

		FinalResponse += $"""

		Calculated in {ElapsedTime}ms

		Roll Analysis:
		- Maximum Possible Roll: {(long)(FaceCount + Mod) * RollCount}
		- Minimum Possible Roll: {Mod * ((FaceCount + 1)> 0 ? RollCount : -RollCount)}
		- Chance of Perfect Roll: {1 / Math.Pow(FaceCount + 1, RollCount) * 100:0.########################}%
		""";

		await message.ReplyAsync(FinalResponse);
	}

	/// <summary>
	/// Runs a single iteration of the roller algorithm, returning the result.
	/// </summary>
	public static uint Next()
	{
		uint e = x ^ (x << 11); x = y; y = z; z = w;
		return w = w ^ (w >> 19) ^ e ^ (e >> 8);
	}

	/// <summary> Randomly fills a given array with numbers. </summary>
	/// <param name="rolls"> The array to fill. </param>
	/// <param name="max"> The inclusive maximum roll result. </param>
	/// <param name="mod"> The modifier to apply to every roll. </param>
	/// <returns> The time taken to fill the array in milliseconds. </returns>
	public static double NextArray(ref readonly Span<uint> rolls, uint max, uint mod)
	{
		int RollCount = rolls.Length & int.MaxValue - 1;
		uint a = w, b = x, c = y, d = z, e;

		Stopwatch Timer = Stopwatch.StartNew();

		for (int i = 0; i < RollCount;)
		{
			e = b ^ (b << 11); b = c; c = d; d = a;
			a = a ^ (a >> 19) ^ e ^ (e >> 8);
			ulong MultiResult = (ulong)a * max;

			rolls[i++] = (uint)(MultiResult >> 32) + mod;
			rolls[i++] = (uint)(((uint)MultiResult * (ulong)max) >> 32) + mod;
		}

		Timer.Stop();

		w = a; x = b; y = c; z = d;

		return Timer.Elapsed.TotalMilliseconds;
	}

	/// <summary>
	/// Calculates the maximum possible result according to the given <paramref name="count"/>, <paramref name="max"/>, and <paramref name="mod"/>, randomly generating a number in that range.
	/// </summary>
	/// <param name="count"> The number of rolls to simulate. </param>
	/// <param name="max"> The maximum result of each simulated roll. </param>
	/// <param name="mod"> The modifier to apply to each simulated roll. </param>
	/// <param name="result"> The <see cref="ulong"/> variable to store the random result in. </param>
	/// <returns> The time taken to generate the result in milliseconds. </returns>
	public static double Next(uint count, uint max, uint mod, out ulong result)
	{
		ulong a = w, b = x, c = y, d = z, e;

		Stopwatch Timer = Stopwatch.StartNew();

		e = b ^ (b << 11); b = c; c = d; d = a; a = a ^ (a >> 19) ^ e ^ (e >> 8);
		a = a ^ (a >> 19) ^ e ^ (e >> 8);

		result = a % (count * (max + mod));

		Timer.Stop();

		w = (uint)a; x = (uint)b; y = (uint)c; z = (uint)d;

		return Timer.Elapsed.TotalMilliseconds;
	}
}

using System.Runtime.CompilerServices;
using System.Text;

namespace Bot.Interactions.Helpers;

/// <summary>
/// Contains constants, variables and the function responsible for probability calculations and roll results.
/// </summary>
[type: SkipLocalsInit]
internal static class DiceRoller
{
	/// <summary>
	/// Initializes the roller, preparing the mersenne twister fields, and allocating the roll buffer.
	/// </summary>
	static DiceRoller()
	{
		x = (uint)DateTime.Now.Ticks;
		y = (1812433253 * x) + 1;
		z = (1812433253 * y) + 1;
		w = (1812433253 * z) + 1;

		_rolls = new uint[2048];
	}

	/// <summary>
	/// The message to format and append per standard roll result.
	/// </summary>
	private static readonly CompositeFormat _rollString = CompositeFormat.Parse("{0}, ");

	/// <summary>
	/// The message to format and append per critical success.
	/// </summary>
	private static readonly CompositeFormat _critString = CompositeFormat.Parse("{0}*, ");

	/// <summary>
	/// The message to format and append per critical failure.
	/// </summary>
	private static readonly CompositeFormat _failString = CompositeFormat.Parse("{0}!, ");

	/// <summary>
	/// Used for creating roll response strings efficiently.
	/// </summary>
	private static readonly StringBuilder _responseBuilder = new(512);

	/// <summary>
	/// Holds roll results.
	/// </summary>
	private static readonly uint[] _rolls;
	private static uint w, x, y, z;

	public static string Run(uint count, uint faces, uint mod)
	{
		faces--;

		bool largeRoll = count > 2048;
		Span<uint> rolls = stackalloc uint[(int)(largeRoll ? 0 : count)];
		ulong result = 0;

		if (largeRoll)
		{
			NextTotal(count & uint.MaxValue - 1, faces, mod, out result);
			if ((count & 1) == 0) result += (Next() % faces) + mod;
		}
		else
		{
			NextArray(in rolls, faces + 1, mod);
			if ((count & 1) == 1) rolls[^1] = (Next() % (faces + 1)) + mod;
		}

		uint maxCount = 0; uint minCount = 0;
		string finalResponse;

		if (!largeRoll)
		{
			_responseBuilder.Clear();
			_responseBuilder.Append("Rolled `");
			uint maxResult = faces + mod;

			for (int i = 0; i < count; i++)
			{
				if (rolls[i] != mod)
				{
					if (rolls[i] != maxResult)
					{
						_responseBuilder.AppendFormat(null, _rollString, rolls[i]);
					}
					else
					{
						_responseBuilder.AppendFormat(null, _critString, rolls[i]);
						maxCount++;
					}
				}
				else
				{
					_responseBuilder.AppendFormat(null, _failString, rolls[i]);
					minCount++;
				}
			}

			_responseBuilder.Length -= 2;

			if (_responseBuilder.Length > 1536)
			{
				_responseBuilder.Length = 1536;
				_responseBuilder.Append("...`");
			}
			else
			{
				_responseBuilder.Append('`');
			}

			for (int i = 0; i < rolls.Length; i++) result += rolls[i];

			finalResponse = _responseBuilder.ToString();
		}
		else
		{
			finalResponse = $"Rolled {count} die";
		}

		finalResponse += $" with a total of: {result}\n";

		if (!largeRoll) finalResponse += $"""

		Roll Data:
		- Number of Max Value Rolls: {maxCount}
		- Number of Min Value Rolls: {minCount}
		- Average Value of All Rolls: {result / count}
		""";

		finalResponse += $"""

		Roll Analysis:
		- Maximum Possible Roll: {(long)(faces + mod) * count}
		- Minimum Possible Roll: {mod * ((faces + 1)> 0 ? count : -count)}
		- Chance of Perfect Roll: {1 / Math.Pow(faces + 1, count) * 100:0.########################}%
		""";

		return finalResponse;
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
	public static void NextArray(ref readonly Span<uint> rolls, uint max, uint mod)
	{
		int rollCount = rolls.Length & int.MaxValue - 1;
		uint a = w, b = x, c = y, d = z, e;

		for (int i = 0; i < rollCount;)
		{
			e = b ^ (b << 11); b = c; c = d; d = a;
			a = a ^ (a >> 19) ^ e ^ (e >> 8);
			ulong MultiResult = (ulong)a * max;

			rolls[i++] = (uint)(MultiResult >> 32) + mod;
			rolls[i++] = (uint)(((uint)MultiResult * (ulong)max) >> 32) + mod;
		}

		w = a; x = b; y = c; z = d;
	}

	/// <summary>
	/// Calculates the maximum possible result according to the given <paramref name="count"/>, <paramref name="max"/>, and <paramref name="mod"/>, randomly generating a number in that range.
	/// </summary>
	/// <param name="count"> The number of rolls to simulate. </param>
	/// <param name="max"> The maximum result of each simulated roll. </param>
	/// <param name="mod"> The modifier to apply to each simulated roll. </param>
	/// <param name="result"> The <see cref="ulong"/> variable to store the random result in. </param>
	public static void NextTotal(uint count, uint max, uint mod, out ulong result)
	{
		ulong a = w, b = x, c = y, d = z, e;

		e = b ^ (b << 11); b = c; c = d; d = a; a = a ^ (a >> 19) ^ e ^ (e >> 8);
		a = a ^ (a >> 19) ^ e ^ (e >> 8);

		result = a % (count * (max + mod));

		w = (uint)a; x = (uint)b; y = (uint)c; z = (uint)d;
	}
}

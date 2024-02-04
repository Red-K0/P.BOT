namespace P_BOT;

internal static class FastRandom
{
	public static uint Next(uint seed, uint limit)
	{
		uint state = seed;
		state ^= state << 13;
		state ^= state >> 17;
		state ^= state << 05;
		return state & limit;
	}
	public static int Next24(int seed)
	{
		int state = seed;
		state ^= state << 13;
		state ^= state >> 17;
		state ^= state << 05;
		return state & 0xFFFFFF;
	}
}

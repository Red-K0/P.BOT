namespace P_BOT;

internal static class FastRandom
{
	public static uint Next(uint seed, uint limit)
	{
		uint state = seed;
		state ^= state << 13;
		state ^= state >> 17;
		state ^= state << 05;

#if DEBUG_RANDOM
		Messages.Logging.AsVerbose($"The number 0x{state:X} was generated, and limited to 0x{limit:X}.");

#endif
		return state & limit;
	}
	public static int Color()
	{
		int state = Environment.TickCount;
		state ^= state << 13;
		state ^= state >> 17;
		state ^= state << 05;

#if DEBUG_RANDOM
		Messages.Logging.AsVerbose($"The number 0x{state:X} was generated with seed 0x{seed:X}, and limited to 0x{0xFFFFFF:X}.");

#endif
		return state & 0xFFFFFF;
	}
}

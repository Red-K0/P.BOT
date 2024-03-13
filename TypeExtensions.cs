namespace P_BOT;

internal static class TypeExtensions
{
	/// <summary>
	/// Converts the value of this instance to its equivalent <see cref="InteractionMessageProperties"/> representation.
	/// </summary>
	public static InteractionMessageProperties ToInteraction(this MessageProperties Obj) => new()
	{
		AllowedMentions = Obj.AllowedMentions,
		Attachments = Obj.Attachments,
		Components = Obj.Components,
		Content = Obj.Content,
		Embeds = Obj.Embeds,
		Flags = Obj.Flags,
		Tts = Obj.Tts
	};

	#region Special Remove
	const string OffsetOOB =
	"The index of the given character + the offset were outside the bounds of the string.";

	/// <summary>
	/// Returns a new string in which all the characters following the first occurrence of the specified Unicode character in this string, offset by the given number of characters, have been deleted.
	/// </summary>
	/// <returns> A new string that is equivalent to this string except for the removed characters. </returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static string Remove(this string obj, char value, int offset)
	{
		int index = obj.IndexOf(value) + offset;
		if (index >= obj.Length) throw new ArgumentOutOfRangeException(nameof(offset), OffsetOOB);
		if (index >= 0) return obj.Remove(index);
		else return obj;
	}

	/// <summary>
	/// Returns a new string in which all the characters following the first occurrence of the specified Unicode character in this string, offset by the given number of characters, have been deleted.
	/// </summary>
	/// <returns> A new string that is equivalent to this string except for the removed characters. </returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static string Remove(this string obj, string value, int offset)
	{
		int index = obj.IndexOf(value) + offset;
		if (index >= obj.Length) throw new ArgumentOutOfRangeException(nameof(offset), OffsetOOB);
		if (index >= 0) return obj.Remove(index);
		else return obj;
	}
	#endregion
}
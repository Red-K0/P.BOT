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
}
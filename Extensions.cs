using System.Runtime.InteropServices;
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

internal static partial class PBOT_C
{
	/// <summary> Unmanaged method imported via PBOT_C.dll, enables the use of virtual terminal sequences globally. </summary>
	/// <returns> True if successful, otherwise returns false. </returns>
	[LibraryImport("PBOT_C.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static partial bool EnableVirtual();

	#region Sequences
	private const string ESC = "\x1b";
	private const string CSI = ESC + "[";

	public  const string Red   = CSI + "91m";
	public  const string Green = CSI + "92m";
	public  const string Blue  = CSI + "94m";
	public  const string None  = CSI + "37m";
	#endregion
}
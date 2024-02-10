using P_BOT.Command_Processing.Helpers;
using static P_BOT.EmbedComponents;
using static P_BOT.EmbedHelpers;
namespace P_BOT.Command_Processing;

internal sealed partial class SlashCommand
{
	/// <summary> Creates a post, experimental. </summary>
	public partial Task CreatePost(string content, bool anonymous, bool draft)
	{
		// Get an ID for the post.
		ulong InternalPostID = Convert.ToUInt64(DataBackend.ReadMemory(3, DataBackend.Pages.Counter)) + 1;

		MessageProperties msg_prop = CreateEmbed
		(
			content,
			anonymous ?
			CreateAuthorObject("Posted by an Anonymous User", URL_ANONUSER) :
			CreateAuthorObject($"Posted by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			DateTime.Now,
			CreateFooterObject($"Post ID: {InternalPostID}")
		);

		if (draft)
		{
			return RespondAsync(InteractionCallback.Message(new() { Embeds = msg_prop.Embeds, Flags = MessageFlags.Ephemeral }));
		}
		else
		{
			client.Rest.SendMessageAsync(SERVER_POSTFEED, msg_prop);

			ulong ExternalPostID = PostFunctions.StoreID(InternalPostID);

			return RespondAsync(InteractionCallback.Message(new()
			{
				Content = $"Post created successfully, view it [here]({SERVER_LINK}{SERVER_POSTFEED}/{ExternalPostID})",
				Flags = MessageFlags.Ephemeral
			}));
		}
	}
}
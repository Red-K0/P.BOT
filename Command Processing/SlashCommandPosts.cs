using P_BOT.Command_Processing_Helpers;
using static P_BOT.EmbedComponents;
using static P_BOT.EmbedHelpers;
namespace P_BOT;

internal partial class SlashCommand
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

			// Reserve the ID.
			DataBackend.WriteMemory(3, DataBackend.Pages.Counter, InternalPostID.ToString());

			// Gets the message ID of the last sent message in the feed channel, almost always the post created by this method.
			RestMessage ExternalPostID = client.Rest.GetMessagesAsync(SERVER_POSTFEED, new() { Limit = 1 }).ToBlockingEnumerable().First();

			// Store the Internal ID and its relevant external ID in the ID List.
			DataBackend.AppendMemory(DataBackend.Pages.PostDatabaseIDList, $"{InternalPostID}, {ExternalPostID}");

			return RespondAsync(InteractionCallback.Message(new()
			{
				Content = $"Post created successfully, view it [here]({SERVER_LINK}{SERVER_POSTFEED}/{ExternalPostID})",
				Flags = MessageFlags.Ephemeral
			}));
		}
	}
}
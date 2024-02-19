// Post Commands Extension File
// This file contains commands that are relevant to the bot's post system and its functions.
// An example of these commands is the CreatePost() command, which creates and records a post.
// Commands in this file heavily rely on content found in the PostDB folder.

using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;
using static P_BOT.EmbedComponents;
using static P_BOT.EmbedHelpers;

namespace P_BOT.Command_Processing;
public sealed partial class SlashCommand
{
	#region Attributes
	[SlashCommand("post", "Create a post (WIP).")]
	public partial Task CreatePost
	(
		[SlashCommandParameter(Name = "content", Description = "The text-based content of the post.")]
		string content,

		[SlashCommandParameter(Name = "image", Description = "The image to attach to the post.")]
		Attachment? image = null,

		[SlashCommandParameter(Name = "anonymous", Description = "Should the post be made anonymously?")]
		bool anonymous = false,

		[SlashCommandParameter(Name = "draft", Description = "Should a private preview of the post be created instead?")]
		bool draft = false
	);
	#endregion

	/// <summary> Creates a post, experimental. </summary>
	public partial Task CreatePost(string content, Attachment? image, bool anonymous, bool draft)
	{
		// Make sure the interaction doesn't time out
		Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

		// Get an ID for the post.
		ulong InternalPostID = Convert.ToUInt64(DataBackend.ReadMemory(3, DataBackend.Pages.Counter)) + 1;

		MessageProperties msg_prop = CreateEmbed
		(
			content,
			anonymous ?
			CreateAuthorObject("Posted by an Anonymous User", URL_ANONUSER) : // < Note the colon
			CreateAuthorObject($"Posted by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			DateTime.Now,
			CreateFooterObject($"Post ID: {InternalPostID}"),
			ImageURL: new(image?.Url)
		);

		if (draft)
		{
			return Context.Interaction.SendFollowupMessageAsync(new() { Embeds = msg_prop.Embeds });
		}
		else
		{
			_ = client.Rest.SendMessageAsync(SERVER_POSTFEED, msg_prop.WithContent($"Post #{InternalPostID}")).Result;

			ulong ExternalPostID = PostFunctions.StoreID(InternalPostID);

			// Creates the corresponding thread for the post, and then resets the content to empty.
			client.Rest.CreateGuildThreadAsync(SERVER_POSTFEED, ExternalPostID, new($"Post #{InternalPostID}"));
			client.Rest.    ModifyMessageAsync(SERVER_POSTFEED, ExternalPostID, new(s => s.WithContent("_ _")));

			return Context.Interaction.SendFollowupMessageAsync(new()
			{
				Content = $"Post created successfully, view it [here.]({SERVER_LINK}{SERVER_POSTFEED}/{ExternalPostID})",
				Flags = MessageFlags.Ephemeral
			});
		}
	}
}
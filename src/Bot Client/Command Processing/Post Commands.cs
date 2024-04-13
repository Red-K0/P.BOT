// Post Commands Extension File
// This file contains commands that are relevant to the bot's post system and its functions.
// An example of these commands is the CreatePost() command, which creates and records a post.
// Commands in this file heavily rely on content found in the PostDB folder.

using NetCord.Services.ApplicationCommands;
using static PBot.Command_Processing.Helpers.Posts;
using static PBot.Embeds;
namespace PBot.Command_Processing;

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

	/// <summary>
	/// Creates a post, experimental.
	/// </summary>
	public async partial Task CreatePost(string content, Attachment? image, bool anonymous, bool draft)
	{
#if DEBUG_COMMAND
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		// Make sure the interaction doesn't time out
		await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

		// Get an ID for the post.
		ulong InternalPostID = Convert.ToUInt64(await Pages.Read(Pages.Files.Counters, 3)) + 1;

		MessageProperties msg_prop = Generate
		(
			content,
			anonymous ?
			CreateAuthor("Posted by an Anonymous User", GetAssetURL("Anonymous Profile Icon.png")) : // < Note the colon
			CreateAuthor($"Posted by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			DateTime.Now,
			CreateFooter($"Post ID: {InternalPostID}"),
			imageURLs: [image?.Url],
			refID: Context.User.Id
		);

		if (draft)
		{
			await Context.Interaction.SendFollowupMessageAsync(msg_prop.ToInteraction());
		}
		else
		{
			_ = client.Rest.SendMessageAsync(CHANNEL, msg_prop.WithContent($"Post #{InternalPostID}")).Result;

			ulong ExternalPostID = StoreID(InternalPostID);

			// Creates the corresponding thread for the post, and then resets the content to empty.
			await client.Rest.CreateGuildThreadAsync(CHANNEL, ExternalPostID, new($"Post #{InternalPostID}"));
			await client.Rest.ModifyMessageAsync(CHANNEL, ExternalPostID, new(s => s.WithContent("_ _")));

			await Context.Interaction.SendFollowupMessageAsync(new()
			{
				Content = $"Post created successfully, view it [here.]({SERVER_LINK}{CHANNEL}/{ExternalPostID})",
				Flags = MessageFlags.Ephemeral
			});
		}

#if DEBUG_COMMAND
		Messages.Messages.Logging.AsVerbose($"CreatePost Completed [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif
	}
}
// Post Commands Extension File
// This file contains commands that are relevant to the bot's post system and its functions.
// An example of these commands is the CreatePost() command, which creates and records a post.
// Commands in this file heavily rely on content found in the PostDB folder.

using NetCord.Services.ApplicationCommands;
using static PBot.Embeds;
using static PBot.Files;
namespace PBot.Commands;

public sealed partial class SlashCommands
{
	#region Attributes
	[SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested")]
	[SlashCommand("post", "WIP")]
	public partial Task CreatePost
	(
		[SlashCommandParameter(Name = "content", Description = "The text-based content of the post.")]
		string content,

		[SlashCommandParameter(Name = "image", Description = "The image to attach to the post.")]
		Attachment? image = null,

		[SlashCommandParameter(Name = "anonymous", Description = "Should the post be made anonymously?")]
		bool anonymous = false,

		[SlashCommandParameter(Name = "channel", Description = "The channel to create the post in, defaults to the current channel.")]
		Channel? channel = null,

		[SlashCommandParameter(Name = "draft", Description = "Should a private preview of the post be created instead?")]
		bool draft = false
	);
	#endregion Attributes

	/// <summary>Command task. Creates an embed with the specified <paramref name="content"/> and <paramref name="image"/>.</summary>
	/// <param name="content">The text-based content of the post.</param>
	/// <param name="image">The image to attach to the post.</param>
	/// <param name="anonymous">Should the post be made anonymously?</param>
	/// <param name="channel">The channel to create the post in, defaults to the current channel.</param>
	/// <param name="draft">Should a private preview of the post be created instead?</param>
	public async partial Task CreatePost(string content, Attachment? image, bool anonymous, Channel? channel, bool draft)
	{
		await RespondAsync(InteractionCallback.Message(new() { Content = $"{(draft ? "Draft" : "Post")} Created Successfully", Flags = MessageFlags.Ephemeral }));

		await Client.Rest.SendMessageAsync(channel?.Id ?? Context.Channel.Id, Generate(CreateProperties(
		content,
		anonymous ? CreateAuthor($"{(draft ? "Draft" : "Post")} Created Anonymously", GetAssetURL("Anonymous Profile Picture.png"))
				  : CreateAuthor(Context.User.GetDisplayName(), Context.User.GetAvatar()),
		DateTime.Now,
		CreateFooter($"{(draft ? "Draft" : "Post")} ID: {await ReadCounter(CounterLines.Posts, draft ? 0 : 1)}"),
		image?.ProxyUrl,
		refId: Context.User.Id
		)).ToMessage());
	}
}
using P_BOT.Command_Processing.Helpers;
using static P_BOT.EmbedComponents;
using static P_BOT.EmbedHelpers;
namespace P_BOT.Command_Processing;

/// <summary> Contains the commands used by P.BOT and their associated tasks. </summary>
internal sealed partial class SlashCommand
{
	/// <summary> Command task. Checks if P.BOT's system is active. </summary>
	public partial Task SystemsCheck()
	{
		const string response =
		"""
		
		System is active and running.
		
		""";
		return RespondAsync(InteractionCallback.Message(response));
	}

	/// <summary> Unimplemented. </summary>
	public partial Task CreatePoll()
	{
		return RespondAsync(InteractionCallback.Message(ERROR_CODE_NODEV));
	}

	/// <summary> Command task. Gets the definition of the term specified in the <paramref name="term"/> parameter.</summary>
	public partial Task GetDefinition(Define.DefineChoices term)
	{
		_ = Define.Definitions.TryGetValue(term, out string? definition);
		MessageProperties msg_prop = CreateEmbed
		(
			definition,
			CreateAuthorObject("PPP Encyclopedia", URL_RULESICON),
			DateTimeOffset.UtcNow,
			ReplyTo: Context.User.Id
		);

		return RespondAsync(InteractionCallback.Message(new() { Embeds = msg_prop.Embeds }));
	}

	/// <summary> Command task. Gets the avatar of the <paramref name="user"/>, in a specific format if specified in <paramref name="format"/>. </summary>
	public partial Task GetAvatar(User user, ImageFormat format)
	{
		user ??= Context.User;
		MessageProperties msg_prop = CreateEmbed
		(
			user.HasAvatar ?
			$"Sure, [here]({user.GetAvatarUrl(format)}) is <@{user.Id}>'s avatar " :
			$"Sorry, <@{user.Id}> does not currently have an avatar set, [here]({user.DefaultAvatarUrl}) is the default discord avatar",

			CreateAuthorObject($"{user.Username}'s Avatar", user.GetAvatarUrl(ImageFormat.Png).ToString()),
			DateTimeOffset.UtcNow,
			ReplyTo: Context.User.Id,
			ImageURL: new(user.GetAvatarUrl(format).ToString())
		);

		return RespondAsync(InteractionCallback.Message(new() { Embeds = msg_prop.Embeds, AllowedMentions = AllowedMentionsProperties.None }));
	}

	/// <summary> Translates a given <see cref="string"/> from the <paramref name="source_lang"/> to the <paramref name="target_lang"/>, and responds with the output. </summary>
	public partial Task Translate(string input, Translation.Options source_lang, Translation.Options target_lang)
	{
		_ = RespondAsync(InteractionCallback.Message("Translating your input, please wait..."));
		MessageProperties msg_prop = CreateEmbed
		(
			Translation.GetTranslation(input, source_lang, target_lang),
			CreateAuthorObject("Translation Processed", URL_TLICON),
			DateTime.Now,
			CreateFooterObject($"Translation requested by {Context.User.Username}", Context.User.GetAvatarUrl().ToString()),
			0x72767D
		);
		return Context.Client.Rest.SendMessageAsync(Context.Channel.Id, msg_prop);
	}

	/// <summary> Toggles the state of the module specified in the <paramref name="module"/> parameter. </summary>
	public partial Task ToggleModule(Options.Modules module)
	{
		bool result = false;
		switch (module)
		{
			case Options.Modules.DnDTextModule: Options.DnDTextModule ^= true; result = Options.DnDTextModule; break;
		}
		return RespondAsync(InteractionCallback.Message(result ? $"The module '{module}' has been successfully enabled." : $"The module '{module}' has been successfully disabled."));
	}
}
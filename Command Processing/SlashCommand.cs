using NetCord.Services.ApplicationCommands;
using P_BOT.Command_Processing.Helpers;

namespace P_BOT.Command_Processing;
internal sealed partial class SlashCommand : ApplicationCommandModule<SlashCommandContext>
{
	[SlashCommand(CMD_STPOLL_NAME, CMD_STPOLL_DESC)]
	public partial Task CreatePoll();

	[SlashCommand(CMD_DEFINE_NAME, CMD_DEFINE_DESC)]
	public partial Task GetDefinition
	(
		  [SlashCommandParameter(Name = CMD_DEFINE_PR1N, Description = CMD_DEFINE_PR1D)] Define.DefineChoices term
	);

	[SlashCommand(CMD_AVATAR_NAME, CMD_AVATAR_DESC)]
	public partial Task GetAvatar
	(
		  [SlashCommandParameter(Name = CMD_AVATAR_PR1N, Description = CMD_AVATAR_PR1D)] User user
		, [SlashCommandParameter(Name = CMD_AVATAR_PR2N, Description = CMD_AVATAR_PR2D)] ImageFormat format = ImageFormat.Png
	);

	[SlashCommand(CMD_PPPOST_NAME, CMD_PPPOST_DESC)]
	public partial Task CreatePost(string content, bool anonymous = false, bool draft = false);

	[SlashCommand(CMD_NTPING_NAME, CMD_NTPING_DESC)]
	public partial Task SystemsCheck();

	[SlashCommand(CMD_TOGGLE_NAME, CMD_TOGGLE_DESC)]
	public partial Task ToggleModule
	(
		 [SlashCommandParameter(Name = CMD_TOGGLE_PR1N, Description = CMD_TOGGLE_PR1D)] Options.Modules module
	);

	[SlashCommand(CMD_HTTPTL_NAME, CMD_HTTPTL_DESC)]
	public partial Task Translate
	(
		  [SlashCommandParameter(Name = CMD_HTTPTL_PR1N, Description = CMD_HTTPTL_PR1D)] string input
		, [SlashCommandParameter(Name = CMD_HTTPTL_PR2N, Description = CMD_HTTPTL_PR2D)] Translation.Options source_lang = Translation.Options.en
		, [SlashCommandParameter(Name = CMD_HTTPTL_PR3N, Description = CMD_HTTPTL_PR3D)] Translation.Options target_lang = Translation.Options.ja
	);
}
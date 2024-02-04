using NetCord.Services.ApplicationCommands;
namespace P_BOT;
/// <summary> Contains the commands used by P.BOT and their associated tasks. </summary>
public class SlashCommands : ApplicationCommandModule<SlashCommandContext>
{
	#region Fun

	/// <summary> Command task. Checks if P.BOT's system is active. </summary>
	[SlashCommand(CMD_NTPING_NAME, CMD_NTPING_DESC)]
	public Task SystemsCheck()
	{
		string response =
			"""
			
			System is active and running.
			
			""";
		return RespondAsync(InteractionCallback.Message(response));
	}

#if SecretSanta

		/// <summary> Command task. Used for the secret santa seasonal event. </summary>
		[SlashCommand(CMD_NAME_SANTA, CMD_DESC_SANTA)] //TODO Add command description
		public Task SecretSanta()
		{
			string[] STR = File.ReadAllLines(MEMORY_SECRETSANTA_SENDER);
			string[] STRid = new string[STR.Length]; string[] STRrc = new string[STR.Length]; string response = ""; bool Valid = true;
			Array.Copy(STR, STRid, STR.Length); Array.Copy(STR, STRrc, STR.Length);
			for (int i = 0; i < STR.Length; i++)
			{
				STRrc[i] = STRrc[i][(STRrc[i].IndexOf(' ')+1)..];
				STRid[i] = STRid[i].Remove(STR[i].IndexOf(' '));
				if (STRid[i].Contains(Context.User.Id.ToString().Replace("<@", null).Replace(">", null)))
				{
					Valid = false;
					response = $"""
					\0# 🎄 Secret Santa 2023 🎄
					Sorry, you already have a recipient assigned to you: {STRrc[i]}. Merry Christmas.
					""";
				}
			}
			if (Valid)
			{
				//Variable assignments
				STR = File.ReadAllLines(MEMORY_SECRETSANTA_RECIEVER);
				Random RNG = new(); int i = 0; int INT;

				//Null response check
				while (true)
				{
					INT = RNG.Next(0, STR.Length);
					response = $"""
					\0# 🎄 Secret Santa 2023 🎄
					Your secret santa recipient is {STR[INT]}, Merry Christmas .
					""";
					if (STR[INT].StartsWith('*'))
					{
						i++;
						if (i < STR.Length) continue;
						else { response = $"""
							\0# 🎄 Secret Santa 2023 🎄
							Sorry, there are no gift recipients left in the gift pool, you can check back later to see if there are new recipients, or simply celebrate normally. Merry Christmas.
							"""; }
					}
					else
					{
						string WriteTo = $"<@{Context.User.Id}> {STR[INT]}";
						STR[INT] = $"* {STR[INT]}"; File.WriteAllLines(MEMORY_SECRETSANTA_RECIEVER, STR);
						Array.Clear(STR); STR = File.ReadAllLines(MEMORY_SECRETSANTA_SENDER);
						STR = [.. STR, WriteTo]; File.WriteAllLines(MEMORY_SECRETSANTA_SENDER, STR);
					}
					break;
				}
			}
			//Response generation
			MessageProperties msg_prop = new() { Content = response, Flags = MessageFlags.Ephemeral, AllowedMentions = null };
			InteractionMessageProperties imsg_prop = new() { Flags = msg_prop.Flags, Content = msg_prop.Content };
			return RespondAsync(InteractionCallback.Message(imsg_prop));
		}
#endif

	#endregion

	#region Utility

	/// <summary> Unimplemented. </summary>
	[SlashCommand(CMD_STPOLL_NAME, CMD_STPOLL_DESC)]
	public Task CreatePoll()
	{
		return RespondAsync(InteractionCallback.Message(ERROR_CODE_NODEV));
	}

	/// <summary> Command task. Gets the definition of the term specified in the <paramref name="term"/> parameter.</summary>
	[SlashCommand(CMD_DEFINE_NAME, CMD_DEFINE_DESC)]
	public Task Define([SlashCommandParameter(Name = CMD_DEFINE_PR1N, Description = CMD_DEFINE_PR1D)] DefineData.DefineChoices term)
	{
		string? definition;
		MessageProperties msg_prop;
		InteractionMessageProperties imessageprop;
		switch (term)
		{
			case DefineData.DefineChoices.MeaningOf_PPP:
				_ = DefineData.Definitions.TryGetValue(term, out definition);
				msg_prop = EmbedHelpers.CreateEmbed(
				definition,
				"PPP Encyclopedia",
				URL_RULESICON,
				DateTimeOffset.UtcNow);

				msg_prop.MessageReference = new(Context.User.Id);
				imessageprop = new() { Embeds = msg_prop.Embeds };

				return RespondAsync(InteractionCallback.Message(imessageprop));
			case DefineData.DefineChoices.WhatIs_ThePPP:
				_ = DefineData.Definitions.TryGetValue(term, out definition);
				msg_prop = EmbedHelpers.CreateEmbed(
				definition,
				"PPP Encyclopedia",
				URL_RULESICON,
				DateTimeOffset.UtcNow);

				msg_prop.MessageReference = new(Context.User.Id);
				imessageprop = new() { Embeds = msg_prop.Embeds };

				return RespondAsync(InteractionCallback.Message(imessageprop));
			default: return RespondAsync(InteractionCallback.Message(ERROR_DEFINEFAIL));
		}
	}

	/// <summary> Command task. Gets the avatar of the <paramref name="user"/>, in a specific format if specified in <paramref name="format"/>. </summary>
	[SlashCommand(CMD_AVATAR_NAME, CMD_AVATAR_DESC)]
	public Task GetAvatar([SlashCommandParameter(Name = CMD_AVATAR_PR1N, Description = CMD_AVATAR_PR1D)] User user,
						  [SlashCommandParameter(Name = CMD_AVATAR_PR2N, Description = CMD_AVATAR_PR2D)] ImageFormat format = ImageFormat.Png)
	{
		user ??= Context.User;
		MessageProperties msg_prop = EmbedHelpers.CreateEmbed(
		user.HasAvatar ? $"Sure, [here]({user.GetAvatarUrl(format)}) is <@{user.Id}>'s avatar " : $"Sorry, <@{user.Id}> does not currently have an avatar set, [here]({user.DefaultAvatarUrl}) is the default discord avatar",
		$"{user.Username}'s Avatar",
		user.GetAvatarUrl(ImageFormat.Png).ToString(),
		DateTimeOffset.UtcNow, ImageURL: new(user.GetAvatarUrl(format).ToString()));

		msg_prop.MessageReference = new(Context.User.Id);
		msg_prop.AllowedMentions = null;
		InteractionMessageProperties imessageprop = new() { Embeds = msg_prop.Embeds };

		return RespondAsync(InteractionCallback.Message(imessageprop));
	}

	/// <summary> Unimplemented. </summary>
	[SlashCommand(CMD_HTTPTL_NAME, CMD_HTTPTL_DESC)]
	public Task Translate([SlashCommandParameter(Name = CMD_HTTPTL_PR1N, Description = CMD_HTTPTL_PR1D)] string input,
						  [SlashCommandParameter(Name = CMD_HTTPTL_PR2N, Description = CMD_HTTPTL_PR2D)] Translation.SupportedLanguages source_lang = Translation.SupportedLanguages.en,
						  [SlashCommandParameter(Name = CMD_HTTPTL_PR3N, Description = CMD_HTTPTL_PR3D)] Translation.SupportedLanguages target_lang = Translation.SupportedLanguages.ja)
	{
		_ = RespondAsync(InteractionCallback.Message("Translating your input, please wait..."));
		MessageProperties msg_prop = EmbedHelpers.CreateEmbed(
			$"""
			Original Text: {input}

			Translated Text: {Translation.GetTranslation(input, source_lang, target_lang)}
			""",
			"Translation Completed",
			URL_TLICON,
			DateTime.Now,
			$"Translation requested by {Context.User.Username}",
			Context.User.GetAvatarUrl().ToString(),
			0x72767D
		);
		return Context.Client.Rest.SendMessageAsync(Context.Channel.Id, msg_prop);
	}

	#endregion

	#region Modules

	/// <summary> Toggles the state of the module specified in the <paramref name="module"/> parameter. </summary>
	[SlashCommand(CMD_TOGGLE_NAME, CMD_TOGGLE_DESC)]
	public Task ToggleModule([SlashCommandParameter(Name = CMD_TOGGLE_PR1N, Description = CMD_TOGGLE_PR1D)] Options.Modules module)
	{
		bool result = false;
		switch (module)
		{
			case Options.Modules.DnDTextModule: Options.DnDTextModule ^= true; result = Options.DnDTextModule; break;
		}
		return RespondAsync(InteractionCallback.Message(result ? $"The module '{module}' has been successfully enabled." : $"The module '{module}' has been successfully disabled."));
	}

	#endregion
}
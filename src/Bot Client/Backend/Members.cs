using System.Collections.Frozen;
using System.Text;
namespace Bot.Backend;

/// <summary>
/// Contains the cached members list, role list, and their relevant methods. This class possesses a static constructor not shipped in the GitHub repo for privacy reasons.
/// </summary>
internal static class Members
{
	/// <summary>
	/// Represents a <see cref="GuildUserInfo"/> object with additional bot-related fields.
	/// </summary>
	/// <param name="info">The <see cref="GuildUserInfo"/> to base the object on.</param>
	public sealed class Member(GuildUserInfo info)
	{
		public GuildUserInfo Info = info;

		public bool IsFounder = FounderRoles.ContainsKey(info.User.Id);

		public string DisplayName => (Info.User.Nickname ?? Info.User.GlobalName ?? Info.User.Username).FromEscapedUnicode().ToEscapedMarkdown();
		public Role? PersonalRole => FounderRoles.TryGetValue(Info.User.Id, out Role? role) ? role : null;

		// Spam Filter
		public bool SpamLastMessageHeading { get; set; }

		public int SpamSameMessageCount { get; set; }
		public int SpamLastAttachmentSize { get; set; }
		public string SpamLastMessage { get; set; } = "";
	}

	/// <summary>
	/// The bot's internal member list, generated from the <c>members-search</c> endpoint.
	/// </summary>
	public static Dictionary<ulong, Member> MemberList { get; set; } = [];

	/// <summary>
	/// The bot's internal list of event roles, and their appropriate descriptions.
	/// </summary>
	public static FrozenDictionary<ulong, string> Accolades { get; private set; } = null!;

	/// <summary>
	/// The bot's internal list of founder roles, alongside their owners' user IDs.
	/// </summary>
	public static FrozenDictionary<ulong, Role> FounderRoles { get; private set; } = null!;

	/// <summary>
	/// Returns a string summarizing a user's accolades and other important roles.
	/// </summary>
	public static async Task<string> GetUserAccolades(Member member)
	{
		string index = (Array.IndexOf([.. MemberList.Keys], member.Info.User.Id) + 1).ToString();

		if (index.Length == 2 && index[^2] == '1')
		{
			index += "th";
		}
		else
		{
			index += index[^1] switch { '1' => "st", '2' => "nd", '3' => "rd", _ => "th" };
		}

		StringBuilder AccoladeString = new($">>> <@&{(member.IsFounder
			? member.PersonalRole!.Id
			: await Files.GetCounter(Files.CounterLines.CultistID))}>\n- The {index} member.");

		foreach (ulong role in member.Info.User.RoleIds.Where(Accolades.ContainsKey)) AccoladeString.Append($"\n<@&{role}>\n- {Accolades[role]}");
		return AccoladeString.ToString();
	}

	/// <summary>
	/// Initializes the class with data from the <c>members-search</c> endpoint.
	/// </summary>
	/// <summary>
	/// Initializes the class with data from the <c>members-search</c> endpoint.
	/// </summary>
	public static async Task LoadDictionaries()
	{
		Dictionary<ulong, Role> FounderRolesSurrogate = [];
		foreach (KeyValuePair<string, string> pair in await Files.GetDictionary(Files.Names.FounderRoles))
		{
			ulong RoleID = Convert.ToUInt64(pair.Value);
			FounderRolesSurrogate.Add(Convert.ToUInt64(pair.Key), Core.Guild.Roles[RoleID != 0 ? RoleID : (await Files.GetCounter(Files.CounterLines.FounderID))]);
		}
		FounderRoles = FounderRolesSurrogate.ToFrozenDictionary();

		Dictionary<ulong, string> AccoladesSurrogate = [];
		foreach (KeyValuePair<string, string> pair in await Files.GetDictionary(Files.Names.Accoaldes))
		{
			AccoladesSurrogate.Add(Convert.ToUInt64(pair.Key), pair.Value.Trim());
		}
		Accolades = AccoladesSurrogate.ToFrozenDictionary();

		await foreach (GuildUserInfo? item in Client.Rest.SearchGuildUsersAsync(GuildID))
		{
			MemberList.Add(item.User.Id, new(item));
		}
		MemberList = MemberList.Reverse().ToDictionary();
	}
}

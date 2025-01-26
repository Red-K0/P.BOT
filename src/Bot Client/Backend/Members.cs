using System.Collections.Frozen;
using System.Text;

namespace Bot.Backend;

/// <summary>
/// Contains the member list, accolade list, and founder lists.
/// </summary>
internal static class Members
{
	/// <summary>
	/// Populates the <see cref="Accolades"/> and <see cref="FounderRoles"/> lists from their respective files.
	/// </summary>
	static Members()
	{
		Accolades = new Dictionary<ulong, string>(Files.GetDictionary(Files.Names.Accoaldes).Result
			.Select(static p => new KeyValuePair<ulong, string>(ulong.Parse(p.Key), p.Value.Trim())))
			.ToFrozenDictionary();

		Dictionary<ulong, Role> founderRoles = [];
		foreach (KeyValuePair<string, string> pair in Files.GetDictionary(Files.Names.FounderRoles).Result)
		{
			ulong RoleID = Convert.ToUInt64(pair.Value);
			founderRoles.Add(Convert.ToUInt64(pair.Key), Core.Guild.Roles[RoleID != 0 ? RoleID : Files.GetCounter(Files.CounterLines.FounderID).Result]);
		}
		FounderRoles = founderRoles.ToFrozenDictionary();
	}

	/// <summary>
	/// Used for creating event role accolade strings efficiently.
	/// </summary>
	private static readonly StringBuilder _accoladeBuilder = new(512);

	/// <summary>
	/// The event role description list, indexed by role ID.
	/// </summary>
	public static FrozenDictionary<ulong, string> Accolades { get; }

	/// <summary>
	/// The founder role list, indexed by owner ID.
	/// </summary>
	public static FrozenDictionary<ulong, Role> FounderRoles { get; }

	/// <summary>
	/// The member list, generated with <see cref="RestClient.SearchGuildUsersAsync(ulong, GuildUsersSearchPaginationProperties?, RestRequestProperties?)"/>.
	/// </summary>
	public static OrderedDictionary<ulong, Member> MemberList = [];

	/// <summary>
	/// Returns a string summarizing a member's accolades and other important roles.
	/// </summary>
	/// <param name="member">The member to return accolades for.</param>
	public static async Task<string> GetAccolades(Member member)
	{
		string index = (MemberList.IndexOf(member.Info.User.Id) + 1).ToString();

		if (index.Length == 2 && index[^2] == '1')
		{
			index += "th";
		}
		else
		{
			index += index[^1] switch { '1' => "st", '2' => "nd", '3' => "rd", _ => "th" };
		}

		_accoladeBuilder.Clear();

		_accoladeBuilder.Append($">>> <@&{(member.IsFounder
			? member.PersonalRole!.Id
			: await Files.GetCounter(Files.CounterLines.CultistID))}>\n- The {index} member.");

		foreach (ulong role in member.Info.User.RoleIds.Where(Accolades.ContainsKey)) _accoladeBuilder.Append($"\n<@&{role}>\n- {Accolades[role]}");

		return _accoladeBuilder.ToString();
	}

	/// <summary>
	/// Initializes the class' member list using <see cref="RestClient.SearchGuildUsersAsync(ulong, GuildUsersSearchPaginationProperties?, RestRequestProperties?)"/>.
	/// </summary>
	public static async Task LoadMemberList()
	{
		List<GuildUserInfo?> memberList = new(100);

		await foreach (GuildUserInfo? item in Client.Rest.SearchGuildUsersAsync(GuildID)) memberList.Add(item);

		memberList.Reverse();
		MemberList.EnsureCapacity(memberList.Count);

		MemberList.Clear();
		foreach (GuildUserInfo? info in memberList) if (info != null) MemberList.Add(info.User.Id, new(info));
	}

	/// <summary>
	/// Represents a server member, consists of a <see cref="GuildUserInfo"/> object with additional bot-related fields.
	/// </summary>
	/// <param name="info">The <see cref="GuildUserInfo"/> to construct the object around.</param>
	public sealed class Member(GuildUserInfo info)
	{
		/// <summary>
		/// The member's guild information.
		/// </summary>
		public GuildUserInfo Info = info;

		/// <summary>
		/// Whether the member is considered a server founder.
		/// </summary>
		public bool IsFounder = FounderRoles.ContainsKey(info.User.Id);

		/// <summary>
		/// The member's display name.
		/// </summary>
		public string DisplayName => (Info.User.Nickname ?? Info.User.GlobalName ?? Info.User.Username).FromEscapedUnicode().ToEscapedMarkdown();

		/// <summary>
		/// The member's personal role, if they have one.
		/// </summary>
		public Role? PersonalRole => IsFounder ? FounderRoles[Info.User.Id] : null;

		/// <summary>
		/// Whether the member's last message started with a markdown heading modifier '<c>#</c>'.
		/// </summary>
		public bool SpamLastMessageHeading { get; set; }

		/// <summary>
		/// How many times in a row the member's sent the same message.
		/// </summary>
		public int SpamSameMessageCount { get; set; }

		/// <summary>
		/// The size of the member's last sent attachment.
		/// </summary>
		public int LastAttachmentSize { get; set; }

		/// <summary>
		/// The member's last sent message.
		/// </summary>
		public string LastMessage { get; set; } = "";
	}
}

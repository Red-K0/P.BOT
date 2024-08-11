using BitFaster.Caching.Lru;
using System.Collections.Frozen;
using System.Text;

namespace PBot.Caches;

/// <summary>
/// Contains the recent messages cache and its relevant methods.
/// </summary>
internal static class Messages
{
	/// <summary>
	/// Contains all messages in the bot's cache.
	/// </summary>
	public static ConcurrentLru<ulong, Message> Recent { get; } = new(capacity: 10000);

	/// <summary>
	/// The index of the last assigned <see cref="IgnoreCache"/> slot.
	/// </summary>
	private static sbyte IgnoreCounter = -1;

	/// <summary>
	/// A short array of messages to be ignored by <see cref="Events.MessageDelete(MessageDeleteEventArgs)"/> to prevent clutter.
	/// </summary>
	public static readonly ulong[] IgnoreCache = [0, 0, 0, 0, 0];

	/// <summary>
	/// Adds a message ID to the <see cref="IgnoreCache"/>.
	/// </summary>
	public static void IgnoreID(ulong id)
	{
		if (IgnoreCounter == 4) IgnoreCounter = -1;
		IgnoreCounter++;
		IgnoreCache[IgnoreCounter] = id;
	}
}

/// <summary>
/// Contains the cached members list, role list, and their relevant methods. This class possesses a static constructor not shipped in the GitHub repo for privacy reasons.
/// </summary>
internal static partial class Members
{
	/// <summary>
	/// Represents a <see cref="GuildUserInfo"/> object with additional bot-related fields.
	/// </summary>
	/// <param name="info">The <see cref="GuildUserInfo"/> to base the object on.</param>
	public sealed class Member(GuildUserInfo info)
	{
		public GuildUserInfo Info = info;

		public bool IsFounder = FounderRoles.ContainsKey(info.User.Id);

		public string DisplayName => (Info.User.Nickname ?? Info.User.GlobalName ?? Info.User.Username).ToParsedUnicode().ToEscapedMarkdown();
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
	public static Dictionary<ulong, Member> List { get; private set; } = [];

	/// <summary>
	/// The bot's internal list of event roles, and their appropriate descriptions.
	/// </summary>
	public static readonly FrozenDictionary<ulong, string> Accolades;

	/// <summary>
	/// The bot's internal list of founder roles, alongside their owners' user IDs.
	/// </summary>
	public static readonly FrozenDictionary<ulong, Role> FounderRoles;

	/// <summary>
	/// The bot's internal role list, generated with <see cref="RestClient.GetGuildRolesAsync(ulong, RestRequestProperties?)"/>.
	/// </summary>
	public static readonly Dictionary<ulong, Role> Roles;

	/// <summary>
	/// Checks if the role referenced by the ID is an event role.
	/// </summary>
	/// <param name="id"> The ID to check. </param>
	public static bool IsAccolade(ulong id) => Accolades.ContainsKey(id);

	/// <summary>
	/// Returns a string summarizing a user's accolades and other important roles.
	/// </summary>
	public static string GetUserAccolades(Member member)
	{
		string index = (Array.IndexOf([.. List.Keys], member.Info.User.Id) + 1).ToString();

		if (index.Length == 2 && index[^2] == '1')
		{
			index += "th";
		}
		else
		{
			index += index[^1] switch
			{
				'1' => "st",
				'2' => "nd",
				'3' => "rd",
				_ => "th"
			};
		}

		StringBuilder AccoladeString = new($">>> <@&{(member.IsFounder ? FounderRoleId : CultistRoleId)}>\n- The {index} member.");
		foreach (ulong role in member.Info.User.RoleIds.Where(IsAccolade)) AccoladeString.Append($"\n<@&{role}>\n- {Accolades[role]}");
		return AccoladeString.ToString();
	}

	/// <summary>
	/// Initializes the class with data from the <c>members-search</c> endpoint.
	/// </summary>
	public static void Load()
	{
		IEnumerable<KeyValuePair<ulong, Member>> TempList = [];
		foreach (var item in Client.Rest.SearchGuildUsersAsync(GuildID).ToBlockingEnumerable()) TempList = TempList.Append(new KeyValuePair<ulong, Member>(item.User.Id, new(item)));
		List = TempList.Reverse().ToDictionary();
	}
}
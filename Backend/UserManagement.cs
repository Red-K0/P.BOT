using System.Net.Http.Json;
using System.Text.RegularExpressions;
namespace P_BOT;

internal static partial class UserManagement
{
	private static string MembersSearch = "";
	public static Response[] MemberList = [];

	/// <summary> Initializes the class with data from the <c>members_search</c> endpoint. </summary>
	public static async void InitMembersSearch()
	{
		Dictionary<string, int> dict = []; dict.Add("limit", 500);
		JsonContent Content = JsonContent.Create(dict);
		var x = await client.Rest.SendRequestAsync(HttpMethod.Post, Content, $"/guilds/{1131100534250680433}/members-search", resourceInfo: new TopLevelResourceInfo(1131100534250680433));
		byte[] response = new byte[x.Length];
		x.Read(response, 0, (int)x.Length);
		MembersSearch = System.Text.Encoding.Default.GetString(response);

		int max = MemberCountRegex().Matches(MembersSearch).Count + 2;
		for (int i = 1; i < max; i++)
		{
			MemberList = [.. MemberList, GetMembersSearch(i)];
		}
	}

	/// <summary> Gets the relevant Response object for a specified server member. </summary>
	public static Response GetMembersSearch(int Member = -1)
	{
		string str = MembersSearch;
		str = str[(str.IndexOf('[') + 1)..str.LastIndexOf(']')];

		int offset = 0, oldoffset = 0;
		if (Member != -1)
		{
			for (int i = 0; i < Member; i++)
			{
				oldoffset = offset + 3;
				offset = str.IndexOf(",{\"member\"", offset + 3);
			}
			oldoffset -= 2;
			if (offset == -1) str = str[(oldoffset)..]; else str = str[oldoffset..offset];
		}
		str = str.Replace(",{\"member\"", ",\n\n{\"member\"");

		return Parse(str);

		static Response Parse(string str)
		{
			string Iterator(bool Date = false, bool UserSkip = false, bool AvatarSkip = false, bool Assets = false)
			{
				string response;
				if    (UserSkip) str = str[(str.IndexOf(':') + 1)..];
				if (!AvatarSkip) str = str[(str.IndexOf(':') + 1)..];

				if (!Assets && str.Contains(',')) response = str.Remove(str.IndexOf(','));
				else
				if (Assets) { response = str.Remove(str.IndexOf(",\"b")); }
				else
				response = str;

				response = response.Replace("}", "");
				if (Date && response.Contains(':')) str = str[str.IndexOf(',')..];

				if (Assets && response.Contains("sku_id")) str = str[(str.IndexOf(",\"b") + 10)..];

				if (response.Equals("null", StringComparison.Ordinal)) return null!;
				return response.Replace("\"", "");
			}
			ulong[] RoleArray(string setstring)
			{
				ulong[] Roles = [];
				setstring = setstring[1..^1];

				for (int i = 0; i < setstring.Count(f => f == ',') + 1; i++)
				{
					if (setstring.Contains(','))
					{
						Roles = [.. Roles, Convert.ToUInt64(setstring.Remove(','))];
						setstring = setstring[(setstring.IndexOf(',') + 1)..];
					}
					else
					{
						Roles = [.. Roles, Convert.ToUInt64(setstring)];
					}
				}
				return Roles;
			}

			// Trimming of members header.
			str = str[20..];

			// Use Iterator() to parse the entire resopnse into a Response object.
			return new() {
				Member = new() {
					Avatar = Iterator(AvatarSkip: true),
					CommunicationDisabledUntil = Convert.ToDateTime(Iterator(true)),
					Flags = Convert.ToInt32(Iterator()),
					JoinedAt = Convert.ToDateTime(Iterator(true)),
					Nick = Iterator(),
					Pending = Convert.ToBoolean(Iterator()),
					PremiumSince = Convert.ToDateTime(Iterator(true)),
					Roles = RoleArray(Iterator()),
					UnusualDMActivityUntil = Convert.ToDateTime(Iterator(true)),
					User = new() {
						ID = Convert.ToUInt64(Iterator(UserSkip: true)),
						Username = Iterator(),
						Avatar = Iterator(),
						Discriminator = Convert.ToInt32(Iterator()),
						PublicFlags = Convert.ToInt32(Iterator()),
						PremiumType = Convert.ToInt32(Iterator()),
						Flags = Convert.ToInt32(Iterator()),
						Banner = Iterator(),
						AccentColor = Convert.ToInt32(Iterator()),
						GlobalName = Iterator(),
						AvatarDecorationData = Iterator(Assets: true),
						BannerColor = Convert.ToInt32(Iterator())
					},
					Mute = Convert.ToBoolean(Iterator()),
					Deaf = Convert.ToBoolean(Iterator())
				},
				SourceInviteCode = Iterator(),
				JoinSourceType = Convert.ToInt32(Iterator()),
				InviterID = Convert.ToUInt64(Iterator()),
			};
		}
	}

	public struct Response
	{
		public Member Member { get; set; }
		public string SourceInviteCode { get; set; }
		public int JoinSourceType { get; set; }
		public ulong InviterID { get; set; }
	}
	public struct Member
	{
		public string Avatar { get; set; }
		public DateTime CommunicationDisabledUntil { get; set; }
		public int Flags { get; set; }
		public DateTime JoinedAt { get; set; }
		public string Nick { get; set; }
		public bool Pending { get; set; }
		public DateTime PremiumSince { get; set; }
		public ulong[] Roles { get; set; }
		public DateTime UnusualDMActivityUntil { get; set; }
		public User User { get; set; }
		public bool Mute { get; set; }
		public bool Deaf { get; set; }
	}
	public struct User
	{
		public ulong ID { get; set; }
		public string Username { get; set; }
		public string Avatar { get; set; }
		public int Discriminator { get; set; }
		public int PublicFlags { get; set; }
		public int Flags { get; set; }
		public int PremiumType { get; set; }
		public string Banner { get; set; }
		public int AccentColor { get; set; }
		public string GlobalName { get; set; }
		public string AvatarDecorationData { get; set; }
		public int BannerColor { get; set; }
	}

	[GeneratedRegex(",{\"member\"")]
	private static partial Regex MemberCountRegex();
}

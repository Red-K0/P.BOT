namespace PBot.Caches;
internal static partial class Members
{
	public sealed class Member
	{
		public Member(GuildUserInfo userObject)
		{
			Data = userObject ?? throw new ArgumentNullException(nameof(userObject));
			IsFounder = FounderIDs.Contains(userObject.User.Id);
			PersonalRole = IsFounder ? FounderRoles[Array.IndexOf(FounderIDs, userObject.User.Id)] : Cultist;
			PersonalRoleColor = (IsFounder && PersonalRole != BaseFounderRole) ? Roles[PersonalRole].Color.RawValue : null;
			DisplayName = (userObject.User.Nickname ?? (userObject.User.GlobalName ?? userObject.User.Username)).ToParsedUnicode().ToEscapedMarkdown();
			SpamLastMessage = "";
			SpamLastAttachmentSize = 0;
		}

		public GuildUserInfo Data    { get; }
		public bool IsFounder        { get; }
		public ulong PersonalRole    { get; }
		public int? PersonalRoleColor { get; }
		public string DisplayName    { get; }

		// Spam Filter
		public bool SpamLastMessageHeading  { get; set; }
		public int SpamSameMessageCount     { get; set; }
		public int SpamLastAttachmentSize   { get; set; }
		public string SpamLastMessage       { get; set; }
	}

	public static string GetUserAccolades(Member? member)
	{
		if (member == null) return "> None";

		string index = (Array.IndexOf([.. List.Keys], member.Data.User.Id) + 1).ToString();

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

		string AccoladeString = $">>> <@&{(member.IsFounder ? BaseFounderRole : Cultist)}>\n- The {index} member of the PPP.";
		foreach (ulong role in member.Data.User.RoleIds.Where(IsAccolade))
		{
			AccoladeString += role switch
			{
				SemiAnniversary => $"\n<@&{SemiAnniversary}>\n- Attended the 6-Month Server Anniversary",
				HyakkanoEnjoyer => $"\n<@&{HyakkanoEnjoyer}>\n- Read the first 50 Chapters of 100 Girlfriends",
				CelesteSpeedrun => $"\n<@&{CelesteSpeedrun}>\n- Finish a celeste speedrun in under 50 minutes",
				GoldenJadeBerry => $"\n<@&{GoldenJadeBerry}>\n- Helped Jade pick out proposal ring, limited-time",
				SecretSanta2023 => $"\n<@&{SecretSanta2023}>\n- Sent a Secret Santa Gift in 2023",
				_ => ""
			};
		}
		return AccoladeString;
	}

	#region IDs
	private static readonly ulong[] FounderIDs =
	[
		916208242382753833,  // Lu
		456226577798135808,  // Kira
		689505671183597636,  // Fifer
		756630474222338100,  // Cash
		994585563577462795,  // Court
		1102683500169138247, // Zeb
		1124777547687788626, // Eun
		542121504796573749,  // Maho
		1094719984820813977, // Blue
		382295809292500994,  // Yetti
		184004211799752705,  // JD
		617913771888738314,  // Charlie
		807236715118723094,  // dumdum
		1126943628535799828, // Jade
		998011405594808340,  // Sofi
		1157673662686699641, // BunnySis
		770764486763216976,  // Soup
		947297809730777148,  // Rae
		1096090443478990979, // Pixi
		1141261327973761076, // Angel
		1144066017283289118  // Marie
	];

	private static readonly ulong[] FounderRoles =
	[
		BaseFounderRole,     // Lu
		BaseFounderRole,     // Kira
		1146513857272102942, // Fifer
		1133926760598225016, // Cash
		1131949116293922958, // Court
		BaseFounderRole,     // Zeb
		1131107716388102214, // Eun
		BaseFounderRole,     // Maho
		1133946248945205319, // Blue
		1133928290529325136, // Yetti
		BaseFounderRole,     // JD
		1136362136562577468, // Charlie
		1215419073425571952, // dumdum
		1146493991760695328, // Jade
		BaseFounderRole,     // Sofi
		1146497280476708924, // BunnySis
		1146530881025101994, // Soup
		1147538420344815636, // Rae
		1147560956835139597, // Pixi
		1185966727977246780, // Angel
		BaseFounderRole      // Marie
	];

	private static readonly ulong[] Accolades =
	[
		SemiAnniversary,
		HyakkanoEnjoyer,
		CelesteSpeedrun,
		GoldenJadeBerry,
		SecretSanta2023,
	];

	private const ulong
					Cultist = 1155447510823874621,
			BaseFounderRole = 1147935997770862612,
		SemiAnniversary = 1200608387835121706,
			SecretSanta2023 = 1193330313162657934,
			HyakkanoEnjoyer = 1190873907503304754,
			CelesteSpeedrun = 1219835818357686282,
			GoldenJadeBerry = 1233161143036411926;
	#endregion
}
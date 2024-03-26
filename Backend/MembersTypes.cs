namespace P_BOT;
internal static partial class Members
{
	// XML Structure Types
	private struct Response
	{
		public Member Member			{ get; set; }
		public string SourceInviteCode	{ get; set; }
		public int JoinSourceType		{ get; set; }
		public ulong InviterID			{ get; set; }
	}
	private struct Member
	{
		public string AvatarHash					{ get; set; }
		public DateTime CommunicationDisabledUntil	{ get; set; }
		public int Flags							{ get; set; }
		public DateTime JoinedAt					{ get; set; }
		public string Nick							{ get; set; }
		public bool Pending							{ get; set; }
		public DateTime PremiumSince				{ get; set; }
		public ulong[] Roles						{ get; set; }
		public DateTime UnusualDMActivityUntil		{ get; set; }
		public User User							{ get; set; }
		public bool Mute							{ get; set; }
		public bool Deaf							{ get; set; }
	}
	private struct User
	{
		public ulong ID						{ get; set; }
		public string Username				{ get; set; }
		public string AvatarHash			{ get; set; }
		public int Discriminator			{ get; set; }
		public int PublicFlags				{ get; set; }
		public int Flags					{ get; set; }
		public int PremiumType				{ get; set; }
		public string Banner				{ get; set; }
		public int AccentColor				{ get; set; }
		public string GlobalName			{ get; set; }
		public string AvatarDecorationData	{ get; set; }
		public int BannerColor				{ get; set; }
	}

	// Final User Object
	public struct UserObject
	{
		public ulong ID						{ get; set; }
		public string Username				{ get; set; }
		public string GlobalName			{ get; set; }
		public int Discriminator			{ get; set; }
		public PremiumType PremiumType		{ get; set; }
		public UserFlags PublicFlags		{ get; set; }
		public Customization Customization	{ get; set; }
		public Invite Invite				{ get; set; }
		public Server Server				{ get; set; }
	}
	public struct Invite
	{
		public string Code		{ get; set; }
		public int Type			{ get; set; }
		public ulong SenderID	{ get; set; }
	}
	public struct Customization
	{
		public int AccentColor				{ get; set; }
		public string AvatarHash			{ get; set; }
		public string AvatarDecorationData	{ get; set; }
		public string Banner				{ get; set; }
		public int BannerColor				{ get; set; }
		public ulong PersonalRole			{ get; set; }
		public int PersonalRoleColor		{ get; set; }
	}
	public struct Server
	{
		public string AvatarHash				{ get; set; }
		public DateTime MutedUntil				{ get; set; }
		public GuildMemberFlags Flags						{ get; set; }
		public DateTime JoinedAt				{ get; set; }
		public string Nickname					{ get; set; }
		public bool Verified					{ get; set; }
		public DateTime NitroSince				{ get; set; }
		public ulong[] Roles					{ get; set; }
		public DateTime UnusualDMActivityUntil	{ get; set; }
		public bool IsVCMuted					{ get; set; }
		public bool IsVCDeafened				{ get; set; }
	}

	// Data Enumerations

	/// <summary>
	/// The user's guild flags.
	/// </summary>
	public enum GuildMemberFlags
	{
		DID_REJOIN				= 1 << 0,
		COMPLETED_ONBOARDING	= 1 << 1,
		BYPASSES_VERIFICATION	= 1 << 2,
		STARTED_ONBOARDING		= 1 << 3
	}

	/// <summary>
	/// The user's account flags.
	/// </summary>
	public enum UserFlags : long
	{
		/// <summary> User is a discord employee. </summary>
		STAFF                        = (long) 1 << 00,

		/// <summary> User is a discord partner. </summary>
		PARTNER                      = (long) 1 << 01,

		/// <summary> User has the 'HypeSquad Events' badge. </summary>
		HYPESQUAD                    = (long) 1 << 02,

		/// <summary> User has the 'Bug Hunter' badge. </summary>
		BUG_HUNTER_LEVEL_1           = (long) 1 << 03,

		/// <summary> User has SMS recovery for 2FA enabled. </summary>
		MFA_SMS                      = (long) 1 << 04,

		/// <summary> User dismissed the Nitro promotion. </summary>
		PREMIUM_PROMO_DISMISSED      = (long) 1 << 05,

		/// <summary> User is a House of Bravery Member. </summary>
		HYPERSQUAD_ONLINE_HOUSE_1    = (long) 1 << 06,

		/// <summary> User is a House of Brilliance Member. </summary>
		HYPERSQUAD_ONLINE_HOUSE_2    = (long) 1 << 07,

		/// <summary> User is a House of Balance Member. </summary>
		HYPERSQUAD_ONLINE_HOUSE_3    = (long) 1 << 08,

		/// <summary> User has the 'Early Supporter' badge. </summary>
		PREMIUM_EARLY_SUPPORTER      = (long) 1 << 09,

		/// <summary> User is a team. See <see href="https://discord.com/developers/docs/topics/teams"/>. </summary>
		TEAM_PSEUDO_USER             = (long) 1 << 10,

		/// <summary> User has a pending partner/verification application. </summary>
		INTERNAL_APPLICATION         = (long) 1 << 11,

		/// <summary> User is the SYSTEM account. </summary>
		SYSTEM                       = (long) 1 << 12,

		/// <summary> User has unread messages from Discord. </summary>
		HAS_UNREAD_URGENT_MESSAGES   = (long) 1 << 13,

		/// <summary> User has the 'Golden Bug Hunter' badge. </summary>
		BUG_HUNTER_LEVEL_2           = (long) 1 << 14,

		/// <summary> User is pending deletion for being underage in DOB prompt. </summary>
		UNDERAGE_DELETED             = (long) 1 << 15,

		/// <summary> User is a verified bot. See <see href="https://support.discord.com/hc/articles/360040720412"/>. </summary>
		VERIFIED_BOT                 = (long) 1 << 16,

		/// <summary> User has the 'Early Verified Developer' badge. </summary>
		VERIFIED_DEVELOPER           = (long) 1 << 17,

		/// <summary> User has the 'Moderator Program Alumni' badge. </summary>
		CERTIFIED_MODERATOR          = (long) 1 << 18,

		/// <summary> User is a bot with an interactions endpoint. </summary>
		BOT_HTTP_INTERACTIONS        = (long) 1 << 19,

		/// <summary> User's account is disabled for spam. </summary>
		SPAMMER                      = (long) 1 << 20,

		/// <summary> User's Nitro features are disabled. </summary>
		DISABLE_PREMIUM              = (long) 1 << 21,

		/// <summary> User has the 'Active Developer' badge. See <see href="https://support-dev.discord.com/hc/articles/10113997751447"/>. </summary>
		ACTIVE_DEVELOPER             = (long) 1 << 22,

		/// <summary> User's account has a high global rate limit. </summary>
		HIGH_GLOBAL_RATE_LIMIT       = (long) 1 << 33,

		/// <summary> User's account is deleted. </summary>
		DELETED                      = (long) 1 << 34,

		/// <summary> User's account is disabled for suspicious activity. </summary>
		DISABLED_SUSPICIOUS_ACTIVITY = (long) 1 << 35,

		/// <summary> User's account was manually deleted. </summary>
		SELF_DELETED                 = (long) 1 << 36,

		/// <summary> User has a manually selected discriminator. </summary>
		[Obsolete]
		PREMIUM_DISCRIMINATOR        = (long) 1 << 37,

		/// <summary> User has used the desktop client. </summary>
		USED_DESKTOP_CLIENT          = (long) 1 << 38,

		/// <summary> User has used the web client. </summary>
		USED_WEB_CLIENT              = (long) 1 << 39,

		/// <summary> User has used the mobile client. </summary>
		USED_MOBILE_CLIENT           = (long) 1 << 40,

		/// <summary> User's account is temporarily or permanently disabled. </summary>
		DISABLED                     = (long) 1 << 41,

		/// <summary> User has a verified email. </summary>
		VERIFIED_EMAIL               = (long) 1 << 43,

		/// <summary> User is quarantined. See <see href="https://support.discord.com/hc/articles/6461420677527"/>. </summary>
		QUARANTINED                  = (long) 1 << 44,

		/// <summary> User is a collaborator and has staff permissions. </summary>
		COLLABORATOR                 = (long) 1 << 50,

		/// <summary> User is a restricted collaborator and has staff permissions. </summary>
		RESTRICTED_COLLABORATOR      = (long) 1 << 51
	}

	/// <summary>
	/// The user's type of nitro subscription.
	/// </summary>
	public enum PremiumType
	{
		/// <summary> No nitro subscription. </summary>
		None = 0,
		/// <summary> Nitro Classic subscription. </summary>
		NitroClassic = 1,
		/// <summary> Standard Nitro subscription. </summary>
		Nitro = 2,
		/// <summary> Nitro Basic subscription. </summary>
		NitroBasic = 3
	}

	#region IDs
	private static readonly ulong[] FounderIDs =
	[
		    Lu_ID,  Kira_ID, Fifer_ID,     Cash_ID, // 4
		 Court_ID,   Zeb_ID,   Eun_ID,     Maho_ID, // 8
		  Blue_ID, Yetti_ID,    JD_ID,  Charlie_ID, // 12
		dumdum_ID,  Jade_ID,  Sofi_ID, BunnySis_ID, // 16
		  Soup_ID,   Rae_ID,  Pixi_ID,    Angel_ID, // 20
		 Marie_ID // Marie
	];

	private static readonly ulong[] FounderRoleIDs =
	[
		    Lu_RoleID,  Kira_RoleID, Fifer_RoleID,     Cash_RoleID, // 4
		 Court_RoleID,   Zeb_RoleID,   Eun_RoleID,     Maho_RoleID, // 8
		  Blue_RoleID, Yetti_RoleID,    JD_RoleID,  Charlie_RoleID, // 12
		dumdum_RoleID,  Jade_RoleID,  Sofi_RoleID, BunnySis_RoleID, // 16
		  Soup_RoleID,   Rae_RoleID,  Pixi_RoleID,    Angel_RoleID, // 20
		 Marie_RoleID // Marie

	];

	private static readonly ulong[] EventIDs =
	[
		SixMonthAnniversary,
		SecretSanta2023,
		HyakkanoEnjoyer,
		CelesteSpeedrun
	];

	private const ulong
		      Lu_ID = 916208242382753833,        Lu_RoleID = 0,
		    Kira_ID = 456226577798135808,      Kira_RoleID = 0,
		   Fifer_ID = 689505671183597636,     Fifer_RoleID = 1146513857272102942,
			Cash_ID = 756630474222338100,      Cash_RoleID = 1133926760598225016,
		   Court_ID = 994585563577462795,     Court_RoleID = 1131949116293922958,
			 Zeb_ID = 1102683500169138247,      Zeb_RoleID = 0,
			 Eun_ID = 1124777547687788626,      Eun_RoleID = 1131107716388102214,
			Maho_ID = 542121504796573749,      Maho_RoleID = 0,
			Blue_ID = 1094719984820813977,     Blue_RoleID = 1133946248945205319,
		   Yetti_ID = 382295809292500994,     Yetti_RoleID = 1133928290529325136,
			  JD_ID = 184004211799752705,        JD_RoleID = 0,
		 Charlie_ID = 617913771888738314,   Charlie_RoleID = 1136362136562577468,
		  dumdum_ID = 807236715118723094,    dumdum_RoleID = 1215419073425571952,
			Jade_ID = 1126943628535799828,     Jade_RoleID = 1146493991760695328,
			Sofi_ID = 998011405594808340,      Sofi_RoleID = 0,
		BunnySis_ID = 1157673662686699641, BunnySis_RoleID = 1146497280476708924,
			Soup_ID = 770764486763216976,      Soup_RoleID = 1146530881025101994,
			 Rae_ID = 947297809730777148,       Rae_RoleID = 1147538420344815636,
			Pixi_ID = 1096090443478990979,     Pixi_RoleID = 1147560956835139597,
		   Angel_ID = 1141261327973761076,    Angel_RoleID = 1185966727977246780,
		   Marie_ID = 1144066017283289118,    Marie_RoleID = 0;

	public const ulong
					Cultist = 1155447510823874621,
					Founder = 1147935997770862612,
		SixMonthAnniversary = 1200608387835121706,
			SecretSanta2023 = 1193330313162657934,
			HyakkanoEnjoyer = 1190873907503304754,
			CelesteSpeedrun = 1219835818357686282;
	#endregion
}
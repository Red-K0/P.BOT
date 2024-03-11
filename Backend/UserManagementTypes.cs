namespace P_BOT;
internal static partial class UserManagement
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
		public PublicFlags PublicFlags		{ get; set; }
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
		public Flags Flags						{ get; set; }
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
	public enum Flags
	{
		DID_REJOIN				= 1 << 0,
		COMPLETED_ONBOARDING	= 1 << 1,
		BYPASSES_VERIFICATION	= 1 << 2,
		STARTED_ONBOARDING		= 1 << 3
	}
	public enum PublicFlags
	{
		STAFF						= 1 << 00,
		PARTNER						= 1 << 01,
		HYPESQUAD					= 1 << 02,
		BUG_HUNTER_LEVEL_1			= 1 << 03,
		HYPERSQUAD_ONLINE_HOUSE_1	= 1 << 06,
		HYPERSQUAD_ONLINE_HOUSE_2	= 1 << 07,
		HYPERSQUAD_ONLINE_HOUSE_3	= 1 << 08,
		PREMIUM_EARLY_SUPPORTER		= 1 << 09,
		TEAM_PSEUDO_USER			= 1 << 10,
		BUG_HUNTER_LEVEL_2			= 1 << 14,
		VERIFIED_BOT				= 1 << 16,
		VERIFIED_DEVELOPER			= 1 << 17,
		CERTIFIED_MODERATOR			= 1 << 18,
		BOT_HTTP_INTERACTIONS		= 1 << 19,
		ACTIVE_DEVELOPER			= 1 << 22
	}
	public enum PremiumType
	{
		None = 0,
		NitroClassic = 1,
		Nitro = 2,
		NitroBasic = 3
	}

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

	#region IDs
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
	#endregion

}
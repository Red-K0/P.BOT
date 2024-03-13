using System.Net.Http.Json;
using System.Text.RegularExpressions;
using P_BOT.Backend;
namespace P_BOT;

internal static partial class UserManagement
{
	[GeneratedRegex(",{\"member\"")]
	private static partial Regex MemberCountRegex();
	public static Dictionary<ulong, UserObject> MemberList = [];

	/// <summary>
	/// Initializes the class with data from the <c>members_search</c> endpoint.
	/// </summary>
	public static async void MembersSearch()
	{
#if DEBUG
		Stopwatch Timer = Stopwatch.StartNew();
#endif

		Dictionary<string, int> dict = []; dict.Add("limit", 500);
		JsonContent Content = JsonContent.Create(dict);
		Stream x = await client.Rest.SendRequestAsync(HttpMethod.Post, Content, $"/guilds/{1131100534250680433}/members-search", resourceInfo: new TopLevelResourceInfo(1131100534250680433));

		byte[] response = new byte[x.Length]; x.Read(response, 0, (int)x.Length);
		await x.DisposeAsync();

		string MembersSearch = System.Text.Encoding.Default.GetString(response);
		int max = MemberCountRegex().Matches(MembersSearch).Count + 2;
		Response[] WorkingArray = [];

		for (int i = 1; i < max; i++)
		{
			WorkingArray = [.. WorkingArray, GetMembersSearch(i)];
		}

		// Unicode Escape Repair
		for (int i = 0; i < WorkingArray.Length; i++)
		{
			Member @Member = WorkingArray[i].Member;
			User @User = Member.User;

			Member.Nick = Parsing.EscapedUnicode(Member.Nick);
			User.Username = Parsing.EscapedUnicode(User.Username);
			User.GlobalName = Parsing.EscapedUnicode(User.GlobalName);

			Member.User = User;
			WorkingArray[i].Member = Member;
		}

		ParseResponse(WorkingArray.Reverse().ToArray());

#if DEBUG
		Messages.Logging.AsVerbose($"Member List Ready ({max - 1} members loaded) [{Timer.ElapsedMilliseconds}ms]");
		Timer.Reset();
#endif

		Response GetMembersSearch(int Member = -1)
		{
			string str = MembersSearch;
			str = str[(str.IndexOf('[') + 1)..str.LastIndexOf(']')];

			int Offset = 0, OldOffset = 0;
			if (Member != -1)
			{
				for (int i = 0; i < Member; i++)
				{
					OldOffset = Offset + 3;
					Offset = str.IndexOf(",{\"member\"", Offset + 3);

#if DEBUG_OFFSET
			if (offset == -1) offset = str.Length;
			Messages.Logging.AsVerbose($"Offset {i:D2} found at 0x{offset:X4}. Previous offset was at 0x{oldoffset:X4}.");
#endif
				}
				OldOffset -= 2;
				str = Offset == -1 ? str[OldOffset..] : str[OldOffset..Offset];

#if DEBUG_OFFSET
			Messages.Logging.AsVerbose($"""

				All {Member:D2} offsets calculated succesfully. Final result was between offsets 0x{oldoffset:X4} and 0x{offset:X4}. Result was:
				{str}

				Break (Y/N)?
				""");
			if (Console.ReadKey(true).Key == ConsoleKey.Y) Debugger.Break();
#endif
			}
			str = str.Replace(",{\"member\"", ",\n\n{\"member\"");

			return Parse(str);

			static Response Parse(string str)
			{
				string? Iterator(bool Date = false, bool UserSkip = false, bool AvatarSkip = false, bool Assets = false, bool RoleSkip = false)
				{
					string response;

					if (RoleSkip)
					{
						response = str[str.IndexOf('[')..];
						response = response.Remove(response.IndexOf(']'));
						str = str[(str.IndexOf("],") + 2)..];
						return response;
					}

					if (UserSkip)
					{
						str = str[(str.IndexOf(':') + 1)..];
					}

					if (!AvatarSkip)
					{
						str = str[(str.IndexOf(':') + 1)..];
					}

					response = !Assets && str.Contains(',') ? str.Remove(str.IndexOf(',')) : Assets ? str.Remove(str.IndexOf(",\"b")) : str;

					response = response.Replace("}", "");
					if (Date && response.Contains(':'))
					{
						str = str[str.IndexOf(',')..];
					}

					if (Assets && response.Contains("sku_id"))
					{
						str = str[(str.IndexOf(",\"b") + 10)..];
					}

					return response.Equals("null", StringComparison.Ordinal) ? null : response.Replace("\"", "");
				}
				ulong[] ToRoleArray(string RoleString)
				{
					ulong[] Roles = [];
					RoleString = RoleString[1..^1];

					RoleString = RoleString.Replace("\"", "");
					int max = RoleString.Count(f => f == ',') + 1;

					for (int i = 0; i < max; i++)
					{
						if (RoleString.Contains(','))
						{
							Roles = [.. Roles, Convert.ToUInt64(RoleString.Remove(RoleString.IndexOf(',')))];
							RoleString = RoleString[(RoleString.IndexOf(',') + 1)..];
						}
						else
						{
							Roles = [.. Roles, Convert.ToUInt64(RoleString)];
						}
					}
					return Roles;
				}

				// Trimming of members header.
				str = str[20..];

#pragma warning disable CS8601, CS8604

				// Use Iterator() to parse the entire response into a Response object.
				return new()
				{
					Member = new()
					{
						AvatarHash = Iterator(AvatarSkip: true),
						CommunicationDisabledUntil = Convert.ToDateTime(Iterator(true)),
						Flags = Convert.ToInt32(Iterator()),
						JoinedAt = Convert.ToDateTime(Iterator(true)),
						Nick = Iterator(),
						Pending = Convert.ToBoolean(Iterator()),
						PremiumSince = Convert.ToDateTime(Iterator(true)),
						Roles = ToRoleArray(Iterator(RoleSkip: true)),
						UnusualDMActivityUntil = Convert.ToDateTime(Iterator(true)),
						User = new()
						{
							ID = Convert.ToUInt64(Iterator(UserSkip: true)),
							Username = Iterator(),
							AvatarHash = Iterator(),
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

#pragma warning restore
			}
		}
	}

	private static void ParseResponse(Response[] WorkingArray)
	{
		IReadOnlyDictionary<ulong, Role> Roles = client.Rest.GetGuildRolesAsync(Convert.ToUInt64(SERVER_LINK[29..^1])).Result;

		foreach (Response Response in WorkingArray)
		{
			Response OldUser = Response;
			User @User = OldUser.Member.User;
			Member @Member = OldUser.Member;
			UserObject NewUser = new()
			{
				ID = User.ID,
				Username = User.Username,
				Discriminator = User.Discriminator,
				GlobalName = User.GlobalName,

				Customization = new()
				{
					AccentColor = User.AccentColor,
					AvatarDecorationData = User.AvatarDecorationData,
					AvatarHash = User.AvatarHash,
					Banner = User.Banner,
					BannerColor = User.BannerColor
				},
				Invite = new()
				{
					Code = OldUser.SourceInviteCode,
					SenderID = OldUser.InviterID,
					Type = OldUser.JoinSourceType
				},
				Server = new()
				{
					AvatarHash = Member.AvatarHash,
					Flags = (Flags)Member.Flags,
					IsVCDeafened = Member.Deaf,
					IsVCMuted = Member.Mute,
					MutedUntil = Member.CommunicationDisabledUntil,
					JoinedAt = Member.JoinedAt,
					Nickname = Member.Nick,
					NitroSince = Member.PremiumSince,
					Roles = Member.Roles,
					UnusualDMActivityUntil = Member.UnusualDMActivityUntil,
					Verified = Member.Pending
				},

				PublicFlags = (PublicFlags)User.PublicFlags,
				PremiumType = (PremiumType)User.PremiumType,
			};

			// If the user is a bot, retain their default role.
			ulong PersonalRole = (User.Discriminator == 0) ? 0 : NewUser.Server.Roles[0];

			// If the user is a founder, apply their unique founder role.
			if (FounderIDs.Contains(NewUser.ID))
			{
				PersonalRole = FounderRoleIDs[Array.IndexOf(FounderIDs, User.ID)];
			}

			// Modify struct with new personal role.
			Customization TempCustom = NewUser.Customization;
			TempCustom.PersonalRole = PersonalRole;

			TempCustom.PersonalRoleColor = (PersonalRole != 0 && Roles.TryGetValue(PersonalRole, out Role? TempRole)) ? TempRole.Color.RawValue : -1;
			if (TempCustom.PersonalRoleColor == 0)
			{
				TempCustom.PersonalRoleColor = -1;
			}

			NewUser.Customization = TempCustom;

			MemberList.Add(NewUser.ID, NewUser);
		}
	}

	public static bool IsFounder(ulong ID) => FounderIDs.Contains(ID);
	public static bool IsEventRole(ulong ID) => EventIDs.Contains(ID);
}
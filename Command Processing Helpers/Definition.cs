// The helper class supporting the /define command.

using NetCord.Services.ApplicationCommands;
namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains data involving dictionaries and the <see cref="SlashCommand.GetDefinition(Choices)"/> command.
/// </summary>
public static class Definition
{
	#region Key-Value pair defintions
	/// <summary>
	/// The meaning of the PPP Abbreviation.
	/// </summary>
	private static readonly KeyValuePair<Choices, string> PPP_Meaning = new(Choices.MeaningOf_PPP, """
		'PPP' stands for \"Perfect Protection Polygon\".

		History:
		>>> Originally, the PPP was called the 'Protection Triangle', as it consisted of three members. This was then expanded to the 'Protection Square', 'Protection Hexagon', and upon people's unawareness of what a heptagon is, the 'Protection Polygon'. The 'Perfect' prefix was added for alliteration, resulting in the 'Perfect Protection Polygon' or as its more commonly known, 'The PPP'.
		""");

	/// <summary>
	/// The history of the PPP.
	/// </summary>
	private static readonly KeyValuePair<Choices, string> PPP_History = new(Choices.WhatIs_ThePPP, """
		*The PPP has had a fairly long history, documented here. For a summary, look up 'What does 'PPP' stand for?' instead*

		Originally, the PPP was not a server, or even a thread, instead a concept randomly created in a channel by three of the original founders, Courtney, Eun and Soarin. The channel in question was #lounge in the server Vent Bistro, and the intention was a 'Protection Triangle' for a member known at the time as Kira☆Kira.

		Kira was a particularly innocent member of the server, and as so more and more members joined the triangle. The 4th member, Lu joined after and turned it into a 'Protection Square'. Cash joined next, making it a 'Protection Pentagon'. [Removed] was the 6th member, followed by Mahoroa resulting in it becoming a heptagon.

		At the time, Lu was unaware of what a heptagon was, resulting in simplification of the name to 'Perfect Protection Polygon'. Blue, the 8th member, was forcibly inducted into the polygon, rounding off the original 8 founders: Courtney, Eun, Soarin, Lu, Cash, [Removed], Mahoroa and Blue, and official declaration of the polygon on <t:1689637680> by Courtney.

		After official declaration, the original thread '#\🛡 Perfect Protection Polygon \🛡' was created, and the first official set of rules was written.
		> Rules of the PPP
		> - Forfeit all mortal possessions to @Kira☆Kira
		> - Constantly remind @Kira☆Kira how amazing they are
		> - No server takeover plotting
		> - Whatever @Kira☆Kira demands
		
		After many declarations of war (including a [war tune](https://youtu.be/S-UmqvA7uR8)), Kira☆Kira declared that the members of the PPP "live their lives, eat their fill, and enjoy everything the world has to offer.", and the PPP ended its warmongering era (to much disappointment), and the PPP entered its phase of semi-worship.

		Upon entering this era, the 9th member, jdphenix, joined the ~~cult~~ polygon, and the 10th member, fifer, also joined shortly after. Upon reaching 10 members, the thread has started to become somewhat crowded, and a decision was reached to finally move the PPP into a server of its own.
		
		Lu handled the server creation (despite never actually handling its ownership due to taking a break from discord) and ownership was transferred to Eun, the current owner of the server. After creation, the server continued to slowly grow, gaining more members until eventually, 20 members were reached, and the server hit a relative turning point.
		
		The decision at the time was whether or not to turn the server public, but after much debate, said idea never went through, but moderator elections were held regardless in celebration, resulting in the current mod team and layout. As more members joined, the server continued to grow, and more features / channels were added, expanding the server into its current state.

		At a point in time, @Kira☆Kira decided to leave discord for personal reasons. This put the server in a tough spot, given that the central idea revolved around her. However, due to the server having grown large enough to be capable of sustaining itself, the server instead transformed into more of a hangout spot, rather than its original intent, bringing us to the present state of the server.
		""");
	#endregion

	/// <summary>
	/// Contains definitions used for the <see cref="SlashCommand.GetDefinition(Choices)"/> command.
	/// </summary>
	public static readonly Dictionary<Choices, string> Values = new([PPP_Meaning, PPP_History]);

	/// <summary>
	/// A list of possible choices for the <see cref="SlashCommand.GetDefinition(Choices)"/> command.
	/// </summary>
	public enum Choices
	{
		/// <summary>
		/// The meaning of the PPP Abbreviation.
		/// </summary>
		[SlashCommandChoice(Name = "What does 'PPP' stand for?")] MeaningOf_PPP,

		/// <summary>
		/// The history of the PPP.
		/// </summary>
		[SlashCommandChoice(Name = "What is the PPP?")] WhatIs_ThePPP
	}
}
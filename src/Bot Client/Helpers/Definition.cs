// The helper class supporting the /define command.

using NetCord.Services.ApplicationCommands;
namespace PBot.Commands.Helpers;

/// <summary>
/// Contains data involving dictionaries and the <see cref="SlashCommands.GetDefinition(Choices)"/> command.
/// </summary>
public static class Definition
{
	/// <summary>
	/// A list of possible choices for the <see cref="SlashCommands.GetDefinition(Choices)"/> command.
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
		[SlashCommandChoice(Name = "What is the PPP?")] WhatIs_ThePPP,

		/// <summary>
		/// Angel's Iceland travel guide.
		/// </summary>
		[SlashCommandChoice(Name = "Iceland Travel Guide")] IcelandGuide
	}

	#region Key-Value pair defintions
	private static readonly KeyValuePair<Choices, MessageProperties> Codex_PPP = new(Choices.WhatIs_ThePPP, Embeds.Generate
	(
		"PPP Codex",
		[
			Embeds.CreateField("What does 'PPP' stand for?", """
				**P**erfect **P**rotection **P**olygon
				"""),

			Embeds.CreateField("Where does 'PPP' come from?", """
				Originally, the PPP was not a server, or even a thread, instead a concept randomly created in a channel by three of the original founders, Courtney, Eun and Soarin. The channel in question was #lounge in the server Vent Bistro, and the intention was a 'Protection Triangle' for a member known at the time as Kira☆Kira.

				Kira was a particularly innocent member of the server, and as so more and more members joined the triangle. The 4th member, Lu joined after and turned it into a 'Protection Square'. Cash joined next, making it a 'Protection Pentagon'. [Removed] was the 6th member, followed by Mahoroa resulting in it becoming a heptagon.

				At the time, Lu was unaware of what a heptagon was, resulting in simplification of the name to 'Perfect Protection Polygon'. Blue, the 8th member, was forcibly inducted into the polygon, rounding off the original 8 founders: Courtney, Eun, Soarin, Lu, Cash, [Removed], Mahoroa and Blue, and official declaration of the polygon on <t:1689637680> by Courtney.
				"""),

			Embeds.CreateField("Founding of the PPP", """
				After official declaration, the original thread '#\🛡 Perfect Protection Polygon \🛡' was created, and the first official set of rules was written.
				> Rules of the PPP
				> - Forfeit all mortal possessions to @Kira☆Kira
				> - Constantly remind @Kira☆Kira how amazing they are
				> - No server takeover plotting
				> - Whatever @Kira☆Kira demands
				
				After many declarations of war (including a [war tune](https://youtu.be/S-UmqvA7uR8)), Kira☆Kira declared that the members of the PPP "live their lives, eat their fill, and enjoy everything the world has to offer.", and the PPP ended its warmongering era (to much disappointment), and the PPP entered its phase of semi-worship.
				
				Upon entering this era, the 9th member, jdphenix, joined the ~~cult~~ polygon, and the 10th member, fifer, also joined shortly after. Upon reaching 10 members, the thread has started to become somewhat crowded, and a decision was reached to finally move the PPP into a server of its own.
				"""),

			Embeds.CreateField("Server Creation", """
				Lu handled the server creation (despite never actually handling its ownership due to taking a break from discord) and ownership was transferred to Eun, the current owner of the server. After creation, the server continued to slowly grow, gaining more members until eventually, 20 members were reached, and the server hit a relative turning point.

				The decision at the time was whether or not to turn the server public, but after much debate, said idea never went through, but moderator elections were held regardless in celebration, resulting in the current mod team and layout. As more members joined, the server continued to grow, and more features / channels were added, expanding the server into its current state.

				At a point in time, Kira decided to leave discord for personal reasons. This put the server in a tough spot, given that it revolved around her. However, due to the server having grown large enough to be capable of sustaining itself, the server instead transformed into more of a hangout spot.
				""")
		],
		1124777547687788626
	));

	private static readonly KeyValuePair<Choices, MessageProperties> Guide_Iceland = new(Choices.IcelandGuide, Embeds.Generate
	(
		"🏔 Angel's Iceland Travel Guide 🏔",
		[
		Embeds.CreateField("Health", """
			- There are doctors (healthcare centers) in all major towns
			- Pharmacies are called 'apótek' or 'lyfjaverslun', you'll find them in most towns
			- OtC pain stuff is only at pharmacies
			- Most places are safe, there's nowhere particularly 'bad', but pickpocketing can happen if you're not paying attention
			- Don't leave anything laying around in the capital, it will get lost/taken
			- Check the weather constantly
			- Never climb on any ice
			- Don't walk on or near glaciers
			- Avoid mud pots (look those up)
			- Never stop a car anywhere that isn't specifically a safe parking spot, it will get you in trouble
			- Cliffs and wind, lots of cliffs and wind
			- Pick this thing up while there - https://www.112.is/en/112-appid
			"""),

		Embeds.CreateField("Shopping", """
			- Iceland has a *ton* of souvenir shops and specialties, so check out everything you can
			- The most common specialties are silver jewllery, which you'll want to pick up with why you're there
			- Lava stones and fish leather are *everywhere*, make sure to check for the high quality stuff
			- Check out all the different shops you can, everywhere has something different
			- Most shops are open from 9am - 6pm from Monday to Friday, and from 10am to 4pm on Saturday
			- Almost everywhere is closed on Sunday, but shopping malls in the capital are always open
			VAT Refund System 
			- Your purchases were over 4000 ISK \ $29 (including VAT) total per store in the same 24 hours
			- If the refund is over 5000 ISK \ $36, you're going to have to show what you're refunding at customs
			- Everything under 5000 ISK can be refunded directly at the departure halls of most airports, but 'Keflavík International Airport' is the best one
			"""),

		Embeds.CreateField("Currency", """
			- Iceland uses the Króna or ISK
			- There are 1, 5, 10, 50 and 100 Króna coins
			- There are 500, 1000, 5000 and 10000 Króna notes
			- **In Iceland you use periods instead of commas, and vice versa**
			- You can exchange currency at all banks
			- Banks are generally open from 9am - 4pm Monday to Friday
			- Banks are closed on public holidays, check your calendars
			- 'Keflavík International Airport' offers exchange services 24/7 at Arion Bank
			"""),

		Embeds.CreateField("Banking", """
			- Electron and Maestro cards are the standard debit cards
			- VISA and MasterCard are the major banking / credit card providers, and are serviced everywhere
			- Double check exchange rates with your home bank, they can differ
			- **Iceland does not support swipe-and-sign cards, you need to use the 4-digit PIN system**
			- You can use Apple Pay pretty much everywhere, even at smaller shops
			Cash Advance
			- VISA, MasterCard and and Diners Club are available at any bank or ATM
			- JCB is only available at 'Kreditkort, Ármúli 28-30, 108 Reykjavík'
			ATMs
			- Check with your bank to see if they add extra charges
			- Re: 4-digit PIN system
			""")
		],
		1141261327973761076
	));
	#endregion

	/// <summary>
	/// Contains definitions used for the <see cref="SlashCommands.GetDefinition(Choices)"/> command.
	/// </summary>
	public static readonly Dictionary<Choices, MessageProperties> Entries = new([Codex_PPP, Guide_Iceland]);
}
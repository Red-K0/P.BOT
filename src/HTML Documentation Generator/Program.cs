using System.Text.RegularExpressions;

// Script Configuration
const string sourceXmlPath = "Source.xml";
const string htmlPageTitle = "P.BOT Documenation";
const string cssString     = "body{background:#191c1e;color:#c2c2c2;font:medium Helvetica;font-size:medium;font-weight:400;}red{color:#e0474b}lime{color:#e4ff10}yellow{color:#ffd702}blue{color:#1a94ff}pink{color:#d86ac2}orange{color:#efa553}paleblue{color:#8c90e5}lightblue{color:#86dbfd}";
const string baseHTML      = $"<!DOCTYPE html><html><head><title>{htmlPageTitle}</title><style>{cssString}</style></head><body>";

string xmlSource = File.ReadAllText(sourceXmlPath);

string xmlContents = "";
int memberCount = ((xmlSource.Length - xmlSource.Replace("<member name", null).Length) / "<member name".Length) + 1;

for (int i = 0; i < memberCount; i++)
{
	string workingString = xmlSource[(xmlSource.IndexOf("<member name") + 14)..];
	string nameString = workingString.Remove(workingString.IndexOf("\">\r\n"));

	if (nameString.Contains("RegularExpressions")) continue;

	string descString = workingString.Replace("\r\n", null);
	string paramString = "";

	descString = descString[(descString.IndexOf("<summary>") + 9)..].Trim();
	descString = descString.Remove(descString.IndexOf("</summary>")).Replace("<", "&lt;").Replace(">", "&gt;");

	if (workingString.Remove(workingString.IndexOf("</member>")).Contains("<param name"))
	{
		string paramWorkingString = workingString[workingString.IndexOf("<param name")..];
		paramWorkingString = paramWorkingString.Remove(paramWorkingString.IndexOf("</member>"));
		string paramOriginalString = paramWorkingString;

		int paramCount = ((paramWorkingString.Length - paramWorkingString.Replace("</param>", null).Length) / "</param>".Length);
		for (int j = 0; j < paramCount; j++)
		{
			paramString += "<li><orange>";
			paramString += paramWorkingString.Remove(paramWorkingString.IndexOf('>') - 1)[(paramWorkingString.IndexOf("<param name") + 13)..] + "</orange>:";
			paramString += paramOriginalString.Remove(paramOriginalString.IndexOf("</param>"))[(paramOriginalString.IndexOf('>') + 1)..].Trim().Replace("<", "&lt;").Replace(">", "&gt;") + "</li>";
			paramOriginalString = paramOriginalString[(paramOriginalString.IndexOf("</param>") + 8)..];
			paramWorkingString = paramOriginalString;
		}
	}

	xmlContents += $"<br>{(Parse(nameString).Contains('.')?Parse(nameString).Trim() :$"<blue>{Parse(nameString).Trim()}</blue>")}<br>{descString.Trim()}<br>" + (paramString != "" ? $"Parameters:{paramString.Trim()}" : "");
	xmlSource = xmlSource[(xmlSource.IndexOf(nameString) + nameString.Length)..];
}

File.WriteAllText("F:\\!PBOT\\docs\\index.html", Minify(baseHTML + Highlight(Clean(xmlContents)) + "</body></html>"));

static string Parse(string member)
{
	member = member
	// Global namespace removal
	.Replace("System.",                         null)
	.Replace("PBot.",                           null)
	.Replace("NetCord.",                        null)
	.Replace("Rest.",                           null)
	.Replace("Gateway.",                        null)

	//Highlight known classes
	.Replace("RestClient.",                     "<blue>RestClient</blue>.")
	.Replace("Client.",                         "<blue>Client</blue>.")
	.Replace("Embeds.",                         "<blue>Embeds</blue>.")
	.Replace("Extensions.",                     "<blue>Extensions</blue>.")
	.Replace("Pages.",                          "<blue>Pages</blue>.")
	.Replace("PHelper.",                        "<blue>PHelper</blue>.")

	// Caches namespace
	.Replace(".Messages",                       ".<blue>Messages</blue>")
	.Replace(".Members",                        ".<blue>Members</blue>")

	// Commands namespace
	.Replace(".SlashCommands",                  ".<blue>SlashCommands</blue>")
	.Replace(".TextCommands",                   ".<blue>TextCommands</blue>")

	// Commands.Helpers namespace
	.Replace(".Definition",                     ".<blue>Definition</blue>")
	.Replace(".Posts",                          ".<blue>Posts</blue>")
	.Replace(".ProbabilityStateMachine",        ".<blue>ProbabilityStateMachine</blue>")
	.Replace(".Wikipedia",                      ".<blue>Wikipedia</blue>")

	// Messages namespace
	.Replace(".Functions",                      ".<blue>Functions</blue>")
	.Replace(".Events",                         ".<blue>Events</blue>")
	.Replace(".Logging",                        ".<blue>Logging</blue>")

	// Parameterless method fix
	.Replace(".Alphanumeric",                   ".Alphanumeric()")
	.Replace(".EnableVirtualAndHideCursor",     ".EnableVirtualAndHideCursor()")
	.Replace(".InitXShift128",                  ".InitXShift128()")
	.Replace(".Load" ,                          ".Load()")
	.Replace(".MapClientHandlers" ,             ".MapClientHandlers()")
	.Replace(".Start",                          ".Start()")
	.Replace(".Start()InteractionHandler",      ".StartInteractionHandler()") // Account for line above.
	.Replace(".SystemsCheck",                   ".SystemsCheck()")

	// Dot seperator highlighting
	.Replace(".",                               "<paleblue>.</paleblue>")

	// Add parameter spacing
	.Replace(",",                               ", ");

	// Member highlighting
	for (int i = 0; i < member.Length; i++)
	{
		// Constant and member highlighting
		if (member[i] == '.')
		{
			if (member.IndexOf('.', i + 1) != -1) continue;

			if (member.IndexOf('(', i) == -1)
			{
				member = member.Insert(i + 12, "<lightblue>");
				member += "</lightblue>";
			}
			continue;
		}

		// Out syntax conversion
		if (member[i] == '@')
		{
			for (int ii = i - 1; ii > -1; ii--)
			{
				if (member[ii] == ' ')
				{
					member = member.Insert(ii, " <red>out</red>");
					break;
				}
			}
			member = string.Concat(member.Remove(i + 15), member.AsSpan(i + 16));
			continue;
		}

		if (member[i] == 'E')
		{
			if (member[i + 1] == 'v')
			{
				if (member.Substring(i, 9) == "EventArgs")
				{
					for (int ii = i - 1; ii > -1; ii--)
					{
						if (member[ii] is ' ' or '(')
						{
							member = member.Insert(ii + 1, "<blue>");
							member = member.Insert(i + 15, "</blue>");
							break;
						}
						if (ii == 0)
						{
							member = "<blue>" + member.Insert(i + 9, "</blue>");
						}
					}
					i += 16;
					continue;
				}
			}
		}
	}

	string types;

	if (member.Contains('(')) types = member[member.IndexOf('(')..];
	else if (!member.Contains('.')) types = member;
	else if (!member.Contains('<')) return "<orange>" + member + "</orange>";
	else return member;

	types = types
	// Integral type highlighting
	.Replace("SByte",                            "<red>sbyte</red>")
	.Replace("Byte",                             "<red>byte</red>")
	.Replace("UInt16",                           "<red>ushort</red>")
	.Replace("Int16",                            "<red>short</red>")
	.Replace("UInt32",                           "<red>uint</red>")
	.Replace("Int32",                            "<red>int</red>")
	.Replace("UInt64",                           "<red>ulong</red>")
	.Replace("Int64",                            "<red>long</red>")

	// Base type highlighting
	.Replace("String",                           "<red>string</red>")
	.Replace("Boolean",                          "<red>bool</red>")
	.Replace("null",                             "<red>null</red>")

	// Message with its fixes
	.Replace("InteractionMessageProperties",     "<blue>InteractionMessageProperties</blue>")
	.Replace("MessageProperties",                "<blue>MessageProperties</blue>")
	.Replace("RestMessage",                      "<blue>RestMessage</blue>")
	.Replace("LogMessage",                       "<blue>LogMessage</blue>")
	.Replace("Message",                          "<blue>Message</blue>")
	.Replace("<blue>Message</blue>s",            "Messages")

	// NetCord property types
	.Replace("EmbedAuthorProperties",            "<blue>EmbedAuthorProperties</blue>")
	.Replace("EmbedFooterProperties",            "<blue>EmbedFooterProperties</blue>")
	.Replace("EmbedFieldProperties",             "<blue>EmbedFieldProperties</blue>");

	if (member.Contains('(')) member = member.Remove(member.IndexOf('(')) + types;
	else member = types;

	return !member.Contains('<') && !string.Equals(member[0].ToString(), member[0].ToString().ToUpperInvariant(), StringComparison.Ordinal) ? "<orange>" + member + "</orange>" : member;
}
static string Clean(string str) => new Regex(Regex.Escape("<br>")).Replace(str, "", 1)
.Replace("&lt;c&gt;", null)
.Replace("&lt;/c&gt;", null)
.Replace("&lt;see cref=", null)
.Replace("&lt;see langword=", null)
.Replace("&lt;paramref name=", null)
.Replace("M:", null)
.Replace("F:", null)
.Replace("T:", null)
.Replace("P:", null);
static string Highlight(string str)
{
	byte highlight = 0;
	string target; bool procTarget = true;
	
	str = str .Replace("Nullable{", "<blue>Nullable</blue>{");

	// Syntax highlighting
	for (int i = 0; i < str.Length; i++)
	{
		if (str[i] == '(')
		{
			if (str[i - 1] != ' ')
			{
				for (int ii = 2; ii < int.MaxValue; ii++)
				{
					if (str[i - ii] == '.')
					{
						str = str.Insert(i - ii + 12, "<lime>");
						str = str.Insert(i + 6, "</lime>");
						break;
					}
				}
				i = str.IndexOf('(', i);
			}
		}

		// Brace match highlighing
		if (str[i] is '(' or '[' or '{')
		{
			switch (highlight % 3)
			{
				case 0:
					str = str.Insert(i, "<yellow>");
					str = str.Insert(i + 9, "</yellow>");
					i += 17;
					break;
				case 1:
					str = str.Insert(i, "<blue>");
					str = str.Insert(i + 7, "</blue>");
					i += 13;
					break;
				case 2:
					str = str.Insert(i, "<pink>");
					str = str.Insert(i + 7, "</pink>");
					i += 13;
					break;
			}
			highlight++;
			continue;
		}
		if (str[i] is ')' or ']' or '}')
		{
			highlight--;
			switch (highlight % 3)
			{
				case 0:
					str = str.Insert(i, "<yellow>");
					str = str.Insert(i + 9, "</yellow>");
					i += 17;
					break;
				case 1:
					str = str.Insert(i, "<blue>");
					str = str.Insert(i + 7, "</blue>");
					i += 13;
					break;
				case 2:
					str = str.Insert(i, "<pink>");
					str = str.Insert(i + 7, "</pink>");
					i += 13;
					break;
			}
			continue;
		}

		// Highlight references
		if (str[i] == '"')
		{
			str = str.Insert(i, "<orange>"); i += 9;
			if (procTarget)
			{
				target = str[i..str.IndexOf('"', i + 1)];
				Console.WriteLine(target + "\n");

				str = str[..i] + Parse(target) + str[str.IndexOf('"', i + 1)..];

				procTarget = false;
			}
			else
			{
				procTarget = true;
			}
			str = str.Insert(i, "</orange>"); i += 8;
			continue;
		}
	}

	// Second highlight pass
	str = str
	.Replace("\"", "")
	.Replace("/&gt;", null)
	.Replace("EmbedAuthorProperties<", "<blue>EmbedAuthorProperties</blue><")
	.Replace("EmbedFooterProperties<", "<blue>EmbedFooterProperties</blue><")
	.Replace("EmbedFieldProperties<", "<blue>EmbedFieldProperties</blue><");

	return str;
}
static string Minify(string str) => str.Trim()
	.Replace("paleblue>", "f>").Replace("paleblue{", "f{")
	.Replace("lightblue>", "m>").Replace("lightblue{", "m{")
	.Replace("blue>", "x>").Replace("blue{", "x{")
	.Replace("orange>", "o>").Replace("orange{", "o{")
	.Replace("yellow>", "y>").Replace("yellow{", "y{")
	.Replace("red>", "r>").Replace("red{", "r{")
	.Replace("pink>", "k>").Replace("pink{", "k{")
	.Replace("lime>", "l>").Replace("lime{", "l{")
	.Replace("\u007f", null);

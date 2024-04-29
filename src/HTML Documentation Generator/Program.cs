using System.Text.RegularExpressions;

// Script Configuration
const string ignoredHashFilePath = "ignored_hash.txt";
const string sourceXmlPath       = "Source.xml";

const string htmlPageTitle       = "P.BOT Documenation";
const string cssFilePath         = "main.css";

const string baseHTML            = "<!DOCTYPE html><html><head><title>" 
                                 + htmlPageTitle
								 + "</title><link href=\""
								 + cssFilePath +
								 "\"rel=\"stylesheet\"type=\"text/css\"/></head><body>";

string xmlContent = File.ReadAllText(sourceXmlPath);
uint hash1 = 5381;

for (int i = 0; i < xmlContent.Length && xmlContent[i] != '\0'; i += 2)
{
	hash1 = ((hash1 << 5) + hash1) ^ xmlContent[i];
	if (i == xmlContent.Length - 1 || xmlContent[i + 1] == '\0') break;
}

string fileHash = hash1.ToString();

if (!string.IsNullOrWhiteSpace(File.ReadAllText(ignoredHashFilePath)) && fileHash == File.ReadAllText(ignoredHashFilePath)) Environment.Exit(0);
else File.WriteAllText(ignoredHashFilePath, fileHash);

string xmlContents = baseHTML;
int memberCount    = ((xmlContent.Length - xmlContent.Replace("<member name", "").Length) / "<member name".Length) + 1;

for (int i = 0; i < memberCount; i++)
{
	string workingString = xmlContent[(xmlContent.IndexOf("<member name") + 14)..];
	string nameString = workingString.Remove(workingString.IndexOf("\">\r\n"));

	if (nameString.Contains("RegularExpressions")) continue;

	string descString = workingString.Replace("\n", "");
	string paramString = "";

	descString = descString[(descString.IndexOf("<summary>") + 9)..].Trim();
	descString = descString.Remove(descString.IndexOf("</summary>")).Replace("<", "&lt;").Replace(">", "&gt;");

	if (workingString.Remove(workingString.IndexOf("</member>")).Contains("<param name"))
	{
		string paramWorkingString = workingString[workingString.IndexOf("<param name")..];
		paramWorkingString = paramWorkingString.Remove(paramWorkingString.IndexOf("</member>"));
		string paramOriginalString = paramWorkingString;

		int paramCount = ((paramWorkingString.Length - paramWorkingString.Replace("</param>", "").Length) / "</param>".Length);
		for (int j = 0; j < paramCount; j++)
		{
			paramString += "<li><m1>";
			paramString += paramWorkingString.Remove(paramWorkingString.IndexOf('>') - 1)[(paramWorkingString.IndexOf("<param name") + 13)..] + ":</m1> ";
			paramString += paramOriginalString.Remove(paramOriginalString.IndexOf("</param>"))[(paramOriginalString.IndexOf('>') + 1)..] + "</li>";
			paramOriginalString = paramOriginalString[(paramOriginalString.IndexOf("</param>") + 8)..];
			paramWorkingString = paramOriginalString;
		}
	}
	xmlContents += $"<br><m1>Member:</m1> {nameString}<br><m1>Summary:</m1> {descString}<br>" + (paramString != "" ? $"<m1>Parameters:</m1>{paramString}" : "");
	xmlContent = xmlContent[(xmlContent.IndexOf(nameString) + nameString.Length)..];
}

xmlContents = new Regex(Regex.Escape("<br>"))
.Replace(xmlContents, "", 1)
.Replace("&lt;c&gt;", "").Replace("&lt;/c&gt;", "")
.Replace("M:", "").Replace("F:", "").Replace("T:", "")
.Replace(",", ", ").Replace(",  ", ", ")
.Replace("&lt;see cref=\"", "'").Replace("&lt;see langword=\"", "'").Replace("\"/&gt;", "'")
.Replace("&lt;paramref name=\"", "'");

xmlContents += "</body></html>";

File.WriteAllText("F:\\!PBOT\\docs\\index.html", xmlContents);

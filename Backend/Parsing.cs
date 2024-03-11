using System.Globalization;
using System.Text.RegularExpressions;

namespace P_BOT;
internal static partial class Parsing
{

	public static string EscapedUnicode(string Unparsed)
	{
		if (Unparsed == null) return "";
		char[] CharArray = Unparsed.ToCharArray();

		// Fix for escape characters in raw text, such as "\u2014" instead of '—' (U+2014 | Em Dash).
		for (int i = 0; i < CharArray.Length - 1; i++)
		{
			// If a raw text unicode identifier is present (\u****).
			if (CharArray[i] == '\\' && CharArray[i + 1] == 'u')
			{
				// Get the chars present after "\u", and merge them into one identifier.
				string Identifier = CharArray[i + 2].ToString()
								  + CharArray[i + 3].ToString()
								  + CharArray[i + 4].ToString()
								  + CharArray[i + 5].ToString();

				// Convert the Identifier into a char value, and replace the '\' w
				// ith it.
				CharArray[i] = Convert.ToChar(int.Parse(Identifier, System.Globalization.NumberStyles.HexNumber));

				// Replace the 'u' and identifier characters with null '\0' characters.
				CharArray[i + 1] = '\0';
				CharArray[i + 2] = '\0';
				CharArray[i + 3] = '\0';
				CharArray[i + 4] = '\0';
				CharArray[i + 5] = '\0';
			}
		}

		Unparsed = new(CharArray);
		return Unparsed.Replace("\0", "");
	}
}
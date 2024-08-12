﻿using System.Text.RegularExpressions;
using static PBot.HtmlTags;
using System.Text;
namespace PBot;

/// <summary>
/// Contains classes and methods for accessing and serializing XML documentation.
/// </summary>
internal static partial class Documentation
{
	[GeneratedRegex(@"System\.(Boolean|Byte|SByte|Char|Decimal|Double|Single|Int32|UInt32|IntPtr|UIntPtr|Int64|UInt64|Int16|UInt16|Object|String)")]
	private static partial Regex SystemTypeRegex();

	/// <summary>
	/// Generates an HTML file from a given file path.
	/// </summary>
	public static async Task Generate()
	{
		List<(string, string)>? Pairs = await Get();

		if (Pairs == null) return;

		string path = Environment.CurrentDirectory;

		StringBuilder Html = new("""
		<!DOCTYPE html>
		<html lang="en">
		<head>
			<meta charset="utf-8">
			<title>Client Documentation</title>
			<meta name="viewport" content="width=device-width">
			<link rel="stylesheet" href="style.css" type="text/css">
		</head>
		<body>
		""");

		char LastChar = 'B';

		foreach ((string, string) pair in Pairs)
		{
			if (pair.Item1[7] != LastChar) { LastChar = pair.Item1[7]; Html.Append("<br>"); }

			Html.Append(pair.Item1);
			Html.Append(pair.Item2);
		}

		await File.WriteAllTextAsync(path.Remove(path.IndexOf("src")) + "docs\\index.html", IndentTags(Html.ToString()));
	}

	/// <summary>
	/// Gets the documentation file as a <see cref="List{T}"/> of string tuples.
	/// </summary>
	private static async Task<List<(string, string)>?> Get()
	{
		const string Source = "Bot Client.xml";

		if (!File.Exists(Source)) return null;

		int StartIndex, EndIndex; string Data;
		string Documentation = await File.ReadAllTextAsync(Source);

		Documentation = Documentation.Remove(Documentation.IndexOf("<member name=\"F:Windows"));

		while (Documentation.Contains("<param name"))
		{
			StartIndex = Documentation.IndexOf("<param name="); EndIndex = Documentation.IndexOf("</param>") + 8;
			Documentation = string.Concat(Documentation.Remove(StartIndex), Documentation.AsSpan(EndIndex));
		}

		while (Documentation.Contains("<returns>"))
		{
			StartIndex = Documentation.IndexOf("<returns>"); EndIndex = Documentation.IndexOf("</returns>") + 10;
			Documentation = string.Concat(Documentation.Remove(StartIndex), Documentation.AsSpan(EndIndex));
		}

		Documentation = Documentation[Documentation.IndexOf("        <member name")..]
		.Replace("    ", null).Replace("\r\n", null).Replace("</member>", null).Replace("<remarks>", null).Replace("<summary>", null)

		.Replace("<member name=\"", MemberOpen).Replace("\"><", $"{MemberClose}{SummaryOpen}<")
		.Replace("</remarks>", SummaryClose).Replace("</summary>", SummaryClose).Replace("\">", MemberClose + SummaryOpen);

		List<(string, string)> Members = new(MemberTagRegex().Count(Documentation));

		for (int i = 0; i < Members.Capacity - 2; i++)
		{
			string Name;

			Data = Documentation[(StartIndex = Documentation.IndexOf(MemberOpen) + 1)..];
			Data = Data[..(EndIndex = Data.IndexOf(MemberClose))];
			Documentation = Documentation[(StartIndex + EndIndex + 1)..];

			Name = Format(Data);

			Data = Documentation[(StartIndex = Documentation.IndexOf(SummaryOpen) + 1)..];
			Data = Data[..(EndIndex = Data.IndexOf(SummaryClose))];
			Documentation = Documentation[(StartIndex + EndIndex + 1)..];

			Data = Data
			.Replace("<code>", Code).Replace("</code>", cCode)
			.Replace("<c>", Code).Replace("</c>", cCode)
			.Replace("<br/>", $"{LT}br/{GT}");

			while (Data.Contains('<'))
			{
				StartIndex = Data.IndexOf('<'); EndIndex = Data.IndexOf('>') + 1;

				string TagData = Data[StartIndex..EndIndex];

				if (TagData.Contains("paramref")) TagData = TagData[16..^3];
				else TagData = Format(Data[(StartIndex + 11)..(EndIndex - 3)], true);

				Data = string.Concat(Data[..StartIndex], TagData, Data[EndIndex..]);
			}

			Data = (Data + cDetails)
			.Replace(LT, "<").Replace(GT, ">")
			.Replace("&lt;\0see cref=\"", null).Replace("&lt;\0paramref name=\"", null)
			.Replace("\"/&gt;", null);

			Members.Add((Name, Data));
		}

		File.Delete(Source);
		return Members;
	}

	/// <summary>
	/// Formats a member's name.
	/// </summary>
	private static string Format(string? str, bool cref = false)
	{
		if (str == null) return "";

		CorrectNamespacesAndFormatting();

		ProcessStringCharacters();

		if (str.Contains('(')) ProcessParameters();

		CleanupString();

		return str;

		void CorrectNamespacesAndFormatting()
		{
			str = str.Replace(".#ctor", null);

			if (str.Contains("M:"))
			{
				str = str.Contains('(') ? str.Insert(str.IndexOf('('), cCyan) : $"{str}{cCyan}()"; // Correction for parameterless methods.
				str = str.Insert(str.Remove(str.IndexOf('(')).LastIndexOf('.') + 1, Cyan);
			}
			else
			{
				if (str.Contains('.'))
				{
					str = str.Insert(str.LastIndexOf('.') + 1, Cyan) + cCyan;
				}
				else if (str.Contains(' '))
				{
					str = str.Insert(str.IndexOf(' ') + 1, Cyan) + cCyan;
				}
			}

			str = str.Replace("T:", null).Replace("M:", null).Replace("P:", null).Replace("F:", null);

			str = str.Replace("PBot.", null).Replace("NetCord.", null).Replace(",", ", ");

			str = string.Concat(Code, str, cCode);
		}

		void ProcessStringCharacters()
		{
			bool nullable = false;
			char[] chars = str.ToCharArray();

			for (int i = 0; i < chars.Length; i++)
			{
				switch (chars[i])
				{
					case '{':
						if (str.Substring(i - 8, 8) == "Nullable")
						{
							nullable = true;
							chars[i] = '\0';
						}
						else
						{
							chars[i] = '<';
						}
						break;
					case '}':
						if (nullable)
						{
							nullable = false;
							chars[i] = '?';
						}
						else
						{
							chars[i] = '>';
						}
						break;
					case '@':
#pragma warning disable S127
						chars[i] = '\0';

						for (int j = i; j > -1; j--)
						{
							if (chars[j] == ' ')
							{
								chars = new string(chars).Insert(j, " out").ToCharArray();
								break;
							}
						}

						i += 4; // Advance index by 4 to account for added string.
#pragma warning restore S127
						break;
				}
			}

			str = new(chars);
		}

		void ProcessParameters()
		{
			int paramIndex = str.IndexOf('(') + 1;

			// Convert types to their simplified names.
			str = str[..paramIndex] + SystemTypeRegex().Replace(str[paramIndex..], match => match.ValueSpan switch
			{
				"System.Boolean" => $"{Blue}bool{cBlue}",
				"System.Byte" => $"{Blue}byte{cBlue}",
				"System.SByte" => $"{Blue}sbyte{cBlue}",
				"System.Char" => $"{Blue}char{cBlue}",
				"System.Decimal" => $"{Blue}decimal{cBlue}",
				"System.Double" => $"{Blue}double{cBlue}",
				"System.Single" => $"{Blue}single{cBlue}",
				"System.Int32" => $"{Blue}int{cBlue}",
				"System.UInt32" => $"{Blue}uint{cBlue}",
				"System.IntPtr" => $"{Blue}nint{cBlue}",
				"System.UIntPtr" => $"{Blue}nuint{cBlue}",
				"System.Int64" => $"{Blue}long{cBlue}",
				"System.UInt64" => $"{Blue}ulong{cBlue}",
				"System.Int16" => $"{Blue}short{cBlue}",
				"System.UInt16" => $"{Blue}ushort{cBlue}",
				"System.String" => $"{Blue}string{cBlue}",
				"System.Object" => $"{Blue}object{cBlue}",
				_ => match.Value
			})
				.Replace("System.", null)
				.Replace("Collections.Generic.IReadOnlyDictionary", "ReadOnlyDictionary");
		}

		void CleanupString()
		{
			str = (cref ? str : string.Concat(Details, Summary, str, cSummary))
			.Replace("<", "&lt;").Replace(">", "&gt;") // Replace '<' & '>' with entities
			.Replace("Nullable", null);                // Replace 'Nullable' notation

			if (!cref) str = str.Replace(LT, "<").Replace(GT, ">");
		}
	}
}

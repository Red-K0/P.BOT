#if false

// The helper class supporting the /translate command.

using NetCord.Services.ApplicationCommands;
namespace P_BOT.Command_Processing.Helpers;

/// <summary>
/// Contains the <see cref="Choices"/> <see cref="Enum"/> and <see cref="Process(string, Translation.Choices, Translation.Choices)"/> method, for use in the <see cref="SlashCommand.GetTranslation(string, Translation.Choices, Translation.Choices)"/> command.
/// </summary>
public static class Translation
{
	#region Full Choices
#if false
	public enum Languages
	{
		[SlashCommandChoice(Name = "Afar")] aa,
		[SlashCommandChoice(Name = "Abkhzian")] ab,
		[SlashCommandChoice(Name = "Afrikaans")] af,
		[SlashCommandChoice(Name = "Akan")] ak,
		[SlashCommandChoice(Name = "Albanian")] sq,
		[SlashCommandChoice(Name = "Amharic")] am,
		[SlashCommandChoice(Name = "Arabic")] ar,
		[SlashCommandChoice(Name = "Aragonese")] an,
		[SlashCommandChoice(Name = "Armenian")] hy,
		[SlashCommandChoice(Name = "Assamese")] @as,
		[SlashCommandChoice(Name = "Avaric")] av,
		[SlashCommandChoice(Name = "Avestan")] ae,
		[SlashCommandChoice(Name = "Aymara")] ay,
		[SlashCommandChoice(Name = "Azerbaijani")] az,
		[SlashCommandChoice(Name = "Bashkir")] ba,
		[SlashCommandChoice(Name = "Bambara")] bm,
		[SlashCommandChoice(Name = "Basque")] eu,
		[SlashCommandChoice(Name = "Belarusian")] be,
		[SlashCommandChoice(Name = "Bengali")] bn,
		[SlashCommandChoice(Name = "Bihari Languages")] bh,
		[SlashCommandChoice(Name = "Bislama")] bi,
		[SlashCommandChoice(Name = "Tibetan")] bo,
		[SlashCommandChoice(Name = "Bosnian")] bs,
		[SlashCommandChoice(Name = "Breton")] br,
		[SlashCommandChoice(Name = "Bulgarian")] bg,
		[SlashCommandChoice(Name = "Burmese")] my,
		[SlashCommandChoice(Name = "Valencian")] ca,
		[SlashCommandChoice(Name = "Czech")] cs,
		[SlashCommandChoice(Name = "Chamorro")] ch,
		[SlashCommandChoice(Name = "Chechen")] ce,
		[SlashCommandChoice(Name = "Chinese")] zh,
		[SlashCommandChoice(Name = "Church Slavonic")] cu,
		[SlashCommandChoice(Name = "Chuvash")] cv,
		[SlashCommandChoice(Name = "Cornish")] kw,
		[SlashCommandChoice(Name = "Corsican")] co,
		[SlashCommandChoice(Name = "Cree")] cr,
		[SlashCommandChoice(Name = "Welsh")] cy,
		[SlashCommandChoice(Name = "Danish")] da,
		[SlashCommandChoice(Name = "German")] de,
		[SlashCommandChoice(Name = "Dhivehi")] dv,
		[SlashCommandChoice(Name = "Dutch")] nl,
		[SlashCommandChoice(Name = "Dzongkha")] dz,
		[SlashCommandChoice(Name = "Greek")] el,
		[SlashCommandChoice(Name = "English")] en,
		[SlashCommandChoice(Name = "Esperanto")] eo,
		[SlashCommandChoice(Name = "Estonian")] et,
		[SlashCommandChoice(Name = "Ewe")] ee,
		[SlashCommandChoice(Name = "Faroese")] fo,
		[SlashCommandChoice(Name = "Persian")] fa,
		[SlashCommandChoice(Name = "Fijian")] fj,
		[SlashCommandChoice(Name = "Finnish")] fi,
		[SlashCommandChoice(Name = "French")] fr,
		[SlashCommandChoice(Name = "Western Frisian")] fy,
		[SlashCommandChoice(Name = "Fulah")] ff,
		[SlashCommandChoice(Name = "Georgian")] ka,
		[SlashCommandChoice(Name = "Gaelic")] gd,
		[SlashCommandChoice(Name = "Irish")] ga,
		[SlashCommandChoice(Name = "Galician")] gl,
		[SlashCommandChoice(Name = "Manx")] gv,
		[SlashCommandChoice(Name = "Guarani")] gn,
		[SlashCommandChoice(Name = "Gujarati")] gu,
		[SlashCommandChoice(Name = "Haitian")] ht,
		[SlashCommandChoice(Name = "Hausa")] ha,
		[SlashCommandChoice(Name = "Hebrew")] he,
		[SlashCommandChoice(Name = "Herero")] hz,
		[SlashCommandChoice(Name = "Hindi")] hi,
		[SlashCommandChoice(Name = "Hiri Motu")] ho,
		[SlashCommandChoice(Name = "Croatian")] hr,
		[SlashCommandChoice(Name = "Hungarian")] hu,
		[SlashCommandChoice(Name = "Igbo")] ig,
		[SlashCommandChoice(Name = "Icelandic")] @is,
		[SlashCommandChoice(Name = "Ido")] io,
		[SlashCommandChoice(Name = "Sichuan Yi")] ii,
		[SlashCommandChoice(Name = "Inuktitut")] iu,
		[SlashCommandChoice(Name = "Interlingue")] ie,
		[SlashCommandChoice(Name = "IALA")] ia,
		[SlashCommandChoice(Name = "Indonesian")] id,
		[SlashCommandChoice(Name = "Inupiaq")] ik,
		[SlashCommandChoice(Name = "Italian")] it,
		[SlashCommandChoice(Name = "Javanese")] jv,
		[SlashCommandChoice(Name = "Japanese")] ja,
		[SlashCommandChoice(Name = "Kalaallisut")] kl,
		[SlashCommandChoice(Name = "Kannada")] kn,
		[SlashCommandChoice(Name = "Kashmiri")] ks,
		[SlashCommandChoice(Name = "Kanuri")] kr,
		[SlashCommandChoice(Name = "Kazakh")] kk,
		[SlashCommandChoice(Name = "Central Khmer")] km,
		[SlashCommandChoice(Name = "Kikuyu")] ki,
		[SlashCommandChoice(Name = "Kinyarwanda")] rw,
		[SlashCommandChoice(Name = "Kirghiz")] ky,
		[SlashCommandChoice(Name = "Komi")] kv,
		[SlashCommandChoice(Name = "Kongo")] kg,
		[SlashCommandChoice(Name = "Korean")] ko,
		[SlashCommandChoice(Name = "Kuanyama")] kj,
		[SlashCommandChoice(Name = "Kurdish")] ku,
		[SlashCommandChoice(Name = "Lao")] lo,
		[SlashCommandChoice(Name = "Latin")] la,
		[SlashCommandChoice(Name = "Latvian")] lv,
		[SlashCommandChoice(Name = "Limburgan")] li,
		[SlashCommandChoice(Name = "Lingala")] ln,
		[SlashCommandChoice(Name = "Lithuanian")] lt,
		[SlashCommandChoice(Name = "Luxembourgish")] lb,
		[SlashCommandChoice(Name = "Luba-Katanga")] lu,
		[SlashCommandChoice(Name = "Ganda")] lg,
		[SlashCommandChoice(Name = "Macedonian")] mk,
		[SlashCommandChoice(Name = "Marshallese")] mh,
		[SlashCommandChoice(Name = "Malayalam")] ml,
		[SlashCommandChoice(Name = "Maori")] mi,
		[SlashCommandChoice(Name = "Marathi")] mr,
		[SlashCommandChoice(Name = "Malay")] ms,
		[SlashCommandChoice(Name = "Malagasy")] mg,
		[SlashCommandChoice(Name = "Maltese")] mt,
		[SlashCommandChoice(Name = "Mongolian")] mn,
		[SlashCommandChoice(Name = "Nauru")] na,
		[SlashCommandChoice(Name = "Navajo")] nv,
		[SlashCommandChoice(Name = "South Ndebele")] nr,
		[SlashCommandChoice(Name = "North Ndebele")] nd,
		[SlashCommandChoice(Name = "Ndonga")] ng,
		[SlashCommandChoice(Name = "Nepali")] ne,
		[SlashCommandChoice(Name = "Nynorsk")] nn,
		[SlashCommandChoice(Name = "Norwegian Bokmål")] nb,
		[SlashCommandChoice(Name = "Norwegian")] no,
		[SlashCommandChoice(Name = "Chichewa")] ny,
		[SlashCommandChoice(Name = "Occitan (Post 1500)")] oc,
		[SlashCommandChoice(Name = "Ojibawa")] oj,
		[SlashCommandChoice(Name = "Oriya")] or,
		[SlashCommandChoice(Name = "Oromo")] om,
		[SlashCommandChoice(Name = "Ossetian")] os,
		[SlashCommandChoice(Name = "Punjabi")] pa,
		[SlashCommandChoice(Name = "Pali")] pi,
		[SlashCommandChoice(Name = "Polish")] pl,
		[SlashCommandChoice(Name = "Portuguese")] pt,
		[SlashCommandChoice(Name = "Pashto")] ps,
		[SlashCommandChoice(Name = "Quechua")] qu,
		[SlashCommandChoice(Name = "Romansh")] rm,
		[SlashCommandChoice(Name = "Romanian")] ro,
		[SlashCommandChoice(Name = "Rundi")] rn,
		[SlashCommandChoice(Name = "Russian")] ru,
		[SlashCommandChoice(Name = "Sango")] sg,
		[SlashCommandChoice(Name = "Sanskrit")] sa,
		[SlashCommandChoice(Name = "Sinhala")] si,
		[SlashCommandChoice(Name = "Slovak")] sk,
		[SlashCommandChoice(Name = "Slovenian")] sl,
		[SlashCommandChoice(Name = "Northern Sami")] se,
		[SlashCommandChoice(Name = "Samoan")] sm,
		[SlashCommandChoice(Name = "Shona")] sn,
		[SlashCommandChoice(Name = "Sindhi")] sd,
		[SlashCommandChoice(Name = "Somali")] so,
		[SlashCommandChoice(Name = "Southern Sotho")] st,
		[SlashCommandChoice(Name = "Spanish")] es,
		[SlashCommandChoice(Name = "Sardinian")] sc,
		[SlashCommandChoice(Name = "Serbian")] sr,
		[SlashCommandChoice(Name = "Swati")] ss,
		[SlashCommandChoice(Name = "Sudanese")] su,
		[SlashCommandChoice(Name = "Swahili")] sw,
		[SlashCommandChoice(Name = "Swedish")] sv,
		[SlashCommandChoice(Name = "Tahitian")] ty,
		[SlashCommandChoice(Name = "Tamil")] ta,
		[SlashCommandChoice(Name = "Tatar")] tt,
		[SlashCommandChoice(Name = "Telugu")] te,
		[SlashCommandChoice(Name = "Tajik")] tg,
		[SlashCommandChoice(Name = "Tagalog")] tl,
		[SlashCommandChoice(Name = "Thai")] th,
		[SlashCommandChoice(Name = "Tigrinya")] ti,
		[SlashCommandChoice(Name = "Tonga")] to,
		[SlashCommandChoice(Name = "Tswana")] tn,
		[SlashCommandChoice(Name = "Tsonga")] ts,
		[SlashCommandChoice(Name = "Turkmen")] tk,
		[SlashCommandChoice(Name = "Turkish")] tr,
		[SlashCommandChoice(Name = "Twi")] tw,
		[SlashCommandChoice(Name = "Uyghur")] ug,
		[SlashCommandChoice(Name = "Ukranian")] uk,
		[SlashCommandChoice(Name = "Urdu")] ur,
		[SlashCommandChoice(Name = "Uzbek")] uz,
		[SlashCommandChoice(Name = "Venda")] ve,
		[SlashCommandChoice(Name = "Vietnamese")] vi,
		[SlashCommandChoice(Name = "Volapük")] vo,
		[SlashCommandChoice(Name = "Walloon")] wa,
		[SlashCommandChoice(Name = "Wolof")] wo,
		[SlashCommandChoice(Name = "Xhosa")] xh,
		[SlashCommandChoice(Name = "Yiddish")] yi,
		[SlashCommandChoice(Name = "Yoruba")] yo,
		[SlashCommandChoice(Name = "Zhuang")] za,
		[SlashCommandChoice(Name = "Zulu")] zu
	}	
#endif
	#endregion

	/// <summary> Sends a GET request to the translation API with the specified parameters. </summary>
	/// <param name="input"> The <see cref="string"/> of text to pass to the translator. Limited to 623 characters. </param>
	/// <param name="source_lang"> The original language of the <paramref name="input"/> string. </param>
	/// <param name="target_lang"> The language to translate the <paramref name="input"/> string to. </param>
	public static async Task<string> Process(string input, Choices source_lang, Choices target_lang)
	{
		const string API_URL = "https://655.mtis.workers.dev/translate";

		if (input.Length > 623)
		{
			return "Sorry, inputs longer than 623 characters aren't supported by the translation API, please try a shorter input." +
				  $"\n>>> \"{input}\" - Length : {input.Length} Characters";
		}

		string output = await client_h.GetStringAsync($"{API_URL}?text={input}&source_lang={source_lang}&target_lang={target_lang}");

		return
		$"""
		Original Text: {input}

		Translated Text: {output.Remove(output.Length - 3)[(output.IndexOf("\"translated_text\":") + 19)..]}
		""";
	}

	/// <summary>
	/// A list of languages supported by the translation server.
	/// </summary>
	public enum Choices
	{
		/// <summary> The language Arabic. </summary>
		[SlashCommandChoice(Name = "Arabic")] ar,
		/// <summary> The language Chinese. </summary>
		[SlashCommandChoice(Name = "Chinese")] zh,
		/// <summary> The language Dutch. </summary>
		[SlashCommandChoice(Name = "Dutch")] nl,
		/// <summary> The language English. </summary>
		[SlashCommandChoice(Name = "English")] en,
		/// <summary> The language Finnish. </summary>
		[SlashCommandChoice(Name = "Finnish")] fi,
		/// <summary> The language French. </summary>
		[SlashCommandChoice(Name = "French")] fr,
		/// <summary> The language German. </summary>
		[SlashCommandChoice(Name = "German")] de,
		/// <summary> The language Greek. </summary>
		[SlashCommandChoice(Name = "Greek")] el,
		/// <summary> The language Hindi. </summary>
		[SlashCommandChoice(Name = "Hindi")] hi,
		/// <summary> The language Italian. </summary>
		[SlashCommandChoice(Name = "Italian")] it,
		/// <summary> The language Japanese. </summary>
		[SlashCommandChoice(Name = "Japanese")] ja,
		/// <summary> The language Korean. </summary>
		[SlashCommandChoice(Name = "Korean")] ko,
		/// <summary> The language Polish. </summary>
		[SlashCommandChoice(Name = "Polish")] pl,
		/// <summary> The language Portuguese. </summary>
		[SlashCommandChoice(Name = "Portuguese")] pt,
		/// <summary> The language Russian. </summary>
		[SlashCommandChoice(Name = "Russian")] ru,
		/// <summary> The language Spanish. </summary>
		[SlashCommandChoice(Name = "Spanish")] es,
		/// <summary> The language Swedish. </summary>
		[SlashCommandChoice(Name = "Swedish")] sv,
		/// <summary> The language Turkish. </summary>
		[SlashCommandChoice(Name = "Turkish")] tr,
		/// <summary> The language Vietnamese. </summary>
		[SlashCommandChoice(Name = "Vietnamese")] vi,
	}
}
#endif
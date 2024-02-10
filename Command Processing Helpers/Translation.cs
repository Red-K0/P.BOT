namespace P_BOT.Command_Processing_Helpers;

/// <summary> Contains the <see cref="Options"/> <see cref="Enum"/> and <see cref="GetTranslation(string, Translation.Options, Translation.Options)"/> method, for use in the <see cref="SlashCommand.Translate(string, Translation.Options, Translation.Options)"/> command.</summary>
internal static class Translation
{
	/// <summary> A list of languages supported by the translation server. </summary>
	public enum Options
	{
		/// <summary> The language Arabic. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Arabic")] ar,
		/// <summary> The language Chinese. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Chinese")] zh,
		/// <summary> The language Dutch. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Dutch")] nl,
		/// <summary> The language English. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "English")] en,
		/// <summary> The language Finnish. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Finnish")] fi,
		/// <summary> The language French. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "French")] fr,
		/// <summary> The language German. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "German")] de,
		/// <summary> The language Greek. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Greek")] el,
		/// <summary> The language Hindi. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Hindi")] hi,
		/// <summary> The language Italian. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Italian")] it,
		/// <summary> The language Japanese. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Japanese")] ja,
		/// <summary> The language Korean. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Korean")] ko,
		/// <summary> The language Polish. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Polish")] pl,
		/// <summary> The language Portuguese. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Portuguese")] pt,
		/// <summary> The language Russian. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Russian")] ru,
		/// <summary> The language Spanish. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Spanish")] es,
		/// <summary> The language Swedish. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Swedish")] sv,
		/// <summary> The language Turkish. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Turkish")] tr,
		/// <summary> The language Vietnamese. </summary>
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Vietnamese")] vi,
	}

	/// <summary> Sends a GET request to the translation API defined at <see cref="URL_TRANSLATE"/> with the specified parameters. </summary>
	/// <param name="input"> The <see cref="string"/> of text to pass to the translator. Limited to 623 characters. </param>
	/// <param name="source_lang"> The original language of the <paramref name="input"/> string. </param>
	/// <param name="target_lang"> The language to translate the <paramref name="input"/> string to. </param>
	public static string GetTranslation(string input, Options source_lang, Options target_lang)
	{
		if (input.Length > 623)
		{
			return "Sorry, inputs longer than 623 characters aren't supported by the translation API, please try a shorter input." +
				  $"\n>>> \"{input}\" - Length : {input.Length} Characters";
		}

		HttpRequestMessage request = new()
		{
			Method = HttpMethod.Get,
			Version = new Version(1, 1),
			RequestUri = new Uri($"{URL_TRANSLATE}?text={input}&source_lang={source_lang}&target_lang={target_lang}")
		};
		HttpResponseMessage response = client_h.Send(request);
		_ = response.EnsureSuccessStatusCode();

		string output = response.Content.ReadAsStringAsync().Result;
		output = output.Remove(output.Length - 3)[(output.IndexOf("\"translated_text\":") + 19)..];
		request.Dispose();

		return
		$"""
		Original Text: {input}

		Translated Text: {output}
		""";
	}
}

#if false //Unsupported by translation API
	public enum Languages
	{
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Afar")] aa,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Abkhzian")] ab,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Afrikaans")] af,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Akan")] ak,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Albanian")] sq,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Amharic")] am,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Arabic")] ar,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Aragonese")] an,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Armenian")] hy,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Assamese")] @as,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Avaric")] av,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Avestan")] ae,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Aymara")] ay,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Azerbaijani")] az,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bashkir")] ba,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bambara")] bm,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Basque")] eu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Belarusian")] be,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bengali")] bn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bihari Languages")] bh,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bislama")] bi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tibetan")] bo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bosnian")] bs,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Breton")] br,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Bulgarian")] bg,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Burmese")] my,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Valencian")] ca,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Czech")] cs,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Chamorro")] ch,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Chechen")] ce,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Chinese")] zh,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Church Slavonic")] cu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Chuvash")] cv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Cornish")] kw,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Corsican")] co,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Cree")] cr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Welsh")] cy,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Danish")] da,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "German")] de,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Dhivehi")] dv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Dutch")] nl,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Dzongkha")] dz,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Greek")] el,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "English")] en,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Esperanto")] eo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Estonian")] et,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ewe")] ee,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Faroese")] fo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Persian")] fa,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Fijian")] fj,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Finnish")] fi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "French")] fr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Western Frisian")] fy,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Fulah")] ff,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Georgian")] ka,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Gaelic")] gd,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Irish")] ga,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Galician")] gl,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Manx")] gv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Guarani")] gn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Gujarati")] gu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Haitian")] ht,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Hausa")] ha,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Hebrew")] he,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Herero")] hz,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Hindi")] hi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Hiri Motu")] ho,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Croatian")] hr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Hungarian")] hu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Igbo")] ig,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Icelandic")] @is,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ido")] io,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sichuan Yi")] ii,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Inuktitut")] iu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Interlingue")] ie,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "IALA")] ia,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Indonesian")] id,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Inupiaq")] ik,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Italian")] it,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Javanese")] jv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Japanese")] ja,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kalaallisut")] kl,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kannada")] kn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kashmiri")] ks,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kanuri")] kr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kazakh")] kk,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Central Khmer")] km,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kikuyu")] ki,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kinyarwanda")] rw,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kirghiz")] ky,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Komi")] kv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kongo")] kg,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Korean")] ko,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kuanyama")] kj,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Kurdish")] ku,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Lao")] lo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Latin")] la,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Latvian")] lv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Limburgan")] li,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Lingala")] ln,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Lithuanian")] lt,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Luxembourgish")] lb,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Luba-Katanga")] lu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ganda")] lg,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Macedonian")] mk,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Marshallese")] mh,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Malayalam")] ml,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Maori")] mi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Marathi")] mr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Malay")] ms,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Malagasy")] mg,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Maltese")] mt,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Mongolian")] mn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Nauru")] na,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Navajo")] nv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "South Ndebele")] nr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "North Ndebele")] nd,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ndonga")] ng,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Nepali")] ne,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Nynorsk")] nn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Norwegian Bokmål")] nb,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Norwegian")] no,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Chichewa")] ny,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Occitan (Post 1500)")] oc,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ojibawa")] oj,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Oriya")] or,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Oromo")] om,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ossetian")] os,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Punjabi")] pa,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Pali")] pi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Polish")] pl,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Portuguese")] pt,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Pashto")] ps,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Quechua")] qu,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Romansh")] rm,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Romanian")] ro,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Rundi")] rn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Russian")] ru,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sango")] sg,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sanskrit")] sa,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sinhala")] si,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Slovak")] sk,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Slovenian")] sl,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Northern Sami")] se,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Samoan")] sm,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Shona")] sn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sindhi")] sd,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Somali")] so,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Southern Sotho")] st,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Spanish")] es,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sardinian")] sc,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Serbian")] sr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Swati")] ss,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Sudanese")] su,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Swahili")] sw,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Swedish")] sv,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tahitian")] ty,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tamil")] ta,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tatar")] tt,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Telugu")] te,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tajik")] tg,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tagalog")] tl,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Thai")] th,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tigrinya")] ti,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tonga")] to,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tswana")] tn,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Tsonga")] ts,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Turkmen")] tk,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Turkish")] tr,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Twi")] tw,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Uyghur")] ug,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Ukranian")] uk,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Urdu")] ur,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Uzbek")] uz,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Venda")] ve,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Vietnamese")] vi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Volapük")] vo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Walloon")] wa,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Wolof")] wo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Xhosa")] xh,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Yiddish")] yi,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Yoruba")] yo,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Zhuang")] za,
		[NetCord.Services.ApplicationCommands.SlashCommandChoice(Name = "Zulu")] zu
	}
#endif
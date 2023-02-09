using System;
using System.Collections.Generic;
using App;
using UnityEngine;

public static class LanguageManager
{
	public static Font[] FontTable;

	public static readonly IDictionary<string, Font> FONTDATA_TABLE;

	public static IDictionary<string, string[]> STRINGS_TABLE;

	public static IDictionary<string, string[]> FONT_TABLE;

	static LanguageManager()
	{
		FONTDATA_TABLE = new Dictionary<string, Font>();
		STRINGS_TABLE = new Dictionary<string, string[]>();
		FONT_TABLE = new Dictionary<string, string[]>();
		Data.Init();
		SetLanguage();
		FONTDATA_TABLE.Add("Default", Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"));
		Font[] array = Resources.LoadAll<Font>("Font");
		foreach (Font font in array)
		{
			FONTDATA_TABLE.Add(font.name, font);
		}
	}

	public static void SetLanguage()
	{
		string lANGUAGE = BuildInfo.LANG_DEFAULT;
		switch (Application.systemLanguage)
		{
		case SystemLanguage.Japanese:
			lANGUAGE = "日本語";
			break;
		case SystemLanguage.Chinese:
		case SystemLanguage.ChineseSimplified:
		case SystemLanguage.ChineseTraditional:
			lANGUAGE = "中国語";
			break;
		case SystemLanguage.English:
			lANGUAGE = "英語";
			break;
		case SystemLanguage.Spanish:
			lANGUAGE = "スペイン語";
			break;
		}
		Settings.LANGUAGE = lANGUAGE;
		if (GetIndex() < 0)
		{
			Settings.LANGUAGE = BuildInfo.LANG_DEFAULT;
		}
	}

	public static string GetCode(string type, string os)
	{
		switch (type)
		{
		case "日本語":
			return "ja";
		case "中国語":
			if (!(os == "iOS"))
			{
				return "zh";
			}
			return "zh-Hans";
		case "英語":
			return "en";
		case "スペイン語":
			return "es";
		default:
			return "";
		}
	}

	public static int GetIndex()
	{
		return Array.IndexOf(BuildInfo.LANG_TYPE, Settings.LANGUAGE);
	}

	public static bool IsDefault()
	{
		return Settings.LANGUAGE == BuildInfo.LANG_DEFAULT;
	}

	public static bool IsJapanese()
	{
		return Settings.LANGUAGE == "日本語";
	}

	public static bool IsChinese()
	{
		return Settings.LANGUAGE == "中国語";
	}

	public static bool AllowVertical()
	{
		if (!IsJapanese())
		{
			return IsChinese();
		}
		return true;
	}

	public static bool Contains(string id)
	{
		return STRINGS_TABLE.ContainsKey(id);
	}

	public static string Get(string id)
	{
		return Get(id, true);
	}

	public static string Get(string id, bool required)
	{
		if (id != null && STRINGS_TABLE.ContainsKey(id))
		{
			int index = GetIndex();
			return STRINGS_TABLE[id][index].Replace("<br>", "\n");
		}
		if (required)
		{
			Debug.LogError("LanguageManager string id=" + id);
		}
		return "";
	}

	public static Font GetFont(string id)
	{
		if (!FONT_TABLE.ContainsKey(id))
		{
			return null;
		}
		int index = GetIndex();
		string key = FONT_TABLE[id][index];
		if (FONTDATA_TABLE.ContainsKey(key))
		{
			return FONTDATA_TABLE[key];
		}
		return null;
	}
}

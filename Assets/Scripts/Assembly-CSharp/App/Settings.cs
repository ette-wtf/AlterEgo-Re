using System.Collections.Generic;
using UnityEngine;

namespace App
{
	[AppUtil.Title("設定")]
	public static class Settings
	{
		public static readonly string[] LANG_LIST;

		public const float BUTTERFLY_WAIT_BOOK = 15f;

		public const float BUTTERFLY_WAIT_ES = 15f;

		public const float WALK_DISTANCE = -1.5f;

		public const float WALK_SPEED = 0.42f;

		public const float BONUS_SECONDS = 600f;

		public const int BOOK_HAYAKAWA_MAX = 30;

		public const int BOOK_HAYAKAWA_BONUS = 2;

		public static Color TRANSITION_COLOR;

		public static IDictionary<string, string> AUDIO_LIST;

		[AppUtil.Title("言語")]
		[AppUtil.ArrayValue("LANG_LIST")]
		public static string LANGUAGE { get; set; }

		[AppUtil.Title("ゲームスピード")]
		public static float GAME_SPEED { get; set; }

		[AppUtil.Title("BGM ON/OFF")]
		public static bool BGM
		{
			get
			{
				return PlayerPrefs.GetInt("BGM", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("BGM", value ? 1 : 0);
				AudioManager.ChangeSettings();
			}
		}

		[AppUtil.Title("SE ON/OFF")]
		public static bool SE
		{
			get
			{
				return PlayerPrefs.GetInt("SE", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("SE", value ? 1 : 0);
				AudioManager.ChangeSettings();
			}
		}

		[AppUtil.Title("引き継ぎID")]
		public static string SaveDataID
		{
			get
			{
				return PlayerPrefs.GetString("SaveDataID");
			}
			set
			{
				PlayerPrefs.SetString("SaveDataID", value);
			}
		}

		public static void Init()
		{
			GAME_SPEED = 1f;
			TRANSITION_COLOR = Color.black;
		}

		static Settings()
		{
			LANG_LIST = BuildInfo.LANG_TYPE;
			AUDIO_LIST = new Dictionary<string, string>
			{
				{ "Default", "Alterego" },
				{ "エスの部屋", "Silence_OffV" },
				{ "Ending", "" }
			};
			Init();
		}
	}
}

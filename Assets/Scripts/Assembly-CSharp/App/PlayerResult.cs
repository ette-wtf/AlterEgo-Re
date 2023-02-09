using UnityEngine;

namespace App
{
	[AppUtil.Title("実績")]
	public static class PlayerResult
	{
		[AppUtil.Title("ページ数合計")]
		public static int BookPageSum
		{
			get
			{
				return PlayerPrefs.GetInt("ResultBookLevel", 0);
			}
			set
			{
				PlayerPrefs.SetInt("ResultBookLevel", value);
			}
		}

		[AppUtil.Title("1タップあたりの獲得EGO")]
		public static EgoPoint TapMax
		{
			get
			{
				return new EgoPoint(PlayerPrefs.GetString("ResultTapMax", "0"));
			}
			set
			{
				if (value > TapMax)
				{
					PlayerPrefs.SetString("ResultTapMax", value.ToString());
				}
			}
		}

		[AppUtil.Title("吹き出しタップ回数")]
		public static int TapCount
		{
			get
			{
				return PlayerPrefs.GetInt("ResultTapCount", 0);
			}
			set
			{
				PlayerPrefs.SetInt("ResultTapCount", value);
			}
		}

		[AppUtil.Title("動画広告視聴回数")]
		public static int AdMovieCount
		{
			get
			{
				return PlayerPrefs.GetInt("ResultAdMovieCount", 0);
			}
			set
			{
				PlayerPrefs.SetInt("ResultAdMovieCount", value);
			}
		}

		[AppUtil.Title("目標達成回数")]
		public static int PrizeCount
		{
			get
			{
				int num = 0;
				for (int i = PrizeLevel.Min; i <= PrizeLevel.Max; i++)
				{
					num += PrizeLevel.Get(PrizeLevel.GetTitle(i));
				}
				return num;
			}
		}

		[AppUtil.Title("エンディング到達")]
		public static string Ending
		{
			get
			{
				return PlayerPrefs.GetString("ResultEnding", "");
			}
			set
			{
				PlayerPrefs.SetString("ResultEnding", value);
			}
		}

		[AppUtil.Hide]
		public static int EndingCount
		{
			get
			{
				return Ending.Length / 2;
			}
		}

		[AppUtil.Title("ログイン日数")]
		public static int LoginCount
		{
			get
			{
				return PlayerPrefs.GetInt("ResultLoginCount", 0);
			}
			set
			{
				PlayerPrefs.SetInt("ResultLoginCount", value);
			}
		}

		[AppUtil.Title("エス反応")]
		public static int EsTapCount
		{
			get
			{
				return PlayerPrefs.GetInt("EsTapCount", 0);
			}
			set
			{
				PlayerPrefs.SetInt("EsTapCount", value);
			}
		}

		[AppUtil.Title("雑談回数")]
		public static int EsTalkCount
		{
			get
			{
				return PlayerPrefs.GetInt("EsTalkCount", 0);
			}
			set
			{
				PlayerPrefs.SetInt("EsTalkCount", value);
			}
		}

		[AppUtil.Title("カタルシス文字数")]
		public static int CatharsisLength
		{
			get
			{
				return PlayerPrefs.GetInt("CatharsisLength", 0);
			}
			set
			{
				PlayerPrefs.SetInt("CatharsisLength", value);
			}
		}
	}
}

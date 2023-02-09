using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace App
{
	[AppUtil.Title("ステータス")]
	public static class PlayerStatus
	{
		public static readonly int TUTORIAL_MAX = 7;

		[AppUtil.Title("シナリオ")]
		[AppUtil.ArrayValue("SCENARIO_NO_LIST")]
		public static string ScenarioNo
		{
			get
			{
				return PlayerPrefs.GetString("ScenarioNo", "1章-1");
			}
			set
			{
				if (!(ScenarioNo == value))
				{
					string scenarioNo = ScenarioNo;
					PlayerPrefs.SetString("ScenarioNo", value);
					Route = Data.GetRoute(value);
					Data.UpdateClearScenarioList();
					Data.UpdateCommentList();
					Data.UpdateEgoBookPower();
					AnalyticsEvent.Custom("scenario_lv", new Dictionary<string, object> { { "scenario_no", value } });
					if (value.Contains("-1"))
					{
						string text = scenarioNo.Split('-')[0];
						AnalyticsManager.SendEvent(new string[3]
						{
							"シナリオ読了",
							text,
							"退行" + PlayerResult.EndingCount
						});
					}
				}
			}
		}

		[AppUtil.Title("EGO")]
		public static EgoPoint EgoPoint
		{
			get
			{
				return JsonUtility.FromJson<EgoPoint>(PlayerPrefs.GetString("EgoPoint", JsonUtility.ToJson(new EgoPoint())));
			}
			set
			{
				PlayerPrefs.SetString("EgoPoint", JsonUtility.ToJson(value));
			}
		}

		[AppUtil.Title("EGO(毎秒)")]
		public static EgoPoint EgoPerSecond
		{
			get
			{
				return Data.EgoPerSecond;
			}
		}

		[AppUtil.Title("EGO(タップx倍)")]
		public static EgoPoint TapPower
		{
			get
			{
				return Data.GetEgoTapPower();
			}
		}

		[AppUtil.Hide]
		public static string[] SCENARIO_NO_LIST
		{
			get
			{
				return Data.SCENARIO_DATA_KEY.ToArray();
			}
		}

		[AppUtil.Title("チュートリアル")]
		public static int TutorialLv
		{
			get
			{
				int num = PlayerPrefs.GetInt("TutorialLv", TUTORIAL_MAX + 1);
				if (ScenarioNo == "1章-1" && num <= 0)
				{
					num = 1;
				}
				return num;
			}
			set
			{
				PlayerPrefs.SetInt("TutorialLv", value);
			}
		}

		[AppUtil.Title("属性(L/C)")]
		public static int LCType
		{
			get
			{
				int num = 0;
				for (int i = UserChoice.Min; i <= UserChoice.Max; i++)
				{
					string title = UserChoice.GetTitle(i);
					if (!title.Contains("対話"))
					{
						continue;
					}
					string text = UserChoice.Get(title);
					if (!(text == "Law"))
					{
						if (text == "Chaos")
						{
							num--;
						}
					}
					else
					{
						num++;
					}
				}
				return num;
			}
		}

		[AppUtil.Title("属性傾向")]
		public static string LCTypeTrend
		{
			get
			{
				if (Mathf.Abs(LCType) <= 3)
				{
					return "Neutral";
				}
				if (LCType >= 0)
				{
					return "Law";
				}
				return "Chaos";
			}
		}

		[AppUtil.Title("ルート")]
		public static string Route
		{
			get
			{
				return PlayerPrefs.GetString("Route");
			}
			set
			{
				PlayerPrefs.SetString("Route", value);
			}
		}

		[AppUtil.Title("ルート傾向")]
		public static string RouteTrend
		{
			get
			{
				if (Route != "")
				{
					return Route;
				}
				if (PlayerResult.Ending.Contains("SE") && PlayerResult.Ending.Contains("ID") && -3 <= LCType && LCType <= 3)
				{
					return "AE";
				}
				if (LCType >= 0)
				{
					return "SE";
				}
				return "ID";
			}
		}

		[AppUtil.Title("３章ルート")]
		public static string Route3
		{
			get
			{
				string text = PlayerPrefs.GetString("Route3");
				if (text == "")
				{
					text = Route;
					Route3 = Route;
				}
				return text;
			}
			set
			{
				PlayerPrefs.SetString("Route3", value);
			}
		}

		[AppUtil.Title("レビュー促進状況")]
		public static string PromptReview
		{
			get
			{
				return PlayerPrefs.GetString("PromptReview");
			}
			set
			{
				PlayerPrefs.SetString("PromptReview", value);
			}
		}

		[AppUtil.Title("日替わり会話")]
		public static bool EnableDailyBonus
		{
			get
			{
				return PlayerPrefs.GetInt("EnableDailyBonus", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("EnableDailyBonus", value ? 1 : 0);
			}
		}

		[AppUtil.Title("読書中の本")]
		public static int ReadingBook
		{
			get
			{
				return PlayerPrefs.GetInt("ReadingBookHayakawa");
			}
			set
			{
				PlayerPrefs.SetInt("ReadingBookHayakawa", value);
			}
		}

		[AppUtil.Title("借りた本")]
		public static string ReadingBookList
		{
			get
			{
				return PlayerPrefs.GetString("ReadingBookHayakawaList");
			}
			set
			{
				PlayerPrefs.SetString("ReadingBookHayakawaList", value);
				Data.UpdateEgoBookPower();
			}
		}
	}
}

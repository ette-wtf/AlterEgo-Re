using System;
using System.Collections.Generic;
using UnityEngine;

namespace App
{
	public static class Data
	{
		public enum BOOK
		{
			PRICE = 0,
			PERSECOND = 1
		}

		private enum PRICE
		{
			BOOK = 0,
			SCENARIO = 1,
			HAYAKAWA = 2
		}

		public static readonly string DATA_FILE_PATH;

		public static DateTime LastUpdateTime;

		public static IDictionary<string, string[]> DIC;

		public static List<string[]> SCENARIO_DATA;

		public static List<string> SCENARIO_DATA_KEY;

		public static List<string> COUNSELING_TYPE_LIST;

		public static List<string> CHOICE_TYPE_LIST;

		public static List<string[]> PRIZE_DATA;

		public static List<string[]> SOUND_DATA;

		public static List<string[]> BOOKEGO_DATA;

		private static List<string> comment_list;

		private static EgoPoint _EgoPerSecond;

		private static List<string> _clear_scenario_list;

		private static EgoPoint[] _NEXT_PRICE_LIST;

		private static List<int> _EgoBookPower;

		public static string[] COMMENT_KEY_LIST
		{
			get
			{
				if (comment_list == null)
				{
					UpdateCommentList();
				}
				return comment_list.ToArray();
			}
		}

		public static EgoPoint EgoPerSecond
		{
			get
			{
				if (_EgoPerSecond == null)
				{
					UpdateEgoPerSecond();
				}
				return _EgoPerSecond;
			}
			set
			{
				_EgoPerSecond = value;
			}
		}

		public static List<string> CLEAR_SCENARIO_LIST
		{
			get
			{
				if (_clear_scenario_list == null)
				{
					UpdateClearScenarioList();
				}
				return _clear_scenario_list;
			}
		}

		public static EgoPoint NEXT_BOOK_PRICE
		{
			get
			{
				return NEXT_PRICE_LIST(PRICE.BOOK);
			}
		}

		public static EgoPoint NEXT_SCENARIO_PRICE
		{
			get
			{
				return NEXT_PRICE_LIST(PRICE.SCENARIO);
			}
		}

		public static EgoPoint NEXT_HAYAKAWA_BOOK_PRICE
		{
			get
			{
				return NEXT_PRICE_LIST(PRICE.HAYAKAWA);
			}
		}

		public static bool ReadingBookIsOver
		{
			get
			{
				return PlayerStatus.ReadingBookList.Length >= 30;
			}
		}

		private static List<int> EgoBookPower
		{
			get
			{
				if (_EgoBookPower == null)
				{
					UpdateEgoBookPower();
				}
				return _EgoBookPower;
			}
		}

		static Data()
		{
			DATA_FILE_PATH = "Ego/scenario-SJIS.txt";
			_EgoPerSecond = null;
			_EgoBookPower = null;
			Init();
			ReadData();
		}

		public static void Init()
		{
			comment_list = null;
			_clear_scenario_list = null;
			_EgoPerSecond = null;
			_EgoBookPower = null;
			_NEXT_PRICE_LIST = null;
		}

		public static void ReadData()
		{
			DIC = new Dictionary<string, string[]>();
			SCENARIO_DATA = new List<string[]>();
			SCENARIO_DATA_KEY = new List<string>();
			COUNSELING_TYPE_LIST = new List<string>();
			CHOICE_TYPE_LIST = new List<string>();
			PRIZE_DATA = new List<string[]>();
			SOUND_DATA = new List<string[]>();
			BOOKEGO_DATA = new List<string[]>();
			LanguageManager.STRINGS_TABLE.Clear();
			LanguageManager.FONT_TABLE.Clear();
			LastUpdateTime = DateTime.Parse("1900/1/1");
			string[] array = new string[2] { "scenario", "data" };
			foreach (string text in array)
			{
				UpdateGameData(Resources.Load<TextAsset>("Scenario/" + text + "-UTF8.txt").text.Replace("\t", "\\t"), true);
			}
			foreach (string[] sCENARIO_DATum in SCENARIO_DATA)
			{
				if (sCENARIO_DATum[1].Contains("診断") && !sCENARIO_DATum[2].Contains("ルート分岐"))
				{
					COUNSELING_TYPE_LIST.Add(sCENARIO_DATum[1]);
				}
				if (!sCENARIO_DATum[0].Contains("4章") && (sCENARIO_DATum[1].Contains("診断") || sCENARIO_DATum[1].Contains("対話")) && !CHOICE_TYPE_LIST.Contains(sCENARIO_DATum[1]))
				{
					CHOICE_TYPE_LIST.Add(sCENARIO_DATum[1]);
				}
			}
			COUNSELING_TYPE_LIST.Add("ゲームクリア");
			array = Resources.Load<TextAsset>("Scenario/BookEgoList.txt").text.Split('\n');
			foreach (string text2 in array)
			{
				BOOKEGO_DATA.Add(text2.Split('\t'));
			}
		}

		public static int[] GET_COUNSELING_TABLE(string scenarioType, int index)
		{
			return Array.ConvertAll(DIC[scenarioType + "_CDATA_" + index], int.Parse);
		}

		public static string[] GET_COUNSELING_TYPE(string scenarioType)
		{
			string key = scenarioType + "_CTYPE";
			if (DIC.ContainsKey(key))
			{
				return DIC[key];
			}
			return null;
		}

		public static EgoPoint[] BOOK_PARAMETER(int index)
		{
			return Array.ConvertAll(DIC["BOOK_PARAMETER" + index], (string s) => new EgoPoint(s));
		}

		public static void UpdateCommentList()
		{
			comment_list = new List<string>();
			bool flag = false;
			switch (PlayerStatus.ScenarioNo)
			{
			case "3章ID-6":
			case "4章ID-1":
			case "4章ID-2":
				flag = true;
				break;
			case "3章SE-6":
			case "4章SE-1":
			case "4章SE-2":
				flag = true;
				break;
			}
			if (flag)
			{
				string[] sCENARIO_NO_LIST = PlayerStatus.SCENARIO_NO_LIST;
				foreach (string text in sCENARIO_NO_LIST)
				{
					if (text.Contains(PlayerStatus.Route))
					{
						string scenarioType = GetScenarioType(text);
						string text2 = "[Es]" + scenarioType + "_吹出";
						for (int j = 1; j <= 8 && LanguageManager.Contains(text2 + j); j++)
						{
							comment_list.Add(text2 + j);
						}
					}
				}
				return;
			}
			comment_list.Add("[Fukidasi]Base");
			for (int k = CounselingResult.Min; k <= CounselingResult.Max; k++)
			{
				for (int l = 1; l <= 10; l++)
				{
					string title = CounselingResult.GetTitle(k);
					string text3 = "[Fukidasi]CR" + title + "_" + CounselingResult.Get(title) + "-" + l;
					if (!LanguageManager.Contains(text3))
					{
						break;
					}
					comment_list.Add(text3);
				}
			}
			for (int m = 1; m <= BookLevel.Max; m++)
			{
				int rank = BookLevel.GetRank(m.ToString());
				for (int n = 1; n <= 5; n++)
				{
					if (rank >= n)
					{
						string text4 = "[Fukidasi]Book" + m + "Rank" + n;
						if (LanguageManager.Contains(text4) && LanguageManager.Get(text4) != "0")
						{
							comment_list.Add(text4);
						}
					}
				}
			}
			if (PlayerStatus.Route != "ID" && PlayerStatus.Route != "SE")
			{
				return;
			}
			for (int num = UserChoice.Min; num <= UserChoice.Max; num++)
			{
				string title2 = UserChoice.GetTitle(num);
				string text5 = "[Es]" + title2 + "_吹出";
				if (!LanguageManager.Contains(text5 + 1))
				{
					continue;
				}
				int num2 = 0;
				if (PlayerStatus.Route == "ID")
				{
					if (UserChoice.Get(title2) == "Chaos")
					{
						num2 = 4;
					}
					else if (UserChoice.Get(title2) == "Neutral")
					{
						num2 = 2;
					}
				}
				else if (PlayerStatus.Route == "SE")
				{
					if (UserChoice.Get(title2) == "Law")
					{
						num2 = 4;
					}
					else if (UserChoice.Get(title2) == "Neutral")
					{
						num2 = 2;
					}
				}
				for (int num3 = 1; num3 <= num2 && LanguageManager.Contains(text5 + num3); num3++)
				{
					comment_list.Add(text5 + num3);
				}
			}
		}

		public static void UpdateEgoPerSecond()
		{
			EgoPoint egoPerSecond = new EgoPoint(0f);
			for (int i = 1; i <= BookLevel.Max; i++)
			{
				egoPerSecond += BookEgoPoint.GetBookEgo(i.ToString(), "per_second", BookLevel.Get(i.ToString()));
			}
			_EgoPerSecond = egoPerSecond;
			if (GameObject.Find("BookListView") != null)
			{
				GameObject.Find("BookListView").BroadcastMessage("UpdateButton");
			}
		}

		public static void UpdateClearScenarioList()
		{
			_clear_scenario_list = new List<string>();
			if (PlayerStatus.ScenarioNo == "退行")
			{
				return;
			}
			string route = PlayerStatus.Route;
			string[] sCENARIO_NO_LIST = PlayerStatus.SCENARIO_NO_LIST;
			foreach (string text in sCENARIO_NO_LIST)
			{
				if (text == PlayerStatus.ScenarioNo)
				{
					break;
				}
				if (PlayerStatus.ScenarioNo.Contains("4章AE"))
				{
					if (!text.Contains("ID") && !text.Contains("SE"))
					{
						_clear_scenario_list.Add(text);
					}
				}
				else
				{
					if (text.Contains("3章") && PlayerStatus.Route != PlayerStatus.Route3)
					{
						continue;
					}
					if (text.Contains("ID") || text.Contains("SE") || text.Contains("AE"))
					{
						if (text.Contains(route))
						{
							_clear_scenario_list.Add(text);
						}
					}
					else
					{
						_clear_scenario_list.Add(text);
					}
				}
			}
			if (_clear_scenario_list.Contains("1章-3") && TimeManager.Get(TimeManager.TYPE.NEXT_BONUS_ES) == TimeSpan.MaxValue)
			{
				TimeManager.Reset(TimeManager.TYPE.NEXT_BONUS_ES, TimeSpan.FromMinutes(0.20000000298023224));
				TimeManager.Reset(TimeManager.TYPE.NEXT_BONUS_BOOK);
			}
			if (!PlayerStatus.ScenarioNo.Contains("4章AE") && PlayerStatus.Route != PlayerStatus.Route3)
			{
				_clear_scenario_list.AddRange(new string[4] { "3章AE-1", "3章AE-2", "3章AE-3", "3章AE-4" });
				_clear_scenario_list.Add("3章" + PlayerStatus.Route + "-5");
				if (PlayerStatus.ScenarioNo.Contains("4章"))
				{
					_clear_scenario_list.Add("3章" + PlayerStatus.Route + "-6");
				}
			}
		}

		private static EgoPoint NEXT_PRICE_LIST(PRICE index)
		{
			if (_NEXT_PRICE_LIST == null)
			{
				UpdatePriceList();
			}
			return _NEXT_PRICE_LIST[(int)index];
		}

		public static void UpdatePriceList()
		{
			_NEXT_PRICE_LIST = new EgoPoint[3];
			EgoPoint egoPoint = null;
			for (int i = 1; i <= BookLevel.Max; i++)
			{
				if (BookLevel.Get(i.ToString()) < 0)
				{
					egoPoint = BookEgoPoint.GetBookEgo(i.ToString(), "price", -1);
					break;
				}
			}
			_NEXT_PRICE_LIST[0] = egoPoint;
			EgoPoint egoPoint2 = null;
			string[] scenarioItemData = GetScenarioItemData(PlayerStatus.ScenarioNo);
			if (scenarioItemData != null && !string.IsNullOrEmpty(scenarioItemData[0]))
			{
				egoPoint2 = new EgoPoint(scenarioItemData[0]);
			}
			_NEXT_PRICE_LIST[1] = egoPoint2;
			EgoPoint egoPoint3 = new EgoPoint(0f);
			if (!ReadingBookIsOver)
			{
				egoPoint3 = new EgoPoint(GssDataHelper.GetData("その他", "読書消費EGO_A")[1]);
				int intParameter = Utility.GetIntParameter("読書消費EGO_B");
				for (int j = 0; j < PlayerStatus.ReadingBookList.Length; j++)
				{
					egoPoint3 *= (float)intParameter;
				}
			}
			_NEXT_PRICE_LIST[2] = egoPoint3;
		}

		public static void GoToNextScenario()
		{
			PlayerStatus.ScenarioNo = GetNextScenarioNo();
		}

		public static string GetNextScenarioNo()
		{
			switch (PlayerStatus.ScenarioNo)
			{
			case "3章ID-6":
				return "4章ID-1";
			case "3章SE-6":
				return "4章SE-1";
			case "3章AE-6":
				return "4章AE-1";
			default:
			{
				int scenarioIndex = GetScenarioIndex(PlayerStatus.ScenarioNo);
				if (scenarioIndex < 0)
				{
					return "";
				}
				scenarioIndex++;
				if (scenarioIndex >= SCENARIO_DATA_KEY.Count)
				{
					return "";
				}
				return SCENARIO_DATA_KEY[scenarioIndex];
			}
			}
		}

		public static string GetNextScenarioSpecific()
		{
			return GetScenarioSpecific(GetNextScenarioNo());
		}

		public static int GetScenarioIndex(string no)
		{
			return SCENARIO_DATA_KEY.IndexOf(no);
		}

		public static string GetScenarioType(string no)
		{
			int scenarioIndex = GetScenarioIndex(no);
			if (scenarioIndex < 0)
			{
				return "";
			}
			return SCENARIO_DATA[scenarioIndex][1];
		}

		public static string GetScenarioSpecific(string no = "")
		{
			if (no == "")
			{
				no = PlayerStatus.ScenarioNo;
			}
			int scenarioIndex = GetScenarioIndex(no);
			if (scenarioIndex < 0)
			{
				return "";
			}
			if (scenarioIndex >= SCENARIO_DATA.Count)
			{
				return "";
			}
			return SCENARIO_DATA[scenarioIndex][2];
		}

		public static string[] GetScenarioItemData(string no)
		{
			int scenarioIndex = GetScenarioIndex(no);
			if (scenarioIndex < 0)
			{
				return null;
			}
			return new List<string>
			{
				SCENARIO_DATA[scenarioIndex][3],
				SCENARIO_DATA[scenarioIndex][4],
				SCENARIO_DATA[scenarioIndex][5]
			}.ToArray();
		}

		public static string GetScenarioKey(string scenarioType, string type1, string type2 = null, int index = 0)
		{
			string text = "_" + type1;
			if (type2 != null)
			{
				text = text + "-" + type2;
			}
			if (index > 0)
			{
				text = text + "-" + index;
			}
			string text2 = "[Es]" + scenarioType + text;
			if (LanguageManager.Contains(text2))
			{
				return text2;
			}
			return null;
		}

		public static bool UpdateGameData(byte[] data)
		{
			return UpdateGameData(AppUtil.ConvertSJISToJson(data), false);
		}

		public static bool UpdateGameData(string jsondata, bool init)
		{
			string[] string_array = JsonUtility.FromJson<AppUtil.JsonString>(jsondata).string_array;
			DateTime dateTime = DateTime.Parse(string_array[0]);
			if (dateTime > LastUpdateTime)
			{
				LastUpdateTime = dateTime;
			}
			else if (!init)
			{
				return false;
			}
			for (int i = 1; i < string_array.Length; i++)
			{
				if (string_array[i].StartsWith("[[SheetName]]"))
				{
					switch (string_array[i].Replace("[[SheetName]]", ""))
					{
					case "GameData":
						i = SetGameData(string_array, i + 1);
						break;
					case "UI":
						i = SetLocalizeData(string_array, i + 1, true);
						break;
					case "ScenarioList":
						i = SetScenarioData(string_array, i + 1);
						break;
					case "Translation":
						i = SetLocalizeData(string_array, i + 1, false);
						break;
					case "目標達成":
						i = SetDataArray(string_array, i + 1, PRIZE_DATA);
						break;
					case "Sound":
						i = SetDataArray(string_array, i + 1, SOUND_DATA);
						break;
					case "MasterData":
						i = OverwriteEgoData(string_array, i + 1);
						break;
					}
				}
			}
			return true;
		}

		private static int SetGameData(string[] allData, int start)
		{
			int i;
			for (i = start + 1; i < allData.Length; i++)
			{
				string[] array = allData[i].Split('\t');
				if (array[0].Contains("[[SheetName]]"))
				{
					break;
				}
				if (array[1] == "")
				{
					continue;
				}
				int num = 0;
				for (int j = 2; j < array.Length; j++)
				{
					if (array[j] != "")
					{
						num++;
					}
				}
				string[] array2 = new string[num];
				Array.Copy(array, 2, array2, 0, num);
				try
				{
					DIC.Add(array[1], array2);
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.ToString() + array[1] + ":" + string.Join(",", array2));
				}
			}
			return i - 1;
		}

		private static int SetLocalizeData(string[] allData, int start, bool UI)
		{
			int i;
			for (i = start + 1; i < allData.Length; i++)
			{
				string[] array = allData[i].Split('\t');
				if (array[0].Contains("[[SheetName]]"))
				{
					break;
				}
				string text = array[2];
				if (UI)
				{
					text = array[0];
					LanguageManager.FONT_TABLE.Add(text, new string[2]
					{
						array[5],
						array[6]
					});
				}
				if (!(text == ""))
				{
					if (LanguageManager.STRINGS_TABLE.ContainsKey(text))
					{
						Debug.LogError("SetLocalizeData key=" + text);
						continue;
					}
					LanguageManager.STRINGS_TABLE.Add(text, new string[2]
					{
						array[3],
						array[4]
					});
				}
			}
			return i - 1;
		}

		private static int SetScenarioData(string[] allData, int start)
		{
			int i;
			for (i = start + 1; i < allData.Length; i++)
			{
				string[] array = allData[i].Split('\t');
				if (array[0].Contains("[[SheetName]]"))
				{
					break;
				}
				SCENARIO_DATA_KEY.Add(array[0]);
				SCENARIO_DATA.Add(array);
			}
			return i - 1;
		}

		public static int OverwriteEgoData(string[] allData, int start)
		{
			int i;
			for (i = start + 1; i < allData.Length; i++)
			{
				string[] array = allData[i].Split('\t');
				if (array[0].Contains("[[SheetName]]"))
				{
					break;
				}
				string text = array[0].Replace("[MasterData]", "");
				if (text.Contains("BOOK_"))
				{
					if (DIC.ContainsKey(text))
					{
						DIC[text][0] = array[1];
						DIC[text][1] = array[2];
						DIC[text][2] = array[3];
					}
					else
					{
						DIC.Add(text, AppUtil.GetChildArray(array, 1, false));
					}
				}
				else if (text.Contains("章"))
				{
					int scenarioIndex = GetScenarioIndex(text);
					SCENARIO_DATA[scenarioIndex][3] = array[1];
					SCENARIO_DATA[scenarioIndex][4] = array[2];
					SCENARIO_DATA[scenarioIndex][5] = array[3];
				}
				else if (text.Contains("目標"))
				{
					int index = int.Parse(text.Replace("目標", "")) - 1;
					PRIZE_DATA[index][4] = array[2];
					PRIZE_DATA[index][5] = array[3];
				}
			}
			return i - 1;
		}

		private static int SetDataArray(string[] allData, int start, List<string[]> outputList)
		{
			int i;
			for (i = start + 1; i < allData.Length; i++)
			{
				string[] array = allData[i].Split('\t');
				if (array[0].Contains("[[SheetName]]"))
				{
					break;
				}
				outputList.Add(array);
			}
			return i - 1;
		}

		public static void Restart()
		{
			SceneTransition.LoadScene("探求");
			BookLevel.Init();
			UserChoice.Init();
			PlayerStatus.Route = "";
			PlayerStatus.Route3 = "";
			PlayerStatus.ScenarioNo = "退行";
			UpdateClearScenarioList();
			UpdateEgoBookPower();
			PlayerStatus.EgoPoint = new EgoPoint(0f);
		}

		public static string GetRoute(string scenarioNo)
		{
			string[] array = new string[3] { "ID", "SE", "AE" };
			foreach (string text in array)
			{
				if (scenarioNo.Contains(text))
				{
					return text;
				}
			}
			return "";
		}

		public static EgoPoint GetEgoTapPower()
		{
			EgoPoint egoPoint = new EgoPoint(1f);
			foreach (string item in CLEAR_SCENARIO_LIST)
			{
				string[] scenarioItemData = GetScenarioItemData(item);
				if (scenarioItemData != null && scenarioItemData[1] == "タップ")
				{
					int num = scenarioItemData[2].Length - 1;
					for (int i = 0; i < num; i++)
					{
						egoPoint *= 10f;
					}
				}
			}
			foreach (string[] pRIZE_DATum in PRIZE_DATA)
			{
				string text = pRIZE_DATum[3];
				if (text.Contains("タップ"))
				{
					for (int j = 0; j < PrizeLevel.Get(pRIZE_DATum[1]); j++)
					{
						string s = text.Replace("タップ獲得EGO ", "").Replace("倍", "");
						egoPoint *= (float)int.Parse(s);
					}
				}
			}
			if (PurchasingItem.GetByLabel("タップ獲得EGO1000倍"))
			{
				egoPoint *= 1000f;
			}
			return egoPoint * BonusTime.BonusValue;
		}

		public static void UpdateEgoBookPower()
		{
			_EgoBookPower = new List<int>();
			long num = 1L;
			foreach (string item in CLEAR_SCENARIO_LIST)
			{
				string[] scenarioItemData = GetScenarioItemData(item);
				if (scenarioItemData != null && scenarioItemData[1] == "時間")
				{
					int effect = int.Parse(scenarioItemData[2]);
					num = AddEgoPerSecond(num, effect);
				}
			}
			foreach (string[] pRIZE_DATum in PRIZE_DATA)
			{
				if (pRIZE_DATum[3] == "本の獲得EGO 2倍")
				{
					int num2 = PrizeLevel.Get(pRIZE_DATum[1]);
					int effect2 = 2;
					for (int i = 0; i < num2; i++)
					{
						num = AddEgoPerSecond(num, effect2);
					}
				}
			}
			if (PurchasingItem.GetByLabel("本の獲得EGO10倍"))
			{
				int effect3 = 10;
				num = AddEgoPerSecond(num, effect3);
			}
			if (PlayerStatus.ReadingBookList.Length > 0)
			{
				int num3 = Math.Min(PlayerStatus.ReadingBookList.Length, 30);
				int effect4 = (int)Math.Pow(2.0, num3);
				num = AddEgoPerSecond(num, effect4);
			}
			_EgoBookPower.Add((int)num);
			UpdateEgoPerSecond();
			UpdatePriceList();
		}

		private static long AddEgoPerSecond(long sum, int effect)
		{
			if (sum * effect > int.MaxValue)
			{
				_EgoBookPower.Add((int)sum);
				return effect;
			}
			return sum * effect;
		}

		public static EgoPoint GetEgoBookPower(EgoPoint baseEgo, bool multiple)
		{
			foreach (int item in EgoBookPower)
			{
				if (multiple)
				{
					baseEgo *= (float)item;
				}
				else
				{
					baseEgo /= (long)item;
				}
			}
			if (multiple)
			{
				baseEgo *= (float)BonusTime.BonusValue;
			}
			else
			{
				baseEgo /= (long)BonusTime.BonusValue;
			}
			return baseEgo;
		}

		public static EgoPoint TapPower(bool close)
		{
			return GetEgoTapPower() * (close ? 5 : 3);
		}
	}
}

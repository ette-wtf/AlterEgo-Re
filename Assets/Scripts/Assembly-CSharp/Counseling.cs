using System;
using System.Collections.Generic;
using System.Linq;
using App;

public class Counseling
{
	private static Random rnd = new Random();

	private static string[] TypeString3;

	private static string ScenarioType;

	public static string[] GetCounselingType(string scenarioType, string data)
	{
		ScenarioType = scenarioType;
		Debug.Log("GetCounselingType scenarioType=" + ScenarioType);
		if (ScenarioType.Contains("診断1"))
		{
			return GetCounselingType1(data);
		}
		if (ScenarioType.Contains("診断2"))
		{
			return GetCounselingType2(data);
		}
		if (ScenarioType.Contains("診断3"))
		{
			return GetCounselingType3(data);
		}
		if (ScenarioType.Contains("診断4"))
		{
			return GetCounselingType4(data);
		}
		return null;
	}

	private static string[] GetCounselingType1(string data)
	{
		string[] array = data.Split(',');
		int[] array2 = new int[Data.GET_COUNSELING_TYPE(ScenarioType).Length];
		for (int i = 0; i < array.Length - 1; i++)
		{
			int[] array3 = Data.GET_COUNSELING_TABLE(ScenarioType, i + 1);
			switch (array[i])
			{
			case "Yes":
				array2[array3[0] - 1] += 10;
				break;
			case "No":
				array2[array3[1] - 1] += 10;
				break;
			case "N/A":
				array2[array3[2] - 1] += 5;
				break;
			}
		}
		int num = Array.IndexOf(array2, new List<int>(array2).Max());
		return new string[1] { Data.GET_COUNSELING_TYPE(ScenarioType)[num] };
	}

	private static string[] GetCounselingType2(string data)
	{
		string text = data.Substring(0, data.Length - 2);
		string text2 = data.Substring(data.Length - 2, 1);
		string text3 = "N";
		if (text.Length - text.Replace("L", "").Length >= 2)
		{
			text3 = "L";
		}
		else if (text.Length - text.Replace("C", "").Length >= 2)
		{
			text3 = "C";
		}
		return new string[1] { text3 + text2 };
	}

	private static string[] GetCounselingType4(string data)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		List<string> list = new List<string>();
		list.Add("");
		string[] array = Data.GET_COUNSELING_TYPE(ScenarioType);
		foreach (string text in array)
		{
			string scenarioKey = Data.GetScenarioKey(ScenarioType, "問", text);
			if (scenarioKey == null)
			{
				continue;
			}
			string[] array2 = LanguageManager.Get(scenarioKey).Split('、');
			dictionary.Add(text, 0);
			string[] array3 = array2;
			foreach (string text2 in array3)
			{
				int length = text2.Length;
				int length2 = data.Replace(text2, "").Length;
				int num = (data.Length - length2) / length;
				dictionary[text] += num;
				if (num > 0)
				{
					list.Add(text2);
				}
			}
		}
		List<KeyValuePair<string, int>> list2 = AppUtil.ReverseByValueS(dictionary);
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		int value = list2[0].Value;
		int num2 = -1;
		foreach (KeyValuePair<string, int> item in list2)
		{
			if (value == 0)
			{
				break;
			}
			if (item.Value == value)
			{
				list3.Add(item.Key);
				continue;
			}
			if (num2 < 0)
			{
				num2 = item.Value;
			}
			if (num2 != 0 && item.Value == num2)
			{
				list4.Add(item.Key);
				continue;
			}
			break;
		}
		string text3 = null;
		string text4 = null;
		if (list3.Count >= 2)
		{
			string[] array4 = list3.OrderBy((string x) => rnd.Next()).Take(2).ToArray();
			text3 = array4[0];
			text4 = array4[1];
		}
		else if (list3.Count == 1)
		{
			text3 = list3[0];
			if (list4.Count >= 1)
			{
				text4 = list4.OrderBy((string x) => rnd.Next()).Take(1).ToArray()[0];
			}
		}
		else
		{
			text3 = "その他";
		}
		if (text3 == "メンヘラ" || text4 == "メンヘラ")
		{
			text3 = "メンヘラ";
			text4 = null;
		}
		List<string> list5 = new List<string>();
		if (text4 == null)
		{
			if (text3 == "その他")
			{
				list5.Add("その他1");
				list5.Add("その他2");
				list5.Add("その他3");
			}
			else
			{
				list5.Add(text3);
			}
		}
		else
		{
			string text5 = text3 + "+" + text4;
			string text6 = text4 + "+" + text3;
			if (Data.GetScenarioKey(ScenarioType, "型", text5) != null)
			{
				list5.Add(text5);
			}
			else if (Data.GetScenarioKey(ScenarioType, "型", text6) != null)
			{
				list5.Add(text6);
			}
			else
			{
				list5.Add(text3);
				list5.Add(text4);
			}
		}
		string[] array5 = list5.ToArray();
		list[0] = array5[rnd.Next(array5.Length)];
		return list.ToArray();
	}

	private static string[] GetCounselingType3(string data)
	{
		string[] typeString = TypeString3;
		int[] array = new int[typeString.Length];
		for (int i = 0; i < typeString.Length; i++)
		{
			array[i] += (data.Length - data.Replace(typeString[i], "").Length) / typeString[i].Length;
		}
		int num = Array.IndexOf(array, new List<int>(array).Max());
		int num2 = Array.LastIndexOf(array, new List<int>(array).Min());
		return new string[1] { typeString[num] + "/" + typeString[num2] };
	}

	public static List<List<string>> GetFukidasiKeyList(string scenarioType)
	{
		ScenarioType = scenarioType;
		TypeString3 = Data.GET_COUNSELING_TYPE(ScenarioType);
		List<List<string>> list = new List<List<string>>();
		string[] typeString = TypeString3;
		foreach (string text in typeString)
		{
			List<string> list2 = new List<string>(new string[1] { text });
			string[] collection = LanguageManager.Get(Data.GetScenarioKey(ScenarioType, "問", text)).Split('、');
			list2.AddRange(new List<string>(collection).OrderBy((string x) => rnd.Next()).ToArray());
			list.Add(list2);
		}
		return list;
	}

	public static List<string> GetFukidasiKey(List<List<string>> allKeyList)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < allKeyList.Count; i++)
		{
			int num = 2;
			if (allKeyList.Count == 3 && i != 1)
			{
				num = 3;
			}
			for (int j = 0; j < num; j++)
			{
				list.Add(allKeyList[i][0] + ":" + AppUtil.Pop(allKeyList[i]));
			}
		}
		return new List<string>(list.OrderBy((string x) => rnd.Next()).ToArray());
	}

	public static List<List<string>> CustomFukidasiKeyList(List<List<string>> allKeyList, string data)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = 0; i < allKeyList.Count; i++)
		{
			dictionary.Add(i, (data.Length - data.Replace(allKeyList[i][0], "").Length) / allKeyList[i][0].Length);
		}
		List<KeyValuePair<int, int>> list = AppUtil.ReverseByValue(dictionary);
		List<List<string>> list2 = new List<List<string>>();
		list2.Add(allKeyList[list[0].Key]);
		list2.Add(allKeyList[list[1].Key]);
		list2.Add(allKeyList[list[3].Key]);
		TypeString3 = new string[3] { "", "", "" };
		for (int j = 0; j < list2.Count; j++)
		{
			TypeString3[j] = list2[j][0];
		}
		return list2;
	}
}

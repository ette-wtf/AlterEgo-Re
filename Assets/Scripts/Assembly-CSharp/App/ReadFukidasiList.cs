using System;
using UnityEngine;

namespace App
{
	public static class ReadFukidasiList
	{
		private static string[] _FukidasiList;

		private static string[] FukidasiList
		{
			get
			{
				if (_FukidasiList == null)
				{
					_FukidasiList = Resources.Load<TextAsset>("Scenario/FukidasiList").text.Split('\n');
					if (PlayerPrefs.HasKey("ReadFukidasiList"))
					{
						string[] string_array = JsonUtility.FromJson<AppUtil.JsonString>(PlayerPrefs.GetString("ReadFukidasiList")).string_array;
						for (int i = 0; i < string_array.Length; i++)
						{
							Add(string_array[i]);
						}
						PlayerPrefs.DeleteKey("ReadFukidasiList");
					}
				}
				return _FukidasiList;
			}
		}

		public static string fukidasiListFlg
		{
			get
			{
				return PlayerPrefs.GetString("ReadFukidasiFlg");
			}
			set
			{
				PlayerPrefs.SetString("ReadFukidasiFlg", value);
			}
		}

		public static bool Contains(string key)
		{
			int num = Array.IndexOf(FukidasiList, key);
			return ConvertBitFlg(fukidasiListFlg, num + 1, true) == "OK";
		}

		public static bool Add(string key)
		{
			int num = Array.IndexOf(FukidasiList, key);
			string text = ConvertBitFlg(fukidasiListFlg, num + 1, false);
			if (text != fukidasiListFlg)
			{
				fukidasiListFlg = text;
				return true;
			}
			return false;
		}

		private static string ConvertBitFlg(string baseBit, int target, bool contains)
		{
			int num = (target - 1) / 4;
			int flagNum = (target - 1) % 4;
			while (baseBit.Length < num + 1)
			{
				baseBit += "0";
			}
			int baseNum = Convert.ToInt32(baseBit[num].ToString(), 16);
			if (contains)
			{
				if (!AppUtil.BitFlagIsSet(baseNum, flagNum))
				{
					return "NG";
				}
				return "OK";
			}
			int num2 = AppUtil.SetBitFlag(baseNum, flagNum, true);
			baseBit = baseBit.Remove(num, 1);
			baseBit = baseBit.Insert(num, num2.ToString("X"));
			return baseBit;
		}
	}
}

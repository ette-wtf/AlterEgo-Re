using UnityEngine;

namespace App
{
	[AppUtil.DataRange]
	[AppUtil.Title("目標達成")]
	public static class PrizeLevel
	{
		private static string label = "目標達成";

		public static int Min
		{
			get
			{
				return 1;
			}
		}

		public static int Max
		{
			get
			{
				return Data.PRIZE_DATA.Count;
			}
		}

		public static string GetTitle(int no)
		{
			return Data.PRIZE_DATA[no - 1][1];
		}

		public static int Get(string name)
		{
			return PlayerPrefs.GetInt(name + label, 0);
		}

		public static void Set(string name, int value)
		{
			PlayerPrefs.SetInt(name + label, value);
			Data.UpdateEgoBookPower();
		}

		public static void Add(string name, int add = 1)
		{
			Set(name, Get(name) + add);
		}
	}
}

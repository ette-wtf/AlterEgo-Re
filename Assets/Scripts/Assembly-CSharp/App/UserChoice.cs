using UnityEngine;

namespace App
{
	[AppUtil.DataRange]
	[AppUtil.Title("選択肢")]
	public static class UserChoice
	{
		private static string label = "UserChoice";

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
				return Data.CHOICE_TYPE_LIST.Count;
			}
		}

		public static string GetTitle(int no)
		{
			return Data.CHOICE_TYPE_LIST[no - 1];
		}

		public static string Get(string title)
		{
			return PlayerPrefs.GetString(label + title);
		}

		public static void Set(string title, string value)
		{
			PlayerPrefs.SetString(label + title, value);
			Data.UpdateCommentList();
		}

		public static void Init()
		{
			for (int i = Min; i <= Max; i++)
			{
				PlayerPrefs.DeleteKey(label + GetTitle(i));
			}
		}
	}
}

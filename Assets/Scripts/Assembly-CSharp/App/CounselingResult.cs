using UnityEngine;

namespace App
{
	[AppUtil.DataRange]
	[AppUtil.Title("診断結果")]
	public static class CounselingResult
	{
		private static string label = "Counseling";

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
				return Data.COUNSELING_TYPE_LIST.Count;
			}
		}

		public static string GetTitle(int no)
		{
			return Data.COUNSELING_TYPE_LIST[no - 1];
		}

		public static string Get(int no)
		{
			return Get(GetTitle(no));
		}

		public static string Get(string scenarioID)
		{
			return PlayerPrefs.GetString(label + scenarioID + "Result");
		}

		public static void Set(string scenarioID, string value)
		{
			PlayerPrefs.SetString(label + scenarioID + "Result", value);
			Data.UpdateCommentList();
		}

		public static void Read(string scenarioID)
		{
			PlayerPrefs.SetInt(label + scenarioID + "New", 1);
		}

		public static bool IsNew(string scenarioID)
		{
			if (Get(scenarioID) != "")
			{
				return PlayerPrefs.GetInt(label + scenarioID + "New", 0) == 0;
			}
			return false;
		}
	}
}

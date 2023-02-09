namespace App
{
	public static class Utility
	{
		public static string GetSoundName(params string[] eventList)
		{
			string text = "";
			foreach (string[] sOUND_DATum in Data.SOUND_DATA)
			{
				bool num = sOUND_DATum[2] == "" || sOUND_DATum[2] == eventList[0];
				bool flag = sOUND_DATum[3] == "" || sOUND_DATum[3] == eventList[1];
				bool flag2 = sOUND_DATum[4] == "" || sOUND_DATum[4] == eventList[2];
				if (num && flag && flag2)
				{
					text = sOUND_DATum[1];
					break;
				}
			}
			Debug.Log("GetSoundName " + string.Join(",", eventList) + "=" + text);
			return text;
		}

		public static bool EsExists()
		{
			if (PlayerStatus.TutorialLv < 7)
			{
				return false;
			}
			if (Data.GetScenarioSpecific().Contains("エス不在"))
			{
				return false;
			}
			return true;
		}

		public static void PromptReview(string type)
		{
			if (!PlayerStatus.PromptReview.Contains(type))
			{
				if (!AppUtil.CallReviewView())
				{
					DialogManager.ShowDialog("ReviewDialog");
				}
				PlayerStatus.PromptReview = PlayerStatus.PromptReview + type + ",";
			}
		}

		public static int GetIntParameter(string name)
		{
			return int.Parse(GssDataHelper.GetData("その他", name)[1]);
		}
	}
}

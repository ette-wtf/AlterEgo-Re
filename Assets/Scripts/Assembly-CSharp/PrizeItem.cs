using App;

public class PrizeItem
{
	private int ItemNum;

	private string Condition;

	public bool IsMax
	{
		get
		{
			int num = int.Parse(Data.PRIZE_DATA[ItemNum - 1][5]);
			int num2 = int.Parse(Data.PRIZE_DATA[ItemNum - 1][4]);
			return PrizeLevel.Get(Condition) >= num / num2;
		}
	}

	public PrizeItem(int num)
	{
		ItemNum = num;
		Condition = Data.PRIZE_DATA[ItemNum - 1][1];
	}

	public string GetGift()
	{
		string baseText = Data.PRIZE_DATA[ItemNum - 1][3];
		return GetGiftText(baseText);
	}

	public string GetGiftLocalizeText()
	{
		string baseText = LanguageManager.Get("[Prize]Gift" + ItemNum);
		return GetGiftText(baseText);
	}

	private string GetGiftText(string baseText)
	{
		if (baseText.Contains("{0}"))
		{
			EgoPoint egoPoint = PlayerStatus.EgoPerSecond * 60f * 60f;
			if (baseText.Contains("3時間分"))
			{
				egoPoint *= 3f;
			}
			return string.Format(baseText, egoPoint.ToString());
		}
		return baseText;
	}

	public float GetCurrentValueRate()
	{
		if (Condition.Contains("本の獲得EGO"))
		{
			EgoPoint egoPoint = new EgoPoint(GetCurrentValue());
			EgoPoint egoPoint2 = new EgoPoint(GetNextCondition());
			return egoPoint / egoPoint2;
		}
		float num = int.Parse(GetCurrentValue());
		float num2 = int.Parse(GetNextCondition());
		return num / num2;
	}

	public string GetCurrentValue()
	{
		switch (Condition)
		{
		case "獲得EGO/秒":
			return PlayerStatus.EgoPerSecond.ToString();
		case "本ページ合計":
			return PlayerResult.BookPageSum.ToString();
		case "獲得EGO/タップ":
			return PlayerResult.TapMax.ToString();
		case "吹き出しタップ回数":
			return PlayerResult.TapCount.ToString();
		case "動画広告視聴回数":
			return PlayerResult.AdMovieCount.ToString();
		case "目標達成回数":
			return PlayerResult.PrizeCount.ToString();
		case "エンディング回数":
			return PlayerResult.EndingCount.ToString();
		case "ログイン日数":
			return PlayerResult.LoginCount.ToString();
		default:
			return "";
		}
	}

	public string GetNextCondition()
	{
		string s = Data.PRIZE_DATA[ItemNum - 1][4];
		if (Condition.Contains("本の獲得EGO"))
		{
			int num = int.Parse(s);
			EgoPoint egoPoint = new EgoPoint(num);
			for (int i = 0; i < PrizeLevel.Get(Condition); i++)
			{
				egoPoint *= (float)num;
			}
			return egoPoint.ToString();
		}
		if (IsMax)
		{
			return Data.PRIZE_DATA[ItemNum - 1][5];
		}
		return (int.Parse(s) * (PrizeLevel.Get(Condition) + 1)).ToString();
	}

	public void AddLevel()
	{
		PrizeLevel.Add(Condition);
	}

	public int GetLevel()
	{
		return PrizeLevel.Get(Condition);
	}

	public string GetUnit()
	{
		if (Condition.Contains("回数"))
		{
			return "回";
		}
		if (Condition.Contains("日数"))
		{
			return "日";
		}
		return "";
	}

	public bool IsEnableButton()
	{
		if (IsMax)
		{
			return false;
		}
		bool result = GetCurrentValueRate() >= 1f;
		if (PlayerStatus.EgoPerSecond.IsZero() && GetGift().Contains("時間分の"))
		{
			result = false;
		}
		return result;
	}
}

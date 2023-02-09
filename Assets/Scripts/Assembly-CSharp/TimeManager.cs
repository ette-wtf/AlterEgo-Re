using System;
using App;
using UnityEngine;

[AppUtil.DataRange]
[AppUtil.Title("時間リスト")]
public class TimeManager : MonoBehaviour
{
	public enum TYPE
	{
		GAME_TOTAL = 0,
		REAL_TOTAL = 1,
		LAST_LOGIN = 2,
		LAST_EGO = 3,
		NEXT_BONUS_BOOK = 4,
		NEXT_BONUS_ES = 5,
		END_BONUS = 6,
		END_BOOK = 7
	}

	private static DateTime LastUpdate
	{
		get
		{
			return PlayerPrefs.GetString("LastUpdate", DateTime.UtcNow.ToBinaryString()).Convert2DateTime();
		}
		set
		{
			PlayerPrefs.SetString("LastUpdate", value.ToBinaryString());
		}
	}

	private static DateTime LastLogin
	{
		get
		{
			return PlayerPrefs.GetString("LastLogin", default(DateTime).ToBinaryString()).Convert2DateTime();
		}
		set
		{
			PlayerPrefs.SetString("LastLogin", value.ToBinaryString());
		}
	}

	public static int Min
	{
		get
		{
			return 0;
		}
	}

	public static int Max
	{
		get
		{
			return Enum.GetNames(typeof(TYPE)).Length - 1;
		}
	}

	public static TimeSpan Now
	{
		get
		{
			return Get(TYPE.REAL_TOTAL);
		}
	}

	public static TYPE GetType(string type)
	{
		if (type == "")
		{
			return TYPE.REAL_TOTAL;
		}
		return (TYPE)Enum.Parse(typeof(TYPE), type);
	}

	public static string GetTitle(int index)
	{
		return ((TYPE)Enum.GetValues(typeof(TYPE)).GetValue(index)).ToString();
	}

	public static string GetKey(TYPE type)
	{
		return "Time:" + type;
	}

	public static TimeSpan Get(string type)
	{
		return Get(GetType(type));
	}

	public static TimeSpan Get(TYPE type)
	{
		TimeSpan timeSpan = TimeSpan.Zero;
		if (type.ToString().Contains("NEXT_"))
		{
			timeSpan = TimeSpan.MaxValue;
		}
		return TimeSpan.Parse(PlayerPrefs.GetString(GetKey(type), timeSpan.ToString()));
	}

	public static void Set(string type, TimeSpan value)
	{
		Set(GetType(type), value);
	}

	public static void Reset(TYPE type)
	{
		Reset(type, TimeSpan.Zero);
	}

	public static void Reset(TYPE type, TimeSpan offset)
	{
		Rebase();
		Set(type, Now + offset);
	}

	public static void Set(TYPE type, TimeSpan value)
	{
		if (Settings.GAME_SPEED != 1f)
		{
			value = new TimeSpan(value.Ticks / (long)Settings.GAME_SPEED);
		}
		PlayerPrefs.SetString(GetKey(type), value.ToString());
	}

	public static void Add(TYPE type, TimeSpan add)
	{
		Set(type, Get(type) + add);
	}

	private void Awake()
	{
		if (PlayerPrefs.HasKey("LastEgoTime"))
		{
			LastUpdate = PlayerPrefs.GetString("LastEgoTime").Convert2DateTime().ToUniversalTime();
			PlayerPrefs.DeleteKey("LastEgoTime");
		}
		DateTime utcNow = DateTime.UtcNow;
		Add(TYPE.REAL_TOTAL, utcNow - LastUpdate);
		LastUpdate = utcNow;
		UpdateLoginDate();
	}

	private void FixedUpdate()
	{
		Rebase();
	}

	private static void Rebase()
	{
		DateTime utcNow = DateTime.UtcNow;
		TimeSpan add = utcNow - LastUpdate;
		Add(TYPE.GAME_TOTAL, add);
		Add(TYPE.REAL_TOTAL, add);
		LastUpdate = utcNow;
	}

	public static TimeSpan GetGapTime(TYPE type)
	{
		return Get(type) - Now;
	}

	public static float SetLastTime(TYPE type)
	{
		float result = (float)(Now - Get(type)).TotalSeconds;
		Set(type, Now);
		return result;
	}

	public static bool IsOverTime(TYPE type)
	{
		return Now > Get(type);
	}

	public static bool IsInTime(TYPE type)
	{
		return Now < Get(type);
	}

	public static void UpdateLoginDate()
	{
		DateTime now = DateTime.Now;
		if ((now.Date - LastLogin.Date).Days > 0)
		{
			PlayerResult.LoginCount++;
			if (PlayerStatus.ScenarioNo.Contains("4章AE"))
			{
				PlayerStatus.EnableDailyBonus = true;
			}
			AnalyticsManager.SendEvent(new string[3]
			{
				"ログイン",
				PlayerResult.LoginCount + "日目",
				""
			});
		}
		LastLogin = now;
	}
}

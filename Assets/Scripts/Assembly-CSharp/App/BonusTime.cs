using System;

namespace App
{
	public static class BonusTime
	{
		public static bool IsActive
		{
			get
			{
				return TimeManager.IsInTime(TimeManager.TYPE.END_BONUS);
			}
		}

		public static int BonusValue
		{
			get
			{
				if (!IsActive)
				{
					return 1;
				}
				return 3;
			}
		}

		public static TimeSpan TimeLeft
		{
			get
			{
				return TimeManager.GetGapTime(TimeManager.TYPE.END_BONUS);
			}
		}

		public static void SetBonus()
		{
			TimeSpan offset = TimeSpan.FromSeconds(600.9000244140625);
			TimeManager.Reset(TimeManager.TYPE.END_BONUS, offset);
		}
	}
}

using System;

namespace App
{
	[AppUtil.Title("多動モード")]
	public static class HyperActive
	{
		private static int _Phase;

		private static int _Point;

		private static float _Time;

		public static TimeSpan LastTime;

		[AppUtil.Title("多動フェーズ")]
		public static int Phase
		{
			get
			{
				return _Phase;
			}
			set
			{
				LastTime = TimeManager.Now;
				Time = 0f;
				Point = 0;
				_Phase = value;
			}
		}

		[AppUtil.Title("多動ポイント")]
		public static int Point
		{
			get
			{
				return _Point;
			}
			set
			{
				_Point = value;
			}
		}

		[AppUtil.Title("多動経過時間")]
		public static float Time
		{
			get
			{
				return _Time;
			}
			set
			{
				_Time = value;
			}
		}

		[AppUtil.Hide]
		public static float PassedTime
		{
			get
			{
				float result = (float)(TimeManager.Now - LastTime).TotalSeconds;
				LastTime = TimeManager.Now;
				return result;
			}
		}

		static HyperActive()
		{
			_Phase = 0;
			_Point = 0;
			_Time = 0f;
			LastTime = TimeManager.Now;
			Init();
		}

		public static void Init()
		{
			Phase = 1;
		}
	}
}

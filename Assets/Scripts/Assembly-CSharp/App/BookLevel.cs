using UnityEngine;

namespace App
{
	[AppUtil.DataRange]
	[AppUtil.Title("読破ページ数")]
	public static class BookLevel
	{
		public const int RankMax = 5;

		private static readonly string label = "BookLevel";

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
				return 15;
			}
		}

		public static string GetTitle(int bookNo)
		{
			return bookNo.ToString();
		}

		public static string GetLabel(int bookNo)
		{
			return bookNo + "." + LanguageManager.Get("[Book]Title" + bookNo);
		}

		public static int Get(string bookNo)
		{
			return PlayerPrefs.GetInt(label + bookNo, -1);
		}

		public static void Set(string bookNo, int value)
		{
			if (value > GetPage(bookNo))
			{
				value = GetPage(bookNo);
			}
			int rank = GetRank(bookNo);
			PlayerPrefs.SetInt(label + bookNo, value);
			int rank2 = GetRank(bookNo);
			if (rank != rank2)
			{
				Data.UpdateCommentList();
				Data.UpdatePriceList();
			}
			Data.EgoPerSecond += BookEgoPoint.GetBookEgo(bookNo, "add_per_second", value - 1);
		}

		public static void Add(string bookNo)
		{
			PlayerResult.BookPageSum++;
			Set(bookNo, Get(bookNo) + 1);
		}

		public static bool IsMax(string bookNo)
		{
			return Get(bookNo) >= GetPage(bookNo);
		}

		public static void Init()
		{
			for (int i = Min; i <= Max; i++)
			{
				PlayerPrefs.DeleteKey(label + GetTitle(i));
			}
		}

		public static int GetPage(string bookNo, int rank = 5)
		{
			return GetPage(int.Parse(bookNo), rank);
		}

		public static int GetPage(int bookNo, int rank = 5)
		{
			if (rank <= 1)
			{
				return 0;
			}
			return int.Parse(LanguageManager.Get("[Book]Page" + bookNo + "Rank" + rank));
		}

		public static int GetRank(string bookNo)
		{
			return GetRank(bookNo, Get(bookNo));
		}

		public static int GetRank(string bookNo, int page)
		{
			for (int num = 5; num > 0; num--)
			{
				if (page >= GetPage(bookNo, num))
				{
					return num;
				}
			}
			return 0;
		}

		public static int GetCurrentInRank(string bookNo)
		{
			return GetCurrentInRank(bookNo, Get(bookNo));
		}

		public static int GetCurrentInRank(string bookNo, int page)
		{
			return page - GetPage(bookNo, GetRank(bookNo, page));
		}

		public static int GetMaxPageInRank(string bookNo)
		{
			return GetMaxPageInRank(bookNo, Get(bookNo));
		}

		public static int GetMaxPageInRank(string bookNo, int page)
		{
			int rank = GetRank(bookNo, page);
			return GetPage(bookNo, rank + 1) - GetPage(bookNo, rank);
		}
	}
}

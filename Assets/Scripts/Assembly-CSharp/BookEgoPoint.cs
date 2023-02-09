using System.Collections.Generic;
using System.Text;
using App;

public static class BookEgoPoint
{
	public static EgoPoint GetBookEgo(string book, string type)
	{
		return GetBookEgo(book, type, BookLevel.Get(book));
	}

	public static EgoPoint GetBookEgo(string book, string type, int page)
	{
		int num = int.Parse(book) - 1;
		switch (type)
		{
		case "price":
			return new EgoPoint(Data.BOOKEGO_DATA[num * 2][page + 1]);
		case "per_second":
			if (page < 0)
			{
				return new EgoPoint(0f);
			}
			return Data.GetEgoBookPower(new EgoPoint(Data.BOOKEGO_DATA[num * 2 + 1][page]), true);
		case "add_per_second":
			return GetBookEgo(book, "per_second", page + 1) - GetBookEgo(book, "per_second", page);
		default:
			return null;
		}
	}

	public static string[] MakeBookEgoListAll()
	{
		List<string> list = new List<string>();
		for (int i = 1; i <= BookLevel.Max; i++)
		{
			string text = i.ToString();
			EgoPoint firstPrice = Data.BOOK_PARAMETER(int.Parse(text))[0];
			EgoPoint firstPerSecond = Data.BOOK_PARAMETER(int.Parse(text))[1];
			string item = MakeBookEgoListEach(text, firstPrice, firstPerSecond);
			list.Add(item);
		}
		return list.ToArray();
	}

	public static string MakeBookEgoListEach(string book, EgoPoint firstPrice, EgoPoint firstPerSecond)
	{
		int page = BookLevel.GetPage(book);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = -1; i < page; i++)
		{
			EgoPoint egoPoint = firstPrice;
			int rank = BookLevel.GetRank(book, i);
			for (int j = 2; j <= rank; j++)
			{
				float num = float.Parse(Data.DIC["BOOK_PRICE_RANK" + j][0]);
				egoPoint *= num;
			}
			int currentInRank = BookLevel.GetCurrentInRank(book, i);
			float num2 = float.Parse(Data.DIC["BOOK_PRICE_GAIN"][0]);
			for (int k = 0; k <= currentInRank; k++)
			{
				egoPoint *= num2;
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append("\t");
			}
			stringBuilder.Append(egoPoint.ToString());
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		for (int l = 0; l <= page; l++)
		{
			EgoPoint egoPoint2 = firstPerSecond;
			float num3 = float.Parse(Data.DIC["BOOK_EARN_GAIN"][0]);
			for (int m = 0; m <= l; m++)
			{
				if (IsRankUp(book, m))
				{
					int rank2 = BookLevel.GetRank(book, m);
					egoPoint2 *= float.Parse(Data.DIC["BOOK_EARN_RANK" + rank2][0]);
				}
				else
				{
					egoPoint2 *= num3;
				}
			}
			if (stringBuilder2.Length > 0)
			{
				stringBuilder2.Append("\t");
			}
			stringBuilder2.Append(egoPoint2.ToString());
		}
		return stringBuilder.ToString() + "\n" + stringBuilder2.ToString();
	}

	public static bool IsRankUp(string book, int page)
	{
		for (int i = 1; i <= 5; i++)
		{
			if (BookLevel.GetPage(book, i) == page)
			{
				return true;
			}
		}
		return false;
	}
}

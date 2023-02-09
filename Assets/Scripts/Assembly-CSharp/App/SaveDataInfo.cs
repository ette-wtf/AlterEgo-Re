using System;

namespace App
{
	public static class SaveDataInfo
	{
		public const string VERSION = "EGO_001";

		public static readonly Type[] TypeList = new Type[7]
		{
			typeof(PlayerStatus),
			typeof(BookLevel),
			typeof(PrizeLevel),
			typeof(PlayerResult),
			typeof(CounselingResult),
			typeof(UserChoice),
			typeof(ReadFukidasiList)
		};
	}
}

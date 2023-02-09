using System;
using UnityEngine;

namespace App
{
	[AppUtil.DataRange]
	[AppUtil.Title("アイテム課金")]
	public static class PurchasingItem
	{
		public static readonly string[] ITEM_LIST = new string[6] { "item1", "item2", "item3", "item4", "item5", "item6" };

		public static readonly string[] LABEL_LIST = new string[6] { "タップ獲得EGO1000倍", "本の獲得EGO10倍", "広告削除", "自我とエス(SE)", "自我とエス(ID)", "自我とエス(AE)" };

		private static string label = "PurchasingItem";

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
				return ITEM_LIST.Length;
			}
		}

		public static string GetTitle(int no)
		{
			return ITEM_LIST[no - 1];
		}

		public static string GetLabel(int no)
		{
			return LABEL_LIST[no - 1];
		}

		public static bool GetByLabel(string itemLabel)
		{
			return Get(ITEM_LIST[Array.IndexOf(LABEL_LIST, itemLabel)]);
		}

		public static bool Get(string name)
		{
			return PlayerPrefs.GetInt(name + label, 0) > 0;
		}

		public static void Set(string name, bool value)
		{
			if (Get(name) == value)
			{
				return;
			}
			PlayerPrefs.SetInt(name + label, value ? 1 : 0);
			string text = LABEL_LIST[Array.IndexOf(ITEM_LIST, name)];
			if (!(text == "広告削除"))
			{
				if (text == "本の獲得EGO10倍")
				{
					Data.UpdateEgoBookPower();
				}
			}
			else
			{
				AdManager.SetActive(!value);
			}
			if (text.Contains("自我とエス"))
			{
				GameObject gameObject = GameObject.Find("TalkManager");
				if (gameObject != null)
				{
					gameObject.GetComponent<SceneTalk>().ResetButtonList();
				}
			}
		}
	}
}

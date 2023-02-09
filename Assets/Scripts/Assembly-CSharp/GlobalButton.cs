using System.Collections.Generic;
using App;
using UnityEngine;
using UnityEngine.UI;

public class GlobalButton : MonoBehaviour
{
	private static readonly Color COLOR_ON = Color.white;

	private static readonly Color COLOR_OFF = new Color(32f / 51f, 32f / 51f, 32f / 51f);

	private static readonly Color COLOR_LOCK = new Color(16f / 51f, 16f / 51f, 16f / 51f);

	private List<PrizeItem> PrizeList = new List<PrizeItem>();

	private void OnEnable()
	{
		for (int i = 1; i <= Data.PRIZE_DATA.Count; i++)
		{
			PrizeList.Add(new PrizeItem(i));
		}
		UpdateButtonStatus();
	}

	private void FixedUpdate()
	{
		UpdateNotice();
	}

	public void UpdateButtonStatus()
	{
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		foreach (Button button in componentsInChildren)
		{
			bool flag = button.name.Contains(base.gameObject.scene.name);
			bool flag2 = false;
			string text = button.name;
			if (text == "GlobalButton記録室" || text == "GlobalButton目標達成")
			{
				flag2 = PlayerStatus.TutorialLv <= PlayerStatus.TUTORIAL_MAX;
			}
			if (flag)
			{
				button.transform.Find("Icon").gameObject.SetActive(true);
				button.transform.Find("Text").gameObject.SetActive(true);
				button.transform.Find("Text").GetComponent<Text>().color = COLOR_ON;
				button.transform.Find("Light").gameObject.SetActive(true);
				button.GetComponent<Button>().enabled = false;
				button.GetComponent<Image>().color = COLOR_ON;
			}
			else if (flag2)
			{
				button.transform.Find("Icon").gameObject.SetActive(false);
				button.transform.Find("Text").gameObject.SetActive(false);
				button.transform.Find("Light").gameObject.SetActive(false);
				button.GetComponent<Button>().enabled = false;
				button.GetComponent<Image>().color = COLOR_LOCK;
			}
			else
			{
				button.transform.Find("Icon").gameObject.SetActive(true);
				button.transform.Find("Text").gameObject.SetActive(true);
				button.transform.Find("Text").GetComponent<Text>().color = COLOR_OFF;
				button.transform.Find("Light").gameObject.SetActive(false);
				button.GetComponent<Button>().enabled = true;
				button.GetComponent<Image>().color = COLOR_OFF;
			}
		}
	}

	private void UpdateNotice()
	{
		GameObject gameObject = GameObject.Find("GlobalButtonエスの部屋");
		bool flag;
		EgoPoint nEXT_HAYAKAWA_BOOK_PRICE;
		if (PlayerStatus.ScenarioNo.Contains("4章AE"))
		{
			flag = PlayerStatus.EnableDailyBonus;
			if (!flag)
			{
				bool num = TimeManager.IsInTime(TimeManager.TYPE.END_BOOK);
				nEXT_HAYAKAWA_BOOK_PRICE = Data.NEXT_HAYAKAWA_BOOK_PRICE;
				flag = !num && PlayerStatus.EgoPoint >= nEXT_HAYAKAWA_BOOK_PRICE;
			}
		}
		else if (PlayerStatus.ScenarioNo.Contains("4章"))
		{
			flag = true;
		}
		else
		{
			nEXT_HAYAKAWA_BOOK_PRICE = Data.NEXT_SCENARIO_PRICE;
			flag = nEXT_HAYAKAWA_BOOK_PRICE != null && PlayerStatus.EgoPoint >= nEXT_HAYAKAWA_BOOK_PRICE && Utility.EsExists();
		}
		gameObject.transform.Find("Notice").gameObject.SetActive(flag);
		if (PlayerStatus.TutorialLv <= PlayerStatus.TUTORIAL_MAX)
		{
			return;
		}
		gameObject = GameObject.Find("GlobalButton探求");
		nEXT_HAYAKAWA_BOOK_PRICE = Data.NEXT_BOOK_PRICE;
		gameObject.transform.Find("Notice").gameObject.SetActive(nEXT_HAYAKAWA_BOOK_PRICE != null && PlayerStatus.EgoPoint >= nEXT_HAYAKAWA_BOOK_PRICE);
		gameObject = GameObject.Find("GlobalButton目標達成");
		bool active = false;
		foreach (PrizeItem prize in PrizeList)
		{
			if (prize.IsEnableButton())
			{
				active = true;
				break;
			}
		}
		gameObject.transform.Find("Notice").gameObject.SetActive(active);
		gameObject = GameObject.Find("GlobalButton記録室");
		bool active2 = CounselingResult.IsNew("ゲームクリア");
		gameObject.transform.Find("Notice").gameObject.SetActive(active2);
	}

	public PrizeItem GetPrizeItem(int num)
	{
		return PrizeList[num - 1];
	}
}

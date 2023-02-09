using System;
using System.Collections;
using App;
using UnityEngine;
using UnityEngine.UI;

public class TalkItem : MonoBehaviour
{
	public string ScenarioNo;

	public EgoPoint Price;

	private string Title1;

	private TextLocalization TimeCounter;

	private string PrizeType;

	private Transform MainButton;

	private void Awake()
	{
		MainButton = base.transform.Find("ScenarioButton");
	}

	private void FixedUpdate()
	{
		UpdateStatus();
	}

	private void OnEnable()
	{
		base.transform.Find("RankUpEffect").gameObject.SetActive(false);
	}

	public void OnClick(GameObject clickObject)
	{
		Debug.Log("TalkItem:OnClick " + clickObject.name);
	}

	public void OnLevelUp(string before, string after)
	{
		StartCoroutine(LevelUpEffect(before, after));
	}

	private IEnumerator LevelUpEffect(string before, string after)
	{
		string text = base.transform.Find("EffectText").GetComponent<Text>().text;
		text = text.Replace("タップ獲得EGO", "タップ獲得EGO\n");
		after = after.Replace("/秒", "<size=38>/秒</size>");
		string text2 = "診断";
		if (Data.GetScenarioType(ScenarioNo).Contains("対話"))
		{
			text2 = "対話";
		}
		if (Data.GetScenarioSpecific(ScenarioNo) == "書評")
		{
			text2 = "本";
		}
		GameObject effect = base.transform.Find("RankUpEffect").gameObject;
		base.transform.Find("RankUpEffect/TitleText").GetComponent<TextLocalization>().SetKey("[UI]RankUpEffect/TitleText" + text2);
		base.transform.Find("RankUpEffect/ValueLayout/EffectText").GetComponent<TextLocalization>().SetText(text);
		base.transform.Find("RankUpEffect/ValueLayout/BeforeText").GetComponent<TextLocalization>().SetText(before);
		base.transform.Find("RankUpEffect/ValueLayout/AfterText").GetComponent<TextLocalization>().SetText(after);
		float num = 0f;
		RectTransform[] componentsInChildren = base.transform.Find("RankUpEffect/ValueLayout").GetComponentsInChildren<RectTransform>();
		foreach (RectTransform rectTransform in componentsInChildren)
		{
			if (!(rectTransform.name == "ValueLayout"))
			{
				rectTransform.anchoredPosition = new Vector2(num, rectTransform.anchoredPosition.y);
				float preferredWidth = rectTransform.GetComponent<Text>().preferredWidth;
				rectTransform.sizeDelta = new Vector2(preferredWidth, 100f);
				num += preferredWidth + 20f;
				if (rectTransform.name == "Arrow" || rectTransform.name == "AfterText")
				{
					num += 10f;
				}
			}
		}
		RectTransform componentInChildren = base.transform.Find("RankUpEffect/ValueLayout").GetComponentInChildren<RectTransform>();
		componentInChildren.anchoredPosition = new Vector2((componentInChildren.parent.GetComponent<RectTransform>().rect.width - num) / 2f, -30f);
		effect.SetActive(true);
		yield return AppUtil.FadeIn(effect);
		Debug.Log("AutoTestEvent:Screenshot");
		SetItem(ScenarioNo);
		yield return AppUtil.Wait(2f);
		yield return AppUtil.FadeOut(effect);
		effect.SetActive(false);
	}

	public void SetItem()
	{
		if (!(base.transform.Find("RetryNoAdButton") != null) && !(base.transform.Find("RestartButtonAE") != null))
		{
			SetItem(ScenarioNo);
		}
	}

	public void SetItem(string scenarioNo, int count = 0, bool rememberMode = false)
	{
		string scenarioSpecific = Data.GetScenarioSpecific(scenarioNo);
		if (scenarioNo.Contains("3章") && (scenarioSpecific.Contains("ルート分岐") || scenarioSpecific.Contains("章END")))
		{
			scenarioNo = scenarioNo.Replace(PlayerStatus.Route3, PlayerStatus.Route);
		}
		ScenarioNo = scenarioNo;
		if (!Utility.EsExists() && !scenarioSpecific.Contains("退行"))
		{
			base.transform.Find("ScenarioRetryButton").gameObject.SetActive(true);
			base.transform.Find("InActiveScreen").gameObject.SetActive(true);
			MainButton.gameObject.SetActive(false);
			base.transform.Find("EffectText").gameObject.SetActive(false);
			MainButton.Find("ButterflyNotice").gameObject.SetActive(false);
			MainButton.Find("Light").gameObject.SetActive(false);
			return;
		}
		base.gameObject.name = "ScenarioItem" + scenarioNo;
		string[] scenarioItemData = Data.GetScenarioItemData(scenarioNo);
		string scenarioType = Data.GetScenarioType(scenarioNo);
		if (count != 0)
		{
			Title1 = scenarioNo[0].ToString() + "-" + count + " " + LanguageManager.Get("[Es]" + scenarioType + "_タイトル");
			if (rememberMode)
			{
				string route = Data.GetRoute(scenarioNo);
				if (route != "")
				{
					Title1 = Title1 + " (" + route + ")";
				}
			}
		}
		if (scenarioNo == "エンドロール")
		{
			Title1 = "3-X " + LanguageManager.Get("[Es]対話23_タイトル");
		}
		base.transform.Find("TitleText").GetComponent<TextLocalization>().SetText(Title1);
		base.transform.Find("SubtitleText").GetComponent<TextLocalization>().SetKey("[Es]" + scenarioType + "_サブタイトル");
		if (scenarioType.Contains("診断"))
		{
			float preferredWidth = base.transform.Find("TitleText").GetComponent<Text>().preferredWidth;
			base.transform.Find("MirrorIcon").gameObject.SetActive(true);
			base.transform.Find("MirrorIcon").GetComponent<RectTransform>().anchoredPosition = new Vector2(75.3f + preferredWidth, 55.5f);
		}
		Color color = Color.white;
		if (scenarioItemData[0] == "" || scenarioItemData[0] == "0")
		{
			base.transform.Find("EffectText").gameObject.SetActive(false);
			bool flag = false;
			string text = null;
			string text2 = null;
			if (scenarioSpecific.Contains("退行"))
			{
				text = "[UI]RestartButton/Text";
				text2 = "RestartButtonAE";
				if (PlayerStatus.Route != "AE")
				{
					flag = true;
					text2 = "RestartButton";
				}
			}
			else if (scenarioSpecific.Contains("雑談"))
			{
				flag = true;
				text = "[UI]TalkButton/Text";
				text2 = "雑談Button";
				if (MainButton.Find("Light") != null)
				{
					UnityEngine.Object.Destroy(MainButton.Find("Light").gameObject);
				}
			}
			else if (scenarioSpecific.Contains("日替"))
			{
				flag = true;
				if (!PlayerStatus.EnableDailyBonus)
				{
					SetButtonActive(false);
				}
				text = "[UI]GreetingButton/Text";
				text2 = "日替わりButton";
			}
			else if (scenarioSpecific.Contains("書評"))
			{
				flag = true;
				text2 = "本を返すButton";
				TimeCounter = null;
				MainButton.Find("PriceText").GetComponent<LetterSpacing>().spacing = 0f;
				if (TimeManager.IsInTime(TimeManager.TYPE.END_BOOK))
				{
					SetButtonActive(false);
					text = "[UI]ReadingButton/Text";
					TimeCounter = MainButton.Find("PriceText").GetComponent<TextLocalization>();
					MainButton.Find("PriceText").GetComponent<TextLocalization>().SetText("");
				}
				else if (PlayerStatus.ReadingBook > 0)
				{
					text = "[UI]GetBackButton/Text";
					PrizeType = "時間";
					string id = "[UI]ScenarioItem/EffectTextTime";
					base.transform.Find("EffectText").GetComponent<TextLocalization>().SetText(2.ToString(LanguageManager.Get(id)));
				}
				else
				{
					text2 = "本を借りるButton";
					if (!Data.ReadingBookIsOver)
					{
						Price = Data.NEXT_HAYAKAWA_BOOK_PRICE;
						text = null;
						MainButton.Find("PriceText").GetComponent<TextLocalization>().SetText(Price.ToString(LanguageManager.Get("[UI]ScenarioButton/PriceText")));
					}
					else
					{
						Price = null;
						text = "[UI]TakeBookButton/Text";
						MainButton.Find("PriceText").GetComponent<LetterSpacing>().spacing = -20f;
					}
				}
				GameObject obj = base.transform.Find("BookText").gameObject;
				string text3 = Data.GetScenarioKey("本を借りる1", "9選択肢", PlayerStatus.ReadingBook.ToString());
				if (text3 == null)
				{
					text3 = "[UI]BookInfo/TitleText";
				}
				obj.GetComponent<TextLocalization>().SetKey(text3);
			}
			else if (scenarioSpecific.Contains("自我とエス"))
			{
				MainButton.gameObject.SetActive(false);
				flag = true;
				if (PurchasingItem.GetByLabel(scenarioSpecific))
				{
					if (base.transform.Find("IchEsButton") != null)
					{
						MainButton = base.transform.Find("IchEsButton");
					}
					text2 = scenarioSpecific + "Button";
				}
				else
				{
					MainButton = base.transform.Find("IchEsButtonUnlocked");
					text2 = "IchEsButtonUnlocked";
				}
			}
			MainButton.gameObject.SetActive(flag);
			base.transform.Find("ScenarioRetryButton").gameObject.SetActive(!flag);
			if (flag)
			{
				if (text != null)
				{
					MainButton.Find("PriceText").GetComponent<TextLocalization>().SetKey(text);
				}
				MainButton.gameObject.name = text2;
			}
			else
			{
				base.transform.Find("ScenarioRetryButton/Text").GetComponent<TextLocalization>().SetKey(text);
				base.transform.Find("ScenarioRetryButton").gameObject.name = text2;
			}
		}
		else if (rememberMode)
		{
			base.transform.Find("ScenarioRetryButton").gameObject.SetActive(true);
			MainButton.gameObject.SetActive(false);
			string key = "[UI]RememberButton/Text";
			base.transform.Find("ScenarioRetryButton/Text").GetComponent<TextLocalization>().SetKey(key);
			base.transform.Find("ScenarioRetryButton").gameObject.name = "RetryNoAdButton";
			base.transform.Find("EffectText").gameObject.SetActive(false);
		}
		else if (!Data.CLEAR_SCENARIO_LIST.Contains(scenarioNo))
		{
			Price = new EgoPoint(scenarioItemData[0]);
			MainButton.Find("PriceText").GetComponent<TextLocalization>().SetText(Price.ToString(LanguageManager.Get("[UI]ScenarioButton/PriceText")));
			string id2 = "[UI]ScenarioItem/EffectTextTap";
			PrizeType = "タップ";
			if (scenarioItemData[1] == "時間")
			{
				PrizeType = "時間";
				id2 = "[UI]ScenarioItem/EffectTextTime";
			}
			base.transform.Find("EffectText").GetComponent<TextLocalization>().SetText(int.Parse(scenarioItemData[2]).ToString(LanguageManager.Get(id2)));
			base.transform.Find("ScenarioRetryButton").gameObject.SetActive(false);
			if (scenarioNo != PlayerStatus.ScenarioNo)
			{
				base.transform.Find("InActiveScreen").gameObject.SetActive(true);
				SetButtonActive(false);
			}
			else
			{
				base.transform.Find("InActiveScreen").gameObject.SetActive(false);
			}
		}
		else
		{
			base.transform.Find("ScenarioRetryButton").gameObject.SetActive(true);
			if (!AdInfo.ENABLE)
			{
				base.transform.Find("ScenarioRetryButton/Text").GetComponent<TextLocalization>().SetKey("[UI]ScenarioRetryButton/TextNoAd");
			}
			MainButton.gameObject.SetActive(false);
			base.transform.Find("EffectText").gameObject.SetActive(false);
			color = new Color(0.54f, 0.54f, 0.54f);
		}
		base.transform.Find("TitleText").GetComponent<Text>().color = color;
		base.transform.Find("SubtitleText").GetComponent<Text>().color = color;
		base.transform.Find("MirrorIcon").GetComponent<Image>().color = color;
		UpdateStatus();
	}

	private void UpdateStatus()
	{
		if (TimeCounter != null)
		{
			if (TimeManager.IsInTime(TimeManager.TYPE.END_BOOK))
			{
				TimeSpan gapTime = TimeManager.GetGapTime(TimeManager.TYPE.END_BOOK);
				TimeCounter.SetParameters(gapTime.ToString("h\\:mm\\:ss"));
			}
			else
			{
				SetItem();
				SetButtonActive(true);
			}
		}
		else if (MainButton.gameObject.activeSelf && Price != null && IsActive())
		{
			bool buttonActive = PlayerStatus.EgoPoint >= Price;
			SetButtonActive(buttonActive);
		}
	}

	public bool IsActive()
	{
		return !base.transform.Find("InActiveScreen").gameObject.activeSelf;
	}

	public void SetButtonActive(bool buttonEnable)
	{
		MainButton.GetComponent<Button>().interactable = buttonEnable;
		if (MainButton.Find("ButterflyNotice") != null)
		{
			MainButton.Find("ButterflyNotice").gameObject.SetActive(buttonEnable);
		}
		if (MainButton.Find("Light") != null)
		{
			MainButton.Find("Light").gameObject.SetActive(buttonEnable);
		}
	}

	public new string GetType()
	{
		return PrizeType;
	}
}

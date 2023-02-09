using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using App;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogEgo : Dialog
{
	private float WAIT_PAGE_TIME = 0.08f;

	private float WAIT_PAGE_ALPHA = 0.9f;

	protected override bool OnClick(GameObject clickObject)
	{
		Debug.Log("DialogEgo:OnClick " + clickObject.name);
		if (base.OnClick(clickObject))
		{
			return true;
		}
		switch (clickObject.name)
		{
		case "GetButton":
		case "ReadButton":
		case "RejectButton":
		case "RetryButtonFromDialog":
		case "CounselingButton":
		case "SkipButton":
			Close();
			break;
		case "StartChapterEndButton":
			base.transform.Find("Icon1").gameObject.SetActive(false);
			base.transform.Find("Icon2").gameObject.SetActive(true);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
			AppUtil.DelayAction(this, 0.5f, delegate
			{
				Close();
			});
			break;
		case "RestartButton":
			base.transform.Find("Icon1").gameObject.SetActive(false);
			base.transform.Find("Icon2").gameObject.SetActive(true);
			GetComponent<CanvasGroup>().blocksRaycasts = false;
			AppUtil.DelayAction(this, 0.5f, delegate
			{
				Data.Restart();
			});
			break;
		case "NoPlayButton1":
			TimeManager.Reset(TimeManager.TYPE.NEXT_BONUS_BOOK, TimeSpan.FromMinutes(15.0));
			Close();
			break;
		case "NoPlayButton2":
			Close();
			break;
		case "ShopButton":
		case "Shop1Button":
			DialogManager.ShowDialog("ShopDialog", 1);
			break;
		case "Shop2Button":
			DialogManager.ShowDialog("ShopDialog", 2);
			break;
		case "TutorialButton":
			DialogManager.ShowDialog("TutorialDialog");
			break;
		case "CreditsButton":
			DialogManager.ShowDialog("CreditsDialog");
			break;
		case "AppButton":
			Application.OpenURL("http://caracolu.com/app/escape/");
			break;
		case "ReferenceButton":
			Application.OpenURL("http://alterego.caracolu.com/references");
			break;
		case "ReviewButton":
			AppUtil.OpenReviewURL();
			if (base.gameObject.name == "ReviewDialog")
			{
				Close();
			}
			break;
		case "ShopItemButton1":
		case "ShopItemButton2":
		case "ShopItemButton3":
		case "ShopItemButton4":
		case "ShopItemButton5":
		case "ShopItemButton6":
		{
			string text5 = clickObject.name.Replace("ShopItemButton", "");
			StartCoroutine(InAppPurchaser.BuyProductID("item" + text5, CallbackFromPuchaser));
			break;
		}
		case "RestoreButton":
			StartCoroutine(InAppPurchaser.RestorePurchases(CallbackFromPuchaser));
			break;
		case "BookBonusButton":
			AdManager.Show("Reward", delegate(bool reward)
			{
				RewardForBonus("Time", reward, false);
			});
			Close();
			break;
		case "EsBonusButton":
			if (AdInfo.ENABLE)
			{
				AdManager.Show("Reward", delegate(bool reward)
				{
					RewardForBonus("Temp", reward, true);
				});
			}
			else
			{
				RewardForBonus("Temp", true, false);
			}
			Close();
			break;
		case "BookCover":
			DialogManager.ShowDialog("BookPageDialog", args[0], 1);
			break;
		case "BookPageLeft":
			args[1] = (int)args[1] + 1;
			OnShown();
			break;
		case "BookPageRight":
			args[1] = (int)args[1] - 1;
			OnShown();
			break;
		case "Bookmark1":
		case "Bookmark2":
		case "Bookmark3":
		case "Bookmark4":
			args[1] = int.Parse(clickObject.name.Replace("Bookmark", ""));
			OnShown();
			break;
		case "AppShareButton":
		{
			clickObject.GetComponent<Button>().interactable = false;
			StartCoroutine(AppUtil.DelayAction(1f, delegate
			{
				clickObject.GetComponent<Button>().interactable = true;
			}));
			StringBuilder stringBuilder = new StringBuilder();
			string text2 = base.gameObject.name;
			if (text2 == "BookDialog")
			{
				string text3 = (string)args[0];
				if (GameObject.Find("BookPageDialog") != null)
				{
					string text4 = GameObject.Find("BookPageDialog").transform.Find("FukidasiText").GetComponent<VerticalText>().text;
					stringBuilder.Append("「").Append(text4.Replace("\n", "")).Append("」\n");
				}
				else
				{
					string value = LanguageManager.Get("[Book]Description" + text3).Split('\n')[0];
					stringBuilder.Append(value).Append("\n");
				}
				string value2 = LanguageManager.Get("[Book]Author" + text3);
				string value3 = LanguageManager.Get("[Book]Title" + text3);
				stringBuilder.Append(value2).Append("『").Append(value3)
					.Append("』");
			}
			stringBuilder.Append("\n").Append("自分探しタップゲーム『ALTER EGO』 caracolu.com/app/alterego/ #ALTEREGO");
			AppUtil.ShareLocalImage(stringBuilder.ToString(), null, "SNSシェア画像.jpg");
			return true;
		}
		case "OpenSaveDataButton":
			StartCoroutine(Save());
			break;
		case "OpenLoadDataButton":
			DialogManager.ShowDialog("LoadDataDialog");
			break;
		case "LoadDataButton":
			StartCoroutine(Load());
			break;
		case "IDText":
			AppUtil.SetClipBoard(Settings.SaveDataID);
			DialogManager.ShowDialog("CopyToast");
			break;
		case "GameStartButton":
			SceneTransition.LoadScene("探求");
			break;
		case "GetEgoButton":
		{
			string text = base.transform.Find("EgoBanner/EgoText").GetComponent<Text>().text;
			PlayerStatus.EgoPoint += new EgoPoint(text);
			Close();
			PlayerStatus.EnableDailyBonus = false;
			break;
		}
		case "DailyMovieButton":
			AdManager.Show("Reward", delegate(bool reward)
			{
				if (reward)
				{
					DialogManager.ShowDialog("GetDailyBonusDialog", args[0]);
				}
			});
			Close();
			break;
		}
		return true;
	}

	public void RewardForBonus(string type, bool reward, bool dialog)
	{
		if (!reward)
		{
			return;
		}
		PlayerResult.AdMovieCount++;
		switch (type)
		{
		case "Time":
			BonusTime.SetBonus();
			TimeManager.Reset(TimeManager.TYPE.NEXT_BONUS_BOOK, TimeSpan.FromMinutes(15.0));
			break;
		case "Temp":
		{
			EgoPoint egoPoint2 = new EgoPoint(base.transform.Find("EgoBanner/EgoText").GetComponent<Text>().text);
			TimeManager.Reset(TimeManager.TYPE.NEXT_BONUS_ES, TimeSpan.FromMinutes(15.0));
			if (dialog)
			{
				DialogManager.ShowDialog("GetEgoDialog", egoPoint2);
			}
			PlayerStatus.EgoPoint += egoPoint2;
			break;
		}
		case "DailyBonus":
		{
			EgoPoint egoPoint = new EgoPoint(base.transform.Find("EgoBanner/EgoText").GetComponent<Text>().text);
			PlayerStatus.EgoPoint += egoPoint;
			PlayerStatus.EnableDailyBonus = false;
			Debug.Log("RewardForBonus " + egoPoint.ToString());
			break;
		}
		}
	}

	private void CallbackFromPuchaser()
	{
		EventSystem.current.SetSelectedGameObject(null, null);
		if (base.gameObject.activeSelf)
		{
			OnShown();
		}
	}

	protected override void Render()
	{
		switch (base.gameObject.name)
		{
		case "MenuDialog":
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f));
			SlideDialog(true);
			break;
		case "CreditsDialog":
		case "ShopDialog":
		case "TutorialDialog":
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f));
			if (GameObject.Find("MenuDialog") != null)
			{
				GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, GetComponent<RectTransform>().anchoredPosition.y);
				StartCoroutine(ShowMenuPage(GameObject.Find("MenuDialog")));
			}
			else
			{
				SlideDialog(true);
			}
			break;
		case "EndingScreenID":
		case "EndingScreenSE":
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 3f));
			GetComponentInParent<DialogManager>().transform.Find("FilterGroup").GetComponentInChildren<Image>().enabled = false;
			break;
		case "CopyToast":
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f));
			AppUtil.DelayAction(this, 1f, delegate
			{
				Close();
			});
			break;
		case "RankupDialog":
		case "GetPrizeDialog":
		{
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f));
			AppUtil.SetAlpha(base.transform.Find("AdFrame"), 0f);
			base.transform.Find("CloseButton").gameObject.SetActive(false);
			if (AdInfo.ENABLE)
			{
				AppUtil.DelayAction(this, 1.25f, AppUtil.FadeIn(base.transform.Find("AdFrame"), 0.25f));
				AppUtil.DelayAction(this, 1.25f, delegate
				{
					AdManager.Show("Rectangle");
				});
				AppUtil.DelayAction(this, 2f, delegate
				{
					base.transform.Find("CloseButton").gameObject.SetActive(true);
					StartCoroutine(AppUtil.FadeIn(base.transform.Find("CloseButton"), 0.25f));
				});
				break;
			}
			base.transform.Find("AdFrame").gameObject.SetActive(false);
			string text = "RankupLabel";
			if (base.gameObject.name == "GetPrizeDialog")
			{
				text = "PrizeLabel";
			}
			base.transform.Find(text).GetComponent<RectTransform>().anchoredPosition = base.transform.Find(text + "PosNoAd").GetComponent<RectTransform>().anchoredPosition;
			base.transform.Find("CloseButton").GetComponent<RectTransform>().anchoredPosition = base.transform.Find("CloseButtonPosNoAd").GetComponent<RectTransform>().anchoredPosition;
			AppUtil.DelayAction(this, 1.5f, delegate
			{
				base.transform.Find("CloseButton").gameObject.SetActive(true);
				StartCoroutine(AppUtil.FadeIn(base.transform.Find("CloseButton"), 0.25f));
			});
			break;
		}
		case "GetDailyBonusDialog":
		{
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f));
			int intParameter = Utility.GetIntParameter("日替わり動画ボーナス");
			base.transform.Find("EgoBanner/EgoText").GetComponent<Text>().text = ((EgoPoint)args[0] * intParameter).ToString();
			RewardForBonus("DailyBonus", true, false);
			break;
		}
		default:
			StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f));
			break;
		}
		if (base.gameObject.name == "ShopDialog")
		{
			StartCoroutine(AppUtil.FadeIn(base.transform.Find("MainPage/Shop1/Shop1Button"), 0.25f));
			StartCoroutine(AppUtil.FadeIn(base.transform.Find("MainPage/Shop1/Shop2Button"), 0.25f));
			StartCoroutine(AppUtil.FadeIn(base.transform.Find("MainPage/Shop2/Shop1Button"), 0.25f));
			StartCoroutine(AppUtil.FadeIn(base.transform.Find("MainPage/Shop2/Shop2Button"), 0.25f));
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		CancelInvoke();
	}

	private void SlideDialog(bool show)
	{
		Transform transform = base.transform.Find("Background");
		string[] array = new string[3] { "MainPage", "MainPage/Shop1", "MainPage/Shop2" };
		foreach (string text in array)
		{
			if (transform != null)
			{
				break;
			}
			transform = base.transform.Find(text + "/Background");
		}
		float x = transform.GetComponent<RectTransform>().sizeDelta.x;
		float y = GetComponent<RectTransform>().anchoredPosition.y;
		AppUtil.SetAlpha(transform.parent, 1f);
		if (show)
		{
			StartCoroutine(AppUtil.MoveEasingFloat(x, 0f, delegate(float tmp)
			{
				GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp, y);
			}, true, 0.4f, EasingFunction.Ease.EaseOutQuint));
		}
		else
		{
			StartCoroutine(AppUtil.MoveEasingFloat(0f, x, delegate(float tmp)
			{
				GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp, y);
			}, true, 0.3f, EasingFunction.Ease.EaseOutQuint));
		}
	}

	private IEnumerator CloseMenuPage(GameObject menu)
	{
		menu.transform.SetAsLastSibling();
		AppUtil.SetAlpha(menu.transform.Find("InnerPage2"), 0f);
		yield return null;
		AppUtil.SetAlpha(menu.transform.Find("InnerPage2"), WAIT_PAGE_ALPHA);
		yield return AppUtil.WaitRealtime(WAIT_PAGE_TIME);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage2"), 0f);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage3"), WAIT_PAGE_ALPHA);
		yield return AppUtil.WaitRealtime(WAIT_PAGE_TIME);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage3"), 0f);
		menu.transform.Find("MainPage").gameObject.SetActive(true);
	}

	private IEnumerator ShowMenuPage(GameObject menu)
	{
		menu.transform.SetAsLastSibling();
		yield return null;
		menu.transform.Find("MainPage").gameObject.SetActive(false);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage3"), WAIT_PAGE_ALPHA);
		yield return AppUtil.WaitRealtime(WAIT_PAGE_TIME);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage3"), 0f);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage2"), 1f);
		yield return AppUtil.WaitRealtime(WAIT_PAGE_TIME);
		AppUtil.SetAlpha(menu.transform.Find("InnerPage2"), 0f);
	}

	public override void OnShown(params object[] args)
	{
		base.OnShown(args);
		Time.timeScale = Settings.GAME_SPEED;
		GameObject[] array = GameObject.FindGameObjectsWithTag("Beta");
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		switch (base.gameObject.name)
		{
		case "BookDialog":
		{
			bool is1st = (int)base.args[1] == 1;
			string text13 = (string)base.args[0];
			bool flag5 = text13 == "11";
			base.transform.Find("TitleText").gameObject.SetActive(!flag5);
			base.transform.Find("TitleText1").gameObject.SetActive(flag5);
			base.transform.Find("TitleText2").gameObject.SetActive(flag5);
			base.transform.Find("AuthorText").gameObject.SetActive(!flag5);
			base.transform.Find("AuthorText2").gameObject.SetActive(flag5);
			base.transform.Find("TitleText").GetComponent<Text>().text = LanguageManager.Get("[Book]Title" + text13);
			base.transform.Find("AuthorText").GetComponent<Text>().text = LanguageManager.Get("[Book]Author" + text13);
			if (flag5)
			{
				base.transform.Find("TitleText1").GetComponent<Text>().text = "フランケン";
				base.transform.Find("TitleText2").GetComponent<Text>().text = "シュタイン";
				base.transform.Find("AuthorText2").GetComponent<Text>().text = LanguageManager.Get("[Book]Author" + text13);
			}
			string text14 = LanguageManager.Get("[Book]Description" + text13);
			string text15 = text14.Split('\n')[0];
			base.transform.Find("DescriptionTextHead").GetComponent<Text>().text = text15;
			base.transform.Find("DescriptionText").GetComponent<Text>().text = text14.Replace(text15 + "\n\n", "");
			base.transform.Find("BookCover").GetComponent<Button>().enabled = !is1st;
			base.transform.Find("ButterflyParticle").gameObject.SetActive(false);
			if (is1st)
			{
				StartCoroutine(AppUtil.DelayAction(0.2f, delegate
				{
					base.transform.Find("ButterflyParticle").gameObject.SetActive(is1st);
				}));
			}
			base.transform.Find("CloseAllButton").gameObject.SetActive(!is1st);
			base.transform.Find("ReadButton").gameObject.SetActive(is1st);
			break;
		}
		case "BookPageDialog":
		{
			string text9 = (string)base.args[0];
			int num4 = (int)base.args[1];
			int rank = BookLevel.GetRank(text9);
			if (num4 <= 0 || num4 > rank || num4 >= 5)
			{
				Close();
				break;
			}
			string text10 = LanguageManager.Get("[Book]Title" + text9);
			base.transform.Find("TitleText").GetComponent<Text>().text = text10;
			int num5 = 100;
			if (text10.Length > 8)
			{
				num5 = 40;
			}
			base.transform.Find("TitleText").GetComponent<LetterSpacing>().spacing = num5;
			for (int j = 1; j < 5; j++)
			{
				base.transform.Find("Bookmark" + j + "/Text").GetComponent<Text>().text = LanguageManager.Get("[Book]Page" + text9 + "Rank" + j);
				base.transform.Find("Bookmark" + j + "/Active/Text").GetComponent<Text>().text = LanguageManager.Get("[Book]Page" + text9 + "Rank" + j);
				bool flag2 = num4 == j;
				bool flag3 = rank >= j;
				base.transform.Find("Bookmark" + j).GetComponent<Image>().enabled = !flag2;
				base.transform.Find("Bookmark" + j + "/Active").gameObject.SetActive(flag2);
				base.transform.Find("Bookmark" + j + "/Text").GetComponent<Text>().color = (flag3 ? Color.black : new Color(0.75f, 0.75f, 0.75f));
				base.transform.Find("Bookmark" + j).GetComponent<Button>().interactable = flag3;
			}
			string text11 = LanguageManager.Get("[Book]OriginL" + text9);
			text11 = text11.Replace(",", "").Replace(".", "");
			if (text11.Length >= 25)
			{
				text11 = text11.Insert(text11.IndexOf("『"), "\n");
				base.transform.Find("OriginText").GetComponent<Text>().alignment = TextAnchor.UpperCenter;
			}
			else
			{
				base.transform.Find("OriginText").GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
			}
			base.transform.Find("OriginText").GetComponent<VerticalText>().SetText(text11);
			base.transform.Find("FukidasiText").GetComponent<VerticalText>().SetText(LanguageManager.Get("[Fukidasi]Book" + text9 + "Rank" + num4));
			base.transform.Find("QuoteText").GetComponent<VerticalText>().SetText(LanguageManager.Get("[Book]QuoteL" + text9 + "Rank" + num4));
			base.transform.Find("PageText").GetComponent<Text>().text = LanguageManager.Get("[Book]Page" + text9 + "Rank" + num4);
			break;
		}
		case "MenuDialog":
		{
			base.transform.Find("MainPage/SwitchBGM").GetComponent<Toggle>().isOn = Settings.BGM;
			base.transform.Find("MainPage/SwitchSE").GetComponent<Toggle>().isOn = Settings.SE;
			Text component3 = base.transform.Find("MainPage/ReferenceButton/TitleText").GetComponent<Text>();
			component3.text = "<size=" + (float)component3.fontSize * 1.2f + ">" + component3.text.Substring(0, 1) + "</size>" + component3.text.Substring(1, component3.text.Length - 1);
			break;
		}
		case "ComebackDialog":
		case "GetEgoDialog":
			if (base.args.Length != 0)
			{
				base.transform.Find("EgoBanner/Text").GetComponent<Text>().text = base.args[0].ToString();
			}
			break;
		case "DailyBonusDialog":
		{
			int intParameter = Utility.GetIntParameter("日替わり動画ボーナス");
			base.transform.Find("DailyMovieButton/Text2").GetComponent<TextLocalization>().SetParameters(intParameter);
			base.transform.Find("EgoBanner/EgoText").GetComponent<Text>().text = base.args[0].ToString();
			break;
		}
		case "ChapterEndDialog":
		case "RestartDialogSE":
		case "RestartDialogID":
			base.transform.Find("Icon1").gameObject.SetActive(true);
			base.transform.Find("Icon2").gameObject.SetActive(false);
			if (base.gameObject.name == "RestartDialogID")
			{
				InvokeRepeating("ArrangeText", 0f, 0.84f);
			}
			break;
		case "GetPrizeDialog":
		{
			string text5 = (string)base.args[0];
			base.transform.Find("PrizeLabel/PrizeGroup1").gameObject.SetActive(false);
			base.transform.Find("PrizeLabel/PrizeGroup2").gameObject.SetActive(false);
			base.transform.Find("PrizeLabel/PrizeGroup3").gameObject.SetActive(false);
			string text6 = "";
			string text7 = "PrizeLabel/";
			EgoPoint egoPoint = null;
			float num = 1f;
			if (text5.Contains("分のEGO"))
			{
				string pointInString = text5.Replace("1時間分のEGO", "").Replace("3時間分のEGO", "").Replace("<br>を獲得", "");
				PlayerStatus.EgoPoint += new EgoPoint(pointInString);
				text7 += "PrizeGroup1";
			}
			else if (text5.Contains("本の獲得EGO"))
			{
				text6 = LanguageManager.Get("[UI]RankupLabel/EgoText");
				text7 += "PrizeGroup2";
				egoPoint = PlayerStatus.EgoPerSecond;
				num = float.Parse(text5.Replace("本の獲得EGO", "").Replace("倍", ""));
			}
			else
			{
				text6 = "{0}";
				text7 += "PrizeGroup3";
				egoPoint = Data.TapPower(false);
				num = float.Parse(text5.Replace("タップ獲得EGO", "").Replace("倍", ""));
			}
			base.transform.Find(text7).gameObject.SetActive(true);
			Image component = base.transform.Find(text7 + "/Icon").GetComponent<Image>();
			component.sprite = (Sprite)base.args[2];
			component.SetNativeSize();
			base.transform.Find(text7 + "/PrizeText").GetComponent<TextLocalization>().SetText((string)base.args[1]);
			if (text6 != "")
			{
				EgoPoint arg = egoPoint * num;
				base.transform.Find(text7 + "/ValueLayout/EgoTextBefore").GetComponent<Text>().text = string.Format(text6, egoPoint);
				base.transform.Find(text7 + "/ValueLayout/EgoTextAfter").GetComponent<Text>().text = string.Format(text6, arg);
			}
			if (!text7.Contains("Group2"))
			{
				break;
			}
			float num2 = 0f;
			RectTransform[] componentsInChildren = base.transform.Find(text7 + "/ValueLayout").GetComponentsInChildren<RectTransform>(true);
			foreach (RectTransform rectTransform in componentsInChildren)
			{
				if (!(rectTransform.name == "ValueLayout"))
				{
					rectTransform.anchoredPosition = new Vector2(num2, rectTransform.anchoredPosition.y);
					float preferredWidth = rectTransform.GetComponent<Text>().preferredWidth;
					rectTransform.sizeDelta = new Vector2(preferredWidth, rectTransform.sizeDelta.y);
					num2 += preferredWidth + 5f;
				}
			}
			num2 -= 5f;
			RectTransform componentInChildren = base.transform.Find(text7 + "/ValueLayout").GetComponentInChildren<RectTransform>();
			componentInChildren.anchoredPosition = new Vector2((componentInChildren.parent.GetComponent<RectTransform>().rect.width - num2) / 2f, -200f);
			break;
		}
		case "RankupDialog":
		{
			string text12 = (string)args[0];
			int rank2 = BookLevel.GetRank(text12);
			bool flag4 = rank2 == 5;
			string format = LanguageManager.Get("[UI]RankupLabel/TitleText" + (flag4 ? "Max" : ""));
			base.transform.Find("RankupLabel/TitleText").GetComponent<TextLocalization>().SetText(string.Format(format, LanguageManager.Get("[Book]Title" + text12)));
			int num6 = BookLevel.Get(text12);
			format = LanguageManager.Get("[UI]RankupLabel/EgoText");
			base.transform.Find("RankupLabel/ValueLayout/EgoTextBefore").GetComponent<Text>().text = string.Format(format, BookEgoPoint.GetBookEgo(text12, "per_second", num6 - 1).ToString());
			base.transform.Find("RankupLabel/ValueLayout/EgoTextAfter").GetComponent<Text>().text = string.Format(format, BookEgoPoint.GetBookEgo(text12, "per_second", num6).ToString());
			float num7 = 0f;
			RectTransform[] componentsInChildren = base.transform.Find("RankupLabel/ValueLayout").GetComponentsInChildren<RectTransform>(true);
			foreach (RectTransform rectTransform2 in componentsInChildren)
			{
				if (!(rectTransform2.name == "ValueLayout"))
				{
					rectTransform2.anchoredPosition = new Vector2(num7, rectTransform2.anchoredPosition.y);
					float preferredWidth2 = rectTransform2.GetComponent<Text>().preferredWidth;
					rectTransform2.sizeDelta = new Vector2(preferredWidth2, rectTransform2.sizeDelta.y);
					num7 += preferredWidth2 + 5f;
				}
			}
			num7 -= 5f;
			RectTransform componentInChildren2 = base.transform.Find("RankupLabel/ValueLayout").GetComponentInChildren<RectTransform>();
			componentInChildren2.anchoredPosition = new Vector2((componentInChildren2.parent.GetComponent<RectTransform>().rect.width - num7) / 2f, 0f);
			base.transform.Find("RankupLabel/QuoteText").GetComponent<HyphenationJpn>().SetText(LanguageManager.Get("[Book]QuoteS" + text12 + "Rank" + rank2));
			format = LanguageManager.Get("[UI]RankupLabel/OriginText");
			base.transform.Find("RankupLabel/OriginText").GetComponent<Text>().text = string.Format(format, LanguageManager.Get("[Book]Author" + text12), LanguageManager.Get("[Book]Title" + text12));
			RectTransform component2 = base.transform.Find("RankupLabel/OriginText").GetComponent<RectTransform>();
			float b = -220f - base.transform.Find("RankupLabel/QuoteText").GetComponent<Text>().preferredHeight;
			component2.anchoredPosition = new Vector2(y: Mathf.Max(-340f, b), x: component2.anchoredPosition.x);
			break;
		}
		case "BonusMovieDialog":
		{
			if (base.transform.Find("EgoBanner/EgoText") != null)
			{
				base.transform.Find("EgoBanner/EgoText").GetComponent<Text>().text = (PlayerStatus.EgoPerSecond * 3f * 60f * 60f).ToString();
			}
			TextLocalization textLocalization = ((!(base.transform.Find("BookBonusButton/Text") != null)) ? base.transform.Find("EsBonusButton/Text").GetComponent<TextLocalization>() : base.transform.Find("BookBonusButton/Text").GetComponent<TextLocalization>());
			if (AdInfo.ENABLE)
			{
				textLocalization.transform.parent.GetComponent<Button>().interactable = AdManager.IsReady("Reward");
				if (AdManager.IsReady("Reward"))
				{
					textLocalization.SetKey("[UI]PlayButton/Text");
				}
				else
				{
					textLocalization.SetKey("[UI]NoAdMovieMessage");
				}
				break;
			}
			textLocalization.SetKey("[UI]GetButton/Text");
			string[] array2 = new string[5] { "Text2-1", "Text2-2", "Text3-1", "Text3-2", "EgoBanner/BonusText" };
			foreach (string text8 in array2)
			{
				if (base.transform.Find(text8) != null)
				{
					base.transform.Find(text8).GetComponent<TextLocalization>().SetKey("[UI]BonusMovieDialog/" + text8 + "NoAd");
				}
			}
			break;
		}
		case "TutorialDialog":
			base.transform.Find("MainPage/CloseButton").gameObject.SetActive(GameObject.Find("MenuDialog") == null);
			base.transform.Find("MainPage/BackButton").gameObject.SetActive(GameObject.Find("MenuDialog") != null);
			break;
		case "ShareDialog":
		{
			bool flag = Utility.EsExists();
			if (PlayerStatus.ScenarioNo.Contains("4章ID"))
			{
				flag = false;
			}
			base.transform.Find("Es").gameObject.SetActive(flag);
			base.transform.Find("Background").GetComponent<Image>().fillAmount = (flag ? 1f : 0.8f);
			Vector2 anchoredPosition = base.transform.Find("ShareButton").GetComponent<RectTransform>().anchoredPosition;
			anchoredPosition.y = -454f;
			if (flag)
			{
				anchoredPosition.y -= 200f;
				int num3 = UnityEngine.Random.Range(1, 11);
				base.transform.Find("Es/Fukidasi/Text").GetComponent<TextLocalization>().SetKey("[EsTalk]SNSシェア-" + num3);
			}
			base.transform.Find("ShareButton").GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
			break;
		}
		case "ShopDialog":
		{
			base.transform.Find("MainPage/RestoreButton").gameObject.SetActive(Application.platform == RuntimePlatform.IPhonePlayer);
			int num8 = (int)base.args[0];
			base.transform.Find("MainPage/Shop1").gameObject.SetActive(num8 == 1);
			base.transform.Find("MainPage/Shop2").gameObject.SetActive(num8 == 2);
			int num9 = ((num8 == 1) ? 1 : 4);
			for (int k = num9; k < num9 + 3; k++)
			{
				bool flag6 = !PurchasingItem.Get(PurchasingItem.ITEM_LIST[k - 1]);
				base.transform.Find("MainPage/Shop" + num8 + "/ShopItemButton" + k + "/DisableFilter").gameObject.SetActive(!flag6);
				base.transform.Find("MainPage/Shop" + num8 + "/ShopItemButton" + k).GetComponent<Button>().enabled = flag6;
			}
			base.transform.Find("MainPage/CloseButton").gameObject.SetActive(GameObject.Find("MenuDialog") == null);
			base.transform.Find("MainPage/BackButton").gameObject.SetActive(GameObject.Find("MenuDialog") != null);
			break;
		}
		case "EndingScreenID":
		case "EndingScreenSE":
			AppUtil.DelayAction(this, 5f, delegate
			{
				base.transform.Find("Mirror1").gameObject.SetActive(true);
				StartCoroutine(AppUtil.FadeIn(base.transform.Find("Mirror1")));
			});
			AppUtil.DelayAction(this, 8f, delegate
			{
				StartCoroutine(AppUtil.FadeOut(base.transform.Find("Mirror1")));
				base.transform.Find("Mirror2").gameObject.SetActive(true);
				StartCoroutine(AppUtil.FadeIn(base.transform.Find("Mirror2")));
				AudioManager.PlaySound("Click", "", "ShowResultButton");
			});
			AppUtil.DelayAction(this, 11f, delegate
			{
				base.transform.Find("EndingText").gameObject.SetActive(true);
			});
			AppUtil.DelayAction(this, 16f, delegate
			{
				SceneTransition.LoadScene("タイトル", Color.black, 3f);
			});
			break;
		case "RetryDialog":
			if (AdInfo.ENABLE)
			{
				if (AdManager.IsReady("Reward"))
				{
					base.transform.Find("RetryButtonFromDialog").GetComponent<Button>().interactable = true;
					base.transform.Find("RetryButtonFromDialog/Text").GetComponent<TextLocalization>().SetKey("[UI]RetryButtonFromResult/Text");
				}
				else
				{
					base.transform.Find("RetryButtonFromDialog").GetComponent<Button>().interactable = false;
					base.transform.Find("RetryButtonFromDialog/Text").GetComponent<TextLocalization>().SetKey("[UI]NoAdMovieMessage");
				}
			}
			else
			{
				base.transform.Find("AttentionText1").gameObject.SetActive(false);
				base.transform.Find("RetryButtonFromDialog/Text").GetComponent<TextLocalization>().SetKey("[UI]RetryButtonFromResult/TextNoAd");
			}
			break;
		case "MoveDataDialog":
			if (!PlayerPrefs.HasKey("ScenarioNo"))
			{
				base.transform.Find("OpenSaveDataButton").GetComponent<Button>().interactable = false;
				base.transform.Find("OpenSaveDataButton/Text").GetComponent<Text>().color = new Color(0.7f, 0.7f, 0.7f);
			}
			base.transform.Find("OpenSaveDataButton/Text").GetComponent<TextLocalization>().SetKey("[UI]OpenSaveDataButton/Text");
			break;
		case "SaveDataDialog":
		{
			string saveDataID = Settings.SaveDataID;
			string text3 = "";
			string text4 = "";
			if (!string.IsNullOrEmpty(saveDataID))
			{
				DateTime dateTime = SaveDataManager.SaveDataTime.ToLocalTime();
				CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("ja");
				text3 = dateTime.ToString(cultureInfo);
				text4 = dateTime.AddDays(30.0).ToString(cultureInfo);
			}
			base.transform.Find("SaveData/IDText").GetComponent<Text>().text = saveDataID;
			base.transform.Find("SaveData/DateText").GetComponent<TextLocalization>().SetParameters(text3, text4);
			break;
		}
		case "LoadDataDialog":
			base.transform.Find("LoadData/ErrorText").GetComponent<TextLocalization>().SetKey("[UI]LoadDataDialog/ErrorText");
			base.transform.Find("LoadData/InputField/Text").GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
			break;
		case "SkipCounselingDialog":
		{
			string text = (string)base.args[0];
			string text2 = LanguageManager.Get(Data.GetScenarioKey(text, "型", CounselingResult.Get(text)), false);
			base.transform.Find("ResultText").GetComponent<TextLocalization>().SetParameters(text2);
			break;
		}
		}
	}

	public override IEnumerator OnClosed()
	{
		AdManager.Hide("Rectangle");
		switch (base.gameObject.name)
		{
		case "MenuDialog":
			SlideDialog(false);
			break;
		case "ShopDialog":
		case "CreditsDialog":
		case "TutorialDialog":
			if (GameObject.Find("MenuDialog") != null)
			{
				yield return CloseMenuPage(GameObject.Find("MenuDialog"));
			}
			else
			{
				SlideDialog(false);
			}
			break;
		}
		if (base.gameObject.name == "ShopDialog")
		{
			StartCoroutine(AppUtil.FadeOut(base.transform.Find("MainPage/Shop1/Shop1Button"), 0.25f));
			StartCoroutine(AppUtil.FadeOut(base.transform.Find("MainPage/Shop1/Shop2Button"), 0.25f));
			StartCoroutine(AppUtil.FadeOut(base.transform.Find("MainPage/Shop2/Shop1Button"), 0.25f));
			StartCoroutine(AppUtil.FadeOut(base.transform.Find("MainPage/Shop2/Shop2Button"), 0.25f));
		}
		yield return _003C_003En__0();
	}

	public void OnValueChanged(string args)
	{
		Transform transform = base.transform.Find("LoadData/LoadDataButton");
		if (transform != null)
		{
			bool flag = args != "" && args.Length >= 1;
			transform.GetComponent<Button>().interactable = flag;
			Color white = Color.white;
			white.a = (flag ? 1f : 0.5f);
			transform.GetComponentInChildren<Text>().color = white;
		}
	}

	public void OnValueChanged(string[] args)
	{
		bool flag = bool.Parse(args[1]);
		string text = args[0];
		if (!(text == "SwitchBGM"))
		{
			if (text == "SwitchSE")
			{
				Settings.SE = flag;
			}
		}
		else
		{
			Settings.BGM = flag;
		}
	}

	private void ArrangeText()
	{
		base.transform.Find("FlavorText").localRotation = Quaternion.identity;
		GetComponentInChildren<CommentUI>().ArrangeText();
		base.transform.Find("FlavorText").Rotate(new Vector3(0f, 0f, 4.6f));
	}

	private IEnumerator Save()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		base.transform.Find("OpenSaveDataButton/Text").GetComponent<TextLocalization>().SetKey("[UI]MoveDataDialog/Processing");
		string errorMsg = "";
		for (int retryCount = 0; retryCount < 5; retryCount++)
		{
			yield return SaveDataManager.Save(delegate(string ret)
			{
				errorMsg = ret;
			});
			if (errorMsg == "SUCCESS")
			{
				break;
			}
		}
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		if (errorMsg == "SUCCESS")
		{
			DialogManager.ShowDialog("SaveDataDialog");
		}
		else
		{
			DialogManager.ShowDialog("MoveDataErrorDialog");
		}
	}

	private IEnumerator Load()
	{
		TextLocalization errorText = base.transform.Find("LoadData/ErrorText").GetComponent<TextLocalization>();
		errorText.SetKey("[UI]LoadDataDialog/ErrorText");
		string loadID = base.transform.Find("LoadData/InputField/Text").GetComponent<Text>().text;
		if (Settings.SaveDataID == loadID)
		{
			errorText.SetKey("[UI]LoadDataDialog/ErrorText2");
			yield break;
		}
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		base.transform.Find("LoadData/LoadDataButton/Text").GetComponent<TextLocalization>().SetKey("[UI]MoveDataDialog/Processing");
		bool ret = false;
		for (int retryCount = 0; retryCount < 5; retryCount++)
		{
			yield return SaveDataManager.Load(loadID, delegate(bool result)
			{
				ret = result;
			});
			if (ret)
			{
				break;
			}
		}
		if (ret)
		{
			DialogManager.CloseAllDialog();
			DialogManager.ShowDialog("MoveDataSuccessDialog");
		}
		else
		{
			base.transform.Find("LoadData/LoadDataButton/Text").GetComponent<TextLocalization>().SetKey("[UI]LoadDataDialog/ButtonText");
			errorText.SetKey("[UI]LoadDataDialog/ErrorText1");
		}
		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	[CompilerGenerated]
	[DebuggerHidden]
	private IEnumerator _003C_003En__0()
	{
		return base.OnClosed();
	}
}

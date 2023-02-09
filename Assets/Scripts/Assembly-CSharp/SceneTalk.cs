using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using App;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTalk : SceneBase
{
	public TextLocalization EsComment;

	public ResultCanvas ResultCanvas;

	private bool WaitingChoice;

	private string CounselingData;

	private int TalkCount;

	private IEnumerator PreComment;

	private System.Random rnd = new System.Random();

	private string ScenarioNo;

	private string ScenarioType;

	private List<string> WordListForDisplay;

	private TalkItem ClickedTalkItem;

	[SerializeField]
	private Sprite EsStandard;

	[SerializeField]
	private Sprite EsStandardBG;

	[SerializeField]
	private Sprite EsCrazy;

	[SerializeField]
	private Sprite EsCrazyBG;

	[SerializeField]
	private Sprite[] PaperSprite;

	protected override void Start()
	{
		base.Start();
		if (PlayerStatus.ScenarioNo == "3章AE-5")
		{
			PlayerStatus.Route = "AE";
		}
		bool num = Utility.EsExists();
		if (num)
		{
			StartCoroutine(SetEsStatus("待機"));
			SetButtonList();
		}
		else
		{
			StartCoroutine(SetEsStatus("不在"));
			if (Data.GetScenarioSpecific().Contains("退行"))
			{
				SetButtonList();
			}
		}
		EsComment.transform.parent.gameObject.SetActive(false);
		GameObject.Find("MessageScreen").GetComponent<Image>().raycastTarget = false;
		Color color;
		ColorUtility.TryParseHtmlString("#646464E4", out color);
		base.transform.Find("UICanvas/UI_Footer").GetComponent<Image>().color = color;
		base.transform.Find("UICanvas/UI_Footer/ScrollShade1").GetComponent<Image>().color = color;
		base.transform.Find("UICanvas/UI_Footer/ScrollShade2").GetComponent<Image>().color = color;
		if (num && PlayerStatus.ScenarioNo.Contains("4章ID"))
		{
			StartCoroutine(SetEsCrazy());
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (!(base.transform.Find("UICanvas/UI1/ButterflyBonus") != null) || base.transform.Find("UICanvas/UI1/ButterflyBonus").gameObject.activeSelf || !base.transform.Find("UICanvas/UI2/TalkItemGroup").gameObject.activeSelf || !AdManager.IsReady("Reward") || !TimeManager.IsOverTime(TimeManager.TYPE.NEXT_BONUS_ES) || PlayerStatus.EgoPerSecond.IsZero())
		{
			return;
		}
		base.transform.Find("UICanvas/UI1/ButterflyBonus").gameObject.SetActive(true);
		Animator butterfly = base.transform.Find("UICanvas/UI1/ButterflyBonus").GetComponent<Animator>();
		butterfly.enabled = false;
		StartCoroutine(AppUtil.DelayAction(1f, delegate
		{
			if (butterfly != null)
			{
				butterfly.enabled = true;
			}
		}));
	}

	public void ResetButtonList()
	{
		TalkItem[] componentsInChildren = base.transform.Find("UICanvas/UI2/TalkItemGroup/Viewport/Content").GetComponentsInChildren<TalkItem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
		SetButtonList();
	}

	private void SetButtonList()
	{
		if (PlayerStatus.ScenarioNo == "3章AE-6" && Data.GetRoute(PlayerStatus.ScenarioNo) != PlayerStatus.Route)
		{
			PlayerStatus.ScenarioNo = "3章" + PlayerStatus.Route + "-6";
		}
		string text = PlayerStatus.ScenarioNo.Split('-')[0];
		if (text.Contains("3章"))
		{
			text = "3章" + PlayerStatus.Route3;
		}
		string start = text + "-1";
		string text2 = "";
		foreach (string item in Data.SCENARIO_DATA_KEY)
		{
			if (item.Contains(text))
			{
				text2 = item;
			}
			else if (text2 != "")
			{
				break;
			}
		}
		SetButtonList(start, text2);
	}

	private void SetButtonList(string start, string end)
	{
		Transform transform = base.transform.Find("UICanvas/UI2/TalkItemGroup/Viewport/Content");
		GameObject gameObject = base.transform.Find("UICanvas/UI2/TalkItemGroup/Viewport/Content/ScenarioItem").gameObject;
		gameObject.SetActive(true);
		int num = 1;
		int num2 = 0;
		string value = "";
		bool flag = false;
		float num3 = 0f;
		for (int i = 1; i <= 2; i++)
		{
			if (i == 2)
			{
				if (!PlayerStatus.ScenarioNo.Contains("4章AE"))
				{
					break;
				}
				start = "1章-1";
				end = "3章AE-6";
				flag = true;
				num3 = 1570f;
				transform.Find("RememberScroll").gameObject.SetActive(true);
			}
			int scenarioIndex = Data.GetScenarioIndex(start);
			int scenarioIndex2 = Data.GetScenarioIndex(end);
			for (int j = scenarioIndex; j <= scenarioIndex2; j++)
			{
				string text = Data.SCENARIO_DATA_KEY[j];
				string[] scenarioItemData = Data.GetScenarioItemData(text);
				if (scenarioItemData == null)
				{
					break;
				}
				if (!(scenarioItemData[0] == ""))
				{
					num2 = ((!text.Contains(value)) ? 1 : (num2 + 1));
					value = text.Split('-')[0];
					GameObject obj = UnityEngine.Object.Instantiate(gameObject, transform);
					obj.GetComponent<TalkItem>().SetItem(text, num2, flag);
					obj.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f - ((float)(232 * (num - 1) + 10 + 20) + num3), 0f);
					num++;
				}
			}
		}
		if (flag)
		{
			GameObject obj2 = UnityEngine.Object.Instantiate(gameObject, transform);
			obj2.GetComponent<TalkItem>().SetItem("エンドロール", 0, flag);
			obj2.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 0f - ((float)(232 * (num - 1) + 10 + 20) + num3), 0f);
			num++;
		}
		gameObject.SetActive(false);
		transform.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, (float)(232 * (num - 1) + 120 + 20) + num3 + 160f);
		if (!flag)
		{
			SetFilter(false);
		}
		else
		{
			base.transform.Find("UICanvas/UI2/TalkItemGroup/Viewport/Content/ScenarioFilter").gameObject.SetActive(false);
		}
	}

	private void SetFilter(bool move)
	{
		Transform filter = base.transform.Find("UICanvas/UI2/TalkItemGroup/Viewport/Content/ScenarioFilter");
		filter.gameObject.SetActive(true);
		filter.SetAsLastSibling();
		int num = 0;
		TalkItem[] componentsInChildren = GetComponentsInChildren<TalkItem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].IsActive())
			{
				num++;
			}
		}
		float num2 = -num * 232 - 20;
		if (num == 0)
		{
			num2 = 0f;
		}
		if (move)
		{
			float y = filter.GetComponent<RectTransform>().anchoredPosition.y;
			StartCoroutine(AppUtil.MoveEasingFloat(y, num2, delegate(float tmp)
			{
				filter.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, tmp);
			}, false, 3f));
		}
		else
		{
			filter.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, num2);
		}
	}

	public new void OnClick(GameObject clickObject)
	{
		if (base.OnClick(clickObject))
		{
			return;
		}
		Debug.Log("SceneTalk:OnClick " + clickObject.name);
		if (PreComment != null)
		{
			StopCoroutine(PreComment);
			PreComment = null;
		}
		TalkItem componentInParent = clickObject.GetComponentInParent<TalkItem>();
		if (componentInParent != null)
		{
			ClickedTalkItem = componentInParent;
		}
		ChoiceGroup componentInParent2 = clickObject.GetComponentInParent<ChoiceGroup>();
		if (componentInParent2 != null)
		{
			componentInParent2.SelectItem(clickObject);
			EsComment.transform.parent.gameObject.SetActive(false);
			WaitingChoice = false;
			string text = clickObject.name.Replace("Button", "").Replace("Choice", "");
			switch (text)
			{
			case "Law":
				text = "L";
				break;
			case "Chaos":
				text = "C";
				break;
			case "Neutral":
				text = "N";
				break;
			case "NA":
				text = "N/A";
				break;
			}
			CounselingData = CounselingData + text + ",";
			return;
		}
		switch (clickObject.name)
		{
		case "ScenarioButton":
			ScenarioNo = clickObject.GetComponentInParent<TalkItem>().ScenarioNo;
			ScenarioType = Data.GetScenarioType(ScenarioNo);
			if (Data.GetScenarioSpecific(ScenarioNo).Contains("章END"))
			{
				DialogManager.ShowDialog("ChapterEndDialog");
			}
			else
			{
				StartCoroutine(StartCounseling());
			}
			return;
		case "RetryNoAdButton":
			ScenarioNo = clickObject.GetComponentInParent<TalkItem>().ScenarioNo;
			ScenarioType = Data.GetScenarioType(ScenarioNo);
			if (ScenarioNo == "エンドロール")
			{
				SceneTransition.Transit(delegate
				{
					SceneManager.LoadScene("Ending", LoadSceneMode.Additive);
				});
			}
			else
			{
				StartCoroutine(StartCounseling());
			}
			break;
		case "StartChapterEndButton":
			StartCoroutine(StartCounseling());
			return;
		case "RestartButton":
			DialogManager.ShowDialog("RestartDialog" + PlayerStatus.Route);
			break;
		case "RestartButtonAE":
			ScenarioNo = clickObject.GetComponentInParent<TalkItem>().ScenarioNo;
			ScenarioType = Data.GetScenarioType(ScenarioNo);
			StartCoroutine(StartCounseling());
			break;
		case "雑談Button":
		case "日替わりButton":
		case "本を借りるButton":
		case "自我とエス(SE)Button":
		case "自我とエス(ID)Button":
		case "自我とエス(AE)Button":
		{
			string text4 = clickObject.name.Replace("Button", "");
			int count = int.Parse(Data.DIC[text4 + "COUNT"][0]);
			int[] array = AppUtil.RandomArray(1, count);
			int num2 = array[0];
			if (text4 == "本を借りる")
			{
				if (PlayerStatus.ReadingBookList.Length == 0)
				{
					num2 = 1;
				}
			}
			else if (num2 == PlayerPrefs.GetInt("Last" + text4 + "Index"))
			{
				num2 = array[1];
			}
			PlayerPrefs.SetInt("Last" + text4 + "Index", num2);
			ScenarioNo = text4;
			ScenarioType = text4 + num2;
			StartCoroutine(StartCounseling());
			if (text4 == "日替わり")
			{
				clickObject.transform.parent.GetComponent<TalkItem>().SetButtonActive(false);
			}
			break;
		}
		case "IchEsButtonUnlocked":
			DialogManager.ShowDialog("ShopDialog", 2);
			break;
		case "本を返すButton":
			ScenarioNo = "本を返す";
			ScenarioType = "本を返す共通";
			StartCoroutine(StartCounseling());
			break;
		case "ScenarioRetryButton":
			ScenarioNo = clickObject.GetComponentInParent<TalkItem>().ScenarioNo;
			ScenarioType = Data.GetScenarioType(ScenarioNo);
			DialogManager.ShowDialog("RetryDialog");
			return;
		case "RetryButtonFromResult":
			AdManager.Show("Reward", delegate(bool ret)
			{
				RetryWithReward(ret);
			});
			return;
		case "RetryButtonFromDialog":
			AdManager.Show("Reward", delegate(bool ret)
			{
				RetryWithReward(ret);
			});
			return;
		case "ApproveButton":
			StartCoroutine(Closing());
			return;
		case "MessageScreen":
			GameObject.Find("MessageScreen").GetComponent<Image>().raycastTarget = false;
			return;
		case "InputButton":
		{
			InputField component = base.transform.Find("UICanvas/UI2/InputGroup/ThinkingArea").GetComponent<InputField>();
			CounselingData = component.text;
			PlayerResult.CatharsisLength = CounselingData.Length;
			WaitingChoice = false;
			return;
		}
		case "TalkArea":
		{
			ScenarioNo = null;
			ScenarioType = null;
			TalkCount++;
			if (!PlayerStatus.ScenarioNo.Contains("4章ID"))
			{
				PlayerResult.EsTapCount++;
			}
			int scenarioIndex = Data.GetScenarioIndex(PlayerStatus.ScenarioNo);
			int scenarioIndex2 = Data.GetScenarioIndex("1章-2");
			string key = "[EsTalk]1章-通常-9";
			string text2 = "通常";
			if (scenarioIndex >= scenarioIndex2)
			{
				if (TalkCount > 10 && !PlayerStatus.ScenarioNo.Contains("4章ID"))
				{
					text2 = "怒り";
				}
				int num = ((text2 == "怒り") ? 10 : 20);
				string text3 = PlayerStatus.ScenarioNo.Split('-')[0];
				key = "[EsTalk]" + text3 + "-" + text2 + "-" + UnityEngine.Random.Range(1, num + 1);
			}
			if (PreComment != null)
			{
				StopCoroutine(PreComment);
			}
			PreComment = SetComment(text2, key);
			StartCoroutine(PreComment);
			return;
		}
		case "CounselingButton":
			WaitingChoice = true;
			return;
		case "SkipButton":
			WaitingChoice = false;
			return;
		}
		string[] array2 = Data.GET_COUNSELING_TYPE(ScenarioType);
		if (array2 == null || Array.IndexOf(array2, clickObject.name) < 0)
		{
			return;
		}
		CounselingData = CounselingData + clickObject.name + ",";
		WordListForDisplay.Add(clickObject.GetComponentInChildren<Text>().text);
		WaitingChoice = false;
		Button[] componentsInChildren = base.transform.Find("UICanvas/WordGroup/WordItemGroup").GetComponentsInChildren<Button>();
		foreach (Button button in componentsInChildren)
		{
			button.enabled = false;
			if (button.gameObject == clickObject)
			{
				button.transform.Rotate(new Vector3(0f, 0f, -6f));
				continue;
			}
			button.GetComponent<Image>().sprite = PaperSprite[PaperSprite.Length - 1];
			button.GetComponent<Image>().SetNativeSize();
			button.GetComponentInChildren<Text>().enabled = false;
			button.transform.Rotate(new Vector3(0f, 0f, 360f * UnityEngine.Random.Range(0f, 1f)));
		}
	}

	private void RetryWithReward(bool reward)
	{
		if (reward)
		{
			StartCoroutine(StartCounseling());
			PlayerResult.AdMovieCount++;
		}
	}

	private IEnumerator SetComment(string state, string key)
	{
		GameObject commentFrame = EsComment.transform.parent.gameObject;
		if (commentFrame.activeSelf)
		{
			commentFrame.SetActive(false);
			yield return AppUtil.Wait(0.2f);
		}
		if (state == "怒り")
		{
			yield return SetEsStatus("対話");
			base.transform.Find("UICanvas/UI1/TalkArea").gameObject.SetActive(true);
		}
		EsComment.SetKey(key);
		commentFrame.SetActive(true);
		yield return AppUtil.Wait(2f);
		commentFrame.SetActive(false);
		if (state == "怒り")
		{
			yield return SetEsStatus("待機");
		}
		PreComment = null;
	}

	private IEnumerator SetEsCrazy()
	{
		base.transform.Find("BGCanvas/Es正面/EsMain").GetComponent<Image>().sprite = EsCrazy;
		base.transform.Find("BGCanvas/Es正面/Background").GetComponent<Image>().sprite = EsCrazyBG;
		SetEsFace("Main");
		GameObject talkArea = base.transform.Find("UICanvas/UI1/TalkArea").gameObject;
		if (PlayerStatus.ScenarioNo.Contains("4章ID"))
		{
			while (true)
			{
				yield return AppUtil.Wait(UnityEngine.Random.Range(0.5f, 2.5f));
				OnClick(talkArea);
			}
		}
	}

	private void SetEsFace(string target)
	{
		target = target.Replace("<", "").Replace(">", "");
		if (target == "ほほえみ")
		{
			target = "Smile";
		}
		Animator component = base.transform.Find("BGCanvas/Es正面").GetComponent<Animator>();
		component.ResetTrigger("Wait");
		component.SetTrigger(target);
	}

	private IEnumerator SetEsStatus(string state, Action changeScene = null)
	{
		Debug.Log("SetEsStatus " + state);
		base.transform.Find("BGCanvas/Es正面/EsMain").GetComponent<Image>().sprite = EsStandard;
		base.transform.Find("BGCanvas/Es正面/Background").GetComponent<Image>().sprite = EsStandardBG;
		base.transform.Find("UICanvas/UI1/TalkArea").gameObject.SetActive(false);
		string direction = "正面";
		if (state.Contains("対話") || state.Contains("診断1") || state.Contains("雑談") || state.Contains("日替わり") || state.Contains("本を") || state.Contains("自我とエス"))
		{
			state = "対話";
		}
		if (state.Contains("診断2"))
		{
			direction = "横向き";
			state = "横向き";
			base.transform.Find("BGCanvas/Es横向き/Background/MoveEye").gameObject.SetActive(false);
		}
		else if (state.Contains("診断3") || state.Contains("診断4"))
		{
			direction = "アオリ";
			state = "アオリ";
		}
		if (!base.transform.Find("BGCanvas/Es" + direction).gameObject.activeSelf)
		{
			SceneTransition.Transit(delegate
			{
				string[] array = new string[3] { "正面", "横向き", "アオリ" };
				foreach (string text in array)
				{
					base.transform.Find("BGCanvas/Es" + text).gameObject.SetActive(text == direction);
				}
				if (changeScene != null)
				{
					changeScene();
				}
			});
			yield return AppUtil.WaitRealtime(0.75f);
		}
		else if (changeScene != null)
		{
			changeScene();
		}
		EsComment = base.transform.Find("BGCanvas/Es" + direction).GetComponentInChildren<TextLocalization>(true);
		EsComment.transform.parent.gameObject.SetActive(false);
		switch (state)
		{
		case "不在":
			base.transform.Find("BGCanvas/Es正面/EsMain").gameObject.SetActive(false);
			break;
		case "退室":
			StartCoroutine(AppUtil.FadeOut(base.transform.Find("BGCanvas/Es正面/EsMain")));
			break;
		case "待機":
			base.transform.Find("BGCanvas/Es正面/EsMain").gameObject.SetActive(true);
			base.transform.Find("UICanvas/UI1/TalkArea").gameObject.SetActive(true);
			SetEsFace("Wait");
			break;
		case "対話":
		{
			base.transform.Find("BGCanvas/Es正面/EsMain").gameObject.SetActive(true);
			string esFace = "Main";
			if (PlayerStatus.ScenarioNo.Contains("4章AE"))
			{
				if (Data.GetScenarioSpecific(ScenarioNo).Contains("退行"))
				{
					esFace = "Smile";
				}
				else
				{
					string scenarioKey = Data.GetScenarioKey(ScenarioType, "立ち絵");
					if (scenarioKey != null)
					{
						esFace = LanguageManager.Get(scenarioKey);
					}
					else
					{
						switch (ScenarioNo)
						{
						case "雑談":
						case "本を借りる":
						case "本を返す":
							esFace = "Smile";
							break;
						case "自我とエス(ID)":
							esFace = "ID";
							break;
						case "自我とエス(SE)":
							esFace = "SE";
							break;
						case "自我とエス(AE)":
							esFace = "ほほえみ";
							break;
						}
					}
				}
			}
			SetEsFace(esFace);
			break;
		}
		}
	}

	private IEnumerator Closing()
	{
		ResultCanvas.SetActive(false);
		if (Data.GetScenarioKey(ScenarioType, "結び", null, 1) != null)
		{
			yield return EsTalk("結び");
		}
		string scenarioNo = ScenarioNo;
		if (!(scenarioNo == "日替わり"))
		{
			if (scenarioNo == "本を借りる")
			{
				int intParameter = Utility.GetIntParameter("読書時間");
				TimeManager.Reset(TimeManager.TYPE.END_BOOK, TimeSpan.FromHours(intParameter) + TimeSpan.FromSeconds(0.8999999761581421));
				if (ClickedTalkItem.Price != null)
				{
					PlayerStatus.EgoPoint -= ClickedTalkItem.Price;
				}
				ClickedTalkItem.SetItem();
			}
		}
		else
		{
			int intParameter2 = Utility.GetIntParameter(ScenarioNo + CounselingData.Replace(",", ""));
			EgoPoint bonusEgo = PlayerStatus.EgoPerSecond * intParameter2 * 60f * 60f * Settings.GAME_SPEED;
			EsComment.transform.parent.gameObject.SetActive(false);
			yield return AppUtil.Wait(0.2f);
			if (AdInfo.ENABLE)
			{
				DialogManager.ShowDialog("DailyBonusDialog", bonusEgo);
			}
			else
			{
				DialogManager.ShowDialog("GetDailyBonusDialog", bonusEgo);
			}
			yield return AppUtil.Wait(0.2f);
		}
		string state = "待機";
		bool levelUp = ScenarioNo == PlayerStatus.ScenarioNo || ScenarioNo == "本を返す";
		if (levelUp)
		{
			if (Data.GetScenarioSpecific().Contains("エンディング"))
			{
				PlayerResult.Ending += PlayerStatus.Route;
				AdManager.Hide("Banner");
				if (PlayerStatus.Route == "AE")
				{
					SceneTransition.LoadScene("Ending", Color.black, 3f);
					PlayerStatus.ReadingBook = 0;
					PlayerStatus.ReadingBookList = "";
					string value = ((PlayerResult.EsTapCount >= 500) ? "エス崇拝者" : ((PlayerResult.TapCount >= 10000) ? "自問の達人" : ((!PlayerResult.Ending.StartsWith("SE")) ? "解放者" : "管理者")));
					CounselingResult.Set("ゲームクリア", value);
				}
				else
				{
					DialogManager.ShowDialog("EndingScreen" + PlayerStatus.Route);
				}
				PlayerStatus.EgoPoint -= ClickedTalkItem.Price;
				Data.GoToNextScenario();
				yield break;
			}
			if (Data.GetNextScenarioSpecific().Contains("エス不在"))
			{
				state = "退室";
			}
		}
		StartCoroutine(SetEsStatus(state, delegate
		{
			EsComment.transform.parent.gameObject.SetActive(false);
			base.transform.Find("UICanvas/UI2/TalkItemGroup").gameObject.SetActive(true);
			base.transform.Find("UICanvas/UI_Footer").gameObject.SetActive(true);
			base.transform.Find("UICanvas/UI2/ChoiceGroup").gameObject.SetActive(false);
			base.transform.Find("UICanvas/UI2/EhonGroup").gameObject.SetActive(false);
			base.transform.Find("BGCanvas/Es横向き/Background/MoveEye").gameObject.SetActive(false);
			if (levelUp)
			{
				ScenarioLevelUp();
			}
			else if (ScenarioNo == "3章AE-5")
			{
				ResetButtonList();
			}
		}));
	}

	private void ScenarioLevelUp()
	{
		TalkItem clickedTalkItem = ClickedTalkItem;
		string type = clickedTalkItem.GetType();
		string format = LanguageManager.Get("[UI]RankUpEffect/EgoText");
		string before = ((!(type == "タップ")) ? string.Format(format, PlayerStatus.EgoPerSecond) : Data.TapPower(false).ToString());
		string scenarioSpecific = Data.GetScenarioSpecific(ScenarioNo);
		if (ScenarioNo == "本を返す")
		{
			bool readingBookIsOver = Data.ReadingBookIsOver;
			PlayerStatus.ReadingBookList += PlayerStatus.ReadingBook.ToString("X");
			PlayerStatus.ReadingBook = 0;
			if (readingBookIsOver)
			{
				ClickedTalkItem.SetItem();
				return;
			}
		}
		else
		{
			PlayerStatus.EgoPoint -= ClickedTalkItem.Price;
			if (scenarioSpecific.Contains("ルート分岐"))
			{
				if (ScenarioNo.Contains("2章"))
				{
					PlayerStatus.Route = PlayerStatus.RouteTrend;
					PlayerStatus.ScenarioNo = "3章" + PlayerStatus.Route + "-1";
					PlayerStatus.Route3 = PlayerStatus.Route;
				}
				else
				{
					PlayerStatus.ScenarioNo = "3章" + PlayerStatus.Route + "-6";
				}
			}
			else
			{
				Data.GoToNextScenario();
			}
		}
		string after = ((!(type == "タップ")) ? string.Format(format, PlayerStatus.EgoPerSecond) : Data.TapPower(false).ToString());
		clickedTalkItem.OnLevelUp(before, after);
		float waitTime = 0f;
		if (Data.GetScenarioSpecific().Contains("エス不在"))
		{
			waitTime = 0.5f;
		}
		TalkItem[] componentsInChildren = GetComponentsInChildren<TalkItem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetItem();
		}
		StartCoroutine(AppUtil.DelayAction(waitTime, delegate
		{
			SetFilter(ScenarioNo != "本を返す");
		}));
		if (scenarioSpecific.Contains("エンディング"))
		{
			base.transform.Find("UICanvas/UI2/TalkItemGroup").gameObject.SetActive(false);
			base.transform.Find("UICanvas/UI_Footer").gameObject.SetActive(false);
		}
	}

	private IEnumerator EsTalk(string keyHeader, bool wait = true)
	{
		GameObject commentFrame = EsComment.transform.parent.gameObject;
		Image touch = GameObject.Find("MessageScreen").GetComponent<Image>();
		if (wait)
		{
			commentFrame.SetActive(false);
			yield return AppUtil.Wait(1f);
		}
		for (int i = 1; i <= 100; i++)
		{
			string key = Data.GetScenarioKey(ScenarioType, keyHeader, null, i);
			if (key != null)
			{
				commentFrame.SetActive(false);
				yield return AppUtil.Wait(0.2f);
				EsComment.SetKey(key);
				commentFrame.SetActive(true);
				touch.raycastTarget = true;
				while (touch.raycastTarget)
				{
					yield return null;
				}
				continue;
			}
			break;
		}
	}

	private IEnumerator StartCounseling()
	{
		HyperActive.Init();
		CounselingData = "";
		base.transform.Find("UICanvas/UI2/InputGroup/ThinkingArea").GetComponent<InputField>().text = "";
		base.transform.Find("UICanvas/UI2/TalkItemGroup").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI_Footer").gameObject.SetActive(false);
		if (base.transform.Find("UICanvas/UI1/ButterflyBonus") != null)
		{
			base.transform.Find("UICanvas/UI1/ButterflyBonus").gameObject.SetActive(false);
		}
		yield return SetEsStatus(ScenarioType);
		yield return EsTalk("導入");
		string scenarioSpecific = Data.GetScenarioSpecific(ScenarioNo);
		if (ScenarioType.Contains("診断"))
		{
			WaitingChoice = true;
			if (PlayerResult.EndingCount > 0 && !scenarioSpecific.Contains("ルート分岐"))
			{
				DialogManager.ShowDialog("SkipCounselingDialog", ScenarioType);
				while (DialogManager.IsShowing())
				{
					yield return null;
				}
			}
			if (WaitingChoice)
			{
				yield return StartCoroutine(StartCounselingType());
				yield break;
			}
		}
		else
		{
			yield return StartDialogue();
			if (ScenarioNo == "退行")
			{
				Data.Restart();
				yield break;
			}
			if (ScenarioType.Contains("雑談"))
			{
				PlayerResult.EsTalkCount++;
				if (PlayerResult.EsTalkCount >= 100)
				{
					Utility.PromptReview("雑談100回");
				}
			}
		}
		yield return Closing();
	}

	private IEnumerator StartCounselingType()
	{
		WordListForDisplay = new List<string>();
		TextLocalization preEsComment = EsComment;
		string closingKey = "結果導入";
		if (ScenarioType.Contains("診断1"))
		{
			yield return StartCounselingType1();
		}
		else if (ScenarioType.Contains("診断2"))
		{
			yield return StartCounselingType2();
		}
		else if (ScenarioType.Contains("診断3"))
		{
			yield return StartCounselingType3();
		}
		else if (ScenarioType.Contains("診断4"))
		{
			yield return StartCounselingType4();
			if (CounselingData.Length <= 10)
			{
				closingKey += "10";
			}
			else if (CounselingData.Length >= 100)
			{
				closingKey += "100";
			}
		}
		yield return EsTalk(closingKey);
		EsComment = preEsComment;
		EsComment.transform.parent.gameObject.SetActive(false);
		switch (ScenarioType)
		{
		case "診断2-3":
		case "診断2-4":
		case "診断2-5":
			yield return AppUtil.FadeOut(base.transform.Find("UICanvas/UI2/EhonGroup"), 1f);
			StartCoroutine(Closing());
			yield break;
		}
		string[] counselingType = Counseling.GetCounselingType(ScenarioType, CounselingData);
		string text = counselingType[0];
		CounselingResult.Set(ScenarioType, text);
		if (ScenarioType.Contains("診断4"))
		{
			WordListForDisplay.AddRange(AppUtil.GetChildArray(counselingType, 1));
			string text2 = text;
			if (text.Contains("その他"))
			{
				text2 = "ノンジャンル";
			}
			string[] array = text2.Split('+');
			foreach (string type in array)
			{
				string scenarioKey = Data.GetScenarioKey(ScenarioType, "解析中表示単語", type);
				WordListForDisplay.AddRange(LanguageManager.Get(scenarioKey).Split('、'));
			}
		}
		ResultCanvas.RequestResult(ScenarioType, text, WordListForDisplay);
		AppUtil.SetFalse("UICanvas/UI2/EhonGroup");
		AppUtil.SetFalse("BGCanvas/Es横向き/Background/MoveEye");
	}

	private IEnumerator StartCounselingType1()
	{
		int max;
		for (max = 1; max <= 20; max++)
		{
			if (Data.GetScenarioKey(ScenarioType, "問", null, max) == null)
			{
				max--;
				break;
			}
		}
		ChoiceGroup choice = GetComponentInChildren<ChoiceGroup>(true);
		choice.gameObject.SetActive(true);
		SetProgressState(base.transform.Find("UICanvas/UI2/ChoiceGroup"), 0f, max);
		GameObject commentFrame = EsComment.transform.parent.gameObject;
		string[] buttonNameList = new string[3] { "YesButton", "NoButton", "NAButton" };
		for (int i = 1; i <= max; i++)
		{
			yield return choice.SetButton(null, null);
			string key = Data.GetScenarioKey(ScenarioType, "問", null, i);
			commentFrame.SetActive(false);
			yield return AppUtil.Wait(0.2f);
			EsComment.SetKey(key);
			commentFrame.SetActive(true);
			yield return choice.SetButton(buttonNameList, new string[3] { "[UI]YesButton/Text", "[UI]NoButton/Text", "[UI]NAButton/Text" });
			WaitingChoice = true;
			while (WaitingChoice)
			{
				yield return null;
			}
			string text = "";
			string[] array = buttonNameList;
			foreach (string text2 in array)
			{
				if (base.transform.Find("UICanvas/UI2/ChoiceGroup/" + text2).gameObject.activeSelf)
				{
					text = text2.Replace("Button", "");
				}
			}
			string id = key.Replace("問", "解析中表示単語") + "-" + text;
			WordListForDisplay.AddRange(LanguageManager.Get(id).Split('、'));
			SetProgressState(base.transform.Find("UICanvas/UI2/ChoiceGroup"), i, max);
			yield return AppUtil.Wait(0.5f);
		}
		choice.gameObject.SetActive(false);
	}

	private IEnumerator StartCounselingType2()
	{
		EsComment.transform.parent.gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup").gameObject.SetActive(true);
		base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Q0").gameObject.SetActive(true);
		base.transform.Find("UICanvas/UI2/EhonGroup/診断2-1").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/診断2-2").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/診断2-3").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/診断2-4").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/診断2-5").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Ending1").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Ending2").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Ending3").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType).gameObject.SetActive(true);
		base.transform.Find("UICanvas/UI2/EhonGroup/EsTalk").gameObject.SetActive(false);
		base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Background").gameObject.SetActive(true);
		base.transform.Find("UICanvas/UI2/EhonGroup/Background").GetComponent<Image>().color = Color.white;
		StartCoroutine(AppUtil.FadeIn(base.transform.Find("UICanvas/UI2/EhonGroup")));
		EsComment = base.transform.Find("UICanvas/UI2/EhonGroup/EsTalk").GetComponentInChildren<TextLocalization>(true);
		yield return EsTalk("診断導入");
		if (Data.GetScenarioSpecific(ScenarioNo).Contains("ルート分岐"))
		{
			GameObject Q = base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Q0").gameObject;
			StartCoroutine(AppUtil.FadeOut(Q, 0.5f, delegate
			{
				Q.SetActive(false);
			}));
			GameObject gameObject = base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Q1").gameObject;
			gameObject.SetActive(true);
			StartCoroutine(AppUtil.FadeIn(gameObject));
			yield return EsTalk("診断導入2", false);
			AppUtil.SetAlpha(Q, 1f);
		}
		int max;
		for (max = 1; max <= 20; max++)
		{
			if (Data.GetScenarioKey(ScenarioType, max + "問") == null)
			{
				max--;
				break;
			}
		}
		ChoiceGroup choice = GetComponentInChildren<ChoiceGroup>(true);
		choice.gameObject.SetActive(true);
		for (int i = 1; i <= max; i++)
		{
			yield return StartDialogueQuestion(i, max);
		}
		choice.gameObject.SetActive(false);
	}

	private IEnumerator StartCounselingType4()
	{
		while (true)
		{
			base.transform.Find("UICanvas/UI2/InputGroup").gameObject.SetActive(true);
			WaitingChoice = true;
			while (WaitingChoice)
			{
				yield return null;
			}
			base.transform.Find("UICanvas/UI2/InputGroup").gameObject.SetActive(false);
			base.transform.Find("BGCanvas/Es横向き/Background/MoveEye").gameObject.SetActive(true);
			if (CounselingData.Length <= 10)
			{
				string data = CounselingData;
				yield return EsTalk("10文字以内確認");
				ChoiceGroup choice = GetComponentInChildren<ChoiceGroup>(true);
				choice.gameObject.SetActive(true);
				yield return choice.SetButton(new string[2] { "YesButton", "NoButton" }, new string[2] { "[UI]YesButton/Text", "[UI]NoButton/Text" });
				WaitingChoice = true;
				while (WaitingChoice)
				{
					yield return null;
				}
				yield return AppUtil.Wait(0.5f);
				choice.gameObject.SetActive(false);
				CounselingData = data;
				if (!base.transform.Find("UICanvas/UI2/ChoiceGroup/YesButton").gameObject.activeSelf)
				{
					yield return EsTalk("やり直し");
					base.transform.Find("BGCanvas/Es横向き/Background/MoveEye").gameObject.SetActive(false);
					continue;
				}
				break;
			}
			break;
		}
	}

	public void OnInputChanged(string value)
	{
		bool flag = value.Length > 0;
		base.transform.Find("UICanvas/UI2/InputGroup/InputButton").GetComponent<Button>().interactable = flag;
		base.transform.Find("UICanvas/UI2/InputGroup/InputButton/InactiveFilter").gameObject.SetActive(!flag);
	}

	private IEnumerator StartCounselingType3()
	{
		int fukidasiMax = 20;
		GameObject group = base.transform.Find("UICanvas/WordGroup").gameObject;
		TextLocalization[] fukidasiList = group.transform.Find("WordItemGroup").GetComponentsInChildren<TextLocalization>();
		TextLocalization[] array = fukidasiList;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].transform.parent.gameObject.SetActive(false);
		}
		group.transform.Find("EsMessageFrame/EsMessage").GetComponent<TextLocalization>().SetText(LanguageManager.Get(Data.GetScenarioKey(ScenarioType, "診断中説明")));
		SetProgressState(base.transform.Find("UICanvas/WordGroup"), 0f, fukidasiMax);
		SceneTransition.Transit(delegate
		{
			group.SetActive(true);
		});
		while (!group.activeSelf)
		{
			yield return null;
		}
		yield return AppUtil.Wait(1.5f);
		List<List<string>> fukidasiKeyList = Counseling.GetFukidasiKeyList(ScenarioType);
		for (int i = 1; i <= fukidasiMax; i++)
		{
			WaitingChoice = true;
			List<string> fukidasiKey = Counseling.GetFukidasiKey(fukidasiKeyList);
			foreach (TextLocalization item in new List<TextLocalization>(fukidasiList).OrderBy((TextLocalization x) => rnd.Next()))
			{
				item.transform.parent.GetComponent<Image>().sprite = PaperSprite[rnd.Next(0, PaperSprite.Length - 2)];
				item.transform.parent.GetComponent<Image>().SetNativeSize();
				StartCoroutine(AppUtil.FadeIn(item, 0.3f));
				StartCoroutine(AppUtil.FadeIn(item.transform.parent, 0.3f));
				string[] array2 = AppUtil.Shift(fukidasiKey).Split(':');
				item.GetComponent<Text>().enabled = true;
				item.transform.parent.localRotation = Quaternion.identity;
				item.transform.parent.GetComponent<Button>().enabled = true;
				item.transform.parent.name = array2[0];
				item.SetText(array2[1]);
				item.transform.parent.gameObject.SetActive(true);
			}
			yield return AppUtil.Wait(0.3f);
			Debug.Log("AutoTestEvent:Screenshot");
			while (WaitingChoice)
			{
				yield return null;
			}
			SetProgressState(base.transform.Find("UICanvas/WordGroup"), i, fukidasiMax);
			yield return AppUtil.Wait(0.25f);
			array = fukidasiList;
			foreach (TextLocalization textLocalization in array)
			{
				StartCoroutine(AppUtil.FadeOut(textLocalization.transform.parent, 0.3f));
			}
			yield return AppUtil.Wait(0.1f);
			if (i == 10)
			{
				fukidasiKeyList = Counseling.CustomFukidasiKeyList(fukidasiKeyList, CounselingData);
				CounselingData = "";
			}
		}
		array = fukidasiList;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].transform.parent.gameObject.SetActive(true);
		}
		SceneTransition.Transit(delegate
		{
			group.SetActive(false);
		});
		while (!group.activeSelf)
		{
			yield return null;
		}
	}

	private void SetProgressState(Transform parent, float current, float max)
	{
		Transform progressSet = parent.Find("ProgressBarSet");
		progressSet.gameObject.SetActive(max > 1f);
		if (max <= 1f)
		{
			return;
		}
		progressSet.Find("Text").GetComponent<Text>().text = current + "/" + max;
		if (current == 0f)
		{
			progressSet.Find("Bar/Current").GetComponent<Image>().fillAmount = 0f;
			progressSet.Find("Bar/Icon").GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			return;
		}
		float fillAmount = progressSet.Find("Bar/Current").GetComponent<Image>().fillAmount;
		float num = current / max;
		float x = progressSet.Find("Bar/Icon").GetComponent<RectTransform>().anchoredPosition.x;
		float endValue = progressSet.Find("Bar").GetComponent<RectTransform>().rect.width * num;
		StartCoroutine(AppUtil.MoveEasingFloat(fillAmount, num, delegate(float tmp)
		{
			progressSet.Find("Bar/Current").GetComponent<Image>().fillAmount = tmp;
		}, true, 0.25f, EasingFunction.Ease.EaseOutCubic));
		StartCoroutine(AppUtil.MoveEasingFloat(x, endValue, delegate(float tmp)
		{
			progressSet.Find("Bar/Icon").GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp, 0f);
		}, true, 0.25f, EasingFunction.Ease.EaseOutCubic));
	}

	private IEnumerator StartDialogue()
	{
		GameObject commentFrame = EsComment.transform.parent.gameObject;
		Image touch = GameObject.Find("MessageScreen").GetComponent<Image>();
		string scenarioType = ScenarioType;
		for (int i = 1; i <= 100; i++)
		{
			string scenarioKey = Data.GetScenarioKey(ScenarioType, i.ToString());
			string scenarioKey2 = Data.GetScenarioKey(ScenarioType, i + "問");
			string scenarioKey3 = Data.GetScenarioKey(ScenarioType, i + "分岐", "Law");
			string scenarioKey4 = Data.GetScenarioKey(ScenarioType, i + "分岐", "SE");
			string scenarioKey5 = Data.GetScenarioKey(ScenarioType, i + "変化-Law");
			string scenarioKey6 = Data.GetScenarioKey(ScenarioType, i + "変化-立ち絵");
			if (scenarioKey != null || scenarioKey3 != null || scenarioKey4 != null)
			{
				string key = scenarioKey;
				if (scenarioKey3 != null)
				{
					key = Data.GetScenarioKey(ScenarioType, i + "分岐", PlayerStatus.LCTypeTrend);
				}
				if (scenarioKey4 != null)
				{
					string routeTrend = PlayerStatus.RouteTrend;
					key = Data.GetScenarioKey(ScenarioType, i + "分岐", routeTrend);
				}
				if (scenarioKey5 != null)
				{
					string text = LanguageManager.Get(scenarioKey5);
					if (!(text == "<狂気>"))
					{
						if (text == "<ほほえみ>")
						{
							SetEsFace("Smile");
						}
					}
					else
					{
						StartCoroutine(SetEsCrazy());
					}
				}
				if (scenarioKey6 != null)
				{
					string esFace = LanguageManager.Get(scenarioKey6);
					SetEsFace(esFace);
				}
				commentFrame.SetActive(false);
				yield return AppUtil.Wait(0.2f);
				EsComment.SetKey(key);
				commentFrame.SetActive(true);
				touch.raycastTarget = true;
				while (touch.raycastTarget)
				{
					yield return null;
				}
			}
			else
			{
				if (scenarioKey2 == null)
				{
					break;
				}
				yield return StartDialogueQuestion(i);
			}
			if (scenarioType != ScenarioType)
			{
				i = 0;
				scenarioType = ScenarioType;
			}
		}
	}

	private IEnumerator StartDialogueQuestion(int no, int max = 0)
	{
		GameObject commentFrame = EsComment.transform.parent.gameObject;
		commentFrame.SetActive(false);
		if (ScenarioType.Contains("診断"))
		{
			SetProgressState(base.transform.Find("UICanvas/UI2/ChoiceGroup"), no - 1, max);
			for (int i = 0; i <= max; i++)
			{
				base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Q" + i).gameObject.SetActive(i == no);
			}
		}
		if (ScenarioType == "診断2-3" || ScenarioType == "診断2-4")
		{
			base.transform.Find("UICanvas/FukidasiGroup/" + ScenarioType).gameObject.SetActive(true);
		}
		string type = ScenarioType;
		if (ScenarioNo == "本を借りる")
		{
			ScenarioType = "本を借りる1";
			no = 9;
		}
		yield return AppUtil.Wait(0.2f);
		string scenarioKey = Data.GetScenarioKey(ScenarioType, no + "問");
		EsComment.SetKey(scenarioKey);
		commentFrame.SetActive(true);
		ChoiceGroup choice = GetComponentInChildren<ChoiceGroup>(true);
		choice.gameObject.SetActive(true);
		string[] array;
		switch (ScenarioNo)
		{
		case "日替わり":
			array = new string[3] { "Perfect", "Good", "Normal" };
			break;
		case "本を借りる":
		{
			int[] array2 = AppUtil.RandomArray(1, 10);
			int length = PlayerStatus.ReadingBookList.Length;
			int num = Mathf.Min(3, length);
			string text = PlayerStatus.ReadingBookList.Substring(length - num, num);
			List<string> list = new List<string>();
			int[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				int num2 = array3[j];
				string value = num2.ToString("X");
				if (!text.Contains(value))
				{
					list.Add(num2.ToString());
				}
				if (list.Count >= 3)
				{
					break;
				}
			}
			array = list.ToArray();
			break;
		}
		case "本を返す":
			array = new string[3] { "1", "2", "3" };
			break;
		default:
			array = new string[3] { "Law", "Chaos", "Neutral" };
			break;
		}
		if (ScenarioType == "本を返す共通")
		{
			array = new string[3] { "既読", "未読", "" };
		}
		string[] choiceList = new string[3]
		{
			"Choice" + array[0],
			"Choice" + array[1],
			"Choice" + array[2]
		};
		string[] choiceKeyList = new string[3]
		{
			Data.GetScenarioKey(ScenarioType, no + "選択肢", array[0]),
			Data.GetScenarioKey(ScenarioType, no + "選択肢", array[1]),
			Data.GetScenarioKey(ScenarioType, no + "選択肢", array[2])
		};
		bool restartChoice = Data.GetScenarioSpecific(ScenarioNo).Contains("退行");
		bool isRandom = !restartChoice && ScenarioType != "本を返す共通";
		yield return choice.SetButton(choiceList, choiceKeyList, isRandom);
		WaitingChoice = true;
		while (WaitingChoice)
		{
			yield return null;
		}
		yield return AppUtil.Wait(0.5f);
		string choosenItem = "";
		int num3 = 0;
		for (int k = 0; k < choiceList.Length; k++)
		{
			if (base.transform.Find("UICanvas/UI2/ChoiceGroup/" + choiceList[k]).gameObject.activeSelf)
			{
				choosenItem = choiceList[k].Replace("Choice", "");
				num3 = k;
				break;
			}
		}
		Debug.Log("choosenItem=" + choosenItem);
		if (!ScenarioNo.Contains("4章"))
		{
			UserChoice.Set(ScenarioType, choosenItem);
		}
		if (restartChoice)
		{
			if (choosenItem == "Law")
			{
				ScenarioNo = "退行";
			}
			else
			{
				ScenarioNo = "退行中止";
			}
		}
		commentFrame.SetActive(false);
		if (choiceKeyList[0].Contains("診断"))
		{
			string scenarioKey2 = Data.GetScenarioKey(ScenarioType, no + "解析中表示単語", choosenItem);
			if (scenarioKey2 != null)
			{
				WordListForDisplay.AddRange(LanguageManager.Get(scenarioKey2).Split('、'));
			}
			choice.DismissButton();
			SetProgressState(base.transform.Find("UICanvas/UI2/ChoiceGroup"), no, max);
			if (no == max)
			{
				base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Q" + no).gameObject.SetActive(false);
				Transform transform = base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Ending" + (num3 + 1));
				transform.gameObject.SetActive(true);
				StartCoroutine(AppUtil.FadeIn(transform));
				if (ScenarioType == "診断2-5")
				{
					switch (num3)
					{
					case 0:
						ScenarioType = "診断2-4";
						if (PlayerStatus.ScenarioNo.Contains("3章"))
						{
							PlayerStatus.Route = "SE";
						}
						break;
					case 1:
						ScenarioType = "診断2-3";
						if (PlayerStatus.ScenarioNo.Contains("3章"))
						{
							PlayerStatus.Route = "ID";
						}
						break;
					}
				}
				if (ScenarioType == "診断2-3")
				{
					base.transform.Find("UICanvas/UI2/EhonGroup/Background").GetComponent<Image>().color = Color.black;
					base.transform.Find("UICanvas/UI2/EhonGroup/診断2-5/Background").gameObject.SetActive(false);
					base.transform.Find("UICanvas/UI2/EhonGroup/" + ScenarioType + "/Background").gameObject.SetActive(false);
					base.transform.Find("UICanvas/FukidasiGroup/" + ScenarioType).gameObject.SetActive(false);
				}
				if (ScenarioType == "診断2-4")
				{
					base.transform.Find("UICanvas/FukidasiGroup/" + ScenarioType).gameObject.SetActive(false);
				}
			}
		}
		else
		{
			choice.gameObject.SetActive(false);
		}
		yield return AppUtil.Wait(0.5f);
		yield return EsTalk(no + "回答-" + choosenItem, false);
		if (ScenarioNo == "本を借りる")
		{
			PlayerStatus.ReadingBook = int.Parse(choosenItem);
			ScenarioType = type;
		}
		if (ScenarioType == "本を返す共通")
		{
			int readingBook = PlayerStatus.ReadingBook;
			ScenarioType = "本を返す" + readingBook + "-" + choosenItem;
			if (Data.GetScenarioKey(ScenarioType, "1") == null)
			{
				ScenarioType = "本を返す" + 7 + "-" + choosenItem;
			}
		}
	}
}

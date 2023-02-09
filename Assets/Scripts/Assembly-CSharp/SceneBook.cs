using System.Collections;
using App;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneBook : SceneBase
{
	private enum HA_INDEX
	{
		SPEED = 1,
		POINT = 2,
		SECONDS = 3,
		RATE = 4,
		MODE = 5
	}

	public GameObject OpeningGroup;

	public GameObject UICanvas;

	private GameObject WallGroup;

	private GameObject CommentGroup;

	private int HyperActivePoint
	{
		get
		{
			return HyperActive.Point;
		}
		set
		{
			HyperActive.Point = value;
		}
	}

	private int HyperActivePhase
	{
		get
		{
			return HyperActive.Phase;
		}
		set
		{
			HyperActive.Phase = value;
		}
	}

	private float HyperActivePhaseSeconds
	{
		get
		{
			return HyperActive.Time;
		}
		set
		{
			HyperActive.Time = value;
		}
	}

	private int HaMaxPhase
	{
		get
		{
			return GssDataHelper.GetSheet("多動モード").Length - 1;
		}
	}

	private bool IsHAMode
	{
		get
		{
			return GetHaMode().Contains("多動");
		}
	}

	private bool IsHALastMode
	{
		get
		{
			return GetHaMode() == "多動ラスト";
		}
	}

	protected override void Start()
	{
		base.Start();
		Debug.Log("SceneBook:Start()");
		WallGroup = base.transform.Find("WallGroup/Wall").gameObject;
		if (PlayerStatus.ScenarioNo.Contains("4章SE"))
		{
			base.transform.Find("WallGroup/Wall/LineGroup").gameObject.SetActive(false);
			base.transform.Find("WallGroup/Wall/Ego").gameObject.SetActive(true);
			base.transform.Find("WallGroup/Shadow1").gameObject.SetActive(false);
		}
		CommentGroup = base.transform.Find("WallGroup/Wall/CommentGroup").gameObject;
		base.transform.Find("UICanvas/BookListView/Viewport").gameObject.SetActive(false);
		string text = Data.GetScenarioType(PlayerStatus.ScenarioNo);
		if (PlayerStatus.TutorialLv == PlayerStatus.TUTORIAL_MAX)
		{
			int scenarioIndex = Data.GetScenarioIndex(PlayerStatus.ScenarioNo);
			int scenarioIndex2 = Data.GetScenarioIndex("1章-1");
			if (scenarioIndex > scenarioIndex2)
			{
				text = "壁会話3";
			}
		}
		else if (PlayerStatus.ScenarioNo == "退行")
		{
			text = "壁会話9";
		}
		if (text == "オープニング")
		{
			OpeningGroup.SetActive(true);
			UICanvas.SetActive(false);
			StartCoroutine(Opening());
			AdManager.Hide("Banner");
			return;
		}
		if (text.Contains("壁会話"))
		{
			OpeningGroup.SetActive(false);
			UICanvas.SetActive(false);
			StartCoroutine(KingEgo(text));
			AdManager.Show("Banner");
			return;
		}
		OpeningGroup.SetActive(false);
		UICanvas.SetActive(true);
		float haParameters = GetHaParameters(HyperActivePhase, HA_INDEX.RATE);
		string mode = (IsHAMode ? "HyperActive" : "None");
		GetComponentInChildren<CommentManager>().Generate(true, mode, haParameters);
		base.transform.Find("UICanvas/BookListView/Viewport").gameObject.SetActive(true);
		AppUtil.DelayAction(this, 1f, Walking(true));
		AdManager.Show("Banner");
	}

	public void SetTutorial(int lv, bool updateMessage = false)
	{
		if (PlayerStatus.TutorialLv < PlayerStatus.TUTORIAL_MAX)
		{
			if (PlayerStatus.TutorialLv + 1 == lv)
			{
				PlayerStatus.TutorialLv = lv;
				updateMessage = true;
			}
			if (updateMessage)
			{
				string key = "壁会話2_" + PlayerStatus.TutorialLv;
				StartCoroutine(SetEgoMessage(key, true));
			}
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (3 <= PlayerStatus.TutorialLv && PlayerStatus.TutorialLv <= 6)
		{
			if (PlayerStatus.EgoPoint >= Data.BOOK_PARAMETER(1)[0])
			{
				SetTutorial(4);
			}
			if (PlayerResult.BookPageSum > 0)
			{
				SetTutorial(5);
			}
			EgoPoint nEXT_SCENARIO_PRICE = Data.NEXT_SCENARIO_PRICE;
			if (PlayerStatus.EgoPoint * 2f >= nEXT_SCENARIO_PRICE)
			{
				SetTutorial(6);
			}
			if (PlayerStatus.EgoPoint >= nEXT_SCENARIO_PRICE)
			{
				SetTutorial(7);
			}
		}
	}

	private IEnumerator Opening()
	{
		Text egomessage1 = OpeningGroup.transform.Find("OpeningCanvas/Message1").GetComponent<Text>();
		Text egomessage2 = OpeningGroup.transform.Find("OpeningCanvas/Message2").GetComponent<Text>();
		Text author = OpeningGroup.transform.Find("OpeningCanvas/Author").GetComponent<Text>();
		AppUtil.SetAlpha(egomessage1, 0f);
		AppUtil.SetAlpha(egomessage2, 0f);
		AppUtil.SetAlpha(author, 0f);
		yield return AppUtil.Wait(1f);
		yield return AppUtil.FadeIn(egomessage1);
		yield return AppUtil.Wait(1f);
		yield return AppUtil.FadeIn(egomessage2);
		yield return AppUtil.Wait(1f);
		yield return AppUtil.FadeIn(author);
		Debug.Log("AutoTestEvent:Screenshot");
		yield return AppUtil.Wait(2.5f);
		StartCoroutine(AppUtil.FadeOut(egomessage1));
		StartCoroutine(AppUtil.FadeOut(egomessage2));
		yield return AppUtil.FadeOut(author);
		yield return AppUtil.Wait(0.5f);
		yield return AppUtil.FadeOut(OpeningGroup.transform.Find("OpeningCanvas/Background").GetComponent<Image>(), 1f);
		yield return AppUtil.Wait(0.5f);
		SetTutorial(0);
		Data.GoToNextScenario();
		StartCoroutine(KingEgo(Data.GetScenarioType(PlayerStatus.ScenarioNo)));
		AdManager.Show("Banner");
	}

	private IEnumerator KingEgo(string scenarioType)
	{
		OpeningGroup.transform.Find("OpeningCanvas").gameObject.SetActive(false);
		IEnumerator walking = Walking(false);
		yield return AppUtil.Wait(1f);
		StartCoroutine(walking);
		yield return AppUtil.Wait(4.5f);
		StopCoroutine(walking);
		yield return AppUtil.Wait(0.5f);
		base.transform.Find("KingEgo").gameObject.SetActive(true);
		Animator kingEgoAnim = base.transform.Find("KingEgo").GetComponent<Animator>();
		yield return AppUtil.WaitAnimation(kingEgoAnim);
		kingEgoAnim.SetFloat("Speed", 0f);
		int no = 1;
		string[] keyTypeList = new string[3]
		{
			"",
			PlayerStatus.LCTypeTrend,
			PlayerStatus.RouteTrend
		};
		for (; no <= 50; no++)
		{
			string text = "";
			string[] array = keyTypeList;
			foreach (string text2 in array)
			{
				string text3 = scenarioType + "_" + no;
				string text4 = text3 + text2;
				if (LanguageManager.Contains(text4 + "-L"))
				{
					text = text4;
					break;
				}
				text4 = text3 + "同" + text2;
				if (LanguageManager.Contains(text4 + "-L"))
				{
					text = text4;
					break;
				}
			}
			if (text == "")
			{
				yield return SetEgoMessage(text, false);
				break;
			}
			if (LanguageManager.Get(text + "-L") == "[[操作説明]]")
			{
				yield return AppUtil.Wait(0.25f);
				DialogManager.ShowDialog("TutorialDialog");
				while (DialogManager.IsShowing())
				{
					yield return null;
				}
				yield return AppUtil.Wait(0.25f);
			}
			else
			{
				yield return SetEgoMessage(text, false);
			}
		}
		base.transform.Find("MessageCanvas/MessageWindow/MessageL").gameObject.SetActive(false);
		base.transform.Find("MessageCanvas/MessageWindow/MessageR").gameObject.SetActive(false);
		base.transform.Find("MessageCanvas/TutorialWindow/MessageL").gameObject.SetActive(false);
		base.transform.Find("MessageCanvas/TutorialWindow/MessageR").gameObject.SetActive(false);
		yield return AppUtil.Wait(0.5f);
		kingEgoAnim.SetFloat("Speed", -1f);
		yield return AppUtil.WaitAnimation(kingEgoAnim);
		yield return StartCoroutine(AppUtil.FadeOut(OpeningGroup));
		base.transform.Find("KingEgo").gameObject.SetActive(false);
		AppUtil.SetAlpha(UICanvas, 0f);
		AppUtil.SetAlpha(base.transform.Find("UICanvas/UI_Header/Header2"), 0f);
		if (scenarioType == "壁会話3")
		{
			PlayerStatus.TutorialLv = PlayerStatus.TUTORIAL_MAX + 1;
		}
		else if (scenarioType == "壁会話9")
		{
			PlayerStatus.ScenarioNo = "1章-1";
		}
		else
		{
			Data.GoToNextScenario();
			SetTutorial(1, true);
		}
		yield return AppUtil.Wait(0.5f);
		UICanvas.SetActive(true);
		StartCoroutine(AppUtil.FadeIn(UICanvas));
		yield return AppUtil.Wait(0.25f);
		yield return StartCoroutine(AppUtil.FadeIn(base.transform.Find("UICanvas/UI_Header/Header2")));
		base.transform.Find("UICanvas/BookListView/Viewport").gameObject.SetActive(true);
		yield return AppUtil.Wait(1f);
		StartCoroutine(Walking(true));
	}

	private IEnumerator SetEgoMessage(string key, bool tutorial)
	{
		while (DialogManager.IsShowing())
		{
			yield return null;
		}
		string window = "MessageWindow";
		if (tutorial)
		{
			window = "TutorialWindow";
		}
		else
		{
			yield return AppUtil.Wait(0.25f);
		}
		GameObject messageL = base.transform.Find("MessageCanvas/" + window + "/MessageL").gameObject;
		GameObject messageR = base.transform.Find("MessageCanvas/" + window + "/MessageR").gameObject;
		if (base.transform.Find("MessageCanvas/" + window + "/MessageL").gameObject.activeSelf)
		{
			messageL.GetComponent<Animator>().PlayInFixedTime("MessageL", -1, 0.2f);
			messageR.GetComponent<Animator>().PlayInFixedTime("MessageR", -1, 0.2f);
			messageL.GetComponent<Animator>().SetFloat("Speed", -1f);
			messageR.GetComponent<Animator>().SetFloat("Speed", -1f);
			yield return AppUtil.FadeOut(base.transform.Find("MessageCanvas/" + window), 0.15f);
			messageL.SetActive(false);
			messageR.SetActive(false);
			AppUtil.SetAlpha(base.transform.Find("MessageCanvas/" + window), 1f);
		}
		if (key == "")
		{
			yield break;
		}
		yield return AppUtil.Wait(0.25f);
		messageL.GetComponentInChildren<TextLocalization>().SetKey(key + "-L");
		messageL.GetComponent<Animator>().SetFloat("Speed", 1f);
		messageL.SetActive(true);
		Image touch = GameObject.Find("MessageScreen").GetComponent<Image>();
		if (!tutorial && !key.Contains("同"))
		{
			yield return AppUtil.Wait(0.5f);
		}
		messageR.GetComponentInChildren<TextLocalization>().SetKey(key + "-R");
		messageR.GetComponent<Animator>().SetFloat("Speed", 1f);
		messageR.SetActive(true);
		Debug.Log("AutoTestEvent:Screenshot");
		if (!tutorial)
		{
			touch.raycastTarget = true;
			while (touch.raycastTarget)
			{
				yield return null;
			}
		}
	}

	private float GetHaParameters(int phase, HA_INDEX index)
	{
		return float.Parse(GssDataHelper.GetSheet("多動モード")[phase][(int)index]);
	}

	private string GetHaMode()
	{
		return GssDataHelper.GetSheet("多動モード")[HyperActivePhase][5];
	}

	private void UpdateHYPhase(float passedTime)
	{
		HyperActivePhaseSeconds += passedTime;
		int num = (int)GetHaParameters(HyperActivePhase, HA_INDEX.POINT);
		int num2 = (int)GetHaParameters(HyperActivePhase, HA_INDEX.SECONDS);
		if (num > 0)
		{
			if (HyperActivePoint >= num)
			{
				MoveNextPhase();
			}
			if (num2 > 0 && HyperActivePhaseSeconds >= (float)num2)
			{
				MoveNextPhase(false);
			}
		}
		else if (HyperActivePhaseSeconds >= (float)num2)
		{
			MoveNextPhase();
		}
	}

	private void MoveNextPhase(bool next = true)
	{
		HyperActivePhase += (next ? 1 : (-1));
		if (HyperActivePhase > HaMaxPhase)
		{
			HyperActivePhase = 1;
		}
		if (HyperActivePhase <= 0)
		{
			HyperActivePhase = HaMaxPhase;
		}
	}

	private IEnumerator Walking(bool withComment)
	{
		int i = 0;
		CommentManager mgr = GetComponentInChildren<CommentManager>();
		float speed = 1f;
		EasingFunction.Ease easeIn = EasingFunction.Ease.EaseInSine;
		EasingFunction.Ease easeOut = EasingFunction.Ease.EaseOutSine;
		while (true)
		{
			float passedTime = HyperActive.PassedTime;
			UpdateHYPhase(passedTime);
			float fukidasiRate = GetHaParameters(HyperActivePhase, HA_INDEX.RATE);
			string commentMode = (IsHALastMode ? "HyperActiveLast" : (IsHAMode ? "HyperActive" : "None"));
			if (withComment && 0f < fukidasiRate && fukidasiRate < 1f)
			{
				break;
			}
			if (IsHAMode || !mgr.KeepHyperActive())
			{
				speed = GetHaParameters(HyperActivePhase, HA_INDEX.SPEED);
			}
			float moveTime = 0.42f / speed;
			float distance = -1.5f;
			Vector3 start2 = WallGroup.transform.position;
			Vector3 end2 = start2 + new Vector3(0f, 0f, distance);
			Transform cameraT = GameObject.Find("Main Camera").transform;
			Vector3 startCamera = cameraT.position;
			Vector3 endCamera = startCamera + new Vector3(0f, (speed <= 1.2f) ? (-0.04f) : (-0.02f), 0f);
			while (DialogManager.IsShowing())
			{
				yield return null;
			}
			StartCoroutine(AppUtil.MoveEasingVector3(start2, end2, delegate(Vector3 ret)
			{
				WallGroup.transform.position = ret;
			}, false, moveTime, easeIn));
			yield return AppUtil.MoveEasingVector3(startCamera, endCamera, delegate(Vector3 ret)
			{
				cameraT.position = ret;
			}, false, moveTime, easeIn);
			start2 = end2;
			end2 = start2 + new Vector3(0f, 0f, distance);
			while (DialogManager.IsShowing())
			{
				yield return null;
			}
			StartCoroutine(AppUtil.MoveEasingVector3(start2, end2, delegate(Vector3 ret)
			{
				WallGroup.transform.position = ret;
			}, false, moveTime, easeOut));
			yield return AppUtil.MoveEasingVector3(endCamera, startCamera, delegate(Vector3 ret)
			{
				cameraT.position = ret;
			}, false, moveTime, easeOut);
			mgr.UpdateCommentWithDestroy();
			float num = 40f;
			if (base.transform.Find("WallGroup/Wall/Ego").gameObject.activeSelf)
			{
				num = 38.475f;
			}
			while (WallGroup.transform.localPosition.z <= 0f - num)
			{
				WallGroup.transform.Translate(new Vector3(0f, 0f, num));
				Comment[] componentsInChildren = GetComponentsInChildren<Comment>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].transform.Translate(new Vector3(0f, 0f, 0f - num));
				}
			}
			if (withComment && fukidasiRate >= 1f && (float)i >= fukidasiRate - 1f)
			{
				mgr.Generate(false, commentMode, fukidasiRate);
				i = 0;
			}
			else
			{
				i++;
			}
		}
		StartCoroutine(WalkingHAMode());
	}

	private IEnumerator WalkingHAMode()
	{
		float haParameters = GetHaParameters(HyperActivePhase, HA_INDEX.SPEED);
		float moveTime = 0.42f / haParameters * 2f;
		float distance = -1.5f;
		float fukidasiRate = GetHaParameters(HyperActivePhase, HA_INDEX.RATE);
		CommentManager mgr = GetComponentInChildren<CommentManager>();
		float num = moveTime * fukidasiRate;
		InvokeRepeating("GenerateComment", num, num);
		InvokeRepeating("DestroyComment", num, num);
		while (true)
		{
			float passedTime = HyperActive.PassedTime;
			UpdateHYPhase(passedTime);
			float haParameters2 = GetHaParameters(HyperActivePhase, HA_INDEX.RATE);
			if (haParameters2 == 0f || haParameters2 >= 1f)
			{
				break;
			}
			Vector3 position = WallGroup.transform.position;
			Vector3 endValue = position + new Vector3(0f, 0f, distance * 2f);
			yield return StartCoroutine(AppUtil.MoveEasingVector3(position, endValue, delegate(Vector3 ret)
			{
				WallGroup.transform.position = ret;
			}, false, moveTime * 0.95f, EasingFunction.Ease.Linear));
			yield return AppUtil.Wait(moveTime * 0.05f);
			float num2 = 40f;
			if (base.transform.Find("WallGroup/Wall/Ego").gameObject.activeSelf)
			{
				num2 = 38.475f;
			}
			while (WallGroup.transform.localPosition.z <= 0f - num2)
			{
				WallGroup.transform.Translate(new Vector3(0f, 0f, num2));
				Comment[] componentsInChildren = GetComponentsInChildren<Comment>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].transform.Translate(new Vector3(0f, 0f, 0f - num2));
				}
			}
		}
		CancelInvoke("GenerateComment");
		AppUtil.DelayAction(this, moveTime, delegate
		{
			mgr.Generate(false, "HyperActiveLast", fukidasiRate);
		});
		while (GetComponentsInChildren<Comment>().Length != 0)
		{
			Vector3 position2 = WallGroup.transform.position;
			Vector3 endValue2 = position2 + new Vector3(0f, 0f, distance * 2f);
			yield return StartCoroutine(AppUtil.MoveEasingVector3(position2, endValue2, delegate(Vector3 ret)
			{
				WallGroup.transform.position = ret;
			}, false, moveTime * 0.95f, EasingFunction.Ease.Linear));
			yield return AppUtil.Wait(moveTime * 0.05f);
			float num3 = 40f;
			if (base.transform.Find("WallGroup/Wall/Ego").gameObject.activeSelf)
			{
				num3 = 38.475f;
			}
			while (WallGroup.transform.localPosition.z <= 0f - num3)
			{
				WallGroup.transform.Translate(new Vector3(0f, 0f, num3));
				Comment[] componentsInChildren = GetComponentsInChildren<Comment>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].transform.Translate(new Vector3(0f, 0f, 0f - num3));
				}
			}
		}
		CancelInvoke("DestroyComment");
		StartCoroutine(Walking(true));
	}

	private void GenerateComment()
	{
		CommentManager componentInChildren = GetComponentInChildren<CommentManager>();
		float haParameters = GetHaParameters(HyperActivePhase, HA_INDEX.RATE);
		string mode = (IsHALastMode ? "HyperActiveLast" : (IsHAMode ? "HyperActive" : "None"));
		componentInChildren.Generate(false, mode, haParameters);
	}

	private void DestroyComment()
	{
		GetComponentInChildren<CommentManager>().UpdateCommentWithDestroy();
	}

	public void TapEgo(GameObject tapFukidasi)
	{
		Comment component = tapFukidasi.GetComponent<Comment>();
		if (ReadFukidasiList.Add(component.CommentKey))
		{
			HyperActivePoint += 10;
		}
		else
		{
			HyperActivePoint++;
		}
		SetTutorial(3);
		bool num = tapFukidasi.name.Contains("CloseComment");
		EgoPoint egoPoint = Data.TapPower(num);
		if (component.Type != "")
		{
			egoPoint *= 3f;
		}
		string n = "FarEgoEffect";
		if (num)
		{
			n = "CloseEgoEffect";
		}
		GameObject obj = Object.Instantiate(CommentGroup.transform.Find(n).gameObject, CommentGroup.transform);
		obj.name = n;
		obj.GetComponentInChildren<TextMeshPro>().text = egoPoint.ToString("+0");
		obj.transform.position = tapFukidasi.transform.position + new Vector3(0f, -1.5f, 0f);
		obj.transform.SetParent(null);
		obj.GetComponentInChildren<Animator>(true).enabled = true;
		PlayerStatus.EgoPoint += egoPoint;
		PlayerResult.TapMax = egoPoint;
		PlayerResult.TapCount++;
		UpdateHeader();
	}

	public new void OnClick(GameObject clickObject)
	{
		if (!base.OnClick(clickObject))
		{
			clickObject.name = clickObject.name.Replace("(Clone)", "");
			Debug.Log("SceneBook:OnClick " + clickObject.name);
			string text = clickObject.name;
			if (text == "MessageScreen")
			{
				clickObject.GetComponent<Image>().raycastTarget = false;
			}
			else if (clickObject.name.Contains("Comment"))
			{
				TapEgo(clickObject);
			}
		}
	}
}

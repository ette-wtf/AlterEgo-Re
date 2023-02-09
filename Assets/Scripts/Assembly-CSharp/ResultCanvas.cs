using System.Collections;
using System.Collections.Generic;
using App;
using UnityEngine;
using UnityEngine.UI;

public class ResultCanvas : MonoBehaviour
{
	public Sprite[] ResultFrame;

	private string ScenarioType;

	[SerializeField]
	private OnClickHandler CloseButton;

	private void OnEnable()
	{
		SceneCommon.BackButtonList.Add(CloseButton);
	}

	private void OnDisable()
	{
		SceneCommon.BackButtonList.Remove(CloseButton);
	}

	public void SetActive(bool active)
	{
		if (active)
		{
			base.gameObject.SetActive(active);
			Sprite sprite = ResultFrame[Random.Range(0, ResultFrame.Length)];
			base.transform.Find("Result").GetComponent<Image>().sprite = sprite;
			StartCoroutine(AppUtil.FadeIn(base.gameObject));
		}
		else if (base.gameObject.activeSelf)
		{
			StartCoroutine(AppUtil.FadeOut(base.gameObject, 0.5f, delegate
			{
				base.gameObject.SetActive(false);
			}));
		}
	}

	private void OnClick(GameObject clickObject)
	{
		switch (clickObject.name)
		{
		case "CloseButton":
		case "RetryButtonFromResult":
			SetActive(false);
			break;
		case "ShowResultButton":
			StartCoroutine(ShowResult());
			break;
		case "ResultShareButton":
		{
			clickObject.GetComponent<Button>().interactable = false;
			StartCoroutine(AppUtil.DelayAction(1f, delegate
			{
				clickObject.GetComponent<Button>().interactable = true;
			}));
			RectTransform captureTarget = (RectTransform)base.transform.Find("Result/ShareArea");
			string text = base.transform.Find("Result/TextGroup/StatusText1").GetComponent<Text>().text;
			string text2 = base.transform.Find("Result/TextGroup/StatusText2").GetComponent<Text>().text;
			string msg = text + "「" + text2 + "」\n自分探しタップゲーム『ALTER EGO』 caracolu.com/app/alterego/ #ALTEREGO";
			StartCoroutine(AppUtil.Share(false, msg, null, captureTarget));
			break;
		}
		case "Filter":
		case "ChangeButton":
		{
			Transform obj = base.transform.Find("Result/Filter");
			float alpha = obj.GetComponent<CanvasGroup>().alpha;
			AppUtil.SetAlpha(obj, (!(alpha > 0f)) ? 1 : 0);
			break;
		}
		}
	}

	public void RequestResult(string scenarioType, string type, List<string> displayWordList)
	{
		SetActive(true);
		ScenarioType = scenarioType;
		base.transform.Find("ButtonGroup").gameObject.SetActive(false);
		base.transform.Find("Result/Filter/FukidasiGroup").gameObject.SetActive(false);
		base.transform.Find("Result/Filter").GetComponent<CanvasGroup>().blocksRaycasts = false;
		AppUtil.SetAlpha(base.transform.Find("Result/Filter"), 1f);
		base.transform.Find("MirrorFragments").gameObject.SetActive(false);
		AppUtil.SetAlpha(base.transform.Find("Result/TextGroup"), 0f);
		if (AdInfo.ENABLE)
		{
			if (AdManager.IsReady("Reward"))
			{
				base.transform.Find("ButtonGroup/RetryButtonFromResult/").GetComponent<Button>().interactable = true;
				base.transform.Find("ButtonGroup/RetryButtonFromResult/Text").GetComponent<TextLocalization>().SetKey("[UI]RetryButtonFromResult/Text");
			}
			else
			{
				base.transform.Find("ButtonGroup/RetryButtonFromResult/").GetComponent<Button>().interactable = false;
				base.transform.Find("ButtonGroup/RetryButtonFromResult/Text").GetComponent<TextLocalization>().SetKey("[UI]NoAdMovieMessage");
			}
		}
		else
		{
			base.transform.Find("ButtonGroup/RetryButtonFromResult/Text").GetComponent<TextLocalization>().SetKey("[UI]RetryButtonFromResult/TextNoAd");
		}
		if (displayWordList != null)
		{
			AdManager.Show("Native");
			AdManager.Hide("Banner");
			base.transform.Find("Analyzing").gameObject.SetActive(true);
			base.transform.Find("Analyzing").GetComponent<ResultAnalyzing>().StartAnalyzing(displayWordList.ToArray());
			AppUtil.DelayAction(this, 8f, base.transform.Find("Analyzing").GetComponent<ResultAnalyzing>().StopAnalyzing());
		}
		else
		{
			StartCoroutine(ShowResultInLibrary());
		}
		TextLocalization[] componentsInChildren = base.transform.Find("Result/TextGroup").GetComponentsInChildren<TextLocalization>();
		foreach (TextLocalization textLocalization in componentsInChildren)
		{
			switch (textLocalization.name)
			{
			case "結果解説":
				textLocalization.SetKey(Data.GetScenarioKey(scenarioType, "解説", type));
				break;
			case "Title":
			{
				string text2 = LanguageManager.Get("TitlePrefix");
				if (scenarioType == "ゲームクリア")
				{
					text2 = "";
				}
				textLocalization.SetText(text2 + LanguageManager.Get(Data.GetScenarioKey(scenarioType, "結果タイトル")) + LanguageManager.Get("TitlePostfix"));
				break;
			}
			case "StatusText1":
				textLocalization.SetText(LanguageManager.Get(Data.GetScenarioKey(scenarioType, "型コピー", type)));
				break;
			case "StatusText2":
			{
				string text = LanguageManager.Get(Data.GetScenarioKey(scenarioType, "型", type));
				text = LanguageManager.Get("TypePrefix") + text + LanguageManager.Get("TypePostfix");
				textLocalization.SetText(text);
				break;
			}
			}
		}
	}

	private IEnumerator ShowResultInLibrary()
	{
		CounselingResult.Read(ScenarioType);
		base.transform.Find("ButtonGroupLibrary").gameObject.SetActive(true);
		base.transform.Find("ButtonGroupLibrary/ChangeButton").gameObject.SetActive(false);
		base.transform.Find("ButtonGroupLibrary/ResultShareButton").gameObject.SetActive(false);
		for (int j = 1; j <= 4; j++)
		{
			base.transform.Find("Result/Filter/FukidasiGroup/Fukidasi" + j).gameObject.SetActive(false);
		}
		base.transform.Find("Result/Filter/FukidasiGroup").gameObject.SetActive(true);
		base.transform.Find("Result/Filter").GetComponent<Image>().raycastTarget = true;
		base.transform.Find("ButtonGroupLibrary/ChangeButton").GetComponent<Button>().interactable = true;
		for (int i = 1; i <= 4; i++)
		{
			string scenarioType = ScenarioType;
			string key = "[Fukidasi]CR" + scenarioType + "_" + CounselingResult.Get(scenarioType) + "-" + i;
			if (LanguageManager.Contains(key))
			{
				yield return AppUtil.Wait(0.4f);
				base.transform.Find("Result/Filter/FukidasiGroup/Fukidasi" + i).GetComponent<TextLocalization>().SetKey(key);
				base.transform.Find("Result/Filter/FukidasiGroup/Fukidasi" + i).gameObject.SetActive(true);
				continue;
			}
			base.transform.Find("Result/Filter").GetComponent<Image>().raycastTarget = false;
			base.transform.Find("ButtonGroupLibrary/ChangeButton").GetComponent<Button>().interactable = false;
			break;
		}
		yield return AppUtil.Wait(1f);
		StartCoroutine(AppUtil.FadeOut(base.transform.Find("Result/Filter"), 0.25f));
		base.transform.Find("MirrorFragments").gameObject.SetActive(true);
		AudioManager.PlaySound("Click", "", "ShowResultButton");
		yield return AppUtil.Wait(1f);
		yield return AppUtil.FadeIn(base.transform.Find("Result/TextGroup"));
		base.transform.Find("ButtonGroupLibrary/ChangeButton").gameObject.SetActive(true);
		base.transform.Find("ButtonGroupLibrary/ResultShareButton").gameObject.SetActive(true);
		base.transform.Find("Result/Filter").GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	private IEnumerator ShowResult()
	{
		base.transform.Find("Analyzing").gameObject.SetActive(false);
		StartCoroutine(AppUtil.FadeOut(base.transform.Find("Result/Filter"), 0.25f));
		base.transform.Find("MirrorFragments").gameObject.SetActive(true);
		yield return AppUtil.Wait(1f);
		yield return AppUtil.FadeIn(base.transform.Find("Result/TextGroup"));
		AdManager.Hide("Native");
		AdManager.Show("Banner");
		yield return AppUtil.Wait(0.5f);
		base.transform.Find("ButtonGroup").gameObject.SetActive(true);
	}
}

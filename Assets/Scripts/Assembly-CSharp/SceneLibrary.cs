using System;
using App;
using UnityEngine;
using UnityEngine.UI;

public class SceneLibrary : SceneBase
{
	public new void OnClick(GameObject clickObject)
	{
		Debug.Log("OnClick " + clickObject.name);
		switch (clickObject.name)
		{
		case "BookLibraryButton":
			base.transform.Find("BookCanvas").gameObject.SetActive(true);
			StartCoroutine(AppUtil.FadeIn(base.transform.Find("BookCanvas"), 0.25f));
			SetButtonList();
			SceneCommon.BackButtonList.Add(base.transform.Find("BookCanvas/BackButton").GetComponent<OnClickHandler>());
			return;
		case "CounselingLibraryButton":
			base.transform.Find("CounselingCanvas").gameObject.SetActive(true);
			StartCoroutine(AppUtil.FadeIn(base.transform.Find("CounselingCanvas"), 0.25f));
			SetCounselingList();
			SceneCommon.BackButtonList.Add(base.transform.Find("CounselingCanvas/BackButton").GetComponent<OnClickHandler>());
			return;
		case "BackButton":
			OnClickBackKey(clickObject);
			SceneCommon.BackButtonList.Remove(clickObject.GetComponent<OnClickHandler>());
			return;
		case "ResultShareButton":
			return;
		case "ShareButton":
		{
			if (!base.transform.Find("CounselingCanvas").gameObject.activeSelf)
			{
				break;
			}
			RectTransform mirrorFrame = base.transform.Find("CounselingCanvas/Frame").GetComponent<RectTransform>();
			Action<bool> SetUIActive = delegate(bool active)
			{
				clickObject.GetComponent<Button>().interactable = active;
				base.transform.Find("UICanvas").gameObject.SetActive(active);
				base.transform.Find("CounselingCanvas/BackButton").gameObject.SetActive(active);
				Transform transform = GameObject.Find("AppCommon").transform.Find("AdCanvas(Clone)");
				if (transform != null)
				{
					transform.gameObject.SetActive(active);
				}
				if (active)
				{
					mirrorFrame.localScale = Vector2.one;
				}
				else
				{
					float x = mirrorFrame.sizeDelta.x;
					if (x > (float)Screen.width)
					{
						mirrorFrame.localScale = Vector2.one * ((float)Screen.width / x);
					}
				}
			};
			SetUIActive(false);
			StartCoroutine(AppUtil.Share(true, "自分探しタップゲーム『ALTER EGO』 caracolu.com/app/alterego/ #ALTEREGO", null, mirrorFrame));
			StartCoroutine(AppUtil.DelayAction(0.25f, delegate
			{
				SetUIActive(true);
			}));
			return;
		}
		}
		if (clickObject.name.Contains("Book ("))
		{
			int num = int.Parse(clickObject.name.Replace("Book (", "").Replace(")", ""));
			DialogManager.ShowDialog("BookDialog", num.ToString(), 2);
		}
		else if (clickObject.name.Contains("Result"))
		{
			int no = int.Parse(clickObject.name.Replace("Result", ""));
			base.transform.Find("ResultCanvas").GetComponent<ResultCanvas>().RequestResult(CounselingResult.GetTitle(no), CounselingResult.Get(no), null);
		}
		else
		{
			base.OnClick(clickObject);
		}
	}

	private void OnClickBackKey(GameObject backButton = null)
	{
		if (backButton == null)
		{
			backButton = GameObject.Find("BackButton");
		}
		if (!DialogManager.IsShowing() && !(backButton == null))
		{
			StartCoroutine(AppUtil.FadeOut(backButton.transform.parent, 0.25f, delegate
			{
				backButton.transform.parent.gameObject.SetActive(false);
			}));
		}
	}

	private void SetButtonList()
	{
		for (int i = 1; i <= BookLevel.Max; i++)
		{
			base.transform.Find("BookCanvas/List/BookList/Book (" + i + ")").gameObject.SetActive(BookLevel.Get(i.ToString()) >= 0);
		}
	}

	private void SetCounselingList()
	{
		Transform transform = base.transform.Find("CounselingCanvas/Frame/");
		for (int i = 1; i <= CounselingResult.Max; i++)
		{
			bool flag = CounselingResult.Get(i) != "";
			transform.Find("Result" + i).gameObject.SetActive(flag);
			base.transform.Find("CounselingCanvas/Frame/ShadowGroup/Shadow" + i).gameObject.SetActive(flag);
			if (!flag)
			{
				continue;
			}
			string text = LanguageManager.Get(Data.GetScenarioKey(CounselingResult.GetTitle(i), "型", CounselingResult.Get(i)));
			switch (i)
			{
			case 1:
			case 6:
			{
				int[] array = new int[3] { 2, 4, 5 };
				foreach (int num in array)
				{
					if (transform.Find("Result" + i + "/Text" + num) != null)
					{
						transform.Find("Result" + i + "/Text" + num).gameObject.SetActive(false);
					}
				}
				transform.Find("Result" + i + "/Text" + text.Length).gameObject.SetActive(true);
				for (int l = 1; l <= text.Length; l++)
				{
					base.transform.Find("CounselingCanvas/Frame/Result" + i + "/Text" + text.Length + "/" + l).GetComponent<Text>().text = text[l - 1].ToString();
				}
				break;
			}
			case 2:
				transform.Find("Result" + i + "/Text1").GetComponent<Text>().text = text.Substring(0, 5);
				transform.Find("Result" + i + "/Text2").GetComponent<Text>().text = text.Substring(5, text.Length - 5);
				break;
			case 3:
			{
				for (int j = 1; j <= text.Length; j++)
				{
					transform.Find("Result" + i + "/Text" + j).GetComponent<Text>().text = text[j - 1].ToString();
				}
				break;
			}
			case 4:
				transform.Find("Result" + i + "/Text").GetComponent<Text>().text = text;
				break;
			case 5:
				text = text.Insert(4, "\n");
				transform.Find("Result" + i + "/Text").GetComponent<Text>().text = text;
				break;
			case 7:
				foreach (Transform item in transform.Find("Result" + i))
				{
					item.gameObject.SetActive(item.name == "Fragment" || item.name == CounselingResult.Get(i));
				}
				break;
			}
		}
	}
}

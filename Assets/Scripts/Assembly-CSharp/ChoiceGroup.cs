using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceGroup : MonoBehaviour
{
	public GameObject[] ButtonList;

	private void OnEnable()
	{
		GameObject[] buttonList = ButtonList;
		foreach (GameObject obj in buttonList)
		{
			obj.SetActive(false);
			obj.GetComponent<Button>().enabled = false;
		}
		base.transform.Find("ProgressBarSet").gameObject.SetActive(false);
	}

	public void SelectItem(GameObject selectObject)
	{
		if (Array.IndexOf(ButtonList, selectObject) < 0)
		{
			return;
		}
		GameObject[] buttonList = ButtonList;
		foreach (GameObject gameObject in buttonList)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(gameObject == selectObject);
			}
			gameObject.GetComponent<Button>().enabled = false;
		}
	}

	public IEnumerator SetButton(string[] nameList, string[] textKeyList, bool isRandom = false)
	{
		GameObject[] buttonList;
		if (nameList == null)
		{
			buttonList = ButtonList;
			foreach (GameObject obj in buttonList)
			{
				obj.SetActive(false);
				obj.GetComponent<Button>().enabled = false;
			}
			yield break;
		}
		nameList = AppUtil.GetChildArray(nameList, 0);
		textKeyList = AppUtil.GetChildArray(textKeyList, 0);
		int[] array = Enumerable.Range(0, textKeyList.Length).ToArray();
		if (isRandom)
		{
			array = AppUtil.RandomArray(textKeyList.Length);
		}
		for (int j = 0; j < textKeyList.Length; j++)
		{
			ButtonList[j].name = nameList[array[j]];
			if (textKeyList != null)
			{
				string text = LanguageManager.Get(textKeyList[array[j]]);
				ButtonList[j].GetComponentInChildren<TextLocalization>(true).SetText(text);
			}
		}
		ButtonList[0].SetActive(true);
		float anchorY1 = ButtonList[0].GetComponent<RectTransform>().anchoredPosition.y;
		StartCoroutine(AppUtil.MoveEasingFloat(Screen.width, 0f, delegate(float tmp)
		{
			ButtonList[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp, anchorY1);
		}, false, 0.2f, EasingFunction.Ease.EaseOutCubic));
		yield return AppUtil.Wait(0.1f);
		if (textKeyList.Length >= 2 && !string.IsNullOrEmpty(textKeyList[1]))
		{
			ButtonList[1].SetActive(true);
			float anchorY2 = ButtonList[1].GetComponent<RectTransform>().anchoredPosition.y;
			StartCoroutine(AppUtil.MoveEasingFloat(Screen.width, 0f, delegate(float tmp)
			{
				ButtonList[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp, anchorY2);
			}, false, 0.2f, EasingFunction.Ease.EaseOutCubic));
			yield return AppUtil.Wait(0.1f);
		}
		if (textKeyList.Length >= 3 && !string.IsNullOrEmpty(textKeyList[2]))
		{
			ButtonList[2].SetActive(true);
			float anchorY3 = ButtonList[2].GetComponent<RectTransform>().anchoredPosition.y;
			StartCoroutine(AppUtil.MoveEasingFloat(Screen.width, 0f, delegate(float tmp)
			{
				ButtonList[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(tmp, anchorY3);
			}, false, 0.2f, EasingFunction.Ease.EaseOutCubic));
			yield return AppUtil.Wait(0.1f);
		}
		yield return AppUtil.Wait(0.1f);
		buttonList = ButtonList;
		for (int i = 0; i < buttonList.Length; i++)
		{
			buttonList[i].GetComponent<Button>().enabled = true;
		}
	}

	public void DismissButton()
	{
		GameObject[] buttonList = ButtonList;
		for (int i = 0; i < buttonList.Length; i++)
		{
			buttonList[i].SetActive(false);
		}
	}
}

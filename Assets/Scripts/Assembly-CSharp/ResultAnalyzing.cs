using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnalyzing : MonoBehaviour
{
	private Sprite DefaultEs;

	private Text AnalyzingText;

	private IEnumerator WordDisplaying;

	private void Awake()
	{
		AnalyzingText = base.transform.Find("AnalyzingTextGroup/BaseText").GetComponent<Text>();
		ArrangeText(0);
	}

	public void StartAnalyzing(string[] wordList)
	{
		base.transform.Find("AnalyzingTextGroup").gameObject.SetActive(true);
		AppUtil.SetAlpha(base.transform.Find("AnalyzingTextGroup"), 1f);
		base.transform.Find("AnalyzeComplete").gameObject.SetActive(false);
		base.transform.Find("ShowResultButton").gameObject.SetActive(false);
		GetComponent<Animator>().enabled = true;
		DefaultEs = base.transform.Find("Es").GetComponent<Image>().sprite;
		base.transform.Find("Es").GetComponent<Animator>().enabled = true;
		WordDisplaying = DisplayWord(wordList);
		StartCoroutine(WordDisplaying);
	}

	public IEnumerator StopAnalyzing()
	{
		StopCoroutine(WordDisplaying);
		GetComponent<Animator>().enabled = false;
		base.transform.Find("Es").GetComponent<Animator>().enabled = false;
		yield return AppUtil.Wait(1f);
		yield return AppUtil.FadeOut(base.transform.Find("AnalyzingTextGroup"), 0.25f);
		base.transform.Find("AnalyzeComplete").gameObject.SetActive(true);
		base.transform.Find("Es").GetComponent<Image>().sprite = DefaultEs;
		yield return AppUtil.Wait(0.5f);
		base.transform.Find("ShowResultButton").gameObject.SetActive(true);
	}

	public void UpdateAnimation(int timing)
	{
		ArrangeText(timing);
	}

	private void ArrangeText(int timing)
	{
		Transform parent = AnalyzingText.transform.parent;
		if (parent.Find("MessageGroup") != null)
		{
			UnityEngine.Object.Destroy(parent.Find("MessageGroup").gameObject);
		}
		GameObject gameObject = new GameObject("MessageGroup");
		gameObject.AddComponent<RectTransform>();
		gameObject.transform.SetParent(parent, false);
		GameObject gameObject2 = AnalyzingText.gameObject;
		gameObject2.SetActive(true);
		string text = gameObject2.GetComponent<Text>().text;
		for (int i = 0; i < text.Length; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(gameObject2, gameObject.transform);
			obj.GetComponent<Text>().text = text[i].ToString();
			obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(280 + (text.Length - i + 1) * -100, 0f);
			obj.GetComponent<RectTransform>().sizeDelta = Vector2.one * 100f;
			float num = (((i + timing) % 2 != 0) ? UnityEngine.Random.Range(0.9f, 1.2f) : UnityEngine.Random.Range(0.7f, 1f));
			obj.transform.localScale = Vector3.one * num;
			obj.transform.Rotate(new Vector3(0f, 0f, UnityEngine.Random.Range(-15, 15)));
		}
		gameObject2.SetActive(false);
	}

	private IEnumerator DisplayWord(string[] wordList)
	{
		if (base.transform.Find("WordGroup") != null)
		{
			UnityEngine.Object.Destroy(base.transform.Find("WordGroup").gameObject);
		}
		yield return AppUtil.Wait(1f);
		Rect[] area = new Rect[2]
		{
			new Rect(-337f, 126f, 674f, 242f),
			new Rect(-337f, -376f, 674f, 300f)
		};
		GameObject baseObject = base.transform.Find("Word").gameObject;
		GameObject group = new GameObject("WordGroup");
		group.AddComponent<RectTransform>();
		group.transform.SetParent(base.transform, false);
		for (int i = 1; i <= 2; i++)
		{
			string[] array = wordList.OrderBy((string tmp) => Guid.NewGuid()).ToArray();
			string[] array2 = array;
			foreach (string text in array2)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(baseObject, group.transform);
				gameObject.GetComponent<Text>().text = text;
				gameObject.SetActive(true);
				float num = UnityEngine.Random.Range(0.5f, 1.2f);
				if (UnityEngine.Random.Range(0, 10) <= 6)
				{
					gameObject.transform.localScale = Vector3.one * num;
				}
				else
				{
					gameObject.transform.localScale = new Vector3(0f - num, num, num);
				}
				Rect rect = area[UnityEngine.Random.Range(0, area.Length)];
				float x = rect.x + UnityEngine.Random.Range(0f, rect.width);
				float y = rect.y + UnityEngine.Random.Range(0f, rect.height);
				gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
				StartCoroutine(AppUtil.FadeIn(gameObject));
				AppUtil.DelayAction(this, UnityEngine.Random.Range(0.5f, 1.5f), AppUtil.FadeOut(gameObject));
				yield return AppUtil.Wait(UnityEngine.Random.Range(0.2f, 0.55f));
			}
		}
	}
}

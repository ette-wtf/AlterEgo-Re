using System.Linq;
using App;
using UnityEngine;
using UnityEngine.UI;

public class CommentUI : MonoBehaviour
{
	public enum TYPE
	{
		NONE = 0,
		ID = 1,
		SE = 2
	}

	public TYPE Type;

	private void OnEnable()
	{
		if (Type == TYPE.ID || PlayerStatus.ScenarioNo.Contains("4章ID"))
		{
			ArrangeText();
		}
	}

	public void ArrangeText()
	{
		if (base.transform.Find("MessageGroup") != null)
		{
			Object.Destroy(base.transform.Find("MessageGroup").gameObject);
		}
		GameObject gameObject = new GameObject("MessageGroup");
		gameObject.transform.parent = base.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
		rectTransform.anchorMin = Vector2.one * 0.5f;
		rectTransform.anchorMax = Vector2.one * 0.5f;
		rectTransform.sizeDelta = Vector2.zero;
		GameObject gameObject2 = GetComponentInChildren<Text>(true).gameObject;
		gameObject2.GetComponent<Text>().alignment = TextAnchor.MiddleLeft;
		string text = gameObject2.GetComponent<Text>().text;
		gameObject2.SetActive(true);
		Object.Destroy(gameObject2.GetComponent<CenterLeftTextEgo>());
		Vector3 zero = Vector3.zero;
		Vector3 vector = zero;
		float b = 0f;
		float y = 30f;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (c == '\n')
			{
				b = vector.x;
				y = 60f;
				vector = zero - new Vector3(0f, 52.8f, 0f);
				continue;
			}
			GameObject gameObject3 = Object.Instantiate(gameObject2, gameObject.transform);
			RectTransform component = gameObject3.GetComponent<RectTransform>();
			component.anchorMin = Vector2.one * 0.5f;
			component.anchorMax = Vector2.one * 0.5f;
			component.sizeDelta = Vector2.one * 100f;
			component.anchoredPosition = vector;
			component.anchoredPosition += new Vector2(0f, Random.Range(-5f, 5f));
			gameObject3.transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
			gameObject3.transform.Rotate(new Vector3(0f, 0f, Random.Range(-10, 10)));
			string font = LanguageManager.FONTDATA_TABLE.Keys.ToArray()[Random.Range(0, LanguageManager.FONTDATA_TABLE.Keys.Count)];
			if (Random.Range(0f, 1f) >= 0.5f)
			{
				font = "kokumin";
			}
			float num = 42f;
			if (!IsKanji(c))
			{
				num *= 0.75f;
			}
			if (vector == zero)
			{
				font = "kokumin";
				gameObject3.GetComponent<Text>().fontSize = (int)((float)gameObject3.GetComponent<Text>().fontSize * 1.3f);
				vector += new Vector3(num * 1.3f, 0f, 0f);
			}
			else
			{
				vector += new Vector3(num, 0f, 0f);
			}
			gameObject3.GetComponent<TextLocalization>().SetText(c.ToString());
			gameObject3.GetComponent<TextLocalization>().SetFont(font);
		}
		b = Mathf.Max(vector.x, b);
		rectTransform.anchoredPosition = new Vector2((0f - b) / 3f, y);
		gameObject2.SetActive(false);
	}

	public static bool IsKanji(char c)
	{
		if (('一' > c || c > '鿏') && ('豈' > c || c > '\ufaff'))
		{
			if ('㐀' <= c)
			{
				return c <= '䶿';
			}
			return false;
		}
		return true;
	}
}

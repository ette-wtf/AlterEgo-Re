using System.Collections;
using System.Linq;
using App;
using UnityEngine;

public class Comment : MonoBehaviour
{
	public string CommentKey;

	public string Type;

	public bool IsHyperActive;

	private SceneBook gameManager;

	private GameObject NewEffect;

	private void Awake()
	{
		gameManager = GetComponentInParent<SceneBook>();
	}

	private void Start()
	{
		GetComponent<Collider2D>().enabled = false;
		StartCoroutine(AppUtil.FadeIn(base.gameObject));
		if (!(Type == "Butterfly"))
		{
			string text = LanguageManager.Get(CommentKey);
			if (Type != "ID")
			{
				text = "<size=" + (float)GetComponentInChildren<TextMesh>().fontSize * 1.3f + ">" + text.Substring(0, 1) + "</size>" + text.Substring(1, text.Length - 1);
			}
			TextLocalization[] componentsInChildren = GetComponentsInChildren<TextLocalization>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetText(text);
			}
			base.transform.Find("Filter").gameObject.SetActive(true);
			if (!ReadFukidasiList.Contains(CommentKey))
			{
				NewEffect = base.transform.Find("NewEffect").gameObject;
			}
			if (Type == "ID")
			{
				InvokeRepeating("ArrangeText", 0f, 0.84f);
			}
		}
	}

	public bool UpdateStatus(bool isFirst, bool noWalking)
	{
		int num = (int)base.transform.position.z;
		int num2 = (int)((0f - base.transform.position.z) * 10f) + 100;
		GetComponent<SpriteRenderer>().sortingOrder = num2;
		MeshRenderer[] componentsInChildren = GetComponentsInChildren<MeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingOrder = num2 + 2;
		}
		if (Type == "SE")
		{
			base.transform.Find("MessageTextShadow").GetComponent<MeshRenderer>().sortingOrder = num2 + 1;
		}
		base.transform.Find("Filter").GetComponent<SpriteRenderer>().sortingOrder = num2 + 3;
		GetComponent<SpriteMask>().frontSortingOrder = num2 + 5;
		GetComponent<SpriteMask>().backSortingOrder = num2;
		if (noWalking && (float)num <= -2f)
		{
			StartCoroutine("DestroyAfterFadeOut");
			return false;
		}
		if (!isFirst)
		{
			return true;
		}
		if (num < 7)
		{
			GetComponent<Collider2D>().enabled = true;
			base.transform.Find("Filter").gameObject.SetActive(false);
			gameManager.SetTutorial(2);
			if (NewEffect != null)
			{
				bool active = !ReadFukidasiList.Contains(CommentKey);
				NewEffect.SetActive(active);
				NewEffect.GetComponent<SpriteRenderer>().sortingOrder = num2 + 4;
			}
		}
		if (noWalking && (float)num <= 1f && Type != "Butterfly")
		{
			base.gameObject.name = "CloseComment" + Type;
		}
		return true;
	}

	private void OnMouseUpAsButton()
	{
		gameManager.OnClick(base.gameObject);
		GetComponent<Collider2D>().enabled = false;
		AudioManager.PlaySound("Click", "", base.gameObject.name);
		StopCoroutine("DestroyAfterFadeOut");
		base.transform.SetParent(base.transform.parent.parent);
		if (Type != "Butterfly")
		{
			StartCoroutine("DestroyAfterCrash");
		}
		else
		{
			StartCoroutine("DestroyAfterFadeOut");
		}
	}

	private IEnumerator DestroyAfterCrash()
	{
		GetComponent<SpriteMask>().enabled = false;
		GetComponent<Animator>().enabled = true;
		base.transform.localScale = Vector3.one * 0.975f;
		if (Type == "ID")
		{
			StartCoroutine(AppUtil.MoveEasingFloat(1f, 0f, delegate(float value)
			{
				AppUtil.SetAlphaChildren(base.transform.Find("MessageGroup").gameObject, value);
			}, true, 0.3f, EasingFunction.Ease.EaseInSine));
			StartCoroutine(AppUtil.ShakeObject(base.gameObject));
		}
		yield return AppUtil.WaitAnimation(base.gameObject);
		Object.Destroy(base.gameObject);
	}

	public IEnumerator DestroyAfterFadeOut()
	{
		GetComponent<SpriteMask>().enabled = false;
		for (float f = 1f; f >= 0f; f -= 0.1f)
		{
			if (f <= 0.3f)
			{
				GetComponent<Collider2D>().enabled = false;
			}
			AppUtil.SetAlphaChildren(base.gameObject, f);
			yield return AppUtil.Wait(0.01f);
		}
		Object.Destroy(base.gameObject);
	}

	public void ArrangeText()
	{
		if (base.transform.Find("MessageGroup") != null)
		{
			Object.Destroy(base.transform.Find("MessageGroup").gameObject);
		}
		GameObject gameObject = new GameObject("MessageGroup");
		gameObject.transform.parent = base.transform;
		GameObject gameObject2 = base.transform.Find("MessageText").gameObject;
		string text = gameObject2.GetComponent<TextMesh>().text;
		gameObject2.GetComponent<MeshRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
		gameObject2.SetActive(true);
		Vector3 zero = Vector3.zero;
		Vector3 vector = zero;
		float b = 0f;
		float y = 0.1f;
		char[] array = text.ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			char c = array[i];
			if (c == '\n')
			{
				b = vector.x;
				y = 0.35f;
				vector = zero - new Vector3(0f, 0.57199997f, 0f);
				continue;
			}
			GameObject gameObject3 = Object.Instantiate(gameObject2, gameObject.transform);
			gameObject3.transform.localPosition = vector;
			gameObject3.transform.localPosition += new Vector3(0f, Random.Range(-0.05f, 0.05f), 0f);
			gameObject3.transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
			gameObject3.transform.Rotate(new Vector3(0f, 0f, Random.Range(-10, 10)));
			string font = LanguageManager.FONTDATA_TABLE.Keys.ToArray()[Random.Range(0, LanguageManager.FONTDATA_TABLE.Keys.Count)];
			if (Random.Range(0f, 1f) >= 0.5f)
			{
				font = "kokumin";
			}
			float num = 0.42f;
			if (!IsKanji(c))
			{
				num *= 0.75f;
			}
			if (vector == zero)
			{
				font = "kokumin";
				gameObject3.GetComponent<TextMesh>().fontSize = (int)((float)gameObject3.GetComponent<TextMesh>().fontSize * 1.3f);
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
		gameObject.transform.localPosition = new Vector3((0f - b) / 2f, y, 0f);
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

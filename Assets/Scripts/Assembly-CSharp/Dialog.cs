using System.Collections;
using SocialConnector;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
	protected object[] args;

	protected virtual void Awake()
	{
		if (GetComponent<CanvasGroup>() == null)
		{
			base.gameObject.AddComponent<CanvasGroup>();
		}
		AppUtil.SetAlpha(base.gameObject, 0f);
	}

	protected virtual bool OnClick(GameObject clickObject)
	{
		Debug.Log("Dialog:OnClick " + clickObject.name);
		switch (clickObject.name)
		{
		case "CloseButton":
		case "BackButton":
		case "CancelButton":
			Close();
			return true;
		case "CloseAllButton":
			DialogManager.CloseAllDialog();
			break;
		case "ShareButton":
			global::SocialConnector.SocialConnector.Share((string)args[0], (string)args[1], (string)args[3]);
			Close();
			return true;
		}
		return false;
	}

	protected virtual void Close()
	{
		DialogManager.CloseDialog(base.gameObject.name);
	}

	protected virtual void OnEnable()
	{
		base.transform.SetAsLastSibling();
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		Render();
		OnClickHandler[] componentsInChildren = GetComponentsInChildren<OnClickHandler>(true);
		foreach (OnClickHandler onClickHandler in componentsInChildren)
		{
			switch (onClickHandler.name)
			{
			case "CloseButton":
			case "BackButton":
			case "CancelButton":
			case "CloseAllButton":
				SceneCommon.BackButtonList.Add(onClickHandler);
				break;
			}
		}
	}

	protected virtual void Render()
	{
		StartCoroutine(AppUtil.FadeIn(base.gameObject, 0.25f, delegate
		{
			Debug.Log("AutoTestEvent:スクリーンショット");
		}));
	}

	protected virtual void OnDisable()
	{
		OnClickHandler[] componentsInChildren = GetComponentsInChildren<OnClickHandler>(true);
		foreach (OnClickHandler onClickHandler in componentsInChildren)
		{
			switch (onClickHandler.name)
			{
			case "CloseButton":
			case "BackButton":
			case "CancelButton":
			case "CloseAllButton":
				SceneCommon.BackButtonList.Remove(onClickHandler);
				break;
			}
		}
	}

	public virtual void OnShown(params object[] args)
	{
		if (args != null && args.Length != 0)
		{
			this.args = args;
		}
		bool activeSelf = base.gameObject.activeSelf;
		base.gameObject.SetActive(true);
		if (activeSelf)
		{
			return;
		}
		string text = base.gameObject.name;
		if (!(text == "FakeMovieAdScreen"))
		{
			if (text == "ShareDialog")
			{
				Texture2D texture2D = (Texture2D)this.args[2];
				Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), Vector2.one * 0.5f);
				base.transform.Find("ShareImage").GetComponent<Image>().sprite = sprite;
			}
		}
		else
		{
			base.transform.Find("CloseButton").gameObject.SetActive(false);
			base.transform.Find("CountDownText").GetComponent<Text>().text = "10";
			StartCoroutine("FakeMovie");
		}
	}

	public virtual IEnumerator OnClosed()
	{
		GetComponent<CanvasGroup>().blocksRaycasts = false;
		yield return AppUtil.FadeOut(base.gameObject, 0.25f);
		base.gameObject.SetActive(false);
	}

	private IEnumerator FakeMovie()
	{
		int unit = 10;
		Transform text = base.transform.Find("CountDownText");
		for (int i = 9; i >= 0; i--)
		{
			yield return AppUtil.WaitRealtime(1f - (float)(90 / unit * 2) * 0.01f);
			for (int angle2 = 0; angle2 < 90; angle2 += unit)
			{
				yield return AppUtil.WaitRealtime(0.01f);
				text.Rotate(new Vector3(0f, unit, 0f));
			}
			text.GetComponent<Text>().text = i.ToString();
			text.Rotate(new Vector3(0f, 180f, 0f));
			for (int angle2 = 0; angle2 < 90; angle2 += unit)
			{
				yield return AppUtil.WaitRealtime(0.01f);
				text.Rotate(new Vector3(0f, unit, 0f));
			}
			text.localRotation = Quaternion.identity;
		}
		base.transform.Find("CloseButton").gameObject.SetActive(true);
	}
}
